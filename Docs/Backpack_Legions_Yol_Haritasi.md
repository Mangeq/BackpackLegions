# Backpack Legions — Güncel yol haritası

Son inceleme: **20 Ekim 2026**. Projenin güncel yapılacaklar listesi bu belgedir.

## İnceleme kapsamı

Kaynak kod, Resources/Data assetleri, sahnelerin script bağlantıları, Build Settings, önceki teknik açıklamalar ve test kayıtları yeniden incelendi. Bu tur oyun kodu, sahne, prefab, denge veya kayıt değiştirilmedi. **Unity Play Mode, Console ve Windows build testi bu tur çalıştırılmadı.** Aşağıdaki “uygulandı” işaretleri mevcut kod/veriyi anlatır; yeni sürümün hatasız olduğuna dair garanti değildir. Önceki testler kendi tarihleriyle geçerlidir.

Sayısal döküm: [20 Ekim statik inceleme kaydı](Validation/roadmap-audit-2026-10-20.json). Eski tarihsel yol haritası [arşive alındı](Archive/Backpack_Legions_Yol_Haritasi_2026-10-20_Oncesi.md). Böylece eski fiyatlar ve tamamlanmış işler güncel yapılacaklarla karışmıyor.

## Kısa durum

Oyun; hazırlık, kule/eşya seçimi, çanta düzenleme, savaş, boss, sonuç ve sonraki bölüme geçiş döngüsüne sahip. Dört kule türü, beş bölüm verisi, elementli düşmanlar, evrim, konumsal/takılabilir sinerji ve kalıcı yetenek puanı uygulanmış durumda.

**Henüz tamamlanmış ürün değil:** beş bölüm aynı savaş sahnesini kullanıyor; büyücü/havan/birlik skilltree içerikleri taslak; son depo/element/rezervasyon değişiklikleri eski testlerle doğrulanmış sayılmaz. İlk öncelik mevcut kuralları tutarlı hale getirip güncel akışı test etmek, sonra kalan içeriklere geçmek.

## 1. Yapılmış işler

### Temel oyun ve savaş

- [x] GameManager üzerinde hazırlık → savaş → hazırlık → zafer/yenilgi akışı.
- [x] Manuel başlatma ve hazırlık sayacı bitince otomatik başlatma; son düşmanı bekleme.
- [x] Pause/devam, yeniden başlatma, ana menü, 1×/2×/3× hız.
- [x] Hazırlık/savaş/pause durumuna göre eşya ve yeni kule işlemlerini kilitleme.
- [x] Kale canı, kaçan düşmanın kale hasarı, düşman öldürme ödülü.
- [x] Düşmanın bağımsız saldırı ritmi; birliklerle çatışma, birlik ölünce yoluna devam etme.
- [x] Yolu izleyen hava düşmanları; hava düşmanının birlikleri pas geçmesi.
- [x] Element üstünlük/direnç hesabı; gerçek hasar katkısına göre kule XP paylaşımı.

### Dört kule türü

- [x] Okçu, büyücü, havan ve birlik kulesi satın alma ve stat hesabı.
- [x] Boş yer → kule ikonları → bilgi/satın alma; yetersiz altın ve iptal edilen yerin tekrar kullanılabilmesi.
- [x] Okçu/büyücü için Kara/Hava/İkisi; havan için kara hedefleri.
- [x] Menzile ilk giren/son giren düşman hedeflemesi; birlik kulesi bu seçimi kullanmaz.
- [x] Havan alan hasarı, kavisli mermi ve kule başına mermi/efekt yeniden kullanımı.
- [x] Kuleye özel XP/seviye; temel statların TowerData'dan hesaplanması.
- [x] Kule türü değişiminde satış gelirini gösteren onay, otomatik satış, depo/teslim aktarımı; yeni kulede seviye/XP/element sıfırlama.

Güncel temel değerler (eşya, seviye ve skilltree öncesi):

| Kule | Fiyat | Hasar | Atış/saldırı aralığı | TowerData menzili |
|---|---:|---:|---:|---:|
| Okçu | 250 | 13 | 1,1 sn | 4,7 |
| Büyücü | 325 | 29 | 1,9 sn | 3,4 |
| Havan | 375 | 46 | 3,4 sn | 4,8 |
| Birlik | 225 | 4 | 1,35 sn | 3 |

Birlik kulesinin menzil alanı okçu gibi hedefleme küresi anlamına gelmez; asker angajman davranışı ayrıca işler.

### Çanta, depo ve mağaza

