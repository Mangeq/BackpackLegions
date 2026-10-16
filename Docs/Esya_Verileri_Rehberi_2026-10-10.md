# Eşya verileri — 10 Ekim 2026

## Güncel klasör düzeni

```text
Assets/Resources/Data/
  Items/
    Shop/       # 200 mağaza eşyası
    Main/       # 20 ana eşya: dört kule × beş element durumu
    Evolved/    # 20 evrim ürünü
  EvolutionRecipes/ # 21 tarif; kullanıcının recipe_deneme tarifi dahil
  TowerData/    # Kule temelleri ve ana eşya bağlantıları
  EnemyData/    # Düşman özellikleri
  BulletData/   # Okçu/büyücü mermi ayarları
  MortarV1/     # Yalnız MortarShell.mat materyali korundu
```

Shop, Main, Evolved ve EvolutionRecipes altında `Normal`, `Fire`, `Water`, `Earth`, `Air` ayrımı var. Her elementte 40 mağaza eşyası, dört ana eşya ve dört evrim ürünü bulunur. Normal tarif sayısı kullanıcının eklediği deneme tarifi nedeniyle beştir; diğer elementlerde dörderdir.

Örnekler:
- Normal Çırak Asası: `Items/Shop/Normal/v1_apprentice_staff.asset`
- Su Çırak Asası: `Items/Shop/Water/v1_apprentice_staff_water.asset`
- Su büyücüsünün ana eşyası: `Items/Main/Water/main_magictower_water.asset`
- Havan Barut Kesesi: `Items/Shop/Normal/mortar_barut_kesesi_normal.asset`
- Gelişmiş havan eşyası: `Items/Evolved/Normal/mortar_guclendirilmis_barut_normal.asset`
- Tarifi: `EvolutionRecipes/Normal/recipe_mortar_guclendirilmis_barut_normal.asset`

## Veri bağlantıları

Klasörler düzenleme kolaylığı içindir. Çalışma mantığı değiştirilmedi:
- ShopManager.allItems hangi eşyaların mağazada çıkacağını belirler.
- TowerData.defaultWeapon/fireWeapon/waterWeapon/earthWeapon/airWeapon ana eşyaları belirler.
- Tower prefablarının evolutionRecipes listeleri kullanılabilen tarifleri belirler.
- Tarif first + second → result bağlantılarını tutar. Ürünün fiyatı/bonusu kendi ItemData dosyasındadır.
- ItemData.itemOwners ve element eşyanın uygunluğunu belirler. Normal eşya uygun kulede element değişiminden sonra da çalışır; farklı element eşyası etkisiz kalır. Depo bonus vermez.

## Inspector alanları

| Alan | Anlamı |
|---|---|
| Name | Oyuncuya görünen ad |
| Type | Eşya kategorisi; Potion seçmek tüketilebilir davranış eklemez |
| Rarity | Mağaza nadirlik seçimi; bonuslar otomatik artmaz |
| Item Owners | Uygun kuleler; CannonTower havandır |
| Element | Normal/Fire/Water/Earth/Air |
| Item ID | Metin kimliği; bu temizlikte korundu |
| Cost | Alış fiyatı; satış değeri max(0,cost)/2, aşağı yuvarlanır |
| Item Icon | Sprite |
| Item Shape | Sol üst (0,0) başlangıcına göre kaplanan hücreler |
| Center Offset | Sürükleme tutma hücresi |
| Damage Bonus | 0.20 = temel hasara %20 katkı |
| Attack Speed Bonus | 0.25 = atış aralığını 1.25'e böl |
| Critical Chance Bonus | 0.05 = kritik şansına 5 yüzde puanı |
| Health Bonus | Birlik canına oran; diğer kulelerde kullanılmaz |
| Synergy Rules | Hücre, filtre ve bonus kuralları |
| Synergy IDs | Eski kullanılmayan alan; kaldırılmadı |

ItemData ortak tasarım verisidir; her yerleşimin konumu ve dönüşü PlacedItem içinde ayrı tutulur. Hasar bonusları toplanır: temel 100, %20 ve %10 eşya → 130. Aynı sinerji kuralı birden fazla uygun komşu olsa da en fazla bir kez katkı verir. Description açıklamasını değiştirmek kuralı değiştirmez.

## Bu işlemde yapılanlar ve kod açıklaması

261 asset (240 eşya, 21 tarif) Unity AssetDatabase.MoveAsset ile taşındı. GUID korununca sahne, prefab, mağaza, ana eşya ve tarif referansları aynı asset'i göstermeye devam eder. Havanın eski okçu adlarını içeren 60 eşya dosyası ve beş tarif dosyası anlaşılır adlara çevrildi. `itemID` değerleri ve oyuncuya görünen adlar değiştirilmedi.

Beş eşyada boş açıklamalı, bütün bonusları sıfır ve hücreleri boş/kendi üzerinde olan etkisiz sinerji kayıtları kaldırıldı: Toprak ana okçu eşyası, Ateş Çevik Avcı Yayı, Yüzbaşı Zırhı, Kale Kalkanı ve Toprak Topçu Pelerini. Kullanıcının fiyat, şekil, bonus ve recipe_deneme düzenlemeleri korundu. Dolu bir tarif veya etkili sinerji kaldırılmadı.

Oyun sınıfları ve mimarisi değiştirilmedi. Assets dışında üç kontrol dosyası bulunur:
- `Tools/Validation/organize_item_data.cs`: bir defalık taşıma; önce GUID/eski-yeni yol/veri anlık görüntüsünü manifestte saklar, hedef çakışmasını kontrol eder, Unity API ile taşır. Manifest varken yeniden çalışmayı engeller.
- `Tools/Validation/verify_item_data.cs`: GUID/yol kontrolü; dosya adı ve beş etkisiz kural dışında tüm verinin aynı kaldığını karşılaştırır. İkon, şekil, kimlik, mağaza havuzu, ana eşya ve tarif bağlantılarını denetler. Oyun verisine yazmaz; rapor üretir.
- `Tools/Validation/test_migrated_recipes.cs`: Play Mode'da geçici kuleler ve geniş test çantalarıyla 21 gerçek tarifin komşuluk, dalga bekleme, ürün oluşumu ve tekrarlı tamamlamada çoğalmama davranışını kontrol eder. Test kulelerini temizler.

Eski içerik üretim araçlarına mevcut yeni klasör bulunduğunda durma koruması eklendi; eski klasörleri yanlışlıkla yeniden oluşturmamalılar. `evolution_wave_setup.cs` tarif yolu yeni konuma güncellendi. Geçmiş tarihli belgelerdeki eski yollar tarihsel kayıt olarak kalır; güncel yollar için bu rehber ve taşıma manifesti kullanılmalıdır.

## Doğrulama

- 651 veri/referans kontrolü geçti: 261 GUID/yol, veri karşılaştırması, ikon/şekil/kimlik, 80 kule-element-nadirlik havuzu, ana eşyalar ve prefab tarifleri dahil.
- 21 gerçek tarif Play Mode'da geçti; sentetik geniş çantada oyun metotları çağrıldı.
- 20 panel, sürükleme iptali, satış ve kule dönüşümü regresyon kontrolü geçti.
- Fiziksel fareyle baştan sona oynanmış bir bölüm veya build testi değildir.

Raporlar `Docs/Validation/item-data-*.json` içindedir. `item-data-migration-2026-10-10.json` eski-yeni dosya yollarını ve işlem öncesi veriyi içerir; geri dönüş/audit için korunmalıdır.
