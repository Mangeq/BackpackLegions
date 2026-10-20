# Unity — Blackpine V5 test sahnesi

Sahne: `Assets/Scenes/Blackpine_V5_Test.unity`.
Yeniden kullanılabilir harita prefabı: `Assets/Art/BlackpineV5Test/BlackpineV5Map.prefab`.

Görsel Blender tasarımından aktarılmıştır: küçük üs, kompakt köprü, beş numaralı yerleştirme alanı, çevre detayları ve 02 numaralı alandaki referans kule. 35 URP/Lit materyal ve çimen dokusu bağlıdır. Yolun ters normalleri dışa aktarımda düzeltilmiştir. FBX model köküne Y ekseninde 180 derece dönüş uygulanarak Blender düzeni korunmuştur.

17 grupta yaklaşık 497 bin üçgen vardır. Zemin, yol, köprü ve beş alanda toplam 8 MeshCollider bulunur. Bu, üretim performansı için optimize edilmiş bir harita değildir; görsel test içindir.

Sahne açılıp Play ile görüntülenebilir. Ortografik kamera ve yönlü ışık dahil edilmiştir. Düşman dalgaları, kule satın alma/yerleştirme, üs hasarı ve ana menü bağlantısı bu sahneye henüz bağlanmamıştır. Mevcut oyunun başlangıç sahnesi veya build sırası değiştirilmemiştir.

Aktarım öncesi açık GameScane sahnesinde kaydedilmemiş düzenlemeler bulunduğu için `SaveScene(..., saveAsCopy: true)` ile `Assets/Scenes/Backups/GameScane_Before_BlackpineV5_Test.unity` yedeği alındı. Asıl GameScane dosyası kaydedilmedi veya değiştirilmedi. Önceki açık düzenlemeler bu yedek sahnede bulunur.

Kontroller: Play Mode, beş alanın varlığı, materyal/doku referansları, sahne doğrulaması ve konsol kontrolü.
