import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';

import { doktorGuard } from './doktor.guard';

describe('doktorGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) => 
      TestBed.runInInjectionContext(() => doktorGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
