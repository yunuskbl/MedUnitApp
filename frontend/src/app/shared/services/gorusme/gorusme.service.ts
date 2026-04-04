import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
@Injectable({
  providedIn: 'root'
})
export class GorusmeService {

private hubBaglantisi!: signalR.HubConnection;

baglantiyiBaslat(token: string): Promise<void> {
    this.hubBaglantisi = new signalR.HubConnectionBuilder()
      .withUrl('https://medunitapp.onrender.com/hubs/gorusme', {
        accessTokenFactory: () => token   // JWT otomatik gönderilir
      })
      .withAutomaticReconnect()           // bağlantı kopunca otomatik yeniden bağlan
      .build();
      // Gelen mesajları dinle
    this.hubBaglantisi.on('MesajAlindi', (veri) => {
      console.log('Mesaj:', veri);
    });

    // WebRTC sinyallerini dinle
    this.hubBaglantisi.on('SinyalAlindi', (veri) => {
      console.log('Sinyal:', veri);
    });

    this.hubBaglantisi.on('KullaniciBaglandi', (connectionId) => {
      console.log('Karşı taraf bağlandı:', connectionId);
    });

    return this.hubBaglantisi.start();
  }

  odayaKatil(randevuId: string): Promise<void> {
    return this.hubBaglantisi.invoke('OdayaKatil', randevuId);
  }

  mesajGonder(randevuId: string, mesaj: string): Promise<void> {
    return this.hubBaglantisi.invoke('MesajGonder', randevuId, mesaj);
  }

  sinyalGonder(randevuId: string, tip: string, veri: string): Promise<void> {
    return this.hubBaglantisi.invoke('SinyalGonder', randevuId, tip, veri);
  }

  baglantiyiKes(): Promise<void> {
    return this.hubBaglantisi.stop();
  }

}

