# Backpack Legions — Güncel yol haritası

Son genel inceleme: **5 Kasım 2026**. Güncel teknik yapılacaklar listesi bu belgedir. Önceki sürüm [arşivlendi](Archive/Backpack_Legions_Yol_Haritasi_2026-11-05_Oncesi.md).

## İncelemenin kapsamı

Kaynak kod, güncel çalışma ağacı, prefab/data dosyaları, önceki test raporları ve bağlı Unity 2022.3.62f3 editörü incelendi. Canlı sorgular salt okunurdu: Play kapalı, derleme sürmüyor, açık GameScane sahnesi **dirty=true**. Kaydedilmemiş sahne ve kullanıcı değişiklikleri korundu; sahne kaydedilmedi/değiştirilmedi, Play veya build başlatılmadı. İlk sorguda dirty=true, son sorguda dirty=false görüldü; bu tur sahne kaydetme çağrısı yapılmadı, bayrak değişiminin nedeni doğrulanmadı. Console sorgusu hata/uyarı döndürmedi; açık sahnenin hiyerarşi taramasında eksik script sayısı 0. Bu, bütün projede hata olmadığı anlamına gelmez.

Kanıtlar: [statik veri dökümü](Validation/roadmap-audit-2026-11-05.json), [canlı referans incelemesi](Validation/roadmap-live-2026-11-05.json). Statik döküm Resources/Data'yı sayar; sahnenin kullandığı NewMap kopyaları ayrıca canlı referanslarla kontrol edildi. Başlık altında [x] mevcut uygulamayı, [ ] kalan işi gösterir; çalışma zamanı kabulü ayrıca belirtilir.

## Genel durum

Oynanabilir ana döngü, dört kule, beş bölüm içeriği, eşya ekonomisi, elementler, iki sinerji türü, menüler ve kalıcı skilltree altyapısı var. Son dönemde altınla kule yükseltme, modeller, görsel savaş geri bildirimi ve sesler eklendi. En büyük eksikler: güncel sahne/harita bağlantılarının tamamlanması, değişen kuralların regresyonu, yeni ekonominin dengesi ve üç kule ağacının gerçek etkileri.

Tamamlanma yüzdesi vermiyoruz: asset sayısı veya kodun bulunması, dengeli ve test edilmiş bir ürün anlamına gelmez.

## 1. Eski yol haritasından değişen temel kurallar

| Konu | 5 Kasım'daki gerçek durum |
|---|---|
| Kule XP | Kaldırılmış. Enemy öldüğünde altın verir; XP dağıtımı yok. Tower.TryUpgrade ile altın harcanarak seviye artar. |
| Yükseltme fiyatı | Güncel dört kulede 100 + (mevcut seviye − 1) × 50. Seviye 1→5 toplam 700 altın; katalizör ayrıca ödenir. |
| Savaşta kule alımı | CanBuyTowers ve CanLevelUpTowers savaşta da açık; pause/sonuçta kapalı. Eski “yalnız hazırlıkta kule al” kararıyla farklılık açıkça not edildi; bu tur değiştirilmedi. |
| Eşya işlemleri | Hazırlıkta, pause dışında ve itemPlacementEnabled=true iken. Açık sahnede bu seçenek true. Tür/element değiştirme hazırlıkla sınırlı. |
| Saldırı hızı | TowerData.attackSpeed artık **saniyedeki saldırı sayısı**. Büyük sayı hızlıdır; eski saniye/cooldown anlatımı geçersiz. |
| Element | 5. seviyede katalizör mağazası. Ana silaha komşu katalizör anında tüketilip silahı dönüştürür; dalga sonu evriminden ayrıdır. |
| Yeniden element seçimi | Onayda 100 altın, ana silah hemen nötrleşir; yeni katalizör ayrıca 80 altın. Yerleştirilene kadar nötr kalır. Reddetme ücret almaz. |
| Kule türü değişimi | Çantadaki satılabilir eşyalar **takılı parçalarıyla** satışa dahil. Yeni kule seviye 1/Normal; depo ve teslimler devredilir. Normal tek eşya satışıyla aynı kural değildir. |
| Depo | Kuleye özel, sınırsız liste; 12 görünen yerden sonra scroll. Stat vermez. Ana eşyanın depoya gitmesi kodda hâlâ mümkün; eski kullanıcı kararıyla çelişiyor. |
| Freeze | Tüm mağaza yerine sağ tıkla teklif başına rezervasyon. |
| Haritalar | Beş CampaignMaps prefabı ve yükleyici kodu var; açık sahnede CampaignMapLoader yok. Dosyalar hazır olmakla birlikte aktif bölüm geçişi tamamlandı sayılmaz. |

