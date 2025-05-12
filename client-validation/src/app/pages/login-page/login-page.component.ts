import {Component, signal} from '@angular/core';
import {MatFormField, MatFormFieldModule, MatLabel} from "@angular/material/form-field";
import {MatButton, MatButtonModule, MatIconButton} from "@angular/material/button";
import {MatIcon, MatIconModule} from "@angular/material/icon";
import {MatInput, MatInputModule} from "@angular/material/input";
import {AbstractControl, FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {log} from "@angular-devkit/build-angular/src/builders/ssr-dev-server";
import {AuthService} from "../../auth/auth.service";
import {tap} from "rxjs";
import {Router} from "@angular/router";
import {UserService} from "../../services/user.service";

interface  LoginForm {
  username: FormControl<string | null>;
  password:  FormControl<string | null>;
}
@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [
    MatFormField,
    MatIconButton,
    MatIcon,
    MatLabel,
    MatInput,
    MatButton,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.scss'
})
export class LoginPageComponent {
  form: FormGroup<LoginForm> =  new FormGroup<LoginForm>({
    username: new FormControl<string | null>("", [Validators.required, Validators.email]),
    password: new FormControl<string | null>("", [Validators.required, Validators.minLength(8)]),
  })
  #privateVariable!: string;
  hide = signal(true);

  constructor(private authService: AuthService, private userService: UserService, private router: Router) {
    this.#privateVariable = "hello";
  }


  togglePasswordVisibility = () => {
    this.hide.set(!this.hide());
  }

  submit (event: Event) {
    event.preventDefault();

    const { username, password } = this.form.value;
    const isValidForm =  this.form.valid;
    if (isValidForm) {
        this.authService.login(username!, password!)
          .pipe(
            tap((result) => {
              this.userService
                .fetchUserDetailed()
                .subscribe();
            })
          )
          .subscribe();
    }
  }
}
