from pathlib import Path
import json,re
root=Path('Docs/Playtests/2026-10-27')
runs=[('İlk tur',root),('Hava savunması',root/'retry-air-defense'),('Önde birlik',root/'retry-front-blocker'),('Erken eşya yatırımı',root/'retry-campaign-investment')]
rows=[];checks=[];purchases=0;spend_total=0;consoles=[]
for label,folder in runs:
 events=json.loads((folder/'events.json').read_text(encoding='utf-8'))
 before=None;spend=0
 for e in events:
  data=e['data'];kind=e['kind']
  if kind=='result':rows.append((label,data['chapter'],data['waves'] if data['state']=='Victory' else data['wave']+1,data['waves'],data['state'],data['hp']))
  if kind=='state' and data['state']=='Preparation':before=data['gold'];spend=0
  if kind in ('buy_towers','shop'):
   values=[int(x) for x in re.findall(r'spent=(\d+)',data)];purchases+=len(values);spend+=sum(values);spend+=sum(int(x) for x in re.findall(r'reroll -(\d+)',data))
  if kind=='prepared':checks.append({'run':label,'chapter':data['chapter'],'wave':data['wave']+1,'before':before,'spent':spend,'after':data['gold'],'passed':before-spend==data['gold']});spend_total+=spend
  if kind=='console':consoles.append({'run':label,'data':data})
