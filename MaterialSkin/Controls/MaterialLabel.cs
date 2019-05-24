using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
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
        [Browsable(false)]
        public int Depth { get; set; }
        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;
        [Browsable(false)]
        public MouseState MouseState { get; set; }
        public Style Style { get; set; }
        public Shades Shade { get; set; }


        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            ForeColor = SkinManager.GetPrimaryTextColor();
            switch (Style)
            {
                case Style.Body:
                    Font = SkinManager.ROBOTO_REGULAR_11;
                    break;
                case Style.Small:
                    Font = SkinManager.ROBOTO_REGULAR_8;
                    break;
                case Style.Title:
                    Font = SkinManager.ROBOTO_TITLE;
                    break;
                case Style.Title1:
                    Font = SkinManager.ROBOTO_TITLE1;
                    break;              
                default:
                    throw new ArgumentOutOfRangeException();
            }

            BackColorChanged += (sender, args) => ForeColor = SkinManager.GetPrimaryTextColor();            
        }

        protected override void OnPaint(PaintEventArgs e)
        {            
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            e.Graphics.Clear(Parent.BackColor);

            Brush b = Shade == Shades.None
                ? MaterialSkinManager.Instance.GetPrimaryTextBrush()
                : MaterialSkinManager.GetMaterialBrush(Shade);

            
            e.Graphics.FillRectangle(new SolidBrush(BackColor), 0,0, Width, Height);

            e.Graphics.DrawString(this.Text, this.Font, b, e.ClipRectangle, 
                new StringFormat { 
                    Alignment = MaterialRaisedButton.ContentToTextHAlignment(TextAlign), 
                    LineAlignment = MaterialRaisedButton.ContentToTextVAlignment(TextAlign) });  
        }
    }
}
