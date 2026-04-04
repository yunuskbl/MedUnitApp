import { Component, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-banner-section',
  imports: [],
  templateUrl: './banner-section.component.html',
  styleUrl: './banner-section.component.css'
})
export class BannerSectionComponent {

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {}

  randevuyaGit(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    const el = document.querySelector('.contact-con');
    el?.scrollIntoView({ behavior: 'smooth' });
  }

  hakkimizdaGit(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    const el = document.querySelector('.about-main-section');
    el?.scrollIntoView({ behavior: 'smooth' });
  }
}
