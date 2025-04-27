import {Injectable} from '@angular/core';
import {MatPaginatorIntl} from "@angular/material/paginator";

@Injectable()
export class CustomMatPaginatorIntl extends MatPaginatorIntl {
  constructor() {
    super();

    this.getAndInitTranslations();
  }

  getAndInitTranslations() {
      this.itemsPerPageLabel = "К-ть рядків на сторінці";
      this.nextPageLabel = "Наступна сторінка";
      this.previousPageLabel = "Минула сторінка";
      this.firstPageLabel = "На початок";
      this.lastPageLabel = "В кінець";
      this.changes.next();
  }

 override getRangeLabel = (page: number, pageSize: number, length: number) =>  {
    if (length === 0 || pageSize === 0) {
      return `0 з ${length}`;
    }

    length = Math.max(length, 0);
    const startIndex = page * pageSize;
    const endIndex = startIndex < length ? Math.min(startIndex + pageSize, length) : startIndex + pageSize;
    return `${startIndex + 1} - ${endIndex} з ${length}`;
  }
}
