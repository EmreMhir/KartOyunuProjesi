using UnityEngine;

// Kart türlerimizi tanýmlayan bir kategori listesi. Bu satýr class'ýn dýþýnda olmalý.
public enum CardType { Attack, Defense, Heal }

[CreateAssetMenu(fileName = "New CardData", menuName = "Kart Oyunu/CardData")]
public class CardData : ScriptableObject
{
    public new string name;
    public string description;

    public Sprite artwork;

    public int damage; // Hasar, blok veya iyileþtirme miktarýný tutar.

    public CardType cardType; // Kartýmýzýn türünü tutacak YENÝ deðiþken.
    internal object manaCost;
    internal Sprite cardImage;
}