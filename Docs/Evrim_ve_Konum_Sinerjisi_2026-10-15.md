# Evrim, konum sinerjisi ve takılabilir parçalar

15 Ekim 2026 — Onaylanan mimari uygulaması.

## Oyunda iki ayrı sistem

**Evrim:** ItemEvolutionRecipe içindeki iki eşya çantada yan yana durur. Dalga başlangıcında eşleşme hazırlanır; başarılı dalga sonunda hâlâ geçerliyse iki eşya tüketilir ve sonuç oluşur. Sonuç çantaya sığmazsa depoya, orası da doluysa bekleyen teslimlere gider. Bu zamanlama değiştirilmedi. Takılı parçalar evrimde ikinci malzeme olarak sayılmaz.

**Sinerji:** ItemSynergyRecipe belirli bir eşyaya, başka bir eşyanın konumuna veya üzerine takılmasına göre bonus verir. Birleşme ve dalga bekleme yoktur. Koşul bozulduğunda bonus kalkar. ItemData assetinin değeri değiştirilmez; yalnız o yerleştirilmiş eşyanın katkısı hesaplanır. Aynı kılıcın başka bir kopyası kendiliğinden bonus kazanmaz.

Mevcut oyunda eşyalar ayrı ayrı saldırmıyor: kuleye stat katkısı veriyor. Dolayısıyla “kılıç +5 hasar kazansın” bu kılıcın kuleye katkısına +5 eklenmesi demektir. Ayrı eşya saldırı sistemi eklenmedi.

## Hazır örnek

`Assets/Resources/Data/SynergyRecipes/LegionSword_Crown.asset`

- Hedef: Lejyoner Kılıcı.
- Sağlayıcı: Lejyon Tacı.
- Konum: kılıcın döndürülmemiş şeklinin en sağ sütununun hemen dışındaki üst kare.
- Alternatif: tacı doğrudan kılıcın üzerine bırakıp takmak.
- Etki: +5 sabit hasar, en fazla bir etkinleşme.

Bunlar birlik kulesine uygun eşyalardır. Test için okçuya zorla koyman gerekmez. Kılıç ve tacın kendi mevcut eşya bonusları korunur; bu tarif onların üzerine eklenir.

Kılıç döndürülürse gerekli kare de aynı yönde döner. Tacın kapladığı karelerden birinin tarif karesine denk gelmesi yeterlidir. Aynı sağlayıcının birden fazla karesi denk gelse bile tek eşya sayılır.

## Inspector üzerinden yeni tarif oluşturma

1. Project penceresinde `Assets/Resources/Data/SynergyRecipes` klasörüne gir.
2. Sağ tık → Create → Backpack → Synergy Recipe.
3. `Target Item`: bonusu alacak eşyanın ItemData assetini sürükle.
4. `Filter`: belirli iki eşya için `SpecificItem` seç.
5. `Support Item`: bonusu sağlayacak eşyanın ItemData assetini sürükle.
6. `Placement`: aşağıdaki üç seçenekten birini seç.
7. `Cells`: gerekiyorsa hedef eşyanın yerel kare koordinatlarını gir.
8. `Max Activations`: kaç ayrı sağlayıcının bonusunun toplanabileceğini belirle. Başlangıç için 1.
9. `Description`: oyuncuya koşulu ve etkisini anlatan açıklamayı yaz.
10. `Bonus`: istediğin statları doldur.
11. **Hedef ItemData içindeki `Synergy Recipes` listesine oluşturduğun tarifi ekle.** Yalnız klasörde asset oluşturmak yetmez. UI'ya bağlaman gerekmez; hedef ItemData üzerinden okunur.

| Placement | Koşul |
|---|---|
| Cells | Sağlayıcı tarif karelerinden birinde durur. |
| Attached | Sağlayıcı hedefin üzerine takılır. Cells kullanılmaz. |
| CellsOrAttached | İki yoldan biri geçerlidir; aynı tarifin etkinleşme sınırı ortaktır. |

`Filter = Any` bütün eşyaları, `Type` seçilen ItemType değerini kabul eder. Bu alanlar eski genel kuralların taşınması için de korunmuştur. İki belirli eşya tasarlarken SpecificItem kullan.

