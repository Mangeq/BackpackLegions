# Ana eşya evrimi ve element mağazaları — 9 Ekim 2026

> Aynı gün sonraki güncelleme: Ücretli element/kule tipi değiştirme artık uygulandı. Bu belge ilk uzmanlaşma adımını anlatır. Güncel devamı: [Kule/element değiştirme](Kule_Element_Degisimi_2026-10-09.md).

## Kesinleşen oyun kuralları

- Element seçimi ana eşyayı seçilen elementin karşılığına dönüştürür. Ana eşya aynı hücrelerde ve aynı dönüşte kalır; satılamaz, depoya konulamaz, birleşme tarifine harcanamaz. Bu dönüşüm hazırlıktaki seçim anındadır; iki eşyalı bir dalgalık craft ile aynı işlem değildir.
- Önceden satın alınmış normal eşyalar oyuncuda kalır. Çantadayken bonusları çalışır; oyuncu satmayı veya depolamayı kendisi seçer.
- **Depodaki hiçbir eşya hasar, hız, kritik, can veya komşuluk bonusu vermez.** Depo yalnız saklama alanıdır.
- Emir'in seçimi: mağazanın mevcut teklifleri element seçilince değişmez. Satılmış slotlar boş kalır; dondurma korunur. İlk ücretli yenilemede veya dondurulmamış dalga arası yenilemede yeni element havuzu kullanılır. Dondurulmuş eski teklifler, dondurma kaldırılıp yenilenene kadar alınabilir.
- Yeni havuz yalnız ilgili kule türü ve seçilen elemente ait eşyaları sunar. Normal teklifler bu havuza karıştırılmaz.
- Elementi tekrar değiştirme ve değişim cezası bu adımın kapsamında değildir. İlk seçim hâlâ seviye 5 ve hazırlık kuralına bağlıdır.

## Onaylanan hasar tablosu

Döngü: **Su → Ateş → Hava → Toprak → Su**. Ok yönünde saldırı ×1,5, ters yönde ×0,5. Aynı element, karşılıklı olmayan eşleşmeler ve normal saldırılar ×1.

Satırlar saldıran kule, sütunlar düşmandır:

| Saldırı / Düşman | Normal | Ateş | Su | Toprak | Hava |
|---|---:|---:|---:|---:|---:|
| Normal | 1 | 1 | 1 | 1 | 1 |
| Ateş | 1 | 1 | 0,5 | 1 | 1,5 |
| Su | 1 | 1,5 | 1 | 0,5 | 1 |
| Toprak | 1 | 1 | 1,5 | 1 | 0,5 |
| Hava | 1 | 0,5 | 1 | 1,5 | 1 |

Eski Normal kimlikli EnemyData dosyalarındaki özel weakness/resistance listeleri uyumluluk için korunur. Böyle bir özel liste varsa Normal sütunundaki varsayılanın yerini o liste alır. Element kimliği bulunan düşmanda tek kaynak bu döngüdür; eski listeler ikinci kez çarpılmaz.

Örnek bölümde Keşifçi Normal, Akıncı Ateş, Muhafız Toprak, Gök Perisi Hava, Elit Su, Savaş Lordu Ateş olarak bağlandı. Can, hız, hasar, XP ödülü ve dalga adetleri değiştirilmedi. Hava **hareket türü** ile Hava **elementi** farklı alanlardır; bir yer düşmanı da Hava elementine sahip olabilir.

## Kodun görev dağılımı

### ItemData ve TowerData: tasarım verisi

`ItemData.element` mağaza filtresidir. Tek başına sahip olunan eşyanın bonusunu kapatmaz. Eski normal eşyaların çalışması bu sayede korunur.

