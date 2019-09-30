namespace MaterialSkin.Controls
{
    using MaterialSkin.Animations;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    public class MaterialRadioButton : RadioButton, IMaterialControl
    {
        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        [Browsable(false)]
        public MouseState MouseState { get; set; }

        [Browsable(false)]
        public Point MouseLocation { get; set; }

        private bool ripple;

        [Category("Behavior")]
        public bool Ripple
        {
            get { return ripple; }
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

        // animation managers
        private readonly AnimationManager _animationManager;
        private readonly AnimationManager _rippleAnimationManager;

        // size related variables which should be recalculated onsizechanged
        private Rectangle _radioButtonBounds;
        private int _boxOffset;

        // size constants
        private const int HEIGHT_RIPPLE = 37;
        private const int HEIGHT_NO_RIPPLE = 20;
        private const int RADIOBUTTON_SIZE = 19;
        private const int RADIOBUTTON_SIZE_HALF = RADIOBUTTON_SIZE / 2;
        private const int RADIOBUTTON_OUTER_CIRCLE_WIDTH = 2;
        private const int RADIOBUTTON_INNER_CIRCLE_SIZE = RADIOBUTTON_SIZE - (2 * RADIOBUTTON_OUTER_CIRCLE_WIDTH);
        private const int TEXT_OFFSET = 22;
        private bool justClicked;

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

            LostFocus += (sender, args) => justClicked = false;

            TabStopChanged += (sender, e) => TabStop = true;

            CheckedChanged += (sender, args) => _animationManager.StartNewAnimation(Checked ? AnimationDirection.In : AnimationDirection.Out);

            SizeChanged += OnSizeChanged;

            Ripple = true;
            MouseLocation = new Point(-1, -1);
        }

        private void OnSizeChanged(object sender, EventArgs eventArgs)
        {
            _boxOffset = Height / 2 - (int)Math.Ceiling(RADIOBUTTON_SIZE / 2d);
            _radioButtonBounds = new Rectangle(_boxOffset, _boxOffset, RADIOBUTTON_SIZE, RADIOBUTTON_SIZE);
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size strSize;

            using (NativeTextRenderer NativeText = new NativeTextRenderer(CreateGraphics()))
            {
                strSize = NativeText.MeasureLogString(Text, SkinManager.getLogFontByType(MaterialSkinManager.fontType.Body1));
            }

            int w = _boxOffset + TEXT_OFFSET + strSize.Width;
            return Ripple ? new Size(w, HEIGHT_RIPPLE) : new Size(w, HEIGHT_NO_RIPPLE);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // clear the control
            g.Clear(Parent.BackColor);

            int RADIOBUTTON_CENTER = _boxOffset + RADIOBUTTON_SIZE_HALF;
            Point animationSource = new Point(RADIOBUTTON_CENTER, RADIOBUTTON_CENTER);


            double animationProgress = _animationManager.GetProgress();

            int colorAlpha = Enabled ? (int)(animationProgress * 255.0) : SkinManager.GetCheckBoxOffDisabledColor().A;
            int backgroundAlpha = Enabled ? (int)(SkinManager.GetCheckboxOffColor().A * (1.0 - animationProgress)) : SkinManager.GetCheckBoxOffDisabledColor().A;
            float animationSize = (float)(animationProgress * 8f);
            float animationSizeHalf = animationSize / 2;
            animationSize = (float)(animationProgress * 9f);
            int rippleHeight = (HEIGHT_RIPPLE % 2 == 0) ? HEIGHT_RIPPLE - 3 : HEIGHT_RIPPLE - 2;

            Color RadioColor = Color.FromArgb(colorAlpha, Enabled ? SkinManager.ColorScheme.AccentColor : SkinManager.GetCheckBoxOffDisabledColor());

            // draw ripple focus
            if (Ripple && !justClicked && !_rippleAnimationManager.IsAnimating() && (Focused || MouseState == MouseState.HOVER || MouseState == MouseState.DOWN))
            {
                int rippleSize = rippleHeight;
                using (SolidBrush rippleBrush = new SolidBrush(Color.FromArgb(MouseState == MouseState.DOWN ? 60 : 40, Checked ? RadioColor : Color.Black)))
                {
                    using (GraphicsPath path = DrawHelper.CreateRoundRect(animationSource.X - rippleSize / 2, animationSource.Y - rippleSize / 2, rippleSize, rippleSize, rippleSize / 2))
                    {
                        g.FillPath(rippleBrush, path);
                    }
                }
            }
            // draw ripple animation
            if (Ripple && _rippleAnimationManager.IsAnimating())
            {
                for (int i = 0; i < _rippleAnimationManager.GetAnimationCount(); i++)
                {
                    double animationValue = _rippleAnimationManager.GetProgress(i);
                    int rippleSize = (_rippleAnimationManager.GetDirection(i) == AnimationDirection.InOutIn) ? (int)(rippleHeight * (0.8d + (0.2d * animationValue))) : rippleHeight;

                    using (SolidBrush rippleBrush = new SolidBrush(Color.FromArgb((int)((animationValue * 40)), ((bool)_rippleAnimationManager.GetData(i)[0]) ? Color.Black : RadioColor)))
                    {
                        using (GraphicsPath path = DrawHelper.CreateRoundRect(animationSource.X - rippleSize / 2, animationSource.Y - rippleSize / 2, rippleSize, rippleSize, rippleSize / 2))
                        {
                            g.FillPath(rippleBrush, path);
                        }
                    }
                }
            }

            // draw radiobutton circle
            using (Pen pen = new Pen(DrawHelper.BlendColor(Parent.BackColor, Enabled ? SkinManager.GetCheckboxOffColor() : SkinManager.GetCheckBoxOffDisabledColor(), backgroundAlpha), 2))
            {
                g.DrawEllipse(pen, new Rectangle(_boxOffset, _boxOffset, RADIOBUTTON_SIZE, RADIOBUTTON_SIZE));
            }

            if (Enabled)
            {
                using (Pen pen = new Pen(RadioColor, 2))
                {
                    g.DrawEllipse(pen, new Rectangle(_boxOffset, _boxOffset, RADIOBUTTON_SIZE, RADIOBUTTON_SIZE));
                }
            }

            if (Checked)
            {
                using (SolidBrush brush = new SolidBrush(RadioColor))
                {
                    g.FillEllipse(brush, new RectangleF(RADIOBUTTON_CENTER - animationSizeHalf, RADIOBUTTON_CENTER - animationSizeHalf, animationSize, animationSize));
                }
            }

            // Text
            using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
            {
                Rectangle textLocation = new Rectangle(_boxOffset + TEXT_OFFSET, 0, Width, Height);
                NativeText.DrawTransparentText(Text, SkinManager.getLogFontByType(MaterialSkinManager.fontType.Body1),
                    Enabled ? SkinManager.GetPrimaryTextColor() : SkinManager.GetDisabledOrHintColor(),
                    textLocation.Location,
                    textLocation.Size,
                    NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Middle);
            }
        }

        private bool IsMouseInCheckArea()
        {
            return ClientRectangle.Contains(MouseLocation);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

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
                if (justClicked)
                {
                    justClicked = false;
                    Parent.Focus();
                }
            };
            MouseDown += (sender, args) =>
            {
                MouseState = MouseState.DOWN;

                if (Ripple && args.Button == MouseButtons.Left && IsMouseInCheckArea())
                {
                    _rippleAnimationManager.SecondaryIncrement = 0;
                    _rippleAnimationManager.StartNewAnimation(AnimationDirection.InOutIn, new object[] { !Checked });
                }

                justClicked = true;

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
    }
}
