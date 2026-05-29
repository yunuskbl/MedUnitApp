import { Routes } from '@angular/router';
import { AdminGuard } from './guards/admin/admin.guard';
import { AuthGuard } from './guards/auth/auth.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/home/home.component').then((m) => m.HomeComponent),
  },
  {
    path: 'contact',
    loadComponent: () =>
      import('./pages/contact/contact.component').then((m) => m.ContactComponent),
  },
  {
    path: 'services',
    loadComponent: () =>
      import('./shared/sections/service-card/service-card.component').then((m) => m.ServiceCardComponent),
  },
  {
    path: 'pages',
    loadComponent: () =>
      import('./shared/sections/process-section/process-section.component').then((m) => m.ProcessSectionComponent),
  },
  {
    path: 'projects',
    loadComponent: () =>
      import('./shared/sections/project-section/project-section.component').then((m) => m.ProjectSectionComponent)
  },
  {
    path: 'about',
    loadComponent: () =>
      import('./shared/sections/about-section/about-section.component').then((m) => m.AboutSectionComponent),
  },
  {
    path: 'admin',
    loadComponent: () =>
      import('./pages/admin/admin.component').then(m => m.AdminComponent),
    canActivate: [AdminGuard]
  },
  {
    path: 'dashboard',
    loadComponent: () =>
      import('./pages/dashboard/dashboard.component').then(m => m.DashboardComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'sifremi-unuttum',
    loadComponent: () =>
      import('./pages/forgot-password/forgot-password.component').then(m => m.ForgotPasswordComponent),
  },
  {
    path: 'sifre-yenile',
    loadComponent: () =>
      import('./pages/reset-password/reset-password.component').then(m => m.ResetPasswordComponent),
  },
  {
    path: 'kvkk',
    loadComponent: () =>
      import('./pages/kvkk/kvkk.component').then(m => m.KvkkComponent),
  },
  {
    path: 'fiyatlandirma',
    loadComponent: () =>
      import('./pages/pricing/pricing.component').then(m => m.PricingComponent),
  }
];

