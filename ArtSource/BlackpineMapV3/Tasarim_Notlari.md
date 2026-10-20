# Karaçam V3 — çevre yoğunluğu ve savunma üssü

Blender kaynak tasarımıdır. Unity sahnelerine, Assets dosyalarına veya oyun koduna aktarım yapılmadı.

## Çevre

- Bitki örtüsü rastgele homojen dağıtılmak yerine 10 orman altı kümesi çevresinde yoğunlaştırıldı.
- Eğrelti otları, küçük çamlar, çalılar, çimen bıçakları, yosunlu taşlar ve yaprak döküntüleri eklendi.
- Yol kenarlarına küçük taşlar, boş köşelere oduncu alanı, tomruk istifi, eski mezar taşları ve küçük bir yol tapınağı yerleştirildi.
- Yol çevresindeki geçiş koridoru ve beş kule alanının yakın çevresi bitki dağıtımından çıkarıldı.

## Üs

Yolun doğu çıkışında, mevcut 56 × 48 m harita sınırları içinde taş ve ahşaptan bir sınır karakolu tasarlandı. Yol batı kapısından avluya girer ve ana binaya ulaşır.

- Taş temel, döşeli avlu, alçak mazgallı surlar ve iki gözetleme kulesi.
- Açık ahşap giriş kapısı ve kapı önünde iki mangal.
- Ahşap hatıllı taş ana bina; tek tek kiremitlerden mavi-yeşil çatı, baca ve pencereler.
- Bordo ve altın lejyon sancakları.
- Erzak sandıkları, variller, kuyu, silah rafı ve yüksek işaret sancağı.

Üssün alanındaki önceki ağaç ve dekorlar silinmeden görünümden gizlendi. Beş kule koyma alanı ve küçük köprü korunur. Üs canı, saldırı hedefi, çarpışma, navigasyon veya oyun mantığı eklenmedi.

## Dosyalar

- `Blackpine_Map_V3_With_Base.blend`: düzenlenebilir harita; ana kamera ve çimen dokusu dosyada.
- `Map_V3_Base_Beauty.png`: genel görünüm.
- `Base_Closeup.png`: üs detay görünümü.

Yeni içerik `V3_Understory`, `V3_Environmental_Stories`, `V3_Base_Architecture` ve `V3_Base_Details` koleksiyonlarında düzenlenmiştir. Yeniden üretim betiği: `Tools/Environment/build_map_v3.py`.
