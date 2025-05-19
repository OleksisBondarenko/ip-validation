import { Routes } from '@angular/router';
import {AuditPageComponent} from "./pages/audit-page/audit-page.component";
import {LoginPageComponent} from "./pages/login-page/login-page.component";
import {canActiveAuth} from "./guards/auth.guard";
import {PolicyControlPageComponent} from "./pages/policy-control-page/policy-control-page.component";

export const routes: Routes = [
  {
    path: 'login', component: LoginPageComponent
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
  }

];
