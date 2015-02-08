using System;
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
        public AnimationUsage AnimationUsage { get; set; }
        public Point MouseLocation { get; set; }

        private bool _ripple = false;
        public bool Ripple
        {
            get { return _ripple; }
            set
            {
                _ripple = value;
                AutoSize = AutoSize; //Make AutoSize directly set the bounds.
                Invalidate();
            }
        }

        private readonly AnimationManager animationManager;
        private readonly AnimationManager rippleAnimationManager;

        private const int CHECKBOX_SIZE = 18;
        private const int CHECKBOX_SIZE_HALF = CHECKBOX_SIZE / 2;
        private const int CHECKBOX_INNER_BOX_SIZE = CHECKBOX_SIZE - 4;

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

        public override Size GetPreferredSize(Size proposedSize)
        {
            int boxOffset = Height / 2 - CHECKBOX_SIZE_HALF;
            int w = boxOffset + CHECKBOX_SIZE + 2 + (int)CreateGraphics().MeasureString(Text, SkinManager.ROBOTO_MEDIUM_10).Width;
            return Ripple ? new Size(w, 30) : new Size(w, 20);
        }

        private static readonly Point[] CHECKMARK_LINE = { new Point(3, 8), new Point(7, 12), new Point(14, 5) };
        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            g.Clear(Parent.BackColor);

            int boxOffset = Height / 2 - CHECKBOX_SIZE_HALF;
            var CHECKBOX_CENTER = boxOffset + CHECKBOX_SIZE_HALF -1;


            double animationProgress = animationManager.GetProgress();

            int colorAlpha = Enabled ? (int)(animationProgress * 255.0) : SkinManager.GetCheckBoxOffDisabledColor().A;
            int backgroundAlpha = Enabled ? (int)(SkinManager.GetCheckboxOffColor().A * (1.0 - animationProgress)) : SkinManager.GetCheckBoxOffDisabledColor().A;

            var brush = new SolidBrush(Color.FromArgb(colorAlpha, Enabled ? SkinManager.ColorPair.AccentColor : SkinManager.GetCheckBoxOffDisabledColor()));
            var brush3 = new SolidBrush(Enabled ? SkinManager.ColorPair.AccentColor : SkinManager.GetCheckBoxOffDisabledColor());
            var pen = new Pen(brush.Color);

            if (Ripple && rippleAnimationManager.IsAnimating())
            {
                for (int i = 0; i < rippleAnimationManager.GetAnimationCount(); i++)
                {
                    var animationValue = rippleAnimationManager.GetProgress(i);
                    var animationSource = new Point(CHECKBOX_CENTER, CHECKBOX_CENTER);
                    var rippleBrush = new SolidBrush(Color.FromArgb((int)((animationValue * 40)), ((bool)rippleAnimationManager.GetData(i)[0]) ? Color.Black : brush.Color));
                    var rippleHeight = (Height % 2 == 0) ? Height - 3 : Height - 2;
                    var rippleSize = (rippleAnimationManager.GetDirection(i) == AnimationDirection.InOutIn) ? (int)(rippleHeight * (0.8d + (0.2d * animationValue))) : rippleHeight;
                    using (var path = DrawHelper.CreateRoundRect(animationSource.X - rippleSize / 2, animationSource.Y - rippleSize / 2, rippleSize, rippleSize, rippleSize / 2))
                    {
                        g.FillPath(rippleBrush, path);
                    }

                    rippleBrush.Dispose();
                }
            }

            brush3.Dispose();

            var checkMarkLineFill = new Rectangle(boxOffset, boxOffset, (int)(17.0 * animationProgress), 17);
            using (var checkmarkPath = DrawHelper.CreateRoundRect(boxOffset, boxOffset, 17, 17, 1f))
            {
                SolidBrush brush2 = new SolidBrush(DrawHelper.BlendColor(Parent.BackColor, Enabled ? SkinManager.GetCheckboxOffColor() : SkinManager.GetCheckBoxOffDisabledColor(), backgroundAlpha));
                Pen pen2 = new Pen(brush2.Color);
                g.FillPath(brush2, checkmarkPath);
                g.DrawPath(pen2, checkmarkPath);

                g.FillRectangle(new SolidBrush(Parent.BackColor), boxOffset + 2, boxOffset + 2, CHECKBOX_INNER_BOX_SIZE - 1, CHECKBOX_INNER_BOX_SIZE - 1);
                g.DrawRectangle(new Pen(Parent.BackColor), boxOffset + 2, boxOffset + 2, CHECKBOX_INNER_BOX_SIZE - 1, CHECKBOX_INNER_BOX_SIZE - 1);

                brush2.Dispose();
                pen2.Dispose();

                if (Enabled)
                {
                    g.FillPath(brush, checkmarkPath);
                    g.DrawPath(pen, checkmarkPath);
                }
                else if (Checked)
                {
                    g.SmoothingMode = SmoothingMode.None;
                    g.FillRectangle(brush, boxOffset + 2, boxOffset + 2, CHECKBOX_INNER_BOX_SIZE, CHECKBOX_INNER_BOX_SIZE);
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                }

                g.DrawImageUnscaledAndClipped(DrawCheckMarkBitmap(), checkMarkLineFill);
            }

            SizeF stringSize = g.MeasureString(Text, SkinManager.ROBOTO_MEDIUM_10);
            g.DrawString(Text, SkinManager.ROBOTO_MEDIUM_10, Enabled ? SkinManager.GetMainTextBrush() : SkinManager.GetDisabledOrHintBrush(), boxOffset + 20, Height / 2 - stringSize.Height / 2);

            pen.Dispose();
            brush.Dispose();
        }

        private Bitmap DrawCheckMarkBitmap()
        {
            Bitmap checkMark = new Bitmap(17, 17);
            Graphics g = Graphics.FromImage(checkMark);

            g.Clear(Color.Transparent);
            g.DrawLines(new Pen(Parent.BackColor, 2), CHECKMARK_LINE);

            return checkMark;
        }

        public override bool AutoSize
        {
            get { return base.AutoSize; }
            set
            {
                base.AutoSize = value;
                if (value)
                {
                    Size = new Size(10, 10);
                }
            }
        }

        private bool IsMouseInCheckArea()
        {
            int boxOffset = Height / 2 - 9;
            using (var checkmarkPath = DrawHelper.CreateRoundRect(boxOffset, boxOffset, 17, 17, 2f))
            {
                return checkmarkPath.IsVisible(MouseLocation);
            }
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
            };
            MouseLeave += (sender, args) =>
            {
                MouseLocation = new Point(-1, -1);
                MouseState = MouseState.OUT;
            };
            MouseDown += (sender, args) =>
            {
                MouseState = MouseState.DOWN;

                if (Ripple && args.Button == MouseButtons.Left && IsMouseInCheckArea())
                {
                    rippleAnimationManager.SecondaryIncrement = 0;
                    rippleAnimationManager.StartNewAnimation(AnimationDirection.InOutIn, data: new object[] { Checked });
                }
            };
            MouseUp += (sender, args) =>
            {
                MouseState = MouseState.HOVER;
                rippleAnimationManager.SecondaryIncrement = 0.08;
            };
            MouseMove += (sender, args) =>
            {
                MouseLocation = args.Location;
                Cursor = IsMouseInCheckArea() ? Cursors.Hand : Cursors.Default;
            };
        }

    }
}
