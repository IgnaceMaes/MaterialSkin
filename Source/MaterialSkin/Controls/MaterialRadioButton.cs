using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using MaterialSkin.Animations;

namespace MaterialSkin.Controls
{
    public class MaterialRadioButton : RadioButton, IMaterialControl
    {
        // size constants
        private const int RADIOBUTTON_SIZE = 19;

        private const int RADIOBUTTON_SIZE_HALF = RADIOBUTTON_SIZE / 2;
        private const int RADIOBUTTON_OUTER_CIRCLE_WIDTH = 2;
        private const int RADIOBUTTON_INNER_CIRCLE_SIZE = RADIOBUTTON_SIZE - 2 * RADIOBUTTON_OUTER_CIRCLE_WIDTH;

        // animation managers
        private readonly AnimationManager _animationManager;

        private readonly AnimationManager _rippleAnimationManager;
        private int _boxOffset;

        // size related variables which should be recalculated onsizechanged
        private Rectangle _radioButtonBounds;

        private bool ripple;

        [Browsable(false)]
        public Point MouseLocation { get; set; }

        [Category("Behavior")]
        public bool Ripple
        {
            get => ripple;
            set
            {
                ripple = value;
                AutoSize = AutoSize; //Make AutoSize directly set the bounds.

                if (value)
                {
                    Margin = new Padding(0);
                }

                Invalidate();
            }
        }

        public MaterialRadioButton()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, true);

            _animationManager = new AnimationManager
            {
                AnimationType = AnimationType.EaseInOut,
                Increment = 0.06
            };
            _rippleAnimationManager = new AnimationManager(false)
            {
                AnimationType = AnimationType.Linear,
                Increment = 0.10,
                SecondaryIncrement = 0.08
            };
            _animationManager.OnAnimationProgress += sender => Invalidate();
            _rippleAnimationManager.OnAnimationProgress += sender => Invalidate();

            CheckedChanged += (sender, args) => _animationManager.StartNewAnimation(Checked ? AnimationDirection.In : AnimationDirection.Out);

            SizeChanged += OnSizeChanged;

