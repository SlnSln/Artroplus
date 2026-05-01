# CLAUDE.md — Artroplus Proje Geliştirme Kuralları

Bu dosya, projedeki tüm AI destekli geliştirme oturumları için bağlayıcı kurallardır.
Herhangi bir değişiklik yapılmadan önce bu dosyanın tamamı okunmalı ve kurallara uyulmalıdır.
Projede agent tarafından yapılan arka plandaki işler Türkçe dilinde yapılacak ve sorulan sorular Türkçe dilinde cevaplanacak.

---

## 📌 PROJE BİLGİLERİ

| Alan | Değer |
|---|---|
| **Proje Adı** | Artroplus |
| **GitHub Repo** | https://github.com/SlnSln/Artroplus.git |
| **Teknoloji** | ASP.NET Core 8 LTS |
| **Veritabanı** | SQL Server |
| **UI Teması** | Metronic v9.4.9 Demo 1 |
| **Mimari** | 5 Katmanlı (Core / Data / Service / Api / Web) |

### Proje Katmanları
| Katman | Sorumluluk |
|---|---|
| `Artroplus.Core` | Entity, DTO, Interface, IRepository tanımları |
| `Artroplus.Data` | Repository implementasyonu, DbContext, Migrations |
| `Artroplus.Service` | İş mantığı servisleri |
| `Artroplus.Api` | HTTP API (yalnızca localhost erişimi) |
| `Artroplus.Web` | Kullanıcı arayüzü (Metronic v9.4.9 Demo 1) |

> **Mimari Kural:** Web katmanından gelen tüm istekler `Artroplus.Api` üzerinden geçerek DB'ye ulaşır. Web katmanı doğrudan Data katmanına erişemez.

---

## 🔒 GÜVENLİK KURALLARI — DEĞİŞTİRİLEMEZ

### 1. KİMLİK DOĞRULAMA (Authentication)

- **Tüm Razor Page code-behind dosyaları** (`*.cshtml.cs`) `[Authorize]` attribute'ü taşımalıdır.
- **Tüm MVC Controller sınıfları** en az `[Authorize]` ile korunmalıdır.
- Yeni bir sayfa veya controller oluşturulduğunda `[Authorize]` **varsayılan olarak eklenir**, bilinçli bir gerekçe olmadan atlanmaz.
- `[AllowAnonymous]` yalnızca Login, Register ve public landing sayfaları için kullanılabilir; her kullanımda gerekçe yorum satırı olarak belirtilmelidir.

**Rol bazlı erişim zorunlu olan rotalar:**

| Controller / Page | Gerekli Rol |
|---|---|
| `SettingsController` | `[Authorize(Roles = "Yönetici,Admin")]` |
| Yeni admin sayfaları | `[Authorize(Roles = "...,Admin")]` — Admin her zaman dahil |

---

### 2. CSRF KORUMASI

- `[IgnoreAntiforgeryToken]` **kesinlikle kullanılamaz**. Bu attribute projenin hiçbir dosyasına eklenmeyecektir.
- POST işlemi içeren tüm handler ve action'larda `[ValidateAntiForgeryToken]` kullanılmalıdır.
- Razor form'larında `@Html.AntiForgeryToken()` veya `asp-antiforgery="true"` bulunmalıdır.

---

### 3. API GÜVENLİĞİ — IP KISITLAMA

- `Artroplus.Api` projesi yalnızca `localhost / 127.0.0.1` kaynaklı isteklere yanıt vermelidir.
- `Program.cs` içindeki IP filtreleme middleware'i **kaldırılamaz ve devre dışı bırakılamaz**.
- Yeni API endpoint'i eklendiğinde middleware zincirinin bu endpoint'i kapsadığı doğrulanmalıdır.

**Referans middleware yapısı (`Program.cs`):**
```csharp
// IP Whitelist Middleware — Kaldırılmaz
app.Use(async (context, next) =>
{
    var remoteIp = context.Connection.RemoteIpAddress;
    if (remoteIp == null ||
        (!IPAddress.IsLoopback(remoteIp) &&
         !remoteIp.Equals(IPAddress.Parse("127.0.0.1"))))
    {
        context.Response.StatusCode = 403;
        return;
    }
    await next();
});
```

---

### 4. HASSAS BİLGİ YÖNETİMİ

- `appsettings.json` içine **asla** şifre, API key, connection string parolası, secret token yazılmaz.
- Tüm hassas değerler **Environment Variable** üzerinden okunur:
  ```csharp
  // DOĞRU
  var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

  // YANLIŞ — Kesinlikle yapılmaz
  "Password": "gizlisifre123"
  ```
