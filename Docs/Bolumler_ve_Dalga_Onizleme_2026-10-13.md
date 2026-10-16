# Bölümler, elementli düşmanlar ve dalga önizlemesi

> Güncel denge: [13–14 Ekim zorluk revizyonu](Zorluk_Dengesi_2026-10-13.md). Aşağıdaki ilk kurulum tablosu ve element çarpanları tarihsel değerlerdir.

13 Ekim 2026. Emir'in onayladığı görev dağılımı uygulandı. Beş bölüm şimdilik aynı GameScane haritasını kullanıyor. İleride her bölümün ayrı haritası, her beş bölümün ortak teması olacak.

## Oyuncunun gördüğü davranış

Ana menüde beş bölüm de seçilebilir. Bölüm kartı dalga sayısını, toplam düşmanı ve hazırlık süresini gerçek bölüm verisinden okur. İkinci dalgada iki tür birlikte gelir; üçüncü dalga uçan düşman dalgasıdır. Sonraki bazı dalgalar iki, bazıları üç tür içerir. Her bölümün son dalgasında bir boss ve destek düşmanları vardır.

Dalga göstergesinin yanındaki bilgi paneli hazırlıkta kendiliğinden açılır. Göstergeye tıklamak paneli açar/kapatır; üzerine gelmek de önizlemeyi gösterir. Hazırlıkta başlayacak dalgayı, savaşta bir sonraki dalgayı gösterir. Düşman adı, adet, can, kara/hava türü, element, zayıflık ve direnç görünür. Pause ve sonuç ekranında gizlenir. Büyük oyun panellerinin arkasında kalır; kendi arka planı dünya tıklamalarını engeller.

## Sayısal ayarlar

| Bölüm | Dalga | Toplam düşman | Can çarpanı | Hasar çarpanı | İlk–son hazırlık |
|---|---:|---:|---:|---:|---:|
| 1 | 5 | 40 | 1,00 | 1,00 | 60–80 sn |
| 2 | 7 | 60 | 1,25 | 1,15 | 75–105 sn |
| 3 | 9 | 80 | 1,50 | 1,30 | 90–130 sn |
| 4 | 11 | 100 | 1,75 | 1,45 | 105–155 sn |
| 5 | 13 | 120 | 2,00 | 1,60 | 120–180 sn |

Toplam 45 dalga ve 400 düşman. Bölüm başına toplam düşman +20, dalga +2, temel hazırlık +15 saniye artar. Bölüm içinde her yeni dalga hazırlığı +5 saniyedir. Can her bölümde ilk bölümdeki temel değerin %25'i, hasar %15'i kadar artar; önceki bölümün üstüne yüzde uygulanmaz. Altın ve XP ödülleri temel değerin bölüm başına %10'u kadar artar. Can/hasar/altın tam sayıya yuvarlanır. Hareket hızı ve saldırı aralığı kaynak düşmandaki gibi kalır.

Bunlar doğrusal veri artışlarıdır; hissedilen zorluk, düşman karışımı ve oyuncunun eşyalarına da bağlıdır. Beş bölümün tamamı henüz oyuncu ile baştan sona dengelenmedi.

## Element hesabı

Onaylanan döngü: **Su → Ateş → Hava → Toprak → Su**. Avantaj ×1,5; ters eşleşme ×0,5; diğer eşleşmeler ×1.

Örneğin Ateş düşmanına 40 Su hasarı 60, 40 Hava hasarı 20, 40 Ateş/Toprak/Normal hasarı 40 olarak uygulanır. Kara/hava hareket türü elementten bağımsızdır: uçan düşman Su elementli de olabilir. Kule hedefleme filtresi hâlâ ayrıca çalışır.

EnemyData.GetDamageMultiplier ve Enemy.TakeDamage bu hesabı zaten içeriyordu. İkinci bir çarpan katmanı eklenmedi; mevcut akış kullanıldı. Yeni bölüm düşmanlarına element verildi. Enemy.Start içinde modele element renginden hafif bir ton uygulanır. MaterialPropertyBlock kullanıldığı için ortak materyal değiştirilmez; her karede materyal üretilmez.

## Kodun görevleri ve çalışma sırası

**ChapterData.cs:** Bir bölümün ScriptableObject verisidir. Bölüm numarası/adı, sahne adı, tema numarası, hazırlık tabanı, dalga başına süre artışı ve dalga listesi burada durur. PreparationFor sıfırdan başlayan dalga indeksine göre süre hesaplar. TotalEnemies grupları toplar. SelectedIndex menü seçimini sahneler arasında taşır; kalıcı kayıt değildir.

**MainMenuManager.cs:** Seçilen ChapterData'yı ekrana yansıtır. Başlatılınca SelectedIndex'i yazar ve SceneLoadingUI aracılığıyla bölümün sceneName alanını yükler. Menüdeki beş işaretin hepsi mevcut verilere bağlandı.

