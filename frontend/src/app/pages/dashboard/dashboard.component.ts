import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';

interface Profil {
  id: number;
  ad: string;
  soyad: string;
  email: string;
  rol: string;
  uzmanlik?: string;
}

interface Randevu {
  id: number;
  hastaAd: string;
  doktorAd: string;
  baslangicTarihi: string;
  bitisTarihi: string;
  durum: string;
  notlar?: string;
}

@Component({
  standalone: true,
  selector: 'app-dashboard',
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {

  aktifSekme: 'profil' | 'randevular' | 'sifre' = 'profil';

  profil: Profil | null = null;
  randevular: Randevu[] = [];

  // Profil düzenleme
  profilDuzenle = false;
  yeniAd = '';
  yeniSoyad = '';
  yeniUzmanlik = '';

  // Şifre değiştirme
  mevcutSifre = '';
  yeniSifre = '';
  yeniSifreTekrar = '';

  yukleniyor = false;
  basari = '';
  hata = '';

  private apiUrl = 'https://medunitapp.onrender.com/api';

  constructor(
    private http: HttpClient,
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  ngOnInit(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    if (!localStorage.getItem('token')) {
      this.router.navigate(['/']);
      return;
    }
    this.profilGetir();
    this.randevulariGetir();
  }

  private headers(): HttpHeaders {
    const token = localStorage.getItem('token') || '';
    return new HttpHeaders({ Authorization: `Bearer ${token}` });
  }

  sekmeDegistir(sekme: 'profil' | 'randevular' | 'sifre'): void {
    this.aktifSekme = sekme;
    this.basari = '';
    this.hata = '';
    this.profilDuzenle = false;
  }

  profilGetir(): void {
    this.http.get<Profil>(`${this.apiUrl}/kullanici/profil`, { headers: this.headers() })
      .subscribe({
        next: (data) => {
          this.profil = data;
          this.yeniAd = data.ad;
          this.yeniSoyad = data.soyad;
          this.yeniUzmanlik = data.uzmanlik || '';
        },
        error: () => this.router.navigate(['/'])
      });
  }

  randevulariGetir(): void {
    this.http.get<Randevu[]>(`${this.apiUrl}/randevu`, { headers: this.headers() })
      .subscribe({
        next: (data) => this.randevular = data.map(r => ({ ...r, durum: r.durum.toLowerCase() })),
        error: () => this.randevular = []
      });
  }

  profilKaydet(): void {
    if (!this.yeniAd.trim() || !this.yeniSoyad.trim()) {
      this.hata = 'Ad ve soyad boş bırakılamaz.';
      return;
    }
    this.yukleniyor = true;
    this.hata = '';

    const body: any = { ad: this.yeniAd, soyad: this.yeniSoyad };
    if (this.profil?.rol === 'doktor') body['uzmanlik'] = this.yeniUzmanlik;

    this.http.put<any>(`${this.apiUrl}/kullanici/profil`, body, { headers: this.headers() })
      .subscribe({
        next: (data) => {
          this.profil = { ...this.profil!, ad: data.ad, soyad: data.soyad, uzmanlik: data.uzmanlik };
          localStorage.setItem('kullaniciAd', data.ad);
          this.basari = 'Profil güncellendi.';
          this.profilDuzenle = false;
          this.yukleniyor = false;
          setTimeout(() => this.basari = '', 4000);
        },
        error: (err) => {
          this.hata = err.error?.message || 'Güncelleme başarısız.';
          this.yukleniyor = false;
        }
      });
  }

  sifreDegistir(): void {
    if (!this.mevcutSifre || !this.yeniSifre || !this.yeniSifreTekrar) {
      this.hata = 'Lütfen tüm alanları doldurun.';
      return;
    }
    if (this.yeniSifre !== this.yeniSifreTekrar) {
      this.hata = 'Yeni şifreler eşleşmiyor.';
      return;
    }
    if (this.yeniSifre.length < 6) {
      this.hata = 'Yeni şifre en az 6 karakter olmalıdır.';
      return;
    }

    this.yukleniyor = true;
    this.hata = '';

    this.http.put(`${this.apiUrl}/kullanici/sifre-degistir`,
      { mevcutSifre: this.mevcutSifre, yeniSifre: this.yeniSifre },
      { headers: this.headers() })
      .subscribe({
        next: () => {
          this.basari = 'Şifreniz güncellendi.';
          this.mevcutSifre = '';
          this.yeniSifre = '';
          this.yeniSifreTekrar = '';
          this.yukleniyor = false;
          setTimeout(() => this.basari = '', 4000);
        },
        error: (err) => {
          this.hata = err.error?.message || 'Şifre değiştirme başarısız.';
          this.yukleniyor = false;
        }
      });
  }

  durumRengi(durum: string): string {
    switch (durum) {
      case 'onaylandi': return 'rozet-onay';
      case 'iptal': return 'rozet-iptal';
      default: return 'rozet-bekle';
    }
  }

  durumMetni(durum: string): string {
    switch (durum) {
      case 'onaylandi': return 'Onaylandı';
      case 'iptal': return 'İptal';
      default: return 'Beklemede';
    }
  }

  tarihFormat(tarih: string): string {
    return new Date(tarih).toLocaleString('tr-TR', {
      day: '2-digit', month: '2-digit', year: 'numeric',
      hour: '2-digit', minute: '2-digit'
    });
  }

  rolMetni(rol: string): string {
    switch (rol) {
      case 'doktor': return 'Doktor';
      case 'admin': return 'Admin';
      default: return 'Hasta';
    }
  }
}