## 2. Yapılan işler

### Oyun döngüsü ve savaş

- [x] Hazırlık/savaş/zafer/yenilgi; otomatik veya manuel dalga başlatma, son düşmanı bekleme.
- [x] Pause/devam, yeniden başlatma, menüye dönüş, 1×/2×/3× hız.
- [x] Kale canı/hasarı, kaçan düşman, öldürme altını ve sonuç ödülü.
- [x] Birliklerin düşmanı tutması, bağımsız düşman saldırı ritmi, asker ölünce yoluna devam etme.
- [x] Aynı yolu takip eden hava düşmanları; birlikleri pas geçme.
- [x] Su → Ateş → Hava → Toprak → Su üstünlük döngüsü; düşman verisinde avantaj/direnç/nötr çarpanları.

### Kuleler ve gelişim

- [x] Okçu, büyücü, havan, birlik kulesi; slot seçimi, bilgi/satın alma, iptal ve yetersiz altın.
- [x] Okçu/büyücü Kara/Hava/İkisi; havan kara hedefleri; menzile ilk/son giriş önceliği.
- [x] Havan kavisli mermi, alan hasarı, mermi/efekt havuzu; güncel datada ıskalama şansı/mesafesi alanları.
- [x] Altınla seviye yükseltme ve panel düğmesi; seviye statları ve 5. seviyede element mağazası.
- [x] Kule tipi/element değişimi için kabul-red, ücret ve satış hesabı.
- [x] Uyumsuz element eşyalarında İŞLEVSİZ etiketi.

NewMap kule prefablarının bağlandığı datalara göre temel değerler:

| Kule | Fiyat | Hasar | Saldırı/sn | Menzil | Veri |
|---|---:|---:|---:|---:|---|
| Okçu | 250 | 13 | 1,35 | 4,7 | Resources/Data/TowerData/ArcherData |
| Büyücü | 325 | 29 | ≈0,526 | **5,2** | Prefabs/NewMap/MagicTowerData |
| Havan | 375 | 46 | ≈0,294 | 4,8 | Resources/Data/TowerData/MortarData |
| Birlik | 225 | 4 | ≈0,741 | 3 | Resources/Data/TowerData/UnitData |

Resources altındaki eski MegicData'nın menzili 3,4; NewMap büyücüsünün kullandığı veri farklıdır. Birlik menzili, menzilli kule hedefleme davranışıyla aynı anlamda değildir. Bu değerler seviye/eşya/skill öncesidir; çalışma ağacındaki güncel dosyalar esas alınmıştır.

### Çanta, mağaza, depo ve sinerji

- [x] Her kulede şekilli çanta; alma/taşıma, R/scroll rotasyonu, yerleşim önizlemesi ve iptal geri dönüşü.
- [x] Kuleye özel dört mağaza teklifi, nadirlik/seviye/element filtresi, yenileme ve teklif rezervasyonu.
- [x] Sınırsız depo ve toplu depoya taşıma; depo stat hesabına katılmaz.
- [x] Normal eşya satışı %50 aşağı yuvarlama; ana eşya satılamaz.
- [x] Evrim: tarifli iki komşu eşya başarılı dalga sonunda birleşir; ürün çantaya sığmazsa depoya gider.
- [x] Sürekli sinerji: belirli yerel kare veya takılı parça şartıyla stat bonusu; birleşme yok.
- [x] Takılı parçayı küçük ikonundan çıkarma; hedefle taşıma; normal satış/evrimde parçayı koruma.
- [x] Katalizör dönüşümü, ilk element teklifleri ve normal mağazaya dönüş kodu.
- [ ] Seviyeye göre mağaza teklif sayısını artırma henüz yok; dört slot devam ediyor.

### Arayüz, ses ve görseller

