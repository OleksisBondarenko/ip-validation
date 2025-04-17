import { Component, Input } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import AuditRecordModel from "../../models/auditDataModel";

@Component({
  selector: 'app-audit-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatCardModule,
    MatIconModule,
    DatePipe
  ],
  templateUrl: './audit-list.component.html',
  styleUrls: ['./audit-list.component.scss']
})
export class AuditListComponent {
  @Input() auditList: AuditRecordModel[] = [];
  displayedColumns: string[] = ['auditType', 'resourceName', 'timestamp', 'ip', "domain" ,'host'];
}
