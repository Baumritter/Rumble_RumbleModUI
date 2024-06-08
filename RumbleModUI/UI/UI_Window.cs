using Il2CppTMPro;
using MelonLoader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static RumbleModUI.Baum_API.RectTransformExtension;

namespace RumbleModUI
{
    /// <summary>
    /// Class for modular UI Window creation
    /// </summary>
    public class Window
    {
        /// <summary>
        /// This is a constructor
        /// </summary>
        public Window(string Name, bool debug = false) { this.Name = Name; this.DebugWindow = debug; }

        private Vector3 Scl_1x1 = new Vector3(1f, 1f, 0);
        private bool DebugWindow { get; set; }
        private bool HasTitle { get; set; }
        private GameObject Obj_Title { get; set; }
        private object TitleDragger { get; set; }

        /// <summary>
        /// Name of the Window
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Should be set to the GameObject that has all the Child Elements attached
        /// </summary>
        public GameObject ParentObject { get; set; }

        /// <summary>
        /// Contains all created child elements of this instance
        /// </summary>
        public List<GameObject> Elements = new List<GameObject>();

        /// <summary>
        /// Gets invoked when the window is shown
        /// </summary>
        public virtual event System.Action OnShow;

        /// <summary>
        /// Gets invoked when the window is hidden
        /// </summary>
        public virtual event System.Action OnHide;

        #region UI_Element_Creation
        /// <summary>
        /// Creates a Title Object. Text Content has to be set manually using <see cref="SetTitleText(string, float)"/>
        /// </summary>
        public GameObject CreateTitle(string Name, Transform Parent , Vector3 Position, Vector2 Size)
        {
            if (HasTitle) return null;

            #region Objects
            GameObject T_Obj = new GameObject { name = Name };
            GameObject T_Text = new GameObject { name = "Text" };

            if (DebugWindow) { MelonLogger.Msg("Title - Objects set"); }
            #endregion

            #region Set Parents
            T_Obj.transform.SetParent(Parent);
            T_Text.transform.SetParent(T_Obj.transform);

            if (DebugWindow) { MelonLogger.Msg("Title - Parents set"); }
            #endregion

            #region Add Components
            T_Obj.AddComponent<Image>();
            T_Obj.AddComponent<Button>();
            T_Text.AddComponent<TextMeshProUGUI>();

            if (DebugWindow) { MelonLogger.Msg("Title - Components set"); }
            #endregion

            #region Set Text
            SetTextProperties(T_Text, "Title", 20);
            T_Text.GetComponent<TextMeshProUGUI>().horizontalAlignment = HorizontalAlignmentOptions.Center;
            T_Text.GetComponent<TextMeshProUGUI>().verticalAlignment = VerticalAlignmentOptions.Middle;

            if (DebugWindow) { MelonLogger.Msg("Title - Text set"); }
            #endregion

            #region Set Image
            T_Obj.GetComponent<Image>().sprite = TextureHandler.ConvertToSprite(1, true);
            T_Obj.GetComponent<Image>().type = Image.Type.Tiled;
            ThemeHandler.AddToFGTheme(T_Obj,false);

            if (DebugWindow) { MelonLogger.Msg("Title - Image set"); }
            #endregion

            #region Set RectTransform
            T_Obj.GetComponent<RectTransform>().sizeDelta = Size;
            T_Text.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

            if (DebugWindow) { MelonLogger.Msg("Title - Image Transforms set"); }
            #endregion

            #region Positioning and Scaling
            SetLocalPosAndScale(T_Obj, Position, Scl_1x1);
            SetLocalPosAndScale(T_Text, Vector3.zero, Scl_1x1);

            if (DebugWindow) { MelonLogger.Msg("Title - Position set"); }
            #endregion

            #region Alignment
            SetAnchors(T_Obj, AnchorPresets.TopLeft);
            SetPivot(T_Obj, PivotPresets.TopLeft);

            SetAnchors(T_Text, AnchorPresets.StretchAll);
            SetPivot(T_Text, PivotPresets.TopLeft);

            if (DebugWindow) { MelonLogger.Msg("Title - Anchors/Pivots set"); }
            #endregion

            HasTitle = true;

            Obj_Title = T_Obj;
            Elements.Add(T_Obj);

            return T_Obj;
        }

