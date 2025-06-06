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

export interface FilterConfig {
  type: 'text' | 'date' | 'select' | 'selectMany';
  key: string;
  label: string;
  options?: { value?: any; label?: string }[];
}

export type FilterType = FilterConfig['type']

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
  @Input() filterConfig: FilterConfig[] = [];
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
    this.resetForm();

  }

  private createFormControls() {
    this.filterConfig.forEach(config => {
      // if (config.type === 'date') {
      //   this.filterForm.addControl(`${config.key}_from`, this.fb.control(''));
      //   this.filterForm.addControl(`${config.key}_to`, this.fb.control(''));
      // } else {
        this.filterForm.addControl(config.key, this.fb.control(''));
      // }
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

    this.filterConfig.forEach(config => {
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

  resetForm () {
    const defaultValues = this.defaultValuesByConfig();
    this.filterForm.setValue(defaultValues);
  }

  defaultValuesByConfig (): any {
    // return key == "selectMany" ? [] : "";
    const filterConfig = this.filterConfig;
    const defaultConfig = {} as any;

    filterConfig.forEach(config => {
      if (config.type === "selectMany" ) {
        const newValues = config.options?.map(option => option.value);
        defaultConfig[config.key] = newValues;
      } else {
        defaultConfig[config.key] = "";
      }
    })

    return defaultConfig;
  }
}