- [x] Boot yükleme, ana menü, bölüm seçimi, ayarlar, oyun HUD'u, pause ve ayrı sonuç ekranları.
- [x] Sonuçtan sonraki bölüme/skilltree'ye erişim; gerçek yükleme ilerlemesi ve tekrar deneme.
- [x] Birleşik kule/çanta/depo/mağaza paneli; kule statları ve hedefleme seçenekleri.
- [x] Eşya/kule/birlik/düşman hover bilgisi, can barları ve dalga önizlemesi.
- [x] ESC/dışarı tıklama kapatma ve sürükleme iptali kodu.
- [x] Ses/efekt seviyesi, ekran/kalite/FPS/VSync ayarları ve ayar kaydı.
- [x] GameAudio, ButtonAudio ve 33 kaynak SFX; kule, yakın dövüş, item, UI, kale ve sonuç olaylarına bağlantı.
- [x] Yeni kule model/mermi referansları, TowerModelShot/LegionWeapon atış sunumu, LegionFigure hareket/vuruş/ölüm görselleri, LegionFx element/kritik efektleri ve UI hareketleri.
- [x] Blackpine çevre/kale görsel çalışmaları ve beş harita prefabı üretimi.
- [ ] Son modeller, animasyon/mermi çıkış noktaları, efekt okunurluğu ve ses miksinin gerçek oynanış kabulü; dosya varlığı bu kabulün yerine geçmez.

### Skilltree ve kalıcılık

- [x] Kule türü başına ortak dört ağaç; SkillData/SkillTreeData ve Inspector'dan bağlama.
- [x] Can oranına göre 1/2/3 puan; aynı bölümde yalnız en iyi skor farkını kazanma.
- [x] Puan, açılan yetenekler, bölüm rekoru ve harcama makbuzlarının kalıcılığı; ücretsiz sıfırlama/iade.
- [x] 11 okçu yeteneği etkin; temel ve koşullu etkiler mevcut.
- [ ] Büyücü 12, havan 12, birlik 12 düğüm hâlâ **available=false**: görsel/data var, etkin yetenek yok.
- [ ] Bozuk/eski sürüm kayıttan güvenli kurtarma; bölüm kilidi ve yarım kalan oyuna devam kapsamı.

## 3. Bölüm ve içerik durumu

Açık sahnenin WaveManager listesi **Assets/Prefabs/NewMap/Chapters** altındaki beş ChapterData'yı kullanıyor. Resources/Data/Chapters kopyaları da duruyor; yalnız orayı düzenlemek aktif oyun içeriğini değiştirdiğini kanıtlamaz.

| Bölüm | Dalga | Düşman | Başlangıç altını | İlk hazırlık | Final boss canı |
|---|---:|---:|---:|---:|---:|
| 1 | 5 | 180 | 850 | 60 sn | 2400 |
| 2 | 7 | 260 | 1000 | 75 sn | 3840 |
| 3 | 9 | 340 | 1150 | 90 sn | 5280 |
| 4 | 11 | 420 | 1300 | 105 sn | 6720 |
| 5 | 13 | 532 | 1450 | 120 sn | 8160 |

Her sonraki dalgada +5 sn hazırlık. Canlı referans sorgusunda her finalde 1 boss ve iki destek grubu doğrulandı. Son bölüm toplamı doğrusal +80 dizisindeki 500 hedefinden farklı; bu tur denge değiştirilmedi.

Beş CampaignMaps prefabında CampaignMap, spawn ve goal referansları var; sırasıyla 5/6/7/8/9 kule noktası bulundu. Ancak açık GameScane'de yükleyici bulunmadı, kayıtlı scene/prefab metin taramasında da yükleyici GUID bağlantısı görülmedi. Aktif kökler eski test alanları ve Blackpine V5 harita kökünü içeriyor. **Sıradaki harita işi sıfırdan beş harita üretmek değil, hazırlanmış haritaları doğru akışa bağlayıp navigasyon ve bölüm geçişini doğrulamak.** Build Settings Boot/MainMenu/GameScane içeriyor; tek ortak sahneden farklı prefab yüklemek mümkündür.

Resources/Data sayımı: 200 mağaza itemi, 20 ana item, 20 evrim sonucu, 4 katalizör, 21 evrim tarifi, 22 sürekli sinerji tarifi, 47 skill, 4 ağaç, 30 kampanya düşman verisi. Bunlar varyantları da içeren dosya sayılarıdır; ayrı mekanik/model sayısı veya bütün referansların aktif olduğu iddiası değildir.

## 4. Test geçmişi ve bugün açık kalanlar

