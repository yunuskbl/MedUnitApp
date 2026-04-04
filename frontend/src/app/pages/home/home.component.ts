import { Component, Inject, OnInit, PLATFORM_ID, Type } from '@angular/core';
import { CommonModule } from '@angular/common';
import { isPlatformBrowser } from '@angular/common';
import { BannerSectionComponent } from '../../shared/sections/banner-section/banner-section.component';
import { AboutSectionComponent } from '../../shared/sections/about-section/about-section.component';
import { BlogSectionComponent } from '../../shared/sections/blog-section/blog-section.component';
import { CounterSectionComponent } from '../../shared/sections/counter-section/counter-section.component';
import { TestimonialComponent } from '../../shared/sections/testimonial/testimonial.component';
import { TeamSectionComponent } from '../../shared/sections/team-section/team-section.component';
import { ProcessSectionComponent } from '../../shared/sections/process-section/process-section.component';
import { ProjectSectionComponent } from '../../shared/sections/project-section/project-section.component';
import { AppointmentSectionComponent } from '../../shared/sections/appointment-section/appointment-section.component';
import { ServiceCardComponent } from '../../shared/sections/service-card/service-card.component';

@Component({
  standalone: true,
  selector: 'app-home',
  imports: [
    CommonModule,
    BannerSectionComponent,
    ServiceCardComponent,
    AboutSectionComponent,
    AppointmentSectionComponent,
    ProjectSectionComponent,
    CounterSectionComponent,
    ProcessSectionComponent,
    TeamSectionComponent,
    TestimonialComponent,
    BlogSectionComponent,
  ],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  constructor(@Inject(PLATFORM_ID) private platformId: Object) {}

  ngOnInit(): void { 
  }
 ngAfterViewInit(): void {
  if (!isPlatformBrowser(this.platformId)) return;

  const observer = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
      if (entry.isIntersecting) {
        entry.target.classList.add('animate');
        observer.unobserve(entry.target);
      }
    });
  }, { threshold: 0.1 });

  const selector = [
    '.servis-kart',
    '.member-con',
    '.blog-box',
    '.treatment-types',
    '.project-img',
    '.about-img-con',
    '.about-txt-con',
    '.process-left-sec',
    '.process-right-sec',
    '.counter-inner-sec',
    '.testimonial-inner-sec',
    '.blogs-inner-con',
    '.team-inner-section',
    '.generic-title',
    '.banner-txt-section'
  ].join(', ');

  document.querySelectorAll(selector).forEach(el => observer.observe(el));
}
}

