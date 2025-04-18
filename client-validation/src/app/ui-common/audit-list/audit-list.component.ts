import {AfterViewInit, Component, Input, OnInit, ViewChild} from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import {MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import AuditRecordModel, {ResponseGetListAudit} from "../../models/auditDataModel";
import {MatPaginator, MatPaginatorModule, PageEvent} from '@angular/material/paginator';
import {AuditRepoService} from "../../services/audit-repo.service";
import {catchError, from, map, Observable, startWith, switchMap} from "rxjs";

@Component({
  selector: 'app-audit-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatCardModule,
    MatIconModule,
    MatPaginatorModule,
    DatePipe
  ],
  templateUrl: './audit-list.component.html',
  styleUrls: ['./audit-list.component.scss']
})
export class AuditListComponent implements OnInit, AfterViewInit {
  pageSizes = [5, 10, 20];
  isLoading = false;
  totalData = 0;
  listData: AuditRecordModel [] = null!;
  dataSource: MatTableDataSource<AuditRecordModel> = null!;
  @ViewChild(MatPaginator) paginator: MatPaginator = null!;
  displayedColumns: string[] = ['auditType', 'resourceName', 'timestamp', 'ip', "domain" ,'host'];

  constructor(private auditRepo: AuditRepoService) {
    this.dataSource = new MatTableDataSource<AuditRecordModel>();
  }

  ngOnInit() {

  }
  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;

    this.paginator.page
      .pipe(
        startWith({}),
        switchMap(() => {
          this.isLoading = true;
          return this.loadData(
            this.paginator.pageIndex,
            this.paginator.pageSize
          ).pipe(catchError((e) => from([])));
        }),
        map((listData) => {
          if (listData == null) return [];
          this.totalData = listData.totalCount;
          this.isLoading = false;
          return listData.data;
        })
      )
      .subscribe((listData) => {
        this.listData = listData;
        this.dataSource = new MatTableDataSource<AuditRecordModel>(this.listData);
      });
   }

  loadData(pageIndex: number = 1, itemsPerPage: number = 10,  searchTerm: string = ""): Observable<ResponseGetListAudit> {
    const query = `?search=${searchTerm}&limit=${itemsPerPage}&start=${(pageIndex * itemsPerPage)}`;

    return this.auditRepo.getListAudit(query);
  }

  onUpdate (event: PageEvent) {
  }
  // getStart() :number {
  //   return (Math.floor((this.currentPage -1 )* this.itemsPerPage) )
  // }

  // onSearch(): void {
  //   this.currentPage = 1; // Reset to first page on new search
  //   this.loadData();
  // }
  //
  // onPageChange(page: PageEvent): void {
  //   debugger
  //   this.currentPage = page.pageIndex;
  //   this.loadData();
  // }
  //
  // onItemsPerPageChange(): void {
  //   this.currentPage = 1; // Reset to first page when changing items per page
  //   this.loadData();
  // }
  //
  //
  // totalPages(): number {
  //   return Math.ceil(this.totalItems / this.itemsPerPage);
  // }
}
