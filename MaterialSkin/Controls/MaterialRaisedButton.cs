using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace MaterialSkin.Controls
{
    public class MaterialRaisedButton : Button, IMaterialControl
    {
        public int Depth { get; set; }
        public MaterialSkinManager SkinManager { get { return MaterialSkinManager.Instance; } }
        public MouseState MouseState { get; set; }
        public bool Primary { get; set; }

        private readonly Timer animationTimer = new Timer { Interval = 5, Enabled = false };
        private double animationValue;
        private Point animationSource;

        public MaterialRaisedButton()
        {
            Primary = true;
            animationTimer.Tick += animationTimer_Tick;
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
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
            if (DesignMode) { pevent.Graphics.Clear(Color.Black); return; }

            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            g.Clear(Parent.BackColor);

            using (var backgroundPath = DrawHelper.CreateRoundRect(ClientRectangle.X,
                ClientRectangle.Y,
                ClientRectangle.Width - 1,
                ClientRectangle.Height - 1,
                2f))
            {
                g.FillPath(Primary ? SkinManager.PrimaryColorBrush : SkinManager.GetRaisedButtonBackgroundBrush(), backgroundPath);
            }

            if (animationValue > 0)
            {
                var rippleBrush = new SolidBrush(Color.FromArgb((int)(51 - (animationValue * 50)), Color.White));
                var rippleSize = (int)(animationValue * Width * 2);
                g.FillEllipse(rippleBrush, new Rectangle(animationSource.X - rippleSize / 2, animationSource.Y - rippleSize / 2, rippleSize, rippleSize));
            }
            
            g.DrawString(Text.ToUpper(), SkinManager.FONT_BUTTON, SkinManager.GetRaisedButtonTextBrush(Primary), ClientRectangle, new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        }
    }
}
