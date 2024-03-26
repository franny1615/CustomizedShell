import { Translation } from "./translation";

export abstract class Spanish {
    static Find(key: string): string {
        for (var i = 0; i < this.translations.length; i++) {
            if (this.translations[i].key == key) {
                return this.translations[i].value;
            }
        }
        return key;
    }

    public static translations: Translation[] = [
        {
            key: "Language",
            value: "Idioma"
        },
        {
            key: "Username",
            value: "Usuario"
        },
        {
            key: "Password",
            value: "Contraseña"
        },
        {
            key: "Login",
            value: "Ingresar"
        },
        {
            key: "Register",
            value: "Registrar"
        },
    ];
}