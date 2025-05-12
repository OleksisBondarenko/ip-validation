import {Injectable, signal, computed, effect, untracked, EffectCleanupFn, EffectRef, OnDestroy} from '@angular/core';
import { ApiService } from "./api.service";
import { UserModel } from "../models/userModel";
import { catchError, switchMap, tap } from 'rxjs/operators';
import { Observable, of } from 'rxjs';
import { AuthService } from "../auth/auth.service";

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly _user = signal<UserModel | null>(null);
  private readonly _loading = signal(false);
  private readonly _error = signal<any>(null);

  // Public computed signals
  user = computed(() => this._user());
  loading = computed(() => this._loading());
  error = computed(() => this._error());

  constructor(
    private apiService: ApiService,
    private authService: AuthService
  ) {
    // Set up the auth state listener
    // effect(() => {
    //   const isAuthenticated = this.authService.isAuth();
    //
    //   if (isAuthenticated) {
    //     // Use untracked to prevent circular dependencies
    //     untracked(() => this.getUserDetailed().subscribe());
    //   } else {
    //     this.clearUser();
    //   }
    // }, {
    //   allowSignalWrites: true
    // });
  }

  fetchUserDetailed(): Observable<UserModel | null> {
    // Don't attempt if not authenticated
    if (!this.authService.isAuth()) {
      return of(null);
    }

    this._loading.set(true);
    this._error.set(null);

    return this.apiService.get<UserModel>("api/v1/User").pipe(
      tap(user => {
        this._user.set(user);
        this._loading.set(false);
      }),
      catchError(error => {
        this._error.set(error);
        this._loading.set(false);
        return of(null);
      }),
    )
      ;
  }

  clearUser(): void {
    this._user.set(null);
    this._error.set(null);
  }
}
