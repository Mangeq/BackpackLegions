# Backpack Legions — Güncel Geliştirme Yol Haritası

Son güncelleme: 9 Ekim 2026. İlk inceleme: 1 Ekim 2026. Kaynaklar: GDD V4 (30 Haziran 2026), Skilltree System V1, önceki yol haritası, proje kodu, canlı Unity 2022.3.62f3 oturumu ve sınırlı Play Mode kontrolleri.

## 9 Ekim 2026 — Kule ve element değiştirme (GDD 4.2–4.3 revizyonu)

- [x] Emir'in önerilen kuralları ve mevcut sınıf dağılımını onaylamasıyla, ana silah satışı yerine onay ekranlı dönüşüm eklendi.
- [x] Element değişimi 100 altın; XP/seviye korunur, uyumsuz eşyalar pasif kalır. Yeni mağaza hemen açılır ve dondurma kalkar.
- [x] Kule tipi değişiminde ana eşya dışındaki çanta itemleri çıkarılır; depo/teslimler devredilir. Yeni kule 1. seviye, 0 XP, Normal elementle başlar.
- [x] 51 dönüşüm kontrolü, 26 XP, 26 element ve 28 temel oyun regresyonu geçti. Console temiz; görseller kontrol edildi.
- Ücretler TowerData Inspector'ında ayarlanabilir; başlangıçta katlanan ceza yok.
- Kod ve kurallar: [Kule/element değiştirme](Kule_Element_Degisimi_2026-10-09.md).

## 9 Ekim 2026 — Ana eşya evrimi ve element mağazası

- [x] Onaylanan mevcut sınıf dağılımıyla ana eşya evrimi, kuleye özel element mağazası ve düşman element kimliği bağlandı.
- [x] Eski eşyalar oyuncuda kalır; depo hiçbir buff vermez. Mevcut mağaza teklifleri ve dondurma sonraki yenilemeye kadar korunur.
- [x] Üç kule × dört element ana eşya dönüşümü; seçime uygun mermi/asker saldırıları ve gerçek avantaj/direnç hasarı.
- [x] 26 özellik, 13 canlı UI/saldırı, 26 XP regresyonu ve 28 Aşama 1 kontrolü geçti. Console temiz.
- Sonraki element değişimi/cezası ve özel görsel üretimi bu adımda yapılmadı.
- Açıklama: [Ana eşya evrimi ve element mağazası](Element_Evrimi_2026-10-09.md).

## 8 Ekim 2026 — Kule XP ve ilk element seçimi

- [x] Yeni yönetici eklenmeden EnemyData → Bullet/Unit → Enemy → Tower → mevcut UI akışı kuruldu.
- [x] 5. seviyede ilk element seçimi; seçim beklerken kayıpsız XP birikimi, hazırlık/pause korumaları, kule başına bağımsız seviye.
- [x] XP çubuğu, seçim düğmeleri ve hover bilgileri. Gerçek saldırılarda seçilen element kullanılır.
- [x] 26 davranış ve 9 UI/bağlantı kontrolü ile 28/28 Aşama 1 regresyonu geçti; Console temiz. Büyük denge çalışması yapılmadı.
- Element değiştirme, element mağazası ve düşman element kimliği henüz tamamlanmadı.
- Kod ve sayısal ayarlar: [Kule XP ve element](Kule_XP_Element_2026-10-08.md).

## 6 Ekim 2026 — Satın alma iptalinde slot kilidi

- [x] Takip regresyonu düzeltildi: genel IsPointerOverGameObject dünya PhysicsRaycaster sonuçlarını da engelliyordu. Güncel konumdan yalnız GraphicRaycaster UI sonuçlarını filtreleyen kontrol kullanılıyor. EventSystem açıkken satın alma/iptal tekrar doğrulandı.

- [x] Başka slota geçerken önceki boş slotun collider kilidi kaldırılıyor; UI tıklamaları arkadaki slotu seçmiyor.
- [x] Normal kapatma ve panelin dışarıdan gizlenmesi seçim temizliğine bağlandı. Dolu slotlar kilitli kalıyor.
- [x] A'yı iptal → B'den satın al → A'ya dönüp satın al dahil 9 akış ve 1 UI engelleme kontrolü geçti.
- Açıklama: [Slot iptal düzeltmesi](C:/Users/Emir/BackPackDefance/Docs/Backpack_Legions_Slot_Iptal_2026-10-06.md).

## 6 Ekim 2026 — Eşya döndürme ve yükseltme izni

- [x] Onaylanan mevcut sınıf dağılımıyla, R/scroll yukarı saat yönünde; scroll aşağı ters yönde 90° döndürme. Dükkan ve çanta sürüklemesi ortak akışı kullanır.
- [x] Yön eşya instance'ında saklanır; ItemData değişmez. İkon ve önizleme dönüşe uyar; tutma hücresi imleç altında korunur.
- [x] Geçersiz bırakma, pause, panel kapanması ve savaş başlangıcında eski konum/yön geri gelir. Döndürme yalnız hazırlıkta açık.
- [x] GameManager.CanUpgradeTowers ve Tower.UpgradeTower koruması: yalnız hazırlıkta ve pause kapalıyken yükseltme. Ücret/ekran/XP akışı bu adımda eklenmedi.
- [x] 18 geometri ve 12 canlı UI/işlem kontrolü geçti. R/scroll donanım girdisi yerine ortak Rotate metoduyla sentetik kontroller yapıldı.
- [x] Aşama 1 regresyonu 28/28 geçti; Console hata/uyarı yok, Play kapalı, sahne dirty=false.
- Açıklama: [Rotasyon](C:/Users/Emir/BackPackDefance/Docs/Backpack_Legions_Rotasyon_2026-10-06.md). Denge testi Emir'in kararıyla sonraya bırakıldı.

## 6 Ekim 2026 — Kule collider ayarları

- [x] Devasa menzil küreleri düzeltildi: okçu 5, büyücü 4 dünya birimi yarıçap. Merkez kaymaları kaldırıldı; başlangıç ölçeği hesaba katılıyor.
- [x] Menzil trigger'ları Ignore Raycast katmanındaki RangeTrigger alt nesnesine taşındı; kökte kinematic Rigidbody ile mevcut Tower olaylarına bağlandı. Gövde kutuları korundu. Birlik kulesinin kullanılmayan küresi kaldırıldı.
- [x] Okçu prefab'ının kaydını engelleyen bir Missing Script kaydı temizlendi. İki kulede 12 fizik/raycast/boyut kontrolü geçti.
- [ ] Yeni menzillerle beş dalgalık bölümün tam denge koşusu yapılacak.
- Açıklama: [Collider düzeltmesi](C:/Users/Emir/BackPackDefance/Docs/Backpack_Legions_Collider_2026-10-06.md). Yedek: `Backups/2026-10-06-colliders`.

## 5 Ekim 2026 — Okçu ve büyücü hedeflemesi

