# Backpack Legions — Güncel Geliştirme Yol Haritası

Son güncelleme: 3 Ekim 2026. İlk inceleme: 1 Ekim 2026. Kaynaklar: GDD V4 (30 Haziran 2026), Skilltree System V1, önceki yol haritası, projedeki 30 oyun scripti, canlı Unity 2022.3.62f3 oturumu ve sınırlı Play Mode kontrolleri.

## 3 Ekim 2026 — Bağımsız düşman saldırısı ve Denge V1

- [x] Emir'in kararı: birliklerin amacı düşmanı oyalamak ve canını azaltmak; düşman kendi normal saldırı ritmini kullanacak. Hasar, fiyat ve eşya bonuslarının ilk dengesi asistana bırakıldı; mimari kararları birlikte alma anlaşması sürüyor.
- [x] `EnemyData.attackInterval` ve düşmanın kendi sayacı; birlik sadece angajman başlangıcında düşmana hedef veriyor. Asker hızının düşman vuruşlarını hızlandırmaması ve pause kontrol edildi.
- [x] 37 veri asset'i ve 2 prefab için V1: okçu 14/1 sn/250 altın, büyücü 30/1,8 sn/325, birlik 4/1,2 sn/225; asker 60 can, yeniden üretim 6 sn. Fast 70 can/12 hasar/1,2 sn/20 ödül; Boss 350 can/25 hasar/1,8 sn/100 ödül.
- [x] Kule satın alma fiyatı `TowerData.cost`tan okunuyor. Eşya fiyat/bonusları düzenlendi, etkisiz parşömene bonus verildi, can iksiri birlik canına bağlandı. Normal elementin çelişen direnç/zayıflığı kaldırıldı. Kritik toplamı %100'de sınırlanıyor.
- [x] Satın alma ve stat aktarımı doğrulandı; 16 eşya/dükkan koşulu geçti. Asker 0,1 ve 3 sn saldırı aralıklarında düşman yaklaşık 1,2 sn'de bir vurdu; pause'da can değişmedi.
- [x] Üç kule, ek eşya almadan, mevcut dört dalga 10× zaman ölçeğinde çalıştırıldı: Victory, kale 64 can, düşman 0, asker 3. Bu tek yerleşim/rasgelelik koşusudur; dengeyi kesinleştiren tester sonucu değildir.
- Ayrıntılı sayılar, kod akışı, test sınırları ve kapalı test ölçümleri: [Denge V1](C:/Users/Emir/BackPackDefance/Docs/Backpack_Legions_Denge_V1.md). Yedek: `Backups/2026-10-03-balance`.

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

| Eski kayıt | 1 Ekim 2026 karşılığı |
|---|---|
| `UnitTower`, `Tower` sınıfından kopuk; `Unit.damage` set edilmiyor | Güncel değil. `UnitTower : Tower`; `base.Start()` kullanılıyor; hasar/kritik/saldırı aralığı/can bonusu askerlere aktarılıyor. |
| Barracks backpack/shop bağlantısı yapılacak | Temel bağlantı uygulandı. Skilltree, element uzmanlığı ve XP yine açık. |
| Kritik şans düzeltmesi kullanıcı tarafından uygulanacak | Uygulanmış: `Bullet` ve `Unit` 0–1 aralığında rastgele değer kullanıyor. |
| Kazanma/kaybetme ayrımı belirsiz | Aynı panel, farklı metin. `GameOver(bool)` bunu ayırıyor. |
| Dalga spawn sistemi tamam | Spawn var; yalnız son dalgada düşmanların bitmesi bekleniyor. Ara dalga hazırlık döngüsü yok. |
| Dükkan slot sıfırlaması çözüldü | Korundu; ayrıca dükkan artık kule başına saklanıyor ve satın alınan slot kule listesinden de siliniyor. |
| Kale kalpleri 100 can için bütünüyle doğru, konu kapandı | Sahnede 100 can doğrulandı. Ancak büyük tek vuruşta iki kalbin birlikte güncellenmemesi riski var; yüzdelik hesap da yok. |
| P0 tamamen bitti; bilinen engel kalmadı | Geçmiş değerlendirme. Yeni stat, başlangıç/faz, sonuç ekranı ve sahne temizliği bulguları aşağıda açık. |
| Skilltree yol haritasında yok | 18 yetenek ve bağımlılıkları eklendi. |

## Mevcut temel: yapılanlar

