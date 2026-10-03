# Backpack Legions — Ana Yol Haritası (V2)

*2026-09-22 · GDD V4 + Skill Tree V1 + canlı proje incelemesi (kod + sahne + asset verisi + play mode testi) baz alındı.*
*Eski `Backpack_Legions_Yol_Haritasi.md`'nin yerini alır.*

---

## 📊 Durum Özeti

| GDD Sistemi | Durum | Not |
|---|---|---|
| Kule başı bağımsız backpack grid | 🟢 %80 | Sağlam. Rotasyon yok |
| Sürükle-bırak + mağazadan yerleştirme | 🟢 %90 | |
| Ortak gold havuzu | 🟢 | Static, sahne yeniden yüklenince elle sıfırlanıyor |
| Kule tipine özel dükkan (RNG + rarity) | 🟡 %50 | Her kule tıklamasında **bedava** yenileniyor, havuz boşsa çöküyor |
| Dalga spawn | 🟡 %40 | Zamanlayıcıyla akıyor, hazırlık fazı yok |
| Kale canı / Game Over / Pause | 🟡 %60 | Kazanınca da "KULE DÜŞTÜ" ekranı çıkıyor |
| Element zayıflık/direnç | 🟡 %20 | Sadece çarpan var. Enum GDD ile uyuşmuyor |
| Barracks (UnitTower) | 🟡 %40 | Birlikler çalışıyor ama stat aktarımı sadece spawn anında |
| Hazırlık ↔ Savaş fazı + kilit | 🔴 %0 | |
| Oyun hızı 1x/2x/3x | 🔴 %0 | |
| Hedefleme tipleri | 🔴 %0 | Şu an "menzile ilk giren" hedefleniyor |
| Global Freeze / Refund | 🔴 %0 | |
| Sinerji / Craft (birleştirme) | 🔴 %0 | `synergyIDs` alanı var, hiç okunmuyor |
| XP / Seviye / Element evrimi | 🔴 %0 | `UpgradeTower()` hiçbir yerden çağrılmıyor |
| Castle HQ + Skill Tree | 🔴 %0 | |
| Savaş Analiz UI | 🔴 %0 | |
| Bestiary / Ustalık | 🔴 %0 | |
| 20 bölüm / tema / wave ölçekleme | 🔴 %0 | 1 harita, 4 dalga |
| Kayıt sistemi (meta ilerleme için şart) | 🔴 %0 | |

**Genel ilerleme: GDD kapsamının kabaca %20-25'i.** Çekirdek prototip oynanabilir durumda: kule al → item al → yerleştir → savaş.

---

## 🏗️ Mimari Değerlendirme

### İyi kurulmuş olanlar ✅
- **`Backpack` / `PlacedItem` / `ItemShapeUtility`**: Saf C#, MonoBehaviour'dan ayrılmış, tek veri kaynağı. Ayrıca `ItemData` (ScriptableObject) hiç değiştirilmiyor, runtime state ayrı tutuluyor. Projenin en temiz katmanı bu.
- **`BackpackDraggable`**: Mağazadan yerleştirme ve backpack içinde taşıma tek kod yolundan geçiyor.
- **Veri odaklı içerik**: Kule, item, düşman ve mermi verisi ScriptableObject olarak tutuluyor.
- **Kule kalıtımı**: `Tower` (abstract) → `ArcherTower` / `MegicTower` / `UnitTower`. GDD'deki doğrudan/dolaylı ayrımına uyuyor.