- **15 Ekim:** skill data ve sinerji/attachment kontrolleri mevcut; sınırlı depo ve eski element/XP varsayımları bugün güncel değil.
- **23 Ekim:** ses entegrasyonu derleme, klip eşleşmesi, ses kaynağı sınırı, pause/mute kontrolleri kaydedilmiş; işitsel miks kabulü yapılmamış.
- **27 Ekim:** dört kulede 44 dönüşüm kontrolü ve 24 nötr element kontrolü geçmiş. Kabul/red, yetersiz altın ve nötr kalma için “hiç test yok” maddesi kaldırıldı.
- **27 Ekim oyuncu akışı:** [rapor](Playtests/2026-10-27/Oynanis_Raporu.md). İlk bölüm üçüncü denemede 4/100 canla kazanılmış; 2–5 kazanılarak bitirilmemiş. Bu sıfır skill kaydı deneyi değil. Yeni altın yükseltmesi ve 5 Kasım değişiklikleri için yeniden denge kanıtı sayılmaz.
- **5 Kasım:** salt okunur editör/kod/data incelemesi; Console hata/uyarı sorgusu boş, açık sahnede missing script 0. Yeni Play/build testi yapılmadı.

## 5. Öncelik sırasıyla yapılacaklar

### A — Güncel kuralları ve sahne bağlantılarını sağlamlaştır

- [ ] **Ana eşya/depo:** EvaluatePlacement ve MoveAllToStorage ana eşyayı depoya taşıyabiliyor. Önceki “taşınamaz” kararıyla çelişki sürüyor. Bu kuralı netleştirip tekli/toplu taşıma ve geri dönüşü birlikte test et.
- [ ] **Harita bağlantısı:** mevcut beş prefabı bölüm seçimine bağla; her bölümde doğru harita, tek aktif kale, doğru spawn, NavMesh yolu ve kule noktaları doğrulansın. PlaneTest silinmesin.
- [ ] **Faz kuralları:** savaşta kule alımı/yükseltmenin güncel davranışını tasarım kaydına bağla; eski hazırlık-kilitli testleri buna göre yenile. Eşya ve dönüşüm kilitlerini ayrı test et.
- [ ] **Aktif veri envanteri:** NewMap/Resources/legacy kopyalarının hangi sahne ve prefablarda kullanıldığını belgeleyip Inspector rehberini güncelle. Bağlantıları incelemeden kopya silme.
- [ ] Altın yükseltmede bir kez ödeme, yetersiz altın, pause/sonuç, 5. seviye mağazası, seviye tavanı ve yeni/mevcut birlik stat aktarımı.
- [ ] Saldırı/sn dönüşümünün tüm kule/asker/UI/skill hesaplarında tutarlılığı; gerçek atış aralıkları ve yeni model animasyonlarıyla kontrol.
- [ ] Dört kulede al/iptal → rezervasyon → drag/rotasyon → depo/satış → sinerji/evrim → element/tür değişimi regresyonu; sahne geçişi ve ESC dahil.

Çıkış ölçütü: güncel kuralları temsil eden tekrar çalıştırılabilir kayıt; yanlış harita/veri, kayıp/çoğalma ve çift ücret yok. Kullanıcı değişiklikleri korunur; kod mimarisi önce konuşulur.

### B — Yeni ekonomi ve kampanya dengesini ölç

- [ ] Altın bütçesini kule satın alma, yükseltme, item, yenileme ve 5. seviyeye ulaşma maliyetiyle birlikte değerlendir. XP döneminin denge sonucunu devralma.
- [ ] Beş bölümü sıfır skill kaydı ve ilerlemiş kayıtla, farklı makul kule/yerleşim planlarıyla bitirmeyi dene; son dalgalar ayrıca kapsansın.
- [ ] Boss + destek, üçüncü dalga hava savunması, element karşılığı, rezervasyon ve sınırsız stoklama gücünü ölç.
- [ ] 5. bölümde 532 sayısını doğrusal hedefle uzlaştır; otomatik olarak 500'e indirme.
- [ ] 1×/2×/3×, düşük FPS, saldırı animasyonu ve havan ıskalama dağılımını karşılaştır.
- [ ] Ölçü kaydı: kalan can, dalga, harcanan altın, kurulum, kaçan düşman ve hasar katkısı. Eski XP katkı sayacı kaldırıldığı için hazır DPS altyapısı varsayma.

Çıkış ölçütü: ilk bölüm birkaç anlaşılır stratejiyle kazanılabilir; ileri bölümler element/yerleşim gerektirir; nerede ve neden kaybedildiği ölçülür. Sayılar bundan sonra kontrollü ayarlanır.

### C — Kalan skilltree içeriklerini tamamla

