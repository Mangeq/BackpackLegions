# Backpack Legions — Denge V1

3 Ekim 2026. Emir sayısal dengeyi asistana bıraktı. Hedef: birlikler düşmanı oyalayıp canını azaltsın; düşman kendi saldırı ritmini kullansın. Bu değerler ilk kapalı test başlangıcıdır, son denge değildir.

## Kuleler ve düşmanlar

| Tür | Can | Hasar | Saldırı aralığı | Fiyat / öldürme ödülü |
|---|---:|---:|---:|---:|
| Okçu | — | 14 | 1 sn | 250 altın |
| Büyücü | — | 30 | 1,8 sn | 325 altın |
| Birlik kulesi (3 asker) | Asker başına 60 | Asker başına 4 | 1,2 sn | 225 altın |
| Fast (ilk dalga prefabının verisi) | 70 | 12 | 1,2 sn | 20 altın ödül |
| Boss | 350 | 25 | 1,8 sn | 100 altın ödül |

Temel kritik şansı tüm kulelerde %10. Mermilerde mevcut kritik çarpanı 2; askerlerde 1,5. Sürekli hedef varsa beklenen DPS: okçu 15,4; büyücü 18,33; asker başına 3,5. Menzil, hedefe erişim, mermi yolculuğu ve ölümde boşa çıkan hasar gerçek DPS'yi düşürür. Büyücü şu an tek hedeflidir; AoE eklenmedi.

Askerin Fast karşısında dış destek olmadan dayanma hedefi ceil(60/12) × 1,2 = 6 saniye, Boss karşısında ceil(60/25) × 1,8 = 5,4 saniye. İlk düşman saldırısı da bir aralık bekler. Asker tek başına Fast düşmanı öldürmeden genellikle ölür; asıl katkısı kulelere zaman kazandırmaktır. Yeniden üretim 2→6 saniye: kaybedilen askerlerin bedeli hissedilir.

Başlangıç 1000 altın korunur. Üç kule toplam 800, eşyalara 200 kalır. İlk dalganın 10 düşmanı öldürülürse 200, üç boss toplam 300 altın sağlar. Mevcut GameOver altını 1000'e sıfırlar; bu yüzden sonuç ekranındaki altın run kazancı ölçüsü değildir.

Normal elementin hem direnç hem zayıflık listelerinde bulunması kaldırıldı; temel hasar artık örtülü ×1,5 almıyor. Boss'un Water direnci korundu. Düşmanın aynı damage değeri kaleye ulaşınca da uygulanır: Fast 12, Boss 25. Ayrı kale hasarı mimarisi eklenmedi.

## Kod akışı

- EnemyData.attackInterval saniye cinsinden düşman saldırı süresidir.
- Unit.Engage, Enemy.AttackTarget'ı eşleşme kurulurken bir kez çağırır. Unit.Combat sadece askerin vuruşlarını yürütür; her asker vuruşu düşmana saldırı komutu vermez.
- Enemy.AttackTarget hedefi kaydeder, yürümeyi durdurur ve sayacı başlatır. Enemy.Update sayacı Time.deltaTime ile azaltır; dolunca hedef birliğe vurur ve kendi aralığını yeniden yükler. Pause'da oyun zamanı durduğu için sayaç ilerlemez.
- Enemy.ResumeMoving hedefi ve angajmanı temizler, yürümeyi açar. isDead koruması aynı ölüme tekrar ödül verilmesini önler.
- UI kule fiyatlarını prefabın TowerData.cost alanından okur. Görünen fiyat ve satın almada düşülen tutar aynı kaynağı kullanır.
- Tower.CalculateStats içindeki kritik sonucu Mathf.Clamp01 ile 0–1 aralığında tutulur. Hasar bonusları toplanır ve temel hasarla çarpılır. Hız bonusları toplanır, aralık (1 + bonus)'a bölünür. Can bonusları birliklere aktarılır.

Örnek: +%18 hız, 1,2 saniyeyi 1,2/1,18 ≈ 1,017 saniyeye indirir. +%35 can, 60 canı 81 yapar. Kritik bonusu +0,03 = +3 yüzde puanı; %10→%13.

## Eşyalar

Fiyatlar yaygın 60, nadir 130, epik 220, efsanevi 360 temelinde. Uzmanlaşmış birlik eşyalarında etki ve alan maliyetine göre istisnalar var. Varsayılan silah kule fiyatına dahil, ek bonusu ve bağımsız fiyatı 0. Can iksiri +%65 birlik canı verir, sadece birlik dükkanına aittir; anlık iyileştirme değildir. Parşömen artık gerçek stat bonusu verir. Placeholder adları/kimlikleri korunur; henüz uygulanmayan topçu için eşya değerleri de aynı ölçeğe getirilmiştir.

Aşağıdaki bonuslar yüzde olarak gösterilir; kritik sütunu yüzde puanıdır. Tüm eşya kayıtları:

