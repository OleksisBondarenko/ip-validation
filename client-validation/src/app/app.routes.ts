import { Routes } from '@angular/router';
import {AuditPageComponent} from "./pages/audit-page/audit-page.component";
import {LoginPageComponent} from "./pages/login-page/login-page.component";
import {canActiveAuth} from "./guards/auth.guard";
import {PolicyControlPageComponent} from "./pages/policy-control-page/policy-control-page.component";
import {ErrorPageComponent} from "./pages/error-page/error-page.component";
import {ValidationErrorPageComponent} from "./pages/validation-error-page/validation-error-page.component";

export const routes: Routes = [
  {
    path: 'error/:resource',
    component: ValidationErrorPageComponent,
  },
  {
    path: 'login',
    component: LoginPageComponent
  },
  {
    path: '',
    component: AuditPageComponent,
    canActivate: [ canActiveAuth ]
  },
  {
    path: 'policy',
    component: PolicyControlPageComponent,
    canActivate: [ canActiveAuth ]
  },

];
