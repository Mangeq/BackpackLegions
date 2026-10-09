# Kule hedeflemesi — 5 Ekim 2026

## Oyuncu davranışı

Okçu veya büyücü kulesinin dükkanı açılınca üstünde iki sıra görünür. İlk sıra İkisi de / Kara / Hava; ikinci sıra İlk giren / Son giren seçer. Birlik kulesinde bu panel görünmez. Her kulenin seçimi kendine aittir ve panel kapatılıp açılınca korunur; yeni oyun yeni kulelerle varsayılan İkisi de + İlk giren seçimine döner. Kalıcı kayıt sistemi eklenmedi.

Örnek: A kara, B hava, C kara düşmanı sırayla menzile girsin. Kara + İlk giren A'yı; Kara + Son giren C'yi; Hava seçeneklerinin ikisi de B'yi seçer. A çıkıp geri girerse sıra B, C, A olur. Bu, düşmanın kaleye uzaklığına veya yol üzerinde ne kadar ilerlediğine bakmaz.

Hazırlık ve savaş sırasında seçim değişebilir. Pause ve sonuç ekranında değiştirilemez. Seçim değişince sonraki atış yeni kurala uyar; saldırı bekleme süresi sıfırlanmaz ve havadaki mermi hedef değiştirmez. Uygun düşman yoksa atış yapılmaz.

## Kodun görevleri ve akışı

### EnemyData.cs — Düşman türü

`MovementType` enum'u `Ground = 0`, `Air = 1` değerlerini taşır. `movementType` varsayılanı Ground olduğu için mevcut düşman verileri kara kabul edilir. Inspector'da Air seçmek yalnız hedefleme sınıfını değiştirir; NavMesh veya uçuş davranışı oluşturmaz. Bu adımda bölümün düşmanları ve denge değerleri değiştirilmedi.

### Tower.cs — Seçim ve menzil sırası

`SelectedTargetType` ve `SelectedTargetOrder` paylaşılan TowerData üzerinde değil, kule nesnesinde tutulur. Bu nedenle bir okçuyu Hava yapmak diğer okçuların seçimini değiştirmez. `SupportsManualTargeting` yalnız ArcherTower ve mevcut adıyla MegicTower için true döner. Setter metotları geçersiz indeksleri ve birlik kulelerini reddeder.

`OnTriggerEnter`, collider'ın üst nesnelerinde Enemy arar. `enemies` listesi ilk giriş sırasını tutar. `enemyColliders` sözlüğü hangi collider'ın hangi düşmana ait olduğunu kaydeder: aynı düşmanın ikinci collider'ı yeni düşman gibi eklenmez. `OnTriggerExit`, düşmanın son collider'ı da çıktığında onu listeden çıkarır. Yeniden giriş liste sonuna eklenir.

`SelectTarget`, yok edilmiş/devre dışı collider kayıtlarını temizler; aktif olmayan veya menzilde collider'ı kalmayan düşmanları listeden çıkarır. Listeyi İlk giren için baştan, Son giren için sondan gezer. Canı sıfır olanları ve seçili türle uyuşmayanları atlar. İlk uygun düşmanı döndürür; uygun hedef yoksa null döner.

`Shoot` coroutine'i her atış öncesi `SelectTarget` çağırır. Hedef yoksa bir sonraki kareyi bekler; hedef varsa mevcut alt sınıfın `Attack` metodunu çağırır ve `currentAttackSpeed` kadar bekler. Okçu/büyücü devre dışı kalınca coroutine ve menzil kayıtları temizlenir. Birlik kulesinin üretim coroutine'lerine müdahale edilmez.

### UİManager.cs — Düğmeler ve izinler

`targetingPanel`, üç `targetTypeButtons` ve iki `targetOrderButtons` Inspector referansı eklendi. Düğmeler `SelectTargetType(int)` ve `SelectTargetOrder(int)` çağırır. Bu metotlar önce oyun fazı, pause ve seçili kuleyi kontrol eder; sonra kuledeki seçimi değiştirir. Tür indeksleri 0 İkisi de, 1 Kara, 2 Hava; sıra indeksleri 0 İlk giren, 1 Son giren şeklindedir.

`RefreshTargetingControls` panel görünürlüğünü, düğme etkileşimini ve seçili düğmenin altın renk vurgusunu günceller. Seçim UI'da saklanmaz; aktif kuleden okunur. Böylece kuleler arasında geçiş doğru seçimi gösterir. Eşya düzenleme kilidi bu seçimlere uygulanmaz.

### BackpackUIManager.cs — Panel dışına tıklama

Yeni hedefleme paneli dükkanın üstüne uzanır. Eski kontrol yalnız ana panel dikdörtgenine baktığından bu düğmeleri dışarıda sayabilirdi. `IsPointerOverPanel`, dikdörtgen dışında kalan tıklamada EventSystem raycast sonucunun en üstteki nesnesinin aynı panelin çocuğu olup olmadığına da bakar. Böylece hedef seçerken çanta kapanmaz.

### Sahne bağlantısı

`GameScane.unity` içindeki `Canvas/İtemShopUİ/TargetingPanel` canlı Unity Editor üzerinden oluşturulup kaydedildi. Beş düğmenin kalıcı event bağlantıları UİManager'a gider. Sahne dosyası elle düzenlenmedi. Başlangıçta panel kapalıdır; uygun kule seçildiğinde görünür.

## Doğrulama ve sınırlar

- 23 hedef seçimi kontrolü: iki kulede altı kombinasyon, çıkış/yeniden giriş, birden fazla collider, ölü hedef, uygun hava hedefi bulunmaması ve birlik kulesinin kapsam dışında kalması.
- 8 atış/UI kontrolü: gerçek oluşturulan mermilerin hedefleri, savaşta değişiklik, mevcut merminin hedefini koruması, pause, kule başına ayrı seçim ve birlik panelinin gizlenmesi.
- 1 düğme merkezi raycast kontrolü. İlk denemede RectTransform pivotu kullanıldığı için yanlış ekran noktası test edildi ve sonuç false oldu. Merkez noktasıyla tekrarlandığında düğme en üst hit olarak bulundu. İlk başarısız çıktı kanıt dosyasında korunur.
- Mevcut Aşama 1 otomasyonunda 28/28 kontrol geçti: satın alma, varsayılan silahlar, eşya taşıma/geri alma, dükkan, savaş kilitleri, zafer, yeniden başlatma ve yenilgi.
- 1920×1080 arayüz görüntüsü incelendi. Son Console sorgusunda hata/uyarı yok; Play kapalı, aktif sahne kaydedilmiş (dirty=false), test kuleleri yok.

Kontroller gerçek Unity Play Mode nesneleriyle yapıldı; giriş/çıkış metotları kontrollü collider'larla çağrıldı, düğme event'leri ve ekran raycast'i sentetik olarak çalıştırıldı. Fiziksel fareyle kullanıcı testi veya gerçek uçan düşman navigasyonu testi değildir. Hava örnekleri geçici runtime EnemyData kullanır; mevcut beş dalgada hava düşmanı bulunmaz.

Kanıtlar: `Docs/Validation/targeting-2026-10-05.json`, `Docs/Validation/stage1-latest.json`. Görüntü: `Docs/media/targeting-2026-10-05.png`. Değişiklik öncesi dosyalar: `Backups/2026-10-05-targeting`.
