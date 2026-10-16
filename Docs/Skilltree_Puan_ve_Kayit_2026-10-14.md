# Kalıcı yetenek puanı ve genişletilmiş ağaç — 14 Ekim 2026

> Güncel mimari SkillData sistemine taşındı. İçerik ekleme ve güncel sınıf görevleri için [SkillData rehberini](SkillData_Kullanim_Rehberi_2026-10-15.md) kullan; aşağıdaki açıklamalar önceki uygulama aşamasıdır.
## Oyuncunun gördüğü davranış

Bölümü kazanırken kalenin kalan can oranı %90 veya üzerindeyse 3, %50 veya üzerindeyse 2, daha düşük pozitif candaysa 1 puanlık sonuç alınır. Yenilgi 0 puandır. Her bölümün en yüksek puanlık sonucu saklanır. Yeni ödül = max(0, yeni sonuç − önceki en iyi sonuç). Böylece 1 puanlık sonucu 3'e yükseltmek 2 ek puan verir. Tekrar oynama veya yenilgi geçmiş puanı azaltmaz. Beş bölümden toplam en fazla 15 puan kazanılabilir.

Ortak puan havuzu bölüm/sahne değişiminde sıfırlanmaz. Zafer ve yenilgi kartında kalan can, bu denemede eklenen puan ve toplam puan görünür. İki karttaki YETENEK AĞACI düğmesi paneli açar; ESC, KAPAT veya dışarı tıklama ağacı kapatır ve alttaki sonuç ekranını korur. Oyun sonuç durumunda donmuş kalır.

Dört kule türünde artık 12'şer düğüm (toplam 48) bulunur. Ortak kökten üç ana dal ayrılır. Yeni altı isim/tür ve görsel bağlantılar öneridir; etkiler, maliyetler ve açılma koşulları henüz onaylanmadı. Bu nedenle satın alma, buff uygulama ve sıfırlama bu adımda açılmadı.

## Kod görev dağılımı

### Assets/Scripts/SkillTreeProgress.cs

Sahneye eklenen bir MonoBehaviour veya yeni manager değildir. GameManager ve UI gerektiğinde normal C# nesnesi oluşturur. Constructor PlayerPrefs içindeki `BackpackLegions.SkillTree.v1` anahtarından JSON kaydını okur. Kayıt yoksa boş bir SaveData oluşturur; sırf panel açıldığı için kayıt yazılmaz.

SaveData içindeki `availablePoints` ortak bakiyedir. `chapters`, her bölüm numarası ile o bölümün `bestPoints` değerini saklar. Bölüm kimliği olarak ChapterData.chapterNumber kullanıldığı için farklı bölümlere benzersiz numara verilmelidir; mevcut bölümün numarasını sonradan değiştirmek kayıt eşleşmesini değiştirir. `version` ileride kayıt biçimini dönüştürmek için sürümdür. `unlockedSkills` gelecekte sabit yetenek kimliklerini tutmak için ayrılmıştır; şu an bu listeye yazan/açılım yapan bir işlem yoktur. `IsUnlocked` yalnız okuma sağlar.

`PointsForResult` saf hesaplamadır; kayıt yazmaz. Yüzde eşiklerinde kayan nokta yuvarlaması olmaması için can × 100 değerini maksimum can × 90/50 ile long olarak karşılaştırır. Geçersiz maksimum can, sıfır can veya yenilgi ödül vermez.

`BestPoints` ilgili bölümün önceki en iyi puanını döndürür. `RecordResult` olumlu fark varsa rekor ve bakiyeyi aynı JSON içine yazıp PlayerPrefs.Save çağırır. İkinci kez aynı sonucu kaydetmek ödül üretmez. Kayıt cihazda kalıcıdır; bulut/hesap senkronizasyonu eklenmedi. Beklenmeyen kayıt sürümü sessizce sıfırlanmaz, hata üretir.

### Assets/Scripts/GameManager.cs

GameOver mevcut ikinci sonuç korumasından sonra aktif ChapterData'yı ve **Base etiketli gerçek savaş üssünü** okur. Eski PlaneTest üssünü FindObjectOfType ile seçmez. Kalan canı SkillTreeProgress.RecordResult'a bildirir ve dönen gerçek ek puanı sonuç metnine yazar. Bölüm verisi yoksa ödül oluşturmaz. Sonuç durumunu belirleme ve sonraki bölüm geçişi hâlâ GameManager'dadır.

HasEnded artık salt okunur public özelliktir. OpenSkillTree hem duraklatmada hem sonuç durumunda çalışır. Combat/Preparation içinde duraklatmadan açılma davranışı değiştirilmedi.

### Assets/Scripts/SkillTreePanelUI.cs

Open, duraklatma veya sonuç durumunu kabul eder ve pointsLabel'a kayıttan bakiyeyi okur. Yetenek açıklamasını seçmek puan harcamaz. Kapanırken oyun durumunu değiştirmez; sonuç ekranı altta kaldığı için yeniden oluşturulmaz. Aynı prefab ana menü ve oyun sahnesinde kullanılır.

### Prefab ve sahne düzenleme araçları

`Tools/Validation/skilltree_expand.cs` mevcut SkillTreeUI prefabında ilk altı düğümü korur; altı düğüm daha ekler, Button.onClick olayını doğru indeksle bağlar, statik eğri dalları ve puan etiketini kurar. Bir defalık uygulama aracıdır; tekrar çalıştırmak düğüm çoğaltabilir, yeniden çalıştırılmamalıdır.

`Tools/Validation/skilltree_results_access.cs` GameScane içindeki mevcut sonuç kartlarına düğme ekler ve özet için alan açar. Prefab/sahne Unity API'leriyle kaydedildi; YAML elle düzenlenmedi.

## Doğrulama ve sınırlar

`Tools/Validation/test_skill_progress.py`: 0/1/49/50/89/90/100 can eşikleri, farklı maksimum can, geçersiz can, tekrar ödül, iyileştirme farkı, yenilgi, ayrı bölüm rekorları ve kaydı yeniden okuma kontrol edildi. Canlı Play testinde 25 canla +1, sonraki 90 canla +2, yenilgide +0 görüldü. Üç senaryonun her birinde 48 Button seçimi ve metin taşması kontrol edildi; sonuçtan açılış ve ESC ile geri dönüş geçti. Console hata/uyarı yoktu.

Testler mevcut oyuncu kaydını yedekleyip geri yükledi; test puanları oyuncuya bırakılmadı. Sonuçlar kontrollü olarak tetiklendi; tam bölüm oynanışı veya uygulamayı tamamen kapatıp yeniden açma testi yapılmadı. Kalıcılık PlayerPrefs.Save ve nesne/Play oturumları arasında yeniden okuma ile doğrulandı. Unity Boot sahnesinde, Play kapalı bırakıldı.

Çıktılar: `Docs/Validation/skill-progress-2026-10-14.json`, `skill-points-victory-2026-10-14.png`, `skilltree-12-nodes-2026-10-14.png`.
