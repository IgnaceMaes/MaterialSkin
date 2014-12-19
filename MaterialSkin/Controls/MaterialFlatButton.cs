using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaterialSkin.Controls
{
    public class MaterialFlatButton : Button, IMaterialControl
    {
        public int Depth { get; set; }
        public MaterialSkinManager SkinManager { get { return MaterialSkinManager.Instance; } }

        public bool Primary { get; set; }

        public MaterialFlatButton()
        {
            Primary = true;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            if (DesignMode) { pevent.Graphics.Clear(Color.Black); return; }

            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            g.Clear(Parent.BackColor);

            g.DrawString(Text.ToUpper(), SkinManager.FONT_BUTTON, Primary ? SkinManager.PrimaryColorBrush : SkinManager.GetMainTextBrush(), ClientRectangle, new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        }
    }
}
