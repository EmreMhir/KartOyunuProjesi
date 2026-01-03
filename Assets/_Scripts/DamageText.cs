using UnityEngine;
using TMPro; // TextMeshPro için þart

public class DamageText : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public float moveSpeed = 100f; // Yukarý çýkma hýzý
    public float lifeTime = 1f;    // Ekranda kalma süresi

    public void Setup(int damageAmount)
    {
        // Yazýyý ayarla (Örn: "-15")
        textComponent.text = "-" + damageAmount.ToString();

        // Rengi kýrmýzý yap (Ýstersen editörden de yapabilirsin)
        textComponent.color = Color.red;

        // Belirlenen süre sonra yok et
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Her karede yukarý doðru hareket et
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
    }
}