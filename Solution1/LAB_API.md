# Лабораторна робота №3: API та віддалені ресурси даних

## Сценарій інтеграції
**Назва:** Збагачена картка товару  
**Проблема користувача:** Користувач хоче бачити ціну товару в різних валютах, інформацію про країну виробника та тематичне зображення категорії — все на одній сторінці товару.

**Як працює:**
1. Користувач відкриває картку товару зі SmartTab.
2. Застосунок бере локальні дані про товар з БД (назва, ціна в UAH, виробник, категорія).
3. Запит до **NBU API** — отримує актуальний курс USD та EUR → показує ціну в трьох валютах.
4. Запит до **REST Countries API** — отримує дані про країну виробника (прапор, столиця, мови, валюта).
5. Запит до **Pexels API** — отримує тематичне зображення для категорії товару.
6. Всі дані об'єднуються і відображаються у секції "Додаткова інформація" на сторінці товару.

## Використані зовнішні API

| API | Endpoint | Отримані дані | Кешування | API Key |
|---|---|---|---|---|
| NBU API | `GET /NBUStatService/v1/statdirectory/exchange?json` | Курс USD, EUR до UAH | 30 хв (`IMemoryCache`) | Ні |
| REST Countries | `GET /v3.1/name/{countryName}?fields=...` | Назва, прапор, столиця, валюта, мови, населення | 24 год (`IMemoryCache`) | Ні |
| Pexels | `GET /v1/search?query={category}&per_page=1` | Зображення категорії, автор фото | 1 год (`IMemoryCache`) | Так |

### Деталі API

**1. NBU API (Національний банк України)**
- Документація: https://bank.gov.ua/ua/open-data/api-dev
- API Key: не потрібен
- Використовується для: конвертації ціни товару з UAH в USD та EUR

**2. REST Countries API**
- Документація: https://restcountries.com/
- API Key: не потрібен
- Використовується для: отримання інформації про країну виробника (прапор, столиця, мови, валюта)

**3. Pexels API**
- Документація: https://www.pexels.com/api/documentation/
- API Key: потрібен (зберігається в `appsettings.Development.json`)
- Використовується для: отримання тематичного зображення категорії товару

## Архітектура

### Сервісний шар
- `ICurrencyApiService` / `CurrencyApiService` — робота з NBU API
- `ICountryApiService` / `CountryApiService` — робота з REST Countries API
- `IPexelsApiService` / `PexelsApiService` — робота з Pexels API

### DTO
- `NbuCurrencyDto` — відповідь NBU API (`[JsonPropertyName]` для snake_case)
- `RestCountryDto`, `CountryName`, `CountryFlags`, `CurrencyInfo` — відповідь REST Countries
- `PexelsResponseDto`, `PexelsPhoto`, `PexelsPhotoSrc` — відповідь Pexels

### ViewModel
- `ProductEnrichedViewModel` — агрегована модель, що комбінує локальні дані з БД + дані з 3 API

### Polly (Resilience)
- Всі HTTP-клієнти зареєстровані з `.AddStandardResilienceHandler()` — timeout, retry, circuit breaker
- Пакет: `Microsoft.Extensions.Http.Resilience`

### Кешування (`IMemoryCache`)
| Дані | Ключ кешу | Час життя | Причина кешування |
|---|---|---|---|
| Курс валют НБУ | `nbu_exchange_rates` | 30 хв | Рідко змінюється, часто запитується |
| Країна виробника | `country:{name}` | 24 год | Статичні дані |
| Зображення Pexels | `pexels:{query}` | 1 год | Rate limit API, рідко змінюється |

## Fallback-стратегії
- **NBU API недоступний** → блок "Ціна в інших валютах" не показується
- **REST Countries недоступний** → блок "Країна виробника" не показується
- **Pexels недоступний** → блок із зображенням категорії не показується
- Якщо деякі API недоступні, але інші працюють — показується повідомлення з переліком недоступних сервісів
- Якщо всі API недоступні — секція "Додаткова інформація" прихована повністю

## Власний Web API
- **Controller:** `ProductsApiController`
- **Route:** `/api/Products`
- **Swagger:** `/swagger`
- **Endpoints:**

| Метод | Endpoint | Опис |
|---|---|---|
| GET | `/api/Products` | Отримати всі товари (з фільтрацією) |
| GET | `/api/Products/{id}` | Отримати товар за ID |
| POST | `/api/Products` | Створити новий товар |
| PUT | `/api/Products/{id}` | Оновити товар |
| DELETE | `/api/Products/{id}` | Видалити товар |

Всі endpoints мають атрибути `[ProducesResponseType]` для Swagger документації.

## Безпека API ключів
- API ключ Pexels зберігається в `appsettings.Development.json` (у `.gitignore`)
- Читання через `IConfiguration["ApiKeys:Pexels"]`
- NBU API та REST Countries не потребують ключів

## Як запустити
1. Скопіювати `appsettings.Development.json.example` → `appsettings.Development.json`
2. Додати API ключ Pexels: `"ApiKeys": { "Pexels": "YOUR_KEY" }`
3. `dotnet ef database update --project SmartTab.Data --startup-project SmartTab.UI`
4. `dotnet run --project SmartTab.UI`
5. Відкрити сторінку товару → секція "Додаткова інформація"
6. Swagger: `/swagger`