### Büyüme önünde engel olanlar ⚠️
1. **Merkezi bir stat sistemi yok.** `CalculateStats()` sadece item bonuslarını üst üste topluyor. Oysa GDD'de stat değiştiren **7 ayrı kaynak** var: item, kule seviyesi, element, sinerji, Castle HQ, skill tree ve bestiary. Bunların her biri kendi `+=` satırını eklerse kısa sürede yönetilemez hale gelir. **Bir sonraki büyük sistemden önce kurulması gereken tek şey bu.**
2. **Merkezi bir hasar pipeline'ı yok.** Hasar `Bullet` → `Enemy.TakeDamage(int, element)` şeklinde, ham sayı olarak geçiyor. Hasarın hangi kuleden geldiği, kritik olup olmadığı ve hangi item'dan geldiği kaybolduğu için Savaş Analiz UI ve bestiary bonusu yazılamaz.
3. **Faz / state yönetimi yok.** Oyun durumu `Time.timeScale = 0/1` ile yönetiliyor. `WaveManager` `Start()`'ta kendiliğinden başlıyor ve zamanlayıcıyla akıyor. Hazırlık/savaş döngüsü, kilitler ve hız kontrolü için bir `GamePhase` state machine gerekiyor.
4. **`UİManager` her işi yapıyor ve static'lere dayanıyor.** Kule satın alma, altın, dalga ve can yazıları, seçili slot... hepsi burada. `GameObject.Find("isim")` ile bağlanıyor, `public static` alanlarla paylaşılıyor. Sahne değişince ve test yazarken sorun çıkarır.
5. **Kule fiyatları iki yerde tanımlı.** `UİManager` içinde 100/150/120 sabit, `TowerData.cost` içinde 300/400/100. UI'da ise "45 Gold" yazıyor. Üç farklı değer var.
6. **Olay (event) sistemi yok.** Her şey birbirini doğrudan çağırıyor (`Enemy.Die` → `EconomyManager` → `UİManager`). Bestiary sayacı, analiz ve XP gibi sistemler "düşman öldü", "hasar verildi" gibi olayları dinlemek zorunda.
7. **Dosya kodlaması (encoding) riski 🔥**: 14 script UTF-8 değil, Windows-1254 kodlamasında. Sınıf adı `UİManager` ve `GameObject.Find("İtemShopUİ")` Türkçe `İ` içeriyor. Senin makinende derleniyor ama farklı dil ayarlı bir makinede, Mac'te veya CI'da **derlenmez**.

---

## 🐞 Canlı Testte ve Veride Bulunan Somut Buglar

| # | Bug | Etki |
|---|---|---|
| B1 | **Item stat ölçeği uyumsuz.** Item'larda `crit+5`, `as+7` var, kule ise 0–1 kesir ölçeğinde çalışıyor (`baseCrit=0.1`, `baseAS=1`). | Mağazadan **tek bir item** alınca kritik şansı %510 olur (her atış kritik). Cooldown da 0.1'e sabitlenir (maksimum atış hızı). Oyun dengesi fiilen yok. |
| B2 | `MegicData.type = ArrowTower` | Büyücü kulesinin dükkanında okçu item'ları çıkıyor. |
| B3 | Rarity havuzunda boşluklar var (Arrow: Rare=0, Legendary=0 · Magic: Legendary=0). `GetRandomItem` boş listeden seçim yapıyor. | Kule seviye 2+ olunca `ArgumentOutOfRange` hatasıyla **dükkan çöker**. Şu an seviye atlama olmadığı için hata gizli kalıyor. |
| B4 | `Tower.OnMouseDown` her tıklamada `GenerateShopItems()` çağırıyor. | **Bedava sınırsız reroll.** 10 gold'luk reroll butonu anlamsız kalıyor. |
| B5 | Tüm dalgalar bitince `GameManager.GameOver()` çağrılıyor. | Kazanınca "KULE DÜŞTÜ ! OYUN BİTTİ !" yazıyor. |
| B6 | `UİManager.itemUIPrefab` referansı kopmuş (Missing). | Şu an kullanılmıyor. Ölü alan. |
| B7 | `Enemy.Start()` içinde 6 adet `Debug.Log`, `TakeDamage` içinde her vuruşta bir log var. | Konsol kirleniyor, performans düşüyor. |
| B8 | Eski yol haritasında `HandleClickOutside` → `IsPointerOverPanel()` düzeltmesi "yapıldı" diye işaretli ama kodda hâlâ `IsPointerOverGameObject()` var. | Doküman ile kod arasında sapma var. Tekrar kontrol et. |
| B9 | `UnitTower` birliklere hasarı sadece spawn anında aktarıyor. `UnitData.defaultWeapon = null`. | Savaş sırasında item eklenirse mevcut birlikler güçlenmiyor. |
| B10 | `Unit.TakeDamage` içinde `health < 0` kontrolü var, `Die()` içinde ise `<= 0`. | Canı tam 0 olan birlik ölmüyor. |
| B11 | Oyun başında dalga zamanlayıcısı bekliyor. Start'a basılınca kule dikecek zaman kalmadan dalga geliyor. | Hazırlık fazı yok (Faz 2'de çözülecek). |

---

## 🗺️ Yol Haritası

