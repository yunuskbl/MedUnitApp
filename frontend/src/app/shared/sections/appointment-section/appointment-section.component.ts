import {
  Component,
  Inject,
  OnDestroy,
  OnInit,
  PLATFORM_ID,
  ViewEncapsulation
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { isPlatformBrowser } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SignalrService } from '../../services/signalr/signalr.service';
import { Subscription } from 'rxjs';

export interface Doktor {
  id: number;
  ad: string;
  soyad: string;
}

export interface Randevu {
  id: number;
  hastaAd: string;
  doktorAd: string;
  baslangicTarihi: string;
  bitisTarihi: string;
  durum: string;
  notlar: string;
  zoomJoinUrl?: string;
  zoomHostUrl?: string;
}
@Component({
  standalone: true,
  selector: 'app-appointment-section',
  encapsulation: ViewEncapsulation.None,
  imports: [CommonModule, FormsModule],
  templateUrl: './appointment-section.component.html',
  styleUrl: './appointment-section.component.css',
})
export class AppointmentSectionComponent implements OnInit, OnDestroy{
  bugun(): string {
    return new Date().toISOString().split('T')[0];
  }
  
  // Form alanları
  secilenDoktorId: number = 0;
  secilenTarih: string = '';
  secilenSaat: string = '';
  notlar: string = '';

  // Durum
  doktorlar: Doktor[] = [];
  randevular: Randevu[] = [];
  yukleniyor = false;
  basari = '';
  hata = '';

  // Zoom modal
  modalAcik = false;
  modalRandevu: Randevu | null = null;
  zoomLink = '';

  // Giriş durumu
  token = '';
  rol = '';
  isBrowser = false;

  acikKartId: number | null = null;
  kartToggle(id: number): void {
    this.acikKartId = this.acikKartId === id ? null : id;
    const r = this.randevular.find((x) => x.id === id);
    console.log('Randevu durum:', r?.durum);
    console.log('Rol:', this.rol);
  }
  private apiUrl = 'https://medunitapp.onrender.com/api';
  private bildirimSub!: Subscription;
  constructor(
    private http: HttpClient,
    private signalrService: SignalrService,
    @Inject(PLATFORM_ID) private platformId: Object,
  ) {}

  ngOnInit(): void {
    this.isBrowser = isPlatformBrowser(this.platformId);
    if (this.isBrowser) {
      this.token = localStorage.getItem('token') || '';
      this.rol = (localStorage.getItem('rol') || '').toLowerCase();

      this.doktorlariGetir();
      if (this.token) {
        this.randevulariGetir();
        this.signalrDinle();
      }
    }
  }

  
  private signalrDinle(): void {
    this.bildirimSub = this.signalrService.bildirim$.subscribe((bildirim) => {
      // Hem onay hem yeni randevu gelince listeyi yenile
      if (bildirim.tip === 'onay' || bildirim.tip === 'doktorHazir') {
        this.randevulariGetir();

        // Bildirimi göster
        this.basari = bildirim.mesaj;
        setTimeout(() => (this.basari = ''), 4000);
      }
    });
  }
  ngOnDestroy(): void {
    this.bildirimSub?.unsubscribe();
  }

  private headers(): HttpHeaders {
    // Her seferinde localStorage'dan taze oku
    const token = isPlatformBrowser(this.platformId)
      ? localStorage.getItem('token') || ''
      : '';
    return new HttpHeaders({ Authorization: `Bearer ${this.token}` });
  }
  doktorlariGetir(): void {
    // Kayıtlı doktorları çek — endpoint yoksa statik liste kullan
    this.http.get<Doktor[]>(`${this.apiUrl}/kullanici/doktorlar`).subscribe({
      next: (data) => (this.doktorlar = data),
      error: () => {
        // Backend henüz hazır değilse geçici statik liste
        this.doktorlar = [
          { id: 1, ad: 'Ahmet', soyad: 'Yılmaz' },
          { id: 2, ad: 'Ayşe', soyad: 'Kaya' },
        ];
      },
    });
  }

