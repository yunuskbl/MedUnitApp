import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { RouterModule } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-forgot-password',
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.css'
})
export class ForgotPasswordComponent {
  email = '';
  yukleniyor = false;
  basari = '';
  hata = '';

  private apiUrl = 'https://medunitapp.onrender.com/api';

  constructor(private http: HttpClient) {}

  gonder(): void {
    if (!this.email.trim()) {
      this.hata = 'Lütfen email adresinizi girin.';
      return;
    }

    this.yukleniyor = true;
    this.hata = '';

    this.http.post(`${this.apiUrl}/auth/unuttum-sifre`, { email: this.email })
      .subscribe({
        next: () => {
          this.basari = 'Eğer bu email kayıtlıysa, şifre sıfırlama linki gönderildi. Spam klasörünüzü de kontrol edin.';
          this.yukleniyor = false;
        },
        error: () => {
          this.basari = 'Eğer bu email kayıtlıysa, şifre sıfırlama linki gönderildi.';
          this.yukleniyor = false;
        }
      });
  }
}
