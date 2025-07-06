using System;
using System.Linq;
using System.Security.Policy;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using MultiBazou.ClientSide;
using MultiBazou.ClientSide.Data;
using MultiBazou.ServerSide;
using MultiBazou.ServerSide.Data;
using MultiBazou.ServerSide.Handle;
using MultiBazou.Shared;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MultiBazou
{
    [BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        private ConfigEntry<string> ConfigModUiToggle;
        public const int MaxPlayer = 2;
        public const int Port = 7777;

        public Client client;
        public ModUI modUI;
        public ContentManager contentManager;
        public GameObject multiplayerGameObject;

        public bool isModInitialized;

        public bool GuiInitialized = false;
        public static ManualLogSource log;

        void Awake()
        {
            log = Logger;
            ConfigModUiToggle = Config.Bind(new ConfigDefinition("General", "modUiKey"), "End",
                new ConfigDescription("This buttons opens the menu for the multiplayer mod."));
        }

        void Start()
        {
            multiplayerGameObject = new GameObject(PluginInfo.Name);
            DontDestroyOnLoad(multiplayerGameObject);
            SceneManager.activeSceneChanged += OnSceneWasLoaded;
            client = multiplayerGameObject.AddComponent<Client>();
            client.Awake();

            modUI = multiplayerGameObject.AddComponent<ModUI>();
            modUI.Awake();

            contentManager = multiplayerGameObject.AddComponent<ContentManager>();

            PreferencesManager.LoadPreferences();
            isModInitialized = true;
            contentManager.Initialize();
            log.LogInfo(PluginInfo.Name + " has been initialized, your game version is " + contentManager.GameVersion);
        }

        private void OnSceneWasLoaded(Scene previousScene, Scene newScene)
        {
            log.LogInfo("scene loaded");
            // OnGUI();
            if (newScene.name == SceneNames.MainMenu) contentManager.Initialize();
            if (!isModInitialized) return;
            
            if (client.isConnected || ServerData.isRunning)
            {
                GameData.dataInitialized = false;
                ModSceneManager.UpdatePlayerScene();
                if (ModSceneManager.IsInMenu() && client.isConnected && !ServerData.isRunning)
                {
                    Client.instance.Disconnect();
                    Application.runInBackground = false;
                }

                if (ModSceneManager.IsInMenu() && ServerData.isRunning)
                {
                    Server.Stop();
                    Application.runInBackground = false;
                }

                if (newScene.name == SceneNames.Master)
                {
                    StartCoroutine(ClientData.instance.Initialize());

                    foreach (var player in
                             ClientData.instance.Players.Where(
                                 player =>
                                     ModSceneManager.IsInGame(player.Value) &&
                                     !ClientData.instance.Players.ContainsKey(player.Value.id)
                             )
                            )
                    {
                        ClientData.instance.SetupPlayerGameObject(player.Value);
                    }
                }
            }
        }

        void Update()
        {
            if (!isModInitialized) return;

            if (GameData.dataInitialized)
            {
                if (Client.instance.isConnected)
                {
                    if (ModSceneManager.IsInGame())
                        ClientData.instance.UpdateClient();
                }
            }

            if (ServerData.isRunning)
            {
                if (Server.Clients.Count == 0)
                {
                    Server.Stop();
                }
            }

            ThreadManager.UpdateThread();
        }

        public void OnApplicationQuit()
        {
            if (ServerData.isRunning)
            {
                foreach (var id in Server.Clients.Keys)
                {
                    ServerSend.DisconnectClient(id, "Server is shutting down.");
                }
            }
        }

        private void OnGUI()
        {
            if (!isModInitialized && !Client.instance.isConnected) return;
            if (!ModSceneManager.IsInMenu())
            {
                log.LogInfo("not in menu");
                return;
            }
            if (!GuiInitialized)
            {
                if (Event.current.Equals(Event.KeyboardEvent(ConfigModUiToggle.Value)))
                    ModUI.Instance.showModUI = !ModUI.Instance.showModUI;
                log.LogInfo("attempted to show UI " + ModUI.Instance.showModUI + " bool value");
                ModUI.Instance.wasinitialized = false;
                modUI.OnGUI();
                GuiInitialized = true;
            }
        }
    }
}