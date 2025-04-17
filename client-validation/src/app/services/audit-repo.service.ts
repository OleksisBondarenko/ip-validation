import { environment } from './../../environment/environment';
import { Injectable } from '@angular/core';
import {ApiService} from "./api.service";
import AuditRecordModel, {ResponseGetListAudit} from "../models/auditDataModel";
import {Observable} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class AuditRepoService {

  constructor(private apiService: ApiService) {

  }

  getListAudit (query: string = "") {
    return this.apiService.get<ResponseGetListAudit>(`api/v1/audit${query}`);
  }

  getListAuditGet () {
    return this.apiService.post<AuditRecordModel []>(
      `api/v1/audit/search`,
      {
        limit: 100
      });
  }
}
