# SkillData sistemi — kendi yeteneğini ekleme ve kod açıklaması

15 Ekim 2026. Okçu 11 etkin, diğer türler 12'şer taslak olmak üzere 47 yetenek ayrı assetlere taşındı. Önceki okçu kimlikleri, bonusları, puanları ve kayıtları korundu. Aşağıdaki akış eski kod içindeki ArcherSkills dizisinin yerini alır.

## 1. Yeni data oluştur

Unity Project penceresinde `Assets/Resources/Data/Skills` altında ilgili klasöre git: Archer (okçu), Magic (büyücü), Mortar (havan), Unit (birlik).

Sağ tık → **Create → Backpack → Skills → Skill Data**. Örneğin `GucluYay.asset` adını ver.

| Alan | Ne gireceksin? |
|---|---|
| Id | Yeni assette otomatik üretilen kalıcı kimlik. Oyuncular kullanmaya başladıktan sonra değiştirme. |
| Tower Type | ArrowTower = okçu, MagicTower = büyücü, CannonTower = havan, UnitTower = birlik. |
| Display Name | Örneğin Güçlü Yay. |
| Description | Oyuncuya gösterilecek açıklama. Elle yazılır; etki değerini değiştirdiğinde açıklamayı da güncelle. |
| Icon | Sprite sürükle. |
| Point Cost | Gereken yetenek puanı. |
| Available | Açık: satın alınabilir. Kapalı: taslak, satın alınmaz ve buff vermez. |
| Prerequisites | Önceden açılması gereken SkillData assetleri. Hepsinin açılması gerekir; kök için liste boş. |
| Effects | Bir veya birden çok etki. Type ve ilgili sayıları seç. |

**Örnek:** Tower Type = ArrowTower, Point Cost = 2, Available açık. Effects listesine bir eleman ekle, Type = DamagePercent, Value = 5 seç. Prerequisites'e `archer.training.asset` sürükle. Description = “Bütün okçu kulelerine +%5 hasar.”

Değerlerde `5` yüzde beş demektir; eski ItemData alanlarının bazılarındaki `0.05` biçimini burada kullanma.

Yeni asseti Create ile oluşturmak yeni Id üretir. Ctrl+D ile kopyalarsan Id de kopyalanır: yeni kopyaya benzersiz bir Id vermelisin. Aynı kimlikli assetler satın alınamaz ve etkileri uygulanmaz. Mevcut assetin yalnız dosya adını değiştirmek kaydını bozmaz; Id'yi değiştirmek eski açılımla bağını koparır.

## 2. Datayı UI'a sürükle

1. `Assets/Prefabs/UI/SkillTreeUI.prefab` prefabını aç.
2. Hierarchy'de kök **SkillTreeUI** objesini seç.
3. Inspector'daki **SkillTreePanelUI** bileşeninde örneğin **OKÇU AĞACI — SkillData sürükle** bölümünü aç.
4. **Düğümler / Nodes** listesine bir eleman ekle.
5. Yeni SkillData'yı bu elemanın **Skill** alanına sürükle.
6. **Position** alanında düğümün merkezini ayarla. X sağa, Y aşağı büyür. Orta alan için yaklaşık X=80–720, Y=50–510 aralığını kullan; düğümler arasında en az 146×80 boyutunu hesaba kat.
7. Ön koşul datalarının da aynı ağaç listesinde olduğundan emin ol.
8. Play'e girip ağacı aç. İsim, ikon, açıklama, fiyat, tıklama ve ön koşul bağlantıları datadan kurulur.

UI Inspector'ındaki bu liste **ortak SkillTreeData assetini** düzenler. Bu nedenle hem ana menü hem oyun içi ağaç aynı içeriği kullanır. Yeni bir tree referansı atarsan “Ağaç Listelerini Yenile” düğmesine bas. Düğüm eklemek için yeni Button üretmen veya onClick bağlaman gerekmez. 12 düğüm sınırı kaldırıldı; daha fazla düğüm gerektiğinde mevcut görsel şablon çoğaltılır. Panel alanı sabittir; çok sayıda düğümü konumlandırırken okunabilirliği korumalısın. Görsel önizleme Play sırasında oluşturulur.