`TowerData` başlangıç eşyasının yanında `fireWeapon`, `waterWeapon`, `earthWeapon`, `airWeapon` referanslarını tutar. `GetMainItem(element)` ilgili asset'i verir. `IsMainItem(item)` beş ana eşya karşılığını tanır; böylece ana eşya değiştiğinde eski `item == defaultWeapon` koruması boşa düşmez. Her kule aynı tasarım asset'ini okuyabilir, ancak seçim instance üzerindedir; ortak asset çalışma sırasında değiştirilmez.

### Tower ve Backpack: güvenli dönüşüm

`Tower.MainItem`, mevcut seçime göre aktif ana eşya verisini döndürür. `TrySelectElement` şu sırayla ilerler:

1. Hazırlık, pause, seviye ve element kontrolleri.
2. Evrilmiş eşyanın veri kontrolü. Eksikse seçim yapılmaz; panel hata nedenini gösterir.
3. Devam eden sürüklemeyi iptal etme. Ana eşya geçici olarak grid dışındaysa yerine döner.
4. Çantadaki mevcut ana eşyayı bulma.
5. `Backpack.TryReplace`: önce yeni şeklin bütün hücrelerini kontrol eder; yalnız eski eşyanın hücrelerini boş kabul eder. Sınır veya başka eşyayla çakışma varsa **hiçbir kayıt değişmez**. Uygunsa aynı konum ve dönüşte yeni PlacedItem yerleştirilir.
6. Seçilen elementi kuleye yazma, bekleyen XP'yi işleme, statları ve mevcut askerleri güncelleme.
7. Açık çanta/mağaza görsellerini yenileme. Mağaza teklif dizisi yeniden üretilmez.

Mevcut 12 ana eşya evriminin şekli başlangıç eşyasıyla aynıdır. Bu yüzden tamamen dolu çanta da dönüşümü engellemez. Gelecekte Inspector'dan yanlış şekil verilirse eşya kaybetmek yerine güvenli hata alınır.

`CalculateStats` yalnız Backpack.Items üzerinden hesap yapmayı sürdürür. Storage bu hesaba ve komşuluk aramasına girmez.

### ShopManager ve ShopUIManager: yeni teklifler

`GetRandomItem(towerType, rarity, element)` üç koşulu birlikte filtreler. Varsayılan element Normal olduğu için mevcut çağrılar bozulmaz. Kule yeni teklif üretirken kendi SelectedElement değerini yollar. Yanlış element veya başka kule türüne geri düşme yoktur; içeriksiz bir havuz boş döner.

UI eski teklifleri gösteriyorsa “Sonraki yenileme: Su eşyaları” gibi bir bilgi yazar. Yenilemeden sonra “4 teklif · Su mağazası” gösterir. Ücret ve dondurma mevcut akışla çalışır.

### EnemyData, Enemy ve Bullet: gerçek saldırı

`EnemyData.element` kimliktir; `WeakTo`, `ResistantTo` ve `GetDamageMultiplier` aynı döngüyü kullanır. `Enemy.TakeDamage` çarpanı buradan alır, gerçek can kaybını ve XP katkısını mevcut yöntemle hesaplar. Örneğin kritik dahil 100 hasarlık su saldırısı ateş düşmanına 150 hasar verir; düşmanın yalnız 20 canı kaldıysa XP katkısı 20 olur.

Mermi seçilen elementi ateşleme anında alır. Elementli merminin rengi MaterialPropertyBlock ile değişir; ortak materyal değiştirilmez. Yaşayan ve yeni doğan askerler kendi kulelerinin elementini kullanır. Hareket, atış sıklığı veya yeni durum etkisi eklenmedi.

### Ana eşya korumaları ve gösterim

`BackpackDraggable` satış/depolama kontrollerinde `Tower.IsMainItem` kullanır. `TowerItemEvolution` ana eşyanın bütün karşılıklarını tariflerden dışlar. Ana eşya evrimi ile iki eşyanın birleşmesi birbirine karışmaz.

