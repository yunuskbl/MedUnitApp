import { Component, Inject, OnInit, PLATFORM_ID} from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
@Component({
  standalone:true,
  selector: 'app-header',
  imports:[CommonModule, RouterModule, FormsModule],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {

isNavbarCollapsed = true;
activeDropdown: string | null = null;
// Modal
  modalAcik = false;
  aktifSekme: 'giris' | 'kayit' = 'giris';

  // Form alanları
  girisEmail = '';
  girisSifre = '';
  kayitAd = '';
  kayitSoyad = '';
  kayitEmail = '';
  kayitSifre = '';
  kayitRol = 'hasta';

  // Durum
  yukleniyor = false;
  hata = '';

  // Giriş durumu
  girisYapildi = false;
  kullaniciAd = '';
  rol = '';

  isBrowser = false;
  private apiUrl = 'https://medunitapp.onrender.com/api';

  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}
ngOnInit(): void {
  this.isBrowser = isPlatformBrowser(this.platformId);
  if (this.isBrowser) {
    const token = localStorage.getItem('token');
    this.kullaniciAd = localStorage.getItem('kullaniciAd') || '';
    this.rol = (localStorage.getItem('rol') || '').toLowerCase();
    this.girisYapildi = !!token;
  }
}
sayfayaGit(id: string): void {
  if (!isPlatformBrowser(this.platformId)) return;
  
  // Menüyü kapat
  this.isNavbarCollapsed = true;
  
  setTimeout(() => {
    const el = document.getElementById(id);
    if (el) {
      el.scrollIntoView({ behavior: 'smooth' });
    }
  }, 100);
}
  toggleNavbar() {
    this.isNavbarCollapsed = !this.isNavbarCollapsed;
  }

  toggleDropdown(id: string) {
    this.activeDropdown = this.activeDropdown === id ? null : id;
  }

  modalAc(sekme: 'giris' | 'kayit' = 'giris') {
    this.aktifSekme = sekme;
    this.modalAcik = true;
    this.hata = '';
  }

  modalKapat() {
    this.modalAcik = false;
    this.hata = '';
    this.formlariSifirla();
  }

  sekmeDegistir(sekme: 'giris' | 'kayit') {
    this.aktifSekme = sekme;
    this.hata = '';
  }

  girisYap() {
    if (!this.girisEmail || !this.girisSifre) {
      this.hata = 'Lütfen tüm alanları doldurun.';
      return;
    }

    this.yukleniyor = true;
    this.hata = '';

    this.http.post<any>(`${this.apiUrl}/auth/giris`, {
      email: this.girisEmail,
      sifre: this.girisSifre
    }).subscribe({
      next: (data) => {
        localStorage.setItem('token', data.token);
        localStorage.setItem('rol', data.rol);
        localStorage.setItem('kullaniciAd', data.ad);
        localStorage.setItem('kullaniciId', data.id.toString());

        this.girisYapildi = true;
        this.kullaniciAd = data.ad;
        this.rol = data.rol;
        this.yukleniyor = false;
        this.modalKapat();

        // Sayfayı yenile — randevu listesi güncellensin
        window.location.reload();
      },
      error: (err) => {
        this.hata = err.error?.message || 'Giriş başarısız.';
        this.yukleniyor = false;
      }
    });
  }

  kayitOl() {
    if (!this.kayitAd || !this.kayitSoyad || !this.kayitEmail || !this.kayitSifre) {
      this.hata = 'Lütfen tüm alanları doldurun.';
      return;
    }

    this.yukleniyor = true;
    this.hata = '';

    this.http.post<any>(`${this.apiUrl}/auth/kayit`, {
      ad: this.kayitAd,
      soyad: this.kayitSoyad,
      email: this.kayitEmail,
      sifre: this.kayitSifre,
      rol: this.kayitRol
    }).subscribe({
      next: (data) => {
        localStorage.setItem('token', data.token);
        localStorage.setItem('rol', data.rol);
        localStorage.setItem('kullaniciAd', data.ad);
        localStorage.setItem('kullaniciId', data.id.toString());

        this.girisYapildi = true;
        this.kullaniciAd = data.ad;
        this.rol = data.rol;
        this.yukleniyor = false;
        this.modalKapat();

        window.location.reload();
      },
      error: (err) => {
        this.hata = err.error?.message || 'Kayıt başarısız.';
        this.yukleniyor = false;
      }
    });
  }

  cikisYap() {
    localStorage.clear();
    this.girisYapildi = false;
    this.kullaniciAd = '';
    this.rol = '';
    window.location.reload();
  }

  private formlariSifirla() {
    this.girisEmail = '';
    this.girisSifre = '';
    this.kayitAd = '';
    this.kayitSoyad = '';
    this.kayitEmail = '';
    this.kayitSifre = '';
    this.hata = '';
  }
}