Alternatif olarak `Assets/Resources/Data/SkillTrees/Archer.asset` gibi ağacı doğrudan açıp aynı Nodes listesini düzenleyebilirsin. Ağaç başlığı, alt başlığı, rengi ve kule türü burada bulunur. Dört mevcut UI sekmesi sırasıyla Archer/Magic/Mortar/Unit assetlerine bağlıdır.

## 3. Desteklenen etkiler

| Type | Alanlar ve davranış | Desteklenen kule |
|---|---|---|
| DamagePercent | Value=5 → +%5 hasar | Hepsi; birlikte asker hasarı |
| AttackSpeedPercent | Value=5 → +%5 hız, atış aralığı kısalır | Hepsi |
| CriticalChancePoints | Value=3 → +3 yüzde puan kritik şansı | Hepsi; TowerData kritik sınırı korunur |
| RangePercent | Value=5 → +%5 menzil; gerçek collider ve stat paneli güncellenir | Okçu, büyücü, havan |
| UnitHealthPercent | Value=10 → askerlerin maksimum canına +%10; yaşayan askerlerin can oranı korunur | Birlik |
| SameTargetHitSpeed | Value=isabet başına hız yüzdesi, Count=en fazla birikim. Hedef değişince sıfır | Okçu |
| OuterRangeDamage | Value=ek hasar yüzdesi, Threshold=menzilin yüzde kaçından sonra. 50 dış yarıdır | Okçu |
| LowHealthDamage | Value=ek hasar yüzdesi, Threshold=hedefin kalan can yüzdesi. 30, %30 altıdır | Okçu |
| EveryNthAttackCritical | Count=kaçıncı saldırı kesin kritik. Value kullanılmaz | Okçu |

Mesafe/can koşulları atış anında değerlendirilir. Özel okçu etkileri diğer kulelere yanlışlıkla seçilirse data hatası gösterilir; sessizce satın alınıp etkisiz kalmaz. Birlik menzili desteklenmez; birliklerin yol üzerindeki düşman yakalama alanı farklı bir mekanizmadır. Yeni bir davranış (örneğin seken ok) için etki türü ve onu uygulayan kod eklemek gerekir. Mevcut türlerde yeni yetenek üretmek kod gerektirmez. Ek ok davranışı eklenmedi; eşyalara ayrılmış durumda.

## Kodun görev dağılımı

### SkillData.cs

İki yeni ScriptableObject'ten biri. İsim/ikon/açıklama/kule türü/fiyat/etkiler/ön koşulların tek kaynağıdır. Effect iç sınıfı Type, Value, Count ve Threshold taşır. ConfigurationError kimlik/isim, negatif/geçersiz değer, desteklenmeyen kule-etki eşleşmesi ve ön koşul döngülerini kontrol eder. Id boşken OnValidate benzersiz kimlik üretir. Asset oyuncu ilerlemesini tutmaz.

### SkillTreeData.cs

Ağaç başlığı, rengi ve kule türünü; Node listesinde SkillData referansı ile konumu saklar. ConfigurationError boş düğüm, başka türden data, yinelenen kimlik veya listede olmayan ön koşulu tespit eder. Bozuk ağacın satın alımı kapatılır; düzenleme tamamlandıktan sonra yeniden açılabilir. Ön koşul için dizideki sıra değil asset referansı kullanılır; düğüm sırasını değiştirmek ön koşulları bozmaz.

### SkillTreeProgress.cs

ArcherSkills sabit içerik dizisi kaldırıldı. Catalog, Resources/Data/Skills altındaki SkillData assetlerini bulur. UnlockError verilen ağacı ve datayı, katalogda tek kimlik bulunmasını, aktifliği, ön koşulları ve bakiyeyi kontrol eder. TryUnlock işlem öncesi kaydı yeniden okur; puan düşer, sabit kimlik açılır ve satın alma makbuzu (id, ödenen puan, kule türü) eklenir. Böylece sonradan fiyat veya yerleşim değişse bile ücretsiz sıfırlama gerçekten ödenen miktarı iade eder. Tree'den çıkarılmış bir düğümün makbuzu da sıfırlamada iade edilir.

Eski v1 JSON kayıt anahtarı korunur. Eski 11 okçu kimliğinin tarihsel 1/2 puan maliyeti yalnız kayıt göçü için saklanır; savaş hesabı bunları kullanmaz. Önceden açılmış bu yeteneklere eksik makbuzlar bellekte eklenir, sonraki kayıt işleminde JSON'a yazılır. Diğer bakiye, bölüm rekorları ve açılımlar korunur. ResetTree kule türüne ait makbuzları temizler; üst üste sıfırlama tekrar puan vermez.