        /// <summary>
        /// Creates a Background Object.
        /// </summary>
        public GameObject CreateBackgroundBox(string Name, Transform Parent, Vector3 Position)
        {
            GameObject temp = new GameObject
            {
                name = Name
            };
            temp.transform.SetParent(Parent);

            SetLocalPosAndScale(temp, Position, Scl_1x1);

            temp.AddComponent<Image>();

            temp.GetComponent<Image>().sprite = TextureHandler.ConvertToSprite(1, true);
            temp.GetComponent<Image>().type = Image.Type.Tiled;
            temp.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            ThemeHandler.AddToFGTheme(temp, false);

            SetAnchors(temp, AnchorPresets.StretchAll);
            SetPivot(temp, PivotPresets.TopLeft);

            Elements.Add(temp);

            return temp;
        }

        /// <summary>
        /// Creates a Dropdown Object.
        /// </summary>
        public GameObject CreateDropdown(string Name, Transform Parent, Vector3 Position, Vector2 DD_Size, Vector2 Ext_Size)
        {
            #region Objects
            GameObject DD_Obj = new GameObject { name = Name };
            //Child Layer 1
            GameObject DD_Label = new GameObject { name = "Label" };
            GameObject DD_Arrow = new GameObject { name = "Arrow" };
            GameObject DD_Template = new GameObject { name = "Template" };
            //Child Layer 1 - 1
            GameObject DD_Viewport = new GameObject { name = "Viewport" };
            GameObject DD_Content = new GameObject { name = "Content" };
            GameObject DD_Item = new GameObject { name = "Item" };
            GameObject DD_ItemLabel = new GameObject { name = "Item Label" };
            GameObject DD_ItemBG = new GameObject { name = "Item Background" };
            //Child Layer 1 - 2
            GameObject DD_Scrollbar = new GameObject { name = "Scrollbar" };
            GameObject DD_Slide = new GameObject { name = "Slide" };
            GameObject DD_Handle = new GameObject { name = "Handle" };

            if (DebugWindow) { MelonLogger.Msg("DropDown - Objects initialised"); }
            #endregion

            #region Set Transforms
            DD_Obj.transform.SetParent(Parent);
            DD_Label.transform.SetParent(DD_Obj.transform);
            DD_Arrow.transform.SetParent(DD_Obj.transform);
            DD_Template.transform.SetParent(DD_Obj.transform);
            DD_Viewport.transform.SetParent(DD_Template.transform);
            DD_Content.transform.SetParent(DD_Viewport.transform);
            DD_Item.transform.SetParent(DD_Content.transform);
            DD_ItemBG.transform.SetParent(DD_Item.transform);
            DD_ItemLabel.transform.SetParent(DD_Item.transform);
            DD_Scrollbar.transform.SetParent(DD_Template.transform);
            DD_Slide.transform.SetParent(DD_Scrollbar.transform);
            DD_Handle.transform.SetParent(DD_Slide.transform);

            if (DebugWindow) { MelonLogger.Msg("DropDown - Parents set"); }
            #endregion

            #region Add Components
            DD_Obj.AddComponent<Image>();
            DD_Obj.AddComponent<TMP_Dropdown>();

            DD_Label.AddComponent<TextMeshProUGUI>();

            DD_Arrow.AddComponent<Image>();

            DD_Template.AddComponent<Image>();
            DD_Template.AddComponent<ScrollRect>();

            DD_Viewport.AddComponent<Image>();
            DD_Viewport.AddComponent<Mask>();

            DD_Content.AddComponent<RectTransform>();

            DD_Item.AddComponent<Toggle>();
            DD_Item.AddComponent<RectTransform>();

            DD_ItemLabel.AddComponent<TextMeshProUGUI>();

            DD_ItemBG.AddComponent<Image>();

            DD_Scrollbar.AddComponent<Scrollbar>();
            DD_Scrollbar.AddComponent<Image>();

            DD_Slide.AddComponent<RectTransform>();

            DD_Handle.AddComponent<Image>();

            if (DebugWindow) { MelonLogger.Msg("DropDown - Components set"); }
            #endregion

            #region Set Text
            SetTextProperties(DD_Label, "", 20);
            SetTextProperties(DD_ItemLabel, "", 20);
            DD_Label.GetComponent<TextMeshProUGUI>().fontSizeMax = 20f;
            DD_Label.GetComponent<TextMeshProUGUI>().fontSizeMin = 8f;
            DD_Label.GetComponent<TextMeshProUGUI>().enableAutoSizing = true;
            DD_ItemLabel.GetComponent<TextMeshProUGUI>().fontSizeMax = 20f;
            DD_ItemLabel.GetComponent<TextMeshProUGUI>().fontSizeMin = 8f;
            DD_ItemLabel.GetComponent<TextMeshProUGUI>().enableAutoSizing = true;

            if (DebugWindow) { MelonLogger.Msg("DropDown - Text set"); }
            #endregion

            #region Set Images
            DD_Arrow.GetComponent<Image>().sprite = TextureHandler.ConvertToSprite(0, false);
            ThemeHandler.AddToBGTheme(DD_Arrow);

            DD_Obj.GetComponent<Image>().sprite = TextureHandler.ConvertToSprite(1, true);
            DD_Obj.GetComponent<Image>().type = Image.Type.Tiled;
            ThemeHandler.AddToFGTheme(DD_Obj, false);
            DD_Handle.GetComponent<Image>().sprite = TextureHandler.ConvertToSprite(1, true);
            DD_Handle.GetComponent<Image>().type = Image.Type.Tiled;
            ThemeHandler.AddToFGTheme(DD_Handle, false);
            DD_Template.GetComponent<Image>().sprite = TextureHandler.ConvertToSprite(1, true);
            DD_Template.GetComponent<Image>().type = Image.Type.Tiled;
            ThemeHandler.AddToFGTheme(DD_Template, false);
            DD_Scrollbar.GetComponent<Image>().sprite = TextureHandler.ConvertToSprite(2, true);
            DD_Scrollbar.GetComponent<Image>().type = Image.Type.Tiled;
            ThemeHandler.AddToBGTheme(DD_Scrollbar);
            DD_Viewport.GetComponent<Image>().sprite = TextureHandler.ConvertToSprite(3, true, 12);
            DD_Viewport.GetComponent<Image>().type = Image.Type.Tiled;
            ThemeHandler.AddToFGTheme(DD_Viewport, false);

            ThemeHandler.AddToFGTheme(DD_ItemBG, false);

            if (DebugWindow) { MelonLogger.Msg("DropDown - Images set"); }
            #endregion

            #region Set RectTransforms
            DD_Obj.GetComponent<RectTransform>().sizeDelta = DD_Size;
            DD_Label.GetComponent<RectTransform>().sizeDelta = new Vector2(-50, 0);
            DD_Arrow.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);         //Arrow Size
            DD_Template.GetComponent<RectTransform>().sizeDelta = Ext_Size;
            DD_Content.GetComponent<RectTransform>().sizeDelta = new Vector2(-4, 44);        //Height of Content
            DD_Viewport.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            DD_Item.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 38);           //Height of Item
            DD_ItemLabel.GetComponent<RectTransform>().sizeDelta = new Vector2(-20, 0);
            DD_ItemBG.GetComponent<RectTransform>().sizeDelta = new Vector2(-1, -1);
            DD_Scrollbar.GetComponent<RectTransform>().sizeDelta = new Vector2(30, 0);      //Width of Scrollbar
            DD_Slide.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            DD_Handle.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);

            if (DebugWindow) { MelonLogger.Msg("DropDown - Image Transforms set"); }
            #endregion

            #region Link Objects
            DD_Obj.GetComponent<TMP_Dropdown>().image = DD_Obj.GetComponent<Image>();
            DD_Obj.GetComponent<TMP_Dropdown>().template = DD_Template.GetComponent<RectTransform>();
            DD_Obj.GetComponent<TMP_Dropdown>().captionText = DD_Label.GetComponent<TextMeshProUGUI>();
            DD_Obj.GetComponent<TMP_Dropdown>().itemText = DD_ItemLabel.GetComponent<TextMeshProUGUI>();

            DD_Template.GetComponent<ScrollRect>().content = DD_Content.GetComponent<RectTransform>();
            DD_Template.GetComponent<ScrollRect>().viewport = DD_Viewport.GetComponent<RectTransform>();
            DD_Template.GetComponent<ScrollRect>().verticalScrollbar = DD_Scrollbar.GetComponent<Scrollbar>();

            DD_Item.GetComponent<Toggle>().targetGraphic = DD_ItemBG.GetComponent<Image>();

            DD_Scrollbar.GetComponent<Scrollbar>().targetGraphic = DD_Handle.GetComponent<Image>();
            DD_Scrollbar.GetComponent<Scrollbar>().handleRect = DD_Handle.GetComponent<RectTransform>();

            if (DebugWindow) { MelonLogger.Msg("DropDown - Objects linked"); }
            #endregion

            #region Change Settings
            DD_Template.GetComponent<ScrollRect>().verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            DD_Template.GetComponent<ScrollRect>().movementType = ScrollRect.MovementType.Clamped;
            DD_Template.GetComponent<ScrollRect>().scrollSensitivity = 30;
            DD_Scrollbar.GetComponent<Scrollbar>().SetDirection(Scrollbar.Direction.BottomToTop, false);
            #endregion

            #region Positioning and Scaling
            SetLocalPosAndScale(DD_Obj, Position, Scl_1x1);
            SetLocalPosAndScale(DD_Label, Vector3.zero, Scl_1x1);
            SetLocalPosAndScale(DD_Arrow, Vector3.zero, Scl_1x1);
            SetLocalPosAndScale(DD_Template, Vector3.zero, Scl_1x1);
            SetLocalPosAndScale(DD_Viewport, Vector3.zero, Scl_1x1);
            SetLocalPosAndScale(DD_Content, Vector3.zero, Scl_1x1);
            SetLocalPosAndScale(DD_Item, Vector3.zero, Scl_1x1);
            SetLocalPosAndScale(DD_ItemLabel, Vector3.zero, Scl_1x1);
            SetLocalPosAndScale(DD_ItemBG, Vector3.zero, Scl_1x1);
            SetLocalPosAndScale(DD_Scrollbar, Vector3.zero, Scl_1x1);
            SetLocalPosAndScale(DD_Slide, Vector3.zero, Scl_1x1);
            SetLocalPosAndScale(DD_Handle, Vector3.zero, Scl_1x1);

            if (DebugWindow) { MelonLogger.Msg("DropDown - Position set"); }
            #endregion

            #region Alignment
            SetAnchors(DD_Obj, AnchorPresets.TopLeft);
            SetPivot(DD_Obj, PivotPresets.TopLeft);

            SetAnchors(DD_Label, AnchorPresets.StretchAll);
            SetPivot(DD_Label, PivotPresets.MiddleCenter);
            SetAnchorPos(DD_Label, -15, 0);

            SetAnchors(DD_Arrow, AnchorPresets.MiddleRight);
            SetPivot(DD_Arrow, PivotPresets.MiddleCenter);
            SetAnchorPos(DD_Arrow, -20, -1);

            SetAnchors(DD_Template, AnchorPresets.HorStretchBottom);
            SetPivot(DD_Template, PivotPresets.TopCenter);

            SetAnchors(DD_Viewport, AnchorPresets.StretchAll);
            SetPivot(DD_Viewport, PivotPresets.TopLeft);
            SetAnchorPos(DD_Viewport, -1, -1);

            SetAnchors(DD_Content, AnchorPresets.HorStretchTop);
            SetPivot(DD_Content, PivotPresets.TopCenter);

            SetAnchors(DD_Item, AnchorPresets.HorStretchMiddle);
            SetPivot(DD_Item, PivotPresets.MiddleCenter);

            SetAnchors(DD_ItemLabel, AnchorPresets.StretchAll);
            SetPivot(DD_ItemLabel, PivotPresets.MiddleCenter);
            SetAnchorPos(DD_ItemLabel, 0, 1);

            SetAnchors(DD_ItemBG, AnchorPresets.StretchAll);
            SetPivot(DD_ItemBG, PivotPresets.MiddleCenter);
            SetAnchorPos(DD_ItemBG, 1, 1);

            SetAnchors(DD_Scrollbar, AnchorPresets.VertStretchRight);
            SetPivot(DD_Scrollbar, PivotPresets.TopRight);

            SetAnchors(DD_Slide, AnchorPresets.StretchAll);
            SetPivot(DD_Slide, PivotPresets.MiddleCenter);

            SetAnchors(DD_Handle, AnchorPresets.BottomLeft);
            SetPivot(DD_Handle, PivotPresets.MiddleCenter);

            if (DebugWindow) { MelonLogger.Msg("DropDown - Anchors/Pivots set"); }
            #endregion

            Elements.Add(DD_Obj);

            return DD_Obj;
        }

        /// <summary>
        /// Creates a InputField Object.
        /// </summary>
        public GameObject CreateInputField(string Name, Transform Parent, Vector3 Position, Vector2 Size)
        {
            #region Objects
            GameObject IF_Obj = new GameObject { name = Name };
            GameObject IF_TextArea = new GameObject { name = "TextArea" };
            GameObject IF_Placeholder = new GameObject { name = "Placeholder" };
            GameObject IF_Text = new GameObject { name = "Text" };

            if (DebugWindow) { MelonLogger.Msg("InputField - Objects set"); }
            #endregion

            #region Set Transforms
            IF_Obj.transform.SetParent(Parent);
            IF_TextArea.transform.SetParent(IF_Obj.transform);
            IF_Placeholder.transform.SetParent(IF_TextArea.transform);
            IF_Text.transform.SetParent(IF_TextArea.transform);

            if (DebugWindow) { MelonLogger.Msg("InputField - Parents set"); }
            #endregion

            #region Add Components
            IF_Obj.AddComponent<Image>();
            IF_Obj.AddComponent<TMP_InputField>();

            IF_TextArea.AddComponent<RectTransform>();
            IF_TextArea.AddComponent<RectMask2D>();

            IF_Placeholder.AddComponent<LayoutElement>();
            IF_Placeholder.AddComponent<TextMeshProUGUI>();

            IF_Text.AddComponent<TextMeshProUGUI>();

            if (DebugWindow) { MelonLogger.Msg("InputField - Components set"); }
            #endregion

            #region Set Text
            SetTextProperties(IF_Placeholder, "Enter text...", 20, true);
            SetTextProperties(IF_Text, "", 20);

            if (DebugWindow) { MelonLogger.Msg("InputField - Text set"); }
            #endregion

            #region Set Placeholder Settings
            IF_Placeholder.GetComponent<LayoutElement>().ignoreLayout = true;
            IF_Placeholder.GetComponent<LayoutElement>().layoutPriority = 1;
            if (DebugWindow) { MelonLogger.Msg("InputField - Placeholder set"); }
            #endregion

            #region Set TextArea Settings
            IF_TextArea.GetComponent<RectMask2D>().padding = new Vector4(-8, -8, -5, -5);
            IF_TextArea.GetComponent<RectMask2D>().softness = new Vector2Int(0, 0);
            if (DebugWindow) { MelonLogger.Msg("InputField - TextArea set"); }
            #endregion

            #region Set Input Field Settings
            IF_Obj.GetComponent<TMP_InputField>().onFocusSelectAll = true;
            IF_Obj.GetComponent<TMP_InputField>().resetOnDeActivation = true;
            IF_Obj.GetComponent<TMP_InputField>().restoreOriginalTextOnEscape = true;
            IF_Obj.GetComponent<TMP_InputField>().richText = true;
            #endregion

            #region Set Images
            IF_Obj.GetComponent<Image>().sprite = TextureHandler.ConvertToSprite(1, true);
            IF_Obj.GetComponent<Image>().type = Image.Type.Tiled;
            ThemeHandler.AddToFGTheme(IF_Obj, false);

            if (DebugWindow) { MelonLogger.Msg("InputField - Images set"); }
            #endregion

            #region Set RectTransforms
            IF_Obj.GetComponent<RectTransform>().sizeDelta = Size;
            IF_TextArea.GetComponent<RectTransform>().sizeDelta = new Vector2(-20, 0);
            IF_Placeholder.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            IF_Text.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);

            if (DebugWindow) { MelonLogger.Msg("InputField - Image Transforms set"); }
            #endregion

            #region Link Objects
            IF_Obj.GetComponent<TMP_InputField>().targetGraphic = IF_Obj.GetComponent<Image>();
            IF_Obj.GetComponent<TMP_InputField>().textViewport = IF_TextArea.GetComponent<RectTransform>();
            IF_Obj.GetComponent<TMP_InputField>().textComponent = IF_Text.GetComponent<TextMeshProUGUI>();
            IF_Obj.GetComponent<TMP_InputField>().placeholder = IF_Placeholder.GetComponent<TextMeshProUGUI>();
            if (DebugWindow) { MelonLogger.Msg("InputField - Objects linked"); }
            #endregion

            #region Positioning and Scaling
            SetLocalPosAndScale(IF_Obj, Position, Scl_1x1);
            SetLocalPosAndScale(IF_TextArea, Vector3.zero, Scl_1x1);
            SetLocalPosAndScale(IF_Placeholder, Vector3.zero, Scl_1x1);
            SetLocalPosAndScale(IF_Text, Vector3.zero, Scl_1x1);

            if (DebugWindow) { MelonLogger.Msg("InputField - Positions set"); }
            #endregion

            #region Alignment
            SetAnchors(IF_Obj, AnchorPresets.TopLeft);
            SetPivot(IF_Obj, PivotPresets.TopLeft);

            SetAnchors(IF_TextArea, AnchorPresets.StretchAll);
            SetPivot(IF_TextArea, PivotPresets.MiddleCenter);

            SetAnchors(IF_Placeholder, AnchorPresets.StretchAll);
            SetPivot(IF_Placeholder, PivotPresets.MiddleCenter);

            SetAnchors(IF_Text, AnchorPresets.StretchAll);
            SetPivot(IF_Text, PivotPresets.MiddleCenter);

            if (DebugWindow) { MelonLogger.Msg("InputField - Alignments set"); }
            #endregion

            Elements.Add(IF_Obj);

            return IF_Obj;
        }

        /// <summary>
        /// Creates a Togglebox Object.
        /// </summary>
        public GameObject CreateToggle(string Name, Transform Parent, Vector3 Position, Vector2 Size)
        {
            #region Objects
            GameObject TB_Obj = new GameObject { name = Name };
            GameObject TB_Background = new GameObject { name = "Background" };
            GameObject TB_Checkmark = new GameObject { name = "Checkmark" };
            //GameObject TB_Label = new GameObject { name = "Label" };

            if (DebugWindow) { MelonLogger.Msg("ToggleBox - Objects set"); }
            #endregion

            #region Set Transforms
            TB_Obj.transform.SetParent(Parent);
            TB_Background.transform.SetParent(TB_Obj.transform);
            TB_Checkmark.transform.SetParent(TB_Background.transform);
            //TB_Label.transform.SetParent(TB_Obj.transform);

            if (DebugWindow) { MelonLogger.Msg("ToggleBox - Parents set"); }
            #endregion

            #region Add Components
            TB_Obj.AddComponent<Toggle>();

            TB_Background.AddComponent<RectTransform>();
            TB_Background.AddComponent<Image>();

            TB_Checkmark.AddComponent<RectTransform>();
            TB_Checkmark.AddComponent<Image>();

            //TB_Label.AddComponent<TextMeshProUGUI>();

            if (DebugWindow) { MelonLogger.Msg("ToggleBox - Components set"); }
            #endregion

            #region Set Text Settings
            //SetTextProperties(TB_Label, "", 20);

            if (DebugWindow) { MelonLogger.Msg("ToggleBox - Text set"); }
            #endregion

            #region Set "Checkmark" Settings
            TB_Checkmark.GetComponent<Image>().sprite = TextureHandler.ConvertToSprite(1, true);
            TB_Checkmark.GetComponent<Image>().type = Image.Type.Tiled;
            ThemeHandler.AddToFGTheme(TB_Checkmark, true);
            if (DebugWindow) { MelonLogger.Msg("ToggleBox - Checkmark set"); }
            #endregion

            #region Set Background Settings
            TB_Background.GetComponent<Image>().sprite = TextureHandler.ConvertToSprite(1, true);
            TB_Background.GetComponent<Image>().type = Image.Type.Tiled;
            //ThemeHandler.AddToBGTheme(TB_Background);
            if (DebugWindow) { MelonLogger.Msg("ToggleBox - Background set"); }
            #endregion

            #region Set Toggle Settings
            if (DebugWindow) { MelonLogger.Msg("ToggleBox - Toggle set"); }
            #endregion

            #region Set RectTransforms
            TB_Obj.GetComponent<RectTransform>().sizeDelta = Size;
            TB_Background.GetComponent<RectTransform>().sizeDelta = Size;
            TB_Checkmark.GetComponent<RectTransform>().sizeDelta = new Vector2(Size.x - 5, Size.y - 5);
            //TB_Label.GetComponent<RectTransform>().sizeDelta = new Vector2(380 - Size_TB.x, Size_TB.y);

            if (DebugWindow) { MelonLogger.Msg("ToggleBox - Transforms set"); }
            #endregion

            #region Link Objects
            TB_Obj.GetComponent<Toggle>().graphic = TB_Checkmark.GetComponent<Image>();
            if (DebugWindow) { MelonLogger.Msg("ToggleBox - Objects linked"); }
            #endregion

            #region Positioning and Scaling
            SetLocalPosAndScale(TB_Obj, Position, Scl_1x1);
            SetLocalPosAndScale(TB_Background, Vector3.zero, Scl_1x1);
            SetLocalPosAndScale(TB_Checkmark, Vector3.zero, Scl_1x1);
            //SetPosition(TB_Label, new Vector3(5, 0f), Scl_1x1);

            if (DebugWindow) { MelonLogger.Msg("ToggleBox - Positions set"); }
            #endregion

            #region Alignment
            SetAnchors(TB_Obj, AnchorPresets.MiddleCenter);
            SetPivot(TB_Obj, PivotPresets.MiddleCenter);

            SetAnchors(TB_Background, AnchorPresets.MiddleCenter);
            SetPivot(TB_Background, PivotPresets.MiddleCenter);

            SetAnchors(TB_Checkmark, AnchorPresets.MiddleCenter);
            SetPivot(TB_Checkmark, PivotPresets.MiddleCenter);

            //SetAnchors(TB_Label, AnchorPresets.MiddleRight);
            //SetPivot(TB_Label, PivotPresets.MiddleLeft);

            if (DebugWindow) { MelonLogger.Msg("ToggleBox - Alignments set"); }
            #endregion

            Elements.Add(TB_Obj);

            return TB_Obj;

        }

        /// <summary>
        /// Creates a TextBox Object.
        /// </summary>
        public GameObject CreateTextBox(string Name,Transform Parent,Vector3 PositionBox, Vector3 PositionText, Vector2 SizeBox, Vector2 SizeText)
        {
            #region Objects
            GameObject D_Obj = new GameObject { name = Name };
            GameObject D_Text = new GameObject { name = "Text" };

            if (DebugWindow) { MelonLogger.Msg("Description - Objects set"); }
            #endregion

            #region Set Parents
            D_Obj.transform.SetParent(Parent);
            D_Text.transform.SetParent(D_Obj.transform);

            if (DebugWindow) { MelonLogger.Msg("Description - Parents set"); }
            #endregion

            #region Add Components
            D_Obj.AddComponent<Image>();
            D_Text.AddComponent<TextMeshProUGUI>();

            if (DebugWindow) { MelonLogger.Msg("Description - Components set"); }
            #endregion

            #region Set Text
            SetTextProperties(D_Text, "", 18);
            D_Text.GetComponent<TextMeshProUGUI>().verticalAlignment = VerticalAlignmentOptions.Top;
            D_Text.GetComponent<TextMeshProUGUI>().enableWordWrapping = true;
            D_Text.GetComponent<TextMeshProUGUI>().overflowMode = TextOverflowModes.Truncate;

            if (DebugWindow) { MelonLogger.Msg("Description - Text set"); }
            #endregion

            #region Set Image
            D_Obj.GetComponent<Image>().sprite = TextureHandler.ConvertToSprite(1, true);
            D_Obj.GetComponent<Image>().type = Image.Type.Tiled;
            ThemeHandler.AddToFGTheme(D_Obj, false);

            if (DebugWindow) { MelonLogger.Msg("Description - Image set"); }
            #endregion

            #region Set RectTransform
            D_Obj.GetComponent<RectTransform>().sizeDelta = SizeBox;
            D_Text.GetComponent<RectTransform>().sizeDelta = SizeText;

            if (DebugWindow) { MelonLogger.Msg("Description - Image Transforms set"); }
            #endregion

            #region Positioning and Scaling
            SetLocalPosAndScale(D_Obj, PositionBox, Scl_1x1);
            SetLocalPosAndScale(D_Text, PositionText, Scl_1x1);

            if (DebugWindow) { MelonLogger.Msg("Description - Position set"); }
            #endregion

            #region Alignment
            SetAnchors(D_Obj, AnchorPresets.StretchAll);
            SetPivot(D_Obj, PivotPresets.TopLeft);

            SetAnchors(D_Text, AnchorPresets.StretchAll);
            SetPivot(D_Text, PivotPresets.TopLeft);

            if (DebugWindow) { MelonLogger.Msg("Title - Anchors/Pivots set"); }
            #endregion

            Elements.Add(D_Obj);

            return D_Obj;
        }

        /// <summary>
        /// Creates a Button Object.
        /// </summary>
        public GameObject CreateButton(string Name,Transform Parent, Vector3 Position, Vector2 Size, string Text)
        {
            #region Objects
            GameObject B_Obj = new GameObject { name = Name };
            GameObject B_Text = new GameObject { name = "Text" };

            if (DebugWindow) { MelonLogger.Msg("Button - Objects set"); }
            #endregion

            #region Set Parents
            B_Obj.transform.SetParent(Parent);
            B_Text.transform.SetParent(B_Obj.transform);

            if (DebugWindow) { MelonLogger.Msg("Button - Parents set"); }
            #endregion

            #region Add Components
            B_Obj.AddComponent<Image>();
            B_Obj.AddComponent<Button>();

            B_Text.AddComponent<TextMeshProUGUI>();

            if (DebugWindow) { MelonLogger.Msg("Button - Components set"); }
            #endregion

            #region Set Text
            SetTextProperties(B_Text, Text, 20);
            B_Text.GetComponent<TextMeshProUGUI>().horizontalAlignment = HorizontalAlignmentOptions.Center;
            B_Text.GetComponent<TextMeshProUGUI>().verticalAlignment = VerticalAlignmentOptions.Middle;

            if (DebugWindow) { MelonLogger.Msg("Button - Text set"); }
            #endregion

            #region Set Image
            B_Obj.GetComponent<Image>().sprite = TextureHandler.ConvertToSprite(1, true);
            B_Obj.GetComponent<Image>().type = Image.Type.Tiled;
            ThemeHandler.AddToFGTheme(B_Obj, false);

            if (DebugWindow) { MelonLogger.Msg("Button - Image set"); }
            #endregion

            #region Set RectTransform
            B_Obj.GetComponent<RectTransform>().sizeDelta = Size;
            B_Text.GetComponent<RectTransform>().sizeDelta = new Vector2(-10f,0f);

            if (DebugWindow) { MelonLogger.Msg("Button - Image Transforms set"); }
            #endregion

            #region Link Object
            B_Obj.GetComponent<Button>().targetGraphic = B_Obj.GetComponent<Image>();

            if (DebugWindow) { MelonLogger.Msg("Button - Links set"); }
            #endregion

            #region Positioning and Scaling
            SetLocalPosAndScale(B_Obj, Position, Scl_1x1);
            SetLocalPosAndScale(B_Text, Vector2.zero, Scl_1x1);

            if (DebugWindow) { MelonLogger.Msg("Button - Position set"); }
            #endregion

            #region Alignment
            SetAnchors(B_Obj, AnchorPresets.BottomLeft);
            SetPivot(B_Obj, PivotPresets.BottomLeft);

            SetAnchors(B_Text, AnchorPresets.StretchAll);
            SetPivot(B_Text, PivotPresets.MiddleCenter);

            if (DebugWindow) { MelonLogger.Msg("Button - Anchors/Pivots set"); }
            #endregion

            Elements.Add(B_Obj);

            return B_Obj;
        }
        #endregion

        /// <summary>
        /// Sets Size and Text of the Title Object. <br/>
        /// Only does something if a Title exists.
        /// </summary>
        public void SetTitleText(string title, float Size)
        {
            if (HasTitle) 
            { 
                SetTextProperties(Obj_Title.transform.GetChild(0).gameObject, title, Size);
                Obj_Title.transform.GetChild(0).GetComponent<TextMeshProUGUI>().horizontalAlignment = HorizontalAlignmentOptions.Center;
                Obj_Title.transform.GetChild(0).GetComponent<TextMeshProUGUI>().verticalAlignment = VerticalAlignmentOptions.Middle;
            }
        }

        private void SetTextProperties(GameObject Input, string Text = "", float fontsize = 16, bool IsPlaceholder = false)
        {
            ThemeHandler.AddToTextTheme(Input);
            Input.GetComponent<TextMeshProUGUI>().text = Text;
            Input.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;
            Input.GetComponent<TextMeshProUGUI>().fontSize = fontsize;
            if (IsPlaceholder)
            {
                Input.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Italic;
            }
        }

        private IEnumerator Dragger()
        {
            WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
            Vector2 Mouse, Offset;

            while (Obj_Title.GetComponent<Button>().currentSelectionState != Selectable.SelectionState.Pressed)
            {
                yield return waitForFixedUpdate;
            }

            Mouse = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            Offset = (Vector2)ParentObject.transform.position - Mouse;

            ParentObject.transform.SetSiblingIndex(ParentObject.transform.parent.childCount - 1);

            while (Obj_Title.GetComponent<Button>().currentSelectionState == Selectable.SelectionState.Pressed)
            {
                Mouse = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                ParentObject.transform.position = Mouse + Offset;
                yield return waitForFixedUpdate;
            }

            RestartDragger();

            yield return null;
        }
        private void RestartDragger()
        {
            if (TitleDragger != null) MelonCoroutines.Stop(TitleDragger);

            TitleDragger = MelonCoroutines.Start(Dragger());
        }

        /// <summary>
        /// Shows the window.
        /// </summary>
        public virtual void ShowWindow()
        {
            ParentObject.SetActive(true);
            OnShow?.Invoke();
            if (HasTitle) TitleDragger = MelonCoroutines.Start(Dragger());
            if (DebugWindow) { MelonLogger.Msg(Name + " - Shown"); }
        }

        /// <summary>
        /// Hides the window.
        /// </summary>
        public virtual void HideWindow()
        {
            if (HasTitle && TitleDragger != null) MelonCoroutines.Stop(TitleDragger);
            ParentObject.SetActive(false);
            OnHide?.Invoke();
            if (DebugWindow) { MelonLogger.Msg(Name + " - Hidden"); }
        }
    }
}
