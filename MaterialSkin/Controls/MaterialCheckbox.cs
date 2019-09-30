namespace MaterialSkin.Controls
{
    using MaterialSkin.Animations;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    public class MaterialCheckbox : CheckBox, IMaterialControl
    {
        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        [Browsable(false)]
        public MouseState MouseState { get; set; }

        [Browsable(false)]
        public Point MouseLocation { get; set; }

        private bool _ripple;

        [Category("Appearance")]
        public bool Ripple
        {
            get { return _ripple; }
            set
            {
                _ripple = value;
                AutoSize = AutoSize; //Make AutoSize directly set the bounds.

                if (value)
                {
                    Margin = new Padding(0);
                }

                Invalidate();
            }
        }

        private readonly AnimationManager _animationManager;
        private readonly AnimationManager _rippleAnimationManager;
        private const int HEIGHT_RIPPLE = 37;
        private const int HEIGHT_NO_RIPPLE = 20;
        private const int TEXT_OFFSET = 22;
        private const int CHECKBOX_SIZE = 18;
        private const int CHECKBOX_SIZE_HALF = CHECKBOX_SIZE / 2;
        private const int CHECKBOX_INNER_CHECKBOX_SIZE = CHECKBOX_SIZE - 4;
        private bool justClicked;
        private int _boxOffset;
        private Rectangle _boxRectangle;

        public MaterialCheckbox()
        {
            _animationManager = new AnimationManager
            {
                AnimationType = AnimationType.EaseInOut,
                Increment = 0.05
            };
            _rippleAnimationManager = new AnimationManager(false)
            {
                AnimationType = AnimationType.Linear,
                Increment = 0.10,
                SecondaryIncrement = 0.08
            };
            _animationManager.OnAnimationProgress += sender => Invalidate();
            _rippleAnimationManager.OnAnimationProgress += sender => Invalidate();

            CheckedChanged += (sender, args) =>
            {
                _animationManager.StartNewAnimation(Checked ? AnimationDirection.In : AnimationDirection.Out);
            };

            LostFocus += (sender, args) => justClicked = false;

            Ripple = true;
            MouseLocation = new Point(-1, -1);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            _boxOffset = HEIGHT_RIPPLE / 2 - 9;
            _boxRectangle = new Rectangle(_boxOffset, _boxOffset, CHECKBOX_SIZE - 1, CHECKBOX_SIZE - 1);
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

        private static readonly Point[] CheckmarkLine = { new Point(3, 8), new Point(7, 12), new Point(14, 5) };


        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // clear the control
            g.Clear(Parent.BackColor);

            int CHECKBOX_CENTER = _boxOffset + CHECKBOX_SIZE_HALF - 1;
            Point animationSource = new Point(CHECKBOX_CENTER, CHECKBOX_CENTER);
            double animationProgress = _animationManager.GetProgress();

            int colorAlpha = Enabled ? (int)(animationProgress * 255.0) : SkinManager.GetCheckBoxOffDisabledColor().A;
            int backgroundAlpha = Enabled ? (int)(SkinManager.GetCheckboxOffColor().A * (1.0 - animationProgress)) : SkinManager.GetCheckBoxOffDisabledColor().A;
            int rippleHeight = (HEIGHT_RIPPLE % 2 == 0) ? HEIGHT_RIPPLE - 3 : HEIGHT_RIPPLE - 2;

            SolidBrush brush = new SolidBrush(Color.FromArgb(colorAlpha, Enabled ? SkinManager.ColorScheme.AccentColor : SkinManager.GetCheckBoxOffDisabledColor()));
            Pen pen = new Pen(brush.Color, 2);

            // draw ripple focus
            if (Ripple && !justClicked && !_rippleAnimationManager.IsAnimating() && (Focused || MouseState == MouseState.HOVER || MouseState == MouseState.DOWN))
            {
                int rippleSize = rippleHeight;
                using (SolidBrush rippleBrush = new SolidBrush(Color.FromArgb(MouseState == MouseState.DOWN ? 60 : 40, Checked ? brush.Color : Color.Black)))
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
                    using (SolidBrush rippleBrush = new SolidBrush(Color.FromArgb((int)((animationValue * 40)), ((bool)_rippleAnimationManager.GetData(i)[0]) ? brush.Color : Color.Black)))
                    {
                        using (GraphicsPath path = DrawHelper.CreateRoundRect(animationSource.X - rippleSize / 2, animationSource.Y - rippleSize / 2, rippleSize, rippleSize, rippleSize / 2))
                        {
                            g.FillPath(rippleBrush, path);
                        }
                    }
                }
            }

            Rectangle checkMarkLineFill = new Rectangle(_boxOffset, _boxOffset, (int)(CHECKBOX_SIZE * animationProgress), CHECKBOX_SIZE);
            using (GraphicsPath checkmarkPath = DrawHelper.CreateRoundRect(_boxOffset - 0.5f, _boxOffset - 0.5f, CHECKBOX_SIZE, CHECKBOX_SIZE, 1))
            {
                if (Enabled)
                {
                    using (Pen pen2 = new Pen(DrawHelper.BlendColor(Parent.BackColor, Enabled ? SkinManager.GetCheckboxOffColor() : SkinManager.GetCheckBoxOffDisabledColor(), backgroundAlpha), 2))
                    {
                        g.DrawPath(pen2, checkmarkPath);
                    }

                    g.DrawPath(pen, checkmarkPath);
                    g.FillPath(brush, checkmarkPath);
                }
                else
                {
                    if (Checked)
                        g.FillPath(brush, checkmarkPath);
                    else
                        g.DrawPath(pen, checkmarkPath);
                }

                g.DrawImageUnscaledAndClipped(DrawCheckMarkBitmap(), checkMarkLineFill);
            }

            // draw checkbox text
            using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
            {
                Rectangle textLocation = new Rectangle(_boxOffset + TEXT_OFFSET, 0, Width - (_boxOffset + TEXT_OFFSET), HEIGHT_RIPPLE);
                NativeText.DrawTransparentText(Text, SkinManager.getLogFontByType(MaterialSkinManager.fontType.Body1),
                    Enabled ? SkinManager.GetPrimaryTextColor() : SkinManager.GetDisabledOrHintColor(),
                    textLocation.Location,
                    textLocation.Size,
                    NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Middle);
            }

            // dispose used paint objects
            pen.Dispose();
            brush.Dispose();
        }

        private Bitmap DrawCheckMarkBitmap()
        {
            Bitmap checkMark = new Bitmap(CHECKBOX_SIZE, CHECKBOX_SIZE);
            Graphics g = Graphics.FromImage(checkMark);

            // clear everything, transparent
            g.Clear(Color.Transparent);

            // draw the checkmark lines
            using (Pen pen = new Pen(Parent.BackColor, 2))
            {
                g.DrawLines(pen, CheckmarkLine);
            }

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
                    _rippleAnimationManager.StartNewAnimation(AnimationDirection.InOutIn, new object[] { Checked });
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
