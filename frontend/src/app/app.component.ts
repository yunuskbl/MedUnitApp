import { Component, Inject, OnInit, PLATFORM_ID,AfterViewInit } from '@angular/core';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { HeaderComponent } from './components/header/header.component';
import { FooterComponent } from "./components/footer/footer.component";
import { SignalrService } from './shared/services/signalr/signalr.service';
import { isPlatformBrowser, NgIf } from '@angular/common';
import { BildirimComponent } from './shared/bildirim/bildirim.component';
import { filter } from 'rxjs';


@Component({
  selector: 'app-root',
  imports: [HeaderComponent, RouterOutlet, FooterComponent, HeaderComponent, BildirimComponent, NgIf],
  standalone: true,
  template: `
    <ng-container *ngIf="!adminSayfasi">
      <app-header></app-header>
      <app-bildirim></app-bildirim>
    </ng-container>
    <router-outlet></router-outlet>
    <ng-container *ngIf="!adminSayfasi">
      <app-footer></app-footer>
    </ng-container>
  `,
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit, AfterViewInit {

    adminSayfasi = false;

  constructor(
    private router: Router,
    private signalrService: SignalrService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  ngOnInit(): void {
    // Route değişince admin sayfası mı kontrol et
    this.router.events.pipe(
      filter(e => e instanceof NavigationEnd)
    ).subscribe((e: any) => {
      this.adminSayfasi = e.url.startsWith('/admin');
    });

    if (isPlatformBrowser(this.platformId)) {
      const token = localStorage.getItem('token');
      const kullaniciId = localStorage.getItem('kullaniciId');
      if (token && kullaniciId) {
        this.signalrService.baslat(token, kullaniciId);
      }
    }
  }
  ngAfterViewInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      // Backend çalışıyorsa SignalR başlat
      const token = localStorage.getItem('token');
      const kullaniciId = localStorage.getItem('kullaniciId');

      if (token && kullaniciId) {
        // 2 saniye bekle — hydration tamamlansın
        setTimeout(() => {
          this.signalrService.baslat(token, kullaniciId);
        }, 2000);
      }
    }
  }

}