- [x] Emir'in kararı: birlik kuleleri hariç, iki bağımsız seçim: Kara / Hava / İkisi de ve menzil collider'ına İlk giren / Son giren. Mesafe veya yol ilerlemesi kullanılmaz.
- [x] Seçimler her kulede ayrı tutulur; başlangıç İkisi de + İlk giren. Hazırlıkta ve savaşta değiştirilebilir; pause/sonuçta değiştirilemez. Uygun tür yoksa kule bekler.
- [x] Menzilden çıkıp geri giren düşman sona eklenir; aynı düşmanın birden fazla collider'ı tek sıra kaydı oluşturur. Ölü hedefler atlanır. Seçim değişikliği sonraki atışa uygulanır; mevcut merminin hedefi değişmez.
- [x] Mevcut EnemyData, Tower ve UİManager görev dağılımı kullanıldı; BackpackUIManager'da panel dışına tıklama kontrolü yeni düğmeleri kapsayacak şekilde düzeltildi.
- [x] 32 benzersiz hedefleme/atış/UI kontrolü ve 28 Aşama 1 regresyon kontrolü geçti. İlk raycast denemesi köşe/pivot noktasını kullandığı için başarısızdı; düğme merkeziyle düzeltilen kontrol geçti. Console hata/uyarı döndürmedi; sahne kaydedildi, Play kapalı ve dirty=false.
- [ ] Gerçek uçan düşman hareketi ve bölüm içeriği ayrı iş. Mevcut bölüm düşmanları Kara; Hava seçilirse bu bölümde kule ateş etmez.
- Kod açıklaması ve test sınırları: [Hedefleme](C:/Users/Emir/BackPackDefance/Docs/Backpack_Legions_Hedefleme_2026-10-05.md). Yedek: `Backups/2026-10-05-targeting`.

## 4 Ekim 2026 — Aşama 1 kapanış denetimi

- [x] Aşama 1’in dört teknik maddesi mevcut uygulama ve yeni doğrulamalarla tamamlandı olarak işaretlendi. Yeni oyun mimarisi veya oynanış kodu eklenmedi.
- [x] `Tools/Validation/run_stage1.py`: 28/28 kontrol geçti; Console temiz, Play kapalı, sahne dirty=false ve test kuleleri yok.
- [x] Eksik script 0. PlaneTest kullanıcı kararıyla korunuyor; otomatik kontroller aktif Base etiketli kaleyi kullanıyor.
- [x] Kaynak yedeği oluşturuldu ve 2.997 dosyanın SHA-256 bütünlüğü doğrulandı: `Backups/stage1-baseline-20261004-210406.zip`. Assets/.meta, Packages, ProjectSettings, Docs, Tools dahil. Ayrı klasörde Unity import/build geri dönüş provası yapılmadı.
- [x] Eski fiyatlar, 29 eşya/dört dalga bilgisi, dalga arası hazırlık/kalp hatası notları ve “Git yok” bilgisi güncellendi. Tarihli eski adım raporları tarihsel kayıt olarak korundu.
- Aşama 2’de kalanlar: hedefleme, kule yükseltme izinleri; satış/döndürme eklendiğinde faz kilidine bağlama. Bunlar Aşama 1 kapanışında yapılmış sayılmadı.

## 4 Ekim 2026 — Oyun hızı ve kabul edilen ilk denge

- [x] Emir örnek bölümü oynadı ve mevcut aşama için dengeyi yeterli buldu; sayısal denge şimdilik korundu.
- [x] GameManager/UİManager görev dağılımı onaylandı. Savaşta 1× / 2× / 3× düğmeleri ve seçili hız vurgusu eklendi.
- [x] Seçim pause sonrasında ve sonraki dalgada korunur; hazırlık 60 gerçek saniyedir. Hazırlık/pause/sonuçta hız değişimi kilitli; yeni oyunda 1×.
- [x] 13 canlı kontrol geçti; UI görüntüsü incelendi, Console temiz. Sahne kaydedildi ve Play kapatıldı.
- Kod açıklaması: [Oyun hızı](C:/Users/Emir/BackPackDefance/Docs/Backpack_Legions_Oyun_Hizi_2026-10-04.md). Yedek: `Backups/2026-10-04-speed-ui`.

## 4 Ekim 2026 — Beş dalgalık örnek bölüm ve kule alım kilidi

- [x] Emir son kararı değiştirdi: kule satın alma da yalnız hazırlıkta; savaş/pause/sonuçta kilitli. Açık satın alma paneli üzerinden de işlem yapılamaz.
- [x] GameScane beş dalga: 6 keşifçi, 8 akıncı, 7 muhafız, 8 elit, son dalgada 1 savaş lordu boss. İlk ve ara hazırlıklar 60 saniye.
- [x] Bölüme özel beş EnemyData ve beş prefab varyantı oluşturuldu. Eski Goblin/Boss verileri korundu; mevcut Enemy ve WaveRound yapısı kullanıldı.
- [x] Son dengeyle 5× zamanlı canlı koşu: Victory, aktif ana kale 64/100 can. Başlangıçta üç kule, kazanılan altınla hazırlıklarda üç ek okçu; eşya alınmadı. Console hata/uyarı döndürmedi. Tek koşu, kesin kapalı test dengesi değildir.
- Sayılar, ekonominin gerekçesi ve deneme sonuçları: [Örnek bölüm 1](C:/Users/Emir/BackPackDefance/Docs/Backpack_Legions_Ornek_Bolum_1.md). Yedek: `Backups/2026-10-04-example-level`.

## 4 Ekim 2026 — Dalga arası 60 saniye hazırlık ve seçili kule dondurma

- [x] Emir'in kararı: ilk ve her sonraki hazırlık **60 saniye**; erken Başlat açık. İlk kararda savaşta kule satın alma serbestti; sonraki istekte yalnız hazırlıkla sınırlandı.
- [x] WaveManager tek dalga üretir ve son düşmanı bekler; sonra GameManager hazırlığa döner. Son dalga temizlenince Victory. Eski 10 saniyelik dalga aralığı kaldırıldı.
- [x] Savaş/pause sırasında eşya satın alma, taşıma ve ücretli yenileme kilitli. Açık sürükleme faz geçişinde kayıpsız iptal edilir. Çanta ve hover bilgileri incelenebilir.
- [x] Dondurma **yalnız seçili kulenin dört slotunu** korur. Dondurulmamış kuleler dalga sonrası ücretsiz yenilenir; dondurulmuş kulede alınan slot boş kalır. Dondurmayı kaldırana kadar teklifler korunur.
- [x] Canlı kısa dalga, satın alma, kilit, sürükleme iptali ve dondurma kontrolleri geçti; arayüz görüntüsü incelendi. Satış/döndürme henüz uygulanmadığı için tamamlandı sayılmadı.
- Kodun ayrıntılı akışı: [Dalga döngüsü açıklaması](C:/Users/Emir/BackPackDefance/Docs/Backpack_Legions_Dalga_Dongusu_2026-10-04.md). Yedek: `Backups/2026-10-04-wave-loop`.

## 4 Ekim 2026 — Hazırlık sayacı ve satın alma geri bildirimi

- [x] Düşman giriş noktası ve NavMesh yolunun ilk yönünü gösteren bar/ok. Ekran dışındaki giriş ekran kenarında gösterilir; oyun sonunda gizlenir.
- [x] Kule satın almada seçili kılıç/yay/asa için kalıcı altın çerçeve. İptal ve başarılı satın alma seçimi temizler.
- [x] Para yetmezse kırmızı yetersiz para mesajı ve eksik altın tutarı; başarısız işlem altın/slot durumunu değiştirmez.
- [x] İlk uygulamada 30 saniyeydi; aynı gün Emir'in isteğiyle ilk ve dalga arası hazırlık **60 saniyeye** çıkarıldı. Erken Başlat, pause ve restart korunuyor.
- [x] Canlı Play Mode'da 16 koşul geçti; sayaç sınırları kontrollü süreyle test edildi, UI görüntüsü incelendi. Console hata/uyarı döndürmedi; Play kapalı ve sahne kaydedildi.
- Kod akışı ve test sınırları: [Hazırlık UI açıklaması](C:/Users/Emir/BackPackDefance/Docs/Backpack_Legions_Hazirlik_UI_2026-10-04.md). Yedek: `Backups/2026-10-04-preparation-ui`.

