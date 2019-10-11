namespace MaterialSkin
{
    using MaterialSkin.Controls;
    using MaterialSkin.Properties;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Text;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class MaterialSkinManager
    {
        private static MaterialSkinManager _instance;

        private readonly List<MaterialForm> _formsToManage = new List<MaterialForm>();

        public delegate void SkinManagerEventHandler(object sender);
        public event SkinManagerEventHandler ColorSchemeChanged;
        public event SkinManagerEventHandler ThemeChanged;

        public static MaterialSkinManager Instance => _instance ?? (_instance = new MaterialSkinManager());

        public int FORM_PADDING = 14;

        // Constructor
        private MaterialSkinManager()
        {
            Theme = Themes.LIGHT;
            ColorScheme = new ColorScheme(Primary.Indigo500, Primary.Indigo700, Primary.Indigo100, Accent.Pink200, TextShade.WHITE);

            // Create and cache Roboto fonts
            // Thanks https://www.codeproject.com/Articles/42041/How-to-Use-a-Font-Without-Installing-it
            // And https://www.codeproject.com/Articles/107376/Embedding-Font-To-Resources

            // Add font to system table in memory and save the font family
            addFont(Resources.Roboto_Thin);
            addFont(Resources.Roboto_Light);
            addFont(Resources.Roboto_Regular);
            addFont(Resources.Roboto_Medium);
            addFont(Resources.Roboto_Bold);
            addFont(Resources.Roboto_Black);

            RobotoFontFamilies = new Dictionary<string, FontFamily>();
            foreach (FontFamily ff in privateFontCollection.Families.ToArray())
            {
                RobotoFontFamilies.Add(ff.Name.Replace(' ', '_'), ff);
            }

            // create and save font handles for GDI
            logicalFonts = new Dictionary<string, IntPtr>(18);
            logicalFonts.Add("H1", createLogicalFont("Roboto Light", 96, NativeTextRenderer.logFontWeight.FW_LIGHT));
            logicalFonts.Add("H2", createLogicalFont("Roboto Light", 60, NativeTextRenderer.logFontWeight.FW_LIGHT));
            logicalFonts.Add("H3", createLogicalFont("Roboto", 48, NativeTextRenderer.logFontWeight.FW_REGULAR));
            logicalFonts.Add("H4", createLogicalFont("Roboto", 34, NativeTextRenderer.logFontWeight.FW_REGULAR));
            logicalFonts.Add("H5", createLogicalFont("Roboto", 24, NativeTextRenderer.logFontWeight.FW_REGULAR));
            logicalFonts.Add("H6", createLogicalFont("Roboto Medium", 20, NativeTextRenderer.logFontWeight.FW_MEDIUM));
            logicalFonts.Add("Subtitle1", createLogicalFont("Roboto", 16, NativeTextRenderer.logFontWeight.FW_REGULAR));
            logicalFonts.Add("Subtitle2", createLogicalFont("Roboto Medium", 14, NativeTextRenderer.logFontWeight.FW_MEDIUM));
            logicalFonts.Add("Body1", createLogicalFont("Roboto", 16, NativeTextRenderer.logFontWeight.FW_REGULAR));
            logicalFonts.Add("Body2", createLogicalFont("Roboto", 14, NativeTextRenderer.logFontWeight.FW_REGULAR));
            logicalFonts.Add("Button", createLogicalFont("Roboto Medium", 14, NativeTextRenderer.logFontWeight.FW_MEDIUM));
            logicalFonts.Add("Caption", createLogicalFont("Roboto", 12, NativeTextRenderer.logFontWeight.FW_REGULAR));
            logicalFonts.Add("Overline", createLogicalFont("Roboto", 10, NativeTextRenderer.logFontWeight.FW_REGULAR));
            // Logical fonts for textbox animation
            logicalFonts.Add("textBox16", createLogicalFont("Roboto", 16, NativeTextRenderer.logFontWeight.FW_REGULAR));
            logicalFonts.Add("textBox15", createLogicalFont("Roboto", 15, NativeTextRenderer.logFontWeight.FW_REGULAR));
            logicalFonts.Add("textBox14", createLogicalFont("Roboto", 14, NativeTextRenderer.logFontWeight.FW_REGULAR));
            logicalFonts.Add("textBox13", createLogicalFont("Roboto Medium", 13, NativeTextRenderer.logFontWeight.FW_MEDIUM));
            logicalFonts.Add("textBox12", createLogicalFont("Roboto Medium", 12, NativeTextRenderer.logFontWeight.FW_MEDIUM));
        }

        // Destructor
        ~MaterialSkinManager()
        {
            // RemoveFontMemResourceEx
            foreach (IntPtr handle in logicalFonts.Values)
            {
                NativeTextRenderer.DeleteObject(handle);
            }
        }

        // Themes
        private Themes _theme;
        public Themes Theme
        {
            get { return _theme; }
            set
            {
                _theme = value;
                UpdateBackgrounds();
                ThemeChanged?.Invoke(this);
            }
        }

        private ColorScheme _colorScheme;
        public ColorScheme ColorScheme
        {
            get { return _colorScheme; }
            set
            {
                _colorScheme = value;
                UpdateBackgrounds();
                ColorSchemeChanged?.Invoke(this);
            }
        }

        public enum Themes : byte
        {
            LIGHT,
            DARK
        }

        // Text
        private static readonly Color TEXT_HIGH_EMPHASIS_LIGHT = Color.FromArgb(222, 255, 255, 255); // Alpha 87%
        private static readonly Brush TEXT_HIGH_EMPHASIS_LIGHT_BRUSH = new SolidBrush(TEXT_HIGH_EMPHASIS_LIGHT);
        private static readonly Color TEXT_HIGH_EMPHASIS_DARK = Color.FromArgb(222, 0, 0, 0); // Alpha 87%
        private static readonly Brush TEXT_HIGH_EMPHASIS_DARK_BRUSH = new SolidBrush(TEXT_HIGH_EMPHASIS_DARK);

        private static readonly Color TEXT_MEDIUM_EMPHASIS_LIGHT = Color.FromArgb(153, 255, 255, 255); // Alpha 60%
        private static readonly Brush TEXT_MEDIUM_EMPHASIS_LIGHT_BRUSH = new SolidBrush(TEXT_MEDIUM_EMPHASIS_LIGHT);
        private static readonly Color TEXT_MEDIUM_EMPHASIS_DARK = Color.FromArgb(153, 0, 0, 0); // Alpha 60%
        private static readonly Brush TEXT_MEDIUM_EMPHASIS_DARK_BRUSH = new SolidBrush(TEXT_MEDIUM_EMPHASIS_DARK);

        private static readonly Color TEXT_DISABLED_OR_HINT_LIGHT = Color.FromArgb(97, 255, 255, 255); // Alpha 38%
        private static readonly Brush TEXT_DISABLED_OR_HINT_LIGHT_BRUSH = new SolidBrush(TEXT_DISABLED_OR_HINT_LIGHT);
        private static readonly Color TEXT_DISABLED_OR_HINT_DARK = Color.FromArgb(97, 0, 0, 0); // Alpha 38%
        private static readonly Brush TEXT_DISABLED_OR_HINT_DARK_BRUSH = new SolidBrush(TEXT_DISABLED_OR_HINT_DARK);

        // Dividers and thin lines
        private static readonly Color DIVIDERS_LIGHT = Color.FromArgb(30, 255, 255, 255); // Alpha 30%
        private static readonly Brush DIVIDERS_LIGHT_BRUSH = new SolidBrush(DIVIDERS_LIGHT);
        private static readonly Color DIVIDERS_DARK = Color.FromArgb(30, 0, 0, 0); // Alpha 30%
        private static readonly Brush DIVIDERS_DARK_BRUSH = new SolidBrush(DIVIDERS_DARK);
        private static readonly Color DIVIDERS_ALTERNATIVE_LIGHT = Color.FromArgb(153, 255, 255, 255); // Alpha 60%
        private static readonly Brush DIVIDERS_ALTERNATIVE_LIGHT_BRUSH = new SolidBrush(DIVIDERS_ALTERNATIVE_LIGHT);
        private static readonly Color DIVIDERS_ALTERNATIVE_DARK = Color.FromArgb(153, 0, 0, 0); // Alpha 60%
        private static readonly Brush DIVIDERS_ALTERNATIVE_DARK_BRUSH = new SolidBrush(DIVIDERS_ALTERNATIVE_DARK);

        // Checkbox / Radio / Switches
        private static readonly Color CHECKBOX_OFF_LIGHT = Color.FromArgb(138, 0, 0, 0);
        private static readonly Brush CHECKBOX_OFF_LIGHT_BRUSH = new SolidBrush(CHECKBOX_OFF_LIGHT);
        private static readonly Color CHECKBOX_OFF_DARK = Color.FromArgb(179, 255, 255, 255);
        private static readonly Brush CHECKBOX_OFF_DARK_BRUSH = new SolidBrush(CHECKBOX_OFF_DARK);
        private static readonly Color CHECKBOX_OFF_DISABLED_LIGHT = Color.FromArgb(66, 0, 0, 0);
        private static readonly Brush CHECKBOX_OFF_DISABLED_LIGHT_BRUSH = new SolidBrush(CHECKBOX_OFF_DISABLED_LIGHT);
        private static readonly Color CHECKBOX_OFF_DISABLED_DARK = Color.FromArgb(77, 255, 255, 255);
        private static readonly Brush CHECKBOX_OFF_DISABLED_DARK_BRUSH = new SolidBrush(CHECKBOX_OFF_DISABLED_DARK);

        // Switch specific
        private static readonly Color SWITCH_OFF_THUMB_LIGHT = Color.FromArgb(255, 255, 255, 255);
        private static readonly Color SWITCH_OFF_THUMB_DARK = Color.FromArgb(255, 190, 190, 190);
        private static readonly Color SWITCH_OFF_TRACK_LIGHT = Color.FromArgb(100, 0, 0, 0);
        private static readonly Color SWITCH_OFF_TRACK_DARK = Color.FromArgb(100, 255, 255, 255);
        private static readonly Color SWITCH_OFF_DISABLED_THUMB_LIGHT = Color.FromArgb(255, 230, 230, 230);
        private static readonly Color SWITCH_OFF_DISABLED_THUMB_DARK = Color.FromArgb(255, 150, 150, 150);

        // Generic back colors - for user controls
        private static readonly Color BACKGROUND_LIGHT = Color.FromArgb(255, 255, 255, 255);
        private static readonly Brush BACKGROUND_LIGHT_BRUSH = new SolidBrush(BACKGROUND_LIGHT);
        private static readonly Color BACKGROUND_DARK = Color.FromArgb(255, 80, 80, 80);
        private static readonly Brush BACKGROUND_DARK_BRUSH = new SolidBrush(BACKGROUND_DARK);
        private static readonly Color BACKGROUND_ALTERNATIVE_LIGHT = Color.FromArgb(10, 0, 0, 0);
        private static readonly Brush BACKGROUND_ALTERNATIVE_LIGHT_BRUSH = new SolidBrush(BACKGROUND_ALTERNATIVE_LIGHT);
        private static readonly Color BACKGROUND_ALTERNATIVE_DARK = Color.FromArgb(10, 255, 255, 255);
        private static readonly Brush BACKGROUND_ALTERNATIVE_DARK_BRUSH = new SolidBrush(BACKGROUND_ALTERNATIVE_DARK);
        private static readonly Color BACKGROUND_HOVER_LIGHT = Color.FromArgb(20, 0, 0, 0);
        private static readonly Brush BACKGROUND_HOVER_LIGHT_BRUSH = new SolidBrush(BACKGROUND_HOVER_LIGHT);
        private static readonly Color BACKGROUND_HOVER_DARK = Color.FromArgb(20, 255, 255, 255);
        private static readonly Brush BACKGROUND_HOVER_DARK_BRUSH = new SolidBrush(BACKGROUND_HOVER_DARK);
        private static readonly Color BACKGROUND_FOCUS_LIGHT = Color.FromArgb(30, 0, 0, 0);
        private static readonly Brush BACKGROUND_FOCUS_LIGHT_BRUSH = new SolidBrush(BACKGROUND_FOCUS_LIGHT);
        private static readonly Color BACKGROUND_FOCUS_DARK = Color.FromArgb(30, 255, 255, 255);
        private static readonly Brush BACKGROUND_FOCUS_DARK_BRUSH = new SolidBrush(BACKGROUND_FOCUS_DARK);
        private static readonly Color BACKGROUND_DISABLED_LIGHT = Color.FromArgb(25, 0, 0, 0);
        private static readonly Brush BACKGROUND_DISABLED_LIGHT_BRUSH = new SolidBrush(BACKGROUND_DISABLED_LIGHT);
        private static readonly Color BACKGROUND_DISABLED_DARK = Color.FromArgb(25, 255, 255, 255);
        private static readonly Brush BACKGROUND_DISABLED_DARK_BRUSH = new SolidBrush(BACKGROUND_DISABLED_DARK);

        // Backdrop colors - for containers, like forms or panels
        private static readonly Color BACKDROP_LIGHT = Color.FromArgb(255, 242, 242, 242);
        private static readonly Brush BACKDROP_LIGHT_BRUSH = new SolidBrush(BACKGROUND_LIGHT);
        private static readonly Color BACKDROP_DARK = Color.FromArgb(255, 50, 50, 50);
        private static readonly Brush BACKDROP_DARK_BRUSH = new SolidBrush(BACKGROUND_DARK);

        // Getters - Using these makes handling the dark theme switching easier
        // Text
        public Color TextHighEmphasisColor => Theme == Themes.LIGHT ? TEXT_HIGH_EMPHASIS_DARK : TEXT_HIGH_EMPHASIS_LIGHT;
        public Brush TextHighEmphasisBrush => Theme == Themes.LIGHT ? TEXT_HIGH_EMPHASIS_DARK_BRUSH : TEXT_HIGH_EMPHASIS_LIGHT_BRUSH;
        public Color TextMediumEmphasisColor => Theme == Themes.LIGHT ? TEXT_MEDIUM_EMPHASIS_DARK : TEXT_MEDIUM_EMPHASIS_LIGHT;
        public Brush TextMediumEmphasisBrush => Theme == Themes.LIGHT ? TEXT_MEDIUM_EMPHASIS_DARK_BRUSH : TEXT_MEDIUM_EMPHASIS_LIGHT_BRUSH;
        public Color TextDisabledOrHintColor => Theme == Themes.LIGHT ? TEXT_DISABLED_OR_HINT_DARK : TEXT_DISABLED_OR_HINT_LIGHT;
        public Brush TextDisabledOrHintBrush => Theme == Themes.LIGHT ? TEXT_DISABLED_OR_HINT_DARK_BRUSH : TEXT_DISABLED_OR_HINT_LIGHT_BRUSH;

        // Divider
        public Color DividersColor => Theme == Themes.LIGHT ? DIVIDERS_DARK : DIVIDERS_LIGHT;
        public Brush DividersBrush => Theme == Themes.LIGHT ? DIVIDERS_DARK_BRUSH : DIVIDERS_LIGHT_BRUSH;
        public Color DividersAlternativeColor => Theme == Themes.LIGHT ? DIVIDERS_ALTERNATIVE_DARK : DIVIDERS_ALTERNATIVE_LIGHT;
        public Brush DividersAlternativeBrush => Theme == Themes.LIGHT ? DIVIDERS_ALTERNATIVE_DARK_BRUSH : DIVIDERS_ALTERNATIVE_LIGHT_BRUSH;

        // Checkbox / Radio / Switch
        public Color CheckboxOffColor => Theme == Themes.LIGHT ? CHECKBOX_OFF_LIGHT : CHECKBOX_OFF_DARK;
        public Brush CheckboxOffBrush => Theme == Themes.LIGHT ? CHECKBOX_OFF_LIGHT_BRUSH : CHECKBOX_OFF_DARK_BRUSH;
        public Color CheckBoxOffDisabledColor => Theme == Themes.LIGHT ? CHECKBOX_OFF_DISABLED_LIGHT : CHECKBOX_OFF_DISABLED_DARK;
        public Brush CheckBoxOffDisabledBrush => Theme == Themes.LIGHT ? CHECKBOX_OFF_DISABLED_LIGHT_BRUSH : CHECKBOX_OFF_DISABLED_DARK_BRUSH;

        // Switch
        public Color SwitchOffColor => Theme == Themes.LIGHT ? CHECKBOX_OFF_DARK : CHECKBOX_OFF_LIGHT;
        public Color SwitchOffThumbColor => Theme == Themes.LIGHT ? SWITCH_OFF_THUMB_LIGHT : SWITCH_OFF_THUMB_DARK;
        public Color SwitchOffTrackColor => Theme == Themes.LIGHT ? SWITCH_OFF_TRACK_LIGHT : SWITCH_OFF_TRACK_DARK;
        public Color SwitchOffDisabledThumbColor => Theme == Themes.LIGHT ? SWITCH_OFF_DISABLED_THUMB_LIGHT : SWITCH_OFF_DISABLED_THUMB_DARK;

        // Control Back colors
        public Color BackgroundColor => Theme == Themes.LIGHT ? BACKGROUND_LIGHT : BACKGROUND_DARK;
        public Brush BackgroundBrush => Theme == Themes.LIGHT ? BACKGROUND_LIGHT_BRUSH : BACKGROUND_DARK_BRUSH;
        public Color BackgroundAlternativeColor => Theme == Themes.LIGHT ? BACKGROUND_ALTERNATIVE_LIGHT : BACKGROUND_ALTERNATIVE_DARK;
        public Brush BackgroundAlternativeBrush => Theme == Themes.LIGHT ? BACKGROUND_ALTERNATIVE_LIGHT_BRUSH : BACKGROUND_ALTERNATIVE_DARK_BRUSH;
        public Color BackgroundDisabledColor => Theme == Themes.LIGHT ? BACKGROUND_DISABLED_LIGHT : BACKGROUND_DISABLED_DARK;
        public Brush BackgroundDisabledBrush => Theme == Themes.LIGHT ? BACKGROUND_DISABLED_LIGHT_BRUSH : BACKGROUND_DISABLED_DARK_BRUSH;
        public Color BackgroundHoverColor => Theme == Themes.LIGHT ? BACKGROUND_HOVER_LIGHT : BACKGROUND_HOVER_DARK;
        public Brush BackgroundHoverBrush => Theme == Themes.LIGHT ? BACKGROUND_HOVER_LIGHT_BRUSH : BACKGROUND_HOVER_DARK_BRUSH;
        public Color BackgroundFocusColor => Theme == Themes.LIGHT ? BACKGROUND_FOCUS_LIGHT : BACKGROUND_FOCUS_DARK;
        public Brush BackgroundFocusBrush => Theme == Themes.LIGHT ? BACKGROUND_FOCUS_LIGHT_BRUSH : BACKGROUND_FOCUS_DARK_BRUSH;

        // Backdrop color
        public Color BackdropColor => Theme == Themes.LIGHT ? BACKDROP_LIGHT : BACKDROP_DARK;
        public Brush BackdropBrush => Theme == Themes.LIGHT ? BACKDROP_LIGHT_BRUSH : BACKDROP_DARK_BRUSH;


        // Font Handling
        public enum fontType
        {
            H1,
            H2,
            H3,
            H4,
            H5,
            H6,
            Subtitle1,
            Subtitle2,
            Body1,
            Body2,
            Button,
            Caption,
            Overline
        }

        public Font getFontByType(fontType type)
        {
            switch (type)
            {
                case fontType.H1:
                    return new Font(RobotoFontFamilies["Roboto_Light"], 96f, FontStyle.Regular, GraphicsUnit.Pixel);
                case fontType.H2:
                    return new Font(RobotoFontFamilies["Roboto_Light"], 60f, FontStyle.Regular, GraphicsUnit.Pixel);
                case fontType.H3:
                    return new Font(RobotoFontFamilies["Roboto"], 48f, FontStyle.Bold, GraphicsUnit.Pixel);
                case fontType.H4:
                    return new Font(RobotoFontFamilies["Roboto"], 34f, FontStyle.Bold, GraphicsUnit.Pixel);
                case fontType.H5:
                    return new Font(RobotoFontFamilies["Roboto"], 24f, FontStyle.Bold, GraphicsUnit.Pixel);
                case fontType.H6:
                    return new Font(RobotoFontFamilies["Roboto_Medium"], 20f, FontStyle.Bold, GraphicsUnit.Pixel);
                case fontType.Subtitle1:
                    return new Font(RobotoFontFamilies["Roboto"], 16f, FontStyle.Regular, GraphicsUnit.Pixel);
                case fontType.Subtitle2:
                    return new Font(RobotoFontFamilies["Roboto_Medium"], 14f, FontStyle.Bold, GraphicsUnit.Pixel);
                case fontType.Body1:
                    return new Font(RobotoFontFamilies["Roboto"], 14f, FontStyle.Regular, GraphicsUnit.Pixel);
                case fontType.Body2:
                    return new Font(RobotoFontFamilies["Roboto"], 12f, FontStyle.Regular, GraphicsUnit.Pixel);
                case fontType.Button:
                    return new Font(RobotoFontFamilies["Roboto"], 14f, FontStyle.Bold, GraphicsUnit.Pixel);
                case fontType.Caption:
                    return new Font(RobotoFontFamilies["Roboto"], 12f, FontStyle.Regular, GraphicsUnit.Pixel);
                case fontType.Overline:
                    return new Font(RobotoFontFamilies["Roboto"], 10f, FontStyle.Regular, GraphicsUnit.Pixel);
            }
            return new Font(RobotoFontFamilies["Roboto"], 14f, FontStyle.Regular, GraphicsUnit.Pixel);
        }


        /// <summary>
        /// Get the font by size - used for textbox label animation, try to not use this for anything else
        /// </summary>
        /// <param name="size">font size, ranges from 12 up to 16</param>
        /// <returns></returns>
        public IntPtr getTextBoxFontBySize(int size)
        {
            string name = "textBox" + Math.Min(16, Math.Max(12, size)).ToString();
            return logicalFonts[name];
        }

        /// <summary>
        /// Gets a Material Skin Logical Roboto Font given a standard material font type
        /// </summary>
        /// <param name="type">material design font type</param>
        /// <returns></returns>
        public IntPtr getLogFontByType(fontType type)
        {
            return logicalFonts[Enum.GetName(typeof(fontType), type)];
        }

        // Font stuff
        private Dictionary<string, IntPtr> logicalFonts;
        private Dictionary<string, FontFamily> RobotoFontFamilies;


        private PrivateFontCollection privateFontCollection = new PrivateFontCollection();

        private void addFont(byte[] fontdata)
        {
            // Add font to system table in memory
            int dataLength = fontdata.Length;

            IntPtr ptrFont = Marshal.AllocCoTaskMem(dataLength);
            Marshal.Copy(fontdata, 0, ptrFont, dataLength);

            NativeTextRenderer.AddFontMemResourceEx(fontdata, dataLength, IntPtr.Zero, out _);

            privateFontCollection.AddMemoryFont(ptrFont, dataLength);
        }

        private IntPtr createLogicalFont(string fontName, int size, NativeTextRenderer.logFontWeight weight)
        {
            // Logical font:
            NativeTextRenderer.LogFont lfont = new NativeTextRenderer.LogFont();
            lfont.lfFaceName = fontName;
            lfont.lfHeight = -size;
            lfont.lfWeight = (int)weight;
            return NativeTextRenderer.CreateFontIndirect(lfont);
        }

        // Dyanmic Themes
        public void AddFormToManage(MaterialForm materialForm)
        {
            _formsToManage.Add(materialForm);
            UpdateBackgrounds();
        }

        public void RemoveFormToManage(MaterialForm materialForm)
        {
            _formsToManage.Remove(materialForm);
        }

        private void UpdateBackgrounds()
        {
            var newBackColor = BackdropColor;
            foreach (var materialForm in _formsToManage)
            {
                materialForm.BackColor = newBackColor;
                UpdateControlBackColor(materialForm, newBackColor);
            }
        }

        private void UpdateControlBackColor(Control controlToUpdate, Color newBackColor)
        {
            // No control
            if (controlToUpdate == null) return;

            // Control's Context menu
            if (controlToUpdate.ContextMenuStrip != null) UpdateToolStrip(controlToUpdate.ContextMenuStrip, newBackColor);

            // Material Tabcontrol pages
            if (controlToUpdate is TabPage)
            {
                ((TabPage)controlToUpdate).BackColor = newBackColor;
            }

            // Material Divider
            else if (controlToUpdate is MaterialDivider)
            {
                controlToUpdate.BackColor = DividersColor;
            }

            // Other Material Skin control
            else if (controlToUpdate.IsMaterialControl())
            {
                controlToUpdate.BackColor = newBackColor;
                controlToUpdate.ForeColor = TextHighEmphasisColor;
            }

            // Other Generic control not part of material skin
            else if (controlToUpdate.HasProperty("BackColor") && !controlToUpdate.IsMaterialControl() && controlToUpdate.Parent != null)
            {
                controlToUpdate.BackColor = controlToUpdate.Parent.BackColor;
                if (controlToUpdate.HasProperty("ForeColor"))
                {
                    controlToUpdate.ForeColor = TextHighEmphasisColor;
                    controlToUpdate.Font = getFontByType(MaterialSkinManager.fontType.Body1);
                }
            }

            // Recursive call to control's children
            foreach (Control control in controlToUpdate.Controls)
            {
                UpdateControlBackColor(control, newBackColor);
            }
        }

        private void UpdateToolStrip(ToolStrip toolStrip, Color newBackColor)
        {
            if (toolStrip == null)
            {
                return;
            }

            toolStrip.BackColor = newBackColor;
            foreach (ToolStripItem control in toolStrip.Items)
            {
                control.BackColor = newBackColor;
                if (control is MaterialToolStripMenuItem && (control as MaterialToolStripMenuItem).HasDropDown)
                {

                    //recursive call
                    UpdateToolStrip((control as MaterialToolStripMenuItem).DropDown, newBackColor);
                }
            }
        }
    }

}
