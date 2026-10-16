# Zorluk dengesi — 13–14 Ekim 2026

> Bu sert denge, sonraki oyuncu geri bildirimiyle yumuşatıldı. Güncel farklar: [Hover ve denge](Hover_ve_Denge_2026-10-14.md).

Bu revizyon, Emir'in beş bölümü element seçmeden ve bütün yerleri doldurmadan kolayca tamamladığı oynanış geri bildirimine dayanır. Önceki bölüm rehberindeki düşman toplamları, can/hasar artışları ve ×1,5/×0,5 kampanya değerleri bu revizyonla değişmiştir.

## Ekonomi ve içerik

| Bölüm | Dalga | Eski düşman | Yeni düşman | Başlangıç altını | Boss canı |
|---|---:|---:|---:|---:|---:|
| 1 | 5 | 40 | 180 | 700 | 3.000 |
| 2 | 7 | 60 | 260 | 850 | 4.800 |
| 3 | 9 | 80 | 340 | 1.000 | 6.600 |
| 4 | 11 | 100 | 420 | 1.150 | 8.400 |
| 5 | 13 | 120 | 500 | 1.300 | 10.200 |

Başlangıçta kullanılan 10.000 test altını kaldırıldı. Her bölüm girişi ve yeniden başlatma ChapterData.startingGold değerini yeniden uygular. Önceki bölümden statik Gold taşınmaz. GameOver ve RestartGame içindeki eski sabit 1.000 atamaları kaldırıldı; bütçenin tek kaynağı bölüm verisidir.

Düşman adedi yaklaşık 4,2–4,5 kat arttı; gelir de sınırsız büyümesin diye ödüller ve eşya fiyatları birlikte ayarlandı. Eşya fiyatları %15 artırılıp bir sonraki 5 altına yuvarlandı; satış hâlâ fiyatın yarısıdır. İlk bölümde Scout/Raider/Guard/Elite/SkyWisp ödülleri 12/16/20/26/18; her bölümde +1 altın. Boss 150 + bölüm indeksi ×30 altın verir. Bölüm indeksi burada ilk bölüm için 0'dır.

İlk dalga 20/23/26/29/32 keşifçidir. Kalan düşmanlar sonraki dalgalara artan ağırlıklarla dağıtılır. İkinci dalga iki tür; üçüncü dalga hava; sonraki dalgalar iki veya üç tür. Her finalde tek boss ve destekler bulunur. Destekler hemen, boss 8 savaş saniyesi sonra doğar. Doğma aralıkları bölüm ve dalga ilerledikçe azalır; duraklatma bunları da durdurur. Hazırlık süreleri bu revizyonda değiştirilmedi.

## Düşman gücü

| Tür | İlk bölüm canı | Bölüm başına ek can | İlk bölüm saldırısı | Kaleye hasar |
|---|---:|---:|---:|---:|
| Scout | 45 | 14 | 9 | 5 |
| Raider | 90 | 30 | 14 | 8 |
| Guard | 170 | 55 | 22 | 12 |
| Elite | 250 | 80 | 30 | 16 |
| SkyWisp | 95 | 35 | 14 | 8 |
| Warlord | 3.000 | 1.800 | 55 | 100 |

Birliğe saldırı hasarı bölüm başına temel değerin %18'i kadar artar. Bossun saldırı aralığı 0,9 saniye; diğerleri 1,2 saniye. Boss, birlikleri uzun süre tek başına oyalamak için bırakılabilecek zararsız bir hedef değildir. Aktif kale 100 can olduğu için boss kaçarsa yenilgi gelir. Kaleye verdiği hasar, birliğe vurduğu hasardan ayrı ayarlanır.

## Element artık neden önemli?

Döngü değişmedi: **Su → Ateş → Hava → Toprak → Su**.

- Zayıflığa vuruş: **×2**.
- Dirence vuruş: **×0,3**.
- Eşleşmeyen diğer elementler: **×1**.
- Elementsiz saldırı: keşifçide **×0,9**, diğer normal düşmanlarda **×0,75**, bossta **×0,45**.

Örneğin Ateş bossuna ham 100 hasar: Su ile 200, Hava ile 30, Normal ile 45, Ateş/Toprak ile 100. Doğru elementli atış, Normal atışın yaklaşık 4,44 katı etkilidir. İlk element hâlâ 5. seviyede seçilir. Erken dalgalarda XP kazanıp sonraki hazırlıkta seçim yapmak gerekir. Depo buff vermez; çantadaki uyumsuz eşya kuralları değişmedi.

EnemyData'ya weaknessMultiplier, resistanceMultiplier, normalAttackMultiplier alanları eklendi. Eski kampanya dışı düşmanların varsayılanı 1,5 / 0,5 / 1 olarak korunur. Gerçek hasar yine Enemy.TakeDamage → EnemyData.GetDamageMultiplier üzerinden bir kez hesaplanır. Bilgi panelleri artık sabit çarpan yazmaz, aynı verileri okur.

## Kuleler ve yerleşim

| Kule | Hasar | Atış aralığı | Menzil | Fiyat | Amaç |
|---|---:|---:|---:|---:|---|
| Okçu | 13 | 1,1 sn | 4,7 | 250 | Uzun yol kapsaması, kara/hava temizliği |
| Büyücü | 29 | 1,9 sn | 3,4 | 325 | Kıvrım/durdurma noktalarında tek hedef ve element hasarı |
| Havan | 46 | 3,4 sn | 4,8 | 375 | Yalnız karada, bir arada tutulan kalabalıklar |
| Birlik | 4 / asker | 1,35 sn | Mevcut asker yakalama alanı | 225 | Düşmanı diğer kulelerin ateşi altında tutmak |