## 4 Ekim 2026 — Hover bilgi panelleri ve can barları

- [x] Emir üç sınıflı görev dağılımını onayladı: `InfoPanelUI` içerik/görünüm; `HoverInfoController` fare hedefi; `HealthBarUI` asker/düşman can göstergesi.
- [x] Dükkanda ve çantada eşya; sahnede kule, birlik ve düşman üzerine gelince ortak panel açılır. Güncel statlar/can okunur, 0,15 sn hover gecikmesi vardır. Sol tuş/sürüklemede, hedef terk edilince ve oyun sonunda kapanır. Pause'da menü dışındaki hedefler incelenebilir.
- [x] UnitPrefab, Goblin ve BossEnemy prefablarında mevcut/maksimum can barı. Asker yeşil, düşman kırmızı; ölümde temizlenir, yeniden doğuşta oluşturulur.
- [x] 30 eşya içeriği; üç kule ve iki aktör türünün hedeflenmesi; shop/backpack raycast; hasar/can bonusu; dört ekran köşesi; pause/sürükleme/ölüm/yeniden üretim/oyun sonu kontrolleri yapıldı. Sentetik ekran/girdi testleri ve 1920×1080 Game View görüntüleri kullanıldı. Console hata/uyarı döndürmedi.
- Kod akışı, Inspector bağlantıları ve test sınırları: [Info UI açıklaması](C:/Users/Emir/BackPackDefance/Docs/Backpack_Legions_Info_UI_2026-10-04.md). Yedek: `Backups/2026-10-04-info-ui`.

## 3 Ekim 2026 — Bağımsız düşman saldırısı ve Denge V1

- [x] Emir'in kararı: birliklerin amacı düşmanı oyalamak ve canını azaltmak; düşman kendi normal saldırı ritmini kullanacak. Hasar, fiyat ve eşya bonuslarının ilk dengesi asistana bırakıldı; mimari kararları birlikte alma anlaşması sürüyor.
- [x] `EnemyData.attackInterval` ve düşmanın kendi sayacı; birlik sadece angajman başlangıcında düşmana hedef veriyor. Asker hızının düşman vuruşlarını hızlandırmaması ve pause kontrol edildi.
- [x] 37 veri asset'i ve 2 prefab için V1: okçu 14/1 sn/250 altın, büyücü 30/1,8 sn/325, birlik 4/1,2 sn/225; asker 60 can, yeniden üretim 6 sn. Fast 70 can/12 hasar/1,2 sn/20 ödül; Boss 350 can/25 hasar/1,8 sn/100 ödül.
- [x] Kule satın alma fiyatı `TowerData.cost`tan okunuyor. Eşya fiyat/bonusları düzenlendi, etkisiz parşömene bonus verildi, can iksiri birlik canına bağlandı. Normal elementin çelişen direnç/zayıflığı kaldırıldı. Kritik toplamı %100'de sınırlanıyor.
- [x] Satın alma ve stat aktarımı doğrulandı; 16 eşya/dükkan koşulu geçti. Asker 0,1 ve 3 sn saldırı aralıklarında düşman yaklaşık 1,2 sn'de bir vurdu; pause'da can değişmedi.
- [x] Üç kule, ek eşya almadan, mevcut dört dalga 10× zaman ölçeğinde çalıştırıldı: Victory, kale 64 can, düşman 0, asker 3. Bu tek yerleşim/rasgelelik koşusudur; dengeyi kesinleştiren tester sonucu değildir.
- Ayrıntılı sayılar, kod akışı, test sınırları ve kapalı test ölçümleri: [Denge V1](C:/Users/Emir/BackPackDefance/Docs/Backpack_Legions_Denge_V1.md). Yedek: `Backups/2026-10-03-balance`.

## 3 Ekim 2026 — İlk sürüm eşya havuzu

- [x] Emir eşya ikonları, bonusları, fiyatları ve boyutlarının yeniden tasarımını asistana bıraktı. 30 satın alınabilir eşya oluşturuldu: okçu/büyücü/birlik için onar; sınıf başına 4 yaygın, 3 nadir, 2 epik, 1 efsanevi.
- [x] Aktif dükkan havuzu yeni asset'lere bağlandı. Eski 29 örnek eşya GUID korunarak `Assets/Archive/PrototypeItems` altına taşındı. Üç başlangıç silahının referansları korundu; ad/ikon/kimlikleri düzenlendi. PlaneTest alanı korundu.
- [x] Yeni eşyalarda benzersiz kimlikler, 1×1 ile 2×3 arasında dolu dikdörtgen şekiller ve şekil içindeki sürükleme merkezleri. Mevcut stat sistemi kullanıldı; özel yetenek veya yeni sınıf eklenmedi.
- [x] 30 veri/yerleşim kontrolü ve 30 canlı satın alma/stat aktarımı senaryosu geçti. Satın almalar mevcut drag metotlarına sentetik pointer olaylarıyla yapıldı; fiziksel fare testi değildir.
- [x] Dükkan seviye 1 oranları %75/%20/%5/%0; üst seviyelerde daha yüksek nadirlik. 10.000 örnek 7501/2003/496/0 verdi. Seviye aralığı dışındaki indeks ve boş havuz koruması eklendi.
- [x] Dükkan ve çanta ikonlarında en-boy oranı korundu. İkonlar mevcut Free Fantasy Items paketinden; Joen Bille atfı `Docs/ART_CREDITS.md` dosyasında.
- [x] Eşya adı/bonus açıklaması tooltip'i 4 Ekim'de eklendi.
- [ ] Efsanevi havuz hazır; oyuncuya açık upgrade akışı olmadığı için normal seviye 1 koşusunda çıkmaz. Kapalı testte yığılmış can bonusları ve ekonomi ölçülecek.
- Yeni eşya tablosu önceki Denge V1 eşya değerlerinin yerini alır: [Görsel katalog](C:/Users/Emir/BackPackDefance/Docs/Backpack_Legions_Item_Katalogu_V1.html), [açıklamalar](C:/Users/Emir/BackPackDefance/Docs/Backpack_Legions_Item_Katalogu_V1.md). Yedek: `Backups/2026-10-03-items-v1`.

## 2 Ekim 2026 — Birlikte kararlaştırılan ilk kod adımı

- [x] Emir ile karar: temel değerler `TowerData`da; ortak item stat hesabı `Tower`da; saldırı/üretim uygulaması mevcut alt sınıflarda.
- [x] `Tower.Start()` başlangıcında `InitializeBaseStats()` eklendi. Damage, kritik şans, saldırı aralığı ve başlangıç seviyesi veriden kule instance'ına kopyalanıyor. Eski seviye 0 değerleri runtime'da en az 1 kabul ediliyor.
- [x] `TowerData.criticalChance` eklendi; varsayılan %10. `attackSpeed` alanının saniye cinsinden saldırı aralığı olduğu Inspector açıklamasına yazıldı.
- [x] Üç kule tipi ve aynı veriyi kullanan ikinci büyücüyle Play Mode kontrolü: 41 sayısal/durum kontrolü geçti. Temel değerler, item ekleme/çıkarma, birliklere aktarım ve upgrade izolasyonu doğrulandı. Büyücü hasarı 25, okçu aralığı 2 saniye. Paylaşılan asset değişmedi.
- [x] Derleme sonrası ve kontrol sonunda Console hata/uyarı döndürmedi; Play Mode kapatıldı.
- Bu adım iki oyun scriptini değiştirir: `Tower.cs`, `TowerData.cs`. Atış döngüsünü başka bir sınıfa taşıma veya yeni faz mimarisi bu adımda yapılmadı. Alttaki 1 Ekim canlı inceleme notları tarihsel kayıt olarak korunuyor.

