import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CarouselModule } from 'ngx-owl-carousel-o';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-project-section',
  imports: [RouterModule, CarouselModule, CommonModule],
  templateUrl: './project-section.component.html',
  styleUrl: './project-section.component.css'
})
export class ProjectSectionComponent {
  carouselOptions = {
    loop: true,
    mouseDrag: true,
    touchDrag: true,
    pullDrag: false,
    dots: false,
    navSpeed: 500,
    margin: 30,
    navText: [
      '<i class="fas fa-chevron-left"></i>',
      '<i class="fas fa-chevron-right"></i>'
    ],
    responsive: {
      0:   { items: 1, margin: 15 },
      600: { items: 2, margin: 20 },
      900: { items: 3, margin: 30 }
    },
    nav: true
  };
  acikItemlar: Set<number> = new Set();

toggleAcik(index: number): void {
  if (this.acikItemlar.has(index)) {
    this.acikItemlar.delete(index);
  } else {
    this.acikItemlar.add(index);
  }
}

acikMi(index: number): boolean {
  return this.acikItemlar.has(index);
}
}
