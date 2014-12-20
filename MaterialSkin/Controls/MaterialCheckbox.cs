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
    public class MaterialCheckbox : CheckBox, IMaterialControl
    {
        public int Depth { get; set; }
        public MaterialSkinManager SkinManager { get { return MaterialSkinManager.Instance; } }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            if (DesignMode) { pevent.Graphics.Clear(Color.Black); return; }

            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            g.Clear(Parent.BackColor);

            using (var checkmarkPath = DrawHelper.CreateRoundRect(0, 0, 16, 16, 2f))
            {
                if (Checked)
                {
                    g.FillPath(Enabled ? SkinManager.AccentColorBrush : SkinManager.GetCheckBoxOffDisabledBrush(), checkmarkPath);
                    g.DrawLines(new Pen(Parent.BackColor, 2), new[] { new Point(3, 8), new Point(6, 11), new Point(13, 4) });
                }
                else
                {
                    g.FillPath(Enabled ? SkinManager.GetCheckboxOffBrush() : SkinManager.GetCheckBoxOffDisabledBrush(), checkmarkPath);
                    g.FillRectangle(new SolidBrush(Parent.BackColor), 2, 2, 12, 12);
                }          
            }

            g.DrawString(Text, SkinManager.FONT_BUTTON, Enabled ? SkinManager.GetMainTextBrush() : SkinManager.GetDisabledOrHintBrush(), 20, 0);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (!DesignMode) Font = SkinManager.FONT_BUTTON;
        }
    }
}
