# Havan kulesi — 9 Ekim 2026

Emir'in onayladığı davranış ve iki sınıflı görev dağılımı uygulandı. Yeni manager eklenmedi. Mevcut `CannonTower` enum değeri korunuyor; oyuncuya **Havan Kulesi**, kodda davranış sınıfı **MortarTower** gösteriliyor.

## Oyunda nasıl çalışıyor?

Boş kule yerinden sağdaki havan ikonunu seçip 350 altına satın alabilirsin. Yalnız hazırlıkta alınır. Yetersiz altın ve dolu yer kontrolleri mevcut satın alma akışındadır.

Havan yalnız kara düşmanlarını hedefler. İlk giren / son giren seçimi değiştirilebilir. Kara/hava/ikisi düğmeleri havanda kilitlidir; Kara seçili kalır. Kod üzerinden hava veya ikisi seçimi de kabul edilmez.

Mermi hedefin **ateş anındaki konumuna** gider. Hedef daha sonra hareket ederse mermi onu takip etmez. Vuruşta patlama alanında kalan kara düşmanlarının her biri bir kez hasar alır. Başlangıç hedefi ölse bile mermi aynı noktaya düşer; oraya gelen başka kara düşmanları da hasar alabilir. Uçan düşmanlar patlamadan etkilenmez.

| TowerData alanı | Başlangıç | Anlamı |
|---|---:|---|
| damage | 70 | Her isabet alan düşmana temel hasar |
| attackSpeed | 3 sn | Atışlar arası süre |
| range | 6 | Hedef alınabilecek dünya yarıçapı |
| explosionRadius | 1,5 | Merminin düştüğü noktadaki patlama yarıçapı |
| mortarFlightTime | 0,25 sn | Mesafeden bağımsız uçuş süresi |
| mortarArcHeight | 0,8 | Düz uçuş çizgisinin üzerindeki en yüksek ek yükseklik |
| criticalChance | %10 | Atış başına kritik ihtimali |
| mortarCriticalMultiplier | 2 | Kritik atış hasar çarpanı |
| cost | 350 altın | Satın alma fiyatı |

Bu değerleri `Assets/Resources/Data/TowerData/MortarData.asset` Inspector'ından değiştirebilirsin. Seviye, eşya ve sinerji hasar/atış aralığı/kritik değerlerini diğer kulelerdeki gibi değiştirir. Patlama yarıçapına özel eşya bonusu eklenmedi.

## Kodun sorumlulukları

### 1. Tower.cs: ortak kule kuralları

Havan `SupportsManualTargeting` kapsamına alındı. Böylece mevcut menzil trigger'ı, collider giriş sırası, saldırı coroutine'i ve hedef seçimi kullanılıyor. Yeni `SupportsAirTargets` özelliği havan için false döner. Başlangıçta hedef türü Kara olur; hem seçim hem hedef bulma sırasında hava engellenir.

Çanta, depo, mağaza dondurma, seviye, XP, element ve ücretli tür değişimi aynı sınıfta kaldı. Her kule ayrı runtime çanta/depo tutar. Menziğin collider yarıçapı model ölçeğine göre düzeltilerek dünya biriminde 6 tutulur.

### 2. MortarTower.cs: tek atış oluşturma

`Start` hazırlık sırasında sekiz mermi ve bunların efektlerini oluşturur; kullanılmayan mermiler pasif tutulur. `Attack(target)` canlı kara düşmanını doğrular, havuzdaki boş mermiyi seçip `Muzzle` noktasına taşır ve etkinleştirir. O andaki hedef konumu, güncel hasar, kritik şansı ve kule sahibini verir. Sekiz mermi de meşgulse atış kaybetmemek için havuz büyür. Gerçek mermi hareketini bu sınıf yapmaz. Havuz bu kulenin çocuklarıdır; kule değişiminde/silinmesinde birlikte temizlenir.

### 3. MortarProjectile.cs: uçuş ve patlama

