# V5 — UnityTower ile uyumlu harita

Tema referansı: kullanıcının `UnityTower.fbx` modeli. İletideki masaüstü kök yolu yerine dosya `Desktop/Backpack         Legions/Kule Tasarımları/UnityTower.fbx` altında bulundu. Projedeki `Assets/UnityTower.fbx` ile SHA256 özeti aynıdır: `B507ED36473F9BFCA38EACBB33A36A32B79D8D4EB73A5304FC430D7B2E5FDCE6`.

Kulenin gerçek FBX materyalleri incelendi: koyu gri dikmeler ve kenarlar, sıcak meşe/kestane yüzeyler, küçük altın-pirinç bağlantılar, siyah ve hardal tonlu sancaklar. Modelde harici doku referansı yoktur.

## Uyumlaştırma

- Küçük üssün yeşil çatısı sıcak kahverengiye, bordo sancağı hardal/siyaha dönüştürüldü.
- Ahşap, kapı ve kenar materyalleri doğrudan referansın paletinden türetildi.
- Ana binaya kuledeki gibi koyu köşe bilezikleri ve seyrek pirinç bağlantılar eklendi.
- Köprü, kule koyma çeperleri, taş duvarlar ve çevre yapılarının açık taşları koyulaştırıldı.
- Çimen dokusunun renk değişimi azaltılarak mimarinin okunması desteklendi.
- Üssün 6,2 × 6,2 m alanı, küçük köprü, çevre detayları ve beş kule alanı korundu.

## Referans önizlemesi

FBX geometrisi ve özgün materyalleri, görünüm karşılaştırması için 02 numaralı alanın üzerinde tek bir kule olarak gösterilir. Kule yüksekliği yaklaşık 4 m'ye ölçeklenmiştir. `REFERENCE_PREVIEW_Tower_On_Pad_02` koleksiyonu gizlenerek alan tekrar boş görülebilir. Bu işlem Unity'ye model yerleştirme veya oynanış ekleme değildir.

`SOURCE_UnityTower_Hidden` koleksiyonunda içe aktarılan kaynak parçalar gizli olarak saklanır. Özgün FBX dosyası değiştirilmemiştir.

## Teslimat

- `Blackpine_V5_Tower_Theme.blend`: paketlenmiş çimen dokusuyla düzenlenebilir kaynak.
- `Map_V5_Tower_Theme.png`: haritanın tamamı.
- `Base_V5_Tower_Theme.png`: üs yakın görünümü.
- `Tower_V5_In_Map.png`: referans kulenin haritadaki görünümü.

Unity sahnelerine, Assets dosyalarına veya oyun koduna değişiklik yapılmadı. Tüm çıktılar ArtSource içinde.