- [x] Her kule için bağımsız 5×5 backpack; `Backpack`, `PlacedItem`, şekil/hücre hesapları.
- [x] Mağazadan sürükleyerek satın alma ve çanta içinde taşıma kodu; geçersiz taşımanın eski konuma dönmesi; yerleşim önizlemesi. Bu incelemede fareyle uçtan uca tekrar test edilmedi.
- [x] Kule oluşunca varsayılan silahın merkeze yerleştirilmesi. Üç kule tipinde canlı olarak birer başlangıç eşyası görüldü.
- [x] Ortak gold; eşya satın alma ve ücretli reroll; düşman öldürme ödülü.
- [x] Okçu, büyücü ve birlik kulesinin mevcut satın alma metotlarıyla kurulması. Kontrolde 1000 gold → 200 gold: birlik 100, okçu 300, büyücü 400.
- [x] Kule tipi ve rarity filtresiyle dükkan üretimi. Aktif sahne havuzunda 29 item referansı var.
- [x] Her kulede dört slotluk kalıcı `myShopItems`; açılışta yeniden zar atılmıyor; satın alınan slot `null` yapılıyor; ücretli reroll yeniliyor.
- [x] Eşya hasar/kritik/hız bonuslarının `CalculateStats()` ile savaşa aktarılması; cooldown için 0.1 saniye alt sınır.
- [x] Okçu/büyücü mermilerinin `Initialize()` ile stat alması; kritik şans ölçeği düzeltmesi.
- [x] Düşmanda weakness ×1.5, resistance ×0.5 hesabı. Bu, element evrim sisteminin tamamlandığı anlamına gelmez.
- [x] `UnitTower : Tower`; backpack ve dükkan ortak tabanı; askere stat aktarımı; yaşayan askerlere yeniden hesaplama.
- [x] Birlik kulelerinin üç asker üretmesi, yakındaki boş düşmanla angajman, düşmanı durdurma, asker ölünce serbest bırakma ve boş slotu yeniden doldurma kodu.
- [x] Birlik için beş ek eşya ve `healthBonus` alanı. Can oranını koruyarak stat aktarımı canlı doğrulandı.
- [x] Dört test dalgası; son dalga sonrası düşman kalmayınca sonuç ekranı. Kontrolde düşman sayısı sıfır, iki kale bileşeninin canı 100 ve sonuç paneli açık görüldü.
- [x] Pause/continue/restart/main menu kod yolları; kazanma/kaybetme metin ayrımı. Her geçiş ve kaybetme senaryosu yeniden test edilmedi.
- [x] Dükkanın satıldı durumunu sıfırlama ve panellerin dışına tıklayınca kapatma düzeltmeleri kodda mevcut.
- [x] Kule fiyatlarını prefabın `TowerData.cost` değerinden okuma; satın alınan slotu dolu sayma ve iptal edilen slotu tekrar açma kodu.

## Önce ele alınacak doğruluk ve sahne işleri

Bu liste uygulama yetkisi değildir; sonraki geliştirme oturumunda sırayı ve çözümü birlikte seçeriz.

