import {computed, Injectable, signal} from '@angular/core';
import {CookieService} from "ngx-cookie-service";
import {Router} from "@angular/router";
import {HttpClient} from "@angular/common/http";
import {ApiService} from "../services/api.service";
import {LoginDto} from "../dto/login.dto";
import {catchError, pipe, tap} from "rxjs";
import {tokenResponse} from "../dto/tokenResponse.dto";
import {UserModel} from "../models/userModel";

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly _authToken = signal<string | null> (null)
  private _isAuth = computed(() => !!this._authToken());

  isAuth = computed(() => this._isAuth());
  authToken = computed(() => this._authToken());

  constructor(
    private cookieService: CookieService,
    private router: Router,
    private apiService: ApiService
  ) {
    const token = cookieService.get('authtoken')
    this._authToken.set(token || null);
  }

  public login (username: string, password: string) {
    const payload: LoginDto = {username: username, password: password};

    return this.apiService.post<tokenResponse>("api/v1/Auth/login", payload).pipe(
      tap(response => {
        this.cookieService.set('authtoken', response.accessToken);
        this._authToken.set(response.accessToken);
        this.router.navigateByUrl("/");
      }),
      catchError(error => {
        this._authToken.set(null);
        throw  error;
      })
    );
  }

  public logout() {
    this._authToken.set(null);
    this.cookieService.deleteAll();
    this.router.navigateByUrl("/login");
  }

  // get isAuth (): boolean {
  //   if (!this.authtoken) {
  //       this.authtoken = this.cookieService.get('authtoken');
  //   }
  //
  //   return !!this.authtoken;
  // }
}
