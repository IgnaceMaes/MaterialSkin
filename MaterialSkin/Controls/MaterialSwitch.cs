namespace MaterialSkin.Controls
{
    using MaterialSkin.Animations;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    /// <summary>
    /// Defines the <see cref="MaterialSwitch" />
    /// </summary>
    public class MaterialSwitch : CheckBox, IMaterialControl
    {
        /// <summary>
        /// Gets or sets the Depth
        /// </summary>
        [Browsable(false)]
        public int Depth { get; set; }

        /// <summary>
        /// Gets the SkinManager
        /// </summary>
        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        /// <summary>
        /// Gets or sets the MouseState
        /// </summary>
        [Browsable(false)]
        public MouseState MouseState { get; set; }

        /// <summary>
        /// Gets or sets the MouseLocation
        /// </summary>
        [Browsable(false)]
        public Point MouseLocation { get; set; }

        /// <summary>
        /// Defines the _ripple
        /// </summary>
        private bool _ripple;

        /// <summary>
        /// Gets or sets a value indicating whether Ripple
        /// </summary>
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

        /// <summary>
        /// Defines the _animationManager
        /// </summary>
        private readonly AnimationManager _animationManager;

        /// <summary>
        /// Defines the _rippleAnimationManager
        /// </summary>
        private readonly AnimationManager _rippleAnimationManager;

        /// <summary>
        /// Defines the THUMB_SIZE
        /// </summary>
        private const int THUMB_SIZE = 22;

        /// <summary>
        /// Defines the THUMB_SIZE_HALF
        /// </summary>
        private const int THUMB_SIZE_HALF = THUMB_SIZE / 2;

        private const int TRACK_SIZE_HEIGHT = (int)(14);
        private const int TRACK_SIZE_WIDTH = (int)(36);
        private const int TRACK_RADIUS = (int)(TRACK_SIZE_HEIGHT / 2);

        private int TRACK_CENTER_Y;
        private int TRACK_CENTER_X_BEGIN;
        private int TRACK_CENTER_X_END;
        private int TRACK_CENTER_X_DELTA;

        private const int RIPPLE_DIAMETER = 30;

        /// <summary>
        /// Defines the _trackOffsetY
        /// </summary>
        private int _trackOffsetY;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialSwitch"/> class.
        /// </summary>
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

            CheckedChanged += (sender, args) =>
            {
                _animationManager.StartNewAnimation(Checked ? AnimationDirection.In : AnimationDirection.Out);
            };

            Ripple = true;
            MouseLocation = new Point(-1, -1);
        }

        /// <summary>
        /// The OnSizeChanged
        /// </summary>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            _trackOffsetY = Height / 2 - THUMB_SIZE_HALF;

            TRACK_CENTER_Y = _trackOffsetY + THUMB_SIZE_HALF - 1;
            TRACK_CENTER_X_BEGIN = TRACK_CENTER_Y;
            TRACK_CENTER_X_END = TRACK_CENTER_X_BEGIN + TRACK_SIZE_WIDTH - (TRACK_RADIUS * 2);
            TRACK_CENTER_X_DELTA = TRACK_CENTER_X_END - TRACK_CENTER_X_BEGIN;
        }

        /// <summary>
        /// The GetPreferredSize
        /// </summary>
        /// <param name="proposedSize">The proposedSize<see cref="Size"/></param>
        /// <returns>The <see cref="Size"/></returns>
        public override Size GetPreferredSize(Size proposedSize)
        {
            var w = _trackOffsetY + THUMB_SIZE + TRACK_SIZE_WIDTH + 2 + (int)CreateGraphics().MeasureString(Text, SkinManager.ROBOTO_MEDIUM_10).Width;
            return Ripple ? new Size(w, THUMB_SIZE + RIPPLE_DIAMETER / 2) : new Size(w, THUMB_SIZE);
        }

        /// <summary>
        /// Defines the CheckmarkLine
        /// </summary>
        private static readonly Point[] CheckmarkLine = { new Point(3, 8), new Point(7, 12), new Point(14, 5) };

        /// <summary>
        /// Defines the TEXT_OFFSET
        /// </summary>
        private const int TEXT_OFFSET = THUMB_SIZE;

        /// <summary>
        /// The OnPaint
        /// </summary>
        /// <param name="pevent">The pevent<see cref="PaintEventArgs"/></param>
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

            // draw ripple animation
            if (Ripple && _rippleAnimationManager.IsAnimating())
            {
                for (var i = 0; i < _rippleAnimationManager.GetAnimationCount(); i++)
                {
                    var rippleAnimProgress = _rippleAnimationManager.GetProgress(i);
                    var rippleDiameter = (Height % 2 == 0) ? Height - 2 : Height - 3;
                    var rippleAnimatedDiameter = (_rippleAnimationManager.GetDirection(i) == AnimationDirection.InOutIn) ? (int)(rippleDiameter * (0.8d + (0.2d * rippleAnimProgress))) : rippleDiameter;

                    Color rippleColor = Color.FromArgb((int)(40 * rippleAnimProgress), // color alpha
                        (bool)_rippleAnimationManager.GetData(i)[0] ? SkinManager.ColorScheme.AccentColor : // On color
                        (SkinManager.Theme == MaterialSkinManager.Themes.LIGHT ? Color.Black : Color.White)); // Off color

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

            // draw switch text
            SizeF stringSize = g.MeasureString(Text, SkinManager.ROBOTO_MEDIUM_10);
            g.DrawString(
                Text,
                SkinManager.ROBOTO_MEDIUM_10,
                Enabled ? SkinManager.GetPrimaryTextBrush() : SkinManager.GetDisabledOrHintBrush(),
                _trackOffsetY + TEXT_OFFSET + TRACK_SIZE_WIDTH, Height / 2 - stringSize.Height / 2);
        }

        /// <summary>
        /// The DrawCheckMarkBitmap
        /// </summary>
        /// <returns>The <see cref="Bitmap"/></returns>
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

        /// <summary>
        /// Gets or sets a value indicating whether AutoSize
        /// </summary>
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

        /// <summary>
        /// The OnCreateControl
        /// </summary>
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            Font = SkinManager.ROBOTO_MEDIUM_10;

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
            };
            MouseDown += (sender, args) =>
            {
                MouseState = MouseState.DOWN;

                if (Ripple && args.Button == MouseButtons.Left)
                {
                    _rippleAnimationManager.SecondaryIncrement = 0;
                    _rippleAnimationManager.StartNewAnimation(AnimationDirection.InOutIn, new object[] { Checked });
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
                Cursor = ClientRectangle.Contains(MouseLocation) ? Cursors.Hand : Cursors.Default;
            };
        }
    }
}
