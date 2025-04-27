import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import {HttpClientModule, provideHttpClient} from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import {provideNativeDateAdapter} from "@angular/material/core";
import provideMatPaginatorIntl from "./providers/custom/paginator.provider";


export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(),
    provideNativeDateAdapter(),
    provideAnimationsAsync(),
    provideMatPaginatorIntl()
  ]
};
