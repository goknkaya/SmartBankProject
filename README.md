# 💳 SmartBank

SmartBank, **ASP.NET Core 8** ile geliştirilmiş, **katmanlı mimariye sahip bir ödeme sistemleri simülasyon platformudur.**  
Proje; kart, müşteri, işlem, reversal, takas, switch ve chargeback modüllerini içerir ve **gerçek bankacılık akışlarını örneklemek** üzere tasarlanmıştır.

---

## 🧩 Katmanlar
- **SmartBank.Api** → REST API katmanı (.NET 8)
- **SmartBank.Application** → İş kuralları, DTO’lar, servis yapısı
- **SmartBank.Infrastructure** → Veritabanı erişimi (EF Core), repository pattern
- **SmartBank.Domain** → Entity sınıfları ve temel domain modelleri
- **SmartBank.Desktop** → WinForms tabanlı masaüstü yönetim ekranları

---

## ⚙️ Kullanılan Teknolojiler
- **.NET 8**
- **Entity Framework Core**
- **AutoMapper**
- **FluentValidation**
- **JWT Authentication**
- **Swagger (API dokümantasyonu)**
- **DevExpress / DataGridView (UI)**

---

## 💼 Modüller ve Kısa Açıklamaları
| Modül | Açıklama |
|--------|-----------|
| **Customer** | Müşteri yönetimi ve kimlik doğrulama işlemleri |
| **Card** | Kart oluşturma, limit yönetimi ve kart durumları |
| **Transaction** | Kartlı işlemlerin kayıt ve yönetimi |
| **Reversal** | Hatalı işlemlerin geri alınması |
| **Clearing (Takas)** | Gün sonu mutabakat ve dosya eşleştirme süreçleri |
| **Switch** | POS–banka arası iletişim ve yönlendirme akışları |
| **Chargeback** | İtiraz ve geri ödeme işlemlerinin yönetimi |

---

## 📸 Ekran Görüntüleri
*(Önerilen ekranlar aşağıda 👇)*  
- **Login / Ana Menü**  
- **CustomerView** (Müşteri listesi + ekleme ekranı)  
- **CardView** (Kart tanımlama + limit yönetimi)  
- **TransactionView** (İşlem oluşturma ve görüntüleme)  
- **ClearingView** (IN/OUT dosyası yükleme ve eşleşme sonuçları)  
- **ChargebackView** (İtiraz ekranı ve log detayları)  
 

```markdown
![CustomerView](Screenshots/customer_view.png)
![TransactionView](Screenshots/transaction_view.png)
![ClearingView](Screenshots/clearing_view.png)
