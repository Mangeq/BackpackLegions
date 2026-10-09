# Sinerji ve evrim — birlikte değerlendirilecek mimari

Durum: Emir görev dağılımını onayladı; 8 Ekim'de uygulandı. Bu belge tasarım kararlarının kaydıdır. Gerçek uygulama, ilk sürüm sınırları ve testler için [uygulama açıklaması](Sinerji_Evrim_2026-10-08.md). İlk sürüm bir başarılı dalga kullanır; çok dalgalı sayaç eklenmedi.

## Mevcut durum

ItemData eşya şekli ve stat bonuslarını tutuyor. synergyIDs alanının çalışan bir hesaplayıcısı yok; SynergyLineManager boş. Tower.CalculateStats eşyaların bonuslarını doğrudan topluyor. Eşyaların ayrı saldırı/hasar sistemi yok: saldırıyı kule yapıyor. WaveManager son düşman temizlenince hazırlığa veya zafere geçiyor. Evrim burada, son dalga dahil başarılı dalga tamamlanmasına bağlanmalı; yalnız BeginPreparation kullanmak son dalgayı atlar.

## Önerilen görev dağılımı

- ItemData: özel bonus kareleri, kabul filtresi (herhangi bir eşya / ItemType / belirli ItemData) ve bonus değerleri. Paylaşılan asset'e runtime bonus yazılmaz.
- ItemEvolutionRecipe (yeni ScriptableObject): iki malzeme, oluşan eşya, gereken dalga sayısı (ilk sürümde 1). Tariflerin Inspector'dan düzenlenmesi.
- ItemSynergyCalculator (yeni, sahneye bağlı olmayan C# sınıfı): yerleşim, döndürülmüş özel kareler ve kabul filtresinden her yerleşik eşyanın etkin bonus katkısını hesaplar. UI veya altına müdahale etmez.
- TowerItemEvolution (yeni, her Tower'ın sahip olduğu normal C# nesnesi): dalga başında tarif çiftlerini kaydeder; başarılı dalga sonunda malzemeleri bir kez tüketir, ürünü yerleştirir; bu kuleye ait teslim bekleyen ürünleri tutar. Aynı malzeme iki tarifte kullanılamaz. Yeni ürün aynı dalgada ikinci kez evrilmez.
- Tower: mevcut çanta/depo sahipliğini korur; etkin eşya katkılarını CalculateStats içinde toplar, evrim nesnesinin sahibidir.
- WaveManager: dalga gerçekten başladığında ve başarıyla bittiğinde kulelere bildirim gönderir. Mağlubiyet/iptal evrim tamamlamaz. GameManager mevcut faz yönetimini korur.
- BackpackUIManager / BackpackDraggable: mevcut yerleşim akışına özel kare gösterimi, evrim bilgisi ve teslim alma kartlarından sürükleme eklenir. Karttan başarısız bırakma ürünü kaybettirmez. Depo ve teslim alanı bonus üretmez.

## Önerilen ayrıntılar

Özel kareler eşyanın yerel koordinatlarıyla tanımlanır ve onunla döner. Çapraz temas evrim sayılmaz. Bir eşya birden fazla özel kareye değse de aynı bonus kuralında bir kez sayılır; ilk sürümde kural aktif/pasif olarak çalışır. Birden fazla kuralın etkisi toplanır; birbirini sonsuza kadar artıran hesap döngüsü olmaz.

Evrim çifti dalga başındaki çantadan seçilir. Ürün önce tüketilen malzemelerden boşalan yer dahil çantada aranır, sonra depoda; ikisinde de yer yoksa o kulenin sınırsız teslim listesine eklenir. Evrim beklemez. Çakışan tariflerde Inspector tarif sırası, sonra çanta hücre sırası kullanılması önerilir; oyuncu başlamadan hangi çiftin evrileceğini görebilir.

Ana item depoya konulamaz/satılamaz kuralı korunmalı. İlk sürümde ana itemin evrim malzemesi olmaması önerilir; ana item evrimi istenirse yeni ürünün ana item kimliğini nasıl devralacağı ayrıca tasarlanmalı.

Önemli karar: Mevcut yapıda itemler ayrı saldırmıyor. Öneri, sinerjinin ilgili itemin kuleye sağladığı katkıyı artırmasıdır. Örneğin normalde +%10 hasar veren item, etkin sinerjiyle +%15 sağlar; diğer itemlerin katkıları değişmez. Bağımsız item saldırıları istenirse bu önerinin kapsamını aşan savaş mimarisi gerektirir.

## Uygulama ve doğrulama sırası

Önce sinerji hesabı ve döndürme/çok hücreli temas testleri; sonra dalga bazlı evrim ve malzeme tek kullanım testleri; sonra çanta/depo/teslim akışı ve iptal testleri. Son olarak örnek tarifler, görsel göstergeler, gerçek dalga ve son dalga doğrulaması, yol haritası ve kod açıklaması.
