using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace MaterialSkin
{
    public class MaterialSkinManager
    {
        private static MaterialSkinManager instance;

        private Color primaryColor;
        public Color PrimaryColor
        {
            get { return primaryColor; }
            set { primaryColor = value; PrimaryColorBrush = new SolidBrush(primaryColor); }
        }
        public SolidBrush PrimaryColorBrush { get; set; }

        public Color PrimaryColorDark { get; set; }
        public SolidBrush PrimaryColorDarkBrush { get; set; }

        private Color accentColor;
        public Color AccentColor
        {
            get { return accentColor; }
            set { accentColor = value; AccentColorBrush = new SolidBrush(accentColor); }
        }
        public SolidBrush AccentColorBrush { get; set; }

        public Color MAIN_TEXT_BLACK = Color.FromArgb(222, 0, 0, 0);
        public Color SECONDARY_TEXT_BLACK = Color.FromArgb(138, 0, 0, 0);
        public Color DISABLED_OR_HINT_TEXT_BLACK = Color.FromArgb(66, 0, 0, 0);
        public Color DIVIDERS_BLACK = Color.FromArgb(31, 0, 0, 0);
        public Brush DIVIDERS_BLACK_BRUSH { get { return new SolidBrush(DIVIDERS_BLACK); } }

        public Color MAIN_TEXT_WHITE = Color.FromArgb(255, 255, 255, 255);
        public Color SECONDARY_TEXT_WHITE = Color.FromArgb(179, 255, 255, 255);
        public Color DISABLED_OR_HINT_TEXT_WHITE = Color.FromArgb(77, 255, 255, 255);
        public Color DIVIDERS_WHITE = Color.FromArgb(31, 255, 255, 255);

        public Color BACKGROUND_WHITE = Color.FromArgb(255, 255, 255, 255);

        public Font FONT_BODY1;
        public Font FONT_BODY2;
        public Font FONT_BUTTON;

        public Color CHECKBOX_OFF_LIGHT = Color.FromArgb(138, 0, 0, 0);
        public Color CHECKBOX_OFF_DISABLED_LIGHT = Color.FromArgb(66, 0, 0, 0);

        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pvd, [In] ref uint pcFonts);
        
        private MaterialSkinManager()
        {
            FONT_BUTTON = new Font(LoadFont(Properties.Resources.Roboto_Medium), 10f);
            FONT_BODY1 = new Font(LoadFont(Properties.Resources.Roboto_Regular), 12f);
            FONT_BODY2 = new Font(LoadFont(Properties.Resources.Roboto_Medium), 12f);
        }

        private FontFamily LoadFont(byte[] fontResource)
        {
            int dataLength = fontResource.Length;
            IntPtr fontPtr = Marshal.AllocCoTaskMem(dataLength);

            Marshal.Copy(fontResource, 0, fontPtr, dataLength);

            uint cFonts = 0;

            AddFontMemResourceEx(fontPtr, (uint) fontResource.Length, IntPtr.Zero, ref cFonts);

            PrivateFontCollection privateFontCollection = new PrivateFontCollection();
            privateFontCollection.AddMemoryFont(fontPtr, dataLength);

            Marshal.FreeCoTaskMem(fontPtr);

            FontFamily fontFamily = privateFontCollection.Families[0];
            return fontFamily;
        }

        public static MaterialSkinManager Instance
        {
            get { return instance ?? (instance = new MaterialSkinManager()); }
        }
    }
}
