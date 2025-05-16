import { Component, OnInit } from '@angular/core';
import { AuditListComponent } from "../../ui-common/audit-list/audit-list.component";
import AuditRecordModel from "../../models/auditData.model";
import { AuditRepoService } from "../../services/audit-repo.service";
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import {MatProgressSpinner} from "@angular/material/progress-spinner";
import {RouterOutlet} from "@angular/router";

@Component({
  selector: 'app-audit-page',
  standalone: true,
  imports: [
    AuditListComponent,
    FormsModule,
    CommonModule,
    RouterOutlet
  ],
  templateUrl: './audit-page.component.html',
  styleUrls: ['./audit-page.component.scss']
})
export class AuditPageComponent {

}
