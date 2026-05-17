using System;
using System.Drawing;
using System.IO;
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
            _ingameUi = new IngameUI(provider, folder);
            _folder = folder;
        }

        public float? FocusedOpacity => 1.0f;

        public bool EnabledInCharacterSelection => true;

        public Image ToolbarImage
        {
            get
            {
                var iconPath = Path.Combine(_folder, "HappyBarsToolbarIcon.png");
                if (File.Exists(iconPath))
                {
                    using var image = Image.FromFile(iconPath);
                    return new Bitmap(image);
                }

                return (Image)Properties.Resources.NewToolbarIcon.Clone();
            }
        }

        public object UserInterfaceForm => _ingameUi;

        public Tuple<int, int> MinSize => new(400, 200);
    }
}
