namespace MaterialSkin.Controls
{
    using MaterialSkin.Animations;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    public class MaterialSwitch : CheckBox, IMaterialControl
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

        private const int THUMB_SIZE = 22;

        private const int THUMB_SIZE_HALF = THUMB_SIZE / 2;

        private const int TRACK_SIZE_HEIGHT = (int)(14);
        private const int TRACK_SIZE_WIDTH = (int)(36);
        private const int TRACK_RADIUS = (int)(TRACK_SIZE_HEIGHT / 2);

        private int TRACK_CENTER_Y;
        private int TRACK_CENTER_X_BEGIN;
        private int TRACK_CENTER_X_END;
        private int TRACK_CENTER_X_DELTA;

        private const int RIPPLE_DIAMETER = 37;

        private bool justClicked;
        private int _trackOffsetY;

        public MaterialSwitch()
        {
            _animationManager = new AnimationManager
            {
                AnimationType = AnimationType.EaseInOut,
                Increment = 0.05
            };
            _rippleAnimationManager = new AnimationManager(false)
            {
                AnimationType = AnimationType.Linear,
                Increment = 0.05,
                SecondaryIncrement = 0.08
            };
            _animationManager.OnAnimationProgress += sender => Invalidate();
            _rippleAnimationManager.OnAnimationProgress += sender => Invalidate();

            LostFocus += (sender, args) => justClicked = false;

            CheckedChanged += (sender, args) =>
            {
                _animationManager.StartNewAnimation(Checked ? AnimationDirection.In : AnimationDirection.Out);
            };

            Ripple = true;
            MouseLocation = new Point(-1, -1);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            _trackOffsetY = Height / 2 - THUMB_SIZE_HALF;

            TRACK_CENTER_Y = _trackOffsetY + THUMB_SIZE_HALF - 1;
            TRACK_CENTER_X_BEGIN = TRACK_CENTER_Y;
            TRACK_CENTER_X_END = TRACK_CENTER_X_BEGIN + TRACK_SIZE_WIDTH - (TRACK_RADIUS * 2);
            TRACK_CENTER_X_DELTA = TRACK_CENTER_X_END - TRACK_CENTER_X_BEGIN;
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size strSize;
            using (NativeTextRenderer NativeText = new NativeTextRenderer(CreateGraphics()))
            {
                strSize = NativeText.MeasureLogString(Text, SkinManager.getLogFontByType(MaterialSkinManager.fontType.Body1));
            }
            var w = TRACK_SIZE_WIDTH + THUMB_SIZE + strSize.Width;
            return Ripple ? new Size(w, RIPPLE_DIAMETER) : new Size(w, THUMB_SIZE);
        }

        private static readonly Point[] CheckmarkLine = { new Point(3, 8), new Point(7, 12), new Point(14, 5) };

        private const int TEXT_OFFSET = THUMB_SIZE;

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            g.Clear(Parent.BackColor);

            var animationProgress = _animationManager.GetProgress();

            // Draw Track
            Color thumbColor = DrawHelper.BlendColor(
                        (Enabled ? SkinManager.GetSwitchOffThumbColor() : SkinManager.GetSwitchOffDisabledThumbColor()), // Off color
                        (Enabled ? SkinManager.ColorScheme.AccentColor : DrawHelper.BlendColor(SkinManager.ColorScheme.AccentColor, SkinManager.GetSwitchOffDisabledThumbColor(), 197)), // On color
                        animationProgress * 255); // Blend amount

            using (var path = DrawHelper.CreateRoundRect(new Rectangle(TRACK_CENTER_X_BEGIN - TRACK_RADIUS, TRACK_CENTER_Y - TRACK_SIZE_HEIGHT / 2, TRACK_SIZE_WIDTH, TRACK_SIZE_HEIGHT), TRACK_RADIUS))
            {
                using (SolidBrush trackBrush = new SolidBrush(
                    Color.FromArgb(Enabled ? SkinManager.GetSwitchOffTrackColor().A : SkinManager.GetSwitchOffDisabledTrackColor().A, // Track alpha
                    DrawHelper.BlendColor( // animate color
                        (Enabled ? SkinManager.GetSwitchOffTrackColor() : SkinManager.GetSwitchOffDisabledTrackColor()), // Off color
                        SkinManager.ColorScheme.AccentColor, // On color
                        animationProgress * 255) // Blend amount
                        .RemoveAlpha())))
                {
                    g.FillPath(trackBrush, path);
                }

            }

            // Calculate animation movement X position
            int OffsetX = (int)(TRACK_CENTER_X_DELTA * animationProgress);

            // Ripples
            int rippleSize = (Height % 2 == 0) ? Height - 2 : Height - 3;

