namespace BlockScripts
{
    public class ElectricityInfo
    {
        public readonly float x, y;

        public readonly short colorCode;

        public ElectricityInfo(float x, float y, short colorCode)
        {
            this.x = x;
            this.y = y;
            this.colorCode = colorCode;
        }
    }
}