`Awake` renderer listesini, renk bloğunu ve tek bir patlama efektini hazırlar. Bunlar her atışta tekrar oluşturulmaz. `Initialize` önceki atışın süre/patlama/hedef kayıtlarını sıfırlar, gizlenmiş mermi görselini açar; başlangıç/bitiş noktalarını, uçuş süresini, yay yüksekliğini, patlama yarıçapını, elementi ve hasarı kopyalar. Bu nedenle mermi uçarken kule statı/elementi değişse bile önceki atışın niteliği değişmez. Kritik bir kez, atış başında hesaplanır; aynı patlamadaki düşmanlara ortak temel hasar gider.

`Update` ilerlemeyi `Time.deltaTime` ile artırır. Dolayısıyla pause ile durur, oyun hızıyla hızlanır. Konum hesabı:

`Lerp(başlangıç, bitiş, t) + yukarı × 4 × yükseklik × t × (1 − t)`

Burada `t` 0–1 arasındadır. Ek yükseklik başlangıç ve bitişte sıfır, ortada `mortarArcHeight` kadardır.

`Explode` önce `exploded` bayrağını açar; aynı mermi ikinci kez hasar veremez. `Physics.OverlapSphereNonAlloc` patlama alanındaki collider'ları önceden ayrılmış 64 elemanlı tamponda bulur. Tampon dolarsa iki katına büyütülüp sorgu tekrarlanır; kalabalıkta hedef kaybolmaz. Her collider'ın üst objelerinde Enemy aranır; kuleler, uçanlar ve ölüler elenir. Yeniden kullanılan `HashSet<Enemy>` aynı düşmanın birden fazla collider'ından gelen tekrarları engeller. Kesişim collider yüzeyine göre yapılır; bu nedenle iri bir düşmanın gövdesi alana giriyorsa merkezi dışarıda olsa da vurulabilir.

Hasar `Enemy.TakeDamage(hasar, element, owner)` üzerinden gider. Mevcut Enemy kodu her düşmanın element çarpanını ayrı uygular ve ölümde gerçek hasar payından XP hesaplar. Örneğin Su havanının 70 hasarı Ateş düşmanına 105, Toprak düşmanına 35 olur; kritik atışta iki katına çıkar.

Patlamada mermi görseli gizlenir ve önceden hazırlanmış kısa efekt oynatılır. Efekt bittiğinde `Release` sahip referansını ve vuruş listesini temizleyip mermiyi pasifleştirir; sonraki atışta aynı nesne kullanılır. Efektte `StopAction.None` kullanılır, silinmez. Test amacıyla havuz dışında oluşturulan mermiler ise efekt bitince silinir. Mermi rengi seçilen elemente göre değişir; ortak materyal değiştirilmez.

## Boyut, isabet ve performans düzeltmesi

Emir'in oyun testinden sonra aynı gün düzeltildi:

- Modelin alt obje ölçeği 2'den 1'e indirildi: genişlik 1,2 → 0,6 dünya birimi. Fizik gövdesi ve namlu noktası buna uyarlandı; 6 birim saldırı menzili değişmedi.
- Eski 0,9 sn uçuşta hız 4 olan gerçek NavMesh akıncısına yapılan tek atış hasar vermedi. Aynı senaryoda 0,25 sn uçuşla 70 hasar doğrulandı. Mermi hâlâ atış anındaki konuma gider; takip veya kesin isabet eklenmedi. Yay yüksekliği 0,8'e indirildi.
- İlk sabit hedef testi bu hareketli hedef sorununu yakalamamıştı. Artık `mortar_moving_probe.cs` gerçek yol takibiyle kontrol ediyor.
- Efekt 12 adet 768 üçgenli küre yerine 6 adet 12 üçgenli küçük parça kullanır: 9.216 → 72 üçgen. Gölge üretme/alma ve ışık probları kapalıdır. Bu geometri karşılaştırmasıdır; FPS artışı yüzdesi değildir.
- Aynı sekiz mermi/efektle 100 atış yapıldı; nesne sayısı büyümedi. 80 hedefli yoğun patlamada tampon büyümesi hiçbir hedefi atlamadı. İlk kullanım/tampon büyümesi tahsis yapabilir; normal tekrar kullanımında yeni fizik sonuç dizisi/renk bloğu oluşturulmaz.
- 12 optimizasyon ve 46 çekirdek kontrolü geçti. Hareketli hedef, pause, çoklu collider, hava ayrımı, element, XP ve dönüşüm kontrolleri dahil. Console temiz. Donanım üzerinde kontrollü FPS karşılaştırması yapılmadığı için kasılmanın tüm olası kaynaklarının bittiği iddia edilmiyor.

