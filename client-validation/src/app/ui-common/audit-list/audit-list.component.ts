import {AfterViewInit, Component, Input, OnInit, signal, ViewChild} from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import {MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import AuditRecordModel, {ResponseGetListAudit} from "../../models/auditData.model";
import {MatPaginator, MatPaginatorIntl, MatPaginatorModule, PageEvent} from '@angular/material/paginator';
import {AuditRepoService} from "../../services/audit-repo.service";
import {catchError, from, map, Observable, startWith, switchMap} from "rxjs";
import {FilterComponent, FilterConfig} from "../filter/filter.component";
import {Filter, FilterRequest} from "../../models/filter.model";
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-audit-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatCardModule,
    MatIconModule,
    MatPaginatorModule,
    DatePipe,
    FilterComponent
  ],
  templateUrl: './audit-list.component.html',
  styleUrls: ['./audit-list.component.scss']
})
export class AuditListComponent implements OnInit, AfterViewInit {
  isEmpty = signal(true);
  filterConfig: FilterConfig [] = environment.filterConfig;

  currentFilters: Filter [] = [];
  pageSizes = [5, 10, 20];
  isLoading = false;
  totalData = 0;
  listData: AuditRecordModel [] = null!;
  dataSource: MatTableDataSource<AuditRecordModel> = null!;
  @ViewChild(MatPaginator) paginator: MatPaginator = null!;
  displayedColumns: string[] = ['auditType', 'resourceName', 'timestamp', 'ip', "domain" ,'host'];

  onFilterApply (filters: any[]) {
    this.currentFilters = filters;

    this.initPaginator();
  }
  onFilterChanged(filters: any[]) {
    this.currentFilters = filters;
    this.paginator.firstPage();
  }

  onFilterReset() {
    this.currentFilters = [];
    this.initPaginator();
  }

  constructor(private auditRepo: AuditRepoService) {
    this.dataSource = new MatTableDataSource<AuditRecordModel>();
  }

  ngOnInit() {

  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;

     this.initPaginator();
   }

   initPaginator () {
     this.paginator.page
       .pipe(
         startWith({}),
         switchMap(() => {
           this.isLoading = true;
           return this.loadData(
             this.paginator.pageIndex * this.paginator.pageSize,
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
         const withFilledEmpty = this.fillEmptyByTotalLength(listData, this.paginator.pageSize);
         this.dataSource = new MatTableDataSource<AuditRecordModel>(withFilledEmpty);
       });
   }

   fillEmptyByTotalLength (list: AuditRecordModel [], totalLength: number) {
      if (list.length < totalLength) {} {
        const diffLength = totalLength - list.length;

        for (let i = 0; i < diffLength; i++) {
          const emptyRecord: AuditRecordModel = {
            id: "",
            auditType: -1,
            auditTypeString: "",
            timeStamp: "",
            auditData: {
              domain: "",
              hostName: "",
              ipAddress: "",
              ip: ""
            }
          }

          list.push(emptyRecord)
        }
     }

      return list;
   }

  // loadData(pageIndex: number = 1, itemsPerPage: number = 10,  searchTerm: string = ""): Observable<ResponseGetListAudit> {
  //   const query = `?search=${searchTerm}&limit=${itemsPerPage}&start=${(pageIndex * itemsPerPage)}`;
  //
  //   return this.auditRepo.getListAudit(query);
  // }

  loadData (pageIndex: number = 1, itemsPerPage: number = 10,  searchTerm: string = ""): Observable<ResponseGetListAudit> {
      // const query = `?search=${searchTerm}&limit=${itemsPerPage}&start=${(pageIndex * itemsPerPage)}`;

      // return this.auditRepo.getListAudit(query);
      const filterRequest: FilterRequest = {
        start: pageIndex,
        limit: itemsPerPage,
        search: searchTerm,
        filters: this.currentFilters
      }

      return this.auditRepo.getListAuditPost(filterRequest)
  }
}
