# Aşama 1 — Tekrar çalıştırılabilir kontrol

Unity'de kaydedilmiş `GameScane` açık, Play kapalı ve MCP for Unity yerel sunucusu `http://127.0.0.1:8080/mcp` üzerinde bağlı olmalı. Araç kaydedilmemiş sahne veya açık Play görürse değişiklik yapmadan durur. Bu proje kimliğine bağlanır.

Proje kökünde PowerShell:

```powershell
& 'C:/Users/Emir/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe' Tools/Validation/run_stage1.py
```

Standart Python 3 kurulumu varsa `python Tools/Validation/run_stage1.py` de kullanılabilir; ek paket gerekmez.

Araç Play'i açar; 01–08 dosyalarındaki C# metot gövdelerini MCP execute_code ile sırayla çalıştırır. Bu dosyalar Assets dışında oldukları için oyun assembly'sine katılmaz, build'e girmez. Oyunun mevcut metotlarına ve sentetik pointer olaylarına dayanır. Test süresince Unity'de manuel oynama yapılmamalı.

28 koşul: eksik script taraması, başlangıç, üç kule fiyatı/kurulumu, üç varsayılan silah, üç eşya alımı, üç taşıma, üç geçersiz taşıma, üç dükkan aç/kapa, üç reroll, Combat kilidi/tekrarlı başlangıç, zafer, restart, yenilgi ve sonuç kilidi.

Eşya ve reroll testlerinde bakiye runtime'da izole edilir; bu bir ekonomi dengeleme testi değildir. Zafer için tek düşmanlı geçici dalga kullanılır, mevcut hasar metodu çağrılır. Dalga/para/test kuleleri kaydedilmez. Fiziksel fare, main menu/exit, bütün GDD özellikleri veya bağımsız build testi değildir.

Her adımın sonucu `Docs/Validation/stage1-latest.json` içine yazılır. Hata varsa araç başarısız çıkar. Console temizlenmez; mevcut hata/uyarılar rapora girer ve geçişi engeller. İlk denemelerde test aracındaki bağlantı hazırlığı ve eşya seçimi varsayımları düzeltildi; nihai koşuda 28/28 geçti.

`finally` bloğu başarılı/başarısız koşuda Play'i kapatır ve önceki runInBackground değerini geri koyar. Bağlantının tamamen kopması veya Python işleminin dışarıdan kapatılması durumunda otomatik temizlik garanti edilemez; Unity'den Play'i kapatıp test değişikliklerini kaydetmeden çık.

## Kaynak yedeği ve geri dönüş

Değişiklikten önce sahneleri kaydet, Play'i kapat ve şu komutu çalıştır:

```powershell
& 'C:/Users/Emir/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe' Tools/Validation/snapshot_project.py
```

Yeni tarih/saatli ZIP `Backups` altına yazılır; eski yedeğin üstüne yazılmaz. Assets (tüm .meta dosyaları dahil), Packages, ProjectSettings, Docs ve Tools arşivlenir. Library/Temp/Logs/Backups tekrar arşivlenmez. Her dosyanın SHA-256 değeri SNAPSHOT_MANIFEST.json içinde tutulur ve ZIP yeniden açılarak bütün dosyalar doğrulanır. Yedek aynı disktedir; harici disk/bulut yedeği yerine geçmez. Projenin mevcut Git deposu ve commit edilmemiş değişiklikleri korunur; bu araç commit/push yapmaz ve ZIP Git geçmişini içermez.

Geri dönüş için mevcut proje klasörünü silme veya üzerine ZIP açma. Unity'yi kapat; ZIP'i **yeni, boş bir klasöre** çıkar ve Unity Hub'dan o klasörü `ProjectSettings/ProjectVersion.txt` içindeki sürümle aç. Unity Library'yi yeniden oluşturur. Böylece yeni eklenmiş dosyaların eski dosyalarla karışması önlenir, mevcut çalışma da korunur. İlk açılışta Console ve bu kontrol akışını tekrar çalıştır.

Mevcut `Backups/2026-10-...` klasörleri yalnız ilgili adımdan önceki kısmi dosyalardır; tam kaynak ZIP'iyle karıştırılmamalı. ZIP bütünlüğü doğrulanır; ayrı klasörde tam Unity import/build geri dönüş provası bu görevde yapılmadı.

4 Ekim kapanış kaydı: `Backups/stage1-baseline-20261004-210406.zip`, 2.997 dosya, 104.692.292 bayt; arşivdeki her dosya SHA-256 ile doğrulandı. Git son commit: `8ce67c4`; bekleyen yerel değişiklikler mevcut.
