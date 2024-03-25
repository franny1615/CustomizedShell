import { Component } from "@angular/core";
import { Theming } from "../utils/theming";

@Component({
    selector: 'theme-toggler',
    templateUrl: './theme-toggler.component.html',
    styleUrls: ['./theme-toggler.component.sass']
})
export class ThemeToggler {
    ngOnInit(): void {
        this.ApplyStyle();
    }

    private ApplyStyle(): void {
        var img = document.getElementById('themeTogglerImg');
        var current = Theming.Current();
        if (current == Theming.LIGHT_MODE) {
            img?.setAttribute('src', 'assets/images/dark-mode.svg');
        } else if(current == Theming.DARK_MODE) {
            img?.setAttribute('src', 'assets/images/light-mode.svg');
        }
    }

    ToggleTheme() : void {
        var current = Theming.Current();
        if (current == Theming.LIGHT_MODE) {
            Theming.DarkMode();
        } else if (current == Theming.DARK_MODE) {
            Theming.LightMode();
        }
        this.ApplyStyle();
    }
}