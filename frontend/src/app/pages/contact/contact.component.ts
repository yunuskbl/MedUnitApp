import {
  Component,
  OnInit,
  Inject,
  PLATFORM_ID,
  AfterViewInit,
} from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { ContactService } from '../../shared/services/contact-service/contact-service.service';

@Component({
  standalone: true,
  selector: 'app-contact',
  host: { ngSkipHydration: 'true' },
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './contact.component.html',
  styleUrl: './contact.component.css',
})
export class ContactComponent implements AfterViewInit {
  form: FormGroup;
  yukleniyor = false;
  basari = '';

  constructor(
    private fb: FormBuilder,
    private contactService: ContactService,
    @Inject(PLATFORM_ID) private platformId: Object,
  ) {
    // 2. FormGroup'ta kullanım
    this.form = this.fb.group({
      name: ['', Validators.required],
      lastname: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: ['+90', [Validators.required, phoneValidator()]],
      message: ['', Validators.required],
    });
  }

  onSubmit(): void {
  if (this.form.invalid) return;
  this.yukleniyor = true;

  const payload = {
    name:     this.form.value.name,
    lastName: this.form.value.lastname,  // TS: lastname → .NET: lastName
    email:    this.form.value.email,
    phone:    this.form.value.phone,
    message:  this.form.value.message
  };

  this.contactService.send(payload).subscribe({
    next: () => {
      this.basari = 'Mesajınız başarıyla gönderildi!';
      this.yukleniyor = false;
      this.form.reset({ phone: '+90' });  // reset sonrası +90 korunur
      setTimeout(() => (this.basari = ''), 5000);
    },
    error: () => {
      this.basari = 'Bir hata oluştu, lütfen tekrar deneyin.';
      this.yukleniyor = false;
    }
  });
}
  ngAfterViewInit(): void {
    if (!isPlatformBrowser(this.platformId)) return;

    const observer = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting) {
            entry.target.classList.add('animate');
            observer.unobserve(entry.target);
          }
        });
      },
      { threshold: 0.1 },
    );

    const selector =
      '.contact-detail, .contact-form, .contact-left-sec, .contact-right-sec';
    document.querySelectorAll(selector).forEach((el) => observer.observe(el));
  }
  onPhoneInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    let value = input.value;

    // + sadece en başta olabilir
    if (!value.startsWith('+')) {
      value = '+' + value.replace(/\+/g, '');
    } else {
      // Baştan sonraki + işaretlerini kaldır
      value = '+' + value.slice(1).replace(/\+/g, '');
    }

    // Rakam ve + dışındaki karakterleri temizle (boşluk ve tireye izin ver)
    value = value.replace(/[^\d\s\-\+]/g, '');

    input.value = value;
    this.form.get('phone')?.setValue(value, { emitEvent: true });
  }

  onPhoneKeydown(event: KeyboardEvent): void {
    const input = event.target as HTMLInputElement;
    const cursorPos = input.selectionStart ?? 0;

    // İlk karakterdeki + silinmesin
    if (
      cursorPos === 1 &&
      (event.key === 'Backspace' || event.key === 'Delete')
    ) {
      event.preventDefault();
    }
    // Pozisyon 0'da + olmayan karakter girilmesini engelle
    if (cursorPos === 0 && event.key !== '+' && event.key.length === 1) {
      event.preventDefault();
    }
  }
  
}

export function phoneValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value;
    if (!value || value === '+90' || value.trim() === '+') 
      return { required: true };

    const phoneRegex = /^\+\d{7,15}$/;
    const cleaned = value.replace(/\s|-/g, '');

    return phoneRegex.test(cleaned) ? null : { invalidPhone: true };
  };
}

