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
    public class MaterialRadioButton : RadioButton, IMaterialControl
    {
        public int Depth { get; set; }
        public MaterialSkinManager SkinManager { get { return MaterialSkinManager.Instance; } }
        public MouseState MouseState { get; set; }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            if (DesignMode) { pevent.Graphics.Clear(Color.Black); return; }

            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            g.Clear(Parent.BackColor);

            if (Checked)
            {
                g.FillEllipse(Enabled ? SkinManager.AccentColorBrush : SkinManager.GetCheckBoxOffDisabledBrush(), 0, 0, 16, 16);
                g.DrawEllipse(new Pen(Parent.BackColor, 2), 3, 3, 10, 10);
            }
            else
            {
                g.FillEllipse(Enabled ? SkinManager.GetCheckboxOffBrush() : SkinManager.GetCheckBoxOffDisabledBrush(), 0, 0, 16, 16);
                g.FillEllipse(new SolidBrush(Parent.BackColor), 2, 2, 12, 12);
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
