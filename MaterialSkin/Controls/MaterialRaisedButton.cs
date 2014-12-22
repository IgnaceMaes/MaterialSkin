using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using MaterialSkin.Animations;

namespace MaterialSkin.Controls
{
    public class MaterialRaisedButton : Button, IMaterialControl
    {
        public int Depth { get; set; }
        public MaterialSkinManager SkinManager { get { return MaterialSkinManager.Instance; } }
        public MouseState MouseState { get; set; }
        public bool Primary { get; set; }

        private readonly AnimationManager animationManager;
        private Point animationSource;

        public MaterialRaisedButton()
        {
            Primary = true;
            
            animationManager = new AnimationManager()
            {
                Increment = 0.03,
                AnimationType = AnimationType.Linear,
                InterruptAnimation = true
            };
            animationManager.OnAnimationProgress += sender => Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);

            animationSource = mevent.Location;
            animationManager.StartNewAnimation(AnimationDirection.In);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            g.Clear(Parent.BackColor);

            using (var backgroundPath = DrawHelper.CreateRoundRect(ClientRectangle.X,
                ClientRectangle.Y,
                ClientRectangle.Width - 1,
                ClientRectangle.Height - 1,
                1f))
            {
                g.FillPath(Primary ? SkinManager.PrimaryColorBrush : SkinManager.GetRaisedButtonBackgroundBrush(), backgroundPath);
            }

            if (animationManager.IsAnimating())
            {
                var animationValue = animationManager.GetProgress();
                var rippleBrush = new SolidBrush(Color.FromArgb((int)(51 - (animationValue * 50)), Color.White));
                var rippleSize = (int) (animationValue * Width * 2);
                g.FillEllipse(rippleBrush, new Rectangle(animationSource.X - rippleSize / 2, animationSource.Y - rippleSize / 2, rippleSize, rippleSize));
            }
            
            g.DrawString(Text.ToUpper(), SkinManager.FONT_BUTTON, SkinManager.GetRaisedButtonTextBrush(Primary), ClientRectangle, new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        }
    }
}
