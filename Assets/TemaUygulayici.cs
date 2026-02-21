using UnityEngine;

public class TemaUygulayici : MonoBehaviour
{
    void Awake()
    {
        Uygula();
    }

    public void Uygula()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        Color arkaPlanRengi = Color.white;
        Color objeRengi = Color.white;

        switch (OyunAyarlari.SeciliTema)
        {
            case OyunAyarlari.Tema.Pastel:
                ColorUtility.TryParseHtmlString("#D9B3B3", out arkaPlanRengi); // Uçuk Pembe
                ColorUtility.TryParseHtmlString("#6B2727", out objeRengi);     // Bordo
                break;
            case OyunAyarlari.Tema.Karanlik:
                ColorUtility.TryParseHtmlString("#121212", out arkaPlanRengi); // Koyu Siyah
                ColorUtility.TryParseHtmlString("#00E5FF", out objeRengi);     // Neon Mavi
                break;
            case OyunAyarlari.Tema.Klasik:
                arkaPlanRengi = Color.black;
                objeRengi = Color.white;
                break;
        }

        cam.backgroundColor = arkaPlanRengi;

        // Sahnede rengi olan HER ŞEYİ bul (Dekoratif toplar, raketler, oyun topu)
        SpriteRenderer[] tumSpriteler = FindObjectsByType<SpriteRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        foreach (SpriteRenderer sr in tumSpriteler)
        {
            // Güçlendirme kutularının kendi renkleri var, onları boyama
            if (sr.GetComponent<Guclendirme>() != null) continue;
            
            sr.color = objeRengi;
        }
    }
}