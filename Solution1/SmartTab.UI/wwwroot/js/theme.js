// ─── Застосовуємо тему ОДРАЗУ (до рендеру сторінки, щоб не було мигання) ───
(function () {
    var saved = localStorage.getItem('smarttab_theme');
    var prefersDark = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
    if (saved === 'dark' || (!saved && prefersDark)) {
        document.documentElement.classList.add('dark');
    }
})();

// ─── Функція перемикання ───
function toggleTheme() {
    var isDark = document.documentElement.classList.toggle('dark');
    localStorage.setItem('smarttab_theme', isDark ? 'dark' : 'light');
    _updateThemeToggle(isDark);
}

// ─── Оновити вигляд тогл-кнопки ───
function _updateThemeToggle(isDark) {
    var track = document.getElementById('theme-track');
    var thumb = document.getElementById('theme-thumb');
    var icon  = document.getElementById('theme-icon');

    if (!track) return;

    if (isDark) {
        track.style.backgroundColor = '#6f00ff';
        thumb.style.transform = 'translateX(24px)';
        if (icon) { icon.classList.remove('fa-moon'); icon.classList.add('fa-sun'); }
    } else {
        track.style.backgroundColor = '#d1d5db';
        thumb.style.transform = 'translateX(0)';
        if (icon) { icon.classList.remove('fa-sun'); icon.classList.add('fa-moon'); }
    }
}

// ─── Після завантаження DOM — синхронізуємо стан кнопки ───
document.addEventListener('DOMContentLoaded', function () {
    var isDark = document.documentElement.classList.contains('dark');
    _updateThemeToggle(isDark);
});
