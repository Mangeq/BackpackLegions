# Sonuç panelleri ve kule türüne göre skilltree tasarımı

Emir'in onayladığı kapsam: zafer/yenilgi ve sonraki bölüm geçişi GameManager'da; dört tür sekmesi ve yetenek açıklamaları yeni SkillTreePanelUI'da. Skilltree her yerleştirilmiş kuleye ayrı değil, okçu/büyücü/havan/birlik türleri için ortak olacak. Bu çalışma yalnız skilltree arayüzünü hazırlar; yetenek ekonomisi ve etkileri eklenmedi.

## Oyuncu akışı

Zaferde **BÖLÜM GEÇİLDİ** paneli görünür. Bölüm adı, tamamlanan dalga sayısı ve kalan altın gerçek durumdan okunur. Sonraki Bölüm, Tekrar Oyna ve Ana Menü düğmeleri bulunur. Sonraki Bölüm, bölüm kataloğundaki sıradaki ChapterData'yı seçer ve onun sahnesini yükler. Şimdilik sahne ortak olsa da doğru dalgalar, süre ve başlangıç bütçesi uygulanır. Son bölümde sonraki düğmesi pasifleşir ve tüm bölümlerin tamamlandığını söyler.

Yenilgide ayrı **KULE DÜŞTÜ** paneli görünür. Tamamlanan dalga ve kalan altın gösterilir. Ana eylem Tekrar Dene'dir; aynı bölüm kendi başlangıç bütçesiyle yeniden yüklenir. Ana Menü de kullanılabilir. Yenilgiden sonraki bölüme geçilemez.

Sonuç ekranları aynı koyu yeşil/krem tipografi düzenini paylaşır. Zaferde mat altın, yenilgide kırmızı vurgu vardır. Oyun arka planı karartılır ve modal giriş engeli dünya tıklamalarını tutar. Bu ekranlarda sahte yıldız, skor veya kazanılmamış ödül gösterilmez.

Ana menü ve oyun içi duraklatmada **YETENEK AĞAÇLARI** düğmesi bulunur. Solda dört tür, ortada altı düğümlü ağaç, sağda seçilen yeteneğin açıklaması vardır. Okçu yeşil, büyücü mor, havan toprak, birlik mavi vurgu kullanır. İkonlar mevcut eşyalardan anlamına uygun seçildi. Panel ESC, Kapat veya arka plana tıklama ile kapanır. Oyun içindeki skilltree kapandığında oyun duraklatılmış kalır; ayrıca Devam Et/ESC gerekir.

## Skilltree kapsamı

Okçu, büyücü ve birlik ağacındaki altışar isim mevcut Skilltree V1 yol haritası aktarımından alındı. Etki açıklamaları taslak olarak gösterilir. Havanın altı düğümü yeni öneridir: Barut Eğitimi, Seri Yükleme, Geniş Patlama, Ağır Mühimmat, Element Barutu ve Kuşatma Ustası.

Ağaç bağlantıları yerleşim önerisidir; ön koşul veya açılma algoritması değildir. Satın alma düğmesi etkin değildir. Puan kazanma/harcama, kalıcı kayıt, seviye, kilit açma, yetenek etkileri ve reset yoktur. Bu kurallar ve sayısal denge ayrıca birlikte belirlenecek. Panel oyun ekonomisine ya da kule statlarına dokunmaz.

## Kodun çalışma biçimi

### GameManager.cs

GameOver ilk sonucu koruyan mevcut kontrolü kullanır. Savaş üretimi durdurulur, zaman sıfırlanır ve sonuç tipine göre yalnız ilgili panel açılır. Eski GameOverPanel referansı yenilgi paneli olarak korunur. İçindeki mevcut tekrar/ana menü düğmeleri yeniden kullanıldı; kart içine taşındıkları için Awake artık bunları çocuklar arasından adlarıyla bulur. Zafer paneli ve metin/düğme referansları Inspector'dan bağlanır.

NextChapterIndex, ActiveChapter'ın WaveManager.chapters listesindeki gerçek sırasını bulur. Dizinin dışına veya boş bir bölüme geçmez. NextChapter yalnız Victory durumunda ve yükleme başlamamışken çalışır; SelectedIndex'i güncelleyip SceneLoadingUI'ya yeni sahneyi verir. RestartGame de aynı yükleme ekranını kullanır; tekrar tıklama yükleme korumasından geçemez. Başlangıç parası mevcut Awake bölüm bütçesi akışından gelir.

OpenSkillTree yalnız duraklatmada açılır. HandleEscape önce skilltree'yi, ardından ayarları ele alır; böylece aynı ESC hem paneli kapatıp hem savaşı devam ettirmez. ContinueGame açık bir skilltree varken çalışmaz.

### SkillTreePanelUI.cs