## 2 Ekim 2026 — Başlangıç, pause ve sonuç geçişleri

- [x] Emir ile karar: oyun durumu mevcut `GameManager` içinde kalacak; ayrı faz yöneticisi eklenmeyecek.
- [x] `GameManager.GameState`: Preparation, Combat, Victory, Defeat. Pause ayrı `IsPaused` bilgisi; önceki zaman ölçeği korunuyor.
- [x] `WaveManager.Start()` yalnız dalga göstergesini hazırlıyor. Üretim `GameManager.StartGame()` → `TryStartWaves()` üzerinden bir kez başlıyor.
- [x] Hazırlık pause/continue işlemleri savaşı başlatmıyor; pause açıkken Start çalışmıyor. Savaş pause/continue önceki hıza dönüyor.
- [x] Sonuç kesinleşince pause paneli kapanıyor, dalga coroutine'leri duruyor. Sonradan gelen başka sonuç veya Start/Continue/TogglePause ilk sonucu değiştirmiyor.
- [x] Canlı kontrolde 20 koşul geçti: başlangıç, tekrarlı Başlat, hazırlık/savaş pause, 2× hıza dönüş, yenilgi/kazanma kilidi ve restart. Escape'in çağırdığı `TogglePause` metodu doğrudan çağrıldı; fiziksel tuş otomasyonu yapılmadı. Dalga tamamlanma bağlantısı yalnız test oturumunda sıfır düşmanlı tek dalga ile sınandı; bu bir tam bölüm playtesti değildir.
- [x] Console hata/uyarı döndürmedi. Geçici test dalgası kaydedilmedi; Play kapatıldı. Oyun scripti değişiklikleri: `GameManager.cs`, `WaveManager.cs`.
- Dalga arası hazırlık, shop/backpack kilidi, Freeze ve ücretsiz reroll sonraki aşamada açık. Oyun hızı UI'ı henüz yok.

## 2 Ekim 2026 — Eksik script temizliği

- [x] `GameScane` canlı sahnesindeki 28 eksik script bileşeni Unity Editor API'siyle kaldırıldı: eski grid üzerindeki 25 düğme, item shop paneli, GameManager nesnesi ve BackpackManager nesnesindeki birer eksik bileşen.
- [x] Çalışan `GameManager` scripti korunmuştur; aynı nesne üzerindeki kayıp script yuvası kaldırılmıştır. GameObject veya C# dosyası silinmedi, mimari değiştirilmedi.
- [x] Önce/sonra karşılaştırması: 388 GameObject korundu; mevcut bileşen kimlikleri ve oyun scriptlerinin nesne referansları aynı; eksik script sayısı 28→0.
- [x] Kaydedilen sahne dosyasındaki fark kontrol edildi: yalnız 28 eksik MonoBehaviour kaydı ve bunların GameObject bileşen listelerindeki bağlantıları değişti; yeni nesne eklenmedi, diğer bileşen kayıtları değişmedi.
- [x] Başlangıç kontrolü: Preparation, pause kapalı, timeScale=0, düşman=0; WaveManager ve ShopManager mevcut. Console hata/uyarı döndürmedi. Tam oynanış testi yapılmadı; Play kapatıldı.
- Yedek: `Backups/2026-10-02-missing-scripts/GameScane.unity`. Yedeğin işlem öncesi sahneyle SHA256 eşitliği doğrulandı. İşlem Unity Undo grubuna da kaydedildi.
- Eski grid ve BackpackManager nesnelerinin kendileri korunuyor; kullanılmayan nesneleri kaldırma ayrı bir iştir. Sıradaki doğruluk maddesi kale kalplerinin hasar sonrası güncellenmesi.

## 2 Ekim 2026 — Kale kalpleri

- [x] `Base` içinde kalp görünürlüğü `currentHealth / maxHealth` oranına bağlandı. Birinci kalp can sıfırdan büyükken, ikinci kalp oran 1/3'ten büyükken, üçüncü kalp 2/3'ten büyükken açık.
- [x] Her hasarda üç kalp de güncelleniyor; doğrudan ölümde ikinci kalbin açık kalması düzeltildi. Fazla hasar sonrası can sıfırda sınırlandırılıyor; ardından mevcut `GameManager.GameOver(false)` akışı çalışıyor.
- [x] Başlangıçta can ve kalpler sıfırlanıyor; UI yöneticisi başlangıçta bir kez bulunuyor. Yeni sınıf veya Inspector bağlantısı eklenmedi.
- [x] Canlı Play Mode'da 25 koşul geçti: sahne başlangıcı, 100→66→33→0, doğrudan 100→0 ve 100→20, fazla hasar, Defeat durumu, 3/100/300/10000 maksimum can ve sıfır maksimum can için bölme koruması. Metotlar doğrudan çağrıldı; düşman çarpışması veya tam bölüm playtesti yapılmadı. Test verileri sahneye kaydedilmedi.
- Sıradaki madde: birlik karşı saldırı ritmi. Davranış Emir ile kararlaştırılacak.

## Çalışma anlaşmamız

### 3 Ekim — Eski alan kararı ve kule düğmeleri

- Emir'in kararı: eski `PlaneTest` alanı olduğu gibi kalacak; devre dışı bırakılmayacak veya silinmeyecek. Etkisiz olduğu ayrıca doğrulanmış sayılmıyor; bu temizlik maddesi şimdilik ertelendi.
- Kule seçimindeki yay ve asa ikonlarının olay bağlantıları doğruydu; düğme ve fiyat nesnelerinin adları tersti. Yayın yanında büyücü fiyatı, asanın yanında okçu fiyatı gösteriliyordu.
- Canlı Editor üzerinden yalnız dört GameObject adı düzeltildi: iki düğme ve iki fiyat etiketi. İkonlar, olay bağlantıları ve C# kodu değişmedi. `GameScane` kaydedildi; yedek `Backups/2026-10-03-tower-ui/GameScane.unity`.
- Play Mode'da gerçek Button.onClick olayları çağrıldı: birlik 225, okçu 250, büyücü 325. Üçünde de fiyat etiketi, seçilen tür, kesilen altın ve oluşan kule eşleşti. Fiziksel fare tıklaması testi yapılmadı. Console hata/uyarı döndürmedi; Play kapatıldı. Düzeltme testten önce kaydedildi; test sonrası Editor sahneyi dirty gösterdiğinden ek değişiklikler otomatik kaydedilmedi.

- Kod mimarisini Emir belirler; seçenekleri ve sonuçlarını birlikte tartışırız. Bu belgede geçen iş sırası öneridir, mimari kararı değildir.
- Bir özelliği uygulamadan önce davranışı, kapsamı, etkilenen mevcut sınıfları ve kabul ölçütlerini birlikte netleştiririz.
- Büyük ve sessiz refactor yapılmaz. Değişiklikler küçük, anlaşılabilir adımlara ayrılır.
- Yazılan her kod için amaç, veri akışı, metotların görevi, neden o yaklaşımın seçildiği, Inspector bağlantıları ve test sonucu açıklanır.
- Emir'in kod hâkimiyeti teslim ölçütüdür. Çalışan fakat açıklanamayan bir değişiklik tamamlanmış sayılmaz.
- Belgelerdeki teknik öneriler otomatik uygulama talimatı değildir. Örneğin eski yol haritasındaki merkezi faz yöneticisi veya kod yorumundaki `TowerManager` fikri henüz onaylanmış karar sayılmaz.

