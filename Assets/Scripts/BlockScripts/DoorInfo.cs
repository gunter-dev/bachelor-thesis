using System.Drawing;

namespace BlockScripts
{
    public class DoorInfo : Coordinates
    {
        public readonly short groupId;

        public Color lightColor;
        
        public DoorInfo(int x, int y, short groupId, Color lightColor) : base(x, y)
        {
            this.groupId = groupId;
            this.lightColor = lightColor;
        }
    }
}