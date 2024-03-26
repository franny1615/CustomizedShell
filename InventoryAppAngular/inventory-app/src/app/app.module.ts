import { NgModule } from "@angular/core";
import { AppRoutingModule } from "./app-routing.module";
import { LoginComponent } from "./login/login.component";
import { AppComponent } from "./app.component";
import { BrowserModule } from "@angular/platform-browser";
import { ThemeToggler } from "./theme-toggler/theme-toggler.component";
import { LocalizationToggler } from "./localization-toggler/localization-toggler.component";
import { LocalizationModal } from "./localization-toggler/localization-modal/localization-modal.component";

@NgModule({
    imports: [
        BrowserModule,
        AppRoutingModule
    ],
    declarations: [
        AppComponent,
        LoginComponent,
        ThemeToggler,
        LocalizationToggler,
        LocalizationModal
    ],
    bootstrap: [AppComponent]
})
export class AppModule {}