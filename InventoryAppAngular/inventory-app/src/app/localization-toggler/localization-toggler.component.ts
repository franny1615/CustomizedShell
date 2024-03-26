import { Component, ViewChild } from "@angular/core";
import { LocalizationModal } from "./localization-modal/localization-modal.component";

@Component({
    selector: 'localization-toggler',
    templateUrl: './localization-toggler.component.html',
    styleUrls: ['./localization-toggler.component.sass']
})
export class LocalizationToggler {
    @ViewChild('localeModal') localizationModal!: LocalizationModal

    ToggleLanguage() : void {
        this.localizationModal.show();
    }
}