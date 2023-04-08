namespace BlockScripts
{
    public class PlatformInfo : Coordinates
    {
        public readonly short colorCode;
        public readonly short size;

        public PlatformInfo(int x, int y, short colorCode, short size) : base(x, y)
        {
            this.colorCode = colorCode;
            this.size = size;
        }
    }
}