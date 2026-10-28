# Kule ve element değişimi — 27 Ekim 2026

- Kule tipi: hedef tür seçilir, değişim bedeli ve çantadaki satılabilir eşyaların toplam getirisi onay kartında gösterilir. Takılı parçalar satışa dahildir. Kabul yeni kuleyi seviye 1, XP 0, Normal element ve başlangıç silahıyla kurar. Depo ve bekleyen teslimler korunur. Reddet ücret almaz.
- Element: Kabul et / Reddet kartı önce açılır. Kabul anında eski ana silah nötr sürümüyle değiştirilir ve kule Normal elemente geçer; element seçim mağazası açılır. Mevcut TowerData yeniden açma bedeli korunur (100 altın); katalizörün mevcut fiyatı ayrıca ödenir (80 altın). İki bedel kartta açıkça belirtilir. Yeni katalizör ana silaha komşu yerleştirilene kadar, altın yetmese veya savaş başlasa da kule nötr kalır. Seviye/XP korunur. Ana silaha takılı parçalar dönüşümde depoya aktarılır.
- Mağaza farklı element katalizörlerini birer kez sunar; mevcut element sunulmaz. Uyumsuz eski element eşyaları silinmez, bonus vermez ve çanta/depo/takılı parça üzerinde İŞLEVSİZ etiketi alır.
- Son canlı Unity kontrolündeki boyutlar: okçu 1,00; büyücü 0,70; eski havan 0,65; NewMap havan 2,50. Kullanılan kule prefablarının gerçek mermi referansları ve oyun kamerasındaki dünya boyutları doğrulandı.
- Arayüz kaplamasının onay kartının Graphic bileşenlerini kapattığı bulundu; kaplama artık onay kartını dışarıda bırakır ve kart açılırken görseller etkinleştirilir.

Canlı Play Mode: dört kulede kabul/red, yetersiz altın, tek sefer ücret, dört ayrı ilk element teklifi, işlevsiz eşya etiketi, tür değişimi satış hesabı ve level/XP/çanta sıfırlama için 44 kontrol. Gerçek ateş ve su katalizörüyle ek dönüşüm kontrolleri. Test verileri Play Mode'dan çıkınca atılır; oyuncu kayıtları değiştirilmez.

Nötr geçiş düzeltmesi: dört kulede ateş elementi, reddetme, kabul anında 0 altınla nötr ana silah, stat yenilemede nötr kalma, Combat durumunda nötr kalma ve yeni su katalizörüyle tekrar dönüşüm için 24 kontrol geçti. Kayıt: [nötr element regresyonu](Validation/neutral-element-2026-10-27.json). Unity hata kaydı boş.
