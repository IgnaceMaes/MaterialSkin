using System;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace MaterialSkin.Controls
{
    public enum Style
    {
        Body,
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


        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            ForeColor = SkinManager.GetPrimaryTextColor();
            switch (Style)
            {
                case Style.Body:
                    Font = SkinManager.ROBOTO_REGULAR_11;
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
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.TextRenderingHint = TextRenderingHint.SystemDefault; 
            base.OnPaint(e);
        }
    }
}
