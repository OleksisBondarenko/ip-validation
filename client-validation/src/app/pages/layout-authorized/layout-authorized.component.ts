import { Component } from '@angular/core';
import {RouterOutlet} from "@angular/router";
import {HeaderComponent} from "../../ui-common/header/header.component";

@Component({
  selector: 'app-layout-authorized',
  standalone: true,
  imports: [
    RouterOutlet,
    HeaderComponent
  ],
  templateUrl: './layout-authorized.component.html',
  styleUrl: './layout-authorized.component.scss'
})
export class LayoutAuthorizedComponent {

}
