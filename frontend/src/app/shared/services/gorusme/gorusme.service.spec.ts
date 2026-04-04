import { TestBed } from '@angular/core/testing';

import { GorusmeService } from './gorusme.service';

describe('GorusmeService', () => {
  let service: GorusmeService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GorusmeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
