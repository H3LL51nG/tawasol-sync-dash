namespace DashSync.Main
{
    public enum GameState
    {
        SETUP, PLAY, GAME_OVER
    }
    public enum ObstacleType
    {
        RING, HELIX, DOUBLE_HELIX
    }

    public enum GameEvents
    {
        SPAWN_RING, SPAWN_OBSTACLES, SPAWN_HELIX, SPAWN_DOUBLE_HELIX, SPAWN_COINS, WAIT
    }
}
