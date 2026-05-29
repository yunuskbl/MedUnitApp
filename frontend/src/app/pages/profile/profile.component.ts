import { Component, OnInit } from '@angular/core';
import { ProfileService } from '../../shared/services/profile/profile.service';
import { ProfilModel } from '../../shared/services/profile/profile.model';
import { FormsModule } from '@angular/forms';
import { CommonModule } from "@angular/common";
import { profile } from 'console';
import { HeaderComponent } from "../../components/header/header.component";

@Component({
  selector: 'app-profil-yonetimi',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  profil: ProfilModel | null = null;
  yukleniyor = true;
  hata: string | null = null;
  biyografiDuzenleme = false;
  ucretDuzenleme = false;

  constructor(private profilService: ProfileService) {}

  ngOnInit(): void {
  this.profilService.getProfil().subscribe({
    next: (data) => {
      console.log('Gelen veri:', data);
      this.profil = data;
      this.yukleniyor = false;
    },
    error: (err) => {
      console.log('Hata:', err);
      this.hata = 'Profil yüklenemedi.';
      this.yukleniyor = false;
    }
  });
}

  profilKaydet(): void {
    if (!this.profil) return;
    this.profilService.updateProfil(this.profil).subscribe({
      next: () => {
        this.biyografiDuzenleme = false;
        this.ucretDuzenleme = false;
      },
      error: () => {
        this.hata = 'Güncelleme başarısız.';
      }
    });
  }
}