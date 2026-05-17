using System;
using System.Drawing;
using VoK.Sdk.Ddo;
using VoK.Sdk.Plugins;

namespace UiRuler
{
    public class PluginUI : IPluginUI
    {
        private IDdoGameDataProvider _provider;
        private IngameUI _ingameUi;
        private readonly string _folder;


        public PluginUI(IDdoGameDataProvider provider, string folder)
        {
            _provider = provider;
            _folder = folder;
        }

        public float? FocusedOpacity => 1.0f;

        public bool EnabledInCharacterSelection => true;

        public Image ToolbarImage
        {
            get
            {
                return (Image)Properties.Resources.NewToolbarIcon.Clone();
            }
        }

        public object UserInterfaceForm => _ingameUi ??= new IngameUI(_provider, _folder);

        public Tuple<int, int> MinSize => new(400, 200);
    }
}
