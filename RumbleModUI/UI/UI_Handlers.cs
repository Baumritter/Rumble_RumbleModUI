using MelonLoader;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RumbleModUI
{
    public static class TextureHandler
    {
        public static bool IsInit = false;

        public static bool debugTexture = false;

        private static Texture2D[] CustomAssets = new Texture2D[4];
        private static string[] CustomAssetsNames = new string[4];

        private static void InitCustomTextures()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            System.IO.Stream stream;


            CustomAssetsNames[0] = assembly.GetName().Name + ".Assets.UI_Arrow.png";
            CustomAssetsNames[1] = assembly.GetName().Name + ".Assets.UI_BaseLight.png";
            CustomAssetsNames[2] = assembly.GetName().Name + ".Assets.UI_BaseLight.png";
            CustomAssetsNames[3] = assembly.GetName().Name + ".Assets.UI_Mask.png";

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
        public static bool debugThemes = false;

        public static Theme ActiveTheme { get; set; }

        public static List<Theme> AvailableThemes = new List<Theme>();

        private static List<TextMeshProUGUI> Theme_Text = new List<TextMeshProUGUI>();
        private static List<Image> Theme_Foreground = new List<Image>();
        private static List<Image> Theme_Background = new List<Image>();


        public static void ChangeTheme(int Theme)
        {
            int tempval = Theme;
            if (Theme <= -1 || Theme >= AvailableThemes.Count) tempval = 0;

            Theme temp = AvailableThemes[tempval];

            ChangeTextColor(temp.Color_Text);
            ChangeFGColor(temp.Color_FG);
            ChangeBGColor(temp.Color_BG);
            ActiveTheme = temp;

            if (debugThemes) { MelonLogger.Msg("Theme - Theme changed"); }

        }
        public static void AddToFGTheme(GameObject Input)
        {
            Theme_Foreground.Add(Input.GetComponent<Image>());
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
            foreach (Image item in Theme_Foreground)
            {
                item.color = Color;
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

    public class Theme
    {
        public Theme() { }
        public Theme(string name, Color color_Text, Color color_FG, Color color_BG)
        {
            Name = name;
            Color_Text = color_Text;
            Color_FG = color_FG;
            Color_BG = color_BG;
        }

        public string Name { get; set; }
        public Color Color_Text { get; set; }
        public Color Color_FG { get; set; }
        public Color Color_BG { get; set; }

    }
}
