import { Component, ViewEncapsulation } from '@angular/core';
import { CommonModule } from '@angular/common';

interface Yorum {
  ad: string;
  unvan: string;
  resim: string;
  yorum: string;
}

@Component({
  selector: 'app-testimonial',
  standalone: true,
  encapsulation: ViewEncapsulation.None,
  imports: [CommonModule],
  templateUrl: './testimonial.component.html',
  styleUrl: './testimonial.component.css'
})
export class TestimonialComponent {

  aktifIndex = 0;

  yorumlar: Yorum[] = [
    {
      ad: 'Ahmet Yılmaz',
      unvan: 'Mutlu Hasta',
      resim: '../../../../assets/images/testimonial-section/img-erkek-1.jpg',
      yorum: 'MedUnit sayesinde kaygı sorunlarımın üstesinden geldim. Terapistim son derece anlayışlı ve profesyoneldi. Herkese tavsiye ederim.'
    },
    {
      ad: 'Ayşe Kaya',
      unvan: 'Mutlu Hasta',
      resim: '../../../../assets/images/testimonial-section/img-kadin-1.jpg',
      yorum: 'Online seans seçeneği hayatımı kolaylaştırdı. Evden çıkmadan profesyonel destek alabilmek harika bir deneyimdi.'
    },
    {
      ad: 'Mehmet Demir',
      unvan: 'Mutlu Hasta',
      resim: '../../../../assets/images/testimonial-section/img-erkek-2.jpg',
      yorum: 'Randevu sistemi çok pratik. Doktorumla görüşme saatim geldiğinde Zoom üzerinden direkt bağlanıyorum, çok kullanışlı.'
    },
    {
      ad: 'Fatma Şahin',
      unvan: 'Mutlu Hasta',
      resim: '../../../../assets/images/testimonial-section/img-kadin-2.jpg',
      yorum: 'Aile terapisi için başvurdum. Eşimle iletişimimiz çok güçlendi. MedUnit ekibine minnettarız.'
    },
    {
      ad: 'Ali Çelik',
      unvan: 'Mutlu Hasta',
      resim: '../../../../assets/images/testimonial-section/img-erkek-3.jpg',
      yorum: 'Depresyon tedavisinde büyük ilerleme kaydettim. Terapistim her adımda yanımdaydı. Teşekkürler MedUnit.'
    }
  ];

  onceki(): void {
    this.aktifIndex = this.aktifIndex === 0
      ? this.yorumlar.length - 1
      : this.aktifIndex - 1;
  }

  sonraki(): void {
    this.aktifIndex = this.aktifIndex === this.yorumlar.length - 1
      ? 0
      : this.aktifIndex + 1;
  }

  secili(index: number): void {
    this.aktifIndex = index;
  }
}
