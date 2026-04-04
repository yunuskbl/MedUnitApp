import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';
import { Bildirim, SignalrService} from '../services/signalr/signalr.service';

@Component({
  standalone: true,
  selector: 'app-bildirim',
  imports: [CommonModule],
  template: `
    <div class="bildirim-konteyner">
      <div class="bildirim-kart" *ngFor="let b of bildirimler"
           [ngClass]="'tip-' + b.tip">
        <div class="bildirim-ust">
          <i class="fas" [ngClass]="{
            'fa-check-circle': b.tip === 'onay',
            'fa-video': b.tip === 'doktorHazir',
            'fa-comment': b.tip === 'mesaj'
          }"></i>
          <span class="bildirim-baslik">
            {{ b.tip === 'onay' ? 'Randevu Onaylandı' :
               b.tip === 'doktorHazir' ? 'Doktor Hazır!' : 'Yeni Mesaj' }}
          </span>
          <button class="kapat" (click)="kapat(b)">✕</button>
        </div>
        <p class="bildirim-mesaj">{{ b.mesaj }}</p>
        <button class="zoom-btn" *ngIf="b.zoomLink" (click)="zoomAc(b.zoomLink!)">
          <i class="fas fa-video"></i> Zoom'da Aç
        </button>
      </div>
    </div>
  `,
  styles: [`
    .bildirim-konteyner {
      position: fixed;
      top: 80px;
      right: 20px;
      z-index: 99999;
      display: flex;
      flex-direction: column;
      gap: 12px;
      max-width: 340px;
    }

    .bildirim-kart {
      background: white;
      border-radius: 12px;
      padding: 16px;
      box-shadow: 0 8px 32px rgba(0,0,0,0.15);
      border-left: 4px solid #3d7a6f;
      animation: slideIn 0.3s ease;
    }

    @keyframes slideIn {
      from { transform: translateX(100%); opacity: 0; }
      to   { transform: translateX(0);    opacity: 1; }
    }

    .tip-onay       { border-left-color: #22c55e; }
    .tip-doktorHazir { border-left-color: #2D8CFF; }
    .tip-mesaj      { border-left-color: #e8724a; }

    .bildirim-ust {
      display: flex;
      align-items: center;
      gap: 8px;
      margin-bottom: 8px;
    }

    .bildirim-ust i      { font-size: 16px; color: #3d7a6f; }
    .tip-onay i          { color: #22c55e; }
    .tip-doktorHazir i   { color: #2D8CFF; }
    .tip-mesaj i         { color: #e8724a; }

    .bildirim-baslik {
      font-weight: 600;
      font-size: 14px;
      color: #1a1a2e;
      flex: 1;
    }

    .kapat {
      background: none;
      border: none;
      color: #94a3b8;
      cursor: pointer;
      font-size: 14px;
      padding: 0;
    }

    .bildirim-mesaj {
      font-size: 13px;
      color: #475569;
      margin: 0 0 10px;
      line-height: 1.5;
    }

    .zoom-btn {
      background: #2D8CFF;
      color: white;
      border: none;
      border-radius: 8px;
      padding: 8px 14px;
      font-size: 13px;
      font-weight: 600;
      cursor: pointer;
      width: 100%;
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 6px;
    }

    .zoom-btn:hover { background: #1a6fd4; }
  `]
})
export class BildirimComponent implements OnInit, OnDestroy {
  bildirimler: Bildirim[] = [];
  private sub!: Subscription;

  constructor(private signalrService: SignalrService) {}

  ngOnInit(): void {
    this.sub = this.signalrService.bildirim$.subscribe(b => {
      this.bildirimler.unshift(b);

      // 8 saniye sonra otomatik kapat
      setTimeout(() => this.kapat(b), 8000);
    });
  }

  kapat(b: Bildirim): void {
    this.bildirimler = this.bildirimler.filter(x => x !== b);
  }

  zoomAc(link: string): void {
    window.open(link, '_blank');
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }
}