## Durumları nasıl okuyacağız?

- **Uygulandı:** Kod yolu var; hangi ölçüde test edildiği ayrıca belirtilir.
- **Canlı doğrulandı:** Bu incelemede belirtilen dar senaryo Unity'de çalıştırıldı.
- **Kısmi:** Altyapı var, tasarımın tamamı yok.
- **Açık:** İncelenen oyun kodunda uygulama bulunmadı.
- **Karar bekliyor:** Tasarım veya mimari Emir ile görüşülecek.

Önceki belgede uçtan uca MVP playtestinin tamamlandığı kayıtlıydı. Bu tarihsel kayıt korunuyor; önceki testin bütün adımları bu incelemede yeniden yapılmadı. Bugünkü doğru tanım: **temel döngüsü çalışan, Barracks entegrasyonu ilerlemiş prototip; GDD V4 kapsamı ve hata kontrolleri henüz tamamlanmadı.** Tamamlanma yüzdesi verilmedi; modüllerin maliyetleri eşit değil.

## Önceki yol haritasında düzeltilen bilgiler

| Eski kayıt | Güncel karşılığı — 4 Ekim 2026 |
|---|---|
| `UnitTower`, `Tower` sınıfından kopuk; `Unit.damage` set edilmiyor | Güncel değil. `UnitTower : Tower`; `base.Start()` kullanılıyor; hasar/kritik/saldırı aralığı/can bonusu askerlere aktarılıyor. |
| Barracks backpack/shop bağlantısı yapılacak | Temel bağlantı uygulandı. XP ve ilk element seçimi uygulandı (8 Ekim); skilltree ve elemente özel mağaza hâlâ açık. |
| Kritik şans düzeltmesi kullanıcı tarafından uygulanacak | Uygulanmış: `Bullet` ve `Unit` 0–1 aralığında rastgele değer kullanıyor. |
| Kazanma/kaybetme ayrımı belirsiz | Aynı panel, farklı metin. `GameOver(bool)` bunu ayırıyor. |
| Dalga spawn sistemi tamam | Her dalganın son düşmanı bekleniyor; ilk ve ara hazırlık 60 saniye. Beş dalgalık örnek bölüm ve son dalgada boss var. |
| Dükkan slot sıfırlaması çözüldü | Korundu; ayrıca dükkan artık kule başına saklanıyor ve satın alınan slot kule listesinden de siliniyor. |
| Kale kalpleri 100 can için bütünüyle doğru, konu kapandı | 2 Ekim’de düzeltildi: her hasarda üç kalp can oranıyla birlikte güncelleniyor; tek vuruşla ölüm doğrulandı. |
| P0 tamamen bitti; bilinen engel kalmadı | Aşama 1 teknik kontrolleri ve yedek/geri dönüş kaydı aşağıda günceldir. Bu, tüm GDD veya build testinin tamamlandığı anlamına gelmez. |
| Skilltree yol haritasında yok | 18 yetenek ve bağımlılıkları eklendi. |

## Mevcut temel: yapılanlar

- [x] Her kule için bağımsız 5×5 backpack; `Backpack`, `PlacedItem`, şekil/hücre hesapları.
- [x] Mağazadan sürükleyerek satın alma ve çanta içinde taşıma kodu; geçersiz taşımanın eski konuma dönmesi; yerleşim önizlemesi. Sentetik pointer olaylarıyla satın alma/taşıma ve geçersiz konumdan geri dönüş test edildi; fiziksel fare otomasyonu değildir.
- [x] Kule oluşunca varsayılan silahın merkeze yerleştirilmesi. Üç kule tipinde canlı olarak birer başlangıç eşyası görüldü.
- [x] Ortak gold; eşya satın alma ve ücretli reroll; düşman öldürme ödülü.
- [x] Okçu, büyücü ve birlik kulesinin mevcut satın alma metotlarıyla kurulması. Güncel maliyetler: birlik 225, okçu 250, büyücü 325; toplam 800 altın, başlangıçtan kalan 200.
- [x] Kule tipi ve rarity filtresiyle dükkan üretimi. Aktif sahne havuzunda 30 eşya var; üç başlangıç silahı ayrı, eski 29 örnek arşivde.
- [x] Her kulede dört slotluk kalıcı `myShopItems`; açılışta yeniden zar atılmıyor; satın alınan slot `null` yapılıyor; ücretli reroll yeniliyor.
- [x] Eşya hasar/kritik/hız bonuslarının `CalculateStats()` ile savaşa aktarılması; cooldown için 0.1 saniye alt sınır.
- [x] Okçu/büyücü mermilerinin `Initialize()` ile stat alması; kritik şans ölçeği düzeltmesi.
- [x] Düşmanda weakness ×1.5, resistance ×0.5 hesabı. Bu, element evrim sisteminin tamamlandığı anlamına gelmez.
- [x] `UnitTower : Tower`; backpack ve dükkan ortak tabanı; askere stat aktarımı; yaşayan askerlere yeniden hesaplama.
- [x] Birlik kulelerinin üç asker üretmesi, yakındaki boş düşmanla angajman, düşmanı durdurma, asker ölünce serbest bırakma ve boş slotu yeniden doldurma kodu.
- [x] Birlik için on satın alınabilir eşya ve `healthBonus` alanı. Can oranını koruyarak stat aktarımı canlı doğrulandı.
- [x] Beş dalgalık örnek bölüm; son dalga boss. Son kayıtlı hızlandırılmış koşuda Victory, aktif kale 64/100 can. Emir normal oynayışta dengeyi mevcut aşama için yeterli buldu.
- [x] Pause/continue/restart/main menu kod yolları; kazanma/kaybetme metin ayrımı. Kazanma, kaybetme ve restart tekrar çalıştırılabilir Aşama 1 kontrolüne dahil; main menu/exit ayrı tam oynanış testi kapsamında.
- [x] Dükkanın satıldı durumunu sıfırlama ve panellerin dışına tıklayınca kapatma düzeltmeleri kodda mevcut.
- [x] Kule fiyatlarını prefabın `TowerData.cost` değerinden okuma; satın alınan slotu dolu sayma ve iptal edilen slotu tekrar açma kodu.

## Önce ele alınacak doğruluk ve sahne işleri

Bu tablo önceki bulguların güncel durumudur. Emir 4 Ekim’de Aşama 1’de kalanları tamamlamayı ve bitenleri işaretlemeyi istedi; yeni mimari kararlar yine birlikte alınır.

