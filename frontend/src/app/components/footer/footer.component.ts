import { Component, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.css'
})
export class FooterComponent {

  email = '';
  basari = '';
  hata = '';

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {}

  newsletterKayit(): void {
    if (!this.email || !this.email.includes('@')) {
      this.hata = 'Geçerli bir email adresi girin.';
      return;
    }
    this.basari = 'Bültenimize başarıyla abone oldunuz!';
    this.hata = '';
    this.email = '';
    setTimeout(() => this.basari = '', 4000);
  }

  randevuyaGit(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    const el = document.querySelector('.contact-con');
    el?.scrollIntoView({ behavior: 'smooth' });
  }
}