# Kule collider düzeltmesi — 6 Ekim 2026

Gövde BoxCollider'ları zaten modele uygundu. Tower.Start menzil küresinin yerel yarıçapını doğrudan veri değerine eşitlediğinden okçunun dünya yarıçapı 15, büyücünün 0,8 ölçekle 8 birim oluyordu.

## Değişiklik

- Okçu dünya yarıçapı 5, büyücü 4 birim oldu. Merkez kaymaları sıfırlandı.
- Birlik kulesinin kullanılmayan küresi kaldırıldı; askerlerin engageRange algılaması korunuyor.
- Gövde kutuları aynı kaldı. Okçu/büyücü küresi RangeTrigger alt nesnesine taşındı. Ignore Raycast katmanı fare ışınlarının menzil küresine takılmasını önler.
- Kule köküne kinematic, gravity kapalı Rigidbody eklendi; alt nesne trigger olayları mevcut Tower.OnTriggerEnter/Exit metoduna iletilir. Yeni script veya yönetici eklenmedi, global fizik ayarları değiştirilmedi.
- Tower.Awake küreyi alt nesnelerde bulur. Start yerel yarıçapı `data.range / max(abs(lossyScale))` hesaplar. TowerData.range artık dünya birimi olarak açıklanır. Başlangıç ölçeği hesaba katılır; oyun sırasında sürekli ölçek değiştirme bu adımın kapsamı değildir.
- Okçu prefab'ında kaydetmeyi engelleyen bir Missing Script kaydı temizlendi; ardından kayıt başarıyla tamamlandı. İlk başarısız kaydın hata mesajı Console geçmişinde kalabilir.

## Doğrulama

İki kulede toplam 12 kontrol geçti: doğru dünya yarıçapı, menzil dışında hedef olmaması, gövde raycast'i, boş alanda menzil küresinin ışını engellememesi, gerçek fizik adımıyla giriş ve çıkış olayları. Trigger metotları elle çağrılmadı; geçici Enemy nesneleri taşınıp fizik adımı beklendi. İlk denemede hazırlığın timeScale=0 değeri simülasyonu durduruyordu; kontrollü testte 1 yapıldı. Test nesneleri Play sonlandırılarak temizlendi.

Menzil azaltması dengeyi etkiler; beş dalganın tamamının yeni menzillerle zafer dengesi yeniden ölçülmedi. Değişiklik öncesi dosyalar Backups/2026-10-06-colliders altında. Kanıt: Docs/Validation/colliders-2026-10-06.json.
