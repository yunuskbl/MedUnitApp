import { Component, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';

@Component({
  selector: 'app-about-section',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './about-section.component.html',
  styleUrls: ['./about-section.component.css']
})
export class AboutSectionComponent {

  videoAcik = false;

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {}

  videoAc(): void {
    this.videoAcik = true;
  }

  videoKapat(): void {
    this.videoAcik = false;
  }

  randevuyaGit(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    const el = document.querySelector('.contact-con');
    el?.scrollIntoView({ behavior: 'smooth' });
  }
}