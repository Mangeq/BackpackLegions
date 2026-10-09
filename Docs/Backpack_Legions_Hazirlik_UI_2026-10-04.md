# Hazırlık ve kule satın alma arayüzü — 4 Ekim 2026

Bu adım mevcut GameManager ve UİManager içinde uygulandı. Yeni yönetici sınıfı eklenmedi. Sahne bağlantıları canlı Unity Editor üzerinden oluşturulup GameScane kaydedildi. Önceki dosyalar Backups/2026-10-04-preparation-ui altında.

## Oyuncunun gördüğü değişiklikler

- Düşman doğma noktası yanında kırmızı DÜŞMAN GİRİŞİ barı ve ilk ilerleme yönünü gösteren ok bulunur. Nokta ekran dışındaysa bar ekran kenarında tutulur.
- Kule seçiminde birlik için kılıç, okçu için yay, büyücü için asa altın çerçeveyle seçili kalır. Aynı anda tek seçim gösterilir; iptal ve satın almada temizlenir.
- Seçilen kuleye para yetmezse panelde kırmızı Yetersiz para! ve eksik altın tutarı görünür. Başarısız satın alma altın düşürmez ve slotu doldurmaz.
- İlk hazırlıkta 30 saniyelik sayaç görünür. Başlat'a basılmasa da süre sonunda savaş başlar. Erken Başlat mümkündür. Pause sayacı durdurur; yeniden başlatma süreyi sıfırdan kurar.

## GameManager: süre ve oyun durumu

`autoStartDelay` Inspector'da ayarlanabilir başlangıç süresidir; sahnede 30 saniye. `PreparationSecondsRemaining` dışarıdan okunabilen, GameManager tarafından değiştirilen kalan süredir. Awake bu değeri başlangıç süresine eşitler.

Update, `TickPreparation(Time.unscaledDeltaTime)` çağırır. Hazırlıkta mevcut oyun Time.timeScale=0 kullandığı için normal deltaTime sıfır olurdu. unscaledDeltaTime gerçek kare süresiyle sayacı ilerletir. Ayrı IsPaused kontrolü sayesinde menüden pause açıldığında yine durur.

TickPreparation yalnız Preparation durumunda çalışır. Süreyi sıfırın altına indirmeden azaltır; sıfırda mevcut StartGame metodunu çağırır. `autoStartAttempted` aynı otomatik çağrının her karede yinelenmesini önler. Manuel Başlat başarılı olunca durum Combat olduğundan kalan süre ikinci bir başlangıç üretmez. WaveManager'ın mevcut tek başlangıç koruması da korunur.

Bu sayaç ilk hazırlık içindir. Dalga arası hazırlık döngüsü henüz uygulanmış değildir.

## UİManager: seçimi ve bilgiyi gösterme

`RefreshPurchaseFeedback` mevcut selectedTowerType değerini okur. Seçilen türün SelectionHighlight nesnesini açar, diğerlerini kapatır. Fiyatı prefab'ın TowerData.cost alanından alır; ayrı bir fiyat tablosu tutmaz. EconomyManager.Gold ile karşılaştırıp seçim/fiyat veya eksik altın mesajını üretir. Seçim, iptal ve satın alma sırasında hemen; altın değişimini takip etmek için Update içinde de yenilenir.

BuyTower'ın mevcut ödeme ve kule oluşturma akışı korunur. Yetersiz bakiye dalı mesajı yenileyerek return eder. Başarı veya iptal seçili türü temizler ve satın alma düğmesini seçim yapılana kadar kapatır. Awake seçim referanslarını temizler; önceki oyundan static seçim kalmaz.

Update kalan süreyi yukarı yuvarlayarak ekranda gösterir. Savaş başlayınca sayaç gizlenir, Başlat düğmesi etkisizleşir. Pause'da hazırlık mesajı duraklatıldığını belirtir.

## Düşman yönü

`CacheEntryDirection`, WaveManager.spawnPoint ile Base arasındaki NavMesh yolunun ilk anlamlı köşesine bakar. Yol bulunamazsa kaleye doğru vektörü kullanır. Mevcut sabit harita için yön Start'ta bir kez hesaplanır.

`UpdateEntryMarker`, dünya konumunu kamera üzerinden ekran ve Canvas konumuna dönüştürür. Barın kendi boyutunu hesaba katarak ekran kenarında 12 birim boşluk bırakır. Okun açısı ilk hareket vektörünün ekrandaki izdüşümünden hesaplanır. Kamera hareketi her karede hesaba katılır; rota veya spawn çalışma sırasında değişirse yönün yeniden hesaplanması ayrıca gerekir. Kazanma/kaybetmede gösterge gizlenir.

Yeni dekoratif UI elemanlarının raycastTarget değeri kapalıdır; alttaki düğmelere tıklamayı engellemezler. Hover paneli Canvas'ın üst çizim sırasında kalır.

## Kontroller

Canlı Play Mode'da 16 koşul geçti: üç ayrı seçim, yetersiz bakiye, iptal, tam fiyatla satın alma, ekran dışı giriş barı, pause, 30 saniye öncesi ve sonrası durum, çift başlangıç koruması, savaşta sayaç/düğme, restart, erken manuel başlangıç, erken başlangıç sonrası tekrar koruması ve sonuçta giriş göstergesi.

Sayaç sınırı özel TickPreparation metoduna kontrollü süre vererek sınandı; 30 saniyelik kesintisiz fiziksel oyuncu testi değildir. Satın alma ve diğer girdiler mevcut metotlar üzerinden çağrıldı. 1920×1080 Game View görüntüsü incelendi: tmp/preparation-ui.png. Build testi yapılmadı.

Kontrol sonunda Console hata/uyarı döndürmedi. Play kapalı, GameScane kaydedilmiş ve dirty=false, runInBackground=false; test kuleleri kalmadı.
