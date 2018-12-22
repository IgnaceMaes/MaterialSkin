using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using MaterialSkin.Animations;

namespace MaterialSkin.Controls
{
    public class MaterialFloatingActionButton : Button, IMaterialControl
    {
        [Browsable(false)]
        public int Depth { get; set; }
        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;
        [Browsable(false)]
        public MouseState MouseState { get; set; }

        private const int FAB_SIZE = 56;
        private const int FAB_MINI_SIZE = 40;
        private const int FAB_ICON_MARGIN = 16;
        private const int FAB_MINI_ICON_MARGIN = 8;
        private const int FAB_ICON_SIZE = 24;

        public bool Mini
        {
            get { return _mini; }
            set { setSize(value); }
        }
        private bool _mini = false;

        public bool AnimateShowHideButton
        {
            get { return _animateShowButton; }
            set { _animateShowButton = value; }
        }
        private bool _animateShowButton;

        public Image Icon
        {
            get { return _icon; }
            set { _icon = value; }
        }

        private Image _icon;
        private bool _isHiding = false;

        private readonly AnimationManager _animationManager;
        private readonly AnimationManager _showAnimationManager;

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

        private void setSize(bool mini)
        {
            Size = mini ? new Size(FAB_MINI_SIZE, FAB_MINI_SIZE) : new Size(FAB_SIZE, FAB_SIZE);
            _mini = mini;
        }

        private void _showAnimationManager_OnAnimationFinished(object sender)
        {
            if (_isHiding)
            {
                Visible = false;
                _isHiding = false;
            }
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            base.OnInvalidated(e);
            // for some reason this is needed as Invalidate() does not trigger a repaint when animating.
            InvokePaint(this, new PaintEventArgs(this.CreateGraphics(), this.ClientRectangle));
        }

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

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            _animationManager.StartNewAnimation(AnimationDirection.In, mevent.Location);
        }
        private Point origin;

        public new void Hide()
        {
            if (Visible)
            {
                _isHiding = true;
                _showAnimationManager.StartNewAnimation(AnimationDirection.Out);
            }
        }

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