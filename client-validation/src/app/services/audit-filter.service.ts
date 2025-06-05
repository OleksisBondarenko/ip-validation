import { Injectable } from '@angular/core';
import {FilterRequest} from "../models/filter.model";
import {ResponseGetListAudit} from "../models/auditData.model";
import {ApiService} from "./api.service";
import {AuditTypeInfo} from "../dto/filter";
import {Observable, shareReplay} from "rxjs";
import {FilterConfig} from "../ui-common/filter/filter.component";

@Injectable({
  providedIn: 'root'
})
export class AuditFilterService {

  private _auditTypesInfo!: Observable<AuditTypeInfo[]>;
  private _auditFilter!: Observable<FilterConfig[]>;
  get auditTypesInfo(): Observable<AuditTypeInfo[]> {
    if (!this._auditTypesInfo) {
      this._auditTypesInfo = this.getInfoAboutAuditTypes()
        .pipe(
          shareReplay(1)
        );
    }

    return this._auditTypesInfo;
  }

  get auditFilter(): Observable<FilterConfig[]> {
    if (!this._auditFilter) {
      this._auditFilter = this.getAuditFilter()
        .pipe(
          shareReplay(1)
        );
    }

    return this._auditFilter;
  }

  constructor(private apiService: ApiService) {

  }
  private getAuditFilter () {
    return this.apiService.get<FilterConfig []>(
      `api/v1/audit/filter`);
  }
  private getInfoAboutAuditTypes () {
    return this.apiService.get<AuditTypeInfo []>(
      `api/v1/audit/filter/auditTypes`);
  }
}
