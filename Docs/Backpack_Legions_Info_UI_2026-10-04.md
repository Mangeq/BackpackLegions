# Bilgi panelleri ve can barları

4 Ekim 2026. Emir, tek ortak InfoPanelUI + HoverInfoController + HealthBarUI görev dağılımını onayladı. GameManager durum sahibi olarak kaldı; savaş hesabı veya denge değerleri değiştirilmedi.

## Kullanım

Dükkandaki ve çantadaki eşyanın, sahnedeki kule/asker/düşman gövdesinin üzerine gelince yaklaşık 0,15 saniye sonra bilgi paneli açılır. Hedef değişince içerik değişir. Fare hedefi terk edince, sol tuş basılıyken/sürüklerken ve oyun sonucu açılınca panel kapanır. Menü/çanta gibi UI öğeleri arkalarındaki dünya hedefini engeller. Pause sırasında menü dışında kalan hedefler incelenebilir.

Eşya: ad, nadirlik, kullanılabilen kule, fiyat, kapladığı alan ve sıfır olmayan bonuslar. Kritik bonusu yüzde puanı olarak belirtilir. İksirlerin tüketilmeyen pasif eşya olduğu yazılır.

Kule: seviye, eşyalardan sonraki hasar, saldırı aralığı ve kritik. Okçu/büyücü ayrıca menzil; birlik kulesi asker sayısı, yeniden üretim süresi ve can bonusu gösterir. Birlik kulesindeki saldırı statları asker başınadır.

Asker: mevcut/maksimum can, güncel saldırı statları, kritik çarpanı ve element. Düşman: mevcut/maksimum can, hasar, saldırı aralığı, hareket hızı, ödül, zayıflık ve dirençler.

Asker ve düşmanların üstünde can barı ve sayısal can görünür. Yeşil asker, kırmızı düşmandır. Tam can dahil sürekli görünür; ekran dışındaki/ölen hedefin barı gizlenir. Barlar tıklama ve sürüklemeyi yakalamaz.

## Kodun görevleri ve veri akışı

### HoverInfoController.cs

Update yalnız fare konumunu ve sol tuş durumunu okur; UpdateHover bu girdilerden hedef ve panel durumunu belirler. Bu ayrım, aynı akışın sentetik konumlarla test edilmesini sağlar.

FindTarget önce EventSystem üzerinden UI raycast sonuçlarını inceler. ShopSlotUI.currentItem dükkandaki, BackpackDraggable.Item çantadaki eşyayı verir. Başka bir UI öğesi üstteyse dünya hedefi aranmaz. Tooltip ve can barlarında raycastTarget kapalı olduğu için kendi kendilerini hedeflemezler.

UI yoksa kameradan dünyaya raycast atılır. Düşman/asker gövdeleri trigger olduğu için trigger'lar dahildir. Kulelerin dev menzil küreleri özellikle atlanır; kule gövdesi seçilir. En yakın katı engelin arkasındaki nesne seçilmez.

Hedef sabit kaldığında 0,15 saniyelik bekleme uygulanır. Panel verisi 0,1 saniyede bir yenilenir; konumu her kare fareyi izler. Time.unscaledTime kullanıldığı için hazırlık/pause durumunda zaman ölçeği sıfır olsa da çalışır. Oyun sonunda kapanır. Tek Instance referansı, HealthBarUI bileşenlerinin sahnedeki ortak bar katmanına ulaşmasını sağlar; yeni bir oyun durumu yöneticisi değildir.

### InfoPanelUI.cs

Show hedefin tipini kontrol eder: ItemData, Tower, Unit veya Enemy. İlgili verileri okuyup başlık/alt başlık/metin/ikon/rengi doldurur. Bonus hesaplamaz; Tower.CalculateStats ve Unit.ApplyStats sonuçlarını okur. ItemData üzerindeki bonus değerleri yalnız açıklama için yüzdeye çevrilir.

Örnek: kritik bonusu 0,12, +12 yüzde puanı olarak gösterilir. Hız alanı currentAttackSpeed saniye cinsinden aralık olduğundan panelde “saldırı aralığı” yazar; hız ile süre karıştırılmaz.

MoveTo fare konumunu Canvas koordinatlarına çevirir. Sağda yer kalmazsa paneli farenin soluna alır; dikey/yatay konumu Canvas sınırlarına sıkıştırır. Panel sol üst pivot kullanır. Hide yalnız görsel paneli kapatır; controller çalışmayı sürdürür.

### HealthBarUI.cs

