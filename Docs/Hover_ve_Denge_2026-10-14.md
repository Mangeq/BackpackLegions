# Dalga paneli ve ölçülü kolaylaştırma

Paneldeki sorun iki ayrı açık kalma koşuluydu: `expanded` tıklama ve hazırlık başlangıcından, `hovering` ise fare girişinden geliyordu. Görünürlük bu ikisinden biri doğruysa açıktı. Fare üstündeyken tıklamak expanded değerini kapatsa da hovering açık kaldığından panel kapanmıyordu.

WavePreviewUI içinde expanded ve IPointerClickHandler kaldırıldı. Awake paneli ilk kareden önce gizler. OnPointerEnter bilgiyi yenileyip paneli açar; OnPointerExit anında gizler. Update artık yalnız hover durumunda, pause/sonuç ekranı yokken görünürlüğe izin verir. Dalga değişimi yalnız metni yeniler. OnDisable hover bilgisini sıfırlar ve paneli kapatır; tekrar etkinleştirmek kendiliğinden açmaz. Panel hiyerarşisi yeniden kurulmadı, yeni sınıf eklenmedi.

Kolaylaştırma önceki sert dengenin üzerine uygulandı:

| Bölüm | Başlangıç altını | Boss canı |
|---|---:|---:|
| 1 | 850 | 2.400 |
| 2 | 1.000 | 3.840 |
| 3 | 1.150 | 5.280 |
| 4 | 1.300 | 6.720 |
| 5 | 1.450 | 8.160 |

Normal düşmanların canı %12, bossların canı %20 azaldı. Altın ödülleri %10 artırılıp tam sayıya yukarı yuvarlandı. Doğma aralıkları %10 uzadı; toplam düşmanlar 180/260/340/420/500 olarak kaldı. XP, dalga sayısı, kule/eşya değerleri ve hazırlık süreleri değişmedi.

Element avantajı ×2 olarak kaldı; direnç ×0,3 yerine ×0,4. Elementsiz saldırı keşifçide ×0,95, diğer normal düşmanlarda ×0,85, bossta ×0,6. Doğru element hâlâ bossta Normal saldırının yaklaşık 3,33 katı etkilidir. Boss kaleye ulaşırsa yenilgi kuralı korunur.

Değişiklikler ChapterData ve EnemyData kampanya asset'lerine Unity AssetDatabase aracılığıyla kaydedildi. Önceki değerler `Docs/Validation/before-soften-2026-10-14.json` dosyasında saklandı. Tekrar çalıştırılarak yüzdelerin üst üste uygulanmaması için uygulama betiği yedek dosyası varsa durur.

Doğrulama raporu: `Docs/Validation/hover-only-2026-10-14.json`. Fare giriş-çıkışları, yeniden açılmama, pause, disable, dalga değişimi ve beş bölümün yeni bütçe/boss değerleri kontrol edilir. Bu kontroller bütün bölümlerin yeni dengeyle baştan sona oynandığı anlamına gelmez.

Sonuç: 34/34 kontrol geçti. Son Console hata/uyarı listesi boş; Unity Play kapalı, Boot sahnesinde bırakıldı.
