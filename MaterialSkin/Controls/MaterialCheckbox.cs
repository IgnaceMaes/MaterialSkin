using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Security.Principal;
using System.Windows.Forms;
using MaterialSkin.Animations;

namespace MaterialSkin.Controls
{
    public class MaterialCheckBox : CheckBox, IMaterialControl
    {
        public int Depth { get; set; }
        public MaterialSkinManager SkinManager { get { return MaterialSkinManager.Instance; } }
        public MouseState MouseState { get; set; }

        private readonly AnimationManager animationManager;

        public MaterialCheckBox()
        {
            animationManager = new AnimationManager()
            {
                AnimationType = AnimationType.EaseInOut,
                Increment = 0.05,
            };
            animationManager.OnAnimationProgress += sender => Invalidate();

            CheckedChanged += (sender, args) => animationManager.StartNewAnimation(Checked ? AnimationDirection.In : AnimationDirection.Out);
        }

        private static readonly Point[] CHECKMARK_LINE = {new Point(3, 8), new Point(6, 11), new Point(13, 4)};
        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            g.Clear(Parent.BackColor);

            double animationProgress = animationManager.GetProgress();

            int colorAlpha = Enabled ? (int) (animationProgress * 255.0) : SkinManager.GetCheckBoxOffDisabledColor().A;
            int backgroundAlpha = Enabled ? (int) (SkinManager.GetCheckboxOffColor().A * (1.0 - animationProgress)) : SkinManager.GetCheckBoxOffDisabledColor().A;
            
            var checkMarkLineFill = new Rectangle(0, 0, (int) (16.0 * animationProgress), 16);

            using (var checkmarkPath = DrawHelper.CreateRoundRect(0, 0, 16, 16, 2f))
            {
                g.FillPath(new SolidBrush(Color.FromArgb(backgroundAlpha, Enabled ? SkinManager.GetCheckboxOffColor() : SkinManager.GetCheckBoxOffDisabledColor())), checkmarkPath);
                g.FillRectangle(new SolidBrush(Parent.BackColor), 2, 2, 12, 12);
                
                using (var brush = new SolidBrush(Color.FromArgb(colorAlpha, Enabled ? SkinManager.AccentColor : SkinManager.GetCheckBoxOffDisabledColor())))
                {
                    if (Enabled)
                    {
                        g.FillPath(brush, checkmarkPath);
                    }
                    else if (Checked)
                    {
                        g.FillRectangle(brush, 2, 2, 12, 12);
                    }

                    g.DrawImageUnscaledAndClipped(DrawCheckMarkBitmap(), checkMarkLineFill);
                }  
            }

            g.DrawString(Text, SkinManager.ROBOTO_MEDIUM_10, Enabled ? SkinManager.GetMainTextBrush() : SkinManager.GetDisabledOrHintBrush(), 20, 0);
        }

        private Bitmap DrawCheckMarkBitmap()
        {
            Bitmap checkMark = new Bitmap(16, 16);
            Graphics g = Graphics.FromImage(checkMark);

            g.Clear(Color.Transparent);
            g.DrawLines(new Pen(Parent.BackColor, 2), CHECKMARK_LINE);

            return checkMark;    
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            Font = SkinManager.ROBOTO_MEDIUM_10;
        }
    }
}
