import {HttpEvent} from "@angular/common/http";

export interface ErrorModel<T> {
  message: string;
  error?: T;
  status?: number;
}
