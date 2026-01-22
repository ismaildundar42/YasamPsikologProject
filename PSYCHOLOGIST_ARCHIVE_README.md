# PSİKOLOG ARŞİV SİSTEMİ - KURULUM TALİMATI

## 🎯 AMAÇ
Psikolog silindiğinde randevuları korumak ve geçmiş verileri gösterebilmek.

## 📋 ADIMLAR

### 1. SQL Script'i Çalıştır
```bash
create_psychologist_archive_table.sql
```
Bu script `PsychologistArchive` tablosunu oluşturur.

### 2. Projeyi Build Et
```bash
dotnet build
```

### 3. Test Et
- Bir psikolog sil
- Randevular tablosunu kontrol et → **SİLİNMEDİ!** ✅
- PsychologistArchive tablosunu kontrol et → **PSİKOLOG BİLGİLERİ KAYITLI!** ✅

## ✅ ARTIK NE OLACAK?

### Psikolog Silindiğinde:
1. ✅ Psikolog bilgileri `PsychologistArchive` tablosuna kopyalanır
2. ✅ Psikolog soft delete edilir (IsDeleted = 1)
3. ✅ Randevular **HİÇ SİLİNMEZ**
4. ✅ Çalışma saatleri silinir
5. ✅ Danışanlar korunur

### Geçmiş Randevularda:
- PsychologistId hala kayıtlı
- Psikolog bilgilerini göstermek için `PsychologistArchive` tablosundan çekeriz
- Kod örneği:

```csharp
// Psikolog silinmişse arşivden getir
var psychologist = await _context.Psychologists.FindAsync(id);
if (psychologist == null || psychologist.IsDeleted)
{
    // Arşivden getir
    var archived = await _context.PsychologistArchive
        .FirstOrDefaultAsync(a => a.OriginalPsychologistId == id);
        
    if (archived != null)
    {
        // Arşiv bilgilerini kullan
        var name = $"{archived.FirstName} {archived.LastName} (Eski Psikolog)";
    }
}
```

## 🎨 GÖRÜNÜM ÖRNEĞİ

Randevu listesinde:
- **Aktif Psikolog:** "Ahmet Yılmaz"
- **Silinmiş Psikolog:** "Mehmet Demir (Eski Psikolog)"

## 📊 ARŞIV TABLOSU YAPISI

| Kolon | Açıklama |
|-------|----------|
| OriginalPsychologistId | Orijinal psikolog ID'si |
| FirstName, LastName | İsim soyisim |
| Email, PhoneNumber | İletişim |
| CalendarColor | Takvim rengi |
| ArchivedAt | Silinme tarihi |
| ArchivedReason | Silme nedeni |
| OriginalCreatedAt | Orijinal oluşturma tarihi |

## 🔥 AVANTAJLAR

1. ✅ Hiçbir ilişkiye dokunmadık
2. ✅ CASCADE sorunları yok
3. ✅ Geçmiş veriler tamamen korunuyor
4. ✅ Raporlar düzgün çalışıyor
5. ✅ Audit trail var
6. ✅ Veri kaybı yok

---

**BAŞARI! 🎉 Artık psikolog silinse bile her şey kayıtlarda!**
