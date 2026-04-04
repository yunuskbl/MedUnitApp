import { CommonModule } from '@angular/common';
import { Component, ChangeDetectorRef } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CarouselModule } from 'ngx-owl-carousel-o';

@Component({
  standalone:true,
  selector: 'app-service-card',
  imports: [CommonModule,CarouselModule,RouterModule],
  templateUrl: './service-card.component.html',
  styleUrl: './service-card.component.css'
})
export class ServiceCardComponent {
  carouselLoaded = false;

  customOptions = {
    loop: true,
    mouseDrag: true,
    touchDrag: false,
    pullDrag: false,
    dots: false,
    navSpeed: 700,
    margin: 20,
    navText: ['', ''],
    responsive: {
      0: {
        items: 1,
        margin: 12
      },
      400: {
        items: 2,
        margin: 15
      },
      740: {
        items: 3,
        margin: 20
      }
    },
    nav: true
  }

  constructor(private cdr: ChangeDetectorRef) {}

  onCarouselLoaded() {
    this.carouselLoaded = true;
    this.cdr.detectChanges();
  }
}
