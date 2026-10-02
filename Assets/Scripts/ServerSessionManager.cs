using Unity.Services.Multiplayer;

public static class GameSessionManager
{
    // Holds the active session globally for the game client/host
    public static ISession CurrentSession { get; set; }
}