Koordinat başlangıcı hedefin döndürülmemiş şeklinin sol üstüdür: X sağa, Y aşağı artar. Örneğin tek sütunlu 1×2 kılıç `(0,0)` ve `(0,1)` kaplıyorsa üst sağ komşu `(1,0)`, alt sağ komşu `(1,1)` olur. Hedefin kendi dolu kareleri konum tarifi için yok sayılır. Üzerine takmak için Cells'e aynı kareyi yazmak yerine Attached seçilir.

## Bonus değerleri ve hesap sırası

| Alan | Örnek | Anlamı |
|---|---|---|
| Flat Damage | 5 | +5 sabit hasar |
| Damage | 0.05 | +%5 hasar katsayısı |
| Attack Speed | 0.05 | Mevcut hız hesabına +%5 katkı |
| Critical Chance | 0.05 | Kritik olasılığına +5 yüzde puanı |
| Health | 0.05 | Birlik canı hesabına +%5 katkı |

**SkillData'dan farklı olarak bu tarifin yüzde alanlarında 5 değil, 0.05 yazılır.** SkillData sistemi değiştirilmedi.

Hasar hesabının ilgili bölümü:

`(seviyeye göre temel hasar × (1 + eşya ve sinerji yüzde bonusları) + sinerji sabit hasarı) × skilltree hasar çarpanı`

Örnek: temel hasar 20, toplam eşya bonusu %10, bu tarif +5 ise skilltree öncesi hasar 27 olur. Skilltree +%5 veriyorsa 28.35 olur. Kritik ve düşman elementi gibi savaş çarpanları mevcut saldırı akışında ayrıca uygulanır. Böylece +5 hasar, skilltree nedeniyle ekrandaki son toplamı tam 5'ten fazla artırabilir.

Tarif yalnız hedef ve sağlayıcı o kulenin tür/element uyumuna uygunsa çalışır. Takılı parçanın kendi normal statları da bir kez eklenir; hedef veya parça uyumsuzsa parça katkısı uygulanmaz. Depodaki hiçbir eşya veya takılı parça stat vermez.

## Takma, çıkarma ve eşya güvenliği

Uygun sağlayıcıyı hedef eşyanın üzerine bırak. Ayrı grid alanı tüketmez; hedefin köşesinde küçük bir ikon olarak görünür. Bu ikonu sürükleyerek çıkarabilir, başka geçerli konuma koyabilir veya normal eşya gibi satabilirsin.

- Hedefi taşımak/döndürmek parçaları da taşır.
- Hedef depoya geçerse bütün grubun bonusu kesilir.
- İptal/geçersiz bırakma eski konumu, yönü ve takılma ilişkisini geri getirir.
- Tarifin limiti doluysa başka parça takılamaz. Yan taraftaki sağlayıcılarla birlikte toplam bonus da Max Activations ile sınırlıdır.
- Üzerinde parça bulunan bir eşyayı başka eşyanın üzerine takmak desteklenmez; iç içe takma zinciri yoktur.
- Ana eşya başka bir eşyanın üzerine takılamaz, satılamaz, depoya taşınamaz. Ana eşya kendi tarifleri varsa parça taşıyabilir.
- Hedef satılırsa yalnız hedefin satış bedeli alınır; takılı parçalar depoya, doluysa teslim listesine gider.
- Hedef evrimde tüketilirse veya ana eşya element değişiminde yenilenirse parçaları yine korunur.
- Kule türü değişiminde satılan çanta eşyalarının parçaları korunur. Depo ve teslimler yeni kuleye geçer.
- Mağazadan doğrudan hedefe takma normal satın almadır. Başarılıysa ücret bir kez kesilir; reddedilirse kesilmez.

## Kodun görev dağılımı

**ItemSynergyRecipe.cs:** Tasarım verisi. Hedef, sağlayıcı filtresi, hücreler, takılma modu, etkinleşme limiti ve bonus. Tek yeni oyun sınıfı budur.

**ItemData.cs:** `synergyRecipes` listesi hedefin hangi tariflere sahip olduğunu belirtir. Eski `synergyRules` alanı göç güvenliği için gizli tutulur, aktif hesapta kullanılmaz. 21 eski kural ayrı assetlere taşındı; böylece eski ve yeni veri aynı bonusu iki kez vermez.

