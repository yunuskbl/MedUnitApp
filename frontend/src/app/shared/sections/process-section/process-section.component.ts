import { Component, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

@Component({
  selector: 'app-process-section',
  standalone: true,
  imports: [],
  templateUrl: './process-section.component.html',
  styleUrl: './process-section.component.css'
})
export class ProcessSectionComponent {

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {}

  randevuyaGit(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    const el = document.querySelector('.contact-con');
    el?.scrollIntoView({ behavior: 'smooth' });
  }
}
