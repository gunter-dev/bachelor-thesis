using Color = System.Drawing.Color;

// A class used by the level generator. It contains mapping of blocks to their assigned color.
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
