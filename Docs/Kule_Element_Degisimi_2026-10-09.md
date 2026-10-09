# Kule ve element değiştirme — 9 Ekim 2026

Emir'in onayladığı oyun kuralları ve mevcut sınıf dağılımı uygulandı. Yeni manager eklenmedi. Bu belge aynı günün önceki ilk-element uygulamasını tamamlar; GDD 4.2–4.3'teki zorunlu satış akışının yerine üzerinde anlaşılmış dönüşüm düğmelerini kullanır.

## Oyuncu akışı

Kule panelinde çantanın altında iki düğme vardır. İşlemler yalnız hazırlıkta, pause kapalıyken yapılır. Seçim ekranı fiyat ve sonuçları gösterir; oyuncu Onayla veya Vazgeç der. Ekran açılması/seçenek seçilmesi altın kesmez. Savaş başlarsa veya kule/panel değişirse açık onay geçersiz olur.

### Element değişimi

- İlk uzmanlaşma hâlâ seviye 5'te ücretsizdir. Ana eşya evrilir; mevcut mağaza teklifleri ve dondurma sonraki yenilemeye kadar korunur.
- Sonraki değişimler **100 altın**. Seviye ve XP korunur. Ana eşya aynı konum/dönüşte yeni element karşılığına dönüşür.
- Normal, mevcut kule türüne uygun eşyalar çalışır. Eski elemente özel eşyalar oyuncuda kalır ama bonus, sinerji veya craft katkısı vermez. Çantada gri görünür ve **UYUMSUZ / Bonus yok** etiketi alır. Yeniden uyumlu elemente dönülürse tekrar çalışabilir.
- Ücretli element değişiminde mağaza hemen yeni havuzdan yenilenir ve dondurma kaldırılır; bu sonuç onay ekranında belirtilir.
- Aynı elementi tekrar seçmek ücret kesmez ve işlem yapmaz. Ana silah satılmaz, kule silahsız kalmaz.

### Kule tipi değişimi

- Çantada ana eşya dışında hiçbir eşya bulunmamalıdır. Oyuncu diğer eşyaları satabilir veya yer varsa depoya taşıyabilir. Düğme, çanta doluyken “Tür için çantayı boşalt” yazar ve kapalıdır.
- **Depo ve teslim bekleyen eşyalar boşaltılmak zorunda değildir.** Olduğu gibi yeni kuleye geçer; bonus vermez.
- Eski ana eşya kaldırılır; yeni türün başlangıç eşyası yeni çantaya yerleşir. Yeni kule **seviye 1, XP 0, Normal element** ile başlar.
- Aynı dünya konumu ve dolu yerleşim slotu korunur. Mağaza yeni türe uygun Normal eşyalarla yeniden kurulur; dondurma kapalıdır.
- Eski birlik kulesinin askerleri ve yeniden doğma coroutine'leri kaldırılır. Yeni tür birlik kulesiyse kendi askerlerini bir kez oluşturur.
- Oyuncunun eşyaları otomatik satılmaz. Eski türe ait eşyalar yeni kuleyle uyumsuzsa pasif kalır; depolanabilir ve satılabilir.

## İlk ücretler ve Inspector ayarı

TowerData'da iki yeni alan var:

| Alan | Başlangıç | Etki |
|---|---:|---|
| elementChangeCost | 100 | İlk seçimden sonraki her element değişiminin sabit bedeli |
| typeChangeRefundRate | 0,5 | Eski kule etiket fiyatının yeni kule alışından düşülecek oranı |

Kule tipi değişim ücreti:

`max(0, yeni kule fiyatı − floor(eski kule fiyatı × iade oranı))`

Mevcut fiyatlarla okçu → büyücü: `325 − 125 = 200 altın`. Büyücü → birlik: `225 − 162 = 63 altın`. Bu iade satın alma indirimi olarak kullanılır; negatif ücretle fazladan altın ödenmez. Eşya yatırımları iade hesabına girmez, çünkü eşyalar zaten oyuncuda kalır.

Cezayı ağırlaştırmak için `elementChangeCost` artırılabilir veya `typeChangeRefundRate` düşürülebilir. Katlanan ceza, sayaç veya dalga başına değişim sınırı eklenmedi. Bunlar istenirse ayrıca kararlaştırılır.

## Kodun görevleri ve işlem sırası

### Tower: oyun kuralı ve dönüşüm

`TrySelectElement` ücretsiz ilk seçim içindir; eski davranışını korur. `TryChangeElement` sonraki ücretli işlemdir. İkisi `ApplyElement` üzerinden aynı ana-eşya doğrulamasını kullanır. Yeni şekil sığmıyorsa Backpack.TryReplace eski kaydı değiştirmez, ücret kesilmez. Başarılı işlem statları, mevcut askerleri ve açık paneli yeniler. Ücretli yol mağazayı da yeniler; ücretsiz ilk seçim mevcut teklifleri korur.

`GetElementChangeError`, `GetTypeChangeError` ve fiyat metotları hem onay ekranı hem gerçek işlem tarafından kullanılır. UI düğmesi kapalı olsa bile kod doğrudan çağrılırsa hazırlık, aynı seçim, altın ve çanta kuralları tekrar kontrol edilir.

`TryChangeTowerType` sırası:

1. Aktif sürüklemeyi geri alır. Geçici olarak grid dışında tutulan eşya, “boş çanta” kontrolünü aşamaz.
2. Hazırlık, mevcut çanta, hedef tür, yeni ana eşya/çanta ölçüleri ve altın kontrolünü yapar.
3. Yeni prefabı aynı konum/dönüşte oluşturup temel kule durumunu hazırlar. Yeni kurulum başarısızsa eski kule ve altın korunur.
4. Ücreti keser; depo referansını ve teslim kayıtlarını yeni kuleye aktarır.
5. Eski birlikleri temizler. Eski kuleyi `retired` olarak işaretler, anında pasifleştirir ve kaldırır. Aynı karedeki ikinci çağrı ücret kesemez, eski kule yeni XP alamaz.

`InitializeTower` tek-seferlik kurulum sağlar. Dönüşümde UI yeni çantayı hemen açabilsin diye kurulum önden yapılır; sonraki Unity Start çağrısı bunu tekrarlamaz. UnitTower.Start yine kendi askerlerini normal akışta oluşturur. `startAtLevelOne` dönüşümde seviye 1 kuralını sağlar; paylaşılan TowerData.towerLevel değiştirilmez.

### ItemData ve hesaplar: uyum tek yerden

`ItemData.IsCompatibleWith(type, element)` iki şeyi birlikte kontrol eder: eşya yeni kule türüne uygun mu, Normal mi veya seçili elementle aynı mı? `Tower.IsItemActive` aktif ana eşyayı da tanır.

`CalculateStats` yalnız Backpack.Items içindeki uyumlu eşyaları toplar. Depo hâlâ hesap dışıdır. `ItemSynergyCalculator` isteğe bağlı uyum filtresi alır; uyumsuz komşu başka bir eşyanın sinerjisini de aktifleştiremez. `TowerItemEvolution` uyumsuz malzemeler veya sonuçlarla craft başlatmaz/tamamlamaz.

### TowerItemEvolution ve UnitTower: sahiplik

`TransferDeliveriesTo` bekleyen teslimleri yeni kuleye tek kez taşır ve eski listeyi boşaltır. Depo grid'i doğrudan devredildiği için eşya konumu, dönüşü ve doluluk korunur. Yeni UI teslimi yeni kulenin Evolution nesnesinden alır; teslim iki kez çoğaltılamaz.

`UnitTower.RemoveUnitsForTypeChange` eski askerlere ait coroutine'leri durdurur, bağlı düşman varsa angajmandan çıkarır, askerleri pasifleştirip kaldırır. Eski kule sonradan asker doğuramaz.

### UİManager ve BackpackUIManager: gösterim

Mevcut UİManager dönüşüm seçeneklerini ve onay ekranını tutar. Açıldığı kuleyi `transitionOwner` ile hatırlar; başka kuleye geçilirse önceki onay kullanılamaz. Onay anında fiyat ve koşullar yeniden okunur. Başarısız işlem nedeni ekranda kalır; yeni seçenek seçince tekrar değerlendirilir.

Kule tipi değişince aktif kule referansı ve açık çanta yeni kuleye yönelir. BackpackUIManager uyumsuz eşyayı gri gösterir ve fontu açıkça bağlı bir etiket oluşturur. InfoPanelUI de uyumsuzluğu belirtir. UI bonus hesaplamaz; Tower/ItemData sonucunu gösterir.

## Testler ve sınırlar

- **29 çekirdek kontrol:** ücret, XP/seviye korunması veya sıfırlanması, envanter/depo aktarımı, teslim sahipliği, eski asker temizliği, slot konumu, mağaza, tekrar çağrı, uyum ve sinerji/craft.
- **14 canlı UI kontrolü:** vazgeç, eksik altın, onay düğmesinin kalıcı event bağlantısı, iki kez onay, savaş başlarken iptal, yeni askerlerin bir kez oluşması, doğru panel/mağaza, sürüklemeyle çanta kontrolünü aşamama.
- **8 sınır kontrolü:** okunabilir uyumsuzluk etiketi, sığmayan ana eşya, kalıcı hata mesajı, eksik para, sonuç ekranı, devredilen teslimi ücretsiz alma ve uyumsuz eşyayı satma.
- Önceki **26 XP**, **26 element** ve **28 Aşama 1** kontrolü geçti. Console hata/uyarı yok.

İlk test denemesinde fixture, mevcut havuzda bulunmayan tek hücreli ve hasar veren normal büyücü eşyası aradı; null veriyi kullanmadan geçerli bir eşya seçen fixture ile düzeltildi. Görsel inceleme fontu eksik etiketi de yakaladı; font bağlanarak yeniden görüntü alındı. Bunlar sonuç raporlarında geçen başarılı koşulardan önce düzeltildi.

Testler sentetik olay/metot çağrılarıyla yapıldı; fiziksel fareyle uzun oyun oturumu veya kapalı test denge geri bildirimi yerine geçmez. İlk ücretler deneme ayarıdır. Yeni model/animasyon eklenmedi.

Raporlar: `Docs/Validation/transition-core-2026-10-09.json`, `transition-ui-2026-10-09.json`, `transition-edges-2026-10-09.json`, `transition-xp-regression-2026-10-09.json`, `transition-element-regression-2026-10-09.json`, `stage1-latest.json`.

Görseller: [Element onayı](Validation/element-change-confirm-2026-10-09.png), [Kule tipi onayı](Validation/tower-type-confirm-2026-10-09.png), [Uyumsuz eşya](Validation/transition-incompatible-2026-10-09.png).

Yedek: `Backups/2026-10-09-transitions`. Sahne UI'si Unity Editor API üzerinden eklendi; sahne/prefab YAML'ı elle değiştirilmedi. Eski GDD PDF dosyaları değiştirilmedi; güncel kararlar yol haritasında ve bu belgede tutuluyor.
