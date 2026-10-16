# Panel kapatma ve otomatik satış — 10 Ekim 2026

**Durum:** Uygulandı ve Unity Play Mode'da 23 kontrol geçti. Satış alanı/onay penceresi Game View'da incelendi; Console temiz. Test sonunda Play kapalı, sahne kayıtlı ve geçici kuleler temiz.

## Panel akışı

`UİManager.CloseTopPanel` önce dönüşüm onayını, ardından birleşik kule panelini veya satın alma panelini kapatır. `GameManager.HandleEscape` açık panel yoksa pause açar; oyun zaten duraklatılmışsa devam eder. Tek ESC hem panel kapatıp hem pause açmaz.

`HandleOutsideClick` ekran noktasını ilgili panel dikdörtgeniyle karşılaştırır. Dönüşümde tam ekran karartma yerine içindeki `Card` kullanılır. Dışarı tıklama sadece en üstteki pencereyi kapatır; satış yapmaz. `LateUpdate` paneli açan tıklamanın aynı karede onu kapatmasını önler. Sürükleme sırasında dışarı bırakma satış/yerleştirme akışına bırakılır; ESC ise sürüklemeyi iptal ederek eşyayı eski yerine koyar.

`Tower` ve `TowerSlot`, kapatma için kullanılan dünya tıklamasının arkada yeni bir pencere açmaması için `ConsumeWorldPanelClick` kontrolünü kullanır. `BackpackUIManager` içindeki ayrı dışarı tıklama kodu kaldırıldı; kapatma tek yerde yönetilir. Yeni manager eklenmedi.

## Tür değişiminde satış

`Tower.TypeChangeSaleValue` ve `TypeChangeSaleCount`, ana eşya dışındaki çanta kayıtlarını hesaplar. Depo ve teslimler hesaba katılmaz. Satış değeri mevcut `EconomyManager.GetSellValue` metodudur: her eşya fiyatının %50'si ayrı ayrı aşağı yuvarlanır.

Onay ekranı eşya sayısını, satış toplamını, dönüşüm ücretini ve işlem sonrası bakiyeyi gösterir. Çantayı elle boşaltmak gerekmez. Ana eşya satış geliri üretmez; yeni kulenin ana eşyasına dönüşür.

`TryChangeTowerType` sürüklemeyi iptal edip koşulları doğrular. Yeni kule başarıyla hazırlanmadıkça satış/ödeme yapmaz. Başarıda bakiye tek seferde güncellenir:

`Yeni altın = mevcut altın + satış toplamı − dönüşüm ücreti`

Dönüşüm ücreti eskisi gibi `max(0, yeni kule fiyatı − eski kule fiyatının %50'si)`dir. Satış geliri ödeme için kullanılabilir; fazlası oyuncuda kalır. Satılan çanta kayıtları çıkarılır, depo/teslimler devredilir ve eski kule işlem dışına alınır. Çift onay ikinci kez para kazandıramaz.

Örnek: 200 altınlık dönüşümde oyuncunun 50 altını ve fiyatı 151 olan iki çanta eşyası varsa gelir 75 + 75 = 150'dir. İşlem sonrası bakiye 0 olur. Depoya dokunulmaz.

## Uygulanan sahne düzenlemesi

`Tools/Validation/setup_panel_improvements.cs` çalıştırıldı ve GameScane kaydedildi. Satış alanı 724×34'ten **724×60** boyutuna çıktı. Taşmayı önlemek için birleşik panel 1600×930 oldu; ekran ölçeklemesi buna göre ayarlandı. Onay metni 17 punto; asker kaldırma uyarısı dahil 259 piksel metin, 326 piksel ayrılmış alana sığıyor.

## Doğrulama

`Tools/Validation/test_panel_improvements.cs`: ESC sırası, iç/dış tıklama, satın alma slotunu serbest bırakma, sürükleme iptali, satış uyarısı, satış geliriyle ödeme, yetersiz bakiye, depo koruma, ana eşya/seviye sıfırlama ve çift onay.

Bu betikteki 20 kontrol ve üç ek kontrol geçti: pause dışına tıklama, satın alma dışına tıklamada slotun açılması, fazla satış gelirinin oyuncuda kalması. Kontroller oyun metodlarını ve ekran koordinatı kontrollerini çağırır; fiziksel klavye/fare otomasyonu kullanılmadı. Game View'da satış alanı/onay yazısı taşmıyor, Console temiz. Sonuçlar `Docs/Validation/panel-improvements-2026-10-10.json` dosyasında. Önceki tarihli dönüşüm testlerindeki dolu-çanta engeli eski kuralı belgeler; bu özellik için güncel kontrol kaynağı `test_panel_improvements.cs` dosyasıdır.
