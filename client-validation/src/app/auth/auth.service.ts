import { Injectable } from '@angular/core';
import {CookieService} from "ngx-cookie-service";
import {Router} from "@angular/router";
import {HttpClient} from "@angular/common/http";
import {ApiService} from "../services/api.service";
import {LoginDto} from "../dto/login.dto";
import {catchError, pipe, tap} from "rxjs";
import {tokenResponse} from "../dto/tokenResponse.dto";

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  authtoken: string;

  constructor(
    private cookieService: CookieService,
    private router: Router,
    private apiService: ApiService
  ) {
    this.authtoken = cookieService.get('authtoken');
  }

  public login (username: string, password: string) {
    const payload: LoginDto = {username: username, password: password};

    this.apiService.post<tokenResponse>("api/v1/Auth/login", payload)
      .pipe(
        catchError((err) => {
          return [];
        }),
      )
      .subscribe(value => {
        this.authtoken = value.accessToken;
        this.cookieService.set('authtoken', value.accessToken);
        this.router.navigateByUrl("/");
      });
  }

  public logout() {
    this.authtoken = "";
    this.cookieService.deleteAll();
    this.router.navigateByUrl("/login");
  }

  get isAuth (): boolean {
    if (!this.authtoken) {
        this.authtoken = this.cookieService.get('authtoken');
    }

    return !!this.authtoken;
  }
}
