# Dalga arası hazırlık, eşya kilidi ve dondurma — 4 Ekim 2026

## Kararlaştırılan davranış

Emir: ilk ve sonraki hazırlıklar 60 saniye; savaşta kule satın alma açık; dondurma yalnız seçili kulenin dört dükkan slotunu kapsar. Mevcut GameManager/WaveManager görev dağılımı korunur. Yeni sınıf eklenmedi.

Oyun açılır → 60 saniye hazırlık → Başlat veya süre sonu → tek dalga üretimi → son düşmanın temizlenmesini bekleme → sonraki hazırlık. Son dalganın son düşmanı temizlenirse Victory; kale düşerse Defeat. Sonuçtan sonra yeni dalga veya hazırlık açılmaz.

Hazırlıkta alışveriş, taşıma, ücretli yenileme ve dondurma kullanılabilir. Pause ve savaşta eşya işlemleri kilitlidir. Kule satın almak savaşta da mümkündür. Bilgi panelleri ile çanta/dükkan içeriği incelenebilir; yalnız işlem girişleri kilitlenir.

## GameManager.cs

- `autoStartDelay`: varsayılan ve GameScane Inspector değeri 60. Önceden sahnede kayıtlı 30 değeri de güncellendi; yalnız C# varsayılanını değiştirmek yeterli olmazdı.
- `CanEditItems`: aktif GameManager Preparation durumunda ve pause kapalıysa true. İşlem girişleri aynı kurala bakar. Yeni manager eklemek yerine mevcut GameManager'ın Awake/OnDestroy içinde tuttuğu referans kullanılır.
- `StartGame`: WaveManager yeni dalga başlatabiliyorsa açık sürüklemeyi iptal eder, Combat'a geçer, zamanı açar ve mevcut TryStartWaves çağrısını yapar. Tekrarlı Başlat Combat durumunda geri döner.
- `BeginPreparation`: yalnız Combat'tan gelir. Zamanı durdurur, kalan süreyi 60'a ve otomatik başlatma bayrağını başlangıca döndürür. ShopUIManager üzerinden ücretsiz dükkan yenilemesini çağırır.
- Sayaç unscaledDeltaTime ile çalışır; hazırlıkta timeScale=0 olsa da azalır. IsPaused kontrolü duraklatmayı sağlar.
- Pause veya oyun sonucu açılırken eldeki sürükleme geri alınır.

## WaveManager.cs

Eski `hasStarted` bütün bölüm için tek seferlikti. Artık `waveRunning` yalnız mevcut dalganın üretim/temizlenme süresini kapsar. `CanStartNextWave`, aynı dalga sürerken veya bütün dalgalar bittiğinde yeni başlangıcı engeller.

SpawnWave yalnız bir dalga üretir. Üretim aralarında mevcut spawnInterval kullanılır; son düşman üretildikten sonra fazladan bir spawn aralığı beklenmez. Ardından WaitUntil, pause kapalı ve Enemy etiketli nesne sayısı sıfır olana kadar bekler. Böylece üretimde kısa süreli boşluk oluşması dalgayı erken bitirmez.

Sonra indeks artar. Son dalgaysa GameOver(true), değilse gösterge sonraki dalgaya güncellenir ve GameManager.BeginPreparation çağrılır. Eski timeBetweenWaves alanı ve kendini otomatik tekrar başlatan coroutine kaldırıldı; dalga arası süreyi artık yalnız GameManager belirler.

## BackpackDraggable.cs ve BackpackUIManager.cs

`isDragging` sürüklemenin gerçekten başladığını, `dragOwner` eşyanın hangi kuleye ait olduğunu tutar. Statik activeDrag yalnız o anda elde olan görsele işaret eder. CanEditItems kontrolü başlangıçta ve bırakma anında yapılır; yalnız UI düğmesini kapatmaya dayanılmaz.

Taşımada mevcut sistem eski hücreleri sürükleme başında boşaltır. Bu yüzden `CancelActiveDrag` gerekir: yerleşik eşya eski origin'e geri konur, görseller yenilenir ve statlar hesaplanır. Dükkandan taşınan geçici görsel ise ücret kesilmeden silinir. Sonradan gelen OnEndDrag, isDragging=false olduğu için tekrar satın alma/yerleştirme yapmaz.

