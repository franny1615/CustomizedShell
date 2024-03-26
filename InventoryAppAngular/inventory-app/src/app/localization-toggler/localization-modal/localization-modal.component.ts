import { Component } from "@angular/core";
import { Locale, Localization } from "../../utils/localization";

@Component({
    selector: 'app-localization-modal',
    templateUrl: './localization-modal.component.html',
    styleUrls: ['./localization-modal.component.sass']
})
export class LocalizationModal {
    public localizationInstance = Localization;

    ngOnInit(): void {
        this.localizationInstance.ApplyCurrent();
    }

    show() {
        document.getElementById('showLocalizationBtn')?.click();
    }

    checkActiveAgainst(locale: Locale): boolean {
        return this.localizationInstance.Current().abbreviation == locale.abbreviation;
    }

    changeLocaleTo(locale: Locale) {
        this.localizationInstance.UpdateTo(locale);
        // document.getElementById('hideLocalizationBtn')?.click();
    }
}