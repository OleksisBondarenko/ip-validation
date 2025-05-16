import { Routes } from '@angular/router';
import {AuditPageComponent} from "./pages/audit-page/audit-page.component";
import {LoginPageComponent} from "./pages/login-page/login-page.component";
import {canActiveAuth} from "./guards/auth.guard";
import {WhiteListPageComponent} from "./pages/white-list-page/white-list-page.component";
import {LayoutAuthorizedComponent} from "./pages/layout-authorized/layout-authorized.component";

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
        component: WhiteListPageComponent
      }
    ]
  },
  {
    path: 'login', component: LoginPageComponent
  },
];
