using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace MaterialSkin.Controls
{
    public class MaterialFlatButton : Button, IMaterialControl
    {
        public int Depth { get; set; }
        public MaterialSkinManager SkinManager { get { return MaterialSkinManager.Instance; } }
        public MouseState MouseState { get; set; }
        public bool Primary { get; set; }

        private readonly Timer animationTimer = new Timer { Interval = 5, Enabled = false };
        private double animationValue;
        private Point animationSource;

        public MaterialFlatButton()
        {
            Primary = true;
            animationTimer.Tick += animationTimer_Tick;
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            animationValue = 0;
            animationSource = mevent.Location;
            animationTimer.Start();
        }

        void animationTimer_Tick(object sender, System.EventArgs e)
        {
            if (animationValue < 1.00)
            {
                animationValue += 0.03;
                Invalidate();
            }
            else
            {
                animationValue = 0;
                animationTimer.Stop();
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            g.Clear(Parent.BackColor);
            if (MouseState != MouseState.OUT)
            {
                g.FillRectangle(SkinManager.GetFlatButtonHoverBackgroundBrush(), ClientRectangle);
            }

            if (animationValue > 0)
            {
                var rippleBrush = new SolidBrush(Color.FromArgb((int)(51 - (animationValue * 50)), Color.Black));
                var rippleSize = (int)(animationValue * Width * 2);
                g.FillEllipse(rippleBrush, new Rectangle(animationSource.X - rippleSize / 2, animationSource.Y - rippleSize / 2, rippleSize, rippleSize));
            }

            g.DrawString(Text.ToUpper(), SkinManager.FONT_BUTTON, Enabled ? (Primary ? SkinManager.PrimaryColorBrush : SkinManager.GetMainTextBrush()) : SkinManager.GetDisabledOrHintBrush(), ClientRectangle, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (DesignMode) return;

            MouseState = MouseState.OUT;
            MouseEnter += (sender, args) =>
            {
                MouseState = MouseState.HOVER;
                Invalidate();
            };
            MouseLeave += (sender, args) => 
            { 
                MouseState = MouseState.OUT;
                Invalidate();
            };
            MouseDown += (sender, args) =>
            {
                MouseState = MouseState.DOWN;
                Invalidate();
            };
            MouseUp += (sender, args) =>
            {
                MouseState = MouseState.HOVER;
                Invalidate();
            };
        }
    }
}
