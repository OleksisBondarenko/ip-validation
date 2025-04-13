import { TestBed } from '@angular/core/testing';

import { AuditRepoService } from './audit-repo.service';

describe('AuditRepoService', () => {
  let service: AuditRepoService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AuditRepoService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
