# Okçu yetenekleri — uygulama ve kod açıklaması

> Güncel mimari SkillData sistemine taşındı. İçerik ekleme ve güncel sınıf görevleri için [SkillData rehberini](SkillData_Kullanim_Rehberi_2026-10-15.md) kullan; aşağıdaki açıklamalar önceki uygulama aşamasıdır.
15 Ekim 2026. Ek ok yeteneği Emir'in kararıyla kaldırıldı; bu davranış eşyalara ait olacak. Yerine başka yetenek eklenmedi. Okçu ağacında 11 etkin düğüm var; diğer üç türün 12'şer düğümü hâlâ taslak.

## Kurallar ve yollar

Kök Okçu Eğitimi 1 puan, diğer yetenekler 2 puan. Kök + hız yolu: Hafif Yaylar → Hızlı Kiriş → Kesintisiz Atış. Kök + menzil yolu: Uzak Atış → Kartal Gözü → Uzun Menzilli Vuruş. Kök + kritik yolu: Kritik Odak → Keskin Uçlar → Avcı Sezgisi → Kusursuz Nişancılık. Önce bağlı önceki düğüm açılır; dallar birbirini kilitlemez. Tam ağacın maliyeti 21 puan, mevcut beş bölümün toplam ödülü en fazla 15 puandır; bütün ağacı doldurmak yerine seçim gerekir.

Okçu Eğitimi ve Keskin Uçlar ayrı ayrı %5 hasar, Hafif Yaylar ve Hızlı Kiriş ayrı ayrı %5 saldırı hızı, Uzak Atış ve Kartal Gözü ayrı ayrı %5 menzil verir. Kritik Odak kritik şansına 3 yüzde puan ekler; normal kritik şansı TowerData sınırını aşmaz.

Kesintisiz Atış aynı hedefe her gerçek isabette %2 hız verir, beş isabette %10 ile sınırlanır. Hedef değişimi veya kaybı sayacı sıfırlar. Kusursuz Nişancılık aynı hedefe her altıncı saldırıyı kesin kritik yapar; burada sayılan isabet değil atıştır. Atış anında menzilin dış yarısındaki hedef Uzun Menzilli Vuruş'tan, canı %30'un altında olan hedef Avcı Sezgisi'nden %10 ek hasar alır. İki koşul beraber sağlanırsa koşullu bonus %20 olur.

Ücretsiz Sıfırla yalnız okçu ağacını temizler, bu ağaca harcanmış puanı iade eder; bölüm rekorları ve diğer türlerin kayıt alanları korunur. Tekrar sıfırlama ikinci kez puan vermez. Açılımlar ve kalan puan aynı kalıcı kayıtta tutulur.

## SkillTreeProgress.cs — kurallar ve kayıt

ArcherSkills dizisi sabit kimlik, isim, açıklama, ön koşul indeksi ve maliyeti bir arada tutar. Kayıt Türkçe isim veya ekrandaki konum yerine `archer.training` gibi kimlikleri saklar; isim değişikliği kaydı bozmaz. CanUnlockArcher yalnız uygunluğu sorgular. TryUnlockArcher işlem öncesinde kaydı tekrar okur; eski bir panel/nesne bakiyesiyle ikinci kez harcama yapılmasını önler. Yeterli puan, ön koşul ve daha önce açılmama koşulları birlikte doğrulanır. Başarılı işlem bakiyeyi azaltır, kimliği unlockedSkills'e ekler ve Save çağırır.

ResetArcher yalnız bilinen okçu kimliklerini kaldırır ve mevcut sabit maliyetleri toplar. İleride maliyetler değiştirilirse eski satın alımların iade politikasını ayrıca ele almak gerekir. Şimdiki sürümde maliyetler sabittir.

Save JSON'u PlayerPrefs'e yazıp kalıcı kaydeder. Gerçek oyuncu anahtarında Changed olayı yayınlanır; ayrı doğrulama anahtarları oyun kulelerini etkilemez. Bölüm ödülü de aynı kayıt yolunu kullanır. Kaydı her atışta diskten okumayız.

## ArcherTower.cs — savaş etkileri

OnEnable/OnDisable, Changed olayına katılır/ayrılır. Böylece mevcut okçular açılım veya sıfırlamada hemen yeniden hesaplanır. Yeni kurulan kule normal CalculateStats başlangıç akışında kaydı okur. CalculateStats önce Tower'ın seviye/eşya/sinerji hesabını çalıştırır, ardından yetenek çarpanlarını uygular. Tekrar çağırmak bonusları üst üste bindirmez.