ActiveEffects, açılmış ve Available olan doğru kule türündeki geçerli assetlerin etkilerini döndürür. Bir düğümü sadece ağacın görünümünden çıkarmak daha önce kazanılmış etkisini silmez; Available kapatmak etkisini devre dışı bırakır, ücretsiz sıfırlama da açılımı kaldırır. Assetleri silmek yerine taslağa almak düzenleme sırasında daha anlaşılırdır.

### Tower.cs ve ArcherTower.cs

Ortak Tower sınıfı CalculateStats sırasında aktif etki listesini okur. Önce temel seviye/eşya/sinerji hesabı yapılır, sonra data yüzdeleri uygulanır. Aynı türün yüzdeleri toplanır: iki %5 hasar etkisi toplam %10 olur. Hasar ve hız bonusları eşya sonrası değerlere uygulanır. Birlik maksimum canı önceki eşya çarpanı ile yetenek çarpanını birlikte kullanır. Yeni kule başlangıçta kayıtlı etkileri alır; SkillTreeProgress.Changed olayı mevcut kuleleri anında günceller. Etkiler her kare Resources'tan okunmaz.

Base OnDisable temizliği artık virtual'dır; ArcherTower base.OnDisable çağırır. Böylece hem olay aboneliği hem hedef/coroutine listesi düzgün temizlenir. CurrentRange üç saldırı kulesinin data menzil bonusunu içerir; collider ve menzil gösterimi bu değeri kullanır.

ArcherTower'da `Has("archer...")` kontrolleri kaldırıldı. İsabet serisinin büyüklüğü/sınırı, uzak hedef ve düşük can eşikleri, kesin kritik aralığı artık Effect değerlerinden okunur. Seri sürümü ve eski mermiyi reddetme koruması korunur. Aynı anda farklı seri etkileri varsa her biri kendi Count sınırına kadar katkı yapar.

### SkillTreePanelUI.cs ve SkillTreePanelUIEditor.cs

Runtime panel seçilen SkillTreeData'nın Nodes listesini gösterir. İhtiyaç kadar görsel düğüm oluşturur, tıklamayı doğru indekse bağlar ve SkillData.prerequisites referanslarından eğri çizgiler üretir. Çizgiler sekme açıldığında oluşturulur, her kare yeniden çizilmez. Oyuncu maliyetleri/görünümü değiştirse bile eski isim/ikon dizileri davranışı belirlemez; eski serialized alanlar yalnız geçmiş kurulum aracı uyumluluğu için tutulur.

Editor sınıfı yalnız Unity Inspector rahatlığı içindir; oyunda çalışmaz. UI bileşeninin Inspector'ında her ağacın Nodes alanını, ilgili assetin SerializedObject'i üzerinden açar. Yani sürüklediğin data prefabın içinde gizli ikinci bir listeye kopyalanmaz; doğrudan ortak ağaç assetine atanır. İki yeni runtime data sınıfı dışında yeni manager eklenmedi.

## Kontroller

`Docs/Validation/skill-data-2026-10-15.json`: 47 asset/4 ağaç, eski kayıt/kimlik/bakiye, tarihsel iade, maliyet değişince gerçek ödemenin iadesi, döngü/yanlış kule koruması, okçunun gerçek mermi/isabet/istatistik kontrolleri ve 47 düğüm metin/seçim kontrolü geçti. 13. düğüm otomatik oluşturuldu; katalog dışında datanın satın alınması reddedildi.

`Docs/Validation/skill-data-authoring-2026-10-15.json`: özel Inspector oluşturuldu. Geçici yeni datalarla büyücü, havan ve birlikte %7 hasar, %9 hız, +4 kritik puanı; saldırı kulelerinde %8 menzil ve birlikte %13 can uygulandı, sıfırlamayla geri alındı. Console temiz. Geçici test assetleri temizlendi ve gerçek oyuncu kaydı geri yüklendi.

Son görüntü `Docs/Validation/skill-data-runtime-2026-10-15.png`. Tam bölüm denge testi yapılmadı. Test için verilen puanlar oyuncuya bırakılmadı. Unity test sonunda Boot sahnesine döndürüldü, Play kapatıldı.
