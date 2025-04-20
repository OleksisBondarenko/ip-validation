// filter.models.ts
export interface Filter {
  type: string;
  alias: string;
  value: FilterValue;
  strict?: boolean;
  starts?: boolean;
  ends?: boolean;
}

export interface FilterValue {
  input?: string;
  operator?: string;
  from?: string | Date;
  to?: string | Date;
  id?: number;
  label?: string;
}

export interface FilterRequest {
  filters: Filter[];
  orderBy?: string;
  orderByDir?: string;
  limit: number;
  start: number;
  search?: string;
}
