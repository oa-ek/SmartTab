# Лабораторна робота №3: API та віддалені ресурси даних

## Сценарій інтеграції
**Назва:** Збагачена картка товару + онлайн-оплата  
**Проблема користувача:** Користувач хоче бачити ціну товару в різних валютах (включаючи валюту країни виробника), інформацію про країну виробника, тематичне зображення категорії — і оплатити замовлення онлайн через Monobank.

**Як працює:**
1. Користувач відкриває картку товару зі SmartTab.
2. Застосунок бере локальні дані про товар з БД (назва, ціна в UAH, виробник, категорія).
3. Запит до **NBU API** — отримує актуальний курс USD та EUR → показує ціну в трьох валютах.
4. Запит до **REST Countries API** — отримує дані про країну виробника (прапор, столиця, мови, валюта).
5. **Ланцюжок: REST Countries → ExchangeRate API → NBU** — отримує код валюти країни виробника → конвертує ціну через USD як посередника (якщо НБУ не має прямого курсу).
6. Запит до **Pexels API** — отримує тематичне зображення для категорії товару.
7. При оформленні замовлення — запит до **Monobank Acquiring API** для створення рахунку та оплати.
8. Webhook або polling перевіряє статус оплати та оновлює замовлення.

## Використані зовнішні API

| API | Endpoint | Отримані дані | Кешування | API Key |
|---|---|---|---|---|
| NBU API | `GET /NBUStatService/v1/statdirectory/exchange?json` | Курс USD, EUR до UAH | 30 хв | Ні |
| REST Countries | `GET /v3.1/name/{countryName}?fields=...` | Назва, прапор, столиця, валюта, мови | 24 год | Ні |
| ExchangeRate API | `GET /v6/latest/USD` | Крос-курси валют (fallback) | 30 хв | Ні |
| Pexels | `GET /v1/search?query={category}&per_page=1` | Зображення категорії | 1 год | Так |
| Monobank Acquiring | `POST /api/merchant/invoice/create` | InvoiceId, PageUrl для оплати | Ні | Так |
| Monobank Acquiring | `GET /api/merchant/invoice/status?invoiceId=...` | Статус оплати | Ні | Так |

### Деталі API

**1. NBU API (Національний банк України)**
- Документація: https://bank.gov.ua/ua/open-data/api-dev
- API Key: не потрібен
- Використовується для: конвертації ціни товару з UAH в USD та EUR

**2. REST Countries API**
- Документація: https://restcountries.com/
- API Key: не потрібен
- Використовується для: отримання інформації про країну виробника (прапор, столиця, мови, валюта)

**3. ExchangeRate API (open.er-api.com)**
- Документація: https://www.exchangerate-api.com/docs/free
- API Key: не потрібен
- Використовується для: fallback конвертації рідкісних валют (TWD, KRW тощо) через USD

**4. Pexels API**
- Документація: https://www.pexels.com/api/documentation/
- API Key: потрібен (зберігається в User Secrets)
- Використовується для: отримання тематичного зображення категорії товару

**5. Monobank Acquiring API**
- Документація: https://api.monobank.ua/docs/acquiring.html
- API Key: X-Token (зберігається в User Secrets)
- Використовується для: створення рахунків на оплату та перевірки статусу платежу

## Архітектура

### Сервісний шар
- `ICurrencyApiService` / `CurrencyApiService` — NBU API + ExchangeRate API fallback
- `ICountryApiService` / `CountryApiService` — REST Countries API
- `IPexelsApiService` / `PexelsApiService` — Pexels API
- `IMonobankService` / `MonobankService` — Monobank Acquiring API

### DTO
- `NbuCurrencyDto` — відповідь NBU API
- `RestCountryDto`, `CountryName`, `CountryFlags`, `CurrencyInfo` — відповідь REST Countries
- `PexelsResponseDto`, `PexelsPhoto`, `PexelsPhotoSrc` — відповідь Pexels
- `MonobankInvoiceRequest`, `MonobankInvoiceResponse`, `MonobankWebhookPayload` — Monobank

### ViewModel
- `ProductEnrichedViewModel` — агрегована модель: локальні дані + дані з 4 API

### Polly (Resilience)
- Всі HTTP-клієнти зареєстровані з `.AddStandardResilienceHandler()` — timeout, retry, circuit breaker
- Пакет: `Microsoft.Extensions.Http.Resilience`

### Кешування (`IMemoryCache`)
| Дані | Ключ кешу | Час життя | Причина кешування |
|---|---|---|---|
| Курс валют НБУ | `nbu_exchange_rates` | 30 хв | Рідко змінюється, часто запитується |
| Країна виробника | `country:{name}` | 24 год | Статичні дані |
| Зображення Pexels | `pexels:{query}` | 1 год | Rate limit API |
| Fallback курс валюти | `fallback_rate_{CODE}` | 30 хв | Рідкісні валюти через ExchangeRate API |

## Fallback-стратегії
- **NBU API недоступний** → блок "Ціна в інших валютах" не показується
- **REST Countries недоступний** → блок "Країна виробника" не показується
- **NBU не має курсу валюти виробника (TWD, KRW тощо)** → fallback через ExchangeRate API (конвертація USD→валюта)
- **Pexels недоступний** → блок із зображенням категорії не показується
- **Monobank webhook не дійшов** → клієнтський polling кожні 3 сек перевіряє статус через `POST /api/orders/{id}/check-payment`
- Якщо деякі API недоступні — показується повідомлення з переліком недоступних сервісів
- Якщо всі API недоступні — секція "Додаткова інформація" прихована повністю

## Власний Web API
- **Controller:** `HomeController`
- **Swagger:** `/swagger`
- **Основні endpoints:**

| Метод | Endpoint | Опис |
|---|---|---|
| GET | `/api/products` | Всі товари |
| GET | `/api/products/{id}` | Товар за ID |
| GET | `/api/products/{id}/enriched` | Збагачена картка (3+ API) |
| POST | `/api/products` | Створити товар |
| PUT | `/api/products/{id}` | Оновити товар |
| DELETE | `/api/products/{id}` | Видалити товар |
| POST | `/api/orders` | Створити замовлення + Monobank інвойс |
| GET | `/api/orders/{id}` | Деталі замовлення |
| GET | `/api/orders/my` | Замовлення поточного користувача |
| POST | `/api/orders/{id}/check-payment` | Перевірити статус оплати в Monobank |
| POST | `/api/monobank/webhook` | Webhook від Monobank |
| GET | `/api/categories` | Всі категорії |
| GET | `/api/manufacturers` | Всі виробники |
| GET | `/api/users` | Всі користувачі (Admin) |

## Безпека API ключів
- API ключі зберігаються в **User Secrets** (поза проєктом, не потрапляють у Git)
- Зберігаються: `Monobank:Token`, `SmtpSettings:Email`, `SmtpSettings:Password`
- `appsettings.Development.json` у `.gitignore`
- NBU API, REST Countries, ExchangeRate API не потребують ключів

## Як запустити
1. Клонувати репозиторій
2. Налаштувати User Secrets:
   ```bash
   cd Solution1/SmartTab.UI
   dotnet user-secrets set "Monobank:Token" "YOUR_MONOBANK_TOKEN"
   dotnet user-secrets set "ApiKeys:Pexels" "YOUR_PEXELS_KEY"
   ```
3. Застосувати міграції:
   ```bash
   dotnet ef database update --project SmartTab.Data --startup-project SmartTab.UI
   ```
4. Запустити:
   ```bash
   dotnet run --project SmartTab.UI
   ```
5. Відкрити: `https://localhost:7104`
6. Swagger: `https://localhost:7104/swagger`
