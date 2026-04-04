import { Component, Input, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { NgIf } from "../../../../../../node_modules/@angular/common/common_module.d-C8_X2MOZ";

@Component({
  selector: 'app-zoom-modal',
  imports: [NgIf],
  templateUrl: './zoom-modal.component.html',
  styleUrl: './zoom-modal.component.css'
})

export class ZoomModalComponent implements OnInit{
  @Input() randevuId!: number;
  @Input() rol!: string;          // 'hasta' | 'doktor'

  zoomLink: string = '';
  yukleniyor: boolean = false;
  hata: string = '';
  modalAcik: boolean = false;
   constructor(private http: HttpClient) {}

  ngOnInit(): void {}

  modalAc(): void {
    this.modalAcik = true;
    this.zoomLinkiGetir();
  }
   modalKapat(): void {
    this.modalAcik = false;
    this.zoomLink = '';
    this.hata = '';
  }

  private zoomLinkiGetir(): void {
    this.yukleniyor = true;
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });

    this.http.get<any>(
      `https://medunitapp.onrender.com/api/zoom/meeting-bilgisi/${this.randevuId}`,
      { headers }
    ).subscribe({
      next: (data) => {
        // Doktor host linkini, hasta join linkini görür
        this.zoomLink = this.rol === 'doktor' ? data.hostUrl : data.joinUrl;
        this.yukleniyor = false;
      },
      error: () => {
        this.hata = 'Zoom linki alınamadı.';
        this.yukleniyor = false;
      }
    });
  }

  zoomAc(): void {
    window.open(this.zoomLink, '_blank');
  }
}

