export abstract class Theming {
    private static themeKey = 'kThemeKey';
    public static LIGHT_MODE = 'light';
    public static DARK_MODE = 'dark';

    public static LightMode(): void {
        document.getElementsByTagName('html')[0].setAttribute('data-bs-theme', this.LIGHT_MODE);
        localStorage.setItem(this.themeKey, this.LIGHT_MODE);
        console.log(`--- ${this.LIGHT_MODE} theme applied ---`);
    }

    public static DarkMode(): void {
        document.getElementsByTagName('html')[0].setAttribute('data-bs-theme', this.DARK_MODE);
        localStorage.setItem(this.themeKey, this.DARK_MODE);
        console.log(`--- ${this.DARK_MODE} theme applied ---`);
    }

    public static Current(): string {
        var theme = localStorage.getItem(this.themeKey);
        if (theme) {
            return theme;
        }
        return this.LIGHT_MODE;
    }

    public static ApplySavedTheme(): void {
        if (this.Current() == this.LIGHT_MODE) {
            this.LightMode();
        } else {
            this.DarkMode();
        }
    }
}