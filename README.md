# 🏋️‍♂️ GymReservation  
**ASP.NET Core MVC – Spor Salonu Yönetim ve Randevu Sistemi**

GymReservation, Web Programlama dersi kapsamında geliştirilmiş, spor salonları için **yönetim, randevu ve yapay zekâ destekli fitness önerileri** sunan bir web uygulamasıdır.  
Proje, gerçek hayattaki bir spor salonu senaryosu baz alınarak tasarlanmıştır.

---

## 🎯 Projenin Amacı

Bu projenin amacı;  
ASP.NET Core MVC, Entity Framework Core, LINQ, Identity ve REST API gibi teknolojilerin **gerçek bir problem üzerinde uygulanmasını** sağlamaktır.

Sistem sayesinde:
- Spor salonları ve sundukları hizmetler yönetilebilir
- Antrenörlerin müsaitlikleri tanımlanabilir
- Üyeler uygun antrenör ve hizmete göre randevu alabilir
- Yapay zekâ desteği ile kişiye özel fitness ve beslenme önerileri sunulur

---

## 🧩 Kullanılan Teknolojiler

- **ASP.NET Core MVC**
- **C#**
- **Entity Framework Core (Code First)**
- **LINQ**
- **SQL Server**
- **ASP.NET Core Identity**
- **Bootstrap 5**
- **HTML5 / CSS3 / JavaScript**
- **Google Gemini API (AI entegrasyonu)**

---

## 👤 Kullanıcı Rolleri

### 🔐 Admin
- Spor salonu ekleme / düzenleme / silme
- Hizmet tanımlama (süre, ücret, salon ilişkisi)
- Antrenör ekleme ve hizmet atama
- Antrenör müsaitliklerini yönetme
- Randevuları görüntüleme ve onaylama
- Yönetim paneli üzerinden genel istatistikleri görme

### 👥 Üye (Kayıtlı Kullanıcı)
- Salonları ve hizmetleri görüntüleme
- Hizmete göre antrenör seçme
- Müsait saatlere göre randevu alma
- Kendi randevularını görüntüleme
- Yapay zekâ destekli fitness önerileri alma

---

## 📅 Randevu Sistemi Özellikleri

- Hizmet → Antrenör → Tarih/Saat adımlarından oluşan randevu akışı
- Aynı antrenör için çakışan randevular engellenir
- Kullanıcının kendi randevularıyla çakışma kontrolü yapılır
- Randevular **Beklemede / Onaylandı / İptal** durumlarına sahiptir

---

## 🧠 Yapay Zekâ (AI) Entegrasyonu

Sistem, **Google Gemini API** ile entegre çalışmaktadır.

Kullanıcıdan alınan bilgiler:
- Cinsiyet
- Yaş
- Boy
- Kilo
- Hedef (kilo verme, kas kazanma vb.)
- Aktivite seviyesi
- Opsiyonel ek bilgiler

Bu verilere göre:
- Kişiye özel **antrenman planı**
- **Beslenme önerileri**
- Motivasyon ve dikkat edilmesi gereken noktalar
oluşturulmaktadır.

---

## 🌐 REST API Kullanımı

Projede REST API kullanılarak veritabanı ile iletişim sağlanmıştır.

Örnek API işlemleri:
- Tüm antrenörleri listeleme
- Belirli bir tarihte uygun antrenörleri getirme
- LINQ ile filtreleme işlemleri

Bu yapı, projenin raporlama ve veri erişim gereksinimlerini karşılamaktadır.

---

## 🗄️ Veritabanı Yapısı (Özet)

- **FitnessCenter** (Salonlar)
- **GymService** (Hizmetler)
- **Trainer** (Antrenörler)
- **TrainerService** (Antrenör–Hizmet ilişkisi)
- **TrainerAvailability** (Müsaitlikler)
- **Appointment** (Randevular)
- **ApplicationUser** (Identity kullanıcı modeli)