- [x] Kule başına bağımsız şekilli çanta; varsayılan 5×5 alan.
- [x] Sürükleyerek alma/taşıma, yerleşim önizlemesi, geçersiz bırakmada geri dönüş.
- [x] R ve fare tekerleğiyle 90° döndürme; eşya kopyasına özel yön.
- [x] Kuleye özel depo; güncel kodda **sınırsız liste**, ekranda iki sütun ve altı satır, fazlasında kaydırma. 12 görünen yer kapasite sınırı değildir.
- [x] Çantadaki eşyaları topluca depoya taşıma düğmesi. Ana eşya istisnası eksik: öncelikli açık iş aşağıda.
- [x] Depodaki eşyaların stat hesabına katılmaması.
- [x] Dört teklif, ücretli yenileme, satın alınan slotun boşalması, kuleye özel tekliflerin korunması.
- [x] **Teklif başına sağ tıkla rezervasyon**; yenilemede ayrılan slotların korunması. Eski tüm-dükkan Freeze metodu artık etkisiz.
- [x] Kule türü, element, nadirlik ve seviyeye göre eşya havuzu.
- [x] Normal eşya satışında maliyetin %50'si, aşağı yuvarlama; ana eşyanın satışı engelli.
- [ ] Seviyeye göre teklif sayısının artması: mevcut dizi dört elemanlı; bu özellik henüz uygulanmış değil.

### Element, evrim ve sinerji

- [x] Su → Ateş → Hava → Toprak → Su döngüsü; düşman verisinde ayrı çarpanlar.
- [x] Güncel element yolu: 5. seviyede element mağazası; **80 altınlık katalizörü ana silaha komşu yerleştirince anlık dönüşüm**. Katalizör tüketilir; bu işlem normal bir-dalgalık evrimden ayrıdır.
- [x] Element alışverişinden sonra saklanan normal tekliflerin/rezervasyonların geri yüklenmesi.
- [x] Eski normal eşyalar korunur ve tür uyumluysa çalışır; başka elemente özel uyumsuz eşya pasif kalır.
- [x] ItemEvolutionRecipe ile yan yana iki malzemenin başarılı dalga sonunda birleşmesi.
- [x] Ürün çantaya sığmazsa kendi deposuna gider; teslim kuyruğu kodu korunuyor. Sınırsız depoda normal “depo doldu” senaryosu artık beklenmez.
- [x] ItemSynergyRecipe ile belirli karede veya üzerine takılı sağlayıcıdan sürekli bonus; birleşme yok.
- [x] Takılı parçayı küçük ikonundan çıkarma; hedefle taşıma; satış/evrimde parçaları koruma.
- [x] Lejyoner Kılıcı + Lejyon Tacı için +5 sabit hasar örneği.

**Güncel olmayan eski kural:** “İlk element ücretsiz seçim düğmesi; sonraki değişim 100 altın” artık aktif akış değildir. TowerData'da 100 alanı dursa da TrySelectElement/TryChangeElement doğrudan dönüşüm yapmıyor. Yeni katalizör yolunun uçtan uca testi açık.

### Menüler ve bilgi ekranları

- [x] Boot yükleme → ana menü → bölüm haritası → oyun akışı; asenkron sahne yükleme ve hata durumunda tekrar deneme.
- [x] Zafer/yenilgi ekranları; yeniden deneme, sonraki bölüm ve skilltree erişimi.
- [x] Kule/çanta/depo/mağaza birleşik ekranı ve kule statları.
- [x] Eşya, kule, birlik ve düşman bilgi panelleri; birlik/düşman can barları.
- [x] Sonraki dalga düşmanları, adet ve element bilgisi için hover paneli.
- [x] ESC ve panel dışına tıklama ile kapatma kodu; sürükleme iptaliyle birlikte yönetim.
- [x] Ana menü, bölüm, ayarlar, HUD, yükleme, sonuç ve skilltree için referans görsel uyarlama sınıfları ve sahne/prefab içeriği.
- [x] Ses/efekt, kalite, çözünürlük, tam ekran, VSync ve FPS ayarları; PlayerPrefs ile saklama.
- [ ] Yeni görsel katmanların farklı çözünürlüklerde tıklama alanı, metin taşması, okunurluk ve ölçek testleri.

### Skilltree ve kalıcılık

