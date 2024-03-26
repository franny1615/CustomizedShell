import { English } from "./languages/en";
import { Spanish } from "./languages/es";

export class Locale {
    description: string
    abbreviation: string

    constructor(desciption: string, abbreviation: string) {
        this.description = desciption;
        this.abbreviation = abbreviation;
    }
}

export abstract class Localization {
    public static currentLocalizationKey = 'kCurrLocale';
    public static SUPPORTED_LOCALES: Locale[] = [
        {
            description: 'English',
            abbreviation: 'en'
        },
        {
            description: 'Español',
            abbreviation: 'es'
        }
    ];

    static Current(): Locale {
        var current = localStorage.getItem(this.currentLocalizationKey);
        if (current && current.length > 0) {
            return JSON.parse(current);
        }
        return this.SUPPORTED_LOCALES[0];
    }

    static UpdateTo(locale: Locale) {
        localStorage.setItem(this.currentLocalizationKey, JSON.stringify(locale));
        this.ApplyCurrent();
    }

    static ApplyCurrent(): void {
        var locale = this.Current();
        var translateable = document.querySelectorAll('[translate]');
        translateable.forEach((element) => {
            var key = element.getAttribute('translate') || '';
            switch (locale.abbreviation) {
                case 'es':
                    element.innerHTML = Spanish.Find(key);
                    break;
                case 'en':
                    element.innerHTML = English.Find(key);
                    break;
            }
        });
    }

    static InstantTranslation(key: string): string {
        var locale = this.Current();
        switch (locale.abbreviation) {
            case 'es':
                return Spanish.Find(key);
            case 'en':
                return English.Find(key);
            default:
                return key;
        }
    }
}