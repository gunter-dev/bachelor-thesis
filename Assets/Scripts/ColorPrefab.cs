using UnityEngine;
using Color = System.Drawing.Color;

[System.Serializable]
public class ColorPrefab
{
    public Color color;
    public GameObject prefab;

    public ColorPrefab(Color color, GameObject prefab)
    {
        this.color = color;
        this.prefab = prefab;
    }
}
