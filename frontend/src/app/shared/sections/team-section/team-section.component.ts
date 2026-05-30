import { Component, Inject, OnInit, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';

interface DoktorKart {
  id: number;
  ad: string;
  soyad: string;
  uzmanlik?: string;
  resim: string;
}

@Component({
  selector: 'app-team-section',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './team-section.component.html',
  styleUrl: './team-section.component.css'
})
export class TeamSectionComponent implements OnInit {

  doktorlar: DoktorKart[] = [];
  private apiUrl = 'https://medunitapp.onrender.com/api';

  private statikResimler = [
    '../../../../assets/images/team-section/img-1.jpg',
    '../../../../assets/images/team-section/img-2.jpg',
    '../../../../assets/images/team-section/img-3.png',
    '../../../../assets/images/team-section/img-4.jpg',
  ];

  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  ngOnInit(): void {
    this.http.get<any[]>(`${this.apiUrl}/kullanici/kullanicilar`).subscribe({
      next: (data) => {
        const doktorListesi = data.filter(u => u.rol?.toLowerCase() === 'doktor');
        this.doktorlar = doktorListesi.map((u, i) => ({
          id: u.id,
          ad: u.ad,
          soyad: u.soyad,
          uzmanlik: u.uzmanlik,
          resim: this.statikResimler[i % this.statikResimler.length]
        }));
      },
      error: () => {
        this.doktorlar = [];
      }
    });
  }

  randevuyaGit(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    const el = document.querySelector('.contact-con');
    el?.scrollIntoView({ behavior: 'smooth' });
  }
}
