# Örnek bölüm 1 — Beş dalga

4 Ekim 2026. GameScane içindeki mevcut harita üzerinde ilk oynanabilir bölüm taslağı. Yeni bir sahne veya dalga mimarisi eklenmedi. PlaneTest korunuyor.

## Dalgalar ve düşmanlar

Hasar, düşmanın tek vuruş değeridir; mevcut Base kodu kaleye ulaşan düşmanda da aynı değeri kullanır. Saldırı aralığı birliklere yaptığı vuruşların bekleme süresidir. Ödül yalnız öldürülen düşman için verilir.

| Dalga | Düşman | Adet | Can / düşman | Hız | Hasar | Saldırı aralığı | Üretim aralığı | Altın / öldürme |
|---|---|---:|---:|---:|---:|---:|---:|---:|
| 1 | Goblin Keşifçi | 6 | 60 | 3 | 8 | 1,4 sn | 2 sn | 15 |
| 2 | Goblin Akıncı | 8 | 70 | 4 | 10 | 1,2 sn | 1,4 sn | 18 |
| 3 | Goblin Muhafız | 7 | 120 | 2,5 | 16 | 1,5 sn | 1,8 sn | 25 |
| 4 | Goblin Elit | 8 | 150 | 3 | 18 | 1,5 sn | 1,5 sn | 30 |
| 5 | Goblin Savaş Lordu — Boss | 1 | 900 | 1,5 | 40 | 2 sn | Tek düşman | 200 |

Keşifçiler başlangıç savunmasını denetler. Akıncılar hız ve daha sık üretimle baskı kurar. Muhafızlar daha yavaş ama dayanıklıdır. Elitler dayanıklılığı daha yüksek hız ve yoğunlukla birleştirir. Boss tek hedef hasarını ve birliklerin oyalama süresini sınar. Boss'un mevcut normal saldırısı kullanılır; yeni özel yetenek, zırh veya element mekaniği eklenmedi.

İlk ve her dalga arası hazırlık 60 saniye. Başlat ile erken geçilebilir; pause süreyi durdurur. Son düşman temizlenmeden sonraki hazırlık açılmaz. Beşinci dalga temizlenince mevcut Victory akışı çalışır.

## Ekonomi ve denge gerekçesi

Başlangıç altını 1.000. Birlik + okçu + büyücü toplam 225 + 250 + 325 = 800 altın; eşya veya sonraki kule için 200 kalır. Tam öldürmede ilk dört dalga sırasıyla 90, 144, 175 ve 240; boss 200 altın verir. Toplam olası düşman ödülü 849'dur. Kaçan düşman ödül vermez, dolayısıyla kötü savunma sonraki hazırlık bütçesini de azaltır.

Birinci dalga tamamen temizlenirse 200 + 90 = 290 altınla ek okçu veya birkaç eşya arasında seçim yapılabilir. Tek zorunlu alışveriş dizisi tasarlanmadı. Bu değerler kapalı testle ayarlanacak başlangıç dengesidir.

Mevcut GameOver, sonuç ekranında altını 1.000'e sıfırlıyor. Sonuç ekranındaki bakiye bölüm boyunca kazanılan toplam ödülü ölçmek için kullanılamaz.

## Kod değişikliği: kule satın alma kilidi

`GameManager.CanBuyTowers`, yalnız Preparation ve pause kapalıyken true döner. Eşya izninden ayrı bir property tutuldu; ileride bu iki kuralı farklılaştırmak istersek bağımsız değiştirilebilir. Yeni yönetici sınıfı yok.

`TowerSlot.OnMouseDown` izin yoksa hemen döner; savaş sırasında boş yuvaya tıklamak satın alma panelini açmaz veya seçili yuvayı değiştirmez.

