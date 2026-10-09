# Sinerji, evrim ve teslim alma — 8 Ekim 2026

Onaylanan mimari uygulandı. Mevcut GameManager korundu. Eşyalar ayrı saldırmıyor; sinerji yalnız bonusu alan eşyanın kuleye katkısını artırıyor. İlk sürümde evrim süresi bir başarılı dalgadır; çok dalgalı ilerleme sayacı yoktur.

## Oyuncu nasıl kullanır?

Çantadaki eşyanın üzerine gelince özel sinerji kareleri gösterilir. Sarı kare uygun komşu bekler; yeşil karede kabul edilen eşya vardır. Özel kareler R/fare tekerleğiyle eşya döndükçe döner. Bir kuralın iki karesi de dolsa bonus iki katına çıkmaz. Depo ve teslim listesi bonus vermez.

Tarifteki iki eşyayı çantada kenardan komşu yerleştir. Çapraz temas yetmez. Panelin üstünde oluşacak ürün görünür; eşya bilgi paneli tarif partnerini de yazar. Wave başladığında seçilen çiftler kaydedilir. Başarıyla bitince malzemeler tüketilir ve ürün oluşur. Son dalga da dahildir; mağlubiyet/iptal evrim tamamlamaz.

Ürün için önce malzemelerden boşalan alan dahil çanta aranır. Gerekirse dört yön denenir. Çantada yer yoksa aynı kulenin deposu aranır. Orada da yer yoksa ürün, deponun altındaki teslim listesine gider. Liste grid kapasitesi tüketmez. `>` ile sıradaki ürüne geçilir; ikon çantaya/depo gridine sürüklenir. Teslim ücretsizdir; başarısız bırakma ürünü listede tutar. Teslim ürününü satmak için önce bir gride yerleştirmen gerekir.

Ana item satılamaz, depoya konulamaz ve evrim malzemesi/ürünü olarak kullanılamaz. Tarifler farklı kuleler arasında eşya kullanamaz.

## Eklenen örnek içerik

| Kule | Malzemeler | Ürün | Özel kare filtresi |
|---|---|---|---|
| Büyü | Çırak Asası + Mana Özü | Mana İşlemeli Asa | İksir türü |
| Okçu | Avcı Yayı + Çevik Eldivenler | Çevik Avcı Yayı | Yalnız Çevik Eldivenler |
| Birlik | Lejyoner Kılıcı + Muhafız Kemeri | Muhafız Kılıcı | Herhangi bir eşya |

Her örnek kaynak eşyanın sağ tarafında iki özel kare bulunur. Uygun yerleşim +0,08 hasar katkısı sağlar (8 yüzde puanı). Evrim ürününde malzemelerin temel katkıları toplanır; üstüne +0,15 hasar ve +0,05 saldırı hızı eklenir. Kritik/can katkıları korunur. Ürün maliyeti malzemelerin toplamıdır; nadirliği Rare'dir. İlk sürüm ikonları kaynak eşyanın ikonunu kullanır. Ürünler mağaza havuzuna eklenmedi, tarifle kazanılır.

Örnek: 30 temel hasarlı büyü kulesinde Çırak Asası +%22, Mana Özü +%12 ve asanın etkin sinerjisi +%8 sağlar. Sonuç `30 × (1 + 0,22 + 0,12 + 0,08) = 42,6` olur. Sinerji, diğer eşyaların katkısını çarpmaz. Bu çiftin ürünü +%49 hasar verir; aynı temel kulede 44,7 hasar oluşur.

## Kodun görev dağılımı

### ItemData ve ItemSynergyCalculator

ItemData.synergyRules tasarım verisidir. Her kuralda yerel kare koordinatları, filtre ve dört stat katkısı bulunur. Bunlar Inspector'dan değiştirilebilir. ItemBonus yalnız dört değeri taşıyan ve toplayan küçük bir veri yapısıdır; yeni bir yönetici değildir.

ItemSynergyCalculator.Cells, özel kareyi item şeklinin boyutuna göre döndürür, sonra yerleşimin Origin değerini ekler. Şeklin içinde olan veya kenardan komşu olmayan yanlış özel kareleri hesaba katmaz. IsActive bu karelerde kabul edilen komşu olup olmadığını kontrol eder. Additional her etkin kuralın katkısını yalnız bir kez toplar.

Tower.CalculateStats mevcut temel item bonuslarına bu sonucu ekler. Paylaşılan ItemData asset'inin damageBonus gibi değerleri değiştirilmez. Böylece bir kuledeki yerleşim aynı itemi taşıyan diğer kuleyi etkilemez. Bir eşyanın sinerji sonucu başka eşyanın bonus hesabına beslenmez; döngüsel artış oluşmaz.

### ItemEvolutionRecipe ve TowerItemEvolution

