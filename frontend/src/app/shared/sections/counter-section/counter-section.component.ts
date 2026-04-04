import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  standalone: true,
  selector: 'app-counter-section',
  imports: [CommonModule],
  templateUrl: './counter-section.component.html',
  styleUrl: './counter-section.component.css'
})
export class CounterSectionComponent implements OnInit {

  psikolog = 0;
  subeler = 0;
  hasta = 0;
  basarilar = 0;

  // Gerçek sayılar — API'den gelince bunlar hedef değer
  private hedefDegerler = {
    psikolog: 2850,
    subeler: 1245,
    hasta: 0,      // API'den gelecek
    basarilar: 2496
  };

  private apiUrl = 'http://localhost:5227/api';

  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  ngOnInit(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    this.istatistikleriGetir();
  }

  istatistikleriGetir(): void {
    // Admin token varsa gerçek veriyi çek
    const token = localStorage.getItem('token');

    if (token) {
      const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });
      this.http.get<any>(`${this.apiUrl}/admin/istatistikler`, { headers })
        .subscribe({
          next: (data) => {
            this.hedefDegerler.hasta = data.toplamHasta;
            this.animasyonBaslat();
          },
          error: () => this.animasyonBaslat()
        });
    } else {
      this.animasyonBaslat();
    }
  }

  private animasyonBaslat(): void {
    this.sayiAnimasyonu('psikolog', this.hedefDegerler.psikolog);
    this.sayiAnimasyonu('subeler', this.hedefDegerler.subeler);
    this.sayiAnimasyonu('hasta', this.hedefDegerler.hasta || 3358);
    this.sayiAnimasyonu('basarilar', this.hedefDegerler.basarilar);
  }

  private sayiAnimasyonu(alan: string, hedef: number): void {
    const sure = 2000;
    const adim = 50;
    const artis = hedef / (sure / adim);
    let simdiki = 0;

    const interval = setInterval(() => {
      simdiki += artis;
      if (simdiki >= hedef) {
        simdiki = hedef;
        clearInterval(interval);
      }
      (this as any)[alan] = Math.floor(simdiki);
    }, adim);
  }
}