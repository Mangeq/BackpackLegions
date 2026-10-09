# Backpack Legions — Steam tanıtım paketi

9 Ekim 2026. Yerel teslim; Steam'e yükleme/yayınlama yapılmadı.

## Teslim

- `backpack-legions-logo.png`: 1536 × 1024, alfa kanallı PNG. Yerleşik image_gen ile oluşturuldu; tam istem `logo-prompt.txt` dosyasında.
- `screenshots/`: Beş adet 1920 × 1080 gerçek oyun görüntüsü. Dosya numaraları önerilen mağaza sıralamasıdır. Ekran görüntülerine pazarlama yazısı, logo veya yapay zekâyla sahne eklenmedi.
- `backpack-legions-trailer.mp4`: 32 saniye, 1920 × 1080, 30 fps, H.264 video / AAC stereo ses.
- `index.html`: Logo, video ve beş görüntüyü birlikte incelemek için yerel galeri.

## Videonun kurgusu

| Süre | İçerik |
|---|---|
| 00:00–00:04 | Hava düşmanlarıyla gerçek savaş; kısa açılış başlığı |
| 00:04–00:05.5 | Çanta mekaniğine geçiş kartı |
| 00:05.5–00:13.5 | Eldivenin taşınması, döndürülmesi, yeniden komşu yerleştirilmesi |
| 00:13.5–00:15 | Element seçimine geçiş kartı |
| 00:15–00:20 | Ateş uzmanlaşması ve element mağazasının yenilenmesi |
| 00:20–00:27 | Yakın plan gerçek çatışma |
| 00:27–00:32 | Logo ve “Yakında • Steam” kapanışı |

## Kaynak ve çekim yöntemi

Çekimler `Assets/Scenes/GameScane.unity` sahnesinin Play modunda alındı. Mevcut üç kule, mevcut dalgalar, düşmanlar ve gerçek saldırı/XP mekanikleri kullanıldı. Yakın çekim için ana kamera yalnız Play modunda yeniden konumlandırıldı. Kayıt simülasyonu 30 fps sabit zaman adımıyla çalıştırıldı; montajda oynanış hızlandırılmadı. Menü sekansları gerçek oyun içi sürükle/bırak ve element seçimi metotlarıyla kaydedildi.

Çanta vitrini mevcut ItemData içerikleri seçilerek sahnelendi; eşya fiyatları savaşta kazanılan altından düşüldü, yerleşimler gerçek grid kurallarından geçirildi. Bu, ardışık bir normal mağaza alışverişi kaydı değildir; mevcut eşyaların ve çalışan çanta/evrim sisteminin gösterimidir. Element seçimi gerçek savaşta kazanılan XP'yi kullandı. Sahne, prefab, denge veya oyun kaynak dosyaları bu iş için kalıcı değiştirilmedi.

Videonun müziği `Tools/Marketing/make_soundtrack.py` ile matematiksel sentezden üretildi; harici müzik, kayıt veya ses örneği kullanılmadı. Müzik tanıtım videosuna aittir, oyunun mevcut ses tasarımını temsil etmez. Oyun içi ses kaydı yoktur.

## Görsel hazırlık durumu

Mevcut prototipte birlikler kapsül, bazı düşmanlar küp gibi geçici modeller kullanıyor. Paket bunu saklamaz. Bu karakter görselleri tamamlandığında aynı çekimlerin yenilenmesi, mağazadaki ilk izlenimi belirgin biçimde güçlendirir. UI Türkçedir. Logo bağımsız bir marka görselidir; oyunun grafik kalitesine ilişkin ekran görüntüsü yerine kullanılmamalıdır.

İstenen bir logo, beş görüntü ve bir video hazırlanmıştır; Steam'in ayrıca istediği kapsül/library ölçülerindeki tüm görseller bu teslimin kapsamı değildir. Logo tek başına tam bir Steam grafik seti değildir.

## Tekrar üretim

Ham kareler `raw/` altında tutulur. Çekim gövdeleri `Tools/Marketing/capture_*.cs` dosyalarında; Unity MCP execute_code ile Play modunda kullanılır. `capture_inventory.cs` ve `capture_elements.cs` hazırlanmış çanta durumuna bağlıdır; bağımsız bir başlangıç kurucusu değildir. `Tools/Marketing/build_trailer.py` mevcut ham karelerden MP4'ü tekrar oluşturur. Python ortamında NumPy; `Tools/Marketing/vendor` altında imageio-ffmpeg kullanıldı.

## Steam kaynakları

- Ekran görüntüsü ve kapsül kuralları: https://partner.steamgames.com/doc/store/assets/standard
- Video ölçüleri, kodek ve oynanış önceliği: https://partner.steamgames.com/doc/store/trailer

Kaynaklar 9 Ekim 2026 tarihinde kontrol edildi. Steam ekran görüntüleri için 1920 × 1080 veya daha büyük 16:9 görüntüler; video için 1080p, 30/60 fps, H.264 ve AAC tercih eder. Son yayın kararı kullanıcıya aittir.
