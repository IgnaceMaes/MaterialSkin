using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MaterialSkin
{
    public static class ColorManager
    {
        public static Dictionary<Colors, ColorPair> Palettes = new Dictionary<Colors, ColorPair>()
        {
            { Colors.Red, new ColorPair(0xF44336, 0xD32F2F, 0xFFCDD2, 0x536DFE, 0xFFFFFF) },
            { Colors.Pink, new ColorPair(0xE91E63, 0xC2185B, 0xF8BBD0, 0x448AFF, 0xFFFFFF) },
            { Colors.Purple, new ColorPair(0x9C27B0, 0x7B1FA2, 0xE1BEE7, 0xFF9800, 0xFFFFFF) },
            { Colors.DeepPurple, new ColorPair(0x673AB7, 0x512DA8, 0xD1C4E9, 0x8BC34A, 0xFFFFFF) },
            { Colors.Indigo, new ColorPair(0x3F51B5, 0x303F9F, 0xC5CAE9, 0xFF4081, 0xFFFFFF) },
            { Colors.Blue, new ColorPair(0x2196F3, 0x1976D2, 0xBBDEFB, 0xFF5722, 0xFFFFFF) },
            { Colors.LightBlue, new ColorPair(0x03A9F4, 0x0288D1, 0xB3E5FC, 0xFF5722, 0xFFFFFF) },
            { Colors.Cyan, new ColorPair(0x00BCD4, 0x0097A7, 0xB2EBF2, 0xFF5722, 0xFFFFFF) },
            { Colors.Teal, new ColorPair(0x009688, 0x00796B, 0xB2DFDB, 0xFFC107, 0xFFFFFF) },
            { Colors.Green, new ColorPair(0x4CAF50, 0x388E3C, 0xC8E6C9, 0xFF5252, 0xFFFFFF) },
            { Colors.LightGreen, new ColorPair(0x8BC34A, 0x689F38, 0xDCEDC8, 0xFF5252, 0x212121) },
            { Colors.Lime, new ColorPair(0xCDDC39, 0xAFB42B, 0xF0F4C3, 0x536DFE, 0x212121) },
            { Colors.Yellow, new ColorPair(0xFFEB3B, 0xFBC02D, 0xFFF9C4, 0x03A9F4, 0x212121) },
            { Colors.Amber, new ColorPair(0xFFC107, 0xFFA000, 0xFFECB3, 0x448AFF, 0x212121) },
            { Colors.Orange, new ColorPair(0xFF9800, 0xF57C00, 0xFFE0B2, 0x4CAF50, 0x212121) },
            { Colors.DeepOrange, new ColorPair(0xFF5722, 0xE64A19, 0xFFCCBC, 0xFFC107, 0xFFFFFF) },
            { Colors.Brown, new ColorPair(0x795548, 0x5D4037, 0xD7CCC8, 0x448AFF, 0xFFFFFF) },
            { Colors.Grey, new ColorPair(0x9E9E9E, 0x616161, 0xF5F5F5, 0x536DFE, 0x212121) },
            { Colors.BlueGrey, new ColorPair(0x607D8B, 0x455A64, 0xCFD8DC, 0xFF5722, 0xFFFFFF) },
        };

        public static ListColorPair ListColors = new ListColorPair(0x212121, 0x727272, 0xB6B6B6);

        public enum Colors
        {
            Red,
            Pink,
            Purple,
            DeepPurple,
            Indigo,
            Blue,
            LightBlue,
            Cyan,
            Teal,
            Green,
            LightGreen,
            Lime,
            Yellow,
            Amber,
            Orange,
            DeepOrange,
            Brown,
            Grey,
            BlueGrey
        }

        public class ColorPair
        {
            public readonly Color PrimaryColor, DarkPrimaryColor, LightPrimaryColor, AccentColor, TextColor;
            public readonly Pen PrimaryPen, DarkPrimaryPen, LightPrimaryPen, AccentPen, TextPen;
            public readonly Brush PrimaryBrush, DarkPrimaryBrush, LightPrimaryBrush, AccentBrush, TextBrush;
            public ColorPair(int primary, int darkPrimary, int lightPrimary, int accent, int text)
            {
                //Color
                PrimaryColor = primary.ToColor();
                DarkPrimaryColor = darkPrimary.ToColor();
                LightPrimaryColor = lightPrimary.ToColor();
                AccentColor = accent.ToColor();
                TextColor = text.ToColor();

                //Pen
                PrimaryPen = new Pen(PrimaryColor);
                DarkPrimaryPen = new Pen(DarkPrimaryColor);
                LightPrimaryPen = new Pen(LightPrimaryColor);
                AccentPen = new Pen(AccentColor);
                TextPen = new Pen(TextColor);

                //Brush
                PrimaryBrush = new SolidBrush(PrimaryColor);
                DarkPrimaryBrush = new SolidBrush(DarkPrimaryColor);
                LightPrimaryBrush = new SolidBrush(LightPrimaryColor);
                AccentBrush = new SolidBrush(AccentColor);
                TextBrush = new SolidBrush(TextColor);
            }
        }

        public class ListColorPair
        {
            public readonly Color Primary, Secondary, Divider;

            public ListColorPair(int primary, int secondary, int divider)
            {
                Primary = primary.ToColor();
                Secondary = secondary.ToColor();
                Divider = divider.ToColor();
            }
        }
    }

    public static class ColorExtension
    {
        /// <summary>
        /// Convert an integer number to a Color.
        /// </summary>
        /// <returns></returns>
        public static Color ToColor(this int argb)
        {
            return Color.FromArgb(
                (argb & 0xff0000) >> 16,
                (argb & 0xff00) >> 8,
                 argb & 0xff);
        }

        /// <summary>
        /// Removes the alpha component of a color.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color RemoveAlpha(this Color color)
        {
            return Color.FromArgb(color.R, color.G, color.B);
        }

        /// <summary>
        /// Converts a 0-100 integer to a 0-255 color component.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public static int PercentageToColorComponent(this int percentage)
        {
            return (int)((percentage / 100d) * 255d);
        }
    }
}
