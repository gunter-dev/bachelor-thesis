using System.Drawing;

namespace BlockScripts
{
    public class KeyHoleInfo : Coordinates
    {
        public readonly short groupId;

        public Color lightColor;
        
        public KeyHoleInfo(int x, int y, short groupId, Color lightColor) : base(x, y)
        {
            this.groupId = groupId;
            this.lightColor = lightColor;
        }
    }
}