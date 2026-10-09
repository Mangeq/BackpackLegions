# Kule XP ve element seçimi — 8 Ekim 2026

> 9 Ekim güncellemesi: Ana eşya evrimi, element mağazası ve düşman element kimliği artık uygulandı. Bu dosya 8 Ekim XP adımını anlatır; güncel devamı [Element evrimi](Element_Evrimi_2026-10-09.md) belgesinde.

Bu adım, Emir'in onayladığı mevcut sınıflarla uygulandı. Yeni bir XP yöneticisi yok. Element listesi kodda olduğu gibi **Normal, Ateş, Su, Toprak, Hava** olarak kaldı.

## Oyuncunun gördüğü akış

Düşmana hasar veren kule, düşman öldüğünde verdiği gerçek hasar oranında XP alır. Birliklerin katkısı kendi kulelerine yazılır. Kule savaş sırasında seviye atlar. Beşinci seviyeye ulaştığında element seçilene kadar seviyesi sabit kalır; XP birikmeye devam eder. Hazırlıkta kule panelini açıp dört elementten biri seçilir. Bekleyen XP o anda işlenir; yeterliyse birden fazla seviye kazanılır.

Seçim bu sürümde bir defalıktır. Savaşta, pause sırasında ve sonuç ekranında seçim yapılamaz. Kule panelinde XP sayısı, çubuğu ve bekleme nedeni görünür; hover bilgi paneli de XP ve elementi gösterir.

## Kod akışı ve sorumluluklar

1. **EnemyData.experienceReward** düşmanın ölümde vereceği toplam XP'dir. Altın ödülünden bağımsızdır.
2. **ArcherTower.Attack / MegicTower.Attack**, Bullet.Initialize'a `this` gönderir. Bullet bu kule referansını ve ateşleme anındaki elementi saklar. İsabette Enemy.TakeDamage'a aktarır. Element henüz seçilmediyse mevcut BulletData türü korunur. Hava hedefi yok olduğunda merminin son konumu artık havadaki hedef konumu olarak korunur.
3. **Unit.Combat**, hasarı `myTower.GetComponent<Tower>()` sahibine bağlar. **UnitTower.ApplyStatsTo** seçilen elementi mevcut askerlere ve sonradan doğanlara uygular. Can oranını koruyan mevcut ApplyStats akışı değişmez.
4. **Enemy.TakeDamage**, zayıflık/direnç çarpanını uyguladıktan sonra gerçekten eksilen canı hesaplar. `damageContributions` sözlüğünde kule başına katkı tutar. Negatif, NaN ve sonsuz girişler yok sayılır. Örnek: 100 canlık düşmanda A 60 hasar verir, B 400 vurur; B'nin katkısı yalnız kalan 40 candır. 100 XP ödülü A'ya 60, B'ye 40 olarak dağıtılır. Kesirli XP yuvarlanmadan saklanır.
5. **Enemy.Die** ödülü yalnız bir kez dağıtır. Sahipsiz hasarın payı başka kuleye aktarılmaz; yok olmuş kulenin payı da diğerlerine dağıtılmaz. **Base.OnTriggerEnter → Enemy.TryReachBase** düşmanı çözümlenmiş olarak işaretler. Böylece kale teması sonrası aynı karede gelen mermi XP kazandıramaz ve bir düşman kaleye iki kez vuramaz.
6. **Tower.AddExperience → ProcessExperience** XP'yi biriktirir ve eşikleri sırayla tüketir. `AwaitingElement` doğruysa döngü durur. **TrySelectElement** hazırlık/pause/seviye/enum kontrollerini yapar, elementi yazar, biriken XP'yi işler, statları askerlere tekrar aktarır. TowerData asset'ine seviye veya savaş statı yazılmaz; her kule bağımsızdır.
7. Eski **UpgradeTower** metodu olası Inspector bağlantıları için kaldı, fakat artık ücretsiz seviye atlatmaz. Yalnız mevcut XP'yi işler; element kapısını aşamaz.
8. **UİManager** seçili kulenin XP ve elementini okur. Dört düğme Inspector'da kalıcı olarak SelectTowerElement'e bağlıdır. Hesaplamayı UI yapmaz. Seçim tamamlandığında düğmeler gizlenir. **InfoPanelUI** yalnız mevcut değerleri gösterir.

## İlk sayısal ayarlar

TowerData Inspector'ında `First Level Experience = 20`, `Experience Increase Per Level = 10`. Eşik formülü: `20 + (mevcut seviye - 1) × 10`.

| Geçiş | Gereken XP | Birinci seviyeden toplam |
|---|---:|---:|
| 1 → 2 | 20 | 20 |
| 2 → 3 | 30 | 50 |
| 3 → 4 | 40 | 90 |
| 4 → 5 | 50 | 140 |
| 5 → 6 | 60 ve element seçimi | 200 |

Seviye başına kodda zaten bulunan artışlar korunur: temel hasar +5, kritik +5 yüzde puanı, temel saldırı aralığı −0,1 saniye. Kritik en fazla %100, saldırı aralığı en az 0,1 saniyedir. Eşya bonusları bu temel değerler üzerinden yeniden hesaplanır.

| Örnek bölüm düşmanı | XP |
|---|---:|
| Keşifçi | 20 |
| Akıncı | 25 |
| Muhafız | 30 |
| Gök Perisi | 30 |
| Elit | 40 |
| Savaş Lordu | 150 |

Diğer eski EnemyData asset'leri varsayılan 20 XP kullanır. Bu değerler ilk ayardır; kapalı test dengesi değildir. Örnek olarak 300 XP alan kule seviye 5'te 160 XP tutar. Element seçince 60 ve 70 XP harcayarak seviye 7 olur, 30 XP kalır.

## Kapsam sınırı

Seçilen element mevcut düşman zayıflık (×1,5) ve direnç (×0,5) listeleriyle gerçek hasarı etkiler. Yeni düşman element kimliği, yakma/dondurma gibi durum etkileri, element değiştirme cezası ve elemente özel eşya havuzu eklenmedi. Bunlar yol haritasında açık. Mağazanın mevcut kule türü/nadirlik filtresi sürer; seviyeyi yeni teklif üretildiğinde okur. Element seçimi mevcut teklifleri veya dondurmayı bozmaz. Bu sistem bölüm içidir; kalıcı kayıt/yükleme eklenmedi.

## Kontrol ve geri dönüş

- 26 davranış kontrolü: katkı, fazla hasar, element çarpanı, ölümün tek ödülü, kale teması, sahipsiz/yok olmuş kaynak, geçersiz hasar, seviye 5 kapısı, XP birikmesi, hazırlık/pause/sonuç kısıtları, bir defalık seçim, dört elementli mermi, askerin elementi ve XP sahibi.
- Gerçek sahnede 6 panel kontrolü ve 3 seçim/mermi bağlantısı kontrolü geçti. UI düğmelerinin kalıcı onClick olayları çağrıldı; fiziksel fare tıklaması otomasyonu yapılmadı.
- Aşama 1 regresyonu: 28/28 geçti; Console hata/uyarı yok. Play kapatıldı, runInBackground eski değerine döndü.
- Ekran görüntüsü: `Docs/Validation/tower-xp-2026-10-08.png`.
- Raporlar: `xp-behavior.json`, `xp-ui.json`, `xp-ui-selection.json`.
- Kaynak ve sahne yedeği: `Backups/2026-10-08-xp`. Unity sahnesi Editor API üzerinden değiştirildi; YAML elle düzenlenmedi.