- `.gitignore` dosyası her zaman şunları içermelidir: `.env`, `appsettings.Production.json`, `secrets.json`

---

### 5. ROL HİYERARŞİSİ VE ADMIN YETKİSİ

- `Admin` rolü projedeki en yüksek yetkidir ve tüm rotalara erişim hakkına sahiptir.
- Rol kısıtlaması tanımlanan her `[Authorize(Roles = "...")]` attribute'üne `Admin` rolü **mutlaka** dahil edilmelidir.

---

## 🎨 FRONTEND VE UI STANDARTLARI (Metronic v9.4.9)

### 6. METRONIC ENTEGRASYONU

- **Bileşen Önceliği:** UI öğeleri oluşturulurken önce Metronic v9.4.9 Demo 1 hazır bileşenleri kullanılacaktır.
- **Tailwind Utility First:** Özel CSS yazımı yasaktır. Tüm stil işlemleri Tailwind class'ları ile yapılmalıdır. `style="..."` kullanımı kabul edilemez.
- **Karanlık Mod Uyumu:** Yeni eklenen her UI öğesi Metronic'in dark/light modlarıyla tam uyumlu olmalıdır.
- **Responsive Tasarım:** `mobile-first` yaklaşımı, `sm:`, `md:`, `lg:` prefixleri kullanılır.

### 7. FORM VE KULLANICI DENEYİMİ (UX)

- **Loading States:** Form gönderimi sırasında butonlar `disabled` ve Metronic spinner gösterilir.
- **Bildirimler:** `SweetAlert2` veya Metronic `Toast` kullanılır. Tarayıcı `alert()` kullanılmaz.
- **Validasyon:** `ModelState.IsValid` kontrolü her POST işleminde zorunludur.

---

## 🏗️ MİMARİ VE KOD STANDARTLARI

### 8. VERİ VE KATMAN YÖNETİMİ

- **DTO/ViewModel:** Entity nesneleri doğrudan View'a gönderilmez.
- **Dependency Injection:** Servisler interface üzerinden `Program.cs`'e kaydedilir.
- **İş Mantığı:** Controller içinde yazılmaz; Service veya Handler katmanına taşınır.
- **Performans:** Salt okunur sorgularda `.AsNoTracking()` kullanılır.

---

## 🔄 İŞ AKIŞI VE VERSİYON KONTROLÜ

### 9. GITHUB PUSH ZORUNLULUĞU

- **GitHub Repo:** `https://github.com/SlnSln/Artroplus.git`
- **Otomatik Commit & Push:** Tamamlanan her iş birimi sonrasında değişiklikler **mutlaka** push edilmelidir.
- **Commit Mesajları:** Kısa, öz ve Türkçe (Örn: `"feat: Stok yönetim sayfası eklendi"`).
- **İşlem Sırası:** `git add .` → `git commit -m "..."` → `git push`

---

## ⚠️ YENİ GELİŞTİRME ÖNCESİ KONTROL LİSTESİ

- [ ] Yeni eklenen sayfa veya endpoint `[Authorize]` ile korunuyor mu?
- [ ] Tasarım Metronic v9.4.9 Demo 1 standartlarına uygun mu?
- [ ] Form işlemlerinde `loading spinner`, `disabled button` ve `Validation` eklendi mi?
- [ ] `[IgnoreAntiforgeryToken]` eklendi mi? → **Eklendiyse kaldır.**
- [ ] Rol kısıtlaması gereken yerlerde `Admin` rolü listeye eklendi mi?
- [ ] Hassas bilgiler (şifre, key) Environment Variable'a taşındı mı?
- [ ] GitHub push yapıldı mı?

---

## 🚫 ASLA YAPILMAYACAKLAR

```
❌ [IgnoreAntiforgeryToken] kullanmak
❌ appsettings.json içine şifre/key yazmak
❌ Metronic bileşen yapısına aykırı veya inline CSS (style="") yazmak
❌ Loading state olmadan form post etmek
❌ View (cshtml) içinde veritabanı sorgusu yazmak
❌ Exception'ı boş catch bloğuyla yutmak (Mutlaka loglanmalı)
❌ Hardcoded IP, URL veya credential kullanmak
❌ [Authorize(Roles = "...")] tanımında Admin rolünü atlamak
❌ Web katmanından doğrudan Data katmanına erişmek (Api üzerinden geçmeli)
```

---

*Bu dosya proje kök dizininde tutulur ve her geliştirme oturumunda agent tarafından okunur.*