Aynı bileşen UnitPrefab, Goblin ve BossEnemy prefablarında bulunur. Awake hedefin Unit/Enemy bileşenini, gövde collider'ını ve kamerayı saklar. LateUpdate mevcut/maksimum canı okur. Barı ortak HealthBarLayer altında oluşturur; gövdenin bounds.max.y yüksekliğini ekran koordinatına dönüştürerek üstünde tutar.

Doluluk oranı Clamp01(current / maximum) değeridir. Örneğin 48/60 → 0,8. Görünen can sayısı yukarı yuvarlanır; bar oranı gerçek kesirli değeri kullanır. Metin sadece sayı değişince yeniden yazılır. Can bonusuyla maksimum can değişirse yeni oran otomatik okunur.

OnDisable/OnDestroy barı siler. Yeniden doğan asker yeni prefab instance'ı olduğu için kendi barını oluşturur. UI canı değiştirmez.

### Mevcut sınıflardaki küçük ekler

- Enemy: CurrentHealth ve MaxHealth salt okunur özellikleri. CurrentHealth negatif değeri göstermez; mevcut savaş değişkeni ve hasar hesabı aynı kalır.
- Unit: CurrentHealth ve MaxHealth salt okunur özellikleri. MaxHealth, ApplyStats tarafından hesaplanan değeri verir; prefabın başlangıç canı sanılarak sabit sayı kullanılmaz.
- BackpackDraggable: Item salt okunur özelliği. Hover kontrolü mevcut özel data alanını okuyabilir, değiştiremez.

## Unity bağlantıları

Canvas altında HealthBarLayer ve HoverInfoUI oluşturuldu. HoverInfoUI üzerinde InfoPanelUI ve HoverInfoController, altında başlangıçta kapalı InfoPanel var. InfoPanelUI alanlarına başlık, alt başlık, gövde metni, ikon, vurgu çizgisi ve panel RectTransform bağlandı. HoverInfoController alanlarına InfoPanelUI ve HealthBarLayer bağlandı. Inspector'dan hoverDelay değiştirilebilir.

Assets/Prefabs/UI/HealthBarVisual.prefab: Track/Fill ve HealthText. Üç aktör prefabındaki HealthBarUI.barPrefab bu prefabı kullanır; heightOffset=0,25. İsimler script tarafından bu prefab içinde aranır; Track/Fill veya HealthText yeniden adlandırılırsa kod da güncellenmelidir.

Mevcut Canvas ayarları, savaş prefablarının collider'ları, PlaneTest alanı ve satın alma/sürükleme kod akışı korunmuştur.

## Doğrulama

- 30 eşyanın adı, ikonu, fiyatı ve metin sığması; üç kule türünün güncel statları.
- Üç kule gövdesi ve trigger kullanan asker/düşman için ekran koordinatından hedef bulma.
- Dükkan ve çanta eşyası hover'ı; UI arkasını seçmeme; tooltip'in tıklamayı engellememesi.
- 1920×1080 Game View'da dört ekran köşesinde panel sınırı; eşya ve düşman ekran görüntüleriyle metin/bar kontrolü.
- Askerde 48/60, düşmanda 63/70 doluluğu; can eşyası eklenince 65/81 değerinin hem panelde hem barda görünmesi.
- Hazırlıkta timeScale=0 iken hover; pointer basılıyken kapanma; bırakınca açılma; ekranı terk edince kapanma; pause sırasında menü dışındaki hover; GameOver sonrası kapanma.
- Düşmanın ölümünde panelin kapanması ve barının silinmesi. Asker ölümü/yeniden üretimi sonrası üç asker ve üç bar; artık bar kalmaması.

Testler mevcut metotlara ve ekran koordinatlarına sentetik girdilerle yapıldı; fiziksel fareyle uçtan uca kullanıcı testi veya başka ekran çözünürlüklerinde görsel kontrol yapılmadı. İlk dükkan/çanta kontrolü UI etkinleştiği aynı karede çalıştırıldığında raycast kaydı hazır değildi; bir sonraki karede ikisi de geçti. İlk düşman bar kontrolü ekran dışındaki hedefteydi; görünür konuma taşındığında doğru doluluk doğrulandı. Pause menüsünün dışında hover yapılabilmesi beklenen davranıştır.

Son kontrol: Console hata/uyarı döndürmedi. Test nesneleri kaydedilmez. Yedek: Backups/2026-10-04-info-ui. Görüntüler: tmp/hover-visible.png ve tmp/hover-enemy.png.
