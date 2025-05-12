import {Component, signal} from '@angular/core';
import {AuthService} from "../../auth/auth.service";
import {UserService} from "../../services/user.service";
import {MatMiniFabButton} from "@angular/material/button";
import {MatIconModule} from "@angular/material/icon";
import {CommonModule} from "@angular/common";
import {MatTooltip} from "@angular/material/tooltip";

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [
    CommonModule,
    MatMiniFabButton,
    MatIconModule,
  MatTooltip],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {
  isAuth= this.authService.isAuth;
  user = this.userService.user;

  constructor(private authService: AuthService, private userService: UserService) {
  }

  get logoutTooltipText () {
    return `Вийти з облікового запису ${this.user()?.email}`
  }

  logout () {
    this.authService.logout();
  }
}
