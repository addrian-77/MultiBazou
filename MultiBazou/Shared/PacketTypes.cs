namespace MultiBazou.Shared
{
    public enum PacketTypes
    {
        #region Lobby and Connection

        Empty = 0,
        Welcome = 1,
        Disconnect,
        ReadyState,
        UpdatePlayersInDictionary,
        UpdatePlayerInDictionary,
        StartGame,
        SpawnPlayer,
        ItemSpawn,
        ItemUpdate,
        ItemPickup,
        ItemDrop,

        #endregion

        #region Player Data

        PlayerInitialPos,
        PlayerPosition,
        PlayerRotation,
        PlayerSceneChange,

        #endregion
    }
}