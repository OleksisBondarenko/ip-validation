// filter.component.ts
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import {FormBuilder, FormGroup, ReactiveFormsModule} from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import {MatError, MatFormField, MatHint, MatLabel} from "@angular/material/form-field";
import {CommonModule, NgSwitch} from "@angular/common";
import {MatOption, MatSelect} from "@angular/material/select";
import { MatDatepickerModule } from "@angular/material/datepicker";
import {MatInput, MatInputModule} from "@angular/material/input";
import {MatButton, MatButtonModule} from "@angular/material/button";


import {OwlDateTimeModule, OwlNativeDateTimeModule} from "@danielmoncada/angular-datetime-picker";
import {map, Observable, of, switchMap} from "rxjs";
import {Filter} from "../../models/filter.model";

export interface FilterConfig {
  type: 'text' | 'date' | 'select' | 'selectMany';
  key: string;
  label: string;
  options?: { value?: any; label?: string }[];
}

// export type FilterType = FilterConfig['type']

@Component({
  selector: 'app-filter',
  templateUrl: './filter.component.html',
  standalone: true,
  imports: [
    MatFormField,
    ReactiveFormsModule,
    NgSwitch,
    MatSelect,
    MatOption,
    MatLabel,
    MatInput,
    MatButton,
    MatButtonModule,
    CommonModule,
    OwlDateTimeModule,
    OwlNativeDateTimeModule,
    MatInputModule,
    MatDatepickerModule,
  ],
  styleUrls: ['./filter.component.scss']
})
export class FilterComponent implements OnInit {
  @Input() initialAppliedConfig: Filter[] = [];
  @Input() filterConfig: Observable<FilterConfig[]> = of([]);
  @Output() filterChanged = new EventEmitter<any>();
  @Output() filterReset = new EventEmitter<void>();
  @Output() filterApply = new EventEmitter<any>();

  filterForm: FormGroup;

  constructor(private fb: FormBuilder) {
    this.filterForm = this.fb.group({});
  }

  ngOnInit() {
    this.createFormControls();
    this.setupFilterChanges();
    this.resetFormByInitConfig();
  }

  private createFormControls() {
    if (!this.filterConfig) {
      return;
    }

    this.filterConfig.subscribe(configs => {
          configs.forEach(config => {
          // if (config.type === 'date') {
          //   this.filterForm.addControl(`${config.key}_from`, this.fb.control(''));
          //   this.filterForm.addControl(`${config.key}_to`, this.fb.control(''));
          // } else {
            this.filterForm.addControl(config.key, this.fb.control(''));
          // }
        })
    });
  }

  private setupFilterChanges() {
    this.filterForm.valueChanges
      .pipe(
        debounceTime(10),
        distinctUntilChanged(),
      )
      .subscribe(value => {
        // this.filterChanged.emit(this.parseFilters(value));
        if (value && typeof value === 'object') {
          const timestamp = value.timestamp;
          if (timestamp && Array.isArray(timestamp) && timestamp.length > 1) {
            value.timestamp = {
              from: timestamp[0],
              to: timestamp[1]
            };
          }
          this.filterChanged.emit(this.parseFilters(value));
        }
      });
  }

  private parseFilters(formValue: any): any[] {
    const filters: any[] = [];

    this.filterConfig.subscribe(configs => {
      configs.forEach(config => {
        const value = formValue?.[config.key];
        const from = value?.from;
        const to = value?.to;

        if (value || (from || to)) {
          filters.push({
            type: this.getFilterType(config.type),
            alias: config.key,
            value: this.createFilterValue(config.type, value, from, to)
          });
        }
      })
    });


    return filters;
  }

  private getFilterType(type: string): string {
    switch(type) {
      case 'date': return 'date';
      case 'selectMany': return 'stringArray';
      default: return 'string';
    }
  }

  private getFilterConfigType(type: string): string {
    switch(type) {
      case 'date': return 'date';
      case 'stringArray': return 'selectMany';
      default: return 'string';
    }
  }

  private createFilterValue(type: string, value: any, from?: any, to?: any) {
    switch(type) {
      case 'date':
        return from && to ? { from: from?.toISOString(), to: to?.toISOString() } : null;
      case 'select':
        return { input: value };
       case 'selectMany':
         return { input: JSON.stringify(value) };
      default:
        return { input: value, operator: 'contains' };
    }
  }

  applyFilters () {
    const newFilters = this.parseFilters(this.filterForm.value);
    this.filterApply.emit(newFilters);
  }
  resetFilters() {
    this.resetForm();
    this.applyFilters();
  }

  resetFormByInitConfig() {
    this.defaultValuesByInitialConfig()
      .subscribe(defaultValues => {
        this.filterForm.reset(defaultValues);
        this.applyFilters();
      });
  }

  resetForm() {
    this.defaultValuesByConfig()
      .subscribe(defaultValues => {
        this.filterForm.reset(defaultValues);
      });
  }

  defaultValuesByInitialConfig(): Observable<any> {
    const initialFilterConfigs =  this.initialAppliedConfig.map((fc) => {
      return {
        key: fc.alias,
        options: fc.type === "stringArray" ?
            JSON.parse(fc.value.input as string) : fc.value.input,
        type: this.getFilterConfigType(fc.type),
        label: ""
      } as FilterConfig
    }) as FilterConfig[]

    return this.filterConfig.pipe(
      map(filterConfigs => {
        const defaultConfig: any = {};

        filterConfigs.forEach(config => {
          const initial =initialFilterConfigs.find(c => c.key === config.key);

          if (initial && initial.type === 'selectMany') {
            defaultConfig[config.key] = initial.options?.length ? initial.options : [] ;
          } else if (initial && initial.type === 'date') {
            defaultConfig[config.key] = '';
          } else if (initial && initial.type !== 'selectMany') {
            defaultConfig[config.key] = initial.options?.length ? initial.options : [];
          }else {
            // fallback if not in initialAppliedConfig
            if (config.type === 'selectMany') {
              defaultConfig[config.key] = config.options?.length ? config.options.map(o => o.value) : [];
            } else {
              defaultConfig[config.key] = '';
            }
          }
        });

        return defaultConfig;
      })
    );
  }


  defaultValuesByConfig(): Observable<any> {
    return this.filterConfig.pipe(
      map(configs => {
        const defaultConfig: any = {};
        configs.forEach(config => {
          if (config.type === 'selectMany') {
            defaultConfig[config.key] = config.options?.map(option => option.value) || [];
          } else {
            defaultConfig[config.key] = '';
          }
        });
        return defaultConfig;
      })
    );
  }

}