**ItemSynergyCalculator.cs:** Saf hesap yapar. Önce hedefin gerçekten çantaya ait olduğunu ve uyumunu kontrol eder. Tarif hücrelerini dönüşe göre çevirir, uygun komşuları ve takılı parçaları toplar. HashSet aynı sağlayıcıyı veya tekrar eklenmiş tarif referansını çift saymayı önler. Sonuç Max Activations ile sınırlanır. Asset veya çanta değiştirmez.

**PlacedItem.cs:** Bir eşyanın çantadaki gerçek kopyasıdır. Data, konum, dönüş yanında Bag, Parent ve Attachments tutar. Bir ItemData'dan iki kılıç oluşturulduğunda her birinin takılı parçaları ayrıdır.

**Backpack.cs:** Grid ve sahiplik işlemlerini yönetir. Place yeni eşya oluşturur; PlaceExisting aynı nesneyi taşır. Bu ayrım parçaların kaybolmasını önler. TryAttach ilişkiyi kurar; Remove takılı parçayı çıkarır veya kök eşyayı grid ile birlikte kaldırır. Parçalar bağımsız grid hücresi doldurmaz.

**BackpackDraggable.cs:** Sürükleme başında eşyanın gerçek nesnesini, kaynak çantayı, varsa ebeveynini saklar. Bırakmada önce geçerli takma hedefini, sonra normal yerleşimi dener. Mağaza ücreti ve teslim talebi işlem başarısına bağlıdır. İptalde kaynak ilişkiyi geri kurar.

**BackpackUIManager.cs:** Küçük parça ikonlarını üretir; her biri kendi drag bileşenine sahiptir. Tarif hücrelerini ve takma önizlemesini gösterir. Aynı model hem çanta hem depo için kullanılır.

**Tower.cs:** Uyumluluğu ve toplam statları hesaplar. Çanta kökleri ve uygun takılı parçaların normal katkılarını, hedeflerin sinerji bonuslarını toplar. Depoyu hesaba katmaz. Element/tür dönüşümünde parça kurtarma çağrısını yapar.

**TowerItemEvolution.cs:** Eski evrim zamanlamasını sürdürür. Yeni ReleaseAttachments ve StoreOrDeliver yardımcıları parçaları güvenli yere aktarır. Delivery artık yalnız ItemData değil PlacedItem içerir; teslim beklerken de nesne kimliği korunur.

**HoverInfoController.cs / InfoPanelUI.cs:** Yerleştirilmiş eşyanın gerçek kopyasına bakar. Aktif sabit/yüzde sinerjiyi, takılı olduğu hedefi ve depoda bonus verilmediğini açıklar. Mağaza açıklaması yalnız data üzerinden gösterilir.

## Doğrulama ve sınırlar

Unity Play Mode testleri geçti:

- Dört dönüş, koşul kalkınca bonusun kalkması, tekrar tarif referansının çift sayılmaması.
- Gerçek sürükleme metotlarıyla takma/çıkarma, iptal, geçersiz bırakma, depo, satış ve mağaza ücreti.
- Stat hesabında +5 ve depoda sıfır katkı; yerleştirilmiş eşyanın bilgi metni.
- Bir dalga sonunda evrim, dolu depoda parça/sonuç teslimi, tekrar tamamlamada çift üretimin engellenmesi ve teslim alma.
- Ana eşya element dönüşümü, kule tipi değişimi, dolu depodan yeni kuleye teslim devri ve doğru satış bakiyesi.
- Parça ikonunun Game View görüntüsü incelendi; Console hata/uyarı vermedi.

Kanıtlar: `Docs/Validation/synergy-runtime-2026-10-15.json`, `synergy-migration-2026-10-15.json`, `synergy-transitions-2026-10-15.json`, `synergy-attachment-2026-10-15.png`. Tekrar çalıştırılabilir kodlar `Tools/Validation/test_synergy_runtime.*`, `test_synergy_transitions.cs` ve `preview_synergy.py` içindedir.

Bunlar kontrollü çalışma zamanı testleridir; bütün bölümlerin oynanış dengesi veya paketlenmiş build testi değildir. Test sonunda Play kapatıldı, Boot açıldı; kullanıcı yetenek kaydı korundu. Bu görev oyun içi envanter için yeni kalıcı kayıt sistemi eklemez.
