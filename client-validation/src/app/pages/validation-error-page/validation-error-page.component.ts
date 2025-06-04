import {Component, inject, signal} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import { ValidationService } from '../../services/validation.service';
import {MatButtonModule} from "@angular/material/button";
import {MatCommonModule} from "@angular/material/core";
import {catchError, map, of, switchMap, takeWhile, tap, timer} from "rxjs";
import {errorContext} from "rxjs/internal/util/errorContext";
import {HttpResponse} from "@angular/common/http";
import {ValidationDto} from "../../dto/validation.dto";
import {JsonPipe} from "@angular/common";

const emptyValidationResult: ValidationDto = {
  body: {
    hostname: '',
    ipAddress: '',
    domain: '',
    message: '',
    resourceName: "",
    userName: "",
  },
  message: "",
  auditCode: "",
};

@Component({
  selector: 'app-validation-error-page',
  standalone: true,
  imports: [MatCommonModule, MatButtonModule, JsonPipe],
  templateUrl: './validation-error-page.component.html',
  styleUrl: './validation-error-page.component.scss'
})
export class ValidationErrorPageComponent {
  resource: string = "";
  isValid: boolean = false;
  validationResult = signal<ValidationDto>(emptyValidationResult);

  currentValidateCount: number = 0;
  maxValidateCount: number = 50; //
  validationInterval: number = 5000;

  private activatedRoute = inject(ActivatedRoute);

  constructor(
    private validateService: ValidationService,
  )
  {
    this.activatedRoute.params.subscribe((params) => {
      this.resource = params['resource'];

      this.checkApiUntilSuccess(this.resource);
    });
  }

  get isReachedMaxCount(): boolean {
    return this.maxValidateCount > this.currentValidateCount;
  }

  checkApiUntilSuccess(resource: string): void {
    timer(0, this.validationInterval)
      .pipe(
        takeWhile(() => this.isReachedMaxCount),
        switchMap(() => {
          this.currentValidateCount ++;
          return this.validate(resource).pipe(
              tap(valid => {
                this.redirectToResource()
              }),
              catchError(err => {
                const receivedBody = err.error;
                debugger
                this.validationResult.set(receivedBody);

                return of(err)
              })
            )
          }
        )
      )
      .subscribe((response) => {
        // this.validationResult.set(response.body) = ;

        if (response?.status === 200) {
          this.isValid = true;
        }
      })
  }

  resetValidateCount(): void {
    this.currentValidateCount = 0;
  }

  validate (resource: string) {
    return this.validateService.validate(resource);
  }

  redirectToResource (): void {
    window.location.href = 'https://'+this.resource+'/';
  }
}
