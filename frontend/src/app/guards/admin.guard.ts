import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { isPlatformBrowser } from '@angular/common';

@Injectable({ providedIn: 'root' })
export class AdminGuard implements CanActivate {

  constructor(
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  canActivate(): boolean {
    if (!isPlatformBrowser(this.platformId)) return false;

    const rol = localStorage.getItem('rol')?.toLowerCase();
    const token = localStorage.getItem('token');

    if (token && rol === 'admin') return true;

    this.router.navigate(['/']);
    return false;
  }
}