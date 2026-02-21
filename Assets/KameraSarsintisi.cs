using UnityEngine;
using System.Collections;

public class KameraSarsıntısı : MonoBehaviour
{
    public static KameraSarsıntısı Instance; // Her yerden kolayca ulaşmak için Singleton
    private Vector3 originalPos;

    void Awake()
    {
        if (Instance == null) Instance = this;
        originalPos = transform.localPosition;
    }

    public void ShakeCamera(float duration, float magnitude)
    {
        StopAllCoroutines(); // Üst üste binen titremeleri sıfırla
        StartCoroutine(Shake(duration, magnitude));
    }

    IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);
            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos; // Titreme bitince kamerayı merkeze al
    }
}