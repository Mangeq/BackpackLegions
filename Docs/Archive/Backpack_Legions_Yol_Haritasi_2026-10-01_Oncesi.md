# Backpack Legions — Geliştirme Yol Haritası

*GDD V4 (2026-06-30 revizyonu) + tüm paylaşılan scriptler baz alınarak güncellendi.*

---

## 🎯 MVP Bitirme Listesi (öncelik sırasına göre)

### Doğrulanmış — bunlar zaten çalışıyor
- Dalga spawn sistemi (`WaveManager`)
- Kule → mermi → hasar zinciri (`Tower`, `Bullet` — iskelet var, matematiği eksik)
- Kale/can sistemi + game over/pause/restart/ana menü akışı (`Base`, `GameManager`, `MainMenuManager`)
- Element tipi altyapısının veri katmanı (`BulletData.bulletType`, `EnemyData.weaknesses/resistances`)

### P0 — MVP'yi bloklayan gerçek eksikler ve buglar

1. ~~Item statları savaşa hiç yansımıyor~~ ✅ **Çözüldü** — `Tower.CalculateStats()` artık `Backpack.Items`'ı toplayıp `currentDamage/currentCriticalChance/currentAttackSpeed`'i güncelliyor, `BackpackDraggable.FinishDrag()` her etkileşim sonunda bunu tetikliyor.
2. ~~`Bullet.cs` kritik hasar hesabı hatalı~~ ✅ **Çözüldü** — `bulletData.Damage` çarpanı kaldırıldı, `BulletData`'dan `Damage`/`criticalChance` alanları da silindi (hiç okunmuyorlardı).
3. ~~Kritik şans ölçek uyumsuzluğu~~ ✅ **Karar verildi** — `Bullet.cs`'te `Random.Range(0f, 100f)` → `Random.Range(0f, 1f)` olacak (Tower/item tarafındaki 0-1 kesir ölçeğine uysun diye). Kullanıcı kendi uygulayacak.
4. ~~`Enemy.cs` paylaşılmadı~~ ✅ **Çözüldü** — paylaşıldı, incelendi. Weakness/resistance çarpanı zaten doğru yazılmış, dokunmadık.
5. ~~`Base.cs` kalp eşiği hatalı~~ ❌ **Yanlış alarm** — `maxHealth = 10000` sadece test amaçlıymış, normalde `100`. `maxHealth = 100` iken ham can değeri zaten yüzdeyle bire bir örtüşüyor, kod olduğu gibi doğru. Listeden düşüyor.
6. ~~`MainMenuManager` sahne adı~~ ✅ **Doğrulandı, sorun yok** — sahne gerçekten `"GameScane"` adında, yazım hatası değilmiş.
7. **Kazanma/kaybetme aynı ekrana mı gidiyor?** — şimdilik erteleniyor, MVP'yi bloklamıyor.

**Bonus bulgu:** `MegicTower.Attack()`'ta `Initialize()` hiç çağrılmıyordu, Büyücü kulesi şimdiye kadar her zaman 0 hasar veriyordu. ✅ **Çözüldü.**

**🎉 P0 listesi tamamlandı.** Kritik şans düzeltmesini uygulayınca MVP'yi bloklayan bilinen bir şey kalmıyor.

## ✅ MVP DOĞRULANDI

