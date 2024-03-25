import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { LoadingIndicator } from './utils/loadingIndicator';
import { Theming } from './utils/theming';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.sass'
})
export class AppComponent {
  constructor(private router: Router) {
    Theming.ApplySavedTheme();
    this.CheckAuth();
  }

  private CheckAuth(): void {
    LoadingIndicator.show();
    var loggedIn = false;
    LoadingIndicator.hide();

    if (loggedIn) {
      // TODO: validate cookie via api
    } else {
      this.router.navigateByUrl('/login');
    }
  }
}
