import {Component, effect, signal} from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {AuditPageComponent} from "./pages/audit-page/audit-page.component";
import {CommonModule} from "@angular/common";
import {MatExpansionPanel} from "@angular/material/expansion";
import {AuthService} from "./auth/auth.service";
import {HeaderComponent} from "./ui-common/header/header.component";


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent, CommonModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'client-validation';

}