| İş | Kanıt / etki | Bitti sayılma ölçütü |
|---|---|---|
| Temel stat kaynağı — 2 Ekim'de çözüldü | Başlangıç değerleri `TowerData`dan kopyalanıyor; büyücü 25 hasar, okçu 2 saniye. | Üç tipte item hesabı ve aynı asset'i kullanan iki büyücüde upgrade izolasyonu canlı doğrulandı. |
| Hazırlıkta erken spawn — 2 Ekim'de çözüldü | Üretim `Start`tan kaldırıldı; açık başlatma komutu gerekiyor. | Başlat öncesi 0 düşman, ilk Başlat sonrası 1 düşman; tekrarlı Başlat ek döngü açmadı. |
| Oyun sonu ve pause sınırları — 2 Ekim'de çözüldü | Durum GameManager'da tutuluyor; pause önceki hıza dönüyor; terminal sonuç korunuyor. | Hazırlık/savaş pause, kazanma/kaybetme kilidi ve restart canlı kontrolleri geçti. |
| Eksik script bileşenleri — 2 Ekim'de temizlendi | 28 eksik bileşen kaldırıldı; nesneler ve çalışan bileşenler korundu. | Canlı tarama 0 eksik script; 388 nesne ve mevcut oyun scripti referansları aynı; başlangıç kontrolü geçti. |
| Kale kalpleri — tamamlandı (2 Ekim) | `Base.UpdateHealthUI` üç kalbi can oranıyla güncelliyor; tek vuruşta ölüm hatası düzeltildi. | 25 canlı koşul geçti; 100→0, 100→20 ve farklı maxHealth değerleri doğrulandı. |
| Birlik karşı saldırı ritmi — tamamlandı (3 Ekim) | Düşman kendi attackInterval sayacıyla vuruyor; birlik oyalama ve yardımcı hasar rolünde. | İki farklı asker saldırı hızıyla düşmanın aynı ritimde vurduğu canlı doğrulandı. |
| Deneme içeriğini ayırmak — ertelendi | Emir'in 3 Ekim kararı: `PlaneTest` olduğu gibi kalacak. | Eski alanın hiçbir etkisinin olmadığı iddia edilmiyor; şu an değişiklik yapılmayacak. |
| UI isim/olay eşleşmesi — tamamlandı | İkon/olay doğru, fiyat etiketlerinin adları tersti; iki düğme ve iki fiyat nesnesi yeniden adlandırıldı. | Üç türde görünen fiyat, seçilen tür, kesilen altın ve oluşan kule canlı doğrulandı. |
| Veri temizliği | Dükkan itemID 5–10 çiftleri tekrarlı; dört 1×2 itemin `centerOffset=(1,1)` noktası şekil dışında. FastEnemy Normal'e hem zayıf hem dirençli. | Kimlikler ve offset davranışı net; düşman listeleri kasıtlı/çelişkisiz. |
| Dükkan/seviye sınırı | Mevcut havuzda dört tipin tüm rarity kümeleri dolu; kodda boş havuz ve seviye>5 koruması yok. | İçerik değişiminde boş küme güvenli ele alınır; seviye sınırı davranışı belirlenir. |

## Önerilen ilerleme sırası ve kabul ölçütleri

### Aşama 1 — Mevcut prototipi güvenilir hale getirmek

- [ ] Yukarıdaki bulguları Emir ile önceliklendirmek; ilk olarak stat kaynağı ve başlangıç/sonuç geçişleri.
- [ ] Üç kule + eşya alımı/taşıma + dükkan aç/kapa + reroll + kazanma/kaybetme + restart kontrolünü tekrarlanabilir hale getirmek.
- [ ] Eksik script ve eski test nesnelerinin gerekli/eski ayrımını yapmak.
- [ ] Değişiklikler öncesi geri dönüş yöntemi belirlemek. Bu klasör şu anda Git deposu olarak algılanmıyor; alternatif yedekleme/VCS ayrıca teyit edilecek.

Çıkış ölçütü: temel akışın beklenen sayıları ve geçişleri kayıtlı; kritik açıklar giderilmiş veya açıkça kabul edilmiş; Emir mevcut akışı açıklayabiliyor.

### Aşama 2 — GDD hazırlık / savaş döngüsü

- [ ] Dalga başlatma, savaş bitişi, hazırlığa dönüş ve oyun sonu kurallarını tanımlamak.
- [ ] Savaşta eşya satın alma/taşıma/döndürme/satış ve shop işlemlerini kilitlemek; açık drag'in faz geçişindeki davranışını belirlemek.
- [ ] Hedefleme kontrolünü envanter kilidinden ayrı tutmak.
- [ ] Son düşman sonrası hazırlık ve dondurulmamış dükkanın ücretsiz yenilenmesi.
- [ ] Global Freeze: hangi kulelerin shop'larını kapsadığına karar verip uygulamak.
- [ ] 1×/2×/3× hız, pause ve continue sırasında önceki hızın korunması.
- [ ] Savaşta yeni kule kurma ve kule yükseltmenin izin durumunu netleştirmek.

Çıkış ölçütü: Hazırlık → Savaş → Hazırlık döngüsü en az iki dalgada işler; kilitli işlem gold/grid/shop durumunu değiştirmez. Sınıf/enum/manager seçimi birlikte yapılacak.

### Aşama 3 — Envanter ve ekonomi derinliği

