using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace MaterialSkin.Controls
{
    public enum Style
    {
        Body,
        Small,
        GrayCounter,        
        Title,
        Title1,
    }

    public class MaterialLabel : Label, IMaterialControl
    {
        private Style _style;
        private FontStyle _fontweight;

        [Browsable(false)]
        public int Depth { get; set; }
        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;
        [Browsable(false)]
        public MouseState MouseState { get; set; }

        public Style Style
        {
            get => _style;
            set { _style = value; ApplyStyle();}
        }

        public FontStyle FontWeight
        {
            get => _fontweight;
            set { _fontweight = value; ApplyStyle();}
        }

        public Shades Shade { get; set; }


        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            ForeColor = SkinManager.GetPrimaryTextColor();
            
            ApplyStyle();

            BackColorChanged += (sender, args) => ForeColor = SkinManager.GetPrimaryTextColor();            
        }

        private void ApplyStyle()
        {
            switch (Style)
            {
                case Style.Body:
                    Font = FontWeight == FontStyle.Bold ? SkinManager.ROBOTO_MEDIUM_11 : SkinManager.ROBOTO_REGULAR_11;
                    break;
                case Style.Small:
                    Font = FontWeight == FontStyle.Bold ? SkinManager.ROBOTO_MEDIUM_8 : SkinManager.ROBOTO_REGULAR_8;
                    break;
                case Style.Title:
                    Font = SkinManager.ROBOTO_TITLE;
                    break;
                case Style.Title1:
                    Font = SkinManager.ROBOTO_TITLE1;
                    break;    
                case Style.GrayCounter:
                    Font = FontWeight == FontStyle.Bold
                        ? SkinManager.ROBOTO_BOLD_HUGE
                        : SkinManager.ROBOTO_REGULAR_HUGE;
                    break; 
                default:
                    throw new ArgumentOutOfRangeException();
            }

            ForeColor = Shade == Shades.None
                ? MaterialSkinManager.Instance.GetPrimaryTextColor()
                : MaterialSkinManager.GetMaterialColor(Shade);

        }

     
    }
}
