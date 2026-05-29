import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { FormsModule } from '@angular/forms';
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
  uzmanlik?: string;
  telefon?: string;
  aktif: boolean;
  klinikId?: number;
  klinikAd?: string;
  olusturulmaTarihi: string;
  uzmanlikDuzenle?: boolean;
  yeniUzmanlik?: string;
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

interface ContactMesaj {
  id: number;
  name: string;
  lastName: string;
  email: string;
  phone: string;
  message: string;
  createdAt: string;
  isRead: boolean;
}

interface Klinik {
  id: number;
  ad: string;
  adres?: string;
  telefon?: string;
  email?: string;
  abonelikTipi: string;
  aktif: boolean;
  olusturulmaTarihi: string;
  uyeSayisi: number;
  duzenlemeModu?: boolean;
}

@Component({
  standalone: true,
  selector: 'app-admin',
  imports: [CommonModule, FormsModule],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.css'
})
export class AdminComponent implements OnInit {

  aktifSekme: 'istatistik' | 'kullanicilar' | 'randevular' | 'mesajlar' | 'klinikler' = 'istatistik';

  contactMesajlar: ContactMesaj[] = [];
  istatistik: Istatistik | null = null;
  kullanicilar: Kullanici[] = [];
  randevular: Randevu[] = [];
  klinikler: Klinik[] = [];

  // Yeni klinik formu
  yeniKlinikAd = '';
  yeniKlinikAdres = '';
  yeniKlinikTelefon = '';
  yeniKlinikEmail = '';
  yeniKlinikAbonelik = 'ucretsiz';
  klinikFormAcik = false;

  yukleniyor = false;
  mesaj = '';

  private apiUrl = 'https://medunitapp.onrender.com/api';

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

  sekmeDegistir(sekme: 'istatistik' | 'kullanicilar' | 'randevular' | 'mesajlar' | 'klinikler'): void {
    this.aktifSekme = sekme;
    if (sekme === 'istatistik') this.istatistikleriGetir();
    if (sekme === 'kullanicilar') this.kullanicilariGetir();
    if (sekme === 'randevular') this.randevulariGetir();
    if (sekme === 'mesajlar') this.mesajlariGetir();
    if (sekme === 'klinikler') this.klinikleriGetir();
  }

  istatistikleriGetir(): void {
    this.http.get<Istatistik>(`${this.apiUrl}/admin/istatistikler`,
      { headers: this.headers() })
      .subscribe({ next: (d) => this.istatistik = d });
  }

  kullanicilariGetir(): void {
    this.yukleniyor = true;
    if (this.klinikler.length === 0) this.klinikleriGetir();
    this.http.get<Kullanici[]>(`${this.apiUrl}/admin/kullanicilar`,
      { headers: this.headers() })
      .subscribe({
        next: (d) => {
          this.kullanicilar = d.map((u) => ({
            ...u,
            rol: ((u as any).rol ?? (u as any).role ?? '').toString().toLowerCase(),
            uzmanlikDuzenle: false,
            yeniUzmanlik: u.uzmanlik || ''
          }));
          this.yukleniyor = false;
        }
      });
  }

  uzmanlikKaydet(k: Kullanici): void {
    this.http.put(`${this.apiUrl}/admin/kullanici/${k.id}/uzmanlik`,
      { uzmanlik: k.yeniUzmanlik },
      { headers: this.headers() })
      .subscribe({
        next: () => {
          k.uzmanlik = k.yeniUzmanlik;
          k.uzmanlikDuzenle = false;
          this.mesaj = 'Uzmanlık güncellendi.';
          setTimeout(() => this.mesaj = '', 3000);
        }
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
      { rol: yeniRol },
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

  klinikAta(kullaniciId: number, klinikIdStr: string): void {
    const klinikId = parseInt(klinikIdStr, 10) || 0;
    this.http.put(`${this.apiUrl}/admin/kullanici/${kullaniciId}/klinik`,
      { klinikId },
      { headers: this.headers() })
      .subscribe({
        next: () => {
          this.mesaj = 'Klinik atandı.';
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

  mesajlariGetir(): void {
    this.yukleniyor = true;
    this.http.get<ContactMesaj[]>(`${this.apiUrl}/contact`, { headers: this.headers() })
      .subscribe({
        next: (d) => { this.contactMesajlar = d; this.yukleniyor = false; }
      });
  }

  mesajOkundu(id: number): void {
    this.http.put(`${this.apiUrl}/contact/${id}/okundu`, {}, { headers: this.headers() })
      .subscribe({
        next: () => {
          const m = this.contactMesajlar.find(x => x.id === id);
          if (m) m.isRead = true;
        }
      });
  }

  klinikleriGetir(): void {
    this.yukleniyor = true;
    this.http.get<Klinik[]>(`${this.apiUrl}/klinik`, { headers: this.headers() })
      .subscribe({
        next: (d) => { this.klinikler = d; this.yukleniyor = false; },
        error: () => this.yukleniyor = false
      });
  }

  klinikOlustur(): void {
    if (!this.yeniKlinikAd.trim()) return;

    this.http.post(`${this.apiUrl}/klinik`,
      {
        ad: this.yeniKlinikAd,
        adres: this.yeniKlinikAdres,
        telefon: this.yeniKlinikTelefon,
        email: this.yeniKlinikEmail,
        abonelikTipi: this.yeniKlinikAbonelik
      },
      { headers: this.headers() })
      .subscribe({
        next: () => {
          this.mesaj = 'Klinik oluşturuldu.';
          this.klinikFormAcik = false;
          this.yeniKlinikAd = '';
          this.yeniKlinikAdres = '';
          this.yeniKlinikTelefon = '';
          this.yeniKlinikEmail = '';
          this.klinikleriGetir();
          setTimeout(() => this.mesaj = '', 3000);
        }
      });
  }

  abonelikRengi(tip: string): string {
    switch (tip) {
      case 'premium': return 'badge-onay';
      case 'temel': return 'badge-bekle';
      default: return '';
    }
  }
}
