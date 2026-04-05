import { Component, ViewEncapsulation } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CarouselModule } from 'ngx-owl-carousel-o';

interface Blog {
  baslik: string;
  ozet: string;
  tarih: string;
  imgUrl: string;
}

@Component({
  selector: 'app-blog-section',
  encapsulation: ViewEncapsulation.None,
  standalone: true,
  imports: [CommonModule, RouterModule, CarouselModule],
  templateUrl: './blog-section.component.html',
  styleUrl: './blog-section.component.css'
})
export class BlogSectionComponent {

  carouselOptions = {
    loop: true,
    mouseDrag: true,
    touchDrag: true,
    pullDrag: false,
    dots: false,
    navSpeed: 500,
    navText: [
      '<i class="fas fa-chevron-left"></i>',
      '<i class="fas fa-chevron-right"></i>'
    ],
    responsive: {
      0:   { items: 1 },
      600: { items: 2 },
      900: { items: 3 }
    },
    nav: true
  };

  bloglar: Blog[] = [
    {
      
      baslik: 'Kaygı Bozukluğuyla Başa Çıkmanın 5 Yolu',
      ozet: 'Günlük hayatınızı etkileyen kaygıyla nasıl başa çıkabileceğinizi öğrenin.',
      tarih: '20 Mar 2024',
      imgUrl: '../../../../assets/images/blog-section/img-1.jpg'
    },
    {
      
      baslik: 'Çift Terapisinde Sık Sorulan Sorular',
      ozet: 'Çift terapisi hakkında merak ettiğiniz her şeyi bu yazıda bulabilirsiniz.',
      tarih: '14 Şub 2024',
      imgUrl: '../../../../assets/images/blog-section/img-2.jpg'
    },
    {
      
      baslik: 'Çocuğunuzun Psikolojik Gelişimi',
      ozet: 'Çocukların duygusal ve zihinsel gelişimi için ebeveynlere öneriler.',
      tarih: '07 Haz 2024',
      imgUrl: '../../../../assets/images/blog-section/img-3.jpg'
    }
  ];
}
