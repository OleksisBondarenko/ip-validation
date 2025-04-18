import { Component, OnInit } from '@angular/core';
import { AuditListComponent } from "../../ui-common/audit-list/audit-list.component";
import AuditRecordModel from "../../models/auditDataModel";
import { AuditRepoService } from "../../services/audit-repo.service";
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import {MatProgressSpinner} from "@angular/material/progress-spinner";

@Component({
  selector: 'app-audit-page',
  standalone: true,
  imports: [AuditListComponent, FormsModule, CommonModule, MatProgressSpinner],
  templateUrl: './audit-page.component.html',
  styleUrls: ['./audit-page.component.scss']
})
export class AuditPageComponent {
  public auditList: AuditRecordModel[] = [];
  public searchTerm: string = '';
  public currentPage: number = 1;
  public itemsPerPage: number = 10;
  public totalItems: number = 0;
  public isLoading: boolean = false;

  constructor(private auditRepo: AuditRepoService) {}

}
