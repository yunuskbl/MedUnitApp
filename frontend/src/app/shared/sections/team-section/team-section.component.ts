import { Component, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';

interface EkipUyesi {
  ad: string;
  unvan: string;
  resim: string;
  aciklama: string;
  facebook: string;
  twitter: string;
  linkedin: string;
}

@Component({
  selector: 'app-team-section',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './team-section.component.html',
  styleUrl: './team-section.component.css'
})
export class TeamSectionComponent {

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {}

  ekip: EkipUyesi[] = [
    {
      ad: 'Alice Waters',
      unvan: 'Kurucu Psikolog',
      resim: '../../../../assets/images/team-section/img-1.jpg',
      aciklama: 'Bireysel ve grup terapisi alanında 15 yıllık deneyime sahip uzman psikolog.',
      facebook: 'https://www.facebook.com',
      twitter: 'https://twitter.com',
      linkedin: 'https://www.linkedin.com'
    },
    {
      ad: 'Jane Doe',
      unvan: 'Terapi Uzmanı',
      resim: '../../../../assets/images/team-section/img-2.jpg',
      aciklama: 'Aile ve çift terapisi konusunda uzmanlaşmış deneyimli terapist.',
      facebook: 'https://www.facebook.com',
      twitter: 'https://twitter.com',
      linkedin: 'https://www.linkedin.com'
    },
    {
      ad: 'John Doe',
      unvan: 'Kurucu Psikolog',
      resim: '../../../../assets/images/team-section/img-3.png',
      aciklama: 'Çocuk ve ergen psikolojisi alanında uzman, sertifikalı psikoterapist.',
      facebook: 'https://www.facebook.com',
      twitter: 'https://twitter.com',
      linkedin: 'https://www.linkedin.com'
    },
    {
      ad: 'Clara Smith',
      unvan: 'Terapi Uzmanı',
      resim: '../../../../assets/images/team-section/img-4.jpg',
      aciklama: 'Anksiyete ve depresyon tedavisinde uzmanlaşmış klinik psikolog.',
      facebook: 'https://www.facebook.com',
      twitter: 'https://twitter.com',
      linkedin: 'https://www.linkedin.com'
    }
  ];

  randevuyaGit(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    const el = document.querySelector('.contact-con');
    el?.scrollIntoView({ behavior: 'smooth' });
  }
}