  randevuOnayla(id: number): void {
    const token = localStorage.getItem('token');
    if (!token) return;

    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });

    this.http
      .put(
        `${this.apiUrl}/randevu/${id}`,
        { durum: 'onaylandi', notlar: '' },
        { headers },
      )
      .subscribe({
        next: () => {
          this.basari = 'Randevu onaylandı!';
          this.randevulariGetir();
        },
        error: () => (this.hata = 'Onaylama işlemi başarısız.'),
      });
  }

  randevulariGetir(): void {
    if (!isPlatformBrowser(this.platformId)) return;

    const token = localStorage.getItem('token');
    if (!token) return;

    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });

    this.http.get<Randevu[]>(`${this.apiUrl}/randevu`, { headers }).subscribe({
      next: (data) => {
        // İptal olanları gizle
        this.randevular = data
          .filter((r) => r.durum.toLowerCase() !== 'iptal')
          .map((r) => ({ ...r, durum: r.durum.toLowerCase() }));
        this.randevular.forEach((r) => {
          this.signalrService.odayaKatil(r.id.toString());
        });
      },
      error: () => (this.randevular = []),
    });
  }

  randevuOlustur(): void {
    // Token'ı direkt localStorage'dan al
    const token = localStorage.getItem('token');
    if (!this.token) {
      this.hata = 'Randevu almak için giriş yapmanız gerekiyor.';
      return;
    }

    if (!this.secilenDoktorId || !this.secilenTarih || !this.secilenSaat) {
      this.hata = 'Lütfen tüm alanları doldurun.';
      return;
    }

    this.yukleniyor = true;
    this.hata = '';
    this.basari = '';

    const baslangic = new Date(`${this.secilenTarih}T${this.secilenSaat}:00`);
    const bitis = new Date(baslangic.getTime() + 45 * 60000);
    const pad = (n: number) => n.toString().padStart(2, '0');
    const formatLocal = (d: Date) =>
      `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}:00`;
    const body = {
      doktorId: this.secilenDoktorId,
      baslangicTarihi: formatLocal(baslangic),
      bitisTarihi: formatLocal(bitis),
      notlar: this.notlar,
    };
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json',
    });

    this.http
      .post<Randevu>(`${this.apiUrl}/randevu`, body, {
        headers: this.headers(),
      })
      .subscribe({
        next: () => {
          this.basari = 'Randevunuz başarıyla oluşturuldu!';
          this.yukleniyor = false;
          this.formuSifirla();
          this.randevulariGetir();
        },
        error: (err) => {
          this.hata = err.error?.message || 'Randevu oluşturulamadı.';
          this.yukleniyor = false;
        },
      });
  }

  randevuIptal(id: number): void {
    const token = localStorage.getItem('token');
    if (!token) return;

    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });

    // Rol hasta ise farklı endpoint kullan
    const url =
      this.rol === 'hasta'
        ? `${this.apiUrl}/randevu/${id}/iptal`
        : `${this.apiUrl}/randevu/${id}`;

    const request =
      this.rol === 'hasta'
        ? this.http.put(url, {}, { headers })
        : this.http.put(url, { durum: 'iptal', notlar: '' }, { headers });

    request.subscribe({
      next: () => {
        this.basari = 'Randevu iptal edildi.';
        this.acikKartId = null;
        this.randevulariGetir();
      },
      error: (err) => {
        this.hata = err.error?.message || 'İptal işlemi başarısız.';
      },
    });
  }

  zoomModalAc(randevu: Randevu): void {
    this.modalRandevu = randevu;
    this.modalAcik = true;
    this.zoomLink = '';

    const token = localStorage.getItem('token');
    if (!token) return;

    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });

    // Güncel randevu bilgisini al — link DB'de var mı kontrol et
    this.http
      .get<any>(`${this.apiUrl}/zoom/meeting-bilgisi/${randevu.id}`, {
        headers,
      })
      .subscribe({
        next: (data) => {
          // Doktor host linki, hasta join linki kullanır
          this.zoomLink = this.rol === 'doktor' ? data.hostUrl : data.joinUrl;
        },
        error: () => {
          this.hata = 'Zoom linki alınamadı.';
          this.modalAcik = false;
        },
      });
  }
  zoomAc(): void {
    if (this.zoomLink) window.open(this.zoomLink, '_blank');
  }

  modalKapat(): void {
    this.modalAcik = false;
    this.modalRandevu = null;
    this.zoomLink = '';
  }

  durumRengi(durum: string): string {
    switch (durum) {
      case 'onaylandi':
        return 'durum-onay';
      case 'iptal':
        return 'durum-iptal';
      default:
        return 'durum-bekle';
    }
  }

  durumMetni(durum: string): string {
    switch (durum) {
      case 'onaylandi':
        return 'Onaylandı';
      case 'iptal':
        return 'İptal';
      default:
        return 'Beklemede';
    }
  }

  gorusmeAktifMi(randevu: Randevu): boolean {
    if (randevu.durum !== 'onaylandi') return false;
    const simdi = new Date();
    const baslangic = new Date(randevu.baslangicTarihi);
    const fark = (baslangic.getTime() - simdi.getTime()) / 60000; // dakika
    return fark <= 10 && fark >= -60; // 10 dk önce aktif olur, 60 dk sonra kapanır
  }

  private formuSifirla(): void {
    this.secilenDoktorId = 0;
    this.secilenTarih = '';
    this.secilenSaat = '';
    this.notlar = '';
  }
}

