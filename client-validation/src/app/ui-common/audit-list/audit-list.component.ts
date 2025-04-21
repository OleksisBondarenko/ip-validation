import {AfterViewInit, Component, Input, OnInit, ViewChild} from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import {MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import AuditRecordModel, {ResponseGetListAudit} from "../../models/auditData.model";
import {MatPaginator, MatPaginatorModule, PageEvent} from '@angular/material/paginator';
import {AuditRepoService} from "../../services/audit-repo.service";
import {catchError, from, map, Observable, startWith, switchMap} from "rxjs";
import {FilterComponent, FilterConfig} from "../filter/filter.component";
import {Filter, FilterRequest} from "../../models/filter.model";

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
  filterConfig: FilterConfig [] = [
    {
      type: 'select',
      key: 'auditType',
      label: 'Audit Type',
      options: [
        { value: '0', label: 'Ok' },
        { value: '1', label: 'NotFound' },
        { value: '2', label: 'NotFoundDomain' },
        { value: '3', label: 'NotFoundEset' },
        { value: '4', label: 'NotValidEsetTimespan' },
        { value: '10', label: 'NoAccessToDb' },
      ]
    },
    {
      type: 'text',
      key: 'resourceName',
      label: 'Resource Name'
    },
    {
      type: 'date',
      key: 'timestamp',
      label: 'Date Range'
    },
    {
      type: 'text',
      key: 'ipAddress',
      label: 'IP Address'
    },
    {
      type: 'select',
      key: 'domain',
      label: 'Domain',
      options: [
        { value: 'localhost', label: 'Localhost' },
        { value: 'test.com', label: 'Test Domain' }
      ]
    }
  ];

  onFilterApply (filters: any[]) {
    this.currentFilters = filters;

    this.initPaginator();
  }
  onFilterChanged(filters: any[]) {
    this.currentFilters = filters;
  }

  onFilterReset() {
    this.currentFilters = [];
    this.initPaginator();

  }

  currentFilters: Filter [] = [];
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
         this.dataSource = new MatTableDataSource<AuditRecordModel>(this.listData);
       });
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
