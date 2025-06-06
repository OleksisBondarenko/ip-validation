import {FilterConfig} from "../app/ui-common/filter/filter.component";

export interface Environment {
  apiUrl: string;
  featureFlag: boolean;
  filterConfig: FilterConfig [];
  production: boolean;
}