> Sıralama bağımlılığa göre yapıldı. Her faz bir öncekinin üstüne kuruluyor. **Faz 0 ve Faz 1 atlanmamalı.** Sonraki her sistem (skill tree, element, Castle HQ, analiz) bunlara dayanıyor.

### FAZ 0 — Stabilizasyon *(kısa, 1–2 oturum)*
- [x] **Encoding**: 14 script UTF-8'e çevrildi (BOM'suz). *(`UİManager` → `UIManager` yeniden adlandırması ve GameObject adlarındaki `İ` hâlâ açık, ayrı karar)*
- [x] **B1**: Item bonusları kesir oldu (0.05 = %5), tüm asset'ler 100'e bölündü. Hasar = base × (1 + Σbonus), Kritik = base + Σbonus, Bekleme = base / (1 + Σhız).
- [x] **B13**: `Bullet` artık hasarı `(int)` kesmeden, olduğu gibi gönderiyor.
- [ ] **B14**: `Tower.cs`, `TowerSlot.cs`, `Enemy.cs` ve `UİManager.cs` içindeki bazı Türkçe karakterler geçmişte `�` olarak bozulmuş (yorumlar + `Enemy` debug mesajı). *Kullanıcı kararı: şimdilik dokunulmayacak.*
- [x] **B2**: `MegicData.type`. Kullanıcı kendisi düzeltti.
- [x] **B3**: Eksik rarity'ler için placeholder item'lar eklendi (`ItemData/Placeholder/`). Tüm 16 kombinasyon çalışıyor. *İtemler belirlenince silinecek.*
- [x] **B4**: Dükkan kule kurulunca bir kez üretiliyor, sadece reroll ile yenileniyor. Satın alınan item kulenin listesinden siliniyor.
- [x] **B5**: Tek panel, `GameOver(bool)`: "BÖLÜM GEÇİLDİ !" / "KULE DÜŞTÜ ! OYUN BİTTİ !". Kazanma son dalga spawn olup sahadaki düşmanlar bitince tetikleniyor.
- [x] **B6, B7, B10**: `itemUIPrefab`/`itemsContainer`/`cellSize` silindi, `Enemy` debug logları silindi, `Unit` ölüm kontrolü `<= 0` yapıldı. *(Boş `GameEnums`/`SynergyLineManager` bilinçli olarak bırakıldı.)*
- [x] Kule fiyatları tek kaynaktan: `UİManager` fiyatı prefab'ın `TowerData.cost`'undan okuyor (Okçu 300 / Büyücü 400 / Birlik 100). *Not: sağdaki `TowerShopPanel` hiçbir koda bağlı değil, "45 Gold" sabit yazı.*
- [x] **B15**: `UnitTower1` prefab'ında `myShopItems` dizisi 4 yapıldı. Boş dizi yüzünden tıklamada exception fırlıyor, backpack açılmıyordu.
- [x] **Birlik kulesi backpack → birlik buff'ı (kod)**: Item'lar birliklere hasar/saldırı hızı/can/kritik (x1.5) veriyor, backpack değişince sahadaki birlikler anında güncelleniyor. `ItemData.healthBonus` eklendi.
- [x] **Birlik kulesi default item'ı**: `UnitDefaultItem` (1x2, 0 stat) → `UnitData.defaultWeapon`.
- [x] **Birlik yol hizası**: Birlikler kuleye en yakın yol parçasının ortasına, yol yönünde diziliyor (`CalculateSlotPositions` + `FindRoadCenter`). Tag / rally noktası gerekmiyor.
- [ ] Artık kullanılmayan: `UnitTower.rallyPointTransform/Up/rallyPoint/directionUp`, `TowerSlot.directionUp`, `UİManager.BuyTower`'daki directionUp satırı, prefab'daki RallyPointMaker objeleri. Temizlensin mi?
- [ ] Aynı virajı paylaşan iki birlik kulesinin birlikleri birbirine çok yaklaşabiliyor (testte 0.08 birim).
- [x] **Birlik item asset'leri** (`ItemData/Unit/`): Akut'un Ayakkabısı, Lejyoner Kılıcı, Deri Kalkan, Savaş Borusu, Komutan Sancağı. Common "Deneme 7–19"dan UnitTower sahipliği kaldırıldı, `ShopManager.allItems`'a eklendi. Play modunda test edildi.
- [x] **B8**: Dışarı tıklama. Sadece backpack + item dükkanı panellerinin içi paneli açık tutuyor. Diğer her yer (HUD/butonlar dahil) kapatıyor.
- [ ] **B12**: Sahnede iki `Base` var (`PlaneTest/Base` trigger + Untagged, `PlaneTestMVP/Base` tag=Base). Hangisi kalacak?
- [ ] Kule fiyatlarını tek kaynağa bağla (`TowerData.cost`).
- [ ] `GameEnums` MonoBehaviour'u boş, `SynergyLineManager` da boş: sil ya da doldur.

