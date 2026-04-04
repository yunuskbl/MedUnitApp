import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';

interface Istatistik {
  toplamKullanici: number;
  toplamHasta: number;
  toplamDoktor: number;
  toplamRandevu: number;
  bekleyenRandevu: number;
  onayliRandevu: number;
  iptalRandevu: number;
}

interface Kullanici {
  id: number;
  ad: string;
  soyad: string;
  email: string;
  rol: string;
  aktif: boolean;
  olusturulmaTarihi: string;
}

interface Randevu {
  id: number;
  hastaAd: string;
  doktorAd: string;
  baslangicTarihi: string;
  bitisTarihi: string;
  durum: string;
  notlar: string;
}

@Component({
  standalone: true,
  selector: 'app-admin',
  imports: [CommonModule],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.css'
})
export class AdminComponent implements OnInit {

  aktifSekme: 'istatistik' | 'kullanicilar' | 'randevular' = 'istatistik';
  istatistik: Istatistik | null = null;
  kullanicilar: Kullanici[] = [];
  randevular: Randevu[] = [];
  yukleniyor = false;
  mesaj = '';

  private apiUrl = 'http://localhost:5227/api';

  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.istatistikleriGetir();
    }
  }

  private headers(): HttpHeaders {
    const token = localStorage.getItem('token') || '';
    return new HttpHeaders({ Authorization: `Bearer ${token}` });
  }

  sekmeDegistir(sekme: 'istatistik' | 'kullanicilar' | 'randevular'): void {
    this.aktifSekme = sekme;
    if (sekme === 'istatistik') this.istatistikleriGetir();
    if (sekme === 'kullanicilar') this.kullanicilariGetir();
    if (sekme === 'randevular') this.randevulariGetir();
  }

  istatistikleriGetir(): void {
    this.http.get<Istatistik>(`${this.apiUrl}/admin/istatistikler`,
      { headers: this.headers() })
      .subscribe({ next: (d) => this.istatistik = d });
  }

  kullanicilariGetir(): void {
    this.yukleniyor = true;
    this.http.get<Kullanici[]>(`${this.apiUrl}/admin/kullanicilar`,
      { headers: this.headers() })
      .subscribe({
        next: (d) => { this.kullanicilar = d; this.yukleniyor = false; }
      });
  }

  randevulariGetir(): void {
    this.yukleniyor = true;
    this.http.get<Randevu[]>(`${this.apiUrl}/admin/randevular`,
      { headers: this.headers() })
      .subscribe({
        next: (d) => { this.randevular = d; this.yukleniyor = false; }
      });
  }

  aktifDegistir(id: number): void {
    this.http.put(`${this.apiUrl}/admin/kullanici/${id}/aktif`, {},
      { headers: this.headers() })
      .subscribe({
        next: () => {
          this.mesaj = 'Kullanıcı durumu güncellendi.';
          this.kullanicilariGetir();
          setTimeout(() => this.mesaj = '', 3000);
        }
      });
  }

  rolDegistir(id: number, yeniRol: string): void {
    this.http.put(`${this.apiUrl}/admin/kullanici/${id}/rol`,
      JSON.stringify(yeniRol),
      { headers: new HttpHeaders({
          Authorization: `Bearer ${localStorage.getItem('token')}`,
          'Content-Type': 'application/json'
        })
      })
      .subscribe({
        next: () => {
          this.mesaj = 'Rol güncellendi.';
          this.kullanicilariGetir();
          setTimeout(() => this.mesaj = '', 3000);
        }
      });
  }

  randevuSil(id: number): void {
    if (!confirm('Randevuyu silmek istediğinizden emin misiniz?')) return;

    this.http.delete(`${this.apiUrl}/admin/randevu/${id}`,
      { headers: this.headers() })
      .subscribe({
        next: () => {
          this.mesaj = 'Randevu silindi.';
          this.randevulariGetir();
          setTimeout(() => this.mesaj = '', 3000);
        }
      });
  }

  durumRengi(durum: string): string {
    switch (durum.toLowerCase()) {
      case 'onaylandi': return 'badge-onay';
      case 'iptal':     return 'badge-iptal';
      default:          return 'badge-bekle';
    }
  }
}