Panel kapanırken, başka kuleye geçerken ve ücretli reroll öncesinde de iptal çalışır. Böylece bir kulede başlayan sürükleme başka kulenin çantasına yazılmaz. GameManager savaşa geçmeden iptal eder; asker/kule statları savaş başlamadan geri yüklenir.

ShopSlotDragStarter savaşta geçici eşya görselini hiç oluşturmaz. Backpack modeline oyun durumu eklenmedi: yeni satın alınan kulenin başlangıç silahını savaşta da kurabilmesi gerekir. Kural oyuncunun işlem girişlerinde tutulur.

## Tower.cs ve ShopUIManager.cs

Her Tower kendi `IsShopFrozen` bilgisini tutar. ToggleShopFreeze yalnız hazırlıkta çalışır. ShopUIManager.ToggleFreeze bunu seçili kulede çağırır; diğer kulelerin durumunu değiştirmez.

`RefreshShopsForPreparation`, kurulumu tamamlanmış ve dondurulmamış kuleleri dolaşıp GenerateShopItems çağırır. Altın harcamaz. Açık panel varsa yeni teklifleri gösterir. Savaşta alınan ve Start işlemi henüz tamamlanmamış bir kule atlanır; zaten kendi Start'ında ilk tekliflerini üretir.

Dondurma kaldırılana kadar teklifler korunur. Dondurulmuş dükkandan hazırlıkta satın alma serbesttir; alınan slot boş kalır. Ücretli reroll için önce dondurmayı kaldırmak gerekir. Yenileme düğmesi para yetersizken, dondurmada, savaşta ve pause'da etkisizdir; RefreshShop metodu da aynı kuralları doğrular. Dondurma düğmesi ve panel altındaki durum mesajı oyuncuya mevcut durumu gösterir.

## Doğrulama ve sınırlar

Canlı Play Mode'da iki kısa test dalgası kullanıldı. Son düşman bekleme, pause sırasında faz değiştirmeme, sonraki hazırlıkta 60 saniye, manuel/otomatik başlatma, çift başlangıç koruması ve son dalga zaferi doğrulandı. Savaşta kule satın alma çalıştı; eşya alımı/taşıma/reroll engellendi. Açık eşya taşıması ve dükkan sürüklemesi savaş başlangıcında kayıpsız iptal edildi.

Seçili kuleyi dondurma, diğer kuleyi etkilememe, dondurulmuş dükkandan satın alma, boş slot/teklif kimliklerinin dalga sonrası aynen korunması, diğer kulenin ücretsiz yenilenmesi ve dondurmayı kaldırınca ücretli reroll kontrol edildi. 1920×1080 görüntü incelendi: `tmp/wave-preparation-freeze.png`.

Kontroller mevcut metotlara ve sentetik pointer olaylarına dayanır; fiziksel fareyle tam bölüm dengeleme testi veya build testi değildir. Test dalgaları ve test kuleleri sahneye kaydedilmez. Önceki kaynaklar ve sahne `Backups/2026-10-04-wave-loop` altında.

Ek kontrol: 60 saniyelik sayaç normal Update ile gerçek zamanlı çalıştırıldı; Başlat çağrılmadan süre sıfıra indi ve testin son dalgası zaferle tamamlandı. Yeniden başlatmada ilk hazırlığın 60 saniye ayarı korunduğu doğrulandı. Yenilgi testi ilk denemede hareketli düşman sayısı beklentisi nedeniyle başarısız oldu; hareketi durdurulmuş düşmanla tekrarlandığında kalan iki spawn'ın iptal edildiği ve StartGame/BeginPreparation çağrılarının Defeat durumunu değiştirmediği doğrulandı.

Son kontrol: Console hata/uyarı döndürmedi. Play kapalı, sahne dirty=false, süre 60, asıl dört dalga korunmuş, test kulesi 0 ve runInBackground=false.

Projede eşya satışı ve döndürme henüz yok. Bu özellikler eklenirken CanEditItems kuralına bağlanmalı; bu adımda satış/rotasyon sistemi tamamlanmış sayılmaz. Kule yükseltme izni, oyun hızı düğmeleri ve hedefleme seçenekleri ayrıca kararlaştırılacak.