Panel seçimi “Su Büyücüsü”, “Hava Okçusu” gibi gösterir. Hover paneli eşyanın ve düşmanın elementini, doğru zayıflık/direnci ve aktif ana eşyanın ikonunu okur. Uzayan mağaza eşya adları için mevcut metin alanlarında yazı boyutu sınırları düzenlendi.

## Eklenen ilk içerik ve sayısal ayarlar

`Assets/Resources/Data/ElementsV1` altında her element için:

- Üç kuleye ait **10'ar mağaza eşyası**: toplam 30; dört elementte 120 yeni teklif verisi.
- Üç ana eşya: asa, yay, kılıç; toplam 12 evrim karşılığı.
- Mevcut üç craft tarifinin element karşılıkları; toplam 12 tarif ve 12 sonuç eşyası. Mevcut normal tarifler de korunur. Tarifler aynı elementin iki belirli eşyasını kullanır; karma element tarifi tanımlanmadı.

Mağazada toplam 150 veri vardır: 30 normal + 120 elementli. Aynı anda hepsi gösterilmez; üç filtre uygulanır. Ana eşyalar ve craft sonuçları mağaza listesine eklenmedi. Topçu hâlâ içeriksizdir.

Bu ilk içerik mevcut şekilleri, ikonları ve temel bonusları temel alır; 120 bağımsız görsel veya 120 yeni eşya mekaniği değildir. Elementli eşya adında element etiketi bulunur. Yeni özel ikon/model/animasyon seti üretilmedi; mermide element rengi kullanılır.

İlk denge: elementli mağaza versiyonu mevcut eşyaya göre +5 yüzde puanı hasar bonusu; fiyat mevcut fiyatın %110'u, yukarı doğru 5 altına yuvarlanır. Ana eşya evrimi +10 yüzde puanı hasar bonusu verir ve ücretsizdir. Elementli craft sonucunda iki malzemenin toplam +10 puanlık ek bonusu korunur. Kapsamlı denge testi sonraya bırakıldı.

Inspector'da eşyaların `element`, `damageBonus`, `cost` alanlarını; TowerData'nın dört ana eşya referansını; düşmanın `element` alanını düzenleyebilirsin. Kod tarafında hasar çarpanlarının tek kaynağı EnemyData.GetDamageMultiplier'dır.

## Doğrulama

- 26 özellik kontrolü geçti: 12 kule/element dönüşümü, tamamen dolu grid, hatalı/eksik veri, eski eşyaların korunması, depodaki bütün bonusların dışlanması, mağaza filtreleri, 25 hasar eşleşmesi, benzersiz ID'ler ve su craft tarifi.
- 13 gerçek sahne/UI kontrolü geçti: sürüklerken seçim, eski teklifler, satılmış slot, dondurma, ücretli yenileme, evrilmiş ana eşyanın satış/depo engeli, yaşayan/yeni asker ve gerçek büyücü/okçu atışları.
- XP'nin 26 regresyon kontrolü yeni ana eşya gereksinimine uyarlanan test verisiyle yeniden geçti.
- Aşama 1 satın alma/envanter/dalga/sonuç regresyonu 28/28 geçti; Console hata/uyarı yok.
- Kontroller otomatik/sentetik çağrılarla yapıldı; fiziksel fareyle baştan sona oyun testi veya kapalı test dengesi yerine geçmez.

Raporlar `Docs/Validation/element-behavior-2026-10-09.json`, `element-ui-2026-10-09.json`, `xp-regression-2026-10-09.json` içinde. Kurulum C# parçaları `Tools/Validation/setup_element_content.cs` ve `setup_enemy_elements.cs`; editör üzerinden çalışırlar. İçerik kurulumunu tekrar çalıştırmak mevcut klasör varsa reddedilir. Kaynak, sahne, kule verisi/prefab ve düşman verisi yedekleri `Backups/2026-10-09-elements` altındadır.

Panel görüntüsü: [Su Büyücüsü](Validation/water-mage-panel-2026-10-09.png).

