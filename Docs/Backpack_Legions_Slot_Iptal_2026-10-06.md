# Kule satın alma iptalinde slot kilidi — 6 Ekim 2026

## Takip düzeltmesi: bütün slotların engellenmesi

İlk düzeltmedeki IsPointerOverGameObject kontrolü kaldırıldı. Sahnedeki PhysicsRaycaster dünya nesnelerini de EventSystem'e bildiriyor; genel pointer kontrolü bunları UI sayarak slot girişini engelleyebiliyordu. Önceki akış testinde EventSystem'i kapatmak bu regresyonu gizledi.

TowerSlot.IsPointerOverUI şimdi tıklamanın güncel ekran konumunda EventSystem.RaycastAll çağırıyor ve yalnız GraphicRaycaster sonuçlarını engelliyor. PhysicsRaycaster sonuçları slot seçimini engellemiyor. Önceki slotu serbest bırakma ve panel kapanış temizliği korunuyor.

Takip kontrolünde EventSystem açık bırakıldı: sekiz Shop slotunda aç/kapa ve dokuz satın alma/iptal kontrolü geçti. Kapat düğmesi üzerindeki sorgu gerçek GraphicRaycaster hit'iyle engellendi; aynı sorgudaki PlaneTestMVP:PhysicsRaycaster sonucu ayırt edildi. İlk düğme sorgusu panelin açıldığı karede yapıldığı için grafik kaydı henüz hazır değildi; sonraki karede kontrol başarılı oldu. Testler programatik olay ve ekran koordinatı sorgularıdır, fiziksel fare otomasyonu değildir. Kanıt: Docs/Validation/slot-ui-filter-2026-10-06.json.

Alttaki ilk uygulama ve test notları tarihsel kayıttır; genel IsPointerOverGameObject kontrolünün yerine yukarıdaki filtre geçmiştir.

Sorun canlı testte doğrulandı: A slotu seçilip collider'ı kapatıldıktan sonra B seçilirse selectedTowerSlot doğrudan B ile değişiyordu. Paneli kapatma yalnız B'yi serbest bırakıyor; A boş olmasına rağmen collider'ı kapalı kalıyordu. UI tıklamalarını dünyadaki slotlardan ayıran kontrol de yoktu. Kullanıcının fiziksel tıklamalarının tam olay sırası kaydedilmedi; aynı kilit durumu seçim geçişiyle yeniden üretildi.

## İki mevcut sınıfta düzeltme

- TowerSlot.OnMouseDown, EventSystem imlecin UI üzerinde olduğunu bildiriyorsa işlemi durdurur. Kapat/satın al gibi UI tıklamaları arkadaki slotu seçemez.
- Başka slot seçmeden önce eski selectedTowerSlot.ReleaseSelection çağrılır. Böylece ilk slot referansı kaybolmadan collider'ı açılır. Yeni seçim kule türünü sıfırlar.
- UİManager.CancelTowerPurchase, slotu serbest bırakıp yer/tür/slot referanslarını ve satın alma düğmesini temizler. TogglePanel kapatırken bu ortak yolu kullanır.
- Panel başka bir kodla gizlenirse UİManager.Update kalan seçimi temizler. Aynı karede yeni slot seçilirse TowerSlot'taki önceki seçimi bırakma kontrolü yine korur.
- BuyTower gizli panel, boş veya dolu slot seçimiyle satın almayı reddeder. Başarılı satın almada slotun dolu ve collider'ının kapalı kalması korunur.

## Test

9 akış kontrolü geçti: kapatınca A'nın açılması; B'den alışverişin A'yı kilitlememesi; A'yı yeniden seçme ve satın alma; panel açıkken slot değiştirip kapatma; paneli doğrudan gizleme; tekrarlı aç/kapa; yetersiz para ardından kapatma; dolu slotların kilitli kalması. Ayrıca mevcut pointer UI üzerindeyken slot seçiminin engellendiği doğrulandı: toplam 10 kontrol.

Test, canlı Play Mode'da OnMouseDown ve gerçek kayıtlı kapat düğmesi onClick olayını programatik çağırır; fiziksel fare tıklaması otomasyonu değildir. İlk test girişimi, pointer UI üzerinde olduğu için yeni koruma tarafından engellendi; testin devamında panel açılmış varsayımı NullReferenceException üretti. Bu test düzeni hatasıydı. UI koruması ayrıca doğrulandıktan sonra 9 akış kontrolünde EventSystem yalnız geçici Play oturumunda kapatıldı; Play sonlandırılınca normal sahne durumu geri geldi. Test kaynağı Tools/Validation/slot_cancel.cs bu kontrollü düzeni gerektirir.

Yedek: Backups/2026-10-06-slot-cancel. Kanıt: Docs/Validation/slot-cancel-2026-10-06.json. Yeni sınıf veya sahne bağlantısı eklenmedi.
