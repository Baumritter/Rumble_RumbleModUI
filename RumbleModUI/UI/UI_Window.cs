using MelonLoader;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Il2CppSystem;
using System;

namespace RumbleModUI
{
    public class Window
    {
        public Window(string Name, bool debug = false) { this.Name = Name; this.DebugWindow = debug; }
        public enum AnchorPresets
        {
            TopLeft,
            TopCenter,
            TopRight,

            MiddleLeft,
            MiddleCenter,
            MiddleRight,

            BottomLeft,
            BottomCenter,
            BottomRight,
            BottomStretch,

            VertStretchLeft,
            VertStretchRight,
            VertStretchCenter,

            HorStretchTop,
            HorStretchMiddle,
            HorStretchBottom,

            StretchAll
        }
        public enum PivotPresets
        {
            TopLeft,
            TopCenter,
            TopRight,

            MiddleLeft,
            MiddleCenter,
            MiddleRight,

            BottomLeft,
            BottomCenter,
            BottomRight,
        }

        private Vector3 Scl_1x1 = new Vector3(1f, 1f, 0);
        private bool DebugWindow { get; set; }
        private bool HasTitle { get; set; }
        private GameObject Obj_Title { get; set; }
        private object TitleDragger { get; set; }

        public string Name { get; set; }
        public GameObject ParentObject { get; set; }

        public List<GameObject> Elements = new List<GameObject>();

        public virtual event System.Action OnShow;
        public virtual event System.Action OnHide;
       
        #region UI_Element_Creation
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
            ThemeHandler.AddToFGTheme(T_Obj);

            if (DebugWindow) { MelonLogger.Msg("Title - Image set"); }
            #endregion

            #region Set RectTransform
            T_Obj.GetComponent<RectTransform>().sizeDelta = Size;
            T_Text.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

            if (DebugWindow) { MelonLogger.Msg("Title - Image Transforms set"); }
            #endregion

            #region Positioning and Scaling
            SetPosition(T_Obj, Position, Scl_1x1);
            SetPosition(T_Text, Vector3.zero, Scl_1x1);

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
        public GameObject CreateBackgroundBox(string Name, Transform Parent, Vector3 Position)
        {
            GameObject temp = new GameObject
            {
                name = Name
            };
            temp.transform.SetParent(Parent);

            SetPosition(temp, Position, Scl_1x1);

            temp.AddComponent<Image>();

            temp.GetComponent<Image>().sprite = TextureHandler.ConvertToSprite(1, true);
            temp.GetComponent<Image>().type = Image.Type.Tiled;
            temp.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            ThemeHandler.AddToFGTheme(temp);

            SetAnchors(temp, AnchorPresets.StretchAll);
            SetPivot(temp, PivotPresets.TopLeft);

            Elements.Add(temp);

            return temp;
        }
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

            if (DebugWindow) { MelonLogger.Msg("DropDown - Text set"); }
            #endregion

            #region Set Images
            DD_Arrow.GetComponent<Image>().sprite = TextureHandler.ConvertToSprite(0, false);
            ThemeHandler.AddToBGTheme(DD_Arrow);

            DD_Obj.GetComponent<Image>().sprite = TextureHandler.ConvertToSprite(1, true);
            DD_Obj.GetComponent<Image>().type = Image.Type.Tiled;
            ThemeHandler.AddToFGTheme(DD_Obj);
            DD_Handle.GetComponent<Image>().sprite = TextureHandler.ConvertToSprite(1, true);
            DD_Handle.GetComponent<Image>().type = Image.Type.Tiled;
            ThemeHandler.AddToFGTheme(DD_Handle);
            DD_Template.GetComponent<Image>().sprite = TextureHandler.ConvertToSprite(1, true);
            DD_Template.GetComponent<Image>().type = Image.Type.Tiled;
            ThemeHandler.AddToFGTheme(DD_Template);
            DD_Scrollbar.GetComponent<Image>().sprite = TextureHandler.ConvertToSprite(2, true);
            DD_Scrollbar.GetComponent<Image>().type = Image.Type.Tiled;
            ThemeHandler.AddToBGTheme(DD_Scrollbar);
            DD_Viewport.GetComponent<Image>().sprite = TextureHandler.ConvertToSprite(3, true, 12);
            DD_Viewport.GetComponent<Image>().type = Image.Type.Tiled;
            ThemeHandler.AddToFGTheme(DD_Viewport);

            ThemeHandler.AddToFGTheme(DD_ItemBG);

            if (DebugWindow) { MelonLogger.Msg("DropDown - Images set"); }
            #endregion

