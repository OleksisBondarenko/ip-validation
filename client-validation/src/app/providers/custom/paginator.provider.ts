import {MatPaginatorIntl} from "@angular/material/paginator";
import {CustomMatPaginatorIntl} from "./CustomMatPaginatorIntl";

export default function provideMatPaginator () {
  return {
    provide: MatPaginatorIntl,
    useClass: CustomMatPaginatorIntl
  };
}
