using Color = System.Drawing.Color;

[System.Serializable]
public class ColorPrefab
{
    public Color color;
    public string pathToPrefab;

    public ColorPrefab(Color color, string pathToPrefab)
    {
        this.color = color;
        this.pathToPrefab = pathToPrefab;
    }
}
