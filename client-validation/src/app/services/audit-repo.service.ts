import { environment } from './../../environment/environment';
import { Injectable } from '@angular/core';
import {ApiService} from "./api.service";
import AuditRecordModel, {ResponseGetListAudit} from "../models/auditData.model";
import {Observable} from "rxjs";
import {FilterRequest} from "../models/filter.model";

@Injectable({
  providedIn: 'root'
})
export class AuditRepoService {

  constructor(private apiService: ApiService) {

  }

  getListAudit (query: string = "") {
    return this.apiService.get<ResponseGetListAudit>(`api/v1/audit${query}`);
  }

  getListAuditPost (filterRequest: FilterRequest) {
    return this.apiService.post<ResponseGetListAudit>(
      `api/v1/audit/search`,
      filterRequest);
  }
}
