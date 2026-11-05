# Blackpine V6 — büyük orman ve kale testi

- Unity sahnesi: `Assets/Scenes/Blackpine_V6_ForestCastle_Test.unity`.
- Harita prefabı: `Assets/Art/BlackpineV6Test/BlackpineV6Map.prefab`.
- Blender kaynağı: `Blackpine_V6_Forest_Castle.blend`.

Harita 56 × 48 metreden 84 × 72 metreye büyütüldü: her eksende 1,5 kat, alan olarak 2,25 kat. Yeni çevre kuşağına 856 çam ve orman altı objeleri eklendi. Nehir ve giriş yolu yeni sınıra uzatıldı. Beş kule alanı ve referans kule korundu.

Küçük kulübenin yerine iki kısa burç, alçak sur, kapı, avlu ve ahşap hatıllı ana binadan oluşan yaklaşık 6,56 × 8,86 m bir kale kullanıldı. Kule temasını koruyan gri taş, sıcak ahşap ve hardal renkler açıldı. Kamera ve gün ışığı test sahnesine eklendi.

Aktarım, yapılandırılmış `unityMCP` yerel sunucusuna standart MCP SDK istemcisiyle canlı bağlanarak yapıldı. Kaynak FBX 21 görünür gruba bölündü; gizli eski sürümler dışarı aktarılmadı. Yaklaşık 1,74 milyon üçgen vardır: bu bir görsel test sürümüdür, üretim performansı için LOD/occlusion optimizasyonu yapılmamıştır.

Ana GameScane sahnesinde kaydedilmemiş değişiklikler bulunduğundan, sahne değiştirilmeden önce `Assets/Scenes/Backups/GameScane_Before_BlackpineV6.unity` dosyasına Save As Copy yedeği alındı. Ana GameScane dosyası ve build başlangıç sırası değiştirilmedi.

Sahne doğrulamasında eksik script veya kırık prefab bulunmadı; materyal/URP shader kontrolü temiz. Düşman dalgaları, kule satın alma ve üs hasarı bu bağımsız görsel test sahnesine bağlanmadı.
