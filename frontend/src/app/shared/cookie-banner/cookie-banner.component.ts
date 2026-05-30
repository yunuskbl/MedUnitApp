import { Component, Inject, OnInit, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-cookie-banner',
  imports: [CommonModule, RouterModule],
  template: `
    <div class="cookie-bant" *ngIf="goster">
      <div class="cookie-icerik">
        <p>
          <i class="fas fa-cookie-bite"></i>
          Bu site, daha iyi bir deneyim sunmak için çerezler kullanmaktadır.
          <a routerLink="/kvkk">KVKK Metnini</a> okuyabilirsiniz.
        </p>
        <div class="cookie-butonlar">
          <button class="btn-kabul" (click)="kabul()">Kabul Et</button>
          <button class="btn-reddet" (click)="reddet()">Reddet</button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .cookie-bant {
      position: fixed;
      bottom: 0;
      left: 0;
      right: 0;
      background: #1a1a2e;
      color: white;
      z-index: 99998;
      animation: slideUp 0.4s ease;
    }

    @keyframes slideUp {
      from { transform: translateY(100%); }
      to   { transform: translateY(0); }
    }

    .cookie-icerik {
      max-width: 1100px;
      margin: 0 auto;
      padding: 16px 24px;
      display: flex;
      align-items: center;
      justify-content: space-between;
      gap: 20px;
      flex-wrap: wrap;
    }

    .cookie-icerik p {
      margin: 0;
      font-size: 14px;
      color: #cbd5e1;
      line-height: 1.6;
      flex: 1;
      min-width: 260px;
    }

    .cookie-icerik i {
      color: #f59e0b;
      margin-right: 8px;
    }

    .cookie-icerik a {
      color: #818cf8;
      text-decoration: underline;
      font-weight: 600;
    }

    .cookie-butonlar {
      display: flex;
      gap: 10px;
      flex-shrink: 0;
    }

    .btn-kabul {
      background: #4f46e5;
      color: white;
      border: none;
      padding: 9px 22px;
      border-radius: 8px;
      font-size: 13px;
      font-weight: 700;
      cursor: pointer;
      transition: background 0.2s;
    }

    .btn-kabul:hover { background: #4338ca; }

    .btn-reddet {
      background: transparent;
      color: #94a3b8;
      border: 1px solid #334155;
      padding: 9px 22px;
      border-radius: 8px;
      font-size: 13px;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.2s;
    }

    .btn-reddet:hover { border-color: #94a3b8; color: white; }
  `]
})
export class CookieBannerComponent implements OnInit {
  goster = false;

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {}

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.goster = !localStorage.getItem('cookieConsent');
    }
  }

  kabul(): void {
    localStorage.setItem('cookieConsent', 'accepted');
    this.goster = false;
  }

  reddet(): void {
    localStorage.setItem('cookieConsent', 'rejected');
    this.goster = false;
  }
}
