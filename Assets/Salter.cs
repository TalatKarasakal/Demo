using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Toggle))]
public class Salter : MonoBehaviour
{
    [Header("Görsel Sürükleme Alanları")]
    public Image arkaPlan; // Arka plan resmi (ArkaPlan objesi buraya gelecek)
    public RectTransform kulp; // Yuvarlak kulp (Kulp objesi buraya gelecek)

    [Header("Renkler")]
    public Color acikRenk = new Color(0.2f, 0.8f, 0.2f); // Yeşil
    public Color kapaliRenk = new Color(0.8f, 0.8f, 0.8f); // Gri

    private Toggle toggle;
    private Vector2 acikPozisyon;
    private Vector2 kapaliPozisyon;

    void Awake()
    {
        toggle = GetComponent<Toggle>();

        if (kulp == null || arkaPlan == null) 
        {
            Debug.LogWarning("Salter: Kulp veya ArkaPlan atanmamış!");
            return;
        }

        // Arka planın genişliğine göre kulpun gideceği yerleri hesapla
        RectTransform arkaPlanRect = arkaPlan.GetComponent<RectTransform>();
        float hareketMesafesi = (arkaPlanRect.rect.width / 2) - (kulp.rect.width / 2) - 2f;
        
        kapaliPozisyon = new Vector2(-hareketMesafesi, 0);
        acikPozisyon = new Vector2(hareketMesafesi, 0);

        toggle.onValueChanged.AddListener(HareketeGec);

        // Oyun başlarken anında doğru pozisyona al
        if (toggle.isOn)
        {
            kulp.anchoredPosition = acikPozisyon;
            arkaPlan.color = acikRenk;
        }
        else
        {
            kulp.anchoredPosition = kapaliPozisyon;
            arkaPlan.color = kapaliRenk;
        }
    }

    void HareketeGec(bool acikMi)
    {
        StopAllCoroutines();
        StartCoroutine(AnimasyonYap(acikMi));
    }

    IEnumerator AnimasyonYap(bool acikMi)
    {
        float t = 0;
        Vector2 baslangicPoz = kulp.anchoredPosition;
        Vector2 hedefPoz = acikMi ? acikPozisyon : kapaliPozisyon;
        
        Color baslangicRenk = arkaPlan.color;
        Color hedefRenk = acikMi ? acikRenk : kapaliRenk;

        while (t < 1f)
        {
            t += Time.deltaTime * 15f; // Animasyon hızı
            kulp.anchoredPosition = Vector2.Lerp(baslangicPoz, hedefPoz, t);
            arkaPlan.color = Color.Lerp(baslangicRenk, hedefRenk, t);
            yield return null;
        }
    }
}