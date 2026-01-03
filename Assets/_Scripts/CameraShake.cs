using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Diðer scriptlerden kolayca ulaþmak için (Singleton benzeri yapý)
    public static CameraShake instance;

    // Kameranýn orijinal pozisyonunu saklayacaðýmýz yer
    private Vector3 originalPos;

    void Awake()
    {
        // Oyun açýlýnca "instance" ben olacaðým de.
        instance = this;
    }

    void Start()
    {
        // Kameranýn oyun baþýndaki durduðu yeri hafýzaya al.
        originalPos = transform.localPosition;
    }

    // --- ÝÞTE DIÞARIDAN ÇAÐIRACAÐIMIZ KOMUT BU ---
    public void Salla(float sure, float guc)
    {
        // Eðer hali hazýrda bir sallantý varsa durdur, yenisi baþlasýn.
        StopAllCoroutines();
        StartCoroutine(SallamaIslemi(sure, guc));
    }

    // Sallantýyý yapan arka plan iþlemi (Coroutine)
    private IEnumerator SallamaIslemi(float sure, float guc)
    {
        float gecenSure = 0.0f;

        while (gecenSure < sure)
        {
            // Rastgele bir saða/sola ve yukarý/aþaðý pozisyon belirle
            float x = Random.Range(-1f, 1f) * guc;
            float y = Random.Range(-1f, 1f) * guc;

            // Kamerayý o rastgele yere ýþýnla
            transform.localPosition = originalPos + new Vector3(x, y, 0);

            gecenSure += Time.deltaTime;

            // Bir sonraki kareyi bekle
            yield return null;
        }

        // Süre bitince kamerayý orijinal yerine geri koy
        transform.localPosition = originalPos;
    }
}