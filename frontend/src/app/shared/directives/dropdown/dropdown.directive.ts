import { Directive, HostListener, ElementRef, Renderer2 } from '@angular/core';

@Directive({
  selector: '[appDropdown]'
})
export class DropdownDirective {
  isOpen = false;

  constructor(private el: ElementRef, private renderer: Renderer2) {}

  @HostListener('mouseEnter')
  onMouseEnter() {
    if (window.innerWidth >= 768) {
      this.open();
    }
  }

  @HostListener('mouseLeave')
  onMouseLeave() {
    if (window.innerWidth >= 768) {
      this.close();
    }
  }

  @HostListener('click')
  onClick() {
    if (window.innerWidth < 768) {
      this.isOpen ? this.close() : this.open();
    }
  }

  open() {
    this.isOpen = true;
    this.renderer.addClass(this.el.nativeElement, 'show');
    const menu = this.el.nativeElement.querySelector('.dropdown-menu');
    if (menu) this.renderer.addClass(menu, 'show');
    const toggle = this.el.nativeElement.querySelector('.dropdown-toggle');
    if (toggle) this.renderer.setAttribute(toggle, 'aria-expanded', 'true');
  }

  close() {
    this.isOpen = false;
    this.renderer.removeClass(this.el.nativeElement, 'show');
    const menu = this.el.nativeElement.querySelector('.dropdown-menu');
    if (menu) this.renderer.removeClass(menu, 'show');
    const toggle = this.el.nativeElement.querySelector('.dropdown-toggle');
    if (toggle) this.renderer.setAttribute(toggle, 'aria-expanded', 'false');
  }
}

