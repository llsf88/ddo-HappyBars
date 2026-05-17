using System;
using VoK.Sdk;
using VoK.Sdk.Ddo;
using VoK.Sdk.Plugins;

namespace UiRuler
{
    public class Plugin : IDdoPlugin
    {
        public Guid PluginId => Guid.Parse("45c65b3f-c8a1-486d-b0a9-8e663db17b73");

        public GameId Game => GameId.DDO;

        public string PluginKey => "happy-bars-plugin-key-001";

        public string Name => "HappyBars";

        public string Description => "Hotbar layout, save/load, and snap tools for DDO";

        public string Author => "Slick";

        public Version Version => GetType().Assembly.GetName().Version;

        internal string Folder { get; set; }

        private PluginUI _ui;

        public static Plugin Instance { get; set; }

        public Plugin()
        {
            Instance = this;
        }

        public void Initialize(IDdoGameDataProvider gameDataProvider, string folder)
        {
            Folder = folder;
            _ui = new PluginUI(gameDataProvider, folder);
        }

        public IPluginUI GetPluginUI()
        {
            return _ui;
        }

        public void Terminate()
        {
        }
    }
}
