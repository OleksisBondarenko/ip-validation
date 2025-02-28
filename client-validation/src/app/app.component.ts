import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {DetailedValidationComponent} from "./detailed-validation/detailed-validation.component";
import {ValidationDetailedListComponent} from "./validation-detailed-list/validation-detailed-list.component";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, ValidationDetailedListComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'client-validation';
}