| İş | Kanıt / etki | Bitti sayılma ölçütü |
|---|---|---|
| Temel stat kaynağı — 2 Ekim'de çözüldü | Başlangıç değerleri `TowerData`dan kopyalanıyor; güncel büyücü 30 hasar/1,8 sn, okçu 14 hasar/1 sn, birlik 4 hasar/1,2 sn. | Üç tipte item hesabı ve aynı asset'i kullanan iki büyücüde upgrade izolasyonu canlı doğrulandı. |
| Hazırlıkta erken spawn — 2 Ekim'de çözüldü, 4 Ekim'de döngü tamamlandı | Üretim `StartGame` üzerinden; Başlat düğmesi veya her hazırlıkta 60 saniyenin dolması tetikler. Pause süreyi durdurur. | Son düşman sonrası hazırlık; manuel/otomatik başlangıç ve tekrarlı çağrı koruması kontrol edildi. |
| Oyun sonu ve pause sınırları — 2 Ekim'de çözüldü | Durum GameManager'da tutuluyor; pause önceki hıza dönüyor; terminal sonuç korunuyor. | Hazırlık/savaş pause, kazanma/kaybetme kilidi ve restart canlı kontrolleri geçti. |
| Eksik script bileşenleri — 2 Ekim'de temizlendi | 28 eksik bileşen kaldırıldı; nesneler ve çalışan bileşenler korundu. | Canlı tarama 0 eksik script; 388 nesne ve mevcut oyun scripti referansları aynı; başlangıç kontrolü geçti. |
| Kale kalpleri — tamamlandı (2 Ekim) | `Base.UpdateHealthUI` üç kalbi can oranıyla güncelliyor; tek vuruşta ölüm hatası düzeltildi. | 25 canlı koşul geçti; 100→0, 100→20 ve farklı maxHealth değerleri doğrulandı. |
| Birlik karşı saldırı ritmi — tamamlandı (3 Ekim) | Düşman kendi attackInterval sayacıyla vuruyor; birlik oyalama ve yardımcı hasar rolünde. | İki farklı asker saldırı hızıyla düşmanın aynı ritimde vurduğu canlı doğrulandı. |
| Deneme içeriğini ayırmak — ertelendi | Emir'in 3 Ekim kararı: `PlaneTest` olduğu gibi kalacak. | Eski alanın hiçbir etkisinin olmadığı iddia edilmiyor; şu an değişiklik yapılmayacak. |
| UI isim/olay eşleşmesi — tamamlandı | İkon/olay doğru, fiyat etiketlerinin adları tersti; iki düğme ve iki fiyat nesnesi yeniden adlandırıldı. | Üç türde görünen fiyat, seçilen tür, kesilen altın ve oluşan kule canlı doğrulandı. |
| Veri temizliği — aktif havuz tamamlandı | 30 yeni eşya ve üç başlangıç silahı benzersiz kimlik/uygun merkez kullanıyor. Eski veriler arşivde. Normal element çelişkisi Denge V1'de giderildi. | Yeni havuzun tüm şekilleri ve başlangıç silahı yanında yerleşimi doğrulandı. |
| Dükkan/seviye sınırı — tamamlandı | 1 altı ilk, 5 üstü son olasılık satırını kullanır. Boş havuz null döndürür. Üç uygulanmış kule için dört nadirlik de mevcut. | -1/99 seviyeleri ve içeriksiz CannonTower havuzu güvenli çalıştı. |

## Önerilen ilerleme sırası ve kabul ölçütleri

### Aşama 1 — Mevcut prototipi güvenilir hale getirmek — teknik maddeler tamamlandı

- [x] Bulgular Emir ile önceliklendirildi; stat kaynağı TowerData, oyun durumu GameManager olarak kararlaştırıldı ve uygulandı. Başlangıç/sonuç/pause düzeltmeleri önceki canlı kontrollerde doğrulandı.
- [x] Üç kule + eşya alımı/taşıma + dükkan aç/kapa + reroll + kazanma/kaybetme + restart, `Tools/Validation/run_stage1.py` ile tekrar çalıştırılabilir hale getirildi. 4 Ekim son koşusu: **28/28 geçti**, Console hata/uyarı yok. [Makine okunabilir sonuç](C:/Users/Emir/BackPackDefance/Docs/Validation/stage1-latest.json).
- [x] Eksik script temizliği yapıldı; son sahne taraması **0 eksik script / 432 nesne**. PlaneTest eski alan olarak ayrıldı ve Emir'in isteğiyle korundu. Buradaki etiketsiz Base ile aktif Base etiketli kale ayrımı kayıtlı; testler aktif kaleyi kullanıyor. Bu madde eski alanın silindiği veya etkisizliğinin kanıtlandığı anlamına gelmez.
- [x] Geri dönüş yöntemi belirlendi: önceki adım yedekleri korunur; `snapshot_project.py` kaynak klasörlerini ve .meta dosyalarını yeni tarih/saatli ZIP'e alıp SHA-256 ile doğrular. Geri dönüş yeni, boş proje klasörüne çıkararak yapılır. Git deposu artık mevcut (son yerel commit `8ce67c4`); eski “Git yok” kaydı güncel değildir. Mevcut commit edilmemiş değişiklikler korunur; bu tur commit/push yapılmadı. [Çalıştırma ve geri dönüş adımları](C:/Users/Emir/BackPackDefance/Tools/Validation/README.md).

Teknik çıkış ölçütü karşılandı: temel akış sayıları/geçişleri kayıtlı ve tekrar çalıştırılabilir; bilinen eski test alanı kararı açıkça kabul edilmiş. Kod açıklamaları ilgili adım belgelerinde mevcut. Emir'in kod hâkimiyeti otomatik testle ölçülmüş sayılmaz; anlaşılmayan noktaları birlikte açıklayacağız. Tam build, bütün GDD ve tüm olası oyun senaryoları bu işaretle tamamlanmış sayılmaz.

### Aşama 2 — GDD hazırlık / savaş döngüsü

- [x] Dalga başlatma, son düşmanda hazırlığa dönüş ve son dalgada zafer; her hazırlık 60 saniye.
- [x] Savaşta mevcut eşya satın alma/taşıma/reroll işlemleri kilitli; açık drag geri alınır.
- [x] Döndürme aynı faz kilidine bağlandı; yalnız hazırlıkta (6 Ekim).
- [x] Satış aynı faz kilidine bağlandı: yalnız hazırlıkta, pause dışında. Savaş başlangıcı ve pause sırasında sürükleme geri alınır (8 Ekim).
- [x] Hedefleme kontrolünü envanter kilidinden ayrı tutmak: okçu/büyücü tür ve giriş sırası seçimi savaşta açık; pause/sonuçta kilitli (5 Ekim).
- [x] Son düşman sonrası hazırlık ve dondurulmamış dükkanların ücretsiz yenilenmesi.
- [x] Freeze kapsamı kararlaştırıldı ve uygulandı: yalnız seçili kulenin dört slotu; global dondurma yok.
- [x] 1×/2×/3× hız, seçili hız vurgusu; pause/continue ve sonraki dalgada hızın korunması.
- [x] Son karar: yeni kule kurma yalnız hazırlıkta; savaşta ve pause sırasında kilitli.
- [x] Kule yükseltme yalnız hazırlıkta ve pause kapalıyken; mevcut metoda koruma eklendi (6 Ekim).

Hazırlık → Savaş → Hazırlık döngüsü ve gold/grid/shop kilitleri doğrulandı. Mevcut GameManager/WaveManager görev dağılımı korundu. Hız, hedefleme, rotasyon ve satış faz kilitleri tamamlandı.

### Aşama 3 — Envanter ve ekonomi derinliği