`UİManager.RefreshPurchaseFeedback` satın alma düğmesinin interactable değerini günceller. Hazırlıkta seçilmiş ve açık kalmış panel savaş sırasında “Kule alımı kilitli · Hazırlığı bekle” mesajı gösterir. Panel kapatılırsa mevcut ReleaseSelection akışı yuvayı serbest bırakır.

`UİManager.BuyTower` da aynı izni kontrol eder. Böylece düğme devre dışı olsa bile metodun başka bir yerden çağrılması altın kesmez, kule oluşturmaz veya yuvayı doldurmaz. Hazırlığa dönünce normal satın alma akışı tekrar çalışır.

## İçerik dosyaları

`Assets/Resources/Data/EnemyData/Chapter01` altında Scout, Raider, Guard, Elite ve Warlord EnemyData asset'leri; `Assets/Prefabs/Chapter01` altında karşılık gelen beş prefab varyantı var. WaveManager'ın sahnedeki beş kaydı bu prefab'lara bağlı.

İlk dört varyant mevcut Goblin görselini kullanır; ölçekleri sırasıyla kaynak prefab'ın 0,90 / 0,85 / 1,10 / 1,20 katıdır. Boss mevcut BossEnemy görselini kullanır. İsimler ve güncel can/saldırı değerleri mevcut hover paneline EnemyData üzerinden yansır. Bunlar yeni çizilmiş beş ayrı model değildir.

Eski FastEnemy/BossEnemy verileri değiştirilmedi. Bölüme özel asset'ler sayesinde sonraki bölümlerin dengesi bunlardan bağımsız kurulabilir. Önceki scriptler ve sahne `Backups/2026-10-04-example-level` altında.

## Test kaydı ve kalan doğrulama

- Kule satın alma savaşta doğrudan BuyTower çağrısıyla denendi: altın ve kule sayısı değişmedi; yuva dolmadı. Başka yuvaya tıklama da seçimi değiştirmedi.
- Pause sırasında kule alımı engellendi. Hazırlıkta ilk üç kule ve sonraki ek okçular satın alınabildi.
- İlk denge denemesi 10× zamanda, başlangıçta bir birlik, bir okçu, bir büyücü; hazırlıkta para yettiğinde bir ek okçu alarak tamamlandı. Eşya alınmadı. Sonuç Victory, aktif ana kale **14/100 can**, toplam beş kuleydi.
- İlk denemede muhafız 140, elit 180, boss 1.000 candı. Başlangıç bölümü için daha fazla hata payı bırakmak amacıyla sırasıyla **120 / 150 / 900** olarak güncellendi ve asset'ler kaydedildi.
- Son değerlerle başlatılan ikinci koşunun kaydı bağlantı/Editor yeniden açılışı nedeniyle alınamadı. Bağlantı yeniden kurulduktan sonra aynı alışveriş kuralıyla 5× zamanda tekrar çalıştırıldı: **Victory, ana kale 64/100 can, toplam altı kule**. İlk üç dalga sonunda can 100; dördüncü dalga sonunda 64; boss sonrasında da 64 kaldı. Eşya satın alınmadı, yalnız kazanılan altın kullanıldı. Dalga 1, 3 ve 4 sonrasında birer okçu alındı.
- İlk koşunun otomatik ara raporu yanlışlıkla PlaneTest'teki etiketsiz Base'i okuyordu. Sonuç ayrıca kontrol edilerek aktif Base etiketli kalenin 14 canı doğrulandı; ikinci koşu aktif kaleyi izlemek üzere düzeltildi. PlaneTest değiştirilmedi.

Son koşuda Console hata/uyarı döndürmedi. Beş prefab'ın Enemy etiketi, EnemyData bağlantıları ve HealthBarUI bileşeni doğrulandı. Hızlandırılmış bu koşular fiziksel fareyle tam oynanış veya build testi değildir; tek yerleşim ve rastlantısal kritik vuruş koşusu dengenin kesinleştiği anlamına gelmez. Ham sonuç: `tmp/chapter-final-status.json`.
