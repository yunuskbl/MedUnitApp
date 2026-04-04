import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AppointmentServiceService {
constructor(private http: HttpClient) {}

  getAppointments() {
    return this.http.get('/api/appointments');
  }

  createAppointment(data: AppointmentServiceService) {
    return this.http.post('/api/appointments', data);
  }
}

