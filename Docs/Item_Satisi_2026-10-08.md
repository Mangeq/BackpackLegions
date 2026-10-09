# Eşya satışı — 8 Ekim 2026

Durum: Tamamlandı. Satış alanı GameScane sahnesindeki birleşik panelin altına bağlandı ve kaydedildi. Canlı Play oturumunda 11 kontrol geçti; ekran görüntüsü incelendi, son konsol kontrolünde hata/uyarı yoktu. Test oturumu kapatıldı.

## Onaylanan kurallar

Çanta ve her kulenin kendi deposundaki eşyalar Sat alanına bırakılır. Geri ödeme ItemData.cost / 2: 151 altınlık eşya 75 altın verir. Fiyat bırakmadan önce görünür. Yalnız hazırlıkta ve oyun duraklatılmamışken satılabilir. Başlangıç silahı korunur; satın alınmamış teklifler satılamaz.

## Kod akışı

Yeni yönetici sınıfı eklenmedi. BackpackDraggable mevcut taşıma işleminin kararlarını vermeye devam eder. BeginDrag yerleşik eşyayı kaynak çanta/depodan geçici çıkarır, önceki konumunu ve yönünü saklar. FinishDrag önce fazı ve seçili kuleyi doğrular; sonra imlecin satış alanında olup olmadığını kontrol eder. Satış uygunsa EconomyManager.AddGold yalnız bir kez çağrılır ve eşya geri yerleştirilmez. isDragging hemen kapatıldığı için ikinci bırakma olayı yeniden ödeme yapamaz.

Satış yasaksa mevcut geri alma yolu eşyayı kendi kaynağına eski konum/yönüyle koyar. Pause, kule değiştirme ve savaş başlangıcı da aynı iptal yolunu kullanır. Kaynak silah koruması TowerData.defaultWeapon referansına dayanır; bu veriyle oluşturulan tüm kopyalar korunur, ana item ayrıca depoya konulamaz.

BackpackUIManager yalnız satış alanının ekran koordinatlarını, fiyat yazısını ve renklerini yönetir. EconomyManager.GetSellValue tek fiyat hesabını sağlar; önizleme ile ödeme aynı hesabı kullanır. Pozitif tam sayı bölmesi küsuratı atar, negatif maliyetler sıfıra sınırlandırılır.

İşlem sonunda Tower.CalculateStats çağrılır. Bu yöntem yalnız Backpack.Items üzerinden bonus hesapladığından çantadan satılan eşyanın etkisi kalkar, depodan satılan eşya statları değiştirmez.

## Doğrulama

Tools/Validation/setup_item_sale_ui.cs Editor API ile mevcut panelin altına satış alanını ekledi. Tools/Validation/item_sale.cs ile fiyat önizlemesi, tek ödeme, stat etkisi, başlangıç silahı, pause, kule değişimi, mağaza teklifi ve savaş iptali sınandı (9/9). Tools/Validation/item_sale_extra.cs ile depodaki başlangıç silahı koruması ve panel dışına bırakma sınandı (2/2).

Sonuçlar: [Ana testler](Validation/item-sale-2026-10-08.json), [ek testler](Validation/item-sale-extra-2026-10-08.json). Testler gerçek Play oturumunda sentetik sürükleme olayları kullanır; elle oynanış testi yerine geçmez. Test eşyaları ve kuleleri yalnız geçici Play oturumunda oluşturuldu. İlk denemede arka planda duran Editor kulelerin Start metodunu çalıştırmadığından test kurulumu hata verdi; arka plan çalışma ayarı geçici açılınca testler geçti ve ayar geri kapatıldı.

## Ana item depo kilidi — 8 Ekim ek kuralı

Kule türünü belirleyen TowerData.defaultWeapon çantada kalır. BackpackDraggable.EvaluatePlacement hedef depo olduğunda bu itemi reddeder. Hem kırmızı yerleşim önizlemesi hem bırakma işlemi aynı kontrolü kullanır. Reddedilen taşımada önceki konum ve yön geri yüklenir; çanta içindeki taşıma ve döndürme devam eder. Yeni sınıf eklenmedi.

Güncel kod Unity'de yenilenip derlendikten sonra ana itemin depoya reddedilmesi, çanta içi taşıma ve normal itemin depoya girmesi testleri 3/3 geçti. Rapor: Validation/main-item-storage-2026-10-08.json. Önceki depoda ana silah testi tarihsel/savunma amaçlıdır; oyuncu artık ana itemi depoya taşıyamaz.

