# Karaçam — stratejik harita V2

56 × 48 m Blender tasarımı. Oyun projesine aktarım yapılmadı. Tüm yeni dosyalar ArtSource altında; Unity sahnelerine, prefablarına ve oynanış koduna dokunulmadı.

## Yol ve beş kule alanı

Yol tek bir okunabilir rota olarak uzatıldı. İlk yarıda iç içe geçen virajlar, ortada uzun dönüş, nehirde dar geçiş, son kısımda çıkış virajı bulunuyor. Yolun köprü çevresindeki genişliği 1,3 m, diğer bölümlerde 1,7 m.

| Alan | Tasarım amacı | Tercih ve sınırlama |
|---|---|---|
| 01 | İlk saç tokası virajına erken müdahale | Birden fazla yol parçasına erişim fırsatı; çıkışa desteği zayıf |
| 02 | İç virajdan çapraz ateş | İlk alanla örtüşme kurulabilir; köprüye uzanmaz |
| 03 | Uzun dönüşün dışından sürekli baskı | Dönüş boyunca hedef görme fırsatı; merkez kaçaklarına müdahalesi sınırlı |
| 04 | Köprü yaklaşımını daraltmak | Geçiş öncesi kontrol; nehir sonrası alanı tek başına kapatmaz |
| 05 | Çıkış virajında son savunma | Kaçakları yakalamaya uygun; ilk dalgaları uzun süre bekler |

Her alanın inşa yarıçapı 1,22 m; taş çeperiyle yaklaşık 3 m çapındadır. Beş alan `Tower_Pads_Exactly_5` koleksiyonunda numaralandırılmıştır. Yol merkezine uzaklıkları `design.json` içinde kontrol edilmiştir.

Taktik görseldeki kesikli çemberler **6,2 m örnek menzil** gösterir; gerçek kule menzili, görüş hattı, hedefleme, düşman hızı ve denge testi uygulanmamıştır. Bu değerler tasarım karşılaştırması içindir. Kayalar ve ağaçlar görsel sınırlar oluşturur; oynanış engeli oldukları varsayılmamıştır.

## Köprü

Önceki 5,6 m civarı açıklık yerine 3,8 m uzunluk, 1,7 m kullanılabilir tabliye genişliği ve 0,25 m yükselme. Tabliye taşları birbirine yakın seviyededir; yüksek basamak etkisi kaldırılmıştır. Alçak korkuluklar yolu kameradan gizlemez.

## Çevre

Yoğunlaştırılmış orman sınırı ve katmanlı çamlar; yosunlu kayalar, çalılar, kütükler, ahşap çitler, eski taş duvar, erzak sandıkları, metal kuşaklı variller, kamp çadırı, sönmüş ateş çemberi, el arabası, küçük ahşap iskele, fener direkleri ve taş enkazı.

## Dosyalar

- `Blackpine_Strategic_Map_V2.blend`: düzenlenebilir ana sahne; çimen dokusu dosyaya paketli.
- `Map_V2_Beauty.png`: oyun kamerasına benzer açılı önizleme.
- `Map_V2_Tactical.png`: üstten rota ve örnek menzil çemberleri.
- `design.json`: alan konumları, köprü boyutları ve rota kontrol noktaları.

`Tactical_Overlay` koleksiyonu Blender dosyasında varsayılan olarak gizlidir. Tasarımı yeniden üretmek için `Tools/Environment/build_map_v2.py` kullanılır. Betik yalnızca ArtSource içine Blender ve görsel çıktı yazar.
