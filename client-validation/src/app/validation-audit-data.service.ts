import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import {Observable, of} from "rxjs";
import AuditDataModel from "./models/auditDataModel";

@Injectable({
  providedIn: 'root',
})
export class ValidationAuditDataService {
  private auditDataJsonURL = 'http://localhost:5001/Audit?orderBy=Timestamp&orderByDir=ASC&limit=100&start=0';

  constructor(private http: HttpClient) {

  }

  getData(): Observable<AuditDataModel []> {
    const data = this.http.get<AuditDataModel[]>(this.auditDataJsonURL);

    return data;
  }

}