            #region Set RectTransforms
            DD_Obj.GetComponent<RectTransform>().sizeDelta = DD_Size;
            DD_Label.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            DD_Arrow.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);         //Arrow Size
            DD_Template.GetComponent<RectTransform>().sizeDelta = Ext_Size;
            DD_Content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 40);        //Height of Content
            DD_Item.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 38);           //Height of Item
            DD_ItemLabel.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            DD_ItemBG.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
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
            DD_Scrollbar.GetComponent<Scrollbar>().SetDirection(Scrollbar.Direction.BottomToTop, false);
            #endregion

            #region Positioning and Scaling
            SetPosition(DD_Obj, Position, Scl_1x1);
            SetPosition(DD_Label, Vector3.zero, Scl_1x1);
            SetPosition(DD_Arrow, Vector3.zero, Scl_1x1);
            SetPosition(DD_Template, Vector3.zero, Scl_1x1);
            SetPosition(DD_Viewport, Vector3.zero, Scl_1x1);
            SetPosition(DD_Content, Vector3.zero, Scl_1x1);
            SetPosition(DD_Item, Vector3.zero, Scl_1x1);
            SetPosition(DD_ItemLabel, Vector3.zero, Scl_1x1);
            SetPosition(DD_ItemBG, Vector3.zero, Scl_1x1);
            SetPosition(DD_Scrollbar, Vector3.zero, Scl_1x1);
            SetPosition(DD_Slide, Vector3.zero, Scl_1x1);
            SetPosition(DD_Handle, Vector3.zero, Scl_1x1);

            if (DebugWindow) { MelonLogger.Msg("DropDown - Position set"); }
            #endregion

            #region Alignment
            SetAnchors(DD_Obj, AnchorPresets.TopLeft);
            SetPivot(DD_Obj, PivotPresets.TopLeft);

            SetAnchors(DD_Label, AnchorPresets.StretchAll);
            SetPivot(DD_Label, PivotPresets.MiddleCenter);
            SetAnchorPos(DD_Label, 10, 0);

            SetAnchors(DD_Arrow, AnchorPresets.MiddleRight);
            SetPivot(DD_Arrow, PivotPresets.MiddleCenter);
            SetAnchorPos(DD_Arrow, -20, -1);

            SetAnchors(DD_Template, AnchorPresets.HorStretchBottom);
            SetPivot(DD_Template, PivotPresets.TopCenter);

            SetAnchors(DD_Viewport, AnchorPresets.StretchAll);
            SetPivot(DD_Viewport, PivotPresets.TopLeft);

            SetAnchors(DD_Content, AnchorPresets.HorStretchTop);
            SetPivot(DD_Content, PivotPresets.TopCenter);

            SetAnchors(DD_Item, AnchorPresets.HorStretchMiddle);
            SetPivot(DD_Item, PivotPresets.MiddleCenter);

            SetAnchors(DD_ItemLabel, AnchorPresets.StretchAll);
            SetPivot(DD_ItemLabel, PivotPresets.MiddleCenter);
            SetAnchorPos(DD_ItemLabel, 15, 0);

            SetAnchors(DD_ItemBG, AnchorPresets.StretchAll);
            SetPivot(DD_ItemBG, PivotPresets.MiddleCenter);

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
            ThemeHandler.AddToFGTheme(IF_Obj);

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
            SetPosition(IF_Obj, Position, Scl_1x1);
            SetPosition(IF_TextArea, Vector3.zero, Scl_1x1);
            SetPosition(IF_Placeholder, Vector3.zero, Scl_1x1);
            SetPosition(IF_Text, Vector3.zero, Scl_1x1);

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
            ThemeHandler.AddToFGTheme(TB_Checkmark);
            if (DebugWindow) { MelonLogger.Msg("ToggleBox - Checkmark set"); }
            #endregion

            #region Set Background Settings
            TB_Background.GetComponent<Image>().sprite = TextureHandler.ConvertToSprite(1, true);
            TB_Background.GetComponent<Image>().type = Image.Type.Tiled;
            ThemeHandler.AddToBGTheme(TB_Background);
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
            SetPosition(TB_Obj, Position, Scl_1x1);
            SetPosition(TB_Background, Vector3.zero, Scl_1x1);
            SetPosition(TB_Checkmark, Vector3.zero, Scl_1x1);
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
            ThemeHandler.AddToFGTheme(D_Obj);

            if (DebugWindow) { MelonLogger.Msg("Description - Image set"); }
            #endregion

            #region Set RectTransform
            D_Obj.GetComponent<RectTransform>().sizeDelta = SizeBox;
            D_Text.GetComponent<RectTransform>().sizeDelta = SizeText;

            if (DebugWindow) { MelonLogger.Msg("Description - Image Transforms set"); }
            #endregion

            #region Positioning and Scaling
            SetPosition(D_Obj, PositionBox, Scl_1x1);
            SetPosition(D_Text, PositionText, Scl_1x1);

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
            ThemeHandler.AddToFGTheme(B_Obj);

            if (DebugWindow) { MelonLogger.Msg("Button - Image set"); }
            #endregion

            #region Set RectTransform
            B_Obj.GetComponent<RectTransform>().sizeDelta = Size;
            B_Text.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

            if (DebugWindow) { MelonLogger.Msg("Button - Image Transforms set"); }
            #endregion

            #region Link Object
            B_Obj.GetComponent<Button>().targetGraphic = B_Obj.GetComponent<Image>();

            if (DebugWindow) { MelonLogger.Msg("Button - Links set"); }
            #endregion

            #region Positioning and Scaling
            SetPosition(B_Obj, Position, Scl_1x1);
            SetPosition(B_Text, Vector3.zero, Scl_1x1);

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


        private void SetPosition(GameObject Input, Vector3 Pos, Vector3 Scale)
        {
            Input.transform.localPosition = Pos;
            Input.transform.localScale = Scale;
        }
        private void SetAnchorPos(GameObject Input, float x, float y)
        {
            Input.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
        }
        private void SetAnchors(GameObject Input, AnchorPresets alignment)
        {
            switch (alignment)
            {
                case (AnchorPresets.TopLeft):
                    AnchorHelper(Input, 0, 1, 0, 1);
                    break;
                case (AnchorPresets.TopCenter):
                    AnchorHelper(Input, .5f, 1, .5f, 1);
                    break;
                case (AnchorPresets.TopRight):
                    AnchorHelper(Input, 1, 1, 1, 1);
                    break;
                case (AnchorPresets.MiddleLeft):
                    AnchorHelper(Input, 0, .5f, 0, .5f);
                    break;
                case (AnchorPresets.MiddleCenter):
                    AnchorHelper(Input, .5f, .5f, .5f, .5f);
                    break;
                case (AnchorPresets.MiddleRight):
                    AnchorHelper(Input, 1, .5f, 1, .5f);
                    break;
                case (AnchorPresets.BottomLeft):
                    AnchorHelper(Input, 0, 0, 0, 0);
                    break;
                case (AnchorPresets.BottomCenter):
                    AnchorHelper(Input, .5f, 0, .5f, 0);
                    break;
                case (AnchorPresets.BottomRight):
                    AnchorHelper(Input, 1, 0, 1, 0);
                    break;
                case (AnchorPresets.BottomStretch):
                    AnchorHelper(Input, 0, 0, 1, 0);
                    break;
                case (AnchorPresets.VertStretchLeft):
                    AnchorHelper(Input, 0, 0, 0, 1);
                    break;
                case (AnchorPresets.VertStretchCenter):
                    AnchorHelper(Input, .5f, 0, .5f, 1);
                    break;
                case (AnchorPresets.VertStretchRight):
                    AnchorHelper(Input, 1, 0, 1, 1);
                    break;
                case (AnchorPresets.HorStretchTop):
                    AnchorHelper(Input, 0, 1, 1, 1);
                    break;
                case (AnchorPresets.HorStretchMiddle):
                    AnchorHelper(Input, 0, .5f, 1, .5f);
                    break;
                case (AnchorPresets.HorStretchBottom):
                    AnchorHelper(Input, 0, 0, 1, 0);
                    break;
                case (AnchorPresets.StretchAll):
                    AnchorHelper(Input, 0, 0, 1, 1);
                    break;
            }
        }
        private void SetPivot(GameObject Input, PivotPresets pivot)
        {
            switch (pivot)
            {
                case (PivotPresets.TopLeft):
                    Input.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
                    break;
                case (PivotPresets.TopCenter):
                    Input.GetComponent<RectTransform>().pivot = new Vector2(.5f, 1);
                    break;
                case (PivotPresets.TopRight):
                    Input.GetComponent<RectTransform>().pivot = new Vector2(1, 1);
                    break;
                case (PivotPresets.MiddleLeft):
                    Input.GetComponent<RectTransform>().pivot = new Vector2(0, .5f);
                    break;
                case (PivotPresets.MiddleCenter):
                    Input.GetComponent<RectTransform>().pivot = new Vector2(.5f, .5f);
                    break;
                case (PivotPresets.MiddleRight):
                    Input.GetComponent<RectTransform>().pivot = new Vector2(1, .5f);
                    break;
                case (PivotPresets.BottomLeft):
                    Input.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
                    break;
                case (PivotPresets.BottomCenter):
                    Input.GetComponent<RectTransform>().pivot = new Vector2(.5f, 0);
                    break;
                case (PivotPresets.BottomRight):
                    Input.GetComponent<RectTransform>().pivot = new Vector2(1, 0);
                    break;
            }
        }
        private void AnchorHelper(GameObject Input, float xmin, float ymin, float xmax, float ymax)
        {
            Input.GetComponent<RectTransform>().anchorMin = new Vector2(xmin, ymin);
            Input.GetComponent<RectTransform>().anchorMax = new Vector2(xmax, ymax);
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

        public virtual void ShowWindow()
        {
            ParentObject.SetActive(true);
            OnShow?.Invoke();
            if (HasTitle) TitleDragger = MelonCoroutines.Start(Dragger());
            if (DebugWindow) { MelonLogger.Msg(Name + " - Shown"); }
        }
        public virtual void HideWindow()
        {
            if (HasTitle && TitleDragger != null) MelonCoroutines.Stop(TitleDragger);
            ParentObject.SetActive(false);
            OnHide?.Invoke();
            if (DebugWindow) { MelonLogger.Msg(Name + " - Hidden"); }
        }
    }
}
