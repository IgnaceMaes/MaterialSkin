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
    /// Defines the <see cref="MaterialFloatingActionButton" />
    /// </summary>
    public class MaterialFloatingActionButton : Button, IMaterialControl
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
        public MaterialSkinManager SkinManager=> MaterialSkinManager.Instance;

        /// <summary>
        /// Gets or sets the MouseState
        /// </summary>
        [Browsable(false)]
        public MouseState MouseState { get; set; }

        /// <summary>
        /// Defines the FAB_SIZE
        /// </summary>
        private const int FAB_SIZE = 56;

        /// <summary>
        /// Defines the FAB_MINI_SIZE
        /// </summary>
        private const int FAB_MINI_SIZE = 40;

        /// <summary>
        /// Defines the FAB_ICON_MARGIN
        /// </summary>
        private const int FAB_ICON_MARGIN = 16;

        /// <summary>
        /// Defines the FAB_MINI_ICON_MARGIN
        /// </summary>
        private const int FAB_MINI_ICON_MARGIN = 8;

        /// <summary>
        /// Defines the FAB_ICON_SIZE
        /// </summary>
        private const int FAB_ICON_SIZE = 24;

        /// <summary>
        /// Gets or sets a value indicating whether Mini
        /// </summary>
        public bool Mini
        {
            get { return _mini; }
            set { setSize(value); }
        }

        /// <summary>
        /// Defines the _mini
        /// </summary>
        private bool _mini = false;

        /// <summary>
        /// Gets or sets a value indicating whether AnimateShowHideButton
        /// </summary>
        public bool AnimateShowHideButton
        {
            get { return _animateShowButton; }
            set { _animateShowButton = value; }
        }

        /// <summary>
        /// Defines the _animateShowButton
        /// </summary>
        private bool _animateShowButton;

        /// <summary>
        /// Gets or sets the Icon
        /// </summary>
        public Image Icon
        {
            get { return _icon; }
            set { _icon = value; }
        }

        /// <summary>
        /// Defines the _icon
        /// </summary>
        private Image _icon;

        /// <summary>
        /// Defines the _isHiding
        /// </summary>
        private bool _isHiding = false;

        /// <summary>
        /// Defines the _animationManager
        /// </summary>
        private readonly AnimationManager _animationManager;

        /// <summary>
        /// Defines the _showAnimationManager
        /// </summary>
        private readonly AnimationManager _showAnimationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialFloatingActionButton"/> class.
        /// </summary>
        public MaterialFloatingActionButton()
        {
            Size = new Size(FAB_SIZE, FAB_SIZE);
            DoubleBuffered = true;
            _animationManager = new AnimationManager(false)
            {
                Increment = 0.03,
                AnimationType = AnimationType.EaseOut
            };
            _animationManager.OnAnimationProgress += sender => Invalidate();

            _showAnimationManager = new AnimationManager(true)
            {
                Increment = 0.1,
                AnimationType = AnimationType.EaseOut
            };
            _showAnimationManager.OnAnimationProgress += sender => Invalidate();
            _showAnimationManager.OnAnimationFinished += _showAnimationManager_OnAnimationFinished;
        }

        /// <summary>
        /// The setSize
        /// </summary>
        /// <param name="mini">The mini<see cref="bool"/></param>
        private void setSize(bool mini)
        {
            Size = mini ? new Size(FAB_MINI_SIZE, FAB_MINI_SIZE) : new Size(FAB_SIZE, FAB_SIZE);
            _mini = mini;
        }

        /// <summary>
        /// The _showAnimationManager_OnAnimationFinished
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        private void _showAnimationManager_OnAnimationFinished(object sender)
        {
            if (_isHiding)
            {
                Visible = false;
                _isHiding = false;
            }
        }

        /// <summary>
        /// The OnInvalidated
        /// </summary>
        /// <param name="e">The e<see cref="InvalidateEventArgs"/></param>
        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            base.OnInvalidated(e);
            // for some reason this is needed as Invalidate() does not trigger a repaint when animating.
            InvokePaint(this, new PaintEventArgs(this.CreateGraphics(), this.ClientRectangle));
        }

        /// <summary>
        /// The OnPaint
        /// </summary>
        /// <param name="pevent">The pevent<see cref="PaintEventArgs"/></param>
        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            Rectangle bounds = new Rectangle(new Point(0, 0), Size);
            GraphicsPath regionPath = new GraphicsPath();

            g.FillEllipse(SkinManager.ColorScheme.AccentBrush, bounds);

            if (_animationManager.IsAnimating())
            {
                for (int i = 0; i < _animationManager.GetAnimationCount(); i++)
                {
                    var animationValue = _animationManager.GetProgress(i);
                    var animationSource = _animationManager.GetSource(i);
                    var rippleBrush = new SolidBrush(Color.FromArgb((int)(51 - (animationValue * 50)), Color.White));
                    var rippleSize = (int)(animationValue * Width * 2);
                    g.FillEllipse(rippleBrush, new Rectangle(animationSource.X - rippleSize / 2, animationSource.Y - rippleSize / 2, rippleSize, rippleSize));
                }
            }
            if (_showAnimationManager.IsAnimating())
            {
                int target = Convert.ToInt32((_mini ? FAB_MINI_SIZE : FAB_SIZE) * _showAnimationManager.GetProgress());
                bounds.Width = target == 0 ? 1 : target;
                bounds.Height = target == 0 ? 1 : target;
                bounds.X = Convert.ToInt32(((_mini ? FAB_MINI_SIZE : FAB_SIZE) / 2) - (((_mini ? FAB_MINI_SIZE : FAB_SIZE) / 2) * _showAnimationManager.GetProgress()));
                bounds.Y = Convert.ToInt32(((_mini ? FAB_MINI_SIZE : FAB_SIZE) / 2) - (((_mini ? FAB_MINI_SIZE : FAB_SIZE) / 2) * _showAnimationManager.GetProgress()));
            }

            if (Icon != null)
            {
                Point iconPos = _mini ? new Point(FAB_MINI_ICON_MARGIN, FAB_MINI_ICON_MARGIN) : new Point(FAB_ICON_MARGIN, FAB_ICON_MARGIN);
                g.DrawImage(Icon, new Rectangle(iconPos, new Size(24, 24)));
            }

            regionPath.AddEllipse(bounds);
            Region = new Region(regionPath);
        }

        /// <summary>
        /// The OnMouseUp
        /// </summary>
        /// <param name="mevent">The mevent<see cref="MouseEventArgs"/></param>
        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            _animationManager.StartNewAnimation(AnimationDirection.In, mevent.Location);
        }

        /// <summary>
        /// Defines the origin
        /// </summary>
        private Point origin;

        /// <summary>
        /// The Hide
        /// </summary>
        public new void Hide()
        {
            if (Visible)
            {
                _isHiding = true;
                _showAnimationManager.StartNewAnimation(AnimationDirection.Out);
            }
        }

        /// <summary>
        /// The Show
        /// </summary>
        public new void Show()
        {
            if (!Visible)
            {
                origin = Location;
                _showAnimationManager.StartNewAnimation(AnimationDirection.In);
                Visible = true;
            }
        }
    }
}