ItemEvolutionRecipe bir ScriptableObject'tir: first, second, result. Assetler `Assets/Resources/Data/Evolutions` altındadır. Her kule prefab'ının evolutionRecipes listesinde ilgili tarifler vardır. Mevcut üç kule prefab'ına birer örnek tarif bağlandı.

Tower.Start, her kule için ayrı TowerItemEvolution nesnesi oluşturur. Bu nesne çanta/deponun sahibi olan Tower'a bağlıdır; global liste yoktur. FindPairs önce Inspector tarif sırasını, sonra hücre sırasını kullanır. used kümesi aynı malzemenin iki eşleşmede seçilmesini engeller. İki malzeme aynı ItemData olsa bile ayrı PlacedItem örnekleri olmak zorundadır.

BeginWave eşleşmelerin o dalgaya ait listesini saklar. CompleteWave yalnız ilgili dalga için çalışır; işlem listesini önce kapatır, sonra malzemeleri tüketir. Bu sayede tekrar gelen tamamlama ikinci ürün oluşturmaz. Yeni oluşan ürün, aynı dalgada başka tarifin malzemesi olmaz. CancelWave yalnız planı siler, malzemeyi tüketmez.

WaveManager geçerli dalga başlamadan hemen önce BeginWave, son düşman temizlenince CompleteWave çağırır. Tamamlama çağrısı zafer kontrolünden önce olduğu için son dalga da işler. StopWaves iptal eder. GameManager'ın mevcut hazırlık/savaş/sonuç görevleri değiştirilmedi.

### Teslim işlemi ve arayüz

Delivery her ürün için ayrı bir kayıt nesnesidir; aynı ItemData'dan iki ürün listede ayrı tutulur. BackpackDraggable teslim kartını sürüklerken kaydı silmez. Yerleşim uygun olduğunda Claim yalnız o kaydı bir kez kaldırır, ardından grid'e yerleştirir. Eski mağaza ücretlendirmesi bu yola uygulanmaz. Pause, panel kapatma ve kule değişiminde kayıt korunur. Teslim görseli işlem sonrası mevcut panel yenileme yoluyla yeniden oluşturulur.

BackpackUIManager hesaplayıcının sonuçlarını gösterir: özel kare renkleri, evrim çiftleri ve teslim kartları. InfoPanelUI eşyanın filtre/bonus açıklamasıyla tarifini yazar. Eklenen alanlar mevcut birleşik panel üzerindedir; ayrı bir envanter yöneticisi eklenmedi.

## Inspector'dan yeni içerik ekleme

1. ItemData içindeki Synergy Rules listesine kural ekle. Cells için item'in sol üstünü (0,0) al; örneğin 1 hücre genişliğindeki bir asanın sağ karesi (1,0), sol karesi (-1,0) olur.
2. Filter seç: Any, Type veya SpecificItem. İlgili kabul alanını doldur. Bonus değerleri kesirdir: 0,08 = 8 yüzde puanı katkı.
3. Yeni ürün ItemData'sını hazırla. Create → Backpack → Evolution Recipe ile tarifi oluştur; iki malzemeyi ve ürünü bağla.
4. İlgili kule prefab'ının Evolution Recipes listesine tarifi ekle. Aynı malzemeler birden fazla tarifte kullanılabiliyorsa listedeki önceki tarif önceliklidir.

## Doğrulama ve sınırlar

- Sinerji/evrim hesabı: **20/20**. Üç filtre, dört yön, kuralın tek sayımı, komşuluk, tek tüketim, iptal, depo/teslim taşması, ana item koruması ve stat izolasyonu.
- Teslim arayüzü: **6/6**. Başarısız bırakma, pause, kule değişimi, satışa bırakma, ücretsiz yerleşim ve tekrarlanan bırakma.
- Gerçek WaveManager callback akışı: **2/2**. Normal dalga sonrası hazırlık ve son dalga sonrası zafer. Bu iki testte düşman sayısı sıfır yapılarak dalga tamamlama sınırına hızlıca gidildi.
- Mevcut oyun regresyonu: **28/28**. Satın alma, taşıma, reroll, savaş, zafer, restart ve yenilgi.
- Ekran görüntüsü incelendi; son konsol kontrolü hatasız. Testler sentetik sürükleme olayları kullanır; uzun süreli oyuncu denge testi değildir.

Raporlar `Docs/Validation/synergy-evolution-2026-10-08.json`, `evolution-delivery-2026-10-08.json`, `evolution-wave-2026-10-08.json`, `evolution-final-wave-2026-10-08.json` ve `stage1-latest.json`. Test kuleleri, geçici ürünler ve dolu gridler yalnız Play oturumunda oluşturuldu. Oturum kapatıldı; arka plan çalışma ayarı eski haline getirildi.
