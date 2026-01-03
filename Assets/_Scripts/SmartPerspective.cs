using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode] // Editörde çalýþsýn
[RequireComponent(typeof(Image))] // Sadece Image'a eklenebilsin
public class SmartPerspective : BaseMeshEffect
{
    [Range(0, 0.5f)]
    [Tooltip("Üst kýsmýn ne kadar daralacaðýný belirler.")]
    public float daraltmaMiktari = 0.2f;

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive() || vh.currentVertCount == 0) return;

        List<UIVertex> verts = new List<UIVertex>();
        vh.GetUIVertexStream(verts);

        // 1. En üstteki Y pozisyonunu bul (Resmin tavaný)
        float maxY = verts[0].position.y;
        for (int i = 1; i < verts.Count; i++)
        {
            if (verts[i].position.y > maxY) maxY = verts[i].position.y;
        }

        // 2. Geniþliði kabaca hesapla
        float minX = verts[0].position.x;
        float maxX = verts[0].position.x;
        for (int i = 1; i < verts.Count; i++)
        {
            if (verts[i].position.x > maxX) maxX = verts[i].position.x;
            if (verts[i].position.x < minX) minX = verts[i].position.x;
        }
        float width = maxX - minX;
        float moveAmount = width * daraltmaMiktari;

        // 3. Sadece en üstte duran noktalarý bul ve merkeze çek
        for (int i = 0; i < verts.Count; i++)
        {
            UIVertex v = verts[i];

            // Bu nokta en üst hizada mý? (Küçük bir hata payý ile kontrol et)
            if (Mathf.Abs(v.position.y - maxY) < 0.01f)
            {
                // Eðer merkezden soldaysa -> Saða çek (+)
                if (v.position.x < 0)
                {
                    v.position.x += moveAmount;
                }
                // Eðer merkezden saðdaysa -> Sola çek (-)
                else if (v.position.x > 0)
                {
                    v.position.x -= moveAmount;
                }
            }
            verts[i] = v;
        }

        vh.Clear();
        vh.AddUIVertexTriangleStream(verts);
    }
}