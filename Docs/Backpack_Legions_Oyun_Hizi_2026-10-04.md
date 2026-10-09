# Oyun hızı — 1× / 2× / 3×

Emir'in onayladığı görev dağılımı korundu: GameManager hız değerini ve zamanı yönetir; UİManager düğmeler ile seçili hız vurgusunu gösterir. Yeni sınıf eklenmedi.

## GameManager

`SelectedSpeed` dışarıdan okunur, yalnız GameManager içinde değiştirilir. Her yeni GameManager 1 değeriyle başlar; static olmadığı için yeniden başlatılan oyuna önceki hız taşınmaz.

`CanChangeSpeed`, oyun Combat durumunda ve pause kapalıysa true döner. `SetGameSpeed(int speed)` önce bu izni ve 1–3 aralığını kontrol eder. Geçerli seçimde SelectedSpeed ve Time.timeScale birlikte güncellenir. Hazırlık, pause ve sonuç ekranında doğrudan metot çağrısı da hızı değiştiremez.

`StartGame` eskiden her dalgayı 1× başlatıyordu; artık Time.timeScale değerini SelectedSpeed'den alır. BeginPreparation zamanı sıfırlar ama SelectedSpeed'e dokunmaz. Böylece savaş 3× → hazırlıkta zaman 0 → sonraki savaş 3× olur.

Mevcut TogglePause, o andaki zaman ölçeğini timeScaleBeforePause içinde saklar; ContinueGame bunu geri yükler. Hız seçimi pause'da kilitli olduğundan saklanan hız ile oyuncunun seçimi çelişmez. Hazırlık pause/continue işlemi ise sıfır zaman ölçeğini korur.

Hazırlık sayacı zaten Time.unscaledDeltaTime kullanır; 60 saniye hız seçimine göre kısalmaz. Hareket, saldırı beklemeleri ve dalga üretim aralıkları mevcut ölçekli oyun zamanı üzerinden hızlanır. Bu adım hasar, fiyat veya düşman canı değerlerini değiştirmez.

## UİManager ve sahne

`speedButtons` ve `speedHighlights` dizileri Inspector'da 1×, 2×, 3× sırasındadır. Canvas altında GameSpeedControls, Başlat düğmesinin altına yerleştirildi. Her düğmenin kalıcı onClick bağlantısı `SelectGameSpeed` metoduna kendi sayısını gönderir.

SelectGameSpeed isteği GameManager'a iletir, ardından görünümü yeniler. `RefreshSpeedControls` yalnız savaşta düğmeleri etkinleştirir ve SelectedSpeed ile eşleşen düğmenin altın renkli Selected görselini açar. Update içinde de çağrıldığı için faz/pause değişimleri arayüze yansır. Seçim hazırlıkta görünmeye devam eder; düğmeler etkisizdir.

## Doğrulama

Canlı Play Mode'da 13 kontrol geçti: hazırlıkta değişim yasağı; üç düğmenin hız ve vurgusu; geçersiz 0/4 değerleri; pause kilidi; devamda 3×; sonraki hazırlıkta seçimin korunması; hazırlık sayacında hız çarpanı olmaması; hazırlık pause/continue; sonraki dalgada 3×; sonuç kilidi; restart ile 1×.

Kısa test dalgaları yalnız runtime'da kullanıldı. Düğmelerin onClick olayları çağrıldı; fiziksel fare testi değildir. Sayaç kontrolünde TickPreparation'a kontrollü 10 saniye verildi. 1920×1080 görüntü incelendi: `tmp/speed-ui.png`. Build veya her hızda tam bölüm denge testi yapılmadı.

Console hata/uyarı döndürmedi. GameScane kayıtlı, dirty=false; Play kapalı, runInBackground=false, test kulesi 0. Önceki dosyalar `Backups/2026-10-04-speed-ui` altında.
