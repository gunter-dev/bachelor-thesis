namespace BlockScripts
{
    public class PlatformInfo
    {
        public readonly float x, y;

        public readonly short colorCode;
        public readonly short size;

        public PlatformInfo(float x, float y, short colorCode, short size)
        {
            this.x = x;
            this.y = y;
            this.colorCode = colorCode;
            this.size = size;
        }
    }
}