| Asset | Fiyat eski→yeni | Hasar % | Kritik puanı | Hız % | Can % |
|---|---:|---:|---:|---:|---:|
| ItemData/Deneme 1.asset | 50→60 | 12 | 3 | 0 | 0 |
| ItemData/Deneme 2.asset | 150→130 | 22 | 5 | 5 | 0 |
| ItemData/Deneme 3.asset | 150→130 | 22 | 5 | 5 | 0 |
| ItemData/Deneme 4.asset | 200→220 | 15 | 5 | 25 | 0 |
| ItemData/Deneme 5.asset | 200→180 | 0 | 0 | 0 | 65 |
| ItemData/Deneme 6.asset | 350→360 | 40 | 12 | 20 | 0 |
| ItemData/Deneme 7.asset | 50→60 | 12 | 3 | 0 | 0 |
| ItemData/Deneme 8.asset | 50→60 | 12 | 3 | 0 | 0 |
| ItemData/Deneme 9.asset | 50→60 | 12 | 3 | 0 | 0 |
| ItemData/Deneme 10.asset | 50→60 | 12 | 3 | 0 | 0 |
| ItemData/Deneme 11.asset | 50→60 | 12 | 3 | 0 | 0 |
| ItemData/Deneme 12.asset | 50→60 | 12 | 3 | 0 | 0 |
| ItemData/Deneme 13.asset | 50→60 | 12 | 3 | 0 | 0 |
| ItemData/Deneme 14.asset | 50→60 | 12 | 3 | 0 | 0 |
| ItemData/Deneme 15.asset | 50→60 | 12 | 3 | 0 | 0 |
| ItemData/Deneme 16.asset | 50→60 | 12 | 3 | 0 | 0 |
| ItemData/Deneme 17.asset | 50→60 | 12 | 3 | 0 | 0 |
| ItemData/Deneme 18.asset | 50→60 | 12 | 3 | 0 | 0 |
| ItemData/Deneme 19.asset | 50→60 | 12 | 3 | 0 | 0 |
| ItemData/Placeholder/Placeholder_Buyucu_Legendary.asset | 350→360 | 40 | 12 | 20 | 0 |
| ItemData/Placeholder/Placeholder_Okcu_Legendary.asset | 350→360 | 40 | 12 | 20 | 0 |
| ItemData/Placeholder/Placeholder_Okcu_Rare.asset | 150→130 | 22 | 5 | 5 | 0 |
| ItemData/Placeholder/Placeholder_Topcu_Legendary.asset | 350→360 | 40 | 12 | 20 | 0 |
| ItemData/Placeholder/Placeholder_Topcu_Rare.asset | 150→130 | 22 | 5 | 5 | 0 |
| ItemData/Unit/Unit_AkutunAyakkabisi.asset | 50→60 | 0 | 0 | 18 | 0 |
| ItemData/Unit/Unit_DeriKalkan.asset | 60→80 | 0 | 0 | 0 | 35 |
| ItemData/Unit/Unit_KomutanSancagi.asset | 200→220 | 20 | 0 | 0 | 50 |
| ItemData/Unit/Unit_LejyonerKilici.asset | 50→60 | 18 | 0 | 0 | 0 |
| ItemData/Unit/Unit_SavasBorusu.asset | 120→130 | 10 | 0 | 20 | 0 |
| TowerData/ArcherDefaultİtem.asset | 100→0 | 0 | 0 | 0 | 0 |
| TowerData/MegicDefaultİtem.asset | 100→0 | 0 | 0 | 0 | 0 |
| TowerData/UnitDefaultItem.asset | 100→0 | 0 | 0 | 0 | 0 |

## Kontroller ve kapalı test

Canlı satın alma: üç kule sonrası 200 altın; başlangıç statları ve üç askerin 60 can/4 hasarı doğrulandı. 16 eşya/dükkan kontrolü geçti: kalkan ekleme/çıkarma, hız bonusu aktarımı, üç kule × dört nadirlik havuzu ve kritik üst sınırı.

Düşman ritmi: asker 0,1 sn aralıkla saldırırken 16,00 oyun saniyesinde 13 düşman vuruşu; asker 3 sn aralıkla saldırırken 33,28 oyun saniyesinde 27 düşman vuruşu. İkisi de düşmanın 1,2 sn aralığına uyuyor. Pause sırasında can değişmedi. Bu kontrollü testte asker canı ve düşman canı geçici yükseltildi, asker hasarı sıfırlandı; değişiklikler asset'e kaydedilmedi.

İlk eşya test oturumunda runtime Backpack referansı boş bulundu; test temiz Play oturumunda yeniden çalıştırıldı ve 16 kontrol geçti. Oturumlar arası canlı script yenileme güvenilir bir test zemini sayılmadı.

Testçilerden istenecekler: dalga başına kaçan düşman, kule yerleşimi, alınan eşyalar, askerlerin ortalama tutma süresi, yeniden doğmadan kaçan düşmanlar, harcanmayan altın ve tercih edilmeyen eşyalar. Aynı harita/tohum/yerleşim karşılaştırmalarıyla tek değişken grubunu ayarlamak gerekir. Dalga arası hazırlık ve shop kilitleri henüz tamamlanmadığından bu V1, mevcut dört test dalgası kapsamındadır. UpgradeTower'ın +5 hasar/+5 kritik puanı/-0,1 sn eski kuralı ve ücretsiz çağrılabilirliği sonraki upgrade tasarımında ele alınmalı; şu an bir satın alma UI akışına bağlı değil.

Yedek: Backups/2026-10-03-balance. Önce/sonra veri envanteri: tmp/balance-before.json ve tmp/balance-after.json.

Tam dalga kontrolü: başlangıca yakın üç ayrı slota birlik, okçu ve büyücü alındı; ek eşya alınmadı. Mevcut dört dalga 10× oyun zamanında Victory ile bitti; kale 64 can, düşman 0, asker 3. Bu tek koşudur; 1× oynanış hissi ve farklı yerleşimler kapalı testte ölçülecek.
