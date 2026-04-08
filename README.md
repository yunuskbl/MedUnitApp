# MedUnit Randevu

Modern bir sağlık platformu: online randevu sistemi, doktor listesi ve kullanıcı dashboard’u içeren full-stack web uygulaması.

## Proje Hakkında
MedUnitApp, kullanıcıların doktorları görüntüleyip kolayca randevu alabileceği modern bir sağlık platformudur. Proje; frontend (Angular) ve backend (.NET Web API) mimarisi üzerine kuruludur.


✨ Özellikler

👤 Kullanıcı Sistemi
    - Kullanıcı kayıt / giriş sistemi
    - JWT tabanlı kimlik doğrulama
    - Güvenli oturum yönetimi
👨‍⚕️ Doktor Sistemi
    - Doktor listesi görüntüleme
    - Doktor detay sayfası
    - Branşa göre filtreleme (opsiyonel geliştirme)
    
📅 Randevu Sistemi
    - Doktor seçerek randevu oluşturma
    - Tarih ve saat seçimi
    - Kullanıcıya özel randevu listesi
    - Randevu iptal etme özelliği
    
📊 Kullanıcı Dashboard
    - Kullanıcı randevularını görüntüleme
    - Profil yönetimi (geliştirme aşamasında)

🎨 UI / UX
    - Modern ve responsive tasarım  
    - Mobil uyumlu yapı
    - Temiz ve sade kullanıcı arayüzü

## Kurulum

### Backend (.NET)
cd backend
dotnet restore
dotnet run

### Frontend (Angular)
cd frontend
npm install
ng serve
