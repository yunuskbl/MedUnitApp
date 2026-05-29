import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-pricing',
  imports: [CommonModule, RouterLink],
  templateUrl: './pricing.component.html',
  styleUrl: './pricing.component.css'
})
export class PricingComponent {
  planlar = [
    {
      isim: 'Ücretsiz',
      fiyat: '₺0',
      periyot: '/ay',
      renk: 'plan-ucretsiz',
      ozellikler: [
        '1 Doktor hesabı',
        '50 randevu/ay',
        'Online randevu formu',
        'Email bildirimleri',
        'Temel dashboard'
      ],
      cta: 'Hemen Başla',
      ctaLink: '/',
      vurgulu: false
    },
    {
      isim: 'Temel',
      fiyat: '₺499',
      periyot: '/ay',
      renk: 'plan-temel',
      ozellikler: [
        '5 Doktor hesabı',
        'Sınırsız randevu',
        'SMS + Email hatırlatıcı',
        'Hasta tıbbi dosya yönetimi',
        'Doktor notları',
        'Video görüşme (Zoom)',
        'Uzmanlık bazlı filtreleme'
      ],
      cta: 'Denemeye Başla',
      ctaLink: '/contact',
      vurgulu: true
    },
    {
      isim: 'Premium',
      fiyat: '₺999',
      periyot: '/ay',
      renk: 'plan-premium',
      ozellikler: [
        'Sınırsız doktor hesabı',
        'Çoklu klinik yönetimi',
        'Klinik sahibi rolü',
        'Tüm Temel özellikler',
        'Öncelikli teknik destek',
        'Özel logo & marka',
        'API erişimi'
      ],
      cta: 'Bizimle İletişime Geçin',
      ctaLink: '/contact',
      vurgulu: false
    }
  ];
}
