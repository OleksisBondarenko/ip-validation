import { TestBed } from '@angular/core/testing';

import { ValidationAuditDataService } from './validation-audit-data.service';

describe('ValidationAuditDataService', () => {
  let service: ValidationAuditDataService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ValidationAuditDataService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
