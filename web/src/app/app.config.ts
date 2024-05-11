import {ApplicationConfig} from '@angular/core';
import {provideRouter} from '@angular/router';
import * as Clients from './api-clients';
import {environment} from "../environments/environment";


import { routes } from './app.routes';
import {provideHttpClient} from "@angular/common/http";
import {provideAnimations} from "@angular/platform-browser/animations";

export const appConfig: ApplicationConfig = {
  providers: [
    {provide: Clients.API_BASE_URL, useFactory: () => environment.apiUrl},
    provideRouter(routes),
    provideHttpClient(),
    provideAnimations()
  ]
};
