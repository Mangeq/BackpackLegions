# Backpack Legions — ekip üretim ve yayın planı

Plan tarihi: **14 Ekim 2026**. Saatler Türkiye saati. Bu belge bir üretim tahminidir; Steam etkinlik tarihleri dışındaki tarihler ekibin hedefidir, yayın garantisi değildir. İlk iki haftanın gerçek çalışma süreleriyle 31 Ekim'de yeniden ölçülecek. Günlük görevler: [Gün gün takvim](Gunluk_Uretim_Takvimi_2026-2027.md).

## 1. Önerilen çıkış takvimi

| Kilometre taşı | Hedef tarih | Teslim / anlamı |
|---|---|---|
| Dört ana kulenin seviye 1, elementsiz modelleri | **17 Ekim 2026 Cumartesi** | Kullanıcının bu hafta sonu teslim bilgisi; pazar izin kalabilsin diye cumartesi kabulü planlandı |
| İlk entegre dört kule build'i | 24 Ekim 2026 | Gerçek modellerle satın alma, saldırı, seçim ve çanta |
| Steam mağaza sayfasını incelemeye gönderme | 19 Kasım 2026 | Kapsüller, açıklamalar, gerçek görüntüler ve temel bilgiler hazır |
| Steam Coming Soon sayfası | **3 Aralık 2026 Perşembe** | Onaya bağlı; wishlist toplamaya başlama |
| Kapalı demo alpha | **14 Ocak 2027 Perşembe** | 10–15 dış oyuncuyla ilk deneyim testi; bütün oyun tamamlanmış değil |
| Herkese açık ilk demo | **4 Şubat 2027 Perşembe** | İki farklı harita, dört kule ve oyunun ayırt edici çanta/element döngüsü |
| Next Fest kayıt için ekip içi son gün | **16 Nisan 2027 Cuma** | Ana oyun üzerinden kayıt, yeşil onay işaretinin kontrolü |
| Festival demosunun kapsamını dondurma | 6 Mayıs 2027 | Yeni sistem eklenmez; iyileştirme ve hata düzeltme |
| Festival inceleme durumunu kesinleştirme | 10 Mayıs 2027 | Demo zaten yayımlanmış olsa da güncel kontrol listeleri/onaylar kontrol edilir |
| Festival demo güncellemesi | **27 Mayıs 2027 Perşembe** | İlk demonun oyuncu verileriyle cilalanmış sürümü |
| Steam Next Fest | **14–21 Haziran 2027, 20.00–20.00** | Resmî etkinlik; Türkiye saati dönüşümü |
| Tam oyun içerik kilidi | **17 Temmuz 2027 Cumartesi** | 20 bölüm ve bütün zorunlu sistemler; yeni içerik açılmaz |
| Kapalı tam oyun beta | **29 Temmuz 2027 Perşembe** | Steam Playtest ile 30–50 oyuncu hedefi; ücretli çıkış değil |
| Daha geniş beta | **19 Ağustos 2027 Perşembe** | Önceki test geçerse 100 oyuncuya kadar; sayı erişim hedefi, garanti değil |
| Son yayın adayı / Steam build incelemesi | **2 Eylül 2027 Perşembe** | Tam özellikli sürüm; onay ve düzeltme payı bırakılır |
| Yayın / erteleme kararı | **11 Eylül 2027 Cumartesi** | Aşağıdaki kalite koşullarıyla karar |
| Tam sürüm 1.0 | **23 Eylül 2027 Perşembe** | Windows PC, Steam; erken erişim değil |
| İlk destek dönemi | 24 Eylül–7 Ekim 2027 | Hata düzeltme, kayıt uyumluluğu, destek |