Branch iç içe veri sınıfı, her türün başlığını, alt açıklamasını, rengini, altı yetenek adı/açıklaması/ikonunu taşır. Bu veri yalnız görünüm içindir; ayrı bir yetenek sistemi veya manager kurulmadı.

SelectBranch düğüm etiketlerini ve ikonlarını değiştirir, ilk düğümü seçer. SelectNode sağ kartı günceller ve seçili düğümü renkle belirtir. Open oyun sahnesindeyse pause şartını kontrol eder. Close modalı gizler. IsOpen/TryCloseOpen ortak ESC sıralamasını sağlar. BlocksWorldInput, açıkken ve kapatıldığı karede dünya tıklamasının arkaya geçmesini önler. Statik referanslar yeni Play oturumunda sıfırlanır, nesne yok edilince temizlenir.

### MainMenuManager.cs ve UİManager.cs

MainMenuManager, ana menü düğmesinden SkillTreePanelUI.Open çağırır ve ESC'de önce onu kapatır. UİManager.HandleOutsideClick skilltree açıkken dünya paneli işlemlerini engeller. Mevcut ayar paneli ve pause sorumlulukları korunur.

### Editor/ResultsAndSkillsBuilder.cs

Bir defalık sahne/prefab kurucusudur. Çalışma zamanında arayüz üretmez. SkillTreeUI.prefab zaten varsa tekrar kurulum yapmayı engeller. Mevcut sonuç panelini günceller, ayrı zafer paneli ekler, menü/pause bağlantılarını kurar. İleride tasarımı düzenlerken kurucuyu tekrar çalıştırmak yerine prefab ve sahne objelerini düzenle.

## Inspector'da nerede?

- `Assets/Prefabs/UI/SkillTreeUI.prefab`: bütün skilltree görünümü ve Branch[] taslak metinleri. Hem MainMenu hem GameScane bunu kullanır.
- GameScane → ResultsCanvas → GameOverPanel: yenilgi kartı.
- GameScane → ResultsCanvas → VictoryPanel: zafer kartı.
- GameScane → GameManager: panel, özet, sonraki bölüm düğmesi ve skilltree referansları.
- MainMenu → MainMenuManager: skillTreePanel referansı; FrontendCanvas/SafeFrame/HomePanel içindeki yeni düğme.

## Doğrulama

15 akış kontrolü geçti: dört türde 24 düğüm seçimi ve açıklaması; metin taşmaması; ana menüden açma; dışarı tıklamayla kapatma; pause/ESC sırası; zaferden ikinci bölüme gerçek yükleme ve doğru bütçe; yenilgiden aynı bölümü tekrar yükleme; yenilgide ilerlemenin engellenmesi; son bölüm sınırı. Console kontrolünde hata/uyarı yoktu. Zafer, yenilgi ve skilltree ekran görüntüleri 1920×1080 Game View üzerinden incelendi.

Rapor: `Docs/Validation/results-skills-2026-10-14.json`. Görseller: `result-victory-2026-10-14.png`, `result-defeat-2026-10-14.png`, `skilltree-final-2026-10-14.png`, `skilltree-havan-final-2026-10-14.png` aynı klasörde. Sonuç ekranları testte doğrudan tetiklendi; bu çalışma tam bölüm oynanışı veya skill etkisi testi değildir.


## Dallanan ağaç revizyonu — 14 Ekim

Orta alanın birleşen elmas bağlantıları kaldırıldı. Başlangıç düğümü alta taşındı; iki ana dal yukarıda üç uç yeteneğe ayrılıyor. İncelen eğri çizgiler, küçük yan dallar ve kökler ağaç siluetini oluşturuyor. Tür sekmeleri ve sağ açıklama kartı aynı sorumlulukta.

`Tools/Validation/skilltree_branch_layout.cs` mevcut prefabı PrefabUtility ile açıp yalnız eski BranchLink/TreeBranches görsellerini değiştirir. Eğrileri 20 sabit UI Image parçasıyla çizer; bunlar raycast almaz ve her karede yeniden üretilmez. Altı mevcut düğümün RectTransform konumları değişir; Button dinleyicileri, ikonlar ve açıklama eşleştirmeleri korunur. Yeni runtime sınıfı eklenmedi, SkillTreePanelUI değişmedi. Bağlantılar açılma ön koşulu veya puan harcama kuralı uygulamaz.

Ortak SkillTreeUI prefabı ana menü ve oyun içi panelin ikisini de günceller. Eski ResultsAndSkillsBuilder ilk kuruluma ait tarihsel araçtır; yeniden çalıştırılmamalıdır.

Doğrulama: dört türde 24 gerçek Button.onClick seçimi, doğru açıklama ve TMP metin taşması kontrolü geçti. Console hata/uyarı yok. Yeni görsel: `Docs/Validation/skilltree-branching-2026-10-14.png`; kontrol çıktısı: `skilltree-branching-checks-2026-10-14.json`. Unity Play kapatıldı ve Boot sahnesine dönüldü.
