import {Component, inject, OnDestroy, signal} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import { ValidationService } from '../../services/validation.service';
import {MatButtonModule} from "@angular/material/button";
import {MatCommonModule} from "@angular/material/core";
import {catchError, interval, map, of, Subscription, switchMap, takeWhile, tap, timer} from "rxjs";
import {errorContext} from "rxjs/internal/util/errorContext";
import {HttpResponse} from "@angular/common/http";
import {ValidationDto} from "../../dto/validation.dto";
import {JsonPipe} from "@angular/common";
import {LoaderComponent} from "../../ui-common/loader/loader.component";
import {LoaderSpinnerComponent} from "../../ui-common/loader-spinner/loader-spinner.component";
import {environment} from "../../../environments/environment";

const emptyValidationResult: ValidationDto = {
  data: {
    hostname: '',
    ipAddress: '',
    domain: '',
    message: '',
    resourceName: "",
    userName: "",
  },
  errorMessage: "",
  auditCode: "",
};

@Component({
  selector: 'app-validation-error-page',
  standalone: true,
  imports: [MatCommonModule, MatButtonModule, JsonPipe, LoaderComponent, LoaderSpinnerComponent],
  templateUrl: './validation-error-page.component.html',
  styleUrl: './validation-error-page.component.scss'
})
export class ValidationErrorPageComponent implements  OnDestroy {
  resource: string = "";
  validationResult = signal<ValidationDto>(emptyValidationResult);

  isValid: boolean = false;
  currentValidateAttempt: number = 0;
  remainingSecondsToRedirect: number = 0;

  readonly maxValidateCount: number = 10; //
  readonly validationInterval: number = 15_000; // 10 seconds
  readonly redirectInterval: number = 1000;

  private redirectTimerSubscription!: Subscription;
  private activatedRoute = inject(ActivatedRoute);

  constructor(
    private validateService: ValidationService,
  )
  {
    this.activatedRoute.queryParams.subscribe((params) => {
      this.resource = params['resource'];

      if (!this.resource) {
        this.redirectByDefault();
        return;
      }

      this.checkApiUntilSuccess(this.resource);
    });
  }

  get isReachedMaxCount(): boolean {
    return this.maxValidateCount < this.currentValidateAttempt;
  }

  checkApiUntilSuccess(resource: string): void {
    timer(0, this.validationInterval)
      .pipe(
        takeWhile(() => (!this.isReachedMaxCount && !this.isValid)),
        switchMap(() => {
          this.currentValidateAttempt ++;
          return this.validate(resource)
            .pipe(
              catchError(err => {
                this.handleValidationErr(err.error);
                return of(err);
              })
            );
        })
      )
      .subscribe((response) => {
        if (response?.status === 200) {
          this.handleValidationOk(response.body);
        }
      })
  }

  handleValidationErr = (validation: ValidationDto) => {
    this.validationResult.set(validation);
  }
  handleValidationOk (validation: ValidationDto)  {
    this.validationResult.set(validation);
    this.isValid = true;
    this.startTimerRedirectToResource();
    //this.redirectToResource()

  }

  resetValidateCount(): void {
    this.currentValidateAttempt = 0;
    this.checkApiUntilSuccess(this.resource);
  }

  validate (resource: string) {
    return this.validateService.validate(resource);
  }

  startTimerRedirectToResource(): void {
    this.redirectTimerSubscription = interval(this.redirectInterval).subscribe(() => {
      if (this.remainingSecondsToRedirect <= 0) {
        this.redirectTimerSubscription.unsubscribe();
        this.redirectToResource();
      } else {
        this.remainingSecondsToRedirect--;
      }
    });
  }

  redirectToResource (): void {
      window.location.href = 'http://'+this.resource+'/';
  }

  redirectByDefault (): void {
    window.location.href = environment.redirectionByDefault;
  }

  ngOnDestroy(): void {
    // Cleanup if component is destroyed
    if (this.redirectTimerSubscription) {
      this.redirectTimerSubscription.unsubscribe();
    }
  }
}