- [x] Kule türü başına ortak ağaç; dört ayrı ağaç verisi.
- [x] SkillData: tür, etkiler, açıklama, ikon, puan maliyeti ve ön koşullar.
- [x] SkillTreeData: düğüm konumları/bağlantıları; UI Inspector üzerinden data bağlama.
- [x] Can oranına göre zafer ödülü: %90+ 3 puan, %50–89 2, pozitif %50 altı 1; yenilgi 0.
- [x] Aynı bölümde yalnız en iyi sonucun artış farkı; tekrar oynayarak sınırsız puan birikimi engelli.
- [x] Açılan yetenekler, puan, bölüm rekorları ve harcama makbuzları kalıcı kayıtlı; ücretsiz ağaç sıfırlama/iade.
- [x] **11 okçu yeteneği etkin.** Genel pasifler ve okçuya özel isabet/mesafe/can/kritik koşulları.
- [ ] **Büyücü 12, havan 12, birlik 12 düğüm taslak:** available=false. Data dosyası veya çizilmiş düğüm, uygulanmış yetenek sayılmaz.
- [ ] Kampanya kilit açma kuralı, yarım kalan bölüme devam ve kapsamlı sürüm/bozuk kayıt kurtarma sistemi.

## 2. İçerik sayıları ve bölüm durumu

| Veri grubu | Asset sayısı |
|---|---:|
| Mağaza eşyaları | 200 |
| Ana eşyalar | 20 |
| Evrim sonuçları | 20 |
| Element katalizörleri | 4 |
| Evrim tarifleri | 21 |
| Sürekli sinerji tarifleri | 22 |
| SkillData | 47 (11 etkin, 36 taslak) |
| SkillTreeData | 4 |
| Kampanya düşman verileri | 30 |
| ChapterData | 5 |

Bunlar dosya sayısıdır: 200 bağımsız mekanik, 30 benzersiz model veya her tarifin sahnede aktif olduğu anlamına gelmez. Element varyantları sayılara dahildir.

| Bölüm | Dalga | Toplam düşman | Başlangıç altını | İlk hazırlık | Boss canı |
|---|---:|---:|---:|---:|---:|
| 1 | 5 | 180 | 850 | 60 sn | 2400 |
| 2 | 7 | 260 | 1000 | 75 sn | 3840 |
| 3 | 9 | 340 | 1150 | 90 sn | 5280 |
| 4 | 11 | 420 | 1300 | 105 sn | 6720 |
| 5 | 13 | **532** | 1450 | 120 sn | 8160 |

Her sonraki dalgada hazırlığa +5 sn ekleniyor. Son dalgalar boss ve destekler içeriyor. Kampanya boss verilerinde avantaj ×2, direnç ×0,4, Normal saldırı ×0,6 ve kale hasarı 100. Bütün bölümler `GameScane` kullanıyor. Build Settings'te Boot, MainMenu ve GameScane etkin. Blackpine_Demo ve Blackpine_V5_Test dosyaları var; ayrı oynanabilir kampanya haritası olarak bağlı değiller.

5. bölüm toplamı eski belgedeki 500'den farklı: **532**. Böylece 180/260/340/420/+80 dizisi son bölümde korunmuyor. Sayı bu revizyonda değiştirilmedi; doğrusal hedefin korunup korunmayacağı denge kontrolüne alındı.

## 3. Öncelikli tutarsızlıklar ve doğrulama işleri

| Öncelik | Bulgu | Etki ve yapılacak iş |
|---|---|---|
| P0 | Ana eşya depoya taşınabiliyor | BackpackDraggable.EvaluatePlacement içinde eski yasak yok; MoveAllToStorage bütün kök eşyaları taşıyor. Önceki açık kararla çelişiyor. İstenen kuralı koruyacak düzeltme ve tekli/toplu taşıma testi ilk iş olmalı. Bu tur kod değiştirilmedi. |
| P0 | Ana eşya yokken durum tutarsızlığı | CalculateStats ana eşya bulamazsa Normal elemente dönüyor; temel hasar hesabı devam ediyor. Tür değişimi ise ana eşya ister. Ana eşyanın depoya kaçması bu yolları etkiliyor; birlikte test edilmeli. |
| P0 | Eski test varsayımları artık yanlış | 15 Ekim testi sınırlı depoyu dolduruyor ve TrySelectElement çağırıyor. Yeni sınırsız depo/katalizör akışını test etmiş sayılamaz. Testler yeni kurallara uyarlanmalı. |
| P1 | Element mağazası ve rezervasyon yeni akış | Seviye 5 geçişi, tekrar element alma, aynı element, para yetmemesi, satın alma iptali, normal tekliflere dönüş, rezervasyon ve faz değişimi test edilmeli. |
| P1 | Sınırsız depo arayüzü | 0/12/13/çok eşya, scroll ile drag, takılı parçalar, toplu taşıma ve tekrar çantaya alma test edilmeli. |
| P1 | Kayıt kurtarma eksik | SkillTreeProgress geçersiz sürümde istisna atıyor; JSON okuma için kurtarma yolu yok. Yedek/sürüm göçü ve bozuk kayıt politikasını birlikte belirlemek gerekiyor. |
| P1 | Güncel tam bölüm dengesi doğrulanmadı | Son element/rezervasyon/depo ve skilltree değişiklikleri ekonomiyi etkiliyor; önceki oyuncu koşuları bu sürümü kapsamaz. |
| P2 | Kullanılmayan/eski API ve açıklamalar | IsShopFrozen sürekli false, ToggleShopFreeze boş; eski element değiştirme metotları etkisiz; SynergyLineManager boş; StoragePileUI geçiş kabuğu. Önce referans denetimi, sonra gerekirse temizlik. Körlemesine silme yok. |