**Neden Haziran festivali?** Şubat 2027 etkinliği 22 Şubat–1 Mart; bu planın ilk demo yayınına çok yakın ve tam oyuna yaklaşık yedi ay uzakta. Haziran seçimi görselleri, ilk oyuncu deneyimini ve mağaza sunumunu geliştirme zamanı verir. Bir oyun yalnız bir Next Fest'e katılabildiğinden Şubat'a başvurup Haziran'a da katılmayı planlamıyoruz. Şubat'ta normal demo yayımlamak tek başına festival katılımı değildir. [Resmî genel kurallar](https://partner.steamgames.com/doc/marketing/upcoming_events/nextfest), [Şubat takvimi](https://partner.steamgames.com/doc/marketing/upcoming_events/nextfest/feb_2027).

## 2. İncelemenin kapsamı ve mevcut durum

Bu çalışma kaynak kod, veri asset'leri, Build Settings, mevcut yol haritaları, 2D brief ve geçmiş doğrulama kayıtlarının incelenmesine dayanır. **Bu tur Unity'de oyun çalıştırılmadı, yeni derleme veya tam oynanış testi yapılmadı.** Kodun bulunması, üretim kalitesinde ve hatasız olduğu anlamına gelmez. Geçmiş test sonuçları yeni test sonucu olarak sunulmuyor.

Unity **2022.3.62f3**, URP **14.0.12**. `Assets/Scripts` altında **42 C# dosyası / 4.696 satır** var; satır sayısı tamamlanma yüzdesi değildir. Build Settings'te Boot, MainMenu, GameScane açık. Beş ChapterData kaydının tamamı aynı GameScane sahnesini kullanıyor. Beş bölüm verisi var; beş ayrı bitmiş harita yok.

Kökteki `YOL_HARITASI.md` 22 Eylül tarihli ve birçok eski eksik içeriyor. `Docs/Backpack_Legions_Yol_Haritasi.md` daha güncel; onun tarihsel alt bölümlerindeki 30 eşya / üç kule gibi eski sayılar da üstteki yeni kayıtlarla birlikte okunmalı. Bu plan için mevcut kod ve 14 Ekim verileri esas alındı. Çalışma ağacında çok sayıda mevcut değişiklik ve veri taşıması var; bunlara müdahale edilmedi.

| Sistem | Bugünkü değerlendirme | Kod / veri kanıtı | Kalan üretim işi |
|---|---|---|---|
| Hazırlık, savaş, zafer, yenilgi | Uygulanmış | GameManager.cs; WaveManager.cs | Yeni görsellerle bütün akış, yükleme ve tekrar başlatma testleri |
| 1×/2×/3×, pause, işlem kilitleri | Uygulanmış | GameManager.cs; UİManager.cs | Aynı senaryoyu üç hızda çalıştırma; animasyon/VFX uyumu |
| Kule satın alma | Dört sınıf mevcut | ArcherTower, MegicTower, UnitTower, MortarTower | Son modeller, okunabilir siluet ve saldırı sunumu |
| Kuleye özel çanta/depo | Uygulanmış | Backpack.cs; BackpackUIManager.cs | Son UI, farklı ekran boyutu, uzun kullanım testi |
| Sürükleme, bırakma, rotasyon | Uygulanmış | BackpackDraggable.cs; Itemshapeutility.cs | Fiziksel fareyle kenar durumları, erişilebilir açıklamalar |
| Dükkan, rarity, reroll, freeze | Uygulanmış | ShopManager.cs; Tower.cs | RNG/ekonomi testleri; tooltip ve satış sunumu |
| Satış ve kule tipi değişimi | Uygulanmış | EconomyManager.cs; Tower.TryChangeTowerType | Depo/teslim/XP durumlarının regresyonu |
| Komşuluk sinerjisi | Uygulanmış | ItemSynergyCalculator.cs | Eğitici sunum, tarif çeşitliliği ve sayısal denge |
| Dalga sonunda eşya evrimi | Uygulanmış | TowerItemEvolution.cs | Dolu çanta/depo, son dalga, teslim iptali senaryoları |
| XP ve element seçimi/değişimi | Uygulanmış | Tower.cs; Enemy.TakeDamage/Die | Son 3D/VFX temsili; seviye görünüm tasarımı |
| Hedefleme | Uygulanmış | Tower.TargetType/TargetOrder | İlk/son menzile giriş davranışını doğru anlatan UI |
| Birlik ve hava düşmanı | Uygulanmış | UnitTower.cs; Unit.cs; Enemy.cs | Yeni model/rig, animasyon, kalabalık yol ve viraj testleri |
| Havan alan hasarı | Uygulanmış; yerel havuz var | MortarTower.cs; MortarProjectile.cs | Diğer efektlerle birlikte performans ve görsel cila |
| Bölümler/dalgalar | Beş veri bölümü | Chapter01–05.asset; ChapterData.cs | 20 ayrı yol düzeni, dört tema, farklı taktikler |
| Eşyalar | 240 ItemData kaydı | Items/Shop: 200; Main: 20; Evolved: 20 | 48 temel kayıt × 5 element durumu için 2D üretim; bunlar 240 özgün mekanik değil |
| Ana menü, bölüm seçimi, yükleme | Uygulanmış | MainMenuManager.cs; SceneLoadingUI.cs | Son tasarım, kilit açma, kayıtla bağlantı, İngilizce |
| Ayarlar | Mevcut, PlayerPrefs kullanıyor | SettingsPanelUI.cs | Müzik/SFX ayrımı, daha geniş ekran testi |
| Oyun ilerlemesini kaydetme | İncelenen kodda yok | ChapterData.SelectedIndex yalnız static; PlayerPrefs ayarlara ait | Sürümlü kayıt, yedek, bozuk kayıt dönüşü, bölüm/HQ/skill/bestiary |
| Castle HQ / skilltree | Açık | Güncel yol haritasında 18 yetenek listesi | Kurallar, stat katmanı, ekranlar, yetenekler, test |
| Bestiary / öğrenme görevleri | Açık | İncelenen runtime kodunda bulunmadı | Karşılaşma/kill sayaçları, ödül ve kayıt |
| Hasar analizi | Kısmi temel | Enemy gerçek hasarı kuleye yazıp XP dağıtıyor | Kritik/isabet/yetenek bilgisi ve kalıcı savaş özeti eksik |
| Öğretici | İncelenen oyun kodunda yok | Unity TutorialInfo paketini oyun öğreticisi saymıyoruz | İlk kule, ilk eşya, rotasyon, sinerji, element adımları |
| Ses/animasyon/VFX | Kısmi / üretim işi | Havan ParticleSystem ve mermi renkleri mevcut | Tam ses sistemi ve yeni modellerin bütün animasyonları |
| Steam | Yerel pazarlama taslağı var | Marketing/Steam/2026-10-09/README.md | Hesap/AppID/sayfa yayını ve entegrasyon durumu dışarıdan doğrulanmadı |

## 3. Koddan çıkan somut öncelikler

1. **Yeni arayüz gelirken referansları kırmamak.** GameManager.Awake, panel altındaki `BackMainMenuButton`, `CountineButton`, `Text (TMP)` gibi isimlere doğrudan erişiyor. UİManager yaklaşık 733 satır ve static bağlantılar taşıyor. Freelancer'ın hiyerarşisini doğrudan değiştirmek null referanslara yol açabilir. Ekimde kritik UI referanslarını açık bağlama, prefab doğrulama; kalan ayrıştırmayı ihtiyaç oldukça yapma. Tüm projeyi baştan yazma işi açılmayacak.
2. **Kayıt ile ayar kaydını ayırmak.** SettingsPanelUI zaten ayar saklıyor; yapılacak iş kampanya/meta kayıt sistemidir. ID ve sürüm şeması önce tasarlanmalı. Bozuk dosya, eski sürüm, reset, demo/tam oyun ayrımı ve yedek geri dönüşü test edilmeli.
3. **20 haritaya geçmeden bölüm yüklemeyi düzeltmek.** ChapterData sceneName taşıyor; Enemy Start hedef kaleyi tag ile buluyor. Yeni sahnelerin kale, spawn, yol/NavMesh, kule noktası ve bölüm verisi sözleşmesi olmalı. Ortak modüler kit ile farklı taktik yol düzenleri üretilecek.
4. **Skilltree öncesi stat ve hasar kaynağını netleştirmek.** Tower.CalculateStats temel + eşya + sinerji hesabını yapıyor. Genel kaynak/modifier katmanı yok. Enemy.TakeDamage sahibini biliyor; hasar kaynağı tamamen yok demek yanlış. Ancak kritik ve tetiklenen yetenek kimliği korunmuyor. Bu bilgi ölçüm ve özel yeteneklerden önce eklenecek.
5. **GDD ile güncel kararları uzlaştırmak.** Elementler Normal/Ateş/Su/Toprak/Hava; eski skilltree Doğa/Buz diyor. Katalizör yeteneği katlanan ücret varsayıyor; güncel ücret sabit. Havanın skill dalı tanımlı değil. Bunlar kod hatası değil, tasarım boşluğu. 24 Ekim'de ürün kararı yazılacak; Martta uygulanmadan önce tekrar kesinleştirilecek.
6. **Performansta havuzu sıfırdan varsaymamak.** Havan sekiz mermiyle ısınan havuza sahip. Normal Bullet ve WaveManager üretimi Instantiate/Destroy kullanıyor. WaveManager son düşmanı tag taramasıyla bekliyor. Önce gerçek build'de profil; sonra sık maliyetli üretimlere havuz ve aktif düşman sayacı.
7. **Test kapsamını dört kuleye genişletmek.** Eski 28 koşulluk test üç kuleyi temel alıyor; sonradan havan/element/evrim için ayrı kayıtlar var. Bunlar birleştirilmeli; özellikle demo build'inde mağaza → çanta → evrim → element → sahne geçişi test edilmeli.
8. **Son denge henüz kapanmış değil.** 14 Ekim başlangıç altınları 850/1000/1150/1300/1450. Bölüm başına tam koşu, boss, reroll şansı ve element kullanma gereksinimi ölçülecek. Beş veri dosyasının bulunması dengeli kampanya demek değil.
9. **Metinler kodda gömülü.** MainMenuManager, GameManager ve UI scriptlerinde Türkçe stringler var. İngilizce/TR tabloya taşıma ve font kontrolü UI entegrasyonuna dahil. Bütün dilleri birden hedeflemiyoruz.
10. **Seviye/model kombinasyonları sınırsız büyümemeli.** Tower.ProcessExperience element seçimi sonrasında seviye artırmayı sürdürüyor. Her sayı için yeni model yapılmayacak: ortak gövde + görsel kademe parçaları + element eklentisi kullanılacak. Gameplay seviye tavanı ayrı tasarım kararı.

Bu maddeler statik inceleme bulguları ve riskleridir; yeni bir canlı testle doğrulanmış hata listesi değildir. Yeni görsel teslimlerinden önce mevcut sürümün çalışır Windows build'i arşivlenmeli.

## 4. Ekip rolleri ve sürdürülebilir kapasite

| Kişi | Kesin sorumluluk | Haftalık üst süre | İş taahhüdü | Toplantı / aksama payı |
|---|---|---:|---:|---:|
| Emir | Kod, Unity entegrasyonu, oynanış, VFX, rig/animasyon, build | 6 × 4 = 24 saat | **18 saat** | 6 saat |
| 3D arkadaş | Bütün 3D modeller, UV, materyal/texture, modüler çevre, model düzeltmeleri | 6 × 3 = 18 saat | **13,5 saat** | 4,5 saat |
| Operasyon arkadaşı | Steam, freelancer takibi, QA organizasyonu, destek, tanıtım, lisans/envanter | **6 saat varsayımı kabul edildi** | 4,75 saat bağımsız iş | 1,25 saat toplantı |
| 2D freelancer | Bütün 2D görseller, UI, ikon, logo/kapsül, pazarlama 2D çıktıları | Sözleşmeye bağlı | Aşamalı teslim | En az iki revizyon turu |

2D bütçesi belirtilmedi; bir rakam varsayılmadı. Freelancer kapasitesi ekip içi saat hesabına eklenmedi. Teklif ve başlangıç tarihi 24 Ekim'e kadar netleşmezse mağaza ve demo takvimi aynı gün yeniden hesaplanmalı. Operasyon arkadaşının altı saati yoğun haftalarda yetmezse yayın/QA işleri öncelik kazanır; içerik paylaşımı azaltılır.

**Pazar herkes için off day.** İlk teslim 17 Ekim cumartesi; 18 Ekim pazar entegrasyon beklenmez. Bir hafta düğün/seyahat nedeniyle bir gün kaybolursa önce boş kapasite kullanılır. İki veya daha fazla gün kaybında haftanın en düşük öncelikli işi sonraki haftaya alınır; gece mesaisi veya pazarla borç kapatılmaz. Bayram/özel günler ilk toplantıda kişisel takvime işlenir; bu plan bunları çalışma zorunluluğu saymaz.

### Haftalık çalışma ve toplantı düzeni

Öneri çalışma saati: Emir 20.00–00.00, modelci 20.00–23.00; günlük takvim tarihi oturumun başladığı gündür. Saatler kişisel düzene göre kaydırılabilir, toplam süre artırılmaz.

| Gün | Emir (4 saat içinde) | Modelci (3 saat içinde) | Operasyon | Ortak ritim |
|---|---|---|---|---|
| Pazartesi | 3,5 saat haftanın ilk işi + 0,5 pay | 2,5 saat taslak/üretim + 0,5 pay | 30 dk hafta listesi | Yazılı hedef; toplantı yok |
| Salı | 3,5 saat uygulama + 0,5 pay | 2,5 saat model + 0,5 pay | 45 dk Steam/freelancer | Blokeri yazıyla bildir |
| Çarşamba | 3,5 saat uygulama/entegrasyon + 0,5 pay | 2,5 saat UV/materyal + 0,5 pay | 45 dk testçi/iletişim | Ara teslim görüntüsü |
| Perşembe | 3,5 saat bütünleştirme + 0,5 pay | 2,5 saat tamamla + 0,5 pay | 45 dk mağaza/iletişim | Yayın hedefleri bu güne yerleşir |
| Cuma | 3 saat test/düzeltme + 15 dk sync + 45 dk pay | 2,5 saat düzeltme + 15 dk sync + 15 dk pay | 1 saat QA + 15 dk sync | **20.00–20.15 engel toplantısı** |
| Cumartesi | 1 saat kabul/build + 1 saat toplantı + 2 saat tampon | 1 saat teslim + 1 saat toplantı + 1 saat tampon | 1 saat rapor + 1 saat toplantı | **20.00–21.00 haftalık oynanış ve planlama** |
| Pazar | İzin | İzin | İzin | Mesaj/teslim/toplantı zorunluluğu yok |

Cumartesi gündemi: 10 dk önceki hedefler, 20 dk build oynama, 10 dk sanat kabulü, 10 dk hata/oyuncu verisi, 10 dk sonraki hafta. Ayın son cumartesisi aynı toplantıda kapsam/saat/bütçe kontrolü yapılır; ek toplantı eklenmez. Freelancer toplantıya zorunlu katılmaz; operasyon tek toplu geri bildirim gönderir.

### Çıkışa kadar iş bütçesi

19 Ekim 2026–26 Eylül 2027 arası 49 haftalık plan penceresi: Emir **882 saat**, modelci **661,5 saat** planlanabilir üretim; bunların tamamı 23 Eylül sabahında hazır değildir, son haftanın bir kısmı çıkış desteğidir. Toplantı/aksama rezervleri bunun dışındadır. Aşağıdaki sayılar teklif değil, ilk planlama tahminidir; iş paketleri kısmen paralel ilerler.

| İş paketi | Emir saat | Modelci saat |
|---|---:|---:|
| Başlangıç stabilizasyonu, tekrar üretilebilir build | 24 | 12 |
| 3D entegrasyonu, rig/animasyon, VFX, ses bağlantıları | 180 | 170 |
| UI entegrasyonu, öğretici, TR/EN, kullanılabilirlik | 90 | 0 |
| Kayıt, stat/hasar katmanı, HQ, 18 yetenek, bestiary, analiz | 200 | 12 |
| 20 bölüm, dört tema, dalga/ekonomi içerikleri | 130 | 300 |
| Profil, regresyon, beta ve cila | 150 | 90 |
| Steam build, yayın ve tanıtım teknik desteği | 65 | 20 |
| İlk destek / beklenmeyen küçük üretim işleri | 20 | 20 |
| **Toplam iş tahmini** | **859** | **624** |
| **Planlanabilir kapasiteden kalan** | **23** | **37,5** |

Ekibin özellikle yazılım tarafında alanı dar. 859 saatten %20 sapma yaklaşık 172 saat / 9,5 üretim haftası ekler. Bu durumda Eylül tarihini zorlamak yerine Kasım–Aralık 2027 penceresi veya açıkça küçültülmüş kapsam gerekir. Haftalık aksama payı zaten düşüldü; aynı pay ikinci kez içerik eklemek için kullanılmayacak. İlk dört kule modelinin hafta sonuna kadarki üretimi bu ileri dönem tablosunda yeniden tam maliyet olarak sayılmadı.

## 5. Sürümlerin kesin iş kapsamı

### İlk demo — 4 Şubat

- Dört kule sınıfı; gerçek 1. seviye nötr modeller, elementleri ayıran modüler işaretler ve VFX. Her level için özgün model zorunlu değil.
- Tek çevre teması içinde **iki ayrı yol düzeni**: öğretici başlangıç ve mekaniklerin kullanıldığı ikinci bölüm. Hedef ilk koşu 25–40 dakika; gerçek test sonucu doğrulanacak.
- Çanta/depo, rotasyon, satış, sinerji, mevcut evrim tarifleri, XP, dört element, hedefleme ve kule değişimi. 240 kayıtlık mevcut havuz korunabilir; yeni özgün eşya sayısı açılmaz.
- Temsilî kara/hızlı/dayanıklı/hava düşmanı rolleri ve bir boss görünümü; sayısal varyantlar ayrı model siparişi sayılmaz.
- Ana menü, ayarlar, pause/sonuç, öğretici, TR/EN, temel ilerleme/ayar kaydı, ses ve hata raporu yolu.
- HQ/skilltree/bestiary zorunlu değil; menüde çalışmayan butonlarla vaat edilmez. Demo ayrı uygulama olarak ana oyuna bağlı; demo save'i ana oyun profilini bozmamalı.

### Festival demosu — 27 Mayıs

İlk demonun aynı odaklı kapsamı; daha iyi öğretici, denge, animasyon, ses, performans ve mağaza yönlendirmesi. Tam oyunun bütün bölümlerini ücretsiz açma işi yok. Meta sistemler oyuncuya değer katıyorsa küçük bir tanıtım bölümü eklenebilir; festival takvimi bunlara bağımlı yapılmaz.

### Tam oyun beta / 1.0

- **4 tema × 5 bölüm = 20 farklı oynanış haritası.** Ortak modüler kitler tekrar kullanılır; her haritaya sıfırdan benzersiz 3D set yapılmaz.
- Dört kule, dört element, nötr durum; gövde + kademe + element eklentisi sistemi. Öneri üç görsel kademe; gameplay seviye sayısından bağımsız.
- Mevcut 240 eşya verisinin tamamında final 2D karşılık; dört temel evrim tarifinin element karşılıkları. Ek tarifler ancak mevcut iş bütçesinden yer açılarak.
- Önerilen sanat üst sınırı: **8 düşman gövde/rig ailesi + 4 boss görünümü + 1 asker tabanı**; ortak rig ve varyasyon kullanılabilir. Bunlar mevcut tamamlanmış asset sayıları değil, bu takvimin üretim sınırlarıdır.
- Kampanya kayıt/kilit açma; HQ, 18 yetenek, bestiary öğrenme görevleri ve kule hasar/DPS özeti. Öneri skilltree kalıcı ilerleme, HQ bölüm içi altınla sınırlı bonus; bu henüz kullanıcı kararı değil, 24 Ekim toplantı maddesi.
- Havan başlangıçta global HQ ve ortak bonuslardan yararlanır. Ayrı altı yetenekli havan dalı yeni kapsamdır; 1.1'e önerilir.
- TR/EN, müzik/SFX ayarları, Steam dağıtımı, kayıt uyumluluğu; başarım ve Cloud için küçük sınırlı paket ve gerçek hesap testi.
- Multiplayer, mobil/konsol portu, workshop, günlük görev/live service, sınırsız yeni eşya ve her kule-level-element için özgün model bu 1.0 takviminde yok.

### 18 yeteneğin uygulama grupları

| Grup | Yetenekler | Ön bağımlılık / karar |
|---|---|---|
| Okçu pasifler | Okçu Eğitimi I, Hafif Yaylar, Ağır Kiriş, Kritik Odak, Element Okları | Hasar/hız/menzil/zırh/kritik sırası; mevcut dört elementle uyum |
| Okçu özel | Kusursuz Nişancılık | Başarılı isabet sayacı, dördüncü vuruş, stun bağışıklık/süre kuralları |
| Büyücü pasifler | Mana Akışı I, Rün Bilgisi, Katalizör, Kaotik Element, Yıkım Büyüsü | RNG ağırlığı, etiketler, sabit değişim ücretine indirim, AoE kapsamı |
| Büyücü özel | Kıyamet Sevdalısı | Hazırlıktaki element değişimi hangi savaş dalgasını güçlendirir? |
| Birlik pasifler | Ordu Düzeni I, Demir Dövme, Kışla İkmali, Element Zırhı | Eşya kaynak ayrımı, spawn aralığı, alınan hasarın elementi |
| Birlik özel | Çifte Sancak, Lejyoner Sinerjisi | Geçici kapasite aşımı, slotlar, sayılan asker ve bonus tavanı |

Değerler eski tasarımdan kopyalanıp kilitlenmeyecek; uygulanmış bonusların analizde ölçülebilmesi ve kayıt yüklemede iki kez uygulanmaması kabul koşulu olacak.

## 6. Sanat ve dış kaynak teslim planı

**3D sorumluluğu arkadaşta; rig/skin ve animasyon uygulaması Emir'de.** Arkadaş animasyona uygun topoloji, ayrık dönecek parçalar, pivot, ölçek ve UV sağlar. Bir kuleyi teslim almak yalnız FBX dosyasını almak değildir: kaynak model, texture/material, ölçek/yön, muzzle/bağlantı noktaları ve Unity içindeki örnek kontrolü gerekir. İlk kulede ortak standardı onaylamadan diğer üçünü çoğaltmayın.

| Paket | Hedef | Kabul |
|---|---|---|
| 3D A | 17 Ekim: dört nötr L1 kule | Dört belirgin siluet, doğru pivot/ölçek; eksik texture yok |
| 3D B | Ekim sonu–Kasım: demo çevre kiti + asker/düşman ilk aileleri | İki yol düzeninde tekrar kullanılabilen parçalar, rig'e uygun mesh |
| 3D C | Aralık: demo boss + element eklentileri | Sadece renk olmayan element işaretleri; kamera mesafesinde okunur |
| 3D D | Şubat–Temmuz: kalan üç tema, kademe parçaları, düşman/bosslar | Önce haftalık parça teslimi; sonra harita paketleri |
| 2D pilot | 24 Ekim | Bir temel eşya + dört varyasyonu, HUD ve çanta paneli; ücretli pilot |
| 2D mağaza | 12 Kasım | Logo, kapsül/kütüphane görselleri; resmî formatlarla export |
| 2D ana arayüz | 21 Kasım | HUD, çanta, mağaza, tooltip, hedefleme, element ve durumlar |
| 2D eşya partileri | 28 Kasım / 12 Aralık / 19 Aralık | Toplam 240 ID eşleşmesi; partiler pilot üretim hızına göre bölünür |
| 2D demo kapanış | 9 Ocak | Menü/sonuç/yükleme/ayarlar/öğretici, iki revizyon turu tamam |
| 2D meta ekranları | 27 Mart | HQ, skilltree, bestiary, analiz; ayrı teklif, sonradan bedelsiz iş varsayılmaz |
| 2D festival paketi | 24 Nisan | Güncel kapsül ve tanıtım grafikleri |
| 2D 1.0 kapanış | 14 Ağustos | Dört tema bölüm haritası, bütün eksik ikonlar ve son revizyon |

Freelancer için mevcut [ayrıntılı 2D brief](2D_Artist_Freelance_Brief_2026-10-14.md) temel alınacak. Buna öğretici, HQ, skilltree, bestiary, analiz ve dört temalı bölüm seçimi ayrı satırlar olarak ek teklif istenecek. VFX için gerekli çizilmiş sprite/texture da 2D siparişe dahil; efekt sistemi, shader davranışı, particle ayarı ve animasyonu Emir yapar. Model UV texture üretimi 3D paketin parçasıdır.

Operasyon arkadaşı teklif, sözleşme, kaynak dosya/ticari kullanım, font ve müzik lisanslarını takip eder. Ses tasarımcısı bu ekipte tanımlı değil: lisanslı SFX/müzik seçimi operasyon + Emir, oyuna bağlama/mix Emir. Özgün besteci istenirse ek bütçe ve teslim tarihi gerekir. Trailer oyun kaydı Emir, kurgu operasyon; kurgu kapasitesi yoksa ayrıca fiyatlandırılmış freelancer işi. Mağaza ve oyun içi 2D tasarım yine freelancer'da.

## 7. Steam ve Next Fest kontrol takvimi

Hesap/sayfa durumuna erişilmedi; aşağıdakiler yapılacak işlerdir. Bu görevde hesap açılmadı, ücret ödenmedi, başvuru veya yayın yapılmadı.

| Tarih | Operasyon işi | Emir katkısı |
|---|---|---|
| 15–24 Ekim | Steamworks hesap/sahiplik durumunu kontrol; yoksa onboarding, banka/vergi/kimlik sürecini başlat; yayıncı adı ve AppID | Oyunun teknik/platform bilgileri |
| 26 Ekim–7 Kasım | Açıklama, etiket, dil, destek adresi, içerik anketi, lisans listesi | Gerçek özellik ve gereksinim kontrolü |
| 9–19 Kasım | Kapsül, gerçek ekran görüntüsü, trailer; mağaza incelemeye | Yeni modellerden çekim ve build |
| 3 Aralık | Onaylı Coming Soon sayfasını yayımla; bağlantıları test et | Yayın sonrası kontrol |
| 4 Aralık sonrası | Haftada iki kısa içerik; ayda bir Steam gelişme yazısı; wishlist kaydı | Haftada en fazla 30 dk çekim; üretim saatine dahil |
| 7 Ocak | Demo uygulaması/depot/başlatma ayarları; incelemeye gönder | Dağıtılabilir demo build |
| 4 Şubat | Onaylı demoyu elle yayımla, ana sayfa bağlantısını kontrol et | Temiz indirme ve açılış testi |
| 16 Nisan | Haziran Next Fest kaydını tamamla, onayı kaydet | Demo durumu ve kapsam teyidi |
| 24 Nisan | Trailer/mağaza son güncellemesi | Çalışan sürümden gerçek görüntü |
| 10 Mayıs | Festival demo onay durumunu kontrol; gerekiyorsa yeniden inceleme işlerini tamamla | Güncel demo build/test |
| 27 Mayıs | Festival güncellemesi, basın paketi | Sürüm etiketi, geri dönüş build'i |
| 3 Haziran | Basın önizlemesi; içerik üretici ve basın iletişimi | Teknik sorular ve hata desteği |
| 14–21 Haziran | Festival desteği ve sonuç takibi | Hata önceliği; yeni sistem yok |
| 29 Temmuz / 19 Ağustos | Kapalı/geniş Steam Playtest davetleri ve geri bildirim | Beta build'leri |
| 2 Eylül | Mağaza özellik/fiyat/gereksinim kontrolü; final build incelemesi | Tam oyun yayın adayı |
| 23 Eylül | Onaylar tamam ise manuel 1.0 yayını, duyuru ve destek | İndirme, save, ilk bölüm doğrulaması |

Steam'in resmî Haziran takviminde kayıt sonu **25 Nisan 2027 23.59 PDT**, Türkiye'de **26 Nisan 09.59**; basın önizlemesi için incelemeye gönderme **17 Mayıs**, genel incelemeye gönderme **31 Mayıs**, önizleme **3 Haziran 20.00**, festival **14–21 Haziran 20.00**. Ekip içi tarihler bilerek daha erkendir. Resmî tarihlerin saat belirtilmeyenlerine saat uydurulmadı. [Haziran 2027 resmî sayfası](https://partner.steamgames.com/doc/marketing/upcoming_events/nextfest/june_2027).

Mağaza ve build ayrı incelemelerden geçer. Coming Soon yayını öncesinde en az bir haftalık inceleme payı ayırın; bu planda iki hafta var. Ürün çıkışından önce Coming Soon sayfasının en az iki hafta açık olması gerekir. Onaylanan ürün kendiliğinden yayımlanmaz. [Steam yayın süreci](https://partner.steamgames.com/doc/store/releasing).

Onboarding için kimlik/banka/vergi doğrulama ve ürün başına **100 USD veya karşılığı Steam Direct ücreti** planlanmalı; freelancer ve diğer giderlerden ayrıdır. Hesaptaki bekleme koşulları yayın sorumlusu tarafından teyit edilecek; bu plan hesap açılışıyla tam çıkış arasında aylar bırakıyor. [Steam onboarding](https://partner.steamgames.com/doc/gettingstarted/onboarding).

Festival haftası ekip yine pazar izinlidir. 14 ve 17 Haziran 20.00'de operasyon sorumlusu kısa yayın/destek oturumu düzenleyebilir; zorunlu değil. Operasyonun altı saatlik sınırı için diğer sosyal paylaşımlar yerine bu oturumlar konur. 20 Haziran pazar kontrol görevi yok; 21 Haziran kapanış raporu pazartesi alınır.

## 8. Hafta hafta teslimler

E = Emir, M = modelci, O = operasyon. Haftalık E işi en fazla 18, M işi 13,5 planlı üretim saatine bölünür. Tablodaki işler günlük takvimde taslak → uygulama → entegrasyon → test → kabul olarak açılır. Çok parçalı hedefler bu bütçeyi aşıyorsa yeni görev eklemek yerine önceki iş tamamlanır. 2D kabul ve revizyon takibi O sütununun parçasıdır.

| Hafta | Tarih | E — yazılım / efekt / animasyon | M — 3D teslimi | O — Steam / QA / dış kaynak |
|---|---|---|---|---|
| H01 | 19–25 Ekim 2026 | Dört kule import, prefab ve saldırı noktaları; çalışır Windows baz build | Kule pivot/UV düzeltmeleri, ortak modüler ölçek | 2D pilot kabulü, Steam onboarding, görev panosu |
| H02 | 26 Ekim–1 Kasım | Kırılgan UI referanslarını bağla; dört kule regresyonu | Orman yol/zemin/kale kitinin ilk parçaları | 2D sözleşme; açıklama/etiket taslağı; gerçek saat raporu |
| H03 | 2–8 Kasım | Kule atış/geri tepme animasyonları; asker rig denemesi | Asker ve ilk kara düşmanı mesh/UV | Kapsül/logo ara teslimi, ses/lisans envanteri |
| H04 | 9–15 Kasım | Oyun HUD ve çanta UI entegrasyonu ilk parti | Hızlı/dayanıklı düşman varyantları | 2D mağaza paketi kabulü; görüntü/fragman hazırlığı |
| H05 | 16–22 Kasım | Mağaza/tooltip/element UI; kayıt ve trailer çekimi | Hava düşmanı ve rig'e uygun teslim | 19 Kasım mağaza incelemesi; ana UI kabulü |
| H06 | 23–29 Kasım | Öğretici ilk kule/eşya/rotasyon adımları | Demo harita 1 çevre yerleşimi | 2D eşya parti 1; mağaza inceleme düzeltmeleri |
| H07 | 30 Kasım–6 Aralık | Öğretici sinerji/element; demo harita 1 oynanış | Demo harita 2 yol ve çevre yerleşimi | 3 Aralık Coming Soon; testçi listesi ve ölçüm tablosu |
| H08 | 7–13 Aralık | İlerleme kayıt şeması ve bölüm açma | Demo boss mesh/UV | 2D eşya parti 2; mağaza ilk hafta raporu |
| H09 | 14–20 Aralık | Kayıt yedek/bozuk dosya; demo ayrımı; TR/EN altyapısı | Dört kule element eklentileri ilk parti | 2D eşya son parti; 240 ID eşleştirme kontrolü |
| H10 | 21–27 Aralık | Demo düşman/asker animasyonları ilk parti | Element eklentileri ikinci parti, materyal düzeltmeleri | Ses/müzik lisansları, menü/öğretici 2D ara teslimi |
| H11 | 28 Aralık–3 Ocak | Demo VFX/SFX ve animasyon bağlantıları; aksama payını koru | Demo modellerinde kamera mesafesi okunabilirliği | Alpha davet listesi ve yılbaşı uygunluk kontrolü |
| H12 | 4–10 Ocak 2027 | Demo build, TR/EN ve performans ilk kontrolü | Demo son sanat düzeltmeleri | 7 Ocak demo incelemesi; 9 Ocak 2D demo kabulü |
| H13 | 11–17 Ocak | 14 Ocak kapalı alpha; ilk beş dakika düzeltmeleri | Alpha görsel geri bildirimleri | 10–15 oyuncu testi, takılma noktaları raporu |
| H14 | 18–24 Ocak | İki demo bölümünde ekonomi/boss dengesi | Görsel engeller ve okunabilirlik revizyonu | İnceleme durumu, dış oyuncu ikinci tur |
| H15 | 25–31 Ocak | Demo yayın adayı; temiz PC, save, restart regresyonu | Final demo asset kontrolü | Yayın metni, destek adresi ve hazır olma kararı |
| H16 | 1–7 Şubat | 4 Şubat demo yayını; kritik hata desteği | Demo hata kaynaklı model düzeltmeleri | Demoyu yayımla, geri bildirim ve wishlist takibi |
| H17 | 8–14 Şubat | Demo geri bildirimine göre en büyük üç sorun | Tema 1 kalan modüler çevre parçaları | Oyuncu oturumu ve sorun sınıflandırması |
| H18 | 15–21 Şubat | Stat kaynak/modifier yapısı; mevcut değerleri koruyan test | Tema 1 harita 3–4 sanat yerleşimi | Meta sistemler 2D brief ve teklif |
| H19 | 22–28 Şubat | Hasar/isabet/ölüm olayları ve kule ölçüm kaydı | Tema 1 harita 5, boss varyantı | Şubat festivaline katılım yok; benzer demo deneyimlerini incele |
| H20 | 1–7 Mart | HQ satın alma, bonus kapsamı ve kayıt | Tema 2 ana çevre kitinin modellemesi | HQ/skilltree taslak UI geri bildirimi |
| H21 | 8–14 Mart | Skilltree düğüm açma, maliyet/reset/kayıt | Tema 2 materyal/UV ve yol parçaları | Yetenek metinlerinin TR/EN kontrolü |
| H22 | 15–21 Mart | Okçu temel pasifler; hasar/menzil/kritik testleri | Tema 2 düşman ailesi | Skill ikonları ara teslim; haftalık QA |
| H23 | 22–28 Mart | Büyücü temel pasifler ve RNG ağırlıkları | Tema 2 boss modeli | 27 Mart meta UI teslimi; kaynak dosya kabulü |
| H24 | 29 Mart–4 Nisan | Birlik temel pasifleri, savunma ve üretim süresi | Tema 2 harita 6–7 sanat yerleşimi | Meta sistem kapalı test listesi |
| H25 | 5–11 Nisan | Özel yetenekler: isabet sayacı/stun/dalga bonusu | Tema 2 harita 8–9 sanat yerleşimi | Festival kaydı ön kontrolü; mağaza metin yenileme |
| H26 | 12–18 Nisan | Çifte üretim/sinerji; yetenek seti entegrasyon testi | Tema 2 harita 10 ve kit düzeltmeleri | 16 Nisan Next Fest kayıt ve onay kontrolü |
| H27 | 19–25 Nisan | Bestiary karşılaşma/öğrenme/ödül ve kayıt | Tema 3 çevre kiti ilk parti | 24 Nisan trailer/kapsül güncellemesi |
| H28 | 26 Nisan–2 Mayıs | Hasar/DPS analiz ekranı, kayıt geçişleri | Tema 3 kit UV/materyal ikinci parti | Festival sayfası ve basın paketi taslağı |
| H29 | 3–9 Mayıs | Festival demo öğretici/denge; 6 Mayıs kapsam kilidi | Festival demo görsel cila | Basın/testçi listesi, trailer izinleri kontrolü |
| H30 | 10–16 Mayıs | Festival demo build ve profil; onay gereksinimleri | Demo performans için mesh/materyal sadeleştirme | 10 Mayıs onay/inceleme kontrolü; basın hazırlığı |
| H31 | 17–23 Mayıs | Festival demo regresyon ve kritik hata kapama | Tema 3 düşman/boss mesh teslimi | 17 Mayıs resmî önizleme inceleme son gününü teyit |
| H32 | 24–30 Mayıs | 27 Mayıs festival demo güncellemesi ve temiz indirme | Tema 3 harita 11–12 sanat yerleşimi | Demo duyurusu, basın paketi ve yayın provası |
| H33 | 31 Mayıs–6 Haziran | Festival build son kontrol; kritik hatalar | Tema 3 harita 13–14 sanat yerleşimi | 31 Mayıs inceleme / 3 Haziran basın önizlemesi |
| H34 | 7–13 Haziran | Demo sürümünü sabitle; destek ve geri dönüş provası | Tema 3 harita 15 son teslim | Next Fest sayfa/dil/link/duyuru son kontrolü |
| H35 | 14–20 Haziran | Next Fest hata desteği; yeni özellik yok | Bildirilen görsel hatalara destek | Festival topluluk/testçi iletişimi; pazar izin |
| H36 | 21–27 Haziran | Festival sonuçları; kampanya 1–10 dalga/harita entegrasyonu | Tema 4 kit ilk parti | 21 Haziran kapanış; en büyük beş sorunun raporu |
| H37 | 28 Haziran–4 Temmuz | Kampanya 11–15; boss davranışları/animasyon bağlantıları | Tema 4 kit ikinci parti, düşman varyantı | Beta oyuncu havuzu; kalan 2D revizyon siparişi |
| H38 | 5–11 Temmuz | Kampanya 16–18 blok düzen ve boss entegrasyonu | Tema 4 harita 16–18 sanat yerleşimi | Kampanya test senaryoları; kayıt test matrisi |
| H39 | 12–18 Temmuz | Kampanya 19–20; içerik kilidi, bütün bölümlere erişim | Tema 4 harita 19–20, son boss ve kademe eklentileri | 17 Temmuz içerik kabulü; açık eksik listesi |
| H40 | 19–25 Temmuz | Tam kampanya smoke test; beta kayıt ve animasyon sorunları | Eksik mesh/texture ve LOD düzeltmeleri | Beta daveti, bug formu ve Playtest dağıtımı |
| H41 | 26 Temmuz–1 Ağustos | 29 Temmuz kapalı beta; kritik hata çözümü | Beta görsel düzeltmeleri | 30–50 oyuncu testi, oturum/ilerleme raporu |
| H42 | 2–8 Ağustos | En zor bölüm/ekonomi dengesi; kayıt regresyonu | Kamera/okunabilirlik/harita sınırı düzeltmeleri | Kullanıcı testleri ve hata tekrar üretimi |
| H43 | 9–15 Ağustos | Normal mermi/düşman profil ve gerekli havuzlama | Mesh/materyal maliyeti ve sanat kapanışı | 14 Ağustos son 2D kabulü; gereksinim ölçümleri |
| H44 | 16–22 Ağustos | 19 Ağustos geniş beta; başarım/Cloud entegrasyon kontrolleri | Sadece beta kaynaklı görsel hatalar | Daha geniş test, ses/lisans ve metin kontrolü |
| H45 | 23–29 Ağustos | Sürüm geçişi, temiz PC ve uzun koşu regresyonu | Son optimizasyon/LOD düzeltmeleri | Fiyat önerisi, destek dokümanı, mağaza vaat denetimi |
| H46 | 30 Ağustos–5 Eylül | 2 Eylül son yayın adayı ve Steam build incelemesi | Yayın adayı sanat hataları | İnceleme takibi; basın kiti ve son trailer |
| H47 | 6–12 Eylül | P0/P1 kapama; 11 Eylül yayın/erteleme kararı | Son kabul ve kaynak arşivi | Çıkış tarihini kesinleştirme, iletişim planı |
| H48 | 13–19 Eylül | Onay düzeltmesi ve sürüm dondurma; geri dönüş provası | Yalnız yayın engelleyen düzeltmeler | Çıkış duyuruları ve destek hazırlığı |
| H49 | 20–26 Eylül | 23 Eylül 1.0; ilk gün indirme/save/oynanış kontrolü | Çıkışta görsel hata desteği | Manuel yayın, topluluk ve günlük sorun özeti |

Harita işleri tek haftada sıfırdan tasarım+sanat+kod anlamına gelmez: tema kitleri önceden hazırlanır; modelci harita kompozisyonunu üretirken Emir haftalık entegrasyon saatinde yol/slot prototipini kontrol eder. H36–39 final dalga ve bölüm bağlantılarının kabul haftalarıdır. Aynı şekilde animasyonlar demo ve her yeni düşman tesliminde hazırlanır; Temmuzda topluca sıfırdan başlanmaz.

## 9. Çıkış kapıları ve erteleme kuralları

| Kapı | Bitti sayılma koşulu | Başarısızsa |
|---|---|---|
| 31 Ekim kapasite | İki haftanın gerçek E/M saatleri ve model entegrasyon süreleri ölçüldü | Tahminleri yenile; kapsamı ilan etmeden düzelt |
| 21 Kasım sanat | Ana UI kullanılabilir, demo model/çevre hattı akıyor | Mağaza 3 Aralık riske girer; yeni tarih ver |
| 9 Ocak demo | İki bölüm baştan sona; bütün zorunlu UI/asset; save/TR/EN; dış test build'i | 4 Şubat yerine **4 Mart** iç alternatif; Haziran hâlâ hedef |
| 30 Ocak yayın | 3 ayrı PC'de temiz açılış; 20 tam demo koşusunda P0/P1 yok; en az 10 yeni oyuncunun 8'i dış yardım olmadan ilk döngüyü tamamlıyor | Demo ertelenir; gereksiz özellik kesilir |
| 10 Nisan festival | Demo erişilebilir, çekirdek geri bildirim çözüldü, mağaza doğru, kayıt yapılabilir | Haziran katılımını taahhüt etme |
| 22 Mayıs festival | Onay durumu uygun; crash/softlock/save kaybı yok; ilk deneyim anlaşılır | Festivalden çekilme kararı; doğrulanmamış Ekim tarihini uydurma |
| 17 Temmuz içerik | 20 bölüm, 18 yetenek, HQ/bestiary/analiz mevcut; tüm içerik erişilebilir | Beta ve 1.0 kayar; beta etiketiyle eksik sistemi gizleme |
| 11 Eylül 1.0 | Son 2 hafta P0/P1 yok; 3 PC'de tam kampanya; save/upgrade; Steam onayı; mağaza vaatleri doğru | **21 Ekim 2027** ilk yedek hedef; kapsamlı sapmada Kasım–Aralık yeniden plan |

P0: açılmama, crash, kayıt kaybı. P1: bölüm ilerleyememe, çoğaltma/ekonomi kırılması, temel etkileşimin bozulması. P2/P3: ilerlemeyi engellemeyen görsel/metin ve küçük sorunlar; açık listeyle sınırlı kabul edilebilir.

Performans hedefi: seçilen referans PC'de 1080p hedef 60 FPS; önce donanım kaydı ve ölçüm yapılır, ölçülmeden Steam minimum gereksinimi ilan edilmez. Yoğun dalga/3×/birlik kalabalığı, art arda 10 restart, ayar değişimi, pencere odağı, farklı çözünürlük, bozuk/eski kayıt ve temiz kurulum test matrisine dahildir. Sadece toplam FPS değil uzun kareler ve bellek artışı da izlenir.

Wishlist hedefi yayın izninin otomatik şartı değil. Operasyon her hafta mağaza ziyaretleri, wishlist net artışı, demo başlangıcı/tamamlama gözlemleri ve en sık üç sorun kaydeder. Başlangıç kitlesi bilinmediği için satış veya wishlist sayısı garanti edilmedi. Oyuncu ölçümü başlangıçta oturum gözlemi/formla yapılabilir; ayrı bir canlı servis kurmak zorunlu değil.

## 10. İlk beş gün ve yönetim kuralları

| Tarih | Emir | Modelci | Operasyon |
|---|---|---|---|
| 14 Ekim Çarşamba | Mevcut build/eksik listesi, planı ekiple paylaş | Dört kulenin teslim durumunu bildir | Görev panosu; ekip takvimi ve freelancer listesi |
| 15 Ekim Perşembe | Bir örnek kule için pivot/ölçek/muzzle sözleşmesi | Örnek kule export ve son üç kulenin üretimi | Steam hesap durumunu kontrol; 2D brief ile teklif iste |
| 16 Ekim Cuma | Örnek kule import denemesi; 20.00'de 15 dk sync | UV/materyal/source paketini hazırla | Teklif kapsamlarını eşleştir; bütçe kararını ekibe getir |
| 17 Ekim Cumartesi | Dört modeli kabul kontrolüne al; entegrasyon hatalarını yaz | Dört nötr L1 kuleyi teslim et | 20.00–21.00 ilk toplantı; rol ve pilot teslimini kaydet |
| 18 Ekim Pazar | İzin | İzin | İzin |

Her görev kartında tek sorumlu, en fazla bir günlük alt iş, bağımlılık, kabul ölçütü ve build/ekran görüntüsü kanıtı olsun. İş akışı: Hazır → Yapılıyor → Kontrol → Bitti. Emir ve modelci aynı anda en fazla iki aktif iş taşısın. Yeni fikirler ayrı listeye alınır; cumartesi maliyeti kadar başka iş çıkarılmadan bu plana eklenmez.

Kritik yol: dört kule teslimi → Unity entegrasyonu → freelancer ana UI/ikonları → iki haritalı öğretici demo → dış test → Steam onayı → demo → festival. Tam sürümde buna stat/kayıt → HQ/skilltree/bestiary → dört tema/20 harita → beta → son inceleme eklenir. Operasyon arkadaşı üreticilerin yerine kod veya 3D yapmaz; onların zamanını koruyan test ve iletişim organizasyonunu üstlenir.

## 11. Kaynaklar ve sonraki güncelleme

- Yerel teknik kaynaklar: `Assets/Scripts`, `Assets/Resources/Data/Items`, `Assets/Resources/Data/Chapters`, `Packages/manifest.json`, `ProjectSettings/EditorBuildSettings.asset`.
- [Güncel teknik yol haritası](Backpack_Legions_Yol_Haritasi.md), [2D freelancer brief](2D_Artist_Freelance_Brief_2026-10-14.md), [mevcut doğrulama aracı](../Tools/Validation/README.md), [yerel Steam taslağı](../Marketing/Steam/2026-10-09/README.md).
- Resmî kaynaklar 14 Ekim 2026'da kontrol edildi: [etkinlik listesi](https://partner.steamgames.com/doc/marketing/upcoming_events), [Next Fest genel](https://partner.steamgames.com/doc/marketing/upcoming_events/nextfest), [Haziran takvimi](https://partner.steamgames.com/doc/marketing/upcoming_events/nextfest/june_2027), [yayın süreci](https://partner.steamgames.com/doc/store/releasing), [onboarding](https://partner.steamgames.com/doc/gettingstarted/onboarding).
- İlk yeniden tahmin: **31 Ekim 2026**, sonra her ayın son cumartesisi. Yeni kesin kapsam kararları ve freelancer teklifi bu belgeye işlenmeli. Günlük takvim uzak aylar için başlangıç taslağıdır; her cumartesi sonraki haftanın alt işleri gerçek duruma göre netleştirilir.