            Color rippleColor = Color.FromArgb(MouseState == MouseState.DOWN ? 60 : 40, // color alpha
                Checked ? SkinManager.ColorScheme.AccentColor : // On color
                (SkinManager.Theme == MaterialSkinManager.Themes.LIGHT ? Color.Black : Color.White)); // Off color

            // draw ripple focus

            // draw ripple focus
            if (Ripple && !justClicked && !_rippleAnimationManager.IsAnimating() && (Focused || MouseState == MouseState.HOVER || MouseState == MouseState.DOWN))
            {
                using (SolidBrush rippleBrush = new SolidBrush(rippleColor))
                {
                    g.FillEllipse(rippleBrush, new Rectangle(TRACK_CENTER_X_BEGIN + OffsetX - rippleSize / 2, TRACK_CENTER_Y - rippleSize / 2, rippleSize, rippleSize));
                }
            }

            // draw ripple animation
            if (Ripple && _rippleAnimationManager.IsAnimating())
            {
                for (int i = 0; i < _rippleAnimationManager.GetAnimationCount(); i++)
                {
                    double rippleAnimProgress = _rippleAnimationManager.GetProgress(i);
                    int rippleAnimatedDiameter = (_rippleAnimationManager.GetDirection(i) == AnimationDirection.InOutIn) ? (int)(rippleSize * (0.8d + (0.2d * rippleAnimProgress))) : rippleSize;
                    rippleColor = Color.FromArgb((int)(40 * rippleAnimProgress), rippleColor.RemoveAlpha());


                    using (SolidBrush rippleBrush = new SolidBrush(rippleColor))
                    {
                        g.FillEllipse(rippleBrush, new Rectangle(TRACK_CENTER_X_BEGIN + OffsetX - rippleAnimatedDiameter / 2, TRACK_CENTER_Y - rippleAnimatedDiameter / 2, rippleAnimatedDiameter, rippleAnimatedDiameter));
                    }
                }
            }


            // draw Thumb Shadow
            RectangleF thumbBounds = new RectangleF(TRACK_CENTER_X_BEGIN + OffsetX - THUMB_SIZE_HALF, TRACK_CENTER_Y - THUMB_SIZE_HALF, THUMB_SIZE, THUMB_SIZE);
            using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(12, 0, 0, 0)))
            {
                g.FillEllipse(shadowBrush, new RectangleF(thumbBounds.X - 2, thumbBounds.Y - 1, thumbBounds.Width + 4, thumbBounds.Height + 6));
                g.FillEllipse(shadowBrush, new RectangleF(thumbBounds.X - 1, thumbBounds.Y - 1, thumbBounds.Width + 2, thumbBounds.Height + 4));
                g.FillEllipse(shadowBrush, new RectangleF(thumbBounds.X - 0, thumbBounds.Y - 0, thumbBounds.Width + 0, thumbBounds.Height + 2));
                g.FillEllipse(shadowBrush, new RectangleF(thumbBounds.X - 0, thumbBounds.Y + 2, thumbBounds.Width + 0, thumbBounds.Height + 0));
                g.FillEllipse(shadowBrush, new RectangleF(thumbBounds.X - 0, thumbBounds.Y + 1, thumbBounds.Width + 0, thumbBounds.Height + 0));
            }

            // draw Thumb
            using (SolidBrush thumbBrush = new SolidBrush(thumbColor))
            {
                g.FillEllipse(thumbBrush, thumbBounds);
            }

            // draw text
            using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
            {
                Rectangle textLocation = new Rectangle(TEXT_OFFSET + TRACK_SIZE_WIDTH, 0, Width - (TEXT_OFFSET + TRACK_SIZE_WIDTH), Height);
                NativeText.DrawTransparentText(
                    Text,
                    SkinManager.getLogFontByType(MaterialSkinManager.fontType.Body1),
                    Enabled ? SkinManager.GetPrimaryTextColor() : SkinManager.GetDisabledOrHintColor(),
                    textLocation.Location,
                    textLocation.Size,
                    NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Middle);
            }
        }

        private Bitmap DrawCheckMarkBitmap()
        {
            var checkMark = new Bitmap(THUMB_SIZE, THUMB_SIZE);
            var g = Graphics.FromImage(checkMark);

            // clear everything, transparent
            g.Clear(Color.Transparent);

            // draw the checkmark lines
            using (var pen = new Pen(Parent.BackColor, 2))
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

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            Font = SkinManager.getFontByType(MaterialSkinManager.fontType.Body1);

            if (DesignMode)
            {
                return;
            }

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

                if (Ripple && args.Button == MouseButtons.Left)
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
                Cursor = ClientRectangle.Contains(MouseLocation) ? Cursors.Hand : Cursors.Default;
            };
        }
    }
}