(root/'economy-checks.json').write_text(json.dumps({'purchase_actions':purchases,'checks':checks,'total_spent':spend_total},ensure_ascii=False,indent=2),encoding='utf-8')
table='\n'.join('| '+str(c)+' | '+label+' | '+str(w)+' / '+str(n)+' | '+('Yenilgi' if result=='Defeat' else 'Zafer' if result=='Victory' else result)+' | '+str(hp)+' / 100 |' for label,c,w,n,result,hp in rows)
report=f'''# Backpack Legions — canlı oynama raporu

27 Ekim 2026 · Unity Editor · 1920 × 1080 · savaşlarda oyunun 3× hız seçeneği.

## Sonuç

Beş bölümün her birine ana menüden girerek oynadım; ilk bölümü iki farklı yerleşimle tekrar denedim. 2–5. bölümleri de erken eşya yatırımı planıyla ikinci kez oynadım. Kampanyayı kazanarak bitiremedim. İlk bölümü üçüncü denemede, boss dalgasını tamamlayarak **4/100 kale canıyla kazandım**. Sonraki bölümler için kapsam tablodaki sonuçlarla sınırlıdır; kazanılamayan bölümün son dalgaları doğrulanmış sayılmaz.

Temel döngü açılıyor ve çalışıyor. En belirgin ihtiyaç, oyuncuya yerleşimin ve erken eşya/element yatırımının önemini daha iyi anlatmak ve ilk bölümün farklı makul stratejilerle kazanılabilirliğini ölçmek. Bu denemelerden doğrudan genel hasar artırma veya düşman canı azaltma kararı çıkarmıyorum.

## Nasıl oynadım?

Boot → ana menü → bölüm haritası → hazırlık → gerçek dalgalar → zafer/yenilgi → ana menü akışını kullandım. Altın, düşman, hasar, XP, dalga süresi ve kule seviyelerine müdahale etmedim. Kuleler normal fiyatlarıyla alındı; eşyalar o anda sunulan mağazadan seçilip oyunun gerçek sürükle-bırak olaylarıyla yerleştirildi. Ücretli yenileme kullanıldı. Kule yerleşimi ve alışveriş tercihlerini koşular arasında değiştirdim.

Oynama, canlı Unity bağlantısından mevcut oyuncu işlemleri ve UI metotları/sentetik sürükleme olaylarıyla yönetildi. Fiziksel fareyle bütün tıklama alanlarının erişilebilirliği ayrıca test edilmiş sayılmaz. Kare sayısı ve oyun zamanı ilerledi; dondurulmuş bir sahne üzerinde sonuç üretilmedi. Görüntüler oyunun gerçek Game görünümünden alındı.

Mevcut kayıtta `archer.training` ve `archer.distance` açıktı. Bu bir sıfır kayıt oyuncusu deneyi değildir. Test sonunda başlangıçtaki kayıt, açık sahne ve arka planda çalışma ayarı geri getirildi. Oyun kodu ve denge verileri bu rapor turunda değiştirilmedi.

## Koşu sonuçları

Dalga sütunu, yenilginin gerçekleştiği dalgayı gösterir; tamamlanan dalga sayısı değildir.

| Bölüm | Deneme | Ulaşılan dalga | Sonuç | Son kale canı |
|---|---|---|---|---|
{table}

İlk turda 1–3. bölümlerin başlangıcında okçu → havan → birlik tercih edildi. 3. bölümde plan değişikliği nedeniyle iki havan/iki birlik oluştu; bu koşu temiz bir strateji karşılaştırması sayılmaz. 4–5. bölümlerde okçu/büyücü ağırlığı artırıldı. Her bölüm için en iyi yerleşimi bulduğumu iddia etmiyorum; teklifler de rastgele geldi. Rotasyonla en sıkı çanta yerleşimi ve kapsamlı sinerji optimizasyonu uygulanmadı.

İlk bölümdeki hava savunması tekrarında iki okçu ve bir büyücüyle başladım. Erken su elementi ve eşya yatırımı sonrası üçüncü dalga 100 kale canıyla tamamlandı; dördüncü dalga sonunda 52 can kaldı. Beşinci dalgada yenildim. Böylece ilk turun üçüncü dalga kaybının tek başına bozuk bir denge kanıtı olmadığı görüldü.

## Çalıştığını gördüklerim

- Boot, bölüm seçimi ve beş bölümün yüklenmesi; her bölümde doğru başlangıç bütçesi.
- Dört kule türünün normal bedellerle satın alınması, hazırlık/savaş geçişi, doğal düşman üretimi, ölüm ödülleri ve kule XP artışı.
- Mevcut tekliften eşya satın alma, gerçek çanta yerleşimi ve su katalizörünün ana silaha komşu yerleşince tüketilip kuleyi suya dönüştürmesi.
- Hazırlığa dönüşte yeni alışveriş, ücretli yenileme ve bir sonraki dalgayı başlatma.
- Savaşta duraklat/devam: oyun zamanı aynı kaldı; hız 3× → 0 → 3× döndü.
- Yenilgi ekranı ve ana menüye dönüş.

Alışveriş kayıtlarında **{purchases} satın alma işlemi** görüldü. **{len(checks)} hazırlık bütçesinin tamamında** başlangıç altını − gerçek harcama = kalan altın eşitliği sağlandı. Bu, satış/evrim/rezervasyonun bütün olasılıklarını kapsayan bir ekonomi garantisi değildir. Koşuların sonunda okunan Unity hata/uyarı kayıtları boştu.

## Bulgular ve öneriler

### 1. İlk bölüm, yerleşim ve erken yatırım bilgisini güçlü biçimde gerektiriyor

Aynı bölümde bir plan üçüncü dalgada biterken, diğer plan aynı dalgayı 100 canla geçti. Öndeki okçuyu erken güçlendirmek belirgin fark yarattı. Sonraki dalga bilgisi mevcut olsa da normal savaş ekranında yaklaşan hava tehdidi ve hangi kulelerin bunu durdurabileceği daha açık gösterilmeli.

**Öneri:** üçüncü dalga öncesine kısa bir hava savunması uyarısı; ilk katalizör açıldığında ana silaha komşu yerleştirme rehberi; kule seçerken menzil/rota önizlemesi. Ardından aynı ilk bölümü en az üç sabit strateji ve birkaç mağaza tohumu ile tekrar ölçelim. İlk ölçümde öncelik kazanılabilirlik ve öğrenilebilirlik olsun.

### 2. Birlik katkısı yerleşime çok duyarlı; yol dışında doğma iddiası doğrulanmadı

Hava savunması koşusunda son bölgeye kurulan birlik kulesi boss öncesinde hâlâ 1. seviyedeydi; yenilgi anında yalnız yaklaşık 0,48 XP almıştı. Öne kurulan birlik daha erken XP aldı. Bu bir rol/yerleşim bulgusudur; birlik sisteminin tamamen çalışmadığı anlamına gelmez.

Ek ölçümde 03/04/05 noktalarındaki dokuz askerin düşman rotasına en kısa mesafeleri **0,07–0,71 birim** çıktı; angajman menzili 0,8. Dolayısıyla bu noktalar için “askerler yolun dışında doğuyor” hatası yazmıyorum. Sonraki adım gerçek savaşta engelleme süresi ve hasar katkısını ölçmek olmalı. Can barlarının dar görüntüde birbirine ve kuleye binmesi ayrıca görsel düzenleme gerektiriyor.

### 3. Mermi görünürlüğü için çıkış noktası ve siluet üzerinde çalışılmalı

Güncel boyutlar canlı sahnede kullanılıyor. Görüntülerde mermiler temel küre biçiminde; kule modelinden çıkan saldırı hissi zayıf. Kod incelemesinde okçu ve büyücü atışları kule kök konumundan doğuyor. Bu, modeldeki silahın ağzını izleyen bir çıkış noktası değil.

**Öneri:** her kuleye model üzerinde atış noktası bağlamak; okçuya ok silueti/iz, büyücüye element rengi ve kısa iz eklemek. Havanın büyük küresini de diğer saldırılarla aynı kamera ölçeğinde değerlendirmek. Önce çıkış noktası ve kontrast; yalnız boyutu tekrar tekrar artırmayalım.

### 4. Savaş görsellerinin bir bölümü hâlâ geçici

Yerdeki düşmanlar küp; askerler basit oval/kapsül biçiminde, hava düşmanları basit mavi biçimlerle görünüyor. Harita ve menü görsellerinin ayrıntısı ile savaş birimleri arasında belirgin bir fark var. Farklı rollerin siluetle ayırt edilmesi zor.

**Öneri:** önce ilk bölümün kara/hava/boss ve asker modellerini tek bir bitmiş görsel setle tamamlamak; saldırı/isabet/ölüm geri bildirimlerini bu set üzerinde doğrulamak.

### 5. Yeni kulelerin katkısı ve XP farkı daha görünür olmalı

Hava savunması koşusunda boss hazırlığında öndeki okçu 20. seviye; büyücü 6, havan 2, diğer okçu ve birlik 1. seviyedeydi. Hasara bağlı XP dağıtımında bu mümkündür. Ancak oyuncu geç kurduğu kuleye neden az değer aldığını kolayca anlayamıyor.

**Öneri:** önce kule başına hasar, engelleme ve XP katkısı kaydı alalım. Sayısal denge değişikliğini bu ölçümden sonra yapalım; bu koşudaki zayıf yerleşimi sistem hatası gibi düzeltmeyelim.

### 6. Dil ve tamamlanma algısı

HUD'da “Start Game”, “WAVE” ile Türkçe metinler birlikte kullanılıyor. Sonuç ekranının görsel seviyesi savaş birimlerinden daha tamamlanmış duruyor. Beş bölüm de doğrudan seçilebiliyor; kampanya kilit açma/ilerleme beklentisi ayrıca netleştirilmeli.

**Öneri:** mevcut arayüzde önce tek dil tutarlılığı; kampanya ilerleme kuralını daha sonra ayrı kabul ölçütüyle ele almak.

## Bu turda doğrulanmayanlar

- Kampanyanın tamamının kazanılarak bitirilmesi; tabloda yenilgiyle biten bölümlerin bossları ve kullanılmamış sonraki bölüm geçişleri.
- Windows bağımsız build, farklı çözünürlükler, gerçek cihaz performansı ve ses kalitesinin dinleyerek değerlendirilmesi.
- Tam evrim/sinerji/rezervasyon/depo regresyonu; kule tipi değişiminin bütün kabul/iptal yolları. Önceki hedefli 44 ve 24 kontroller bu turun tam oynanışı yerine geçmez.
- Manuel fareyle bütün ekranların tıklama alanları ve sürükleme ergonomisi.

## Şimdi ne yapalım?

Öncelik ilk bölümün öğrenilebilirliği ve saldırı okunurluğu olsun: **atış noktaları/mermi görünümü → hava ve katalizör rehberi → üç sabit savunma planıyla tekrar oynama**. 4 canla kazanılan ilk bölüm koşusunu daha tutarlı bir referansa dönüştürüp kule katkısını ölçtükten sonra kalan yetenek ağaçlarına geçmek daha sağlıklı.

## Kanıtlar

- [İlk beş bölümün olay kaydı](events.json), [sonuçları](results.json).
- [Hava savunması tekrar kaydı](retry-air-defense/events.json).
- [Önde birlik tekrar kaydı](retry-front-blocker/events.json), [4 canla zafer ekranı](retry-front-blocker/chapter-1-victory.png).
- [Sonraki bölümlerde erken yatırım kaydı](retry-campaign-investment/events.json).
- [Bütçe karşılaştırmaları](economy-checks.json), [birlik/rota ölçümü](unit-route-observation.json).
- [Üçüncü dalga savaş görüntüsü](retry-air-defense/chapter-1-wave-3-combat.png).
- [İlk bölüm boss dalgası yenilgi ekranı](retry-air-defense/chapter-1-defeat.png).
'''
(root/'Oynanis_Raporu.md').write_text(report,encoding='utf-8')
print({'results':rows,'purchase_actions':purchases,'budget_checks':len(checks),'all_budgets_match':all(x['passed'] for x in checks),'report':str((root/'Oynanis_Raporu.md').resolve())})




