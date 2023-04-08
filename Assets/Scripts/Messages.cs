public static class Messages
{
    public const string FileOpeningError = ": This file cannot be opened. Please try a different file!";

    public const string MissingExit =
        "There is no exit for this level! The only way this level can end is by your character dying.";

    public const string MultipleCharacters = "There cannot be multiple player characters!";

    public const string MultipleExits = "There cannot be multiple exits!";

    public const string NotEnoughSpaceForExit =
        "There is not enough space for the exit. You have to leave a 2x2 pixels large space for it.";

    public const string KeysWithoutKeyHoleWarning =
        "There are some keys, that don't have a key hole. Those have been ignored";

    public static string LayerError(string layerName)
    {
        return "Unknown layer name: '" + layerName + "'. This layer was ignored.";
    }

    public static string LayerCoordinatesWarning(string layerName, int x, int y)
    {
        return layerName + " - (" + x + ", " + y +
               "): There is an invalid color on these coordinates. This pixel has been ignored.";
    }

    public static string KeyHoleWithoutKeysWarning(int x, int y)
    {
        return "(" + x + ", " + y + "): A key hole on these coordinates has no keys assigned to it!";
    }
}