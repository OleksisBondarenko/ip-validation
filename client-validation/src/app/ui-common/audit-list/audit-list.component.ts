import {AfterViewInit, Component, OnInit, signal, ViewChild} from '@angular/core';
import {CommonModule, DatePipe} from '@angular/common';
import {MatTableDataSource, MatTableModule} from '@angular/material/table';
import {MatCardModule} from '@angular/material/card';
import {MatIconModule} from '@angular/material/icon';
import AuditRecordModel, {ResponseGetListAudit} from "../../models/auditData.model";
import {MatPaginator, MatPaginatorModule} from '@angular/material/paginator';
import {AuditRepoService} from "../../services/audit-repo.service";
import {catchError, from, map, Observable, of, startWith, switchMap} from "rxjs";
import {FilterComponent, FilterConfig} from "../filter/filter.component";
import {Filter, FilterRequest} from "../../models/filter.model";
import {environment} from '../../../environments/environment';
import {AuditFilterService} from "../../services/audit-filter.service";
import {AuditTypeStatus} from "../../dto/filter";

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

  currentFilters: Filter [] = [];
  pageSizes = [5, 10, 20];
  isLoading = false;
  totalData = 0;
  listData: AuditRecordModel [] = null!;
  dataSource: MatTableDataSource<AuditRecordModel> = null!;
  @ViewChild(MatPaginator) paginator: MatPaginator = null!;
  displayedColumns: string[] = ['auditType', 'resourceName', 'timestamp', 'ipAddress', "domain" ,'hostname'];

  get filterConfig() {
    return this.filterService.auditFilter;
  }

  onFilterApply (filters: any[]) {
    this.currentFilters = filters;
    this.saveCurrentFiltersSettings()
    this.initPaginator();
  }
  onFilterChanged(filters: any[]) {
    this.currentFilters = filters;
    this.saveCurrentFiltersSettings()
    this.paginator.firstPage();
  }

  onFilterReset() {
    this.currentFilters = [];
    this.saveCurrentFiltersSettings();
    this.initPaginator();
  }

  constructor(
    private auditRepo: AuditRepoService,
    private filterService: AuditFilterService,) {
      this.dataSource = new MatTableDataSource<AuditRecordModel>();
  }

  ngOnInit() {
  }


  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;

     this.initPaginator();
   }

  saveCurrentFiltersSettings () {
    const stringFilterSettings= JSON.stringify(this.currentFilters);
    localStorage.setItem("auditListFilters", stringFilterSettings);
  }

  restoredCurrentFiltersSettings () {
    const stringFilterSettings= localStorage.getItem("auditListFilters");

    if (!!stringFilterSettings) {
       return (JSON.parse(stringFilterSettings)as Filter  [])
       ;
    }

    return [] as Filter [];
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

  isAllowAuditType(auditType: number): Observable<boolean> {
    return this.filterService
      .auditTypesInfo
      .pipe(
        map((auditTypes) => {
          // return auditTypes.some(type => type.type === auditType && type.isAllow)
          const auditTypeInfo = auditTypes.find(typeInfo => typeInfo.type === auditType);
          return auditTypeInfo?.status === AuditTypeStatus.Ok;
        })
    );
  }

  isDbErrorAuditType(auditType: number): Observable<boolean> {
    return this.filterService
      .auditTypesInfo
      .pipe(
        map((auditTypes) => {
          // return auditTypes.some(type => type.type === auditType && type.isAllow)
          const auditTypeInfo = auditTypes.find(typeInfo => typeInfo.type === auditType);
          return auditTypeInfo?.status === AuditTypeStatus.Alert;
        })
      );
  }
}
