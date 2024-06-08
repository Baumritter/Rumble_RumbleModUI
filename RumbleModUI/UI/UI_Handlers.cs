using MelonLoader;
using System.Collections.Generic;
using System.Reflection;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RumbleModUI
{
    internal static class TextureHandler
    {
        private static bool IsInit = false;

        private static bool debugTexture = false;

        private static Texture2D[] CustomAssets = new Texture2D[4];
        private static string[] CustomAssetsNames = new string[4];

        private static void InitCustomTextures()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            System.IO.Stream stream;


            CustomAssetsNames[0] = "RumbleModUI.Assets.UI_Arrow.png";
            CustomAssetsNames[1] = "RumbleModUI.Assets.UI_BaseLight.png";
            CustomAssetsNames[2] = "RumbleModUI.Assets.UI_BaseLight.png";
            CustomAssetsNames[3] = "RumbleModUI.Assets.UI_Mask.png";

            for (int i = 0; i < CustomAssets.Length; i++)
            {
                stream = assembly.GetManifestResourceStream(CustomAssetsNames[i]);
                byte[] Bytes = new byte[stream.Length];
                stream.Read(Bytes, 0, Bytes.Length);

                CustomAssets[i] = new Texture2D(2, 2);
                ImageConversion.LoadImage(CustomAssets[i], Bytes);
                CustomAssets[i].hideFlags = HideFlags.HideAndDontSave;
                if (debugTexture) { MelonLogger.Msg("UI - Import:" + CustomAssetsNames[i]); }
            }
            IsInit = true;
        }
        public static Sprite ConvertToSprite(int Input, bool DoBorders = false, float border = 20)
        {
            if (!IsInit) InitCustomTextures();

            Texture2D Texture = CustomAssets[Input];

            Rect temp = new Rect
            {
                x = 0,
                y = 0,
                width = Texture.width,
                height = Texture.height
            };
            if (DoBorders)
            {
                Vector4 borders = new Vector4(border, border, border, border);
                return Sprite.Create(Texture, temp, new Vector2(0, 0), Texture.width, 0, SpriteMeshType.FullRect, borders);
            }
            else
            {
                return Sprite.Create(Texture, temp, new Vector2(0, 0));
            }
        }
    }

    public static class ThemeHandler
    {
        private static bool debugThemes = false;

        public static Theme ActiveTheme { get; set; }

        public static List<Theme> AvailableThemes = new List<Theme>();

        private static List<TextMeshProUGUI> Theme_Text = new List<TextMeshProUGUI>();
        private static List<Foreground_Item> Theme_Foreground = new List<Foreground_Item>();
        private static List<Image> Theme_Background = new List<Image>();

        public static void ChangeTheme(int Theme)
        {
            int tempval = Theme;
            if (Theme <= -1 || Theme >= AvailableThemes.Count) tempval = 0;

            Theme temp = AvailableThemes[tempval];

            ChangeTextColor(temp.Color_Text_Base);
            ChangeFGColor(temp.Color_FG);
            ChangeBGColor(temp.Color_BG);
            ActiveTheme = temp;

            if (debugThemes) { MelonLogger.Msg("Theme - Theme changed"); }

        }
        public static void AddToFGTheme(GameObject Input, bool AlphaOverride)
        {
            Theme_Foreground.Add(new Foreground_Item { Image = Input.GetComponent<Image>(), FullAlphaOverride = AlphaOverride });
        }
        public static void AddToBGTheme(GameObject Input)
        {
            Theme_Background.Add(Input.GetComponent<Image>());
        }
        public static void AddToTextTheme(GameObject Input)
        {
            Theme_Text.Add(Input.GetComponent<TextMeshProUGUI>());
        }

        private static void ChangeTextColor(Color Color)
        {
            foreach (TextMeshProUGUI item in Theme_Text)
            {
                item.color = Color;
            }

            if (debugThemes) { MelonLogger.Msg("Theme - Text Color changed"); }
        }
        private static void ChangeFGColor(Color Color)
        {
            foreach (Foreground_Item item in Theme_Foreground)
            {
                item.Image.color = Color;
                if (item.FullAlphaOverride) item.Image.color = new Color(item.Image.color.r,item.Image.color.g,item.Image.color.b,1.0f);
            }

            if (debugThemes) { MelonLogger.Msg("Theme - Foreground Color changed"); }
        }
        private static void ChangeBGColor(Color Color)
        {
            foreach (Image item in Theme_Background)
            {
                item.color = Color;
            }
            if (debugThemes) { MelonLogger.Msg("Theme - Background Color changed"); }
        }

    }
    internal class Foreground_Item
    {
        public Image Image;
        public bool FullAlphaOverride;
    }
    public class Theme
    {
        public Theme() { }
        public Theme(string name, Color Text_Base, Color Text_Valid ,Color Text_Error, Color FG, Color BG)
        {
            Name = name;
            Color_Text_Base = Text_Base;
            Color_Text_Error = Text_Error;
            Color_Text_Valid = Text_Valid;
            Color_FG = FG;
            Color_BG = BG;
        }

        public string Name { get; set; }
        public Color Color_Text_Base { get; set; }
        public Color Color_Text_Error { get; set; }
        public Color Color_Text_Valid { get; set; }
        public Color Color_FG { get; set; }
        public Color Color_BG { get; set; }

    }
}
