# Eşya döndürme ve yükseltme izni — 6 Ekim 2026

## Kullanım

Hazırlıkta, dükkandan veya çantadan eşya sürüklerken R ve scroll yukarı saat yönünde 90°, scroll aşağı ters yönde 90° döndürür. Scroll'un sıfırdan farklı okunduğu karede yönüne göre bir adım uygulanır. Sürükleme yokken bu girdiler eşya döndürmez. İkon ve yerleşim önizlemesi birlikte güncellenir; mouse hareketi gerekmez.

Mevcut centerOffset tutma hücresi dönüşle birlikte hesaplanır ve imlecin altında kalır. Bu, eşyanın tıklanan herhangi bir noktasını yeni tutma noktası yapmaz; projedeki belirlenmiş tutma hücresi davranışını korur. Geçersiz bırakma, panel kapatma, kule değiştirme, pause veya savaş başlangıcı eski konumu ve yönü geri getirir. Başarısız dükkan sürüklemesi altın harcamaz; başarılı satın alma bir kez ücretlenir. Çanta içi döndürme/taşıma ücretsizdir.

## Onaylanan görev dağılımı

### PlacedItem: eşyanın kendi dönüşü

Rotation 0–3 çeyrek tur olarak saklanır; 0 başlangıç, 1 saat yönünde 90°, 2 180°, 3 270°. Constructor dönüşü normalize eder. GetOccupiedCells ve BoundingSize dönmüş şekli kullanır. Yön bu nesnede tutulduğu için aynı ItemData'yı kullanan başka eşyalar etkilenmez. Kalıcı save sistemi eklenmedi.

### ItemShapeUtility: ortak dönüş hesabı

Grid'de X sağa, Y aşağı artar. Bir saat yönü adımı `(x,y) → (yükseklik−1−y,x)` dönüşüdür; genişlik ve yükseklik yer değiştirir. NormalizeRotation negatif yönü ve dört turu 0–3 aralığına indirger. GetRotatedShape özgün ItemData hücrelerini değiştirmeden dönmüş hücreler üretir. RotateCell tutma hücresini de aynı kuralla dönüştürür; L şekillerinin boşlukları dolu sayılmaz.

### Backpack: çakışma ve yerleşim

CanPlace ve Place isteğe bağlı rotation parametresi alır. Eski çağrılar varsayılan 0 ile çalışır. Çakışma, sınır kontrolü, yerleştirme ve Remove aynı dönmüş hücreleri kullanır. Place'in mevcut sözleşmesi korunur: çağıran önce CanPlace ile doğrular.

### BackpackDraggable: girdi ve işlemin geri alınması

Update yalnız aktif sürüklemede R/scroll okur ve ikisini aynı Rotate metoduna gönderir. Rotate faz ve kule sahibini kontrol eder, geçici Rotation'ı değiştirir ve mouse sabitken de önizlemeyi yeniler. BeginDrag eski Origin ve Rotation'ı saklar. FinishDrag başarılıysa yeni yönle Place çağırır; başarısızsa saklanan iki değeri geri yükler. CancelDrag aynı geri dönüşü pause, panel kapanması ve savaş başlangıcında kullanır.

Pointer konumu/kamerası saklanır; yeniden çizimde ScreenPointToWorldPointInRectangle ile Canvas koordinatına çevrilir. Böylece dönüş sırasında mouse hareket event'i beklenmez.

### BackpackUIManager: ikon ve hücre görünümü

ConfigureItemVisual ikonun özgün boyutunu korur ve Z ekseninde -90° × Rotation döndürür; yalnız genişlik/yükseklik değiştirip görseli esnetmez. Yerleşmiş görsel, dönmüş hücre dikdörtgeninin merkezine konur. Sürüklenen görselin pivotu centerOffset hücresinin merkezidir. ShowPlacementPreview rotation parametresiyle aynı hücreleri yeşil/kırmızı boyar.

ShopSlotDragStarter'daki tek ek çağrı, ghost oluşturulur oluşturulmaz pointer bilgisini UpdateDrag'e verir. Altın ve yerleşim kararı yine BackpackDraggable'dadır. ItemData'daki eski “döndürme yok” yorumu güncellendi; veri asset'leri değiştirilmedi.

### GameManager ve Tower: yükseltme izni

CanUpgradeTowers yalnız Preparation ve pause kapalıyken true döner. Tower.UpgradeTower girişinde bu izin kontrol edilir; savaş/pause/sonuçta stat veya seviye değişmez. Yükseltme ekranı, ücret, XP ve element akışı eklenmedi; bu adım yalnız mevcut yükseltme metodunun faz güvenliğidir.

## Doğrulama

- 18 geometri/veri kontrolü: L şeklinin dört yönü, sınırlar, çakışma, Remove, ters yön, dört tur ve paylaşılan verinin korunması.
- 12 canlı UI/akış kontrolü: dönmüş alışveriş, tek ücret, ikon-hücre hizası, mouse sabitken dönüş, ters yön, geçerli taşıma, geçersiz bırakmada geri alma, pause/kapama/savaşta iptal, yeniden açma ve yükseltme izinleri.
- Mevcut satın alma, taşıma, dükkan, savaş ve sonuç akışının Aşama 1 regresyonu 28/28 geçti; Console sorgusunda hata/uyarı yok. Son durumda Play kapalı, sahne dirty=false, runtime kuleleri 0. Kanıt: Docs/Validation/stage1-latest.json.
- Testler Unity içinde gerçek kod, UI ve pointer event'leriyle çalışır. Rotate doğrudan çağrılır; fiziksel klavye R ve mouse tekerleği donanım girişi otomatik olarak basılmadı. Girdi yönlendirmesi kodda kontrol edildi.
- Geometri ve UI test kaynakları Tools/Validation/rotation_math.cs ve rotation_ui.cs; kanıt Docs/Validation/rotation-2026-10-06.json. UI testi özel RotationTestTower ve boş çanta/örnek veri ile kontrollü Play oturumu gerektirir, tek başına normal sahnede çalıştırılmamalıdır.
- Önceki kod yedeği Backups/2026-10-06-rotation altında. Yeni sahne/prefab bağlantısı gerekmedi.
