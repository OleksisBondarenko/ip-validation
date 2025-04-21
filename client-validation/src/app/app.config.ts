import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import {HttpClientModule, provideHttpClient} from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import {provideNativeDateAdapter} from "@angular/material/core";
import {NgxMatNativeDateAdapter} from "@cahdev-angular-material-components/datetime-picker";


export const appConfig: ApplicationConfig = {
  providers: [provideRouter(routes),  provideHttpClient(),  provideNativeDateAdapter(), provideAnimationsAsync()]
};