Örnek: eşya sonrası hasar 100 ise iki hasar yeteneğiyle 110 olur. İki hız yeteneğiyle eşya sonrası bekleme süresi 1,10'a bölünür; beş isabetlik seri bonusuyla bir kez daha 1,10'a bölünür. En düşük bekleme süresi 0,1 saniye olarak korunur. Menzil en fazla temel menzilin 1,10 katına çıkar. Mevcut uçuş halindeki mermiler atışta hesaplanan hasarı taşımaya devam eder.

OnTargetSelected hedef takibini yapar. ResetSequence hem atış hem isabet sayacını temizler ve SkillSequenceVersion artırır. Bu sürüm, hedef değişmeden önce fırlatılmış bir okun sonradan yeni hedef serisine hız eklemesini engeller. RegisterSkillHit yalnız yaşayan, hâlâ takip edilen hedef ve doğru seri sürümü için çalışır; hitStacks en fazla 5 olur.

Attack her çağrıda yalnız bir mermi üretir. Mesafe/can bonusunu atış anında hesaplar; altıncı saldırıda mermiye kritik olasılığı 1 gönderir. Bu garanti, sıradan kritik şansı sınırından ayrıdır. Ek ok üretimi eklenmedi.

## Tower.cs, Bullet.cs ve bilgi panelleri

Tower.CurrentRange varsayılan olarak TowerData menzilini verir; okçu bunu yeteneklerle genişletir. RefreshRangeCollider dünya menzilini collider'ın en büyük ölçeğine bölerek yerel yarıçapı günceller. Okçu hedef seçimi ayrıca güncel menzili kontrol eder; sıfırlama sonrası eski geniş alandaki hedefin kullanılmasını önler. Diğer kulelerin hedef seçimine bu ek mesafe filtresi uygulanmaz.

Tower'ın atış döngüsü seçilen hedefi OnTargetSelected ile bildirir. Bullet.Initialize okçunun seri sürümünü saklar; gerçek çarpışmada, hedef canlıysa önce RegisterSkillHit bildirimi sonra mevcut element/hasar akışı çalışır. Kritik hasar mevcut BulletData.critMultiplier üzerinden hesaplanır. InfoPanelUI ve UİManager menzil gösterimi CurrentRange kullanır.

## SkillTreePanelUI.cs — oyuncu etkileşimi

Okçu sekmesi etkin katalogdaki 11 düğümü, diğer sekmeler 12 taslak düğümü gösterir. Okçu için ayrı bağlantı görseli ve yerleşim kullanılır. Sağ kart puan maliyetini, ön koşulu ve açık durumunu gösterir. Aç düğmesi yeterli puan ve ön koşul varsa etkinleşir. Düğmeye basmak işlemi SkillTreeProgress'e iletir ve ekranı yeniden okur. Ücretsiz sıfırlama da aynı sorumluluk ayrımını izler. Panel kendi başına hasar veya menzil hesaplamaz.

## Doğrulama

14 Ekim canlı test raporu: Docs/Validation/archer-skills-2026-10-14.json. Yetersiz bakiye, ön koşul, çift satın alma, eski nesneyle kayıt güncelleme, kalıcı açılım, iade ve bölüm rekoru korunması geçti. Savaş kontrollerinde mevcut/yeni kule, bonusların tekrar hesaplanması, collider menzili, altı saldırıda altı mermi, altıncı merminin gerçek kritik hasarı, isabetten hız kazanımı, hız sınırı, hedef değişimi, eski merminin reddi, düşük can ve sıfırlamanın bonusları kaldırması doğrulandı. 47 görünür düğümün seçimi/metin kontrolü ve panelden gerçek satın alma/iade geçti. Console temizdi.

Tüm etkileri tek testte incelemek için yalnız test kaydına yedi bölüm üzerinden 21 puan verildi; gerçek kampanya hâlâ beş bölümdür. Oyuncunun kaydı yedeklendi ve geri yüklendi. Bu kontrollü testler tam bölüm denge testi değildir.

15 Ekim son iş: sıfırlama etiketi kendi düğmesinin genişliğine bağlandı. Önceki son kod düzeltmesi menzil filtresini yalnız okçuya sınırlar. Son derleme ve UI görünümü ayrıca kontrol edildi.