## 4. Bundan sonra ilerleme sırası

### Aşama A — Güncel mekanikleri sağlamlaştırma (sıradaki iş)

- [ ] Ana eşyanın depoya gitme tutarsızlığını düzeltmek; tekli drag ve toplu taşıma birlikte.
- [ ] Dört kulede satın alma/iptal, katalizör dönüşümü, teklif rezervasyonu, çanta/depo/satış akışını güncel testlere geçirmek.
- [ ] Evrim ve takılı sinerjiyi yeni depo/element akışıyla birlikte doğrulamak; kayıp, çoğalma, çift ücret olmamalı.
- [ ] ESC, dışarı tıklama, pause, savaş başlangıcı ve sonuç anında aktif sürüklemeyi test etmek.
- [ ] Yeni UI'yı 16:9, 16:10 ve dar/geniş pencere oranlarında kontrol etmek.
- [ ] Boot → menü → bölüm → sonuç → sonraki bölüm → menü akışını dört kuleyi içeren Windows test build'inde çalıştırmak.

Bitti ölçütü: yeni kuralları kapsayan tekrar çalıştırılabilir test kaydı; kritik eşya/altın kaybı yok; incelenen build akışında hata yok. Eski test sayıları tek başına bu aşamayı kapatmaz.

### Aşama B — Oynanış ve dengeyi ölçmek

- [ ] Beş bölümü yeni kayıt/skillsiz, kazanılmış skill puanlı ve farklı kule dağılımlarıyla baştan sona oynamak.
- [ ] Element kullanmanın getirisi, boss + destekler, reroll maliyeti, rezervasyon gücü ve sınırsız deponun stoklama etkisini ölçmek.
- [ ] 5. bölümde 532 düşmanı gözden geçirmek; doğrusal toplam isteniyorsa 500 hedefini kontrollü uygulamak.
- [ ] Hasar/öldürme/kaçan düşman/kalan can/harcanan altın ölçülerini kayıt altına almak. Mevcut XP katkısı altyapı sağlar; tam DPS raporu henüz yok.
- [ ] Kule konumu ve hedefleme tercihlerinin anlamlı fark oluşturduğunu sınamak.
- [ ] 1×/2×/3× ve düşük FPS koşullarında sonuç/performans karşılaştırması.

Bitti ölçütü: yalnız “zor/kolay” yorumu değil, hangi bölümde hangi kurulumun neden başarısız olduğuna dair ölçü ve oyuncu notu.

### Aşama C — Kalan üç skilltree

- [ ] Önce büyücü, sonra havan ve birlik için yetenekleri/etkileri/maliyetleri Emir ile belirlemek. Sıra öneridir.
- [ ] Mevcut SkillData etkilerini kullanabilen pasifleri bağlamak; yeni koşullu etki gerekiyorsa mimarisini önce konuşmak.
- [ ] Eski GDD'deki Doğa/Buz, zırh delme, stun ve katlanan element ücreti varsayımlarını güncel sistemle uzlaştırmak.
- [ ] Ek ok biriktirme yeteneğini skilltree'ye koymamak; kullanıcı kararıyla eşya sistemine ait.
- [ ] Satın alma, ön koşul, reset, kayıt geri yükleme ve mevcut/yeni kulelere etkinin yayılmasını test etmek.

Bitti ölçütü: dört ağaçta açık düğümlerin açıklaması, puan bedeli ve gerçek etkisi uyuşur; taslaklar etki üretmez.