Kritik şans düzeltmesi uygulandı, uçtan uca gerçek bir playtest yapıldı (kule alma → mağazadan item satın alma → backpack'e yerleştirme → dalga başlatma → hasar/kritiğin fark yaratması) ve her şey çalışıyor. **MVP tamamlandı.**

Bilinçli olarak dışarıda bırakılanlar (aşağıdaki P1/P2) hâlâ açık — bunlar "unutulan" değil, MVP kapsamı dışında bilinçli tercih: rotasyon, savaş sırasında backpack/shop kilidi, sinerji efektleri, hedefleme tipleri, element/evrim sistemi, Barracks, Castle HQ, Savaş Analiz, Bestiary, bölüm/wave içeriği.

### Sırada ne var
MVP bitti, şimdi tercih meselesi:
1. **Burada bir süre dur** — MVP'yi olduğu gibi bırak, oynanabilir bir prototipin var.
2. **P1'e geç** — rotasyon, faz kilidi, sinerji efektleri; oynanışı daha "tam" hissettirir.
3. **Genişletilmiş roadmap'e (P2) atla** — element/evrim, Castle HQ, Barracks entegrasyonu gibi daha büyük sistemler.

---

## 🔧 MVP Sonrası Playtest Düzeltmeleri

Gerçek oynanış sırasında bulunan, GDD'de öngörülmemiş ama düzeltilmesi gereken iki şey:

- [x] **Dükkan "satıldı" durumu kule değişince sıfırlanmıyordu** — `ShopUIManager.UpdateShopUI()` `currentItem`'ı yeniliyordu ama `isPurchased`'a hiç dokunmuyordu. Bir kulede satın alınan slot, başka bir kulede kalıcı olarak kilitli kalıyordu. `UpdateShopUI()`'a `isPurchased = false` sıfırlaması eklendi.
- [x] **Boşluğa tıklayınca backpack/shop kapanmıyordu** — `BackpackUIManager`'a global tık dinleyicisi eklendi (`Update()` + `HandleClickOutside()`). İlk versiyon `EventSystem.IsPointerOverGameObject()` kullanıyordu ama bu, backpack/shop'la ilgisi olmayan başka bir UI (örn. hazırlık aşamasındaki Start ekranı) ekranı kaplayınca her tıklamayı "UI üzerinde" sayıp kapatmayı engelliyordu. Sadece backpack ve shop panellerinin kendi `RectTransform`'una bakan `IsPointerOverPanel()` ile değiştirildi.

---

### P1 — MVP'de olsa iyi olur, bloklamaz
- Faz kilidinin tam hali (savaş sırasında backpack/shop kilitlenmesi) — GDD bunu "exploit önleme" olarak çerçeveliyor, temel döngü onsuz da test edilebilir
- `synergyIDs` sinerji efektleri
- Rotasyon
- `Base.TakeDamage`'daki tekrarlanan `FindObjectOfType<UİManager>()` — çağrı başına performans kaybı, cache'lenebilir

### P2 — açıkça post-MVP
- `UnitTower`/`Unit` (Barracks tipi) — şu an `Tower` sınıfından tamamen kopuk, backpack/shop/leveling'e hiç bağlı değil, `damage` alanı hiçbir yerden set edilmiyor. Bir gün bağlanması gerekecek ama MVP'de dokunmuyoruz.
- Hedefleme tipleri, Global Freeze, refund, element/evrim sistemi, Castle HQ ağacı, Savaş Analiz/Pause, Bestiary, 20 bölüm

---

## ✅ Tamamlanan (genel roadmap)

- [x] Kule başına bağımsız backpack/grid sistemi (`Backpack`, `PlacedItem`, `ItemShapeUtility`)
- [x] Sürükle-bırak: mağazadan yerleştirme + backpack içi taşıma (`BackpackDraggable`)
- [x] Geçerli/geçersiz hücre highlight (yeşil/kırmızı önizleme)
- [x] Ortak Gold havuzu (`EconomyManager`)
- [x] Kule tipi + rarity bazlı dükkan üretimi (`ShopManager`)
- [x] Varsayılan silahın kule oluşumunda backpack merkezine otomatik yerleşmesi
- [x] Kuleler arası izolasyon (her kule kendi `Backpack` instance'ını sahipleniyor)
- [x] Dalga spawn sistemi (`WaveManager`)
- [x] Kale/can sistemi + game over/pause/restart/ana menü akışı

---

## ⚠️ Çözülmesi gereken çelişki: Rotasyon

GDD 2.1: *"Eşyalar bu matris üzerinde belirli bir alan kaplar ve **stratejik rotasyona (döndürme) tabidir**."*

Backpack sistemini kurarken rotasyonu bilinçli çıkarmıştık. Bu GDD'de açıkça isteniyor — MVP'yi bloklamıyor (P1) ama geri eklenmesi gerekiyor:

- [ ] `PlacedItem`'a rotasyon state'i eklenmeli (0/90/180/270°)
- [ ] `ItemShapeUtility`'ye şekli döndürüp normalize eden bir fonksiyon lazım
- [ ] Sürükleme sırasında döndürme input'u (sağ tık / klavye tuşu / buton — karar verilmeli)
- [ ] `ShowPlacementPreview` döndürülmüş şekle göre hesaplanmalı

---

## Genişletilmiş roadmap (MVP sonrası için)

> Aşağıdaki modüller MVP checklist'ine değil, GDD'nin tamamına dayanıyor. MVP listesini bitirdikten sonra buraya dönüyoruz.

### 1. Faz Yönetimi (Hazırlık ↔ Savaş)
- [ ] `GamePhase` enum + merkezi faz yöneticisi
- [ ] Savaş fazında backpack/shop etkileşiminin kilitlenmesi
- [ ] Hedefleme tipi seçiminin bu kilitten muaf tutulması
- [ ] Dalga bitince otomatik hazırlık fazına dönüş + dondurulmamış dükkanın ücretsiz yenilenmesi
- [ ] Oyun hızı kontrolü (1x/2x/3x)

### 2. Hedefleme Sistemi
- [ ] `TargetingType` enum: Kara / Hava / Yakın / Uzak Öncelikli
- [ ] Kule arayüzünde manuel seçim UI'ı
- [ ] `Tower.Shoot()`'taki `enemies[0]` seçimini targeting type'a göre değiştirme
- [ ] Hava/kara ayrımı için düşman tarafında uçuş flag'i gerekiyor
- **Karar gereken nokta:** "Uzak Öncelikli" — menzile göre en uzak mı, yolda en geride kalan mı?

### 3. Ekonomi Tamamlama
- [ ] Global Freeze: dükkan dondurma butonu + Auto-Reroll muafiyeti
- [ ] Ceza oranlı geri satış: `G_iade = G_maliyet × R_oran` (0.50–0.60 arası)

### 4. Element / Evrim Sistemi
- [ ] `ElementType` enum'un tamamı (Nötr, Ateş, Su... + "Neutral Enemy")
- [ ] XP kazanımı + seviye 5'te Element Sınıfı Seçim Ekranı
- [ ] Kule Tipi Değişimi: envanter boşalmadan buton pasif
- [ ] Element Tipi Değişimi: ana silah satılınca shop pool yenilenmesi
- [ ] Element Değişim Enflasyonu: kule başına kalıcı, katlanan %10 fiyat çarpanı (Tower instance alanı, ScriptableObject'e yazma)
- [ ] Sinerjisiz item'ların ızgarada görsel olarak işaretlenmesi

### 5. Kule Kalıtım Mimarisi — Barracks
- [ ] `UnitTower`'ın `Tower` mimarisine bağlanması (backpack, shop, leveling)
- [ ] Izgaradaki statların spawn edilen `Unit`'e miras aktarımı

### 6. Castle HQ
- [ ] Kale geliştirme ağacı + ortak gold ile satın alma
- [ ] Global pasif buff/debuff sistemi

### 7. Savaş Analiz (Pause) Sistemi
- [ ] Kule başına toplam hasar/DPS takibi
- [ ] Izgaradaki eşyaların tetiklenme frekansı/verim takibi
- [ ] Kule–düşman tipi etkinlik analizi

### 8. Bestiary & Ustalık Kitabı
- [ ] Düşman unlock + "Öğren" butonu + mini görev
- [ ] Görev ilerleme takibi + kalıcı pasif ödül

### 9. Bölüm / Wave Yapısı
- [ ] 20 bölüm, her 5 bölümde tema/harita/düşman ailesi değişimi
- [ ] Wave ölçekleme (logaritmik artış formülü)
- [ ] Bölüm geçiş sistemi