### FAZ 1 — Mimari Temel *(en kritik faz)*
- [ ] **Stat sistemi**: `StatType` (Damage, AttackSpeed, CritChance, CritMultiplier, Range, ArmorPen, UnitHP, SpawnTime...) + `StatModifier { type, value, Flat/PercentAdd/PercentMult, source }` + `StatBlock.GetValue()`. Kule, item ve birlik bu sistemi kullanacak. Tüm sonraki sistemler sadece modifier ekleyip çıkaracak.
- [ ] **Hasar pipeline'ı**: `DamageInfo { amount, element, isCrit, sourceTower, sourceItems }` → `Enemy.TakeDamage(DamageInfo)`. Zayıflık/direnç, bestiary bonusu ve skill tree karşı-element bonusu tek bir fonksiyonda hesaplanacak.
- [ ] **Event bus**: `OnEnemyKilled`, `OnDamageDealt`, `OnWaveStarted/Ended`, `OnPhaseChanged`, `OnGoldChanged`. UI, analiz, bestiary ve XP bunları dinleyecek.
- [ ] **`GameStateManager` / `GamePhase`**: `Preparation → Combat → WaveCleared → (Victory | Defeat)`. `Time.timeScale` hilesi yerine bu kullanılacak.
- [ ] **`UIManager`'ı böl**: `TowerBuildController` (slot + satın alma), `HUDController` (altın/dalga/can), panel controller'ları. Static alanlar yerine Inspector referansları ve event'ler kullanılacak.
- [ ] **`TowerFactory`**: `TowerData`'ya prefab referansı eklenecek. Kule satın alma `if/else` zinciri yerine veri odaklı olacak.
- [ ] Static `Gold` state'i yerine sahne ömrüne bağlı bir `EconomyManager` instance'ı.

