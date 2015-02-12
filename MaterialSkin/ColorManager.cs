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
        public static Dictionary<PrimaryColors, PrimaryColorPair> PrimaryPalettes = new Dictionary<PrimaryColors, PrimaryColorPair>()
        {
            { PrimaryColors.Red, new PrimaryColorPair(0xF44336, 0xD32F2F, 0xFFCDD2, 0xFFFFFF) },
            { PrimaryColors.Pink, new PrimaryColorPair(0xE91E63, 0xC2185B, 0xF8BBD0, 0xFFFFFF) },
            { PrimaryColors.Purple, new PrimaryColorPair(0x9C27B0, 0x7B1FA2, 0xE1BEE7, 0xFFFFFF) },
            { PrimaryColors.DeepPurple, new PrimaryColorPair(0x673AB7, 0x512DA8, 0xD1C4E9, 0xFFFFFF) },
            { PrimaryColors.Indigo, new PrimaryColorPair(0x3F51B5, 0x303F9F, 0xC5CAE9, 0xFFFFFF) },
            { PrimaryColors.Blue, new PrimaryColorPair(0x2196F3, 0x1976D2, 0xBBDEFB, 0xFFFFFF) },
            { PrimaryColors.LightBlue, new PrimaryColorPair(0x03A9F4, 0x0288D1, 0xB3E5FC, 0xFFFFFF) },
            { PrimaryColors.Cyan, new PrimaryColorPair(0x00BCD4, 0x0097A7, 0xB2EBF2, 0xFFFFFF) },
            { PrimaryColors.Teal, new PrimaryColorPair(0x009688, 0x00796B, 0xB2DFDB, 0xFFFFFF) },
            { PrimaryColors.Green, new PrimaryColorPair(0x4CAF50, 0x388E3C, 0xC8E6C9, 0xFFFFFF) },
            { PrimaryColors.LightGreen, new PrimaryColorPair(0x8BC34A, 0x689F38, 0xDCEDC8, 0x212121) },
            { PrimaryColors.Lime, new PrimaryColorPair(0xCDDC39, 0xAFB42B, 0xF0F4C3, 0x212121) },
            { PrimaryColors.Yellow, new PrimaryColorPair(0xFFEB3B, 0xFBC02D, 0xFFF9C4, 0x212121) },
            { PrimaryColors.Amber, new PrimaryColorPair(0xFFC107, 0xFFA000, 0xFFECB3, 0x212121) },
            { PrimaryColors.Orange, new PrimaryColorPair(0xFF9800, 0xF57C00, 0xFFE0B2, 0x212121) },
            { PrimaryColors.DeepOrange, new PrimaryColorPair(0xFF5722, 0xE64A19, 0xFFCCBC, 0xFFFFFF) },
            { PrimaryColors.Brown, new PrimaryColorPair(0x795548, 0x5D4037, 0xD7CCC8, 0xFFFFFF) },
            { PrimaryColors.Grey, new PrimaryColorPair(0x9E9E9E, 0x616161, 0xF5F5F5, 0x212121) },
            { PrimaryColors.BlueGrey, new PrimaryColorPair(0x607D8B, 0x455A64, 0xCFD8DC, 0xFFFFFF) }
        };

        public static Dictionary<AccentColors, AccentColorPair> AccentPalettes = new Dictionary<AccentColors, AccentColorPair>()
        {
            { AccentColors.Red, new AccentColorPair(0xFF5252) },
            { AccentColors.Pink, new AccentColorPair(0xFF4081) },
            { AccentColors.Purple, new AccentColorPair(0xE040FB) },
            { AccentColors.DeepPurple, new AccentColorPair(0x7C4DFF) },
            { AccentColors.Indigo, new AccentColorPair(0x536DFE) },
            { AccentColors.Blue, new AccentColorPair(0x448AFF) },
            { AccentColors.LightBlue, new AccentColorPair(0x40C4FF) },
            { AccentColors.Cyan, new AccentColorPair(0x18FFFF) },
            { AccentColors.Teal, new AccentColorPair(0x64FFDA) },
            { AccentColors.Green, new AccentColorPair(0x69F0AE) },
            { AccentColors.LightGreen, new AccentColorPair(0xB2FF59) },
            { AccentColors.Lime, new AccentColorPair(0xEEFF41) },
            { AccentColors.Yellow, new AccentColorPair(0xFFFF00) },
            { AccentColors.Amber, new AccentColorPair(0xFFD740) },
            { AccentColors.Orange, new AccentColorPair(0xFFAB40) },
            { AccentColors.DeepOrange, new AccentColorPair(0xFF6E40) }
        };

        public static ListColorPair ListColors = new ListColorPair(0x212121, 0x727272, 0xB6B6B6);

        public enum PrimaryColors
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

        public enum AccentColors
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
            DeepOrange
        }

        public class PrimaryColorPair
        {
            public readonly Color PrimaryColor, DarkPrimaryColor, LightPrimaryColor, TextColor;
            public readonly Pen PrimaryPen, DarkPrimaryPen, LightPrimaryPen, TextPen;
            public readonly Brush PrimaryBrush, DarkPrimaryBrush, LightPrimaryBrush, TextBrush;

            public PrimaryColorPair(int primary, int darkPrimary, int lightPrimary, int text)
            {
                //Color
                PrimaryColor = primary.ToColor();
                DarkPrimaryColor = darkPrimary.ToColor();
                LightPrimaryColor = lightPrimary.ToColor();
                TextColor = text.ToColor();

                //Pen
                PrimaryPen = new Pen(PrimaryColor);
                DarkPrimaryPen = new Pen(DarkPrimaryColor);
                LightPrimaryPen = new Pen(LightPrimaryColor);
                TextPen = new Pen(TextColor);

                //Brush
                PrimaryBrush = new SolidBrush(PrimaryColor);
                DarkPrimaryBrush = new SolidBrush(DarkPrimaryColor);
                LightPrimaryBrush = new SolidBrush(LightPrimaryColor);
                TextBrush = new SolidBrush(TextColor);
            }
        }
        public class AccentColorPair
        {
            public readonly Color AccentColor;
            public readonly Pen AccentPen;
            public readonly Brush AccentBrush;

            public AccentColorPair(int accent)
            {
                //Color
                AccentColor = accent.ToColor();

                //Pen
                AccentPen = new Pen(AccentColor);

                //Brush
                AccentBrush = new SolidBrush(AccentColor);
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
