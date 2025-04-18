import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {AuditPageComponent} from "./pages/audit-page/audit-page.component";
import {CommonModule} from "@angular/common";
import {MatExpansionPanel} from "@angular/material/expansion";


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, AuditPageComponent, CommonModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'client-validation';
}