Havan patlama yarıçapı 1,5'ten 1,25'e indi. Birlik sayısı, 6 saniyelik yeniden doğma süresi ve askerlerin 60 temel canı korunur. Başlangıç kritik ihtimali %5'tir.

Yeni arazi bonusu veya kuleye gizli konum çarpanı eklenmedi. Yerleşimin etkisi gerçek menzil, düşmanın menzilde geçirdiği süre, havanın alan hasarı, kara/hava hedefleme ve birliklerin tutma noktalarından gelir. Mevcut NavMesh üzerinde ölçülen yol uzunluğu yaklaşık 22,5 birim. Aynı büyücü merkezi kıvrımdaki konumdan yaklaşık 10,6 birim yolu, kale tarafındaki konumdan 4,8 birim yolu kapsıyor. Bu bir hasar garantisi değildir; hedef yoğunluğu ve durdurma süreleri de etkilidir. Eski PlaneTest alanı değiştirilmedi.

## Seviye ve eşya büyümesi

Eski seviyede +5 sabit hasar, +%5 kritik ve −0,1 sn bekleme vardı. Özellikle düşük temel hasarlı birlik kulesi orantısız büyüyor, yüksek seviyede bekleme 0,1 saniyeye yaklaşıyordu.

Artık kazanılan seviye sayısı `n = Level - 1`:

```
temelHasar = TowerData.damage × (1 + n × 0,05)
temelAtışAralığı = TowerData.attackSpeed / (1 + n × 0,015)
temelKritik = TowerData.criticalChance + n × 0,003
nihaiKritik = en fazla %60
```

Tower.RebuildLevelStats bu hesabı hem ilk kurulumda hem seviye artınca yapar. Büyüme değerleri TowerData Inspector'ında bulunur. Eşya bonusları bundan sonra mevcut CalculateStats içinde uygulanır. İlk seviye için XP 24, her sonraki seviye eşiğine +12 gerekir. İlk element için toplam 168 XP gerekir; seçim beklerken XP kaybolmaz.

Normal eşyaların doğrudan hasar bonusları ×0,8, hız bonusları ×0,75; elementli eşyaların hasar bonusları ×0,9, hız bonusları ×0,85 yapıldı. Bütün eşyalarda doğrudan kritik bonusu ×0,6. Can bonusları, şekiller, sahiplik, komşuluk sinerjisi değerleri ve evrim tarifleri korunur. Böylece yalnız çantayı doldurmanın getirisi azalırken doğru komşuluk ve evrim planlamasının göreli değeri artar.

## Dosyalar ve geri izleme

- ChapterData: başlangıç bütçesi ve dalga içeriği.
- GameManager: her girişte bölüm bütçesini başlatır.
- EconomyManager: 700 güvenli yedek değer; artık 10.000 test parası yok.
- TowerData / Tower: seviye büyümesi ve kritik sınırı.
- EnemyData / Base: element katsayıları, boss işareti ve ayrı kale hasarı.
- WavePreviewUI / InfoPanelUI: güncel katsayılar ve boss kaçış uyarısı.
- MainMenuManager: bölümün başlangıç parasını gösterir.
- Editor/HardBalancePass: bir defalık veri uygulayıcısıdır; çalışma zamanı yöneticisi değildir, tekrar uygulanmasını engeller.

Asıl düzenlenecek asset'ler yine Data/Chapters, Data/EnemyData/Campaign, Data/TowerData ve Data/Items altındadır. Denge öncesi 279 asset'in JSON kaydı `Docs/Validation/hard-balance-original-assets-2026-10-13.json` içinde saklandı. Orijinal Chapter01 düşmanları değiştirilmedi; oynanan kampanya varyantları değişti.

## Test sınırları

Kontrollü savaş karşılaştırmaları rastgele mağaza tekliflerinden bağımsız sabit kurulumlar kullanır. Eşya ve kule değerleri gerçek oyun verisidir; boss senaryoları yalnız bossu izole eder ve destek dalgasını içermez. Bunlar tam bölüm oynanışının veya bütün buildlerin geçilebilirliğinin kanıtı değildir. Nihai test sonuçları `Docs/Validation/hard-balance-combat-2026-10-13.json` ve `hard-balance-data-2026-10-13.json` dosyalarındadır.


## Tamamlanan ölçümler

- 274 veri/gerçek hasar kontrolü geçti.
- 8 bölüm bütçesi/yeniden başlatma/boss önizleme kontrolü ve 5 menü metni kontrolü geçti. Son Console hata/uyarı listesi boş.
- 630 altın değerinde iki başlangıç okçusu ilk dalgayı 100 kale canıyla geçti. Test kurulumunda alışveriş doğrudan yapıldı; gösterilen Gold'dan düşülmedi, maliyetler nesne adlarında ayrıca kaydedildi.
- 2.030 altın değerinde iki 5. seviye, Normal elementli ve dolu çantalı büyücü izole ilk boss karşısında yenildi.
- 3.050 altın değerinde beş 7. seviye kule (havan, iki büyücü, birlik ve okçu), doğru Su elementiyle aynı izole bossu geçti.
- Güçlü testin XP'si ve eşyaları doğrudan verildi; rastgele tekliflerden bu buildi kurmanın ekonomik erişilebilirliği veya boss destek dalgasının temizlenmesi bu testte doğrulanmadı.

Yeni dengeyle tam beş bölüm koşusu hâlâ oyuncu testine ihtiyaç duyuyor. Unity Play kapalı ve Boot sahnesinde bırakıldı.