- [ ] Rotasyon isteğini teyit etmek; GDD istiyor, mevcut uygulama içermiyor. Input, dönme merkezi ve taşma davranışı kararlaştırılacak.
- [ ] Sinerjinin koşulunu tanımlamak: komşuluk, eşya eşleşmesi, element veya başka kural; sonra hesap ve görsel geri bildirim.
- [ ] Craft/birleştirme tarifleri, tüketilen öğeler, oluşan öğenin yeri ve başarısız işlem davranışı. GDD'de var, eski roadmap'te eksikti.
- [ ] %50–60 arası refund oranı, yuvarlama ve hangi maliyetin baz alınacağı.
- [ ] Satışın statlara ve varsayılan/ana silaha etkisi; kuleler arası transfer yasağının uç durum testleri.

Çıkış ölçütü: aynı eşya havuzunda farklı yerleşimlerin farkı ölçülür; iptal edilen işlem item/gold kaybettirmez veya çoğaltmaz.

### Aşama 4 — Hedefleme ve element/XP

- [ ] Kara/hava, yakın/uzak hedef kuralları; düşmanın hareket tipi ve element kimliği.
- [ ] XP'nin kime/nasıl yazıldığı; seviye 5 bekleme ve element seçim zamanı.
- [ ] Element listesi: mevcut Normal/Fire/Water/Earth/Air ile belgelerdeki Ateş/Doğa/Buz ve Neutral Enemy'nin eşleştirilmesi.
- [ ] Ana silah satışıyla element değişimi; dükkan havuzu geçişi; eski eşyaların aktif/pasif işaretlenmesi.
- [ ] Tüm envanter boşken kule tipi değişimi.
- [ ] Kule başına element değişim cezası; oran ve çarpım sırası.
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

## Karar defteri — henüz kararlaştırılmadı

1. Karar alındı: oyun durumu `GameManager`da, dalga üretimi `WaveManager`da. İlk başlangıç/pause/sonuç adımı tamamlandı; dalga arası hazırlık ve etkileşim kilitlerinin ayrıntıları henüz açık.
2. Kule temel statlarının tek kaynağı ne olacak; `attackSpeed` adı hız mı süre mi ifade edecek?
3. Rotasyon kesin kapsamda mı ve oyuncu nasıl döndürecek?
4. Global Freeze tüm kuleleri mi, seçili kulenin bütün slotlarını mı dondurur?
5. Uzak hedef: kuleye uzaklık mı, yol ilerlemesi mi? Kara/hava ifadeleri yalnız o tür mü, öncelik + fallback mi?
6. Element cezası `1 + 0.10×n` mi, `1.10^n` mi? GDD'nin “katlanarak” ifadesi ile %10→%20→%30 örneği aynı şey değil.
7. Skilltree run içi HQ harcaması mı, kalıcı ilerleme mi? Belge ikisini de seçenek olarak bırakıyor.
8. Aynı hedefte “başarılı atış” isabet mi ateşleme mi; stun tekrarları nasıl davranır?
9. Zırh, büyü/fiziksel hasar, kritik ve element çarpanlarının sırası ne?
10. Karar alındı (3 Ekim): düşman kendi saldırı ritmini kullanır; birlik hız bonusu düşmanı hızlandırmaz. İlk sayısal denge asistana bırakıldı, kapalı test geri bildirimleriyle güncellenecek.
11. XP dağıtımı, craft tarifleri ve ana silah satıldığında silahsız kulenin davranışı ne?

## İnceleme sınırı ve kayıt

Canlı kontrol: sahne/prefab/data referansları, Console örneklemesi, mevcut metotlarla üç kule satın alma, çalışma zamanı statları, savaşın ilerlemesi ve sonuç paneli; geçici item ekleyip çıkararak birlik hasarı/can oranı kontrolü. Bu son kontrol fareyle shop satın alma testi değildir. Bu tur yeni oyun kodu, prefab veya sahne düzenlemesi yapılmadı.

Test sonunda Play kapalı, `GameScane` açık, sahne `dirty=false`, `runInBackground=false` ve test kuleleri yok. Console'da kontrol anlarında hata/uyarı gelmedi; bu, eksik script bileşenlerini veya bütün davranış hatalarını dışlamaz. Otomatik test veya build çalıştırılmadı.

Eski yol haritasının aynısı [arşivde](C:/Users/Emir/BackPackDefance/Docs/Archive/Backpack_Legions_Yol_Haritasi_2026-10-01_Oncesi.md). Ayrıntılı bulgular ve mevcut kod akışı [durum raporunda](C:/Users/Emir/BackPackDefance/Docs/Backpack_Legions_Durum_Raporu_2026-10-01.md).