### FAZ 2 — Çekirdek Döngünün Tamamlanması (GDD §1.1, §2.3, §3)
- [ ] Dalga dalga akış: Hazırlık (sınırsız süre, "Dalgayı Başlat" butonu) → Savaş → dalga temizlenince tekrar Hazırlık.
- [ ] **Savaş kilidi**: Combat fazında backpack ve dükkan etkileşimi kapalı. Hedefleme seçimi bu kilitten muaf.
- [ ] **Dükkan yenileme kuralı**: Her hazırlık fazı başında ücretsiz Auto-Reroll. Kule tıklaması dükkanı yenilemesin (B4).
- [ ] **Global Freeze** butonu (Auto-Reroll'dan muaf).
- [ ] **Geri satış (Refund)**: `iade = maliyet × 0.5–0.6`. Item satma + kule satma.
- [ ] **Oyun hızı** 1x / 2x / 3x.
- [ ] **Hedefleme tipleri**: Kara / Hava / Yakın / Uzak. Düşmana `isFlying` bayrağı + yol ilerlemesi (`remainingDistance`) eklenecek. `Tower`'daki `enemies[0]` seçimi strateji deseniyle değiştirilecek.
- [ ] Kule bilgi paneli (statlar + hedefleme seçimi).

### FAZ 3 — Backpack Derinliği (GDD §2.1)
- [ ] **Rotasyon** (GDD istiyor, ama `ItemData`'daki yorum "dönmüyor" diyor. **Karar gerekli**).
- [ ] **Sinerji sistemi**: Komşuluk veya ID tabanlı eşleşme → Faz 1'deki stat sistemine modifier olarak eklenecek. Aktif sinerjiler görsel olarak gösterilecek.
- [ ] **Craft / birleştirme** (GDD §1.1'de geçiyor ama tanımı yok. **Tasarım dokümanı gerekli**).
- [ ] "Ana silah" kavramı (`ItemData.isMainWeapon`). Element değişimi buna bağlı.
- [ ] Item tooltip'i (stat, rarity, sinerji).

### FAZ 4 — Kule Evrimi (GDD §4)
- [ ] XP kazanımı (öldürme/hasar üzerinden, event bus ile) + seviye 1→5.
- [ ] Seviyeye göre rarity şansı (`RarityData` zaten var, veriye taşınacak).
- [ ] Seviye 5'te **Element Sınıfı Seçim Ekranı**.
- [ ] Element silah havuzu (ana silah satılınca havuz yenilenir).
- [ ] Kule tipi değiştirme (envanter tamamen boşken aktif olan buton).
- [ ] Element Değişim Enflasyonu (kule instance'ında kalıcı, +%10 katlanan).
- [ ] Sinerjisi olmayan item'ların grid'de işaretlenmesi.
- [ ] `ElementType` enum'unu GDD ile uyumlu hale getir (Nötr, Ateş, Su, Doğa, Buz... + Neutral Enemy).

### FAZ 5 — Kule Çeşitliliği (GDD §7)
- [ ] Barracks: Birlikler stat'ları kulenin `StatBlock`'undan **canlı** okusun (B9). Birlik HP'si, spawn süresi ve kapasite de stat olsun.
- [ ] Havan/Topçu kulesi (`CannonTower` enum'u var, sınıfı yok): AoE hasar.
- [ ] Mermi/VFX çeşitliliği, zırh delme.

### FAZ 6 — Meta İlerleme
- [ ] **Kayıt sistemi** (JSON). Skill tree ve bestiary için şart.
- [ ] **Castle HQ**: Oyun içi gold ile hazırlık fazında satın alınan global pasifler (düşman yavaşlatma, tüm kulelere atış hızı/menzil).
- [ ] **Skill Tree V1**: Okçu, Büyücü ve Birlik dallarının her biri 6 yetenek + ulti içeriyor. Tümü Faz 1'deki stat/hasar/event sistemine modifier veya tetikleyici olarak eklenecek.
- [ ] **Bestiary**: Düşman kilidi açma → "Öğren" → mini görev (event sayaçları) → kalıcı karşı-düşman hasar bonusu.

### FAZ 7 — Savaş Analiz UI (GDD §5.2)
- [ ] Pause'da açılan overlay: kule başına toplam hasar ve DPS, item tetiklenme frekansı, kule × düşman tipi etkinliği. Veri Faz 1'deki `DamageInfo` ve event bus'tan gelecek.

### FAZ 8 — İçerik ve Bölüm Yapısı (GDD §1.2)
- [ ] `StageData` / `WaveData` ScriptableObject'leri (karışık düşman grupları, gecikmeler).
- [ ] Logaritmik dalga ölçekleme formülü.
- [ ] Bölüm seçim ekranı + bölüm geçişi.
- [ ] 4 tema × 5 bölüm (Orman, Çöl...). Her temanın haritası ve düşman ailesi ayrı.
- [ ] Uçan düşmanlar (hedefleme için gerekli), boss'lar.
- [ ] Denge geçişi (Skill Tree §3'teki test prosedürleri dahil).

### FAZ 9 — Cila ve Yayın
- [ ] SFX/VFX, hasar sayıları, UI teması, ayarlar menüsü, tutorial.
- [ ] Performans (object pooling: mermi ve düşman).
- [ ] Build pipeline.

---

## ❓ Karar Bekleyen Tasarım Soruları

1. **Stat ölçeği**: Item bonusları yüzde (`+5` = %5) mi olsun, kesir (`0.05`) mi? *(Öneri: Inspector'da yüzde gir, kodda /100.)*
2. **Rotasyon**: GDD "var" diyor, koddaki yorum "Backpack Battles'da dönmüyor" diyor. Hangisi geçerli?
3. **Skill Tree nerede yaşıyor?** Doküman hem "Castle HQ üzerinden" hem "kalıcı ilerleme" diyor. Oyun içi gold mu, run'lar arası kalıcı bir para birimi mi?
4. **Craft/birleştirme** nasıl çalışacak? (Aynı 2 item → üst rarity mi? Tarif tabanlı mı?)
5. **Element listesi**: Kodda `Normal/Fire/Water/Earth/Air`, Skill Tree'de `Ateş/Doğa/Buz`, GDD'de `Nötr/Ateş/Su`. Kesin liste ne?
6. **"Uzak Öncelikli"**: Yolda en önde giden mi (üsse en yakın), menzilde en uzak olan mı?
7. **XP kaynağı**: Öldürme mi, verilen hasar mı, dalga sonu toplu mu?
