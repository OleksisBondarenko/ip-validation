import {Component, signal} from '@angular/core';
import {AuthService} from "../../auth/auth.service";
import {UserService} from "../../services/user.service";
import {MatButton, MatButtonModule, MatMiniFabButton} from "@angular/material/button";
import {MatIconModule} from "@angular/material/icon";
import {CommonModule, NgClass} from "@angular/common";
import {MatTooltip} from "@angular/material/tooltip";
import {Router, RouterLink, RouterLinkActive} from "@angular/router";

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [
    CommonModule,
    NgClass,
    MatMiniFabButton,
    MatIconModule,
    MatTooltip,
    RouterLinkActive,
    RouterLink,
    MatButtonModule
  ],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {
  isAuth= this.authService.isAuth;
  user = this.userService.user;

  constructor(
              private authService: AuthService,
              private userService: UserService,
              private router: Router) {
  }

  get logoutTooltipText () {
    return `Вийти з облікового запису ${this.user()?.email}`
  }

  logout () {
    this.authService.logout();
  }

}