### 4. UİManager.cs ve InfoPanelUI.cs: mevcut arayüz

`MortarTowerPrefab` ve `mortarPurchasePortrait` referansları eklendi. Var olan havan ikonu artık satın alma kartını doldurur; eski “Yakında” etiketi kaldırıldı. Kart, birleşik panel ve hover bilgisinde patlama yarıçapı gösterilir. Tür değiştirme ekranına dördüncü seçenek eklendi; mevcut ücret/onay/çanta boşaltma kuralları aynen uygulanır.

Havana dönüşüm yeni fiyat − eski fiyatın %50'sidir. Örneğin okçudan havana 350 − 125 = 225 altın; havandan okçuya 250 − 175 = 75 altın. Depo/teslimler korunur, seviye/XP/element sıfırlanır.

## Eşyalar ve içerik

`Assets/Resources/Data/MortarV1` altında 60 ItemData ve 5 tarif bulunur:

- 10 Normal ve her element için 10 eşya: toplam 50 mağaza teklifi.
- Normal + dört element Havan Namlusu: 5 korunan ana eşya.
- Normal + dört element Güçlendirilmiş Barut: 5 birleşme sonucu.
- Barut Kesesi + Topçu Eldivenleri yan yana bir başarılı dalga geçirince ilgili elementin Güçlendirilmiş Barut eşyasına dönüşür.

İlk içerik mevcut okçu eşyalarının fiyat, şekil, bonus ve nadirlik dağılımından **ayrı asset kopyaları** olarak üretildi. Kule sahibi ve tarif/sinerji referansları havan kopyalarına yönlendirildi. Okçu asset'leri değiştirilmedi. Genel mağaza listesi 150'den 200'e çıktı; her kule/element/nadirlik havuzu dolu.

Model projedeki mevcut cannon modelidir. Ana eşya ve silah ikonlarında havan görseli, destek eşyalarında mevcut ikonlar kullanılır. Bunlar ilk sürüm görselleridir; özel yeni ikon seti üretilmedi. Büyük denge çalışması yapılmadı.

## Kontroller

- 46 çekirdek + 13 entegrasyon kontrolü geçti: çoklu collider, yalnız kara, alan dışı hedef, hareket ederek kaçma, kavis/pause, kritik, element/XP, mağaza, ana eşya, tarifler, depo, satın alma ve tür değişimi.
- 26 mevcut XP + 26 element + 28 temel oyun kontrolü geçti. Eski testlerdeki “topçu havuzu boş” ve “150 mağaza eşyası” beklentileri yeni içerikle güncellendi; filtreler artık dört kulede doğrulanır.
- Canlı fizik/Update/coroutine akışında 26,7 oyun saniyesinde 9 atış × 70 = 630 hasar gözlendi. Test sonunda eski patlama efekti kalmadı.
- Satın alma kartı ve birleşik panel Game View'da incelendi; Console hata/uyarı vermedi.

Test kaynakları `Tools/Validation/test_mortar.cs` ve `test_mortar_integration.cs`; sonuçlar `Docs/Validation/mortar-*.json`. Bu kontroller kapalı testte yapılacak oyun dengesi değerlendirmesinin yerine geçmez.
