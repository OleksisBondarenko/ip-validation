import { environment } from "../../environments/environment";
import { Injectable } from '@angular/core';
import {HttpClient, HttpEvent, HttpParams, HttpResponse} from "@angular/common/http";
import {catchError, Observable, throwError} from "rxjs";
import {ErrorModel} from "../models/errorModel";

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  private apiUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }

  // GET request
  get<T>(endpoint: string, params?: any): Observable<T> {
    let httpParams = new HttpParams();

    if (params) {
      Object.keys(params).forEach(key => {
        if (params[key] !== undefined && params[key] !== null) {
          httpParams = httpParams.append(key, params[key]);
        }
      });
    }

    return this.http.get<T>(`${this.apiUrl}/${endpoint}`, { params: httpParams })
      .pipe(
        catchError(this.handleError)
      );
  }

  // GET request
  getFullResponse<T>(endpoint: string, options?: any): Observable<HttpEvent<T> | ErrorModel<T>> {
    let httpParams = new HttpParams();

    const params = options.params || {};
    if (params) {
      Object.keys(params).forEach(key => {
        if (params[key] !== undefined && params[key] !== null) {
          httpParams = httpParams.append(key, params[key]);
        }
      });
    }

    return this.http.get<T>(`${this.apiUrl}/${endpoint}`, { ...options, params: httpParams })
      .pipe(
        catchError(this.handleErrorFull<T>)
      );
  }

  // POST request
  post<T>(endpoint: string, body: any): Observable<T> {
    return this.http.post<T>(`${this.apiUrl}/${endpoint}`, body)
      .pipe(
        catchError(this.handleError)
      );
  }

  // PATCH request
  patch<T>(endpoint: string, body: any): Observable<T> {
    return this.http.patch<T>(`${this.apiUrl}/${endpoint}`, body)
      .pipe(
        catchError(this.handleError)
      );
  }

  // PUT request
  put<T>(endpoint: string, body: any): Observable<T> {
    return this.http.put<T>(`${this.apiUrl}/${endpoint}`, body)
      .pipe(
        catchError(this.handleError)
      );
  }

  // DELETE request
  delete<T>(endpoint: string): Observable<T> {
    return this.http.delete<T>(`${this.apiUrl}/${endpoint}`)
      .pipe(
        catchError(this.handleError)
      );
  }

  // Error handling
  private handleError(error: any) {
    let errorMessage = 'An unknown error occurred!';
    console.error(error);
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // Server-side error
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }

    console.error(errorMessage);
    return throwError(errorMessage);
  }

  // Error handling
  private handleErrorFull<T>(error: any): Observable<ErrorModel<T>> {
    let customError: ErrorModel<T>= { message: "" };

    customError.error = error.error;
    customError.status = error.status;
    customError.message = 'An unknown error occurred!';
    console.error(error);
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      customError.message = `Error: ${error.error.message}`;
    } else {
      // Server-side error
      customError.message = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }

    console.error(customError.message);
    return throwError(customError);
  }
}
