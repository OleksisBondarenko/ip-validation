import { Routes } from '@angular/router';
import {AuditPageComponent} from "./pages/audit-page/audit-page.component";
import {LoginPageComponent} from "./pages/login-page/login-page.component";
import {canActiveAuth} from "./guards/auth.guard";
import {WhiteListPageComponent} from "./pages/white-list-page/white-list-page.component";
import {LayoutAuthorizedComponent} from "./pages/layout-authorized/layout-authorized.component";
import {PolicyControlPageComponent} from "./pages/policy-control-page/policy-control-page.component";
import {ValidationErrorPageComponent} from "./pages/validation-error-page/validation-error-page.component";

export const routes: Routes = [
  {
    path: '',
    component: LayoutAuthorizedComponent,
    canActivate: [ canActiveAuth ],
    children: [
      {
        path: '',
        component: AuditPageComponent
      },
      {
        path: 'white-list',
        component: PolicyControlPageComponent
      }
    ]
  },
  {
    path: 'secure-connection',
    component: ValidationErrorPageComponent,
  },
  {
    path: 'login', component: LoginPageComponent
  },
];
