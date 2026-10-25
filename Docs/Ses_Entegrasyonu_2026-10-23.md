# Ses entegrasyonu — 23 Ekim 2026

33 kaynak WAV dosyası `Assets/Resources/Audio/SFX` altına orijinalleri değiştirilmeden kopyalandı.

- Üs: pozitif hasarda base_damage; ölümde bir kez base_fall.
- Sonuç: başarı/yenilgi efekti; yenilgi efekti yıkımın başlangıcından 0,55 saniye sonra.
- Kuleler: ok, büyü ve havan gerçek atışta; birlikler yakın dövüş vuruşunda SFX_02.
- Arayüz: buton tıklama/klavye onayı, ana paneller ve bilgi paneli açılış/kapanışı.
- Itemler: başarılı satın alma/yerleştirme, taşıma, takma, teslim alma ve satış. İptal veya geçersiz bırakmada ses çalmaz.

## Item eşleştirmesi

| Ses | Eşya grubu |
|---|---|
| 01 | Yaylar |
| 02 | Kılıçlar, havan doldurma takımı |
| 03 | Deri zırh ve kemer |
| 04 | Miğfer, taç, odak çemberi |
| 05 | Barut |
| 06 | Kalkan, havan namlusu/ağır gülle, toprak özü |
| 07 | Metal zırh ve topçu başlığı |
| 08 | Eldiven |
| 09 | Oklar |
| 10 | Yüzük, kolye, tılsım |
| 11 | Asa, totem, mühür |
| 12 | İksir ve su küresi |
| 13 | Patlayıcı mühimmat ve ateş katalizörü |
| 14 | Çizme |
| 15 | Pelerin, cübbe, kapüşon |
| 16 | Parşömen ve hava küresi |

Seçim eşya malzemesi/türü ve dosyaların ölçülen zarf/ses düzeyi özelliklerine dayanır; numaralı dosyalarda içerik açıklaması yoktur. SFX_13 diğerlerinden belirgin yüksek olduğundan oynatma seviyesi azaltılmıştır. ItemData Inspector `Sound Effect Index`: 0 otomatik, 1–16 özel seçim. Element varyantları eşya ailesini korur. Ayrıntılar: `Validation/sfx/item-mapping.csv`.

## Doğrulama

Açık Unity 2022.3.62f3 editöründe derlendi. 33 klip yüklendi, 244 item eşleşti; 16 item efektinin tamamı kullanılıyor. Havan/yay ayrımı, element ailesi ve Inspector override kontrol edildi.

Canlı Play testi: kareler ilerledi, 19 kaynak/124 buton bağlandı; timeScale=0 arayüz sesi, mute/restore, 12 savaş sesi sınırı ve sonuçta savaş seslerini durdurma geçti. Konsolda hata yok. Play kapatıldı ve arka planda çalışma ayarı geri getirildi. Raporlar `Validation/sfx` içinde. İşitsel dinleme değerlendirmesi yapılmadı.
