import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-reset-password',
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.css'
})
export class ResetPasswordComponent implements OnInit {
  token = '';
  yeniSifre = '';
  yeniSifreTekrar = '';
  yukleniyor = false;
  basari = '';
  hata = '';

  private apiUrl = 'https://medunitapp.onrender.com/api';

  constructor(
    private http: HttpClient,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.token = params['token'] || '';
      if (!this.token) {
        this.hata = 'Geçersiz link. Lütfen şifre sıfırlama işlemini yeniden başlatın.';
      }
    });
  }

  sifreYenile(): void {
    if (!this.yeniSifre || !this.yeniSifreTekrar) {
      this.hata = 'Lütfen tüm alanları doldurun.';
      return;
    }
    if (this.yeniSifre !== this.yeniSifreTekrar) {
      this.hata = 'Şifreler eşleşmiyor.';
      return;
    }
    if (this.yeniSifre.length < 6) {
      this.hata = 'Şifre en az 6 karakter olmalıdır.';
      return;
    }

    this.yukleniyor = true;
    this.hata = '';

    this.http.post(`${this.apiUrl}/auth/sifre-yenile`, {
      token: this.token,
      yeniSifre: this.yeniSifre
    }).subscribe({
      next: () => {
        this.basari = 'Şifreniz başarıyla güncellendi! Ana sayfaya yönlendiriliyorsunuz...';
        this.yukleniyor = false;
        setTimeout(() => this.router.navigate(['/']), 2500);
      },
      error: (err) => {
        this.hata = err.error?.message || 'Şifre güncellenemedi. Link geçersiz veya süresi dolmuş olabilir.';
        this.yukleniyor = false;
      }
    });
  }
}
