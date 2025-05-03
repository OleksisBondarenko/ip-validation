import { Routes } from '@angular/router';
import {AuditPageComponent} from "./pages/audit-page/audit-page.component";
import {LoginPageComponent} from "./pages/login-page/login-page.component";
import {canActiveAuth} from "./guards/auth.guard";

export const routes: Routes = [
  {
    path: 'login', component: LoginPageComponent
  },
  {
    path: '',
    component: AuditPageComponent,
    canActivate: [ canActiveAuth ]
  },

];