            Ripple = true;
            MouseLocation = new Point(-1, -1);
        }

        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        [Browsable(false)]
        public MouseState MouseState { get; set; }

        public override Size GetPreferredSize(Size proposedSize)
        {
            int width = _boxOffset + 20 + (int) CreateGraphics().MeasureString(Text, SkinManager.ROBOTO_MEDIUM_10).Width;
            return Ripple ? new Size(width, 30) : new Size(width, 20);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            Font = SkinManager.ROBOTO_MEDIUM_10;

            if (DesignMode)
            {
                return;
            }

            MouseState = MouseState.OUT;
            MouseEnter += (sender, args) => { MouseState = MouseState.HOVER; };
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
                    _rippleAnimationManager.SecondaryIncrement = 0;
                    _rippleAnimationManager.StartNewAnimation(AnimationDirection.InOutIn, new object[]
                    {
                        Checked
                    });
                }
            };
            MouseUp += (sender, args) =>
            {
                MouseState = MouseState.HOVER;
                _rippleAnimationManager.SecondaryIncrement = 0.08;
            };
            MouseMove += (sender, args) =>
            {
                MouseLocation = args.Location;
                Cursor = IsMouseInCheckArea() ? Cursors.Hand : Cursors.Default;
            };
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            // clear the control
            g.Clear(Parent.BackColor);

            int RADIOBUTTON_CENTER = _boxOffset + RADIOBUTTON_SIZE_HALF;

            double animationProgress = _animationManager.GetProgress();

            int colorAlpha = Enabled ? (int) (animationProgress * 255.0) : SkinManager.GetCheckBoxOffDisabledColor().A;
            int backgroundAlpha = Enabled ? (int) (SkinManager.GetCheckboxOffColor().A * (1.0 - animationProgress)) : SkinManager.GetCheckBoxOffDisabledColor().A;
            float animationSize = (float) (animationProgress * 8f);
            float animationSizeHalf = animationSize / 2;
            animationSize = (float) (animationProgress * 9f);

            SolidBrush brush = new SolidBrush(Color.FromArgb(colorAlpha, Enabled ? SkinManager.ColorScheme.AccentColor : SkinManager.GetCheckBoxOffDisabledColor()));
            Pen pen = new Pen(brush.Color);

            // draw ripple animation
            if (Ripple && _rippleAnimationManager.IsAnimating())
            {
                for (int i = 0; i < _rippleAnimationManager.GetAnimationCount(); i++)
                {
                    double animationValue = _rippleAnimationManager.GetProgress(i);
                    Point animationSource = new Point(RADIOBUTTON_CENTER, RADIOBUTTON_CENTER);
                    SolidBrush rippleBrush = new SolidBrush(Color.FromArgb((int) (animationValue * 40), (bool) _rippleAnimationManager.GetData(i)[0] ? Color.Black : brush.Color));
                    int rippleHeight = Height % 2 == 0 ? Height - 3 : Height - 2;
                    int rippleSize = _rippleAnimationManager.GetDirection(i) == AnimationDirection.InOutIn ? (int) (rippleHeight * (0.8d + 0.2d * animationValue)) : rippleHeight;
                    using (GraphicsPath path = DrawHelper.CreateRoundRect(animationSource.X - rippleSize / 2, animationSource.Y - rippleSize / 2, rippleSize, rippleSize, rippleSize / 2))
                    {
                        g.FillPath(rippleBrush, path);
                    }

                    rippleBrush.Dispose();
                }
            }

            // draw radiobutton circle
            Color uncheckedColor = DrawHelper.BlendColor(Parent.BackColor, Enabled ? SkinManager.GetCheckboxOffColor() : SkinManager.GetCheckBoxOffDisabledColor(), backgroundAlpha);

            using (GraphicsPath path = DrawHelper.CreateRoundRect(_boxOffset, _boxOffset, RADIOBUTTON_SIZE, RADIOBUTTON_SIZE, 9f))
            {
                g.FillPath(new SolidBrush(uncheckedColor), path);

                if (Enabled)
                {
                    g.FillPath(brush, path);
                }
            }

            g.FillEllipse(new SolidBrush(Parent.BackColor), RADIOBUTTON_OUTER_CIRCLE_WIDTH + _boxOffset, RADIOBUTTON_OUTER_CIRCLE_WIDTH + _boxOffset, RADIOBUTTON_INNER_CIRCLE_SIZE, RADIOBUTTON_INNER_CIRCLE_SIZE);

            if (Checked)
            {
                using (GraphicsPath path = DrawHelper.CreateRoundRect(RADIOBUTTON_CENTER - animationSizeHalf, RADIOBUTTON_CENTER - animationSizeHalf, animationSize, animationSize, 4f))
                {
                    g.FillPath(brush, path);
                }
            }
            SizeF stringSize = g.MeasureString(Text, SkinManager.ROBOTO_MEDIUM_10);
            g.DrawString(Text, SkinManager.ROBOTO_MEDIUM_10, Enabled ? SkinManager.GetPrimaryTextBrush() : SkinManager.GetDisabledOrHintBrush(), _boxOffset + 22, Height / 2 - stringSize.Height / 2);

            brush.Dispose();
            pen.Dispose();
        }

        private bool IsMouseInCheckArea()
        {
            return _radioButtonBounds.Contains(MouseLocation);
        }

        private void OnSizeChanged(object sender, EventArgs eventArgs)
        {
            _boxOffset = Height / 2 - (int) Math.Ceiling(RADIOBUTTON_SIZE / 2d);
            _radioButtonBounds = new Rectangle(_boxOffset, _boxOffset, RADIOBUTTON_SIZE, RADIOBUTTON_SIZE);
        }
    }
}
