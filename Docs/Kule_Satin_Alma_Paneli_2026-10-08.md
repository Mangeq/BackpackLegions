# Kule satın alma paneli — 8 Ekim 2026

Boş yere tıklanınca ekranın sağında dört dikey ikon açılır. İlk açılışta hiçbir kule seçili değildir ve detay kartı gizlidir. İkona tıklanınca bilgi kartı ikon şeridinin hemen solunda açılır. İkon sırası Büyü, Topçu, Okçu, Birlik'tir. Seçilen ikon altın rengiyle vurgulanır.

Topçu henüz projede çalışan bir kule/prefab olmadığı için dört seçenekten bu biri “Yakında” durumundadır. İkon açıklamayı açar fakat satın alma yapılamaz. Yeni bir topçu savaş sistemi bu arayüz çalışmasının kapsamına eklenmedi.

## Neler gösterilir?

Kartta kule adı, gerçek prefab modelinin portresi, kısa görev açıklaması, hasar, atış aralığı, kritik şansı ve çanta boyutu görünür. Okçu/büyücü menzil; birlik kulesi asker sayısı ve yeniden üretim süresi gösterir. Birlikte hasarın asker başına olduğu yazılır.

Fiyatın kaynağı TowerData.cost'tur; görseldeki örnek fiyatlar kullanılmadı. Statlar prefab'ın TowerData verisinden ve satın alındığında verilen ana itemin temel katkılarından hesaplanır. Böylece fiyat/denge ayarı Inspector'dan değişince kart da güncellenir. Stat adları ve değerleri ayrı sütunlara hizalandı.

## Kod akışı

Yeni runtime sınıfı eklenmedi. Mevcut UİManager ve TowerSlot görev dağılımı korundu.

1. TowerSlot.OnMouseDown mevcut UI/faz/doluluk kontrolünü yapar, önceki boş yerin geçici kilidini kaldırır, yeni yeri seçer ve TowerShopUI'ı açar. PreparePurchasePanel açık çanta panelini kapatır, seçimi ve kart görünümünü yeniler.
2. Mevcut MegicTowerBTN, ArcherTowerBTN, UnitTowerBTN metodları seçili türü belirler. Yeni CannonTowerBTN yalnız “yakında” seçeneğini gösterir.
3. RefreshPurchaseFeedback seçime uygun prefab'ı bulur. RefreshPurchaseCard bu prefab'ın verilerini kartın metin ve portre alanlarına yazar. Seçim yoksa detay kartı kapalıdır. Fiyat yetmiyorsa düğme etkileşimi kapanır, eksik altın miktarı yazılır. BuyTower içindeki gerçek para/faz kontrolleri ayrıca korunur; düğmenin görünümü tek güvenlik kontrolü değildir.
4. BuyTower mevcut satın alma yoluyla altını düşer, kuleyi seçilen yere kurar, yeri dolu işaretler ve paneli kapatır. İkinci çağrı yeniden ücret alamaz çünkü yer/tür seçimi temizlenmiştir.
5. ClosePurchasePanel mevcut CancelTowerPurchase yolunu kullanır: satın alınmayan slotun collider'ını yeniden açar, yer/tür referanslarını temizler. Savaş başlayınca Update aynı kapatma yolunu çağırır. Pause sırasında alım kilidi devam eder.

## Sahne ve görseller

Canvas/TowerShopUI altında IconRail ve PurchaseDetails düzenlendi. Eski satın alma/seçim düğmeleri taşınarak mevcut UnityEvent bağlantıları korundu. Eski arka plan ve kullanılmayan başlıklar gizlendi. Canvas'ın diğer panelleri yeniden kurulmadı.

Sağ kenara anchor verildi; küçük ekranlarda panel kendi alanı içinde küçülür. Büyü/okçu/birlik portreleri gerçek prefab modellerinden 384×384 transparan PNG olarak render edildi. Topçu ikonu mevcut çevre paketindeki top önizlemini kullanır. Görseller Assets/Prefabs/UI/PurchaseArt altındadır. Geçici render kamerası/model/ışıklar işlem sonunda kaldırıldı; oyun sahnesinde ek önizleme kamerası bırakılmadı.

## Kontroller

Yeni akış için 10/10 kontrol: seçim olmadan ikon açılması, kart/fiyat gösterimi, kapatınca yerin serbest kalması, başka yere geçiş, topçuda ücret alınmaması, yetersiz altın, pause, tek satın alma, tekrar çağrı ve sağdaki yerleşim. Ek savaş kontrolü 1/1: panel kapanır ve boş yerlerin geçici kilidi kalkar. Son görünüm oyun ekranından incelendi.

Raporlar: Validation/purchase-panel-2026-10-08.json ve Validation/purchase-combat-2026-10-08.json. Görsel: Validation/tower-purchase-panel-2026-10-08.png. Kontroller sentetik olay/metot çağrılarıyla yapıldı; gerçek oyuncu tıklama testi ayrıca yapılabilir.

Bu adımdan önceki script ve sahne yedekleri Backups/2026-10-08-tower-purchase içindedir.

Mevcut satın alma, taşıma, reroll ve savaş döngüsünün regresyon kontrolleri de 28/28 geçti (Validation/stage1-latest.json). Son konsol kontrolü temiz; test sonunda Play kapalı, sahne kayıtlı ve geçici kuleler kaldırılmış durumda.