**GameManager.cs:** Awake içinde WaveManager.ConfigureSelectedChapter çağrılır; böylece ilk hazırlık süresi belirlenmeden doğru bölüm seçilir. BeginPreparation her dalgada bölümün yeni süresini alır. Eski autoStartDelay alanı bölüm kataloğu olmayan sahneler için yedek olarak durur.

**WaveManager.cs:** Seçili bölümün dalgalarını çalışma anında kopyalar; oyun sırasında yapılan liste değişiklikleri asset'i değiştirmez. WaveRound artık birden fazla WaveEnemyGroup tutar. Her grup enemyPrefab, enemyCount, spawnInterval ve startDelay içerir. SpawnWave bütün grupların coroutine'lerini aynı karede başlatır. startDelay=0 olan grupların ilk düşmanları birlikte doğar; devamındaki üretimi kendi aralıkları belirler. Önce tüm grupların üretimi, ardından sahadaki son düşmanın temizlenmesi beklenir. Ancak bundan sonra evrim tamamlanması ve sonraki hazırlık/victory çalışır. StopWaves bütün grup üretimlerini durdurur. Eski tek-düşman alanları, eski sahne/test uyumluluğu için korunur; groups doluysa onlar kullanılmaz.

**WavePreviewUI.cs:** Dalga göstergesindeki hover ve tıklamayı dinler. WaveManager.UpcomingWave üzerinden tam olarak üretilecek grupları okur; ayrı bir önizleme düşman listesi yoktur. Describe aynı verilerden metin üretir. Her kare metin oluşturmaz; dalga/oyun durumu değişince veya kullanıcı etkileşiminde yeniler.

**CampaignBuilder.cs:** Editor klasöründe bir defalık içerik kurulum aracıdır; oyunda çalışmaz. 5 bölüm, 30 düşman verisi ve prefab varyantı oluşturdu, sahne/menü referanslarını bağladı. Bölümler zaten varsa tekrar üretimi engeller. Denge değiştirirken tekrar çalıştırmak yerine asset'leri düzenle.

## Inspector'dan nereye müdahale edeceksin?

- `Assets/Resources/Data/Chapters/Chapter01.asset` … `Chapter05.asset`: waves içindeki groups üzerinden düşman prefabı, adet, doğma aralığı/gecikmesi; ayrıca hazırlık süreleri ve sahne adı.
- `Assets/Resources/Data/EnemyData/Campaign/Chapter01` … `Chapter05`: her bölümün Scout, Raider, Guard, Elite, SkyWisp ve Warlord verileri. Can, hasar, ödül ve element buradan değişir.
- `Assets/Prefabs/CampaignEnemies/Chapter01` … `Chapter05`: dalgalarda kullanılan prefab varyantları. Enemy.enemyData ilgili bölümün verisini gösterir.
- GameScane içindeki WaveManager.chapters ve MainMenu içindeki MainMenuManager.chapters: aynı sıralı beş bölümü kullanır.
- WaveBar üzerindeki WavePreviewUI: panel, başlık ve detay metni referansları. Canvas altındaki WavePreviewPanel: boyut/renk/yazı yerleşimi.

Orijinal `EnemyData/Chapter01` ve `Prefabs/Chapter01` değerleri korunmuştur. Kampanya artık yeni Campaign verilerini kullandığı için yeni bölüm dengesi bu yeni klasörlerden düzenlenmelidir. Builder formülleri otomatik canlı ölçekleme değildir; üretilmiş değerler Inspector'da bağımsızdır.

## Gelecekte ayrı haritalar ve beşli temalar

Her ChapterData.sceneName ileride farklı bir sahneye işaret edebilir; o sahneler Build Settings'e eklenmeli ve oynanış bileşenleri/rota/doğma noktaları bağlanmalıdır. İlk beş bölümün themeIndex değeri 0'dır. Bölüm 6–10 için 1, 11–15 için 2 kullanılacak. Bu alan şimdilik veri yeridir; otomatik çevre değiştiren bir tema sistemi yapılmadı. Yeni tema ve haritalar ayrıca tasarlanacak.

## Doğrulama

172 veri/hasar kontrolü ve 10 Play Mode kontrolü geçti. Kontroller: beş bölümün gerçek menü akışıyla yüklenmesi, doğru hazırlık süresi, tek EventSystem, üç farklı grubun aynı anda doğması, pause sırasında üretimin durması, toplam grup adedi, sonraki hazırlık artışı ve önizlemenin gerçek dalgayla eşleşmesi. On altı element eşleşmesi gerçek Enemy.TakeDamage üzerinden ölçüldü. Console hata/uyarı listesi boştu.

Rapor: `Docs/Validation/campaign-2026-10-13.json`. Görsel: `Docs/Validation/campaign-wave-preview-2026-10-13.png`. Testler kontrollü kısa koşulardır; bütün bölümlerin tam oynanış/dengesini veya farklı ekran oranlarını doğrulamaz.