- [ ] Büyücü, havan, birlik yeteneklerini/ön koşullarını/maliyetlerini Emir ile belirle; mevcut 36 taslağı bu kararlara göre doldur.
- [ ] Mevcut SkillData etkileriyle yapılabilen pasiflerden başla; yeni özel etkilerin mimarisini önce tartış.
- [ ] Eski GDD Doğa/Buz, zırh/stun, XP ve element ücret varsayımlarını güncel kurallara uyarla.
- [ ] Açılım, reset, eski kayıt ve mevcut/yeni kuleye etki testleri; toplam erişilebilir puanla maliyet dengesini kontrol et.
- Sabit karar: ek ok biriktirme skilltree'ye değil, eşyalara ait.

### D — Görsel kabul, içerik ve öğretici

- [ ] Yeni dört modelin pivot/ölçek/menzil/gövde collider/mermi çıkış noktası ve hedef dönüşünü oyun kamerasından kontrol et.
- [ ] Düşman/birlik siluetleri, vurulma/ölüm, element ve kritik efektleri kalabalık savaşta okunabilsin; ses seviyeleri dinlenerek onaylansın.
- [ ] UI'larda 16:9/16:10/dar pencere, uzun metin, hover, scroll/drag ve raycast engelleme testleri.
- [ ] Kısa öğretici: kule + altınla yükseltme → eşya şekli/rotasyon → depo/satış → sinerji/evrim → element karşılığı.
- [ ] Bosslara yalnız yüksek candan farklı, birlikte tasarlanmış davranışlar; bestiary/Öğren/ödül sistemi.
- [ ] İlk beş harita kabulünden sonra 20 bölüm ve her beş bölümde tema hedefi; daha fazla prefabı peşinen tamamlandı sayma.

### E — Kalıcı kayıt, build ve yayın

- [ ] SkillTreeProgress geçersiz sürüm/bozuk JSON için güvenli kurtarma, yedek ve göç politikası; mevcut puan kaydı korunmalı.
- [ ] Bölüm kilitleri ve yarım kalan koşuya devam kapsamını belirle; ayar/skill kaydı tüm koşunun kaydı değildir.
- [ ] Windows build'de Boot → bölüm → sonuç → sonraki bölüm → yeniden açılış testi ve profil ölçümü.
- [ ] Havan/impact havuzları mevcut; normal mermi/düşman oluşturma, WaveManager tag taraması, GameAudio periyodik UI taraması ve efekt sayısını ölçüp gerekirse optimize et.
- [ ] TR/EN metin/font çalışması, ses/sanat lisansları, farklı cihazlar ve dış oyuncu testleri.
- [ ] Üretim takvimini gerçek teslim ve test sürelerine göre yeniden değerlendir; tarih geçti diye iş tamamlandı sayma.

## 6. Çalışma anlaşması ve belgeler

GameManager kalır, PlaneTest korunur. Yeni sınıf/mimari kararları birlikte alınır. Yazılan kodun görev dağılımı, veri akışı ve Inspector kullanımı açıklanır. Bu revizyon yalnız belge ve inceleme kayıtlarını değiştirir; gameplay, sahne, prefab, kayıt veya dengeye müdahale edilmedi; commit/push yok.

- [27 Ekim dönüşüm kuralları](Kule_Element_Degisimi_2026-10-27.md): XP ifadeleri altın yükseltme değişiminden önceye aittir.
- [Ses entegrasyonu](Ses_Entegrasyonu_2026-10-23.md)
- [SkillData rehberi](SkillData_Kullanim_Rehberi_2026-10-15.md)
- [Evrim ve sinerji rehberi](Evrim_ve_Konum_Sinerjisi_2026-10-15.md): sınırlı depo ve tür değişiminde parça koruma anlatımı artık bütün işlemler için geçerli değil; güncel ayrım yukarıda.
- [Üretim/yayın planı](Uretim_Yayin_Plani_2026-10-14.md), [günlük takvim](Gunluk_Uretim_Takvimi_2026-2027.md): ayrı üretim tahmini; bu incelemede dış platform tarihleri yeniden doğrulanmadı.

**Önerilen ilk çalışma:** ana eşya/depo ve savaş fazı kurallarını kesinleştir; mevcut haritaların sahne bağlantısını tamamla; altın yükseltme dahil güncel dört-kule regresyonunu çalıştır. Sonra tam bölüm dengesi ve kalan skilltree'lere geç.

