# Oynanış değişiklikleri — 3 Kasım 2026

- Kulelerde sağ tık, bilgi balonu yerine gerçek menzili gösteren halkayı açar/kapatır.
- Kule XP kazanımı kaldırıldı. Seviye yükseltme altınla yapılır; ilk yükseltme 100 altın, sonraki her seviye için ücret 50 altın artar. Hazırlıkta ve savaşta kullanılabilir.
- Saldırı hızı saniyedeki saldırı sayısıdır. Mevcut kule verileri ters çevrilerek önceki temel saldırı temposu korundu. Eşyalar, yetenekler, okçu yığınları ve asker saldırıları aynı hız mantığını kullanır.
- Eşya görseli geç karede doğrudan imlece bağlanır; tutulduğu noktanın uzaklığı korunur. Yerleştirme şimdilik kapalıdır; panelde bu durum gösterilir.
- Askerler kulede doğar ve yol üzerindeki nöbet noktalarına yürür. Yeniden doğan askerler de aynı akışı kullanır.
- Savaş sırasında kule satın alma açıktır. Duraklatma ve bölüm sonucu sırasında kapalıdır.
- Beş ayrı Blender haritası bölüm seçimine bağlandı: Çam Nöbeti (5 alan), Kızıl Kanyon (6), Sis Bataklığı (7), Buz Geçidi (8), Taç Harabeleri (9).

## Kontrol sonuçları

- Altınla yükseltme: 1 → 2 seviye, tam 100 altın harcama; yetersiz altında değişiklik yok.
- Savaş sırasında satın alma: büyü kulesi yerleştirildi, 325 altın harcandı.
- Savaş sırasında yükseltme: başarılı, 100 altın harcandı (`final.json`).
- Saldırı hızı: 0.9090909 → 1.13636363 saldırı/sn; %25 eşya bonusu beklenen sonucu verdi (`speed-drag.json`).
- Sürükleme: imlecin (230, 100) piksel hareketinde eşya aynı miktarda hareket etti. İptalde kaynak eşya geri yüklendi; kapalı durumda sürükleme başlamadı.
- Askerler hazırlıkta kule konumunda görüldü; savaşta yoldaki noktalara ilerledi (`combat.json`).
- Menzil halkası: 4.935 birim, kulenin gerçek menziliyle aynı.
- Beş haritanın bölüm yükleyicisi, alan sayıları ve giriş–kale yolları çalışırken doğrulandı (`maps.json`).
- Son Unity kontrolünde derleme ve çalışma hatası yok. Denemelerden sonra Play modu kapatıldı.

Blender kaynakları ve önizlemeler: `ArtSource/CampaignMaps/`.
Unity modelleri: `Assets/Art/CampaignMaps/`.
Oynanabilir harita prefabları: `Assets/Prefabs/CampaignMaps/`.
Yeniden üretim: `Tools/Environment/build_campaign_maps.py`; Unity bağlantısı: `Backpack > Build Five Campaign Maps`.

## Harita geri alma
Kullanıcının isteğiyle önceki Blackpine V5 haritası, eski kamera açısı ve bölüm adları geri yüklendi. Beş harita yükleyicisi oyun sahnesinden kaldırıldı. Önceki haritanın giriş-kale yolu PathComplete olarak doğrulandı; diğer oynanış değişiklikleri korundu.