- [x] Rotasyon: R ve scroll, 90° adımlar; centerOffset tutma hücresi imleç altında, taşmada eski konum/yöne dönüş (6 Ekim).
- [x] Özel komşu kare sinerjisi: herhangi bir eşya / eşya türü / belirli eşya filtreleri; döndürmeyle birlikte kare dönüşü; ilgili eşyanın katkısına bonus ve sarı/yeşil gösterim (8 Ekim).
- [x] Wave sonunda evrim: iki kenar komşusu malzeme → ürün; bir başarılı dalga, son dalga dahil. Ana item tarif dışında. Ürün çanta → kendi deposu → teslim listesine gider; evrim beklemez (8 Ekim).
- [x] Teslim listesinde kuleye özel ürünler ve ücretsiz sürükleyerek alma; iptalde kayıp/çoğalma engeli. Üç örnek tarif ve üç filtre örneği eklendi. Yeni sistem 28/28, regresyon 28/28 kontrol geçti. [Ayrıntılı kod ve Inspector açıklaması](Sinerji_Evrim_2026-10-08.md).
- [ ] Örnek tarifleri oyun testinde değerlendirmek; içerik havuzuna ek sinerji/tarifleri birlikte belirlemek. Sayısal denge kapalı test geri bildirimleriyle geliştirilecek.
- [x] Satış kuralı onaylandı (8 Ekim): ItemData.cost değerinin %50'si, aşağı yuvarlama; yalnız hazırlıkta, pause dışında. Başlangıç silahı satılamaz.
- [x] Satış birleşik panelin altına bağlandı; fiyat önizlemesi ve 11 canlı kontrol geçti (8 Ekim). Ayrıntı: [Satış kod akışı](Item_Satisi_2026-10-08.md).
- [x] Çantadan satış stat bonusunu kaldırır; depodan satış statları değiştirmez. Başlangıç silahı iki alanda da korunur. Kule değişiminde eşya asıl sahibine döner; tekrarlanan bırakma ikinci ödeme yapmaz.

Çıkış ölçütü: aynı eşya havuzunda farklı yerleşimlerin farkı ölçülür; iptal edilen işlem item/gold kaybettirmez veya çoğaltmaz.

### Aşama 4 — Hedefleme ve element/XP

- [x] Kara/hava/ikisi ve ilk/son menzile giriş hedeflemesi; EnemyData hareket türü sınıflandırması (5 Ekim). Emir'in kararıyla yakın/uzak mesafe yerine collider'a giriş sırası kullanılır.
- [x] Yolu takip eden hava düşmanı: Gök Perisi, 1 birim görsel yükseklik, birlikleri pas geçme, havadaki mermi/can barı/mouse hedefi. Emir'in seçimiyle 3. dalgaya bağlandı: 7 düşman, 1,8 sn aralık korunur (8 Ekim). [Kod açıklaması](Ucan_Dusman_2026-10-08.md).
- [x] Düşman element kimliği ve onaylanan Su → Ateş → Hava → Toprak → Su döngüsü. Avantaj ×1,5, direnç ×0,5; diğer eşleşmeler ×1. Örnek bölümde elementler bağlandı (9 Ekim).
- [x] XP gerçek hasar katkısına göre ölümde kulelere dağıtılır; asker katkısı sahibine yazılır. Kale teması ödülsüzdür. Seviye 5'te element seçimi beklerken XP korunur; seçim yalnız hazırlıkta yapılır (8 Ekim).
- [x] Element listesi kararı: Emir mevcut Normal/Fire/Water/Earth/Air listesini korudu. Oyuncuya Ateş/Su/Toprak/Hava sunulur; mermi ve askerlere aktarılır. Belgelerdeki alternatif isimler bu listeyi değiştirmez. Düşman element kimliği 9 Ekim'de bağlandı.
- [x] İlk element seçiminde ana eşya evrimi ve mağaza havuzu geçişi. Eski teklifler sonraki yenilemeye kadar korunur; dondurma bozulmaz. Eski normal eşyalar oyuncuda kalır, çantadayken çalışır; depodakiler hiçbir bonus vermez. Ana eşya ve evrimleri satılamaz/depolanamaz (9 Ekim).
- [x] 12 ana eşya evrimi, 120 elementli mağaza eşya verisi, mevcut üç tarifin dört element karşılığı. Mevcut şekil/ikon temelli ilk içerik; yeni özel görsel seti değil. [Kod ve kurallar](Element_Evrimi_2026-10-09.md).
- [x] İlk seçimden sonra ücretli element değişimi: hazırlıkta onay ekranı, başlangıçta 100 altın; XP/seviye korunur. Ana eşya dönüşür; yeni mağaza hemen açılır, dondurma kalkar. Uyumsuz eski element eşyası pasif olur; Normal ve türü uygun eşya çalışır. İlk ücretsiz seçim eski teklifleri koruma kuralını sürdürür.
- [x] Kule tipi değişimi: Emir'in 9 Ekim revizyonuyla yalnız çantadaki ana eşya dışındaki itemler çıkarılır; satış zorunlu değildir. Depo/teslimler korunur. Yeni kule seviye 1 / XP 0 / Normal başlar; eski askerler kaldırılır. Başlangıç fiyatı yeni kule fiyatı − eski kule fiyatının %50'si (iade aşağı yuvarlanır, net bedel en az 0).
- [x] İlk ceza kararı: sonraki element değişimleri sabit 100 altın; katlanan ceza yok. TowerData.elementChangeCost ve typeChangeRefundRate Inspector'dan ayarlanır. Emir testten sonra gerekirse cezayı ağırlaştıracak.
- [ ] Havan/topçu isteniyorsa ayrı kule davranışı; enum ve placeholder item bulunması uygulama değildir.

Çıkış ölçütü: bir kule XP kazanır, sınıf seçer, ilgili shop'a geçer; fiyat/stat değişimi başka kuleye sızmaz.

### Aşama 5 — Castle HQ, skilltree ve ölçüm

- [ ] HQ harcamalarının run içi mi kalıcı mı olduğuna karar; satın alma, düğüm bağımlılıkları, maliyetler, seviye sayıları ve reset kuralları.
- [ ] Global buff/debuff kapsamı ve stat hesaplama sırası.
- [ ] Hasar kaynağı, kritik ve yetenek tetiklenme verisini ölçmek; skilltree etkilerini bu veriyle doğrulamak.
- [ ] Aşağıdaki 18 yeteneği küçük gruplar halinde eklemek; basit pasiflerle başlayıp özel tetiklere geçmek.
- [ ] Pause analiz ekranı: kule hasarı/DPS, eşya-yetenek katkısı ve düşman tipine göre etkinlik.

Çıkış ölçütü: satın alınan bir pasifin fiyatı, etkisi ve raporlanan katkısı aynı hesapla uyuşur; kapalı düğüm etki üretmez. Ölçüm altyapısı için sınıf seçimi henüz yapılmadı.

### Aşama 6 — Kalıcı ilerleme ve içerik

- [ ] Bestiary karşılaşma kaydı, Öğren görevi, kill ilerlemesi ve türe özel ödül.
- [ ] Kalıcı sistemler seçilirse kayıt/yükleme; yeniden başlatma ve sürüm değişiminde korunacak veriler.
- [ ] Bölüm geçişi ve dalga ölçekleme formülü; GDD'deki doğrusal zorluk/logaritmik yoğunluk ilişkisini netleştirmek.
- [ ] 20 bölüm ve her beş bölümde tema/harita/düşman ailesi; önce bir bölümü dengeli ve tamamlanabilir örnek haline getirmek.
- [ ] 1×/2×/3× dengeleme, olasılık kontrolleri, performans ve gerçek build testi.

## Skilltree V1 — Uygulama listesi

