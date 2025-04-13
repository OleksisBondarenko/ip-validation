import {Component, Input} from '@angular/core';
import AuditRecordModel from "../../models/auditDataModel";
import {
  MatCard,
  MatCardActions,
  MatCardContent,
  MatCardHeader,
  MatCardSubtitle,
  MatCardTitle
} from "@angular/material/card";
import {MatList, MatListItem} from "@angular/material/list";
import {MatIcon} from "@angular/material/icon";
import {MatDivider} from "@angular/material/divider";
import {DatePipe} from "@angular/common";

@Component({
  selector: 'app-audit-card',
  standalone: true,
  imports: [
    MatCardContent,
    MatCard,
    MatList,
    MatIcon,
    MatListItem,
    MatDivider,
    DatePipe,
    MatCardHeader
  ],
  templateUrl: './audit-card.component.html',
  styleUrl: './audit-card.component.scss'
})
export class AuditCardComponent {
  @Input() auditRecord?: AuditRecordModel;


}
