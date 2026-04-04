import { Injectable, Inject, PLATFORM_ID, NgZone } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';

export interface Bildirim {
  tip: 'onay' | 'doktorHazir' | 'mesaj';
  mesaj: string;
  randevuId?: number;
  zoomLink?: string;
}

@Injectable({ providedIn: 'root' })
export class SignalrService {
  private hub!: signalR.HubConnection;
  private apiUrl = 'https://medunitapp.onrender.com';

  bildirim$ = new Subject<Bildirim>();
  baglandi$ = new Subject<void>();

  constructor(
    @Inject(PLATFORM_ID) private platformId: Object,
    private ngZone: NgZone
  ) {}

  baslat(token: string, kullaniciId: string): void {
    if (!isPlatformBrowser(this.platformId)) return;
    if (this.hub?.state === signalR.HubConnectionState.Connected) return;

    this.ngZone.runOutsideAngular(() => {
      fetch(`${this.apiUrl}/health`)
        .then(() => this.baglantiKur(token, kullaniciId))
        .catch(() => {});
    });
  }

  private baglantiKur(token: string, kullaniciId: string): void {
    this.ngZone.runOutsideAngular(() => {
      this.hub = new signalR.HubConnectionBuilder()
        .withUrl(`${this.apiUrl}/hubs/gorusme`, {
          accessTokenFactory: () => token,
          withCredentials: true
        })
        .build();

      this.hub.on('RandevuGuncellendi', (veri) => {
        this.ngZone.run(() => {
          this.bildirim$.next({
            tip: 'onay',
            mesaj: veri.mesaj,
            randevuId: veri.randevuId
          });
        });
      });

      this.hub.on('DoktorHazir', (veri) => {
        this.ngZone.run(() => {
          this.bildirim$.next({
            tip: 'doktorHazir',
            mesaj: 'Doktor görüşmeye hazır!',
            randevuId: veri.randevuId,
            zoomLink: veri.zoomLink
          });
        });
      });

      this.hub.on('MesajAlindi', (veri) => {
        this.ngZone.run(() => {
          this.bildirim$.next({
            tip: 'mesaj',
            mesaj: veri.mesaj
          });
        });
      });

      this.hub.start()
        .then(() => {
          this.ngZone.run(() => {
            this.hub.invoke('KullaniciyiKaydet', kullaniciId);
            this.baglandi$.next();
          });
        })
        .catch(() => {});
    });
  }

  odayaKatil(randevuId: string): void {
    if (this.hub?.state === signalR.HubConnectionState.Connected) {
      this.hub.invoke('OdayaKatil', randevuId);
    }
  }

  doktorHazir(randevuId: string, zoomLink: string): void {
    if (this.hub?.state === signalR.HubConnectionState.Connected) {
      this.hub.invoke('DoktorHazir', randevuId, zoomLink);
    }
  }

  durdur(): void {
    this.hub?.stop();
  }
}