Hepsi **açık**. Genel kritik, item can bonusu ve spawn kodunun bulunması bu yeteneklerin uygulanmış olduğu anlamına gelmez. Belgedeki değerler tasarım hedefi olarak aktarıldı; belirsiz çarpım ve süre kuralları kararlaştırılacak.

| Dal | Yetenek | Hedef / gereken temel |
|---|---|---|
| Okçu | Okçu Eğitimi I | Temel hasar +%5; global türe göre bonus. |
| Okçu | Hafif Yaylar | Saldırı hızı +%8; cooldown dönüşümü. |
| Okçu | Ağır Kiriş | Menzil +%10, fiziksel zırh delme; zırh modeli eksik. |
| Okçu | Kritik Odak | Kritik şans +%10 ve x2; yüzde puanı/çarpan ve çarpılan hasar kapsamı netleşmeli. |
| Okçu | Element Okları | Ateş/Doğa/Buz düşmanlarına +%15; düşman element kimliği. |
| Okçu | Kusursuz Nişancılık | Aynı hedefte üç başarılı atış sonrası dördüncü kesin kritik + 1 sn stun; isabet sayacı ve durum etkisi. |
| Büyücü | Mana Akışı I | Kitap/element item RNG +%5; item etiketleri ve ağırlık normalizasyonu. |
| Büyücü | Rün Bilgisi | Her büyü eşyası için +%2 hız; eşya sayımı ve faz kapsamı. |
| Büyücü | Katalizör | Element değişim cezası %10→%5; fiyat kuralı. |
| Büyücü | Kaotik Element | Karşı element bonusu ekstra %20, nötr düşmana +%10; hasar sırası ve sınıflandırma. |
| Büyücü | Yıkım Büyüsü | AoE büyü hasarı +%15, kritik büyü şansı +%5; mevcut mermi tek hedefli, AoE ayrı eksik. |
| Büyücü | Kıyamet Sevdalısı | Element değişiminden dalga sonuna +%15 büyü hasarı; hazırlıkta değişim için hangi dalga olduğu netleşmeli. |
| Birlik | Ordu Düzeni I | Asker max HP +%15; mevcut healthBonus aktarımı temel sağlar. |
| Birlik | Demir Dövme | Silah item hasar katkısını %20 fazla aktarma; yalnız silah katkısını ayırma ihtiyacı. |
| Birlik | Kışla İkmali | Üretim %12 hızlanma; saldırı aralığından ayrı spawn süresi. |
| Birlik | Element Zırhı | Belirli elementlerden alınan hasar -%15, verilen +%10; savunma hasar türü bilgisi. |
| Birlik | Çifte Sancak | %15 çift üretim, geçici kapasite aşımı; mevcut sabit slot modelinin davranışı tartışılacak. |
| Birlik | Lejyoner Sinerjisi | Aynı kule askerleri başına %5 zırh/kritik; kendini sayma, toplama/çarpma ve limit kararı. |

## Karar defteri — alınan ve açık kararlar

1. Karar alındı: oyun durumu `GameManager`da, dalga üretimi `WaveManager`da. 60 saniyelik dalga arası hazırlık ve mevcut eşya işlem kilitleri tamamlandı; son kararla savaşta kule satın alma da kilitli.
2. Karar alındı (2 Ekim): temel statlar TowerData’dan gelir; attackSpeed saniye cinsinden saldırı aralığıdır. Ortak eşya hesabı Tower’da; davranış alt sınıflarda.
3. Karar alındı (6 Ekim): sürüklerken R/scroll yukarı saat yönünde, scroll aşağı ters yönde 90°. Yalnız hazırlık; geçersiz bırakmada eski konum/yön korunur.
4. Karar alındı (4 Ekim): dondurma yalnız seçili kulenin dört dükkan slotunu korur; uygulandı.
5. Karar alındı (5 Ekim): yalnız okçu/büyücü için Kara / Hava / İkisi de filtresi ve collider'a İlk giren / Son giren sırası bağımsızdır. Mesafe/yol ilerlemesi değildir; uygun tür yoksa beklenir, diğer türe geçilmez.
6. Karar alındı (9 Ekim): ilk element ücretsiz, sonraki değişimler sabit 100 altın. GDD'deki katlanan ceza bu sürümde uygulanmıyor; test sonrası yeniden değerlendirilecek.
7. Skilltree run içi HQ harcaması mı, kalıcı ilerleme mi? Belge ikisini de seçenek olarak bırakıyor.
8. Aynı hedefte “başarılı atış” isabet mi ateşleme mi; stun tekrarları nasıl davranır?
9. Zırh, büyü/fiziksel hasar, kritik ve element çarpanlarının sırası ne?
10. Karar alındı (3 Ekim): düşman kendi saldırı ritmini kullanır; birlik hız bonusu düşmanı hızlandırmaz. İlk sayısal denge asistana bırakıldı, kapalı test geri bildirimleriyle güncellenecek.
11. XP gerçek hasar katkısıyla dağıtılıyor. İlk üç craft tarifinin dört element karşılığı eklendi (9 Ekim). Ana silahın bütün evrimleri satılamaz/depolanamaz. İlk ve sonraki element seçimleri uygulandı; ücretli değişimde uyumsuz eşyalar tutulur ama çalışmaz.

## 1 Ekim ilk incelemesinin sınırı — tarihsel kayıt

Canlı kontrol: sahne/prefab/data referansları, Console örneklemesi, mevcut metotlarla üç kule satın alma, çalışma zamanı statları, savaşın ilerlemesi ve sonuç paneli; geçici item ekleyip çıkararak birlik hasarı/can oranı kontrolü. Bu son kontrol fareyle shop satın alma testi değildir. Bu tur yeni oyun kodu, prefab veya sahne düzenlemesi yapılmadı.

Test sonunda Play kapalı, `GameScane` açık, sahne `dirty=false`, `runInBackground=false` ve test kuleleri yok. Console'da kontrol anlarında hata/uyarı gelmedi; bu, eksik script bileşenlerini veya bütün davranış hatalarını dışlamaz. Otomatik test veya build çalıştırılmadı.

Eski yol haritasının aynısı [arşivde](C:/Users/Emir/BackPackDefance/Docs/Archive/Backpack_Legions_Yol_Haritasi_2026-10-01_Oncesi.md). Ayrıntılı bulgular ve mevcut kod akışı [durum raporunda](C:/Users/Emir/BackPackDefance/Docs/Backpack_Legions_Durum_Raporu_2026-10-01.md).


### 8 Ekim — Ana item kuralı güncellemesi
- [x] Kule türünü belirleyen ana item depoya konulamaz ve satılamaz. Çanta içinde taşınabilir/döndürülebilir. Depoya bırakma eski konum/yöne geri döner; normal eşyalar depoya taşınabilir. Canlı kontroller: 3/3 geçti (Validation/main-item-storage-2026-10-08.json).

### 8 Ekim — Sağdan açılan kule satın alma paneli
- [x] Boş yer → sağda dört ikon → ikonun solunda detay kartı → satın al akışı bağlandı. Gerçek fiyat/stat verileri, seçili ikon vurgusu, yetersiz para ve faz kilidi mevcut.
- [x] Büyü/okçu/birlik satın alınabilir; henüz uygulanmamış topçu dördüncü ikon olarak Yakında gösterilir.
- [x] Yeni akış 11/11, mevcut oyun regresyonu 28/28 kontrolden geçti; oyun görüntüsü incelendi. Ayrıntılı açıklama: [Kule satın alma paneli](Kule_Satin_Alma_Paneli_2026-10-08.md).
