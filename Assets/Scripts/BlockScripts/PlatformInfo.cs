namespace BlockScripts
{
    public class PlatformInfo : Coordinates
    {
        public readonly short colorCode;

        public PlatformInfo(int x, int y, short colorCode) : base(x, y)
        {
            this.colorCode = colorCode;
        }
    }
}