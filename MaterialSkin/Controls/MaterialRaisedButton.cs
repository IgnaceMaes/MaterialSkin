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
    public class MaterialRaisedButton : Button, IMaterialControl
    {
        public int Depth { get; set; }
        public MaterialSkinManager SkinManager { get { return MaterialSkinManager.Instance; } }

        private readonly Brush textBrush;

        public MaterialRaisedButton()
        {
            textBrush = new SolidBrush(SkinManager.MAIN_TEXT_WHITE);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            if (DesignMode) { pevent.Graphics.Clear(Color.Black); return; }
         
            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            
            g.Clear(Parent.BackColor);

            using (var backgroundPath = DrawHelper.CreateRoundRect(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width - 1, ClientRectangle.Height - 1, 2f))
            {
                g.FillPath(SkinManager.PrimaryColorBrush, backgroundPath);
            }

            g.DrawString(Text.ToUpper(), SkinManager.FONT_BUTTON, textBrush, ClientRectangle, new StringFormat() {Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center});
        }
    }
}
