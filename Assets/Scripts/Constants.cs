public static class Constants
{
    // Tags
    public const string PlayerTag = "Player";
    public const string FanTag = "Fan";
    public const string GravityBlockTag = "Gravity";
    public const string AcceleratorTag = "Accelerator";
    public const string GroundTag = "Ground";
    public const string IceTag = "Ice";
    public const string SlimeTag = "Slime";
    public const string SpikeTag = "Spike";
    public const string SideTag = "Side";
    public const string BoxTag = "Box";
    public const string ButtonTag = "Button";
    public const string ExitTag = "Exit";
    
    // Level Layers
    public const string MainLayer = "main";
    public const string ElectricityLayer = "electricity";
    public const string MovingPlatformLayer = "movingPlatforms";
    public const string KeysLayer = "keys";
    
    // Camera
    public const float MapStartingCoordinate = -0.5f;
    public const float CameraSizeChange = 0.3f;
    public const float MaxRedTintOpacity = 0.7f;
    public const float AppearingTime = 2;
    public const float CreateLevelPanelScreenPercentage = 100 / 1080f;
    
    // Player
    public const float MoveForce = 7f;
    public const float JumpForce = 10f;
    public const float SlidingSpeed = 0.3f;
    public const float SlowedDownSpeed = 0.4f;
    public const float AcceleratorPlusSpeed = 0.6f;
    
    // Player animations
    public const string Idle = "Idle";
    public const string Run = "Run";
    public const string Jump = "Jump";
    
    // Button axis
    public const string Horizontal = "Horizontal";
    public const string Vertical = "Vertical";
    
    // Special blocks
    public const float AcceleratorSpeed = 1.6f;
    public const float PlatformSpeed = 5;
    public const float DistanceTolerance = 0.01f;
    
    // Scenes
    public const string Game = "Scenes/LobbyScene";
    public const string MainMenu = "Scenes/MainMenu";
    public const string CreateLevelScene = "Scenes/CreateLevelScene";
    public const string ErrorScene = "Scenes/ErrorScene";
    
    // Minimal resolution
    public const int MinimalWidth = 1280;
    public const int MinimalHeight = 720;
}