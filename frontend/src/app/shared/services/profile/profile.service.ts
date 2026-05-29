import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProfilModel } from './profile.model';

@Injectable({ providedIn: 'root' })
export class ProfileService {
  private apiUrl = 'https://medunitapp.onrender.com/api/kullanici';

  constructor(private http: HttpClient) { }

  getProfil(): Observable<ProfilModel> {
    return this.http.get<ProfilModel>(`${this.apiUrl}/profil`);
  }

  updateProfil(profil: ProfilModel): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/profil`, profil);
  }
}