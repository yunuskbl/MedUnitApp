import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { ProjectSectionComponent } from './shared/sections/project-section/project-section.component';
import { AboutSectionComponent } from './shared/sections/about-section/about-section.component';
import { AdminGuard } from './guards/admin.guard';

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
  { path: 'about', 
    loadComponent: () =>
      import('./shared/sections/about-section/about-section.component').then((m) => m.AboutSectionComponent),
  },
  {
    path: 'admin',
    loadComponent: () =>
      import('./pages/admin/admin.component').then(m => m.AdminComponent),
    canActivate: [AdminGuard]
  }
];
