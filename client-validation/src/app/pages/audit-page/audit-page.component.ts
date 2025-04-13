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
export class AuditPageComponent implements OnInit {
  public auditList: AuditRecordModel[] = [];
  public searchTerm: string = '';
  public currentPage: number = 1;
  public itemsPerPage: number = 10;
  public totalItems: number = 0;
  public isLoading: boolean = false;

  constructor(private auditRepo: AuditRepoService) {}

  ngOnInit() {
    this.loadData();
  }

  loadData(): void {
    this.isLoading = true;
    const query = `?search=${this.searchTerm}&limit=${this.itemsPerPage}&start=${this.getStart()}`;

    this.auditRepo.getListAudit(query).subscribe({
      next: (response) => {
        this.auditList = response.data; // Assuming paginated response
        this.totalItems = response.totalCount;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error fetching data:', error);
        this.isLoading = false;
      }
    });
  }

  onSearch(): void {
    this.currentPage = 1; // Reset to first page on new search
    this.loadData();
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadData();
  }

  onItemsPerPageChange(): void {
    this.currentPage = 1; // Reset to first page when changing items per page
    this.loadData();
  }

  getStart() :number {
    return (Math.floor((this.currentPage -1 )* this.itemsPerPage) )
  }
  totalPages(): number {
    return Math.ceil(this.totalItems / this.itemsPerPage);
  }
}
