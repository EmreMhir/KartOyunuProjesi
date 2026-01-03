using UnityEngine;

[CreateAssetMenu(fileName = "New Monster", menuName = "Monster")]
public class MonsterData : ScriptableObject
{
    public new string name; // Canavarýn Adý
    public Sprite artwork;  // Canavarýn Resmi

    public int maxHealth;   // Caný

    // EKSÝK OLAN KISIMLAR BUNLARDI:
    public int minDamage;   // En az kaç vursun?
    public int maxDamage;   // En çok kaç vursun?
}