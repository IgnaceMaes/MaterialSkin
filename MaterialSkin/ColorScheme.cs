namespace MaterialSkin
{
    using System.Drawing;

    /// <summary>
    /// Defines the <see cref="ColorScheme" />
    /// </summary>
    public class ColorScheme
    {
        /// <summary>
        /// Defines the PrimaryColor, DarkPrimaryColor, LightPrimaryColor, AccentColor, TextColor
        /// </summary>
        public readonly Color PrimaryColor, DarkPrimaryColor, LightPrimaryColor, AccentColor, TextColor;

        /// <summary>
        /// Defines the PrimaryPen, DarkPrimaryPen, LightPrimaryPen, AccentPen, TextPen
        /// </summary>
        public readonly Pen PrimaryPen, DarkPrimaryPen, LightPrimaryPen, AccentPen, TextPen;

        /// <summary>
        /// Defines the PrimaryBrush, DarkPrimaryBrush, LightPrimaryBrush, AccentBrush, TextBrush
        /// </summary>
        public readonly Brush PrimaryBrush, DarkPrimaryBrush, LightPrimaryBrush, AccentBrush, TextBrush;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorScheme"/> class.
        /// </summary>
        /// <param name="primary">The primary color, a -500 color is suggested here.</param>
        /// <param name="darkPrimary">A darker version of the primary color, a -700 color is suggested here.</param>
        /// <param name="lightPrimary">A lighter version of the primary color, a -100 color is suggested here.</param>
        /// <param name="accent">The accent color, a -200 color is suggested here.</param>
        /// <param name="textShade">The text color, the one with the highest contrast is suggested.</param>
        public ColorScheme(Primary primary, Primary darkPrimary, Primary lightPrimary, Accent accent, TextShade textShade)
        {
            //Color
            PrimaryColor = ((int)primary).ToColor();
            DarkPrimaryColor = ((int)darkPrimary).ToColor();
            LightPrimaryColor = ((int)lightPrimary).ToColor();
            AccentColor = ((int)accent).ToColor();
            TextColor = ((int)textShade).ToColor();

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

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorScheme"/> class.
        /// </summary>
        /// <param name="primary">The primary color</param>
        /// <param name="darkPrimary">A darker version of the primary color</param>
        /// <param name="lightPrimary">A lighter version of the primary color</param>
        /// <param name="accent">The accent color</param>
        /// <param name="textShade">The text color, the one with the highest contrast is suggested.</param>
        public ColorScheme(Color primary, Color darkPrimary, Color lightPrimary, Color accent, TextShade textShade)
        {
            //Color
            PrimaryColor = primary;
            DarkPrimaryColor = darkPrimary;
            LightPrimaryColor = lightPrimary;
            AccentColor = accent;
            TextColor = ((int)textShade).ToColor();

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

    /// <summary>
    /// Defines the <see cref="ColorExtension" />
    /// </summary>
    public static class ColorExtension
    {
        /// <summary>
        /// Convert an integer number to a Color.
        /// </summary>
        /// <param name="argb">The argb<see cref="int"/></param>
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

    //Color constantes
    public enum TextShade
    {
        /// <summary>
        /// Defines the WHITE
        /// </summary>
        WHITE = 0xFFFFFF,

        /// <summary>
        /// Defines the BLACK
        /// </summary>
        BLACK = 0x212121
    }

    /// <summary>
    /// Defines the Primary
    /// </summary>
    public enum Primary
    {
        /// <summary>
        /// Defines the Red50
        /// </summary>
        Red50 = 0xFFEBEE,

        /// <summary>
        /// Defines the Red100
        /// </summary>
        Red100 = 0xFFCDD2,

        /// <summary>
        /// Defines the Red200
        /// </summary>
        Red200 = 0xEF9A9A,

        /// <summary>
        /// Defines the Red300
        /// </summary>
        Red300 = 0xE57373,

        /// <summary>
        /// Defines the Red400
        /// </summary>
        Red400 = 0xEF5350,

        /// <summary>
        /// Defines the Red500
        /// </summary>
        Red500 = 0xF44336,

        /// <summary>
        /// Defines the Red600
        /// </summary>
        Red600 = 0xE53935,

        /// <summary>
        /// Defines the Red700
        /// </summary>
        Red700 = 0xD32F2F,

        /// <summary>
        /// Defines the Red800
        /// </summary>
        Red800 = 0xC62828,

        /// <summary>
        /// Defines the Red900
        /// </summary>
        Red900 = 0xB71C1C,

        /// <summary>
        /// Defines the Pink50
        /// </summary>
        Pink50 = 0xFCE4EC,

        /// <summary>
        /// Defines the Pink100
        /// </summary>
        Pink100 = 0xF8BBD0,

        /// <summary>
        /// Defines the Pink200
        /// </summary>
        Pink200 = 0xF48FB1,

        /// <summary>
        /// Defines the Pink300
        /// </summary>
        Pink300 = 0xF06292,

        /// <summary>
        /// Defines the Pink400
        /// </summary>
        Pink400 = 0xEC407A,

        /// <summary>
        /// Defines the Pink500
        /// </summary>
        Pink500 = 0xE91E63,

        /// <summary>
        /// Defines the Pink600
        /// </summary>
        Pink600 = 0xD81B60,

        /// <summary>
        /// Defines the Pink700
        /// </summary>
        Pink700 = 0xC2185B,

        /// <summary>
        /// Defines the Pink800
        /// </summary>
        Pink800 = 0xAD1457,

        /// <summary>
        /// Defines the Pink900
        /// </summary>
        Pink900 = 0x880E4F,

        /// <summary>
        /// Defines the Purple50
        /// </summary>
        Purple50 = 0xF3E5F5,

        /// <summary>
        /// Defines the Purple100
        /// </summary>
        Purple100 = 0xE1BEE7,

        /// <summary>
        /// Defines the Purple200
        /// </summary>
        Purple200 = 0xCE93D8,

        /// <summary>
        /// Defines the Purple300
        /// </summary>
        Purple300 = 0xBA68C8,

        /// <summary>
        /// Defines the Purple400
        /// </summary>
        Purple400 = 0xAB47BC,

        /// <summary>
        /// Defines the Purple500
        /// </summary>
        Purple500 = 0x9C27B0,

        /// <summary>
        /// Defines the Purple600
        /// </summary>
        Purple600 = 0x8E24AA,

        /// <summary>
        /// Defines the Purple700
        /// </summary>
        Purple700 = 0x7B1FA2,

        /// <summary>
        /// Defines the Purple800
        /// </summary>
        Purple800 = 0x6A1B9A,

        /// <summary>
        /// Defines the Purple900
        /// </summary>
        Purple900 = 0x4A148C,

        /// <summary>
        /// Defines the DeepPurple50
        /// </summary>
        DeepPurple50 = 0xEDE7F6,

        /// <summary>
        /// Defines the DeepPurple100
        /// </summary>
        DeepPurple100 = 0xD1C4E9,

        /// <summary>
        /// Defines the DeepPurple200
        /// </summary>
        DeepPurple200 = 0xB39DDB,

        /// <summary>
        /// Defines the DeepPurple300
        /// </summary>
        DeepPurple300 = 0x9575CD,

        /// <summary>
        /// Defines the DeepPurple400
        /// </summary>
        DeepPurple400 = 0x7E57C2,

        /// <summary>
        /// Defines the DeepPurple500
        /// </summary>
        DeepPurple500 = 0x673AB7,

        /// <summary>
        /// Defines the DeepPurple600
        /// </summary>
        DeepPurple600 = 0x5E35B1,

        /// <summary>
        /// Defines the DeepPurple700
        /// </summary>
        DeepPurple700 = 0x512DA8,

        /// <summary>
        /// Defines the DeepPurple800
        /// </summary>
        DeepPurple800 = 0x4527A0,

        /// <summary>
        /// Defines the DeepPurple900
        /// </summary>
        DeepPurple900 = 0x311B92,

        /// <summary>
        /// Defines the Indigo50
        /// </summary>
        Indigo50 = 0xE8EAF6,

        /// <summary>
        /// Defines the Indigo100
        /// </summary>
        Indigo100 = 0xC5CAE9,

        /// <summary>
        /// Defines the Indigo200
        /// </summary>
        Indigo200 = 0x9FA8DA,

        /// <summary>
        /// Defines the Indigo300
        /// </summary>
        Indigo300 = 0x7986CB,

        /// <summary>
        /// Defines the Indigo400
        /// </summary>
        Indigo400 = 0x5C6BC0,

        /// <summary>
        /// Defines the Indigo500
        /// </summary>
        Indigo500 = 0x3F51B5,

        /// <summary>
        /// Defines the Indigo600
        /// </summary>
        Indigo600 = 0x3949AB,

        /// <summary>
        /// Defines the Indigo700
        /// </summary>
        Indigo700 = 0x303F9F,

        /// <summary>
        /// Defines the Indigo800
        /// </summary>
        Indigo800 = 0x283593,

        /// <summary>
        /// Defines the Indigo900
        /// </summary>
        Indigo900 = 0x1A237E,

        /// <summary>
        /// Defines the Blue50
        /// </summary>
        Blue50 = 0xE3F2FD,

        /// <summary>
        /// Defines the Blue100
        /// </summary>
        Blue100 = 0xBBDEFB,

        /// <summary>
        /// Defines the Blue200
        /// </summary>
        Blue200 = 0x90CAF9,

        /// <summary>
        /// Defines the Blue300
        /// </summary>
        Blue300 = 0x64B5F6,

        /// <summary>
        /// Defines the Blue400
        /// </summary>
        Blue400 = 0x42A5F5,

        /// <summary>
        /// Defines the Blue500
        /// </summary>
        Blue500 = 0x2196F3,

        /// <summary>
        /// Defines the Blue600
        /// </summary>
        Blue600 = 0x1E88E5,

        /// <summary>
        /// Defines the Blue700
        /// </summary>
        Blue700 = 0x1976D2,

        /// <summary>
        /// Defines the Blue800
        /// </summary>
        Blue800 = 0x1565C0,

        /// <summary>
        /// Defines the Blue900
        /// </summary>
        Blue900 = 0x0D47A1,

        /// <summary>
        /// Defines the LightBlue50
        /// </summary>
        LightBlue50 = 0xE1F5FE,

        /// <summary>
        /// Defines the LightBlue100
        /// </summary>
        LightBlue100 = 0xB3E5FC,

        /// <summary>
        /// Defines the LightBlue200
        /// </summary>
        LightBlue200 = 0x81D4FA,

        /// <summary>
        /// Defines the LightBlue300
        /// </summary>
        LightBlue300 = 0x4FC3F7,

        /// <summary>
        /// Defines the LightBlue400
        /// </summary>
        LightBlue400 = 0x29B6F6,

        /// <summary>
        /// Defines the LightBlue500
        /// </summary>
        LightBlue500 = 0x03A9F4,

        /// <summary>
        /// Defines the LightBlue600
        /// </summary>
        LightBlue600 = 0x039BE5,

        /// <summary>
        /// Defines the LightBlue700
        /// </summary>
        LightBlue700 = 0x0288D1,

        /// <summary>
        /// Defines the LightBlue800
        /// </summary>
        LightBlue800 = 0x0277BD,

        /// <summary>
        /// Defines the LightBlue900
        /// </summary>
        LightBlue900 = 0x01579B,

        /// <summary>
        /// Defines the Cyan50
        /// </summary>
        Cyan50 = 0xE0F7FA,

        /// <summary>
        /// Defines the Cyan100
        /// </summary>
        Cyan100 = 0xB2EBF2,

        /// <summary>
        /// Defines the Cyan200
        /// </summary>
        Cyan200 = 0x80DEEA,

        /// <summary>
        /// Defines the Cyan300
        /// </summary>
        Cyan300 = 0x4DD0E1,

        /// <summary>
        /// Defines the Cyan400
        /// </summary>
        Cyan400 = 0x26C6DA,

        /// <summary>
        /// Defines the Cyan500
        /// </summary>
        Cyan500 = 0x00BCD4,

        /// <summary>
        /// Defines the Cyan600
        /// </summary>
        Cyan600 = 0x00ACC1,

        /// <summary>
        /// Defines the Cyan700
        /// </summary>
        Cyan700 = 0x0097A7,

        /// <summary>
        /// Defines the Cyan800
        /// </summary>
        Cyan800 = 0x00838F,

        /// <summary>
        /// Defines the Cyan900
        /// </summary>
        Cyan900 = 0x006064,

        /// <summary>
        /// Defines the Teal50
        /// </summary>
        Teal50 = 0xE0F2F1,

        /// <summary>
        /// Defines the Teal100
        /// </summary>
        Teal100 = 0xB2DFDB,

        /// <summary>
        /// Defines the Teal200
        /// </summary>
        Teal200 = 0x80CBC4,

        /// <summary>
        /// Defines the Teal300
        /// </summary>
        Teal300 = 0x4DB6AC,

        /// <summary>
        /// Defines the Teal400
        /// </summary>
        Teal400 = 0x26A69A,

        /// <summary>
        /// Defines the Teal500
        /// </summary>
        Teal500 = 0x009688,

        /// <summary>
        /// Defines the Teal600
        /// </summary>
        Teal600 = 0x00897B,

        /// <summary>
        /// Defines the Teal700
        /// </summary>
        Teal700 = 0x00796B,

        /// <summary>
        /// Defines the Teal800
        /// </summary>
        Teal800 = 0x00695C,

        /// <summary>
        /// Defines the Teal900
        /// </summary>
        Teal900 = 0x004D40,

        /// <summary>
        /// Defines the Green50
        /// </summary>
        Green50 = 0xE8F5E9,

        /// <summary>
        /// Defines the Green100
        /// </summary>
        Green100 = 0xC8E6C9,

        /// <summary>
        /// Defines the Green200
        /// </summary>
        Green200 = 0xA5D6A7,

        /// <summary>
        /// Defines the Green300
        /// </summary>
        Green300 = 0x81C784,

        /// <summary>
        /// Defines the Green400
        /// </summary>
        Green400 = 0x66BB6A,

        /// <summary>
        /// Defines the Green500
        /// </summary>
        Green500 = 0x4CAF50,

        /// <summary>
        /// Defines the Green600
        /// </summary>
        Green600 = 0x43A047,

        /// <summary>
        /// Defines the Green700
        /// </summary>
        Green700 = 0x388E3C,

        /// <summary>
        /// Defines the Green800
        /// </summary>
        Green800 = 0x2E7D32,

        /// <summary>
        /// Defines the Green900
        /// </summary>
        Green900 = 0x1B5E20,

        /// <summary>
        /// Defines the LightGreen50
        /// </summary>
        LightGreen50 = 0xF1F8E9,

        /// <summary>
        /// Defines the LightGreen100
        /// </summary>
        LightGreen100 = 0xDCEDC8,

        /// <summary>
        /// Defines the LightGreen200
        /// </summary>
        LightGreen200 = 0xC5E1A5,

        /// <summary>
        /// Defines the LightGreen300
        /// </summary>
        LightGreen300 = 0xAED581,

        /// <summary>
        /// Defines the LightGreen400
        /// </summary>
        LightGreen400 = 0x9CCC65,

        /// <summary>
        /// Defines the LightGreen500
        /// </summary>
        LightGreen500 = 0x8BC34A,

        /// <summary>
        /// Defines the LightGreen600
        /// </summary>
        LightGreen600 = 0x7CB342,

        /// <summary>
        /// Defines the LightGreen700
        /// </summary>
        LightGreen700 = 0x689F38,

        /// <summary>
        /// Defines the LightGreen800
        /// </summary>
        LightGreen800 = 0x558B2F,

        /// <summary>
        /// Defines the LightGreen900
        /// </summary>
        LightGreen900 = 0x33691E,

        /// <summary>
        /// Defines the Lime50
        /// </summary>
        Lime50 = 0xF9FBE7,

        /// <summary>
        /// Defines the Lime100
        /// </summary>
        Lime100 = 0xF0F4C3,

        /// <summary>
        /// Defines the Lime200
        /// </summary>
        Lime200 = 0xE6EE9C,

        /// <summary>
        /// Defines the Lime300
        /// </summary>
        Lime300 = 0xDCE775,

        /// <summary>
        /// Defines the Lime400
        /// </summary>
        Lime400 = 0xD4E157,

        /// <summary>
        /// Defines the Lime500
        /// </summary>
        Lime500 = 0xCDDC39,

        /// <summary>
        /// Defines the Lime600
        /// </summary>
        Lime600 = 0xC0CA33,

        /// <summary>
        /// Defines the Lime700
        /// </summary>
        Lime700 = 0xAFB42B,

        /// <summary>
        /// Defines the Lime800
        /// </summary>
        Lime800 = 0x9E9D24,

        /// <summary>
        /// Defines the Lime900
        /// </summary>
        Lime900 = 0x827717,

        /// <summary>
        /// Defines the Yellow50
        /// </summary>
        Yellow50 = 0xFFFDE7,

        /// <summary>
        /// Defines the Yellow100
        /// </summary>
        Yellow100 = 0xFFF9C4,

        /// <summary>
        /// Defines the Yellow200
        /// </summary>
        Yellow200 = 0xFFF59D,

        /// <summary>
        /// Defines the Yellow300
        /// </summary>
        Yellow300 = 0xFFF176,

        /// <summary>
        /// Defines the Yellow400
        /// </summary>
        Yellow400 = 0xFFEE58,

        /// <summary>
        /// Defines the Yellow500
        /// </summary>
        Yellow500 = 0xFFEB3B,

        /// <summary>
        /// Defines the Yellow600
        /// </summary>
        Yellow600 = 0xFDD835,

        /// <summary>
        /// Defines the Yellow700
        /// </summary>
        Yellow700 = 0xFBC02D,

        /// <summary>
        /// Defines the Yellow800
        /// </summary>
        Yellow800 = 0xF9A825,

        /// <summary>
        /// Defines the Yellow900
        /// </summary>
        Yellow900 = 0xF57F17,

        /// <summary>
        /// Defines the Amber50
        /// </summary>
        Amber50 = 0xFFF8E1,

        /// <summary>
        /// Defines the Amber100
        /// </summary>
        Amber100 = 0xFFECB3,

        /// <summary>
        /// Defines the Amber200
        /// </summary>
        Amber200 = 0xFFE082,

        /// <summary>
        /// Defines the Amber300
        /// </summary>
        Amber300 = 0xFFD54F,

        /// <summary>
        /// Defines the Amber400
        /// </summary>
        Amber400 = 0xFFCA28,

        /// <summary>
        /// Defines the Amber500
        /// </summary>
        Amber500 = 0xFFC107,

        /// <summary>
        /// Defines the Amber600
        /// </summary>
        Amber600 = 0xFFB300,

        /// <summary>
        /// Defines the Amber700
        /// </summary>
        Amber700 = 0xFFA000,

        /// <summary>
        /// Defines the Amber800
        /// </summary>
        Amber800 = 0xFF8F00,

        /// <summary>
        /// Defines the Amber900
        /// </summary>
        Amber900 = 0xFF6F00,

        /// <summary>
        /// Defines the Orange50
        /// </summary>
        Orange50 = 0xFFF3E0,

        /// <summary>
        /// Defines the Orange100
        /// </summary>
        Orange100 = 0xFFE0B2,

        /// <summary>
        /// Defines the Orange200
        /// </summary>
        Orange200 = 0xFFCC80,

        /// <summary>
        /// Defines the Orange300
        /// </summary>
        Orange300 = 0xFFB74D,

        /// <summary>
        /// Defines the Orange400
        /// </summary>
        Orange400 = 0xFFA726,

        /// <summary>
        /// Defines the Orange500
        /// </summary>
        Orange500 = 0xFF9800,

        /// <summary>
        /// Defines the Orange600
        /// </summary>
        Orange600 = 0xFB8C00,

        /// <summary>
        /// Defines the Orange700
        /// </summary>
        Orange700 = 0xF57C00,

        /// <summary>
        /// Defines the Orange800
        /// </summary>
        Orange800 = 0xEF6C00,

        /// <summary>
        /// Defines the Orange900
        /// </summary>
        Orange900 = 0xE65100,

        /// <summary>
        /// Defines the DeepOrange50
        /// </summary>
        DeepOrange50 = 0xFBE9E7,

        /// <summary>
        /// Defines the DeepOrange100
        /// </summary>
        DeepOrange100 = 0xFFCCBC,

        /// <summary>
        /// Defines the DeepOrange200
        /// </summary>
        DeepOrange200 = 0xFFAB91,

        /// <summary>
        /// Defines the DeepOrange300
        /// </summary>
        DeepOrange300 = 0xFF8A65,

        /// <summary>
        /// Defines the DeepOrange400
        /// </summary>
        DeepOrange400 = 0xFF7043,

        /// <summary>
        /// Defines the DeepOrange500
        /// </summary>
        DeepOrange500 = 0xFF5722,

        /// <summary>
        /// Defines the DeepOrange600
        /// </summary>
        DeepOrange600 = 0xF4511E,

        /// <summary>
        /// Defines the DeepOrange700
        /// </summary>
        DeepOrange700 = 0xE64A19,

        /// <summary>
        /// Defines the DeepOrange800
        /// </summary>
        DeepOrange800 = 0xD84315,

        /// <summary>
        /// Defines the DeepOrange900
        /// </summary>
        DeepOrange900 = 0xBF360C,

        /// <summary>
        /// Defines the Brown50
        /// </summary>
        Brown50 = 0xEFEBE9,

        /// <summary>
        /// Defines the Brown100
        /// </summary>
        Brown100 = 0xD7CCC8,

        /// <summary>
        /// Defines the Brown200
        /// </summary>
        Brown200 = 0xBCAAA4,

        /// <summary>
        /// Defines the Brown300
        /// </summary>
        Brown300 = 0xA1887F,

        /// <summary>
        /// Defines the Brown400
        /// </summary>
        Brown400 = 0x8D6E63,

        /// <summary>
        /// Defines the Brown500
        /// </summary>
        Brown500 = 0x795548,

        /// <summary>
        /// Defines the Brown600
        /// </summary>
        Brown600 = 0x6D4C41,

        /// <summary>
        /// Defines the Brown700
        /// </summary>
        Brown700 = 0x5D4037,

        /// <summary>
        /// Defines the Brown800
        /// </summary>
        Brown800 = 0x4E342E,

        /// <summary>
        /// Defines the Brown900
        /// </summary>
        Brown900 = 0x3E2723,

        /// <summary>
        /// Defines the Grey50
        /// </summary>
        Grey50 = 0xFAFAFA,

        /// <summary>
        /// Defines the Grey100
        /// </summary>
        Grey100 = 0xF5F5F5,

        /// <summary>
        /// Defines the Grey200
        /// </summary>
        Grey200 = 0xEEEEEE,

        /// <summary>
        /// Defines the Grey300
        /// </summary>
        Grey300 = 0xE0E0E0,

        /// <summary>
        /// Defines the Grey400
        /// </summary>
        Grey400 = 0xBDBDBD,

        /// <summary>
        /// Defines the Grey500
        /// </summary>
        Grey500 = 0x9E9E9E,

        /// <summary>
        /// Defines the Grey600
        /// </summary>
        Grey600 = 0x757575,

        /// <summary>
        /// Defines the Grey700
        /// </summary>
        Grey700 = 0x616161,

        /// <summary>
        /// Defines the Grey800
        /// </summary>
        Grey800 = 0x424242,

        /// <summary>
        /// Defines the Grey900
        /// </summary>
        Grey900 = 0x212121,

        /// <summary>
        /// Defines the BlueGrey50
        /// </summary>
        BlueGrey50 = 0xECEFF1,

        /// <summary>
        /// Defines the BlueGrey100
        /// </summary>
        BlueGrey100 = 0xCFD8DC,

        /// <summary>
        /// Defines the BlueGrey200
        /// </summary>
        BlueGrey200 = 0xB0BEC5,

        /// <summary>
        /// Defines the BlueGrey300
        /// </summary>
        BlueGrey300 = 0x90A4AE,

        /// <summary>
        /// Defines the BlueGrey400
        /// </summary>
        BlueGrey400 = 0x78909C,

        /// <summary>
        /// Defines the BlueGrey500
        /// </summary>
        BlueGrey500 = 0x607D8B,

        /// <summary>
        /// Defines the BlueGrey600
        /// </summary>
        BlueGrey600 = 0x546E7A,

        /// <summary>
        /// Defines the BlueGrey700
        /// </summary>
        BlueGrey700 = 0x455A64,

        /// <summary>
        /// Defines the BlueGrey800
        /// </summary>
        BlueGrey800 = 0x37474F,

        /// <summary>
        /// Defines the BlueGrey900
        /// </summary>
        BlueGrey900 = 0x263238
    }

    /// <summary>
    /// Defines the Accent
    /// </summary>
    public enum Accent
    {
        /// <summary>
        /// Defines the Red100
        /// </summary>
        Red100 = 0xFF8A80,

        /// <summary>
        /// Defines the Red200
        /// </summary>
        Red200 = 0xFF5252,

        /// <summary>
        /// Defines the Red400
        /// </summary>
        Red400 = 0xFF1744,

        /// <summary>
        /// Defines the Red700
        /// </summary>
        Red700 = 0xD50000,

        /// <summary>
        /// Defines the Pink100
        /// </summary>
        Pink100 = 0xFF80AB,

        /// <summary>
        /// Defines the Pink200
        /// </summary>
        Pink200 = 0xFF4081,

        /// <summary>
        /// Defines the Pink400
        /// </summary>
        Pink400 = 0xF50057,

        /// <summary>
        /// Defines the Pink700
        /// </summary>
        Pink700 = 0xC51162,

        /// <summary>
        /// Defines the Purple100
        /// </summary>
        Purple100 = 0xEA80FC,

        /// <summary>
        /// Defines the Purple200
        /// </summary>
        Purple200 = 0xE040FB,

        /// <summary>
        /// Defines the Purple400
        /// </summary>
        Purple400 = 0xD500F9,

        /// <summary>
        /// Defines the Purple700
        /// </summary>
        Purple700 = 0xAA00FF,

        /// <summary>
        /// Defines the DeepPurple100
        /// </summary>
        DeepPurple100 = 0xB388FF,

        /// <summary>
        /// Defines the DeepPurple200
        /// </summary>
        DeepPurple200 = 0x7C4DFF,

        /// <summary>
        /// Defines the DeepPurple400
        /// </summary>
        DeepPurple400 = 0x651FFF,

        /// <summary>
        /// Defines the DeepPurple700
        /// </summary>
        DeepPurple700 = 0x6200EA,

        /// <summary>
        /// Defines the Indigo100
        /// </summary>
        Indigo100 = 0x8C9EFF,

        /// <summary>
        /// Defines the Indigo200
        /// </summary>
        Indigo200 = 0x536DFE,

        /// <summary>
        /// Defines the Indigo400
        /// </summary>
        Indigo400 = 0x3D5AFE,

        /// <summary>
        /// Defines the Indigo700
        /// </summary>
        Indigo700 = 0x304FFE,

        /// <summary>
        /// Defines the Blue100
        /// </summary>
        Blue100 = 0x82B1FF,

        /// <summary>
        /// Defines the Blue200
        /// </summary>
        Blue200 = 0x448AFF,

        /// <summary>
        /// Defines the Blue400
        /// </summary>
        Blue400 = 0x2979FF,

        /// <summary>
        /// Defines the Blue700
        /// </summary>
        Blue700 = 0x2962FF,

        /// <summary>
        /// Defines the LightBlue100
        /// </summary>
        LightBlue100 = 0x80D8FF,

        /// <summary>
        /// Defines the LightBlue200
        /// </summary>
        LightBlue200 = 0x40C4FF,

        /// <summary>
        /// Defines the LightBlue400
        /// </summary>
        LightBlue400 = 0x00B0FF,

        /// <summary>
        /// Defines the LightBlue700
        /// </summary>
        LightBlue700 = 0x0091EA,

        /// <summary>
        /// Defines the Cyan100
        /// </summary>
        Cyan100 = 0x84FFFF,

        /// <summary>
        /// Defines the Cyan200
        /// </summary>
        Cyan200 = 0x18FFFF,

        /// <summary>
        /// Defines the Cyan400
        /// </summary>
        Cyan400 = 0x00E5FF,

        /// <summary>
        /// Defines the Cyan700
        /// </summary>
        Cyan700 = 0x00B8D4,

        /// <summary>
        /// Defines the Teal100
        /// </summary>
        Teal100 = 0xA7FFEB,

        /// <summary>
        /// Defines the Teal200
        /// </summary>
        Teal200 = 0x64FFDA,

        /// <summary>
        /// Defines the Teal400
        /// </summary>
        Teal400 = 0x1DE9B6,

        /// <summary>
        /// Defines the Teal700
        /// </summary>
        Teal700 = 0x00BFA5,

        /// <summary>
        /// Defines the Green100
        /// </summary>
        Green100 = 0xB9F6CA,

        /// <summary>
        /// Defines the Green200
        /// </summary>
        Green200 = 0x69F0AE,

        /// <summary>
        /// Defines the Green400
        /// </summary>
        Green400 = 0x00E676,

        /// <summary>
        /// Defines the Green700
        /// </summary>
        Green700 = 0x00C853,

        /// <summary>
        /// Defines the LightGreen100
        /// </summary>
        LightGreen100 = 0xCCFF90,

        /// <summary>
        /// Defines the LightGreen200
        /// </summary>
        LightGreen200 = 0xB2FF59,

        /// <summary>
        /// Defines the LightGreen400
        /// </summary>
        LightGreen400 = 0x76FF03,

        /// <summary>
        /// Defines the LightGreen700
        /// </summary>
        LightGreen700 = 0x64DD17,

        /// <summary>
        /// Defines the Lime100
        /// </summary>
        Lime100 = 0xF4FF81,

        /// <summary>
        /// Defines the Lime200
        /// </summary>
        Lime200 = 0xEEFF41,

        /// <summary>
        /// Defines the Lime400
        /// </summary>
        Lime400 = 0xC6FF00,

        /// <summary>
        /// Defines the Lime700
        /// </summary>
        Lime700 = 0xAEEA00,

        /// <summary>
        /// Defines the Yellow100
        /// </summary>
        Yellow100 = 0xFFFF8D,

        /// <summary>
        /// Defines the Yellow200
        /// </summary>
        Yellow200 = 0xFFFF00,

        /// <summary>
        /// Defines the Yellow400
        /// </summary>
        Yellow400 = 0xFFEA00,

        /// <summary>
        /// Defines the Yellow700
        /// </summary>
        Yellow700 = 0xFFD600,

        /// <summary>
        /// Defines the Amber100
        /// </summary>
        Amber100 = 0xFFE57F,

        /// <summary>
        /// Defines the Amber200
        /// </summary>
        Amber200 = 0xFFD740,

        /// <summary>
        /// Defines the Amber400
        /// </summary>
        Amber400 = 0xFFC400,

        /// <summary>
        /// Defines the Amber700
        /// </summary>
        Amber700 = 0xFFAB00,

        /// <summary>
        /// Defines the Orange100
        /// </summary>
        Orange100 = 0xFFD180,

        /// <summary>
        /// Defines the Orange200
        /// </summary>
        Orange200 = 0xFFAB40,

        /// <summary>
        /// Defines the Orange400
        /// </summary>
        Orange400 = 0xFF9100,

        /// <summary>
        /// Defines the Orange700
        /// </summary>
        Orange700 = 0xFF6D00,

        /// <summary>
        /// Defines the DeepOrange100
        /// </summary>
        DeepOrange100 = 0xFF9E80,

        /// <summary>
        /// Defines the DeepOrange200
        /// </summary>
        DeepOrange200 = 0xFF6E40,

        /// <summary>
        /// Defines the DeepOrange400
        /// </summary>
        DeepOrange400 = 0xFF3D00,

        /// <summary>
        /// Defines the DeepOrange700
        /// </summary>
        DeepOrange700 = 0xDD2C00
    }
}