### Aşama D — Haritalar, düşmanlar ve ilk deneyim

- [ ] Önce ikinci gerçek haritayı tamamlamak; sahne yükleme, spawn/yol, kale ve kule noktası sözleşmesini doğrulamak.
- [ ] Sonra ilk beş bölümün ayrı haritalarını üretmek; uzun hedef 20 bölüm, her beş bölümde yeni tema.
- [ ] Bossları yalnız yüksek canla değil, birlikte kararlaştırılmış davranışlarla farklılaştırmak.
- [ ] Dört kulenin son modelleri, düşman siluetleri, saldırı/ölüm animasyonları, element VFX ve seslerini entegre etmek; dosya varlığı teslim kabulü sayılmaz.
- [ ] Kısa öğretici: kule seçimi → eşya şekli/rotasyon → depo/satış → sinerji/evrim → element karşılığı.
- [ ] Bestiary: karşılaşma kaydı, Öğren görevi, öldürme ilerlemesi ve ödül kurallarını belirleyip uygulamak.

### Aşama E — Kayıt, performans ve yayın hazırlığı

- [ ] Mevcut skill/ayar kaydını koruyarak sürüm göçü, bozuk kayıt ve yedekten kurtarma.
- [ ] Bölüm kilit açma ve yarım kalan oyuna devam kapsamını belirlemek; sırf puan kaydı var diye tamamlandı saymamak.
- [ ] Gerçek build'de CPU/GC/bellek profilini almak; ardından gerekiyorsa normal mermi/düşman havuzu ve düşman sayacı. Havan havuzu zaten var.
- [ ] TR/EN metinleri ve font/uzun metin testi; mevcut kodda sabit Türkçe metinler var.
- [ ] Build regresyonu, cihaz/çözünürlük testi, sanat/ses lisans kontrolü ve dış oyuncu testleri.
- [ ] Steam/demo üretim takvimini güncel kapasite ve kabul edilmiş teslimlere göre yeniden değerlendirmek.

Üretim/yayın tarihleri ayrı [üretim planında](Uretim_Yayin_Plani_2026-10-14.md) ve [günlük takvimde](Gunluk_Uretim_Takvimi_2026-2027.md). Bu teknik inceleme o tarihleri, model teslimlerini veya dış platform takvimini yeniden doğrulamaz; tarih geçti diye görev tamamlandı sayılmaz.

## 5. Mimari ve çalışma anlaşması

- Mimari kararları Emir ile birlikte alacağız; bu revizyon yeni sınıf/manager onayı değildir.
- GameManager kalır. PlaneTest kullanıcı kararıyla korunur.
- Yeni kodda hangi sınıfın ne yaptığı, veri akışı ve Inspector kullanımı açıklanır; Emir'in kod hâkimiyeti önceliklidir.
- Sayısal denge değiştirilebilir; ama mekanik kural değişiklikleri eski açık kararlarla çelişirse görünür şekilde ele alınır.
- Kaynak ve kayıtlar korunur; test ile statik inceleme birbirinden ayrılır. Git deposu mevcut, çalışma ağacında kullanıcı değişiklikleri var; bu tur commit/push yapılmadı.

## 6. İlgili teknik belgeler

- [SkillData rehberi](SkillData_Kullanim_Rehberi_2026-10-15.md)
- [Evrim ve konum sinerjisi](Evrim_ve_Konum_Sinerjisi_2026-10-15.md) — tarif mantığı geçerli; sınırlı depo ve eski element seçimi anlatımı için bu yol haritasındaki güncel durum esas alınır.
- [Eşya verileri](Esya_Verileri_Rehberi_2026-10-10.md) — sonradan eklenen Elements ve SynergyRecipes klasörleri bu belgedeki sayımla birlikte okunmalı.
- [Bölümler ve dalga önizlemesi](Bolumler_ve_Dalga_Onizleme_2026-10-13.md) — ilk kurulum sayıları tarihsel; güncel sayılar yukarıdaki tabloda.
- [Havan](Havan_Kulesi_2026-10-09.md), [hedefleme](Backpack_Legions_Hedefleme_2026-10-05.md), [satış](Item_Satisi_2026-10-08.md).

**Sıradaki somut adım:** ana eşya/depo kuralını düzeltip yeni katalizör + rezervasyon + sınırsız depo akışının güncel regresyonunu tamamlamak. Bunun ardından kalan skilltree ve yeni harita işlerine daha güvenilir bir temel üzerinden geçebiliriz.
