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
    {        private static MaterialSkinManager _instance;
        private readonly List<MaterialForm> _formsToManage = new List<MaterialForm>();

        public delegate void SkinManagerEventHandler(object sender);
        public event SkinManagerEventHandler ColorSchemeChanged;
        public event SkinManagerEventHandler ThemeChanged;                private Themes _theme;
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
        {            LIGHT,            DARK
        }

        private static readonly Color PRIMARY_TEXT_LIGHT = Color.FromArgb(255, 255, 255, 255);
        private static readonly Brush PRIMARY_TEXT_LIGHT_BRUSH = new SolidBrush(PRIMARY_TEXT_LIGHT);        private static readonly Color PRIMARY_TEXT_DARK = Color.FromArgb(222, 0, 0, 0);
        private static readonly Brush PRIMARY_TEXT_DARK_BRUSH = new SolidBrush(PRIMARY_TEXT_DARK);
        private static readonly Color SECONDARY_TEXT_LIGHT = Color.FromArgb(179, 255, 255, 255);
        private static readonly Brush SECONDARY_TEXT_LIGHT_BRUSH = new SolidBrush(SECONDARY_TEXT_LIGHT);
        private static readonly Color SECONDARY_TEXT_DARK = Color.FromArgb(138, 0, 0, 0);
        private static readonly Brush SECONDARY_TEXT_DARK_BRUSH = new SolidBrush(SECONDARY_TEXT_DARK);
        private static readonly Color DISABLED_OR_HINT_TEXT_LIGHT = Color.FromArgb(30, 255, 255, 255);
        private static readonly Brush DISABLED_OR_HINT_TEXT_LIGHT_BRUSH = new SolidBrush(DISABLED_OR_HINT_TEXT_LIGHT);
        private static readonly Color DISABLED_OR_HINT_TEXT_DARK = Color.FromArgb(60, 0, 0, 0);
        private static readonly Brush DISABLED_OR_HINT_TEXT_DARK_BRUSH = new SolidBrush(DISABLED_OR_HINT_TEXT_DARK);

        private static readonly Color DIVIDERS_LIGHT = Color.FromArgb(31, 255, 255, 255);
        private static readonly Brush DIVIDERS_LIGHT_BRUSH = new SolidBrush(DIVIDERS_LIGHT);        private static readonly Color DIVIDERS_DARK = Color.FromArgb(31, 0, 0, 0);
        private static readonly Brush DIVIDERS_DARK_BRUSH = new SolidBrush(DIVIDERS_DARK);
        private static readonly Color CHECKBOX_OFF_LIGHT = Color.FromArgb(138, 0, 0, 0);
        private static readonly Brush CHECKBOX_OFF_LIGHT_BRUSH = new SolidBrush(CHECKBOX_OFF_LIGHT);
        private static readonly Color CHECKBOX_OFF_DARK = Color.FromArgb(179, 255, 255, 255);
        private static readonly Brush CHECKBOX_OFF_DARK_BRUSH = new SolidBrush(CHECKBOX_OFF_DARK);
        private static readonly Color CHECKBOX_OFF_DISABLED_LIGHT = Color.FromArgb(66, 0, 0, 0);
        private static readonly Brush CHECKBOX_OFF_DISABLED_LIGHT_BRUSH = new SolidBrush(CHECKBOX_OFF_DISABLED_LIGHT);
        private static readonly Color CHECKBOX_OFF_DISABLED_DARK = Color.FromArgb(77, 255, 255, 255);
        private static readonly Brush CHECKBOX_OFF_DISABLED_DARK_BRUSH = new SolidBrush(CHECKBOX_OFF_DISABLED_DARK);

        private static readonly Color SWITCH_OFF_THUMB_LIGHT = Color.FromArgb(255, 255, 255, 255);
        private static readonly Color SWITCH_OFF_THUMB_DARK = Color.FromArgb(255, 190, 190, 190);
        private static readonly Color SWITCH_OFF_TRACK_LIGHT = Color.FromArgb(100, 0, 0, 0);
        private static readonly Color SWITCH_OFF_TRACK_DARK = Color.FromArgb(100, 255, 255, 255);
        private static readonly Color SWITCH_OFF_DISABLED_THUMB_LIGHT = Color.FromArgb(255, 230, 230, 230);
        private static readonly Color SWITCH_OFF_DISABLED_THUMB_DARK = Color.FromArgb(255, 150, 150, 150);
        private static readonly Color SWITCH_OFF_DISABLED_TRACK_LIGHT = Color.FromArgb(50, 0, 0, 0);
        private static readonly Color SWITCH_OFF_DISABLED_TRACK_DARK = Color.FromArgb(50, 255, 255, 255);

        private static readonly Color BUTTON_BACKGROUND_LIGHT = Color.FromArgb(255, 255, 255, 255);
        private static readonly Brush BUTTON_BACKGROUND_LIGHT_BRUSH = new SolidBrush(BUTTON_BACKGROUND_LIGHT);        private static readonly Color BUTTON_BACKGROUND_DARK = Color.FromArgb(40, 40, 40, 255);
        private static readonly Brush BUTTON_BACKGROUND_DARK_BRUSH = new SolidBrush(BUTTON_BACKGROUND_DARK);
        private static readonly Color BUTTON_BACKGROUND_HOVER_LIGHT = Color.FromArgb(20.PercentageToColorComponent(), 0x999999.ToColor());
        private static readonly Brush BUTTON_BACKGROUND_HOVER_LIGHT_BRUSH = new SolidBrush(BUTTON_BACKGROUND_HOVER_LIGHT);
        private static readonly Color BUTTON_BACKGROUND_HOVER_DARK = Color.FromArgb(15.PercentageToColorComponent(), 0xCCCCCC.ToColor());
        private static readonly Brush BUTTON_BACKGROUND_HOVER_DARK_BRUSH = new SolidBrush(BUTTON_BACKGROUND_HOVER_DARK);
        private static readonly Color BUTTON_BACKGROUND_PRESSED_LIGHT = Color.FromArgb(40.PercentageToColorComponent(), 0x999999.ToColor());
        private static readonly Brush BUTTON_BACKGROUND_PRESSED_LIGHT_BRUSH = new SolidBrush(BUTTON_BACKGROUND_PRESSED_LIGHT);
        private static readonly Color BUTTON_BACKGROUND_PRESSED_DARK = Color.FromArgb(25.PercentageToColorComponent(), 0xCCCCCC.ToColor());
        private static readonly Brush BUTTON_BACKGROUND_PRESSED_DARK_BRUSH = new SolidBrush(BUTTON_BACKGROUND_PRESSED_DARK);
        private static readonly Color BUTTON_TEXT_LIGHT = PRIMARY_TEXT_LIGHT;
        private static readonly Brush BUTTON_TEXT_LIGHT_BRUSH = new SolidBrush(BUTTON_TEXT_LIGHT);
        private static readonly Color BUTTON_TEXT_DARK = PRIMARY_TEXT_DARK;
        private static readonly Brush BUTTON_TEXT_DARK_BRUSH = new SolidBrush(BUTTON_TEXT_DARK);
        private static readonly Color BUTTON_DISABLEDTEXT_LIGHT = Color.FromArgb(26.PercentageToColorComponent(), 0x000000.ToColor());
        private static readonly Brush BUTTON_DISABLEDTEXT_LIGHT_BRUSH = new SolidBrush(BUTTON_DISABLEDTEXT_LIGHT);
        private static readonly Color BUTTON_DISABLEDTEXT_DARK = Color.FromArgb(30.PercentageToColorComponent(), 0xFFFFFF.ToColor());
        private static readonly Brush BUTTON_DISABLEDTEXT_DARK_BRUSH = new SolidBrush(BUTTON_DISABLEDTEXT_DARK);
        private static readonly Color CMS_BACKGROUND_LIGHT_HOVER = Color.FromArgb(255, 238, 238, 238);
        private static readonly Brush CMS_BACKGROUND_HOVER_LIGHT_BRUSH = new SolidBrush(CMS_BACKGROUND_LIGHT_HOVER);
        private static readonly Color CMS_BACKGROUND_DARK_HOVER = Color.FromArgb(38, 204, 204, 204);
        private static readonly Brush CMS_BACKGROUND_HOVER_DARK_BRUSH = new SolidBrush(CMS_BACKGROUND_DARK_HOVER);
        private static readonly Color BACKGROUND_LIGHT = Color.FromArgb(255, 255, 255, 255);
        private static readonly Brush BACKGROUND_LIGHT_BRUSH = new SolidBrush(BACKGROUND_LIGHT);
        private static readonly Color BACKGROUND_DARK = Color.FromArgb(255, 51, 51, 51);
        private static readonly Brush BACKGROUND_DARK_BRUSH = new SolidBrush(BACKGROUND_DARK);
        public readonly Color ACTION_BAR_TEXT = Color.FromArgb(255, 255, 255, 255);
        public readonly Brush ACTION_BAR_TEXT_BRUSH = new SolidBrush(Color.FromArgb(255, 255, 255, 255));
        public readonly Color ACTION_BAR_TEXT_SECONDARY = Color.FromArgb(153, 255, 255, 255);
        public readonly Brush ACTION_BAR_TEXT_SECONDARY_BRUSH = new SolidBrush(Color.FromArgb(153, 255, 255, 255));

        public Color GetSwitchOffThumbColor()
        {
            return Theme == Themes.LIGHT ? SWITCH_OFF_THUMB_LIGHT : SWITCH_OFF_THUMB_DARK;
        }
        public Color GetSwitchOffTrackColor()
        {
            return Theme == Themes.LIGHT ? SWITCH_OFF_TRACK_LIGHT : SWITCH_OFF_TRACK_DARK;
        }
        public Color GetSwitchOffDisabledThumbColor()
        {
            return Theme == Themes.LIGHT ? SWITCH_OFF_DISABLED_THUMB_LIGHT : SWITCH_OFF_DISABLED_THUMB_DARK;
        }
        public Color GetSwitchOffDisabledTrackColor()
        {
            return Theme == Themes.LIGHT ? SWITCH_OFF_DISABLED_TRACK_LIGHT : SWITCH_OFF_DISABLED_TRACK_DARK;
        }        public Color GetPrimaryTextColor()
        {
            return Theme == Themes.LIGHT ? PRIMARY_TEXT_DARK : PRIMARY_TEXT_LIGHT;
        }
        public Brush GetPrimaryTextBrush()
        {
            return Theme == Themes.LIGHT ? PRIMARY_TEXT_DARK_BRUSH : PRIMARY_TEXT_LIGHT_BRUSH;
        }
        public Color GetSecondaryTextColor()
        {
            return Theme == Themes.LIGHT ? SECONDARY_TEXT_DARK : SECONDARY_TEXT_LIGHT;
        }
        public Brush GetSecondaryTextBrush()
        {
            return Theme == Themes.LIGHT ? SECONDARY_TEXT_DARK_BRUSH : SECONDARY_TEXT_LIGHT_BRUSH;
        }
        public Color GetDisabledOrHintColor()
        {
            return Theme == Themes.LIGHT ? DISABLED_OR_HINT_TEXT_DARK : DISABLED_OR_HINT_TEXT_LIGHT;
        }
        public Brush GetDisabledOrHintBrush()
        {
            return Theme == Themes.LIGHT ? DISABLED_OR_HINT_TEXT_DARK_BRUSH : DISABLED_OR_HINT_TEXT_LIGHT_BRUSH;
        }
        public Color GetDividersColor()
        {
            return Theme == Themes.LIGHT ? DIVIDERS_DARK : DIVIDERS_LIGHT;
        }
        public Brush GetDividersBrush()
        {
            return Theme == Themes.LIGHT ? DIVIDERS_DARK_BRUSH : DIVIDERS_LIGHT_BRUSH;
        }
        public Color GetCheckboxOffColor()
        {
            return Theme == Themes.LIGHT ? CHECKBOX_OFF_LIGHT : CHECKBOX_OFF_DARK;
        }

        public Color GetSwitchOffColor()
        {
            return Theme == Themes.LIGHT ? CHECKBOX_OFF_DARK : CHECKBOX_OFF_LIGHT;
        }
        public Brush GetCheckboxOffBrush()
        {
            return Theme == Themes.LIGHT ? CHECKBOX_OFF_LIGHT_BRUSH : CHECKBOX_OFF_DARK_BRUSH;
        }
        public Color GetCheckBoxOffDisabledColor()
        {
            return Theme == Themes.LIGHT ? CHECKBOX_OFF_DISABLED_LIGHT : CHECKBOX_OFF_DISABLED_DARK;
        }
        public Brush GetCheckBoxOffDisabledBrush()
        {
            return Theme == Themes.LIGHT ? CHECKBOX_OFF_DISABLED_LIGHT_BRUSH : CHECKBOX_OFF_DISABLED_DARK_BRUSH;
        }
        public Color GetButtonBackgroundColor()
        {
            return BUTTON_BACKGROUND_LIGHT;
        }
        public Brush GetButtonTextBrush(bool primary)
        {
            return primary ? BUTTON_TEXT_LIGHT_BRUSH : BUTTON_TEXT_DARK_BRUSH;
        }
        public Color GetButtonHoverBackgroundColor()
        {
            return Theme == Themes.LIGHT ? BUTTON_BACKGROUND_HOVER_LIGHT : BUTTON_BACKGROUND_HOVER_DARK;
        }
        public Brush GetButtonHoverBackgroundBrush()
        {
            return Theme == Themes.LIGHT ? BUTTON_BACKGROUND_HOVER_LIGHT_BRUSH : BUTTON_BACKGROUND_HOVER_DARK_BRUSH;
        }        public Color GetButtonPressedBackgroundColor()
        {
            return Theme == Themes.LIGHT ? BUTTON_BACKGROUND_PRESSED_LIGHT : BUTTON_BACKGROUND_PRESSED_DARK;
        }
        public Brush GetButtonPressedBackgroundBrush()
        {
            return Theme == Themes.LIGHT ? BUTTON_BACKGROUND_PRESSED_LIGHT_BRUSH : BUTTON_BACKGROUND_PRESSED_DARK_BRUSH;
        }
        public Brush GetButtonDisabledTextBrush()
        {
            return Theme == Themes.LIGHT ? BUTTON_DISABLEDTEXT_LIGHT_BRUSH : BUTTON_DISABLEDTEXT_DARK_BRUSH;
        }
        public Brush GetCmsSelectedItemBrush()
        {
            return Theme == Themes.LIGHT ? CMS_BACKGROUND_HOVER_LIGHT_BRUSH : CMS_BACKGROUND_HOVER_DARK_BRUSH;
        }
        public Color GetApplicationBackgroundColor()
        {
            return Theme == Themes.LIGHT ? BACKGROUND_LIGHT : BACKGROUND_DARK;
        }
        public Color GetControlBackgroundColor()
        {
            return Theme == Themes.LIGHT ? BACKGROUND_LIGHT.Darken((float)0.02) : BACKGROUND_DARK.Lighten((float)0.05);
        }
        public Font ROBOTO_MEDIUM_12;
        public Font ROBOTO_REGULAR_11;
        public Font ROBOTO_MEDIUM_11;
        public Font ROBOTO_BOLD_11;
        public Font ROBOTO_MEDIUM_10;
        public Font ROBOTO_REGULAR_10;
        public Font ROBOTO_BOLD_10;
        public int FORM_PADDING = 14;
        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pvd, [In] ref uint pcFonts);
        private MaterialSkinManager()
        {
            ROBOTO_REGULAR_10 = new Font(LoadFont(Resources.Roboto_Regular), 10f);
            ROBOTO_MEDIUM_10 = new Font(LoadFont(Resources.Roboto_Medium), 10f);
            ROBOTO_BOLD_10 = new Font(LoadFont(Resources.Roboto_Bold), 10f, FontStyle.Bold);
            ROBOTO_REGULAR_11 = new Font(LoadFont(Resources.Roboto_Regular), 11f);
            ROBOTO_MEDIUM_11 = new Font(LoadFont(Resources.Roboto_Medium), 11f);
            ROBOTO_BOLD_11 = new Font(LoadFont(Resources.Roboto_Bold), 11f, FontStyle.Bold);
            ROBOTO_MEDIUM_12 = new Font(LoadFont(Resources.Roboto_Medium), 12f);

        Theme = Themes.LIGHT;
            ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
        }
        public static MaterialSkinManager Instance => _instance ?? (_instance = new MaterialSkinManager());
        public void AddFormToManage(MaterialForm materialForm)
        {
            _formsToManage.Add(materialForm);
            UpdateBackgrounds();
        }
        public void RemoveFormToManage(MaterialForm materialForm)
        {
            _formsToManage.Remove(materialForm);
        }
        private readonly PrivateFontCollection privateFontCollection = new PrivateFontCollection();
        private FontFamily LoadFont(byte[] fontResource)
        {
            int dataLength = fontResource.Length;
            IntPtr fontPtr = Marshal.AllocCoTaskMem(dataLength);
            Marshal.Copy(fontResource, 0, fontPtr, dataLength);

            uint cFonts = 0;
            AddFontMemResourceEx(fontPtr, (uint)fontResource.Length, IntPtr.Zero, ref cFonts);
            privateFontCollection.AddMemoryFont(fontPtr, dataLength);

            return privateFontCollection.Families.Last();
        }
        private void UpdateBackgrounds()
        {
            var newBackColor = GetApplicationBackgroundColor();
            foreach (var materialForm in _formsToManage)
            {
                materialForm.BackColor = newBackColor;
                UpdateControl(materialForm, newBackColor);
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
        public void UpdateControl(Control controlToUpdate)
        {
            UpdateControl(controlToUpdate, GetApplicationBackgroundColor());
        }
        private void UpdateControl(Control controlToUpdate, Color newBackColor)
        {


            if (controlToUpdate == null)
            {
                return;
            }

            if (controlToUpdate.Name == "TrackMe")
            {
                //Debugger.Break();
            }

            var tabControl = controlToUpdate as MaterialTabControl;
            var CurrentTabPage = controlToUpdate as TabPage;


            if (controlToUpdate.ContextMenuStrip != null)
            {
                UpdateToolStrip(controlToUpdate.ContextMenuStrip, newBackColor);
            }
            if (tabControl != null)
            {
                foreach (TabPage tabPage in tabControl.TabPages)
                {
                    tabPage.BackColor = newBackColor;
                }
            }
            else if (controlToUpdate is MaterialDivider)
            {
                controlToUpdate.BackColor = GetDividersColor();
            }
            else if (controlToUpdate.HasProperty("BackColor") && CurrentTabPage == null && !controlToUpdate.IsMaterialControl())
            {         // if the control has a backcolor property set the colors  
                if (controlToUpdate.BackColor != GetControlBackgroundColor())
                {
                    controlToUpdate.BackColor = GetControlBackgroundColor();
                    if (controlToUpdate.HasProperty("ForeColor") && controlToUpdate.ForeColor != GetPrimaryTextColor())
                    {
                        controlToUpdate.ForeColor = GetPrimaryTextColor();

                        if (controlToUpdate.Font == SystemFonts.DefaultFont) // if the control has not had the font changed from default, set a default
                        {
                            controlToUpdate.Font = ROBOTO_REGULAR_11;
                        }
                    }
                }

            }
            else if (controlToUpdate.IsMaterialControl())
            {
               
                controlToUpdate.BackColor = newBackColor;
                controlToUpdate.ForeColor = GetPrimaryTextColor();
            }

            //recursive call
            foreach (Control control in controlToUpdate.Controls)
            {
                UpdateControl(control, newBackColor);
            }

            controlToUpdate.Invalidate();
        }
    }
}
