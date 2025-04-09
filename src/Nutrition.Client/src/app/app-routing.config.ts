import { provideRouter, Routes } from "@angular/router";
import { ApplicationConfig, importProvidersFrom } from "@angular/core";
import { provideHttpClient } from '@angular/common/http';

import { LoginComponent } from "../app/components/login/login.component"
import { RegisterComponent } from "../app/components/register/register.component"

const appRoutes: Routes = [
  { path: "login", component: LoginComponent },
  { path: "register", component: RegisterComponent }
];

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(appRoutes),
    provideHttpClient()
  ]
};
