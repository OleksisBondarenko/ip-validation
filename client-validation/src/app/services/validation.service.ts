import { Injectable } from '@angular/core';
import {ApiService} from "./api.service";
import {HttpResponse} from "@angular/common/http";
import {ValidationDto} from "../dto/validation.dto";

@Injectable({
  providedIn: 'root'
})
export class ValidationService {

  constructor(private apiService: ApiService) { }

  validate (resource: string) {
    // TODO: change object with a real one.
    return this.apiService.getFullResponse<HttpResponse<ValidationDto>>(`api/v1/Validate`, { observe: 'response', params: { resource: resource } });
  }
}
