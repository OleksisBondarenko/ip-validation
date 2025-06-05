import { TestBed } from '@angular/core/testing';

import { AuditFilterService } from './audit-filter.service';

describe('AuditFilterService', () => {
  let service: AuditFilterService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AuditFilterService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
