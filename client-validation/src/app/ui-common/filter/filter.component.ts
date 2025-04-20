// filter.component.ts
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import {FormBuilder, FormGroup, ReactiveFormsModule} from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import {MatError, MatFormField, MatHint, MatLabel} from "@angular/material/form-field";
import {CommonModule, NgSwitch} from "@angular/common";
import {MatOption, MatSelect} from "@angular/material/select";
import {
  MatDatepicker,
  MatDatepickerInput,
  MatDatepickerToggle,
  MatDateRangeInput,
  MatDateRangePicker
} from "@angular/material/datepicker";
import {MatInput} from "@angular/material/input";
import {MatButton, MatButtonModule} from "@angular/material/button";

export interface FilterConfig {
  type: 'text' | 'date' | 'select';
  key: string;
  label: string;
  options?: { value: any; label: string }[];
}

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
    MatDatepickerToggle,
    MatLabel,
    MatInput,
    MatButton,
    MatButtonModule,
    MatDatepickerToggle,
    CommonModule,
    MatDateRangeInput,
    MatDateRangePicker,
    MatHint,
    MatError
  ],
  styleUrls: ['./filter.component.scss']
})
export class FilterComponent implements OnInit {
  @Input() filterConfig: FilterConfig[] = [];
  @Output() filterChanged = new EventEmitter<any>();
  @Output() filterReset = new EventEmitter<void>();
  @Output() filterApply = new EventEmitter<void>();

  filterForm: FormGroup;

  constructor(private fb: FormBuilder) {
    this.filterForm = this.fb.group({});
  }

  ngOnInit() {
    this.createFormControls();
    this.setupFilterChanges();
  }

  private createFormControls() {
    this.filterConfig.forEach(config => {
      if (config.type === 'date') {
        this.filterForm.addControl(`${config.key}_from`, this.fb.control(''));
        this.filterForm.addControl(`${config.key}_to`, this.fb.control(''));
      } else {
        this.filterForm.addControl(config.key, this.fb.control(''));
      }
    });
  }

  private setupFilterChanges() {
    this.filterForm.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged()
      )
      .subscribe(value => {
        this.filterChanged.emit(this.parseFilters(value));
      });
  }

  private parseFilters(formValue: any): any[] {
    const filters: any[] = [];

    this.filterConfig.forEach(config => {
      const value = formValue[config.key];
      const from = formValue[`${config.key}_from`];
      const to = formValue[`${config.key}_to`];

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
      default: return 'string';
    }
  }

  private createFilterValue(type: string, value: any, from?: any, to?: any) {
    switch(type) {
      case 'date':
        return { from: from?.toISOString(), to: to?.toISOString() };
      // case 'date':
      //   return { input: value?.toISOString(), operator: 'eq' };
      case 'select':
        return { id: value };
      default:
        return { input: value, operator: 'contains' };
    }
  }

  applyFilters () {
    this.filterApply.emit();
  }
  resetFilters() {
    this.filterForm.reset();
    this.filterReset.emit();
  }
}
