import { Translation } from "./translation";

export abstract class English {
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
            value: "Language"
        },
        {
            key: "Username",
            value: "Username"
        },
        {
            key: "Password",
            value: "Password"
        },
        {
            key: "Login",
            value: "Login"
        },
        {
            key: "Register",
            value: "Register"
        },
    ];
}