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
        public Point MouseLocation { get; set; }
        public bool Ripple { get; set; }

        private readonly AnimationManager animationManager;
        private readonly AnimationManager rippleAnimationManager;

        public MaterialCheckBox()
        {
            animationManager = new AnimationManager()
            {
                AnimationType = AnimationType.EaseInOut,
                Increment = 0.05
            };
            rippleAnimationManager = new AnimationManager(false)
            {
                AnimationType = AnimationType.Linear,
                Increment = 0.10,
                SecondaryIncrement = 0.08
            };
            animationManager.OnAnimationProgress += sender => Invalidate();
            rippleAnimationManager.OnAnimationProgress += sender => Invalidate();

            CheckedChanged += (sender, args) =>
            {
                animationManager.StartNewAnimation(Checked ? AnimationDirection.In : AnimationDirection.Out);
            };

            Ripple = false;
            MouseLocation = new Point(-1, -1);
        }

        private static readonly Point[] CHECKMARK_LINE = { new Point(3, 8), new Point(6, 11), new Point(13, 4) };
        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            g.Clear(Parent.BackColor);

            bool mouseHand = false;
            int boxOffset = Height / 2 - 8;

            double animationProgress = animationManager.GetProgress();

            int colorAlpha = Enabled ? (int)(animationProgress * 255.0) : SkinManager.GetCheckBoxOffDisabledColor().A;
            int backgroundAlpha = Enabled ? (int)(SkinManager.GetCheckboxOffColor().A * (1.0 - animationProgress)) : SkinManager.GetCheckBoxOffDisabledColor().A;

            var brush = new SolidBrush(Color.FromArgb(colorAlpha, Enabled ? SkinManager.AccentColor : SkinManager.GetCheckBoxOffDisabledColor()));

            if (Ripple && rippleAnimationManager.IsAnimating())
            {
                for (int i = 0; i < rippleAnimationManager.GetAnimationCount(); i++)
                {
                    var animationValue = rippleAnimationManager.GetProgress(i);
                    var animationSource = new Point(boxOffset + 8, boxOffset + 8);
                    var rippleBrush = new SolidBrush(Color.FromArgb((int)((animationValue * 40)), ((bool)rippleAnimationManager.GetData(i)[0]) ? Color.Black : brush.Color));
                    var rippleSize = (rippleAnimationManager.GetDirection(i) == AnimationDirection.InOutIn) ? (int)(Height * (0.8d + (0.2d * animationValue))) : Height;
                    g.FillEllipse(rippleBrush, new Rectangle(animationSource.X - rippleSize / 2, animationSource.Y - rippleSize / 2, rippleSize, rippleSize));
                }
            }

            var checkMarkLineFill = new Rectangle(boxOffset, boxOffset, (int)(16.0 * animationProgress), 16);

            using (var checkmarkPath = DrawHelper.CreateRoundRect(boxOffset, boxOffset, 16, 16, 2f))
            {
                if (checkmarkPath.IsVisible(MouseLocation))
                {
                    mouseHand = true;
                }

                g.FillPath(new SolidBrush(Color.FromArgb(backgroundAlpha, Enabled ? SkinManager.GetCheckboxOffColor() : SkinManager.GetCheckBoxOffDisabledColor())), checkmarkPath);
                g.FillRectangle(new SolidBrush(Parent.BackColor), boxOffset + 2, boxOffset + 2, 12, 12);

                if (Enabled)
                {
                    g.FillPath(brush, checkmarkPath);
                }
                else if (Checked)
                {
                    g.FillRectangle(brush, boxOffset + 2, boxOffset + 2, 12, 12);
                }

                g.DrawImageUnscaledAndClipped(DrawCheckMarkBitmap(), checkMarkLineFill);
            }

            SizeF stringSize = g.MeasureString(Text, SkinManager.ROBOTO_MEDIUM_10);
            g.DrawString(Text, SkinManager.ROBOTO_MEDIUM_10, Enabled ? SkinManager.GetMainTextBrush() : SkinManager.GetDisabledOrHintBrush(), boxOffset + 20, Height / 2 - stringSize.Height / 2);

            brush.Dispose();

            Cursor = (mouseHand) ? Cursors.Hand : Cursors.Default;
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

            if (DesignMode) return;

            MouseState = MouseState.OUT;
            MouseEnter += (sender, args) =>
            {
                MouseState = MouseState.HOVER;
                Invalidate();
            };
            MouseLeave += (sender, args) =>
            {
                MouseLocation = new Point(-1, -1);
                MouseState = MouseState.OUT;
                Invalidate();
            };
            MouseDown += (sender, args) =>
            {
                MouseState = MouseState.DOWN;

                if (Ripple && args.Button == MouseButtons.Left)
                {
                    int boxOffset = Height / 2 - 8;
                    using (var checkmarkPath = DrawHelper.CreateRoundRect(boxOffset, boxOffset, 16, 16, 2f))
                    {
                        if (checkmarkPath.IsVisible(MouseLocation))
                        {
                            rippleAnimationManager.SecondaryIncrement = 0;
                            rippleAnimationManager.StartNewAnimation(AnimationDirection.InOutIn, data: new object[] { Checked });
                        }
                    }
                }

                Invalidate();
            };
            MouseUp += (sender, args) =>
            {
                MouseState = MouseState.HOVER;

                rippleAnimationManager.SecondaryIncrement = 0.08;

                Invalidate();
            };
            MouseMove += (sender, args) =>
            {
                MouseLocation = args.Location;
                Invalidate();
            };
        }

    }
}
