using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using MaterialSkin.Animations;

namespace MaterialSkin.Controls
{
    public class MaterialRadioButton : RadioButton, IMaterialControl
    {
        public int Depth { get; set; }
        public MaterialSkinManager SkinManager { get { return MaterialSkinManager.Instance; } }
        public MouseState MouseState { get; set; }
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

        public MaterialRadioButton()
        {
            animationManager = new AnimationManager()
            {
                AnimationType = AnimationType.EaseInOut,
                Increment = 0.06,
            };
            rippleAnimationManager = new AnimationManager(false)
            {
                AnimationType = AnimationType.Linear,
                Increment = 0.10,
                SecondaryIncrement = 0.08
            };
            animationManager.OnAnimationProgress += sender => Invalidate();
            rippleAnimationManager.OnAnimationProgress += sender => Invalidate();

            CheckedChanged += (sender, args) => animationManager.StartNewAnimation(Checked ? AnimationDirection.In : AnimationDirection.Out);

            Ripple = false;
            MouseLocation = new Point(-1, -1);
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            int boxOffset = Height / 2 - 9;
            int w = boxOffset + 20 + (int)CreateGraphics().MeasureString(Text, SkinManager.ROBOTO_MEDIUM_10).Width;
            return Ripple ? new Size(w, 30) : new Size(w, 20);
        }

        private const int RADIOBUTTON_SIZE = 17;
        private const int RADIOBUTTON_OUTER_CIRCLE_WIDTH = 2;
        private const int RADIOBUTTON_INNER_CIRCLE_SIZE = RADIOBUTTON_SIZE - (2 * RADIOBUTTON_OUTER_CIRCLE_WIDTH);
        private const int RADIOBUTTON_X = 0;
        private const int RADIOBUTTON_Y = 0;
        private const int RADIOBUTTON_CENTER_X = RADIOBUTTON_X + RADIOBUTTON_SIZE / 2;
        private const int RADIOBUTTON_CENTER_Y = RADIOBUTTON_Y + RADIOBUTTON_SIZE / 2;

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            g.Clear(Parent.BackColor);

            bool mouseHand = false;
            int boxOffset = Height / 2 - (int)Math.Ceiling(RADIOBUTTON_SIZE / 2d);

            var animationProgress = animationManager.GetProgress();

            int colorAlpha = Enabled ? (int)(animationProgress * 255.0) : SkinManager.GetCheckBoxOffDisabledColor().A;
            int backgroundAlpha = Enabled ? (int)(SkinManager.GetCheckboxOffColor().A * (1.0 - animationProgress)) : SkinManager.GetCheckBoxOffDisabledColor().A;
            float animationSize = (float)(animationProgress * 8f);
            float animationSizeHalf = animationSize / 2;
            animationSize = (float)(animationProgress*9f);

            var transition = new RectangleF(RADIOBUTTON_CENTER_X - animationSizeHalf + boxOffset, RADIOBUTTON_CENTER_Y - animationSizeHalf + boxOffset, animationSize, animationSize);

            var brush = new SolidBrush(Color.FromArgb(colorAlpha, Enabled ? SkinManager.AccentColor : SkinManager.GetCheckBoxOffDisabledColor()));

            if (Ripple && rippleAnimationManager.IsAnimating())
            {
                for (int i = 0; i < rippleAnimationManager.GetAnimationCount(); i++)
                {
                    var animationValue = rippleAnimationManager.GetProgress(i);
                    var animationSource = new Point(boxOffset + 8, boxOffset + 8);
                    var rippleBrush = new SolidBrush(Color.FromArgb((int)((animationValue * 40)), ((bool)rippleAnimationManager.GetData(i)[0]) ? Color.Black : brush.Color));
                    var rippleSize = (rippleAnimationManager.GetDirection(i) == AnimationDirection.InOutIn) ? (int)((Height - 3) * (0.8d + (0.2d * animationValue))) : Height - 3;
                    g.FillEllipse(rippleBrush, new Rectangle(animationSource.X - rippleSize / 2, animationSource.Y - rippleSize / 2, rippleSize, rippleSize));
                }
            }

            if (new Rectangle(RADIOBUTTON_X + boxOffset, RADIOBUTTON_Y + boxOffset, RADIOBUTTON_SIZE, RADIOBUTTON_SIZE).Contains(MouseLocation))
            {
                mouseHand = true;
            }

            g.FillEllipse(new SolidBrush(Color.FromArgb(backgroundAlpha, Enabled ? SkinManager.GetCheckboxOffColor() : SkinManager.GetCheckBoxOffDisabledColor())), RADIOBUTTON_X + boxOffset, RADIOBUTTON_Y + boxOffset, RADIOBUTTON_SIZE, RADIOBUTTON_SIZE);

            if (Enabled)
                g.FillEllipse(brush, RADIOBUTTON_X + boxOffset, RADIOBUTTON_Y + boxOffset, RADIOBUTTON_SIZE, RADIOBUTTON_SIZE);

            g.FillEllipse(new SolidBrush(Parent.BackColor), RADIOBUTTON_OUTER_CIRCLE_WIDTH + boxOffset, RADIOBUTTON_OUTER_CIRCLE_WIDTH + boxOffset, RADIOBUTTON_INNER_CIRCLE_SIZE, RADIOBUTTON_INNER_CIRCLE_SIZE);

            g.FillEllipse(brush, transition);

            SizeF stringSize = g.MeasureString(Text, SkinManager.ROBOTO_MEDIUM_10);
            g.DrawString(Text, SkinManager.ROBOTO_MEDIUM_10, Enabled ? SkinManager.GetMainTextBrush() : SkinManager.GetDisabledOrHintBrush(), boxOffset + 20, Height / 2 - stringSize.Height / 2);

            brush.Dispose();

            Cursor = (mouseHand) ? Cursors.Hand : Cursors.Default;
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
                    if (new Rectangle(RADIOBUTTON_X + boxOffset, RADIOBUTTON_Y + boxOffset, RADIOBUTTON_SIZE, RADIOBUTTON_SIZE).Contains(MouseLocation))
                    {
                        rippleAnimationManager.SecondaryIncrement = 0;
                        rippleAnimationManager.StartNewAnimation(AnimationDirection.InOutIn, data: new object[] { Checked });
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
