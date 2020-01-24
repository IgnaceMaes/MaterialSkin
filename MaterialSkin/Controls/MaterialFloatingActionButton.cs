namespace MaterialSkin.Controls
{
    using MaterialSkin.Animations;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

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

        public bool DrawShadows { get; set; }

        public bool Mini
        {
            get { return _mini; }
            set
            {
                if (Parent != null)
                    Parent.Invalidate();
                setSize(value);
            }
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
            DrawShadows = true;
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            Size = new Size(FAB_SIZE, FAB_SIZE);
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

        protected override void InitLayout()
        {
            LocationChanged += (sender, e) => { if (DrawShadows) Parent?.Invalidate(); };
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            if (DrawShadows && Parent != null) AddShadowPaintEvent(Parent, drawShadowOnParent);
            if (_oldParent != null) RemoveShadowPaintEvent(_oldParent, drawShadowOnParent);
            _oldParent = Parent;
        }
        Control _oldParent;

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Parent == null) return;
            if (Visible)
                AddShadowPaintEvent(Parent, drawShadowOnParent);
            else
                RemoveShadowPaintEvent(Parent, drawShadowOnParent);
        }

        bool _shadowDrawEventSubscribed = false;
        private void AddShadowPaintEvent(Control control, PaintEventHandler shadowPaintEvent)
        {
            if (_shadowDrawEventSubscribed) return;
            control.Paint += shadowPaintEvent;
            control.Invalidate();
            _shadowDrawEventSubscribed = true;
        }
        private void RemoveShadowPaintEvent(Control control, PaintEventHandler shadowPaintEvent)
        {
            if (!_shadowDrawEventSubscribed) return;
            control.Paint -= shadowPaintEvent;
            control.Invalidate();
            _shadowDrawEventSubscribed = false;
        }

        private void setSize(bool mini)
        {
            _mini = mini;
            Size = _mini ? new Size(FAB_MINI_SIZE, FAB_MINI_SIZE) : new Size(FAB_SIZE, FAB_SIZE);
            fabBounds = _mini ? new Rectangle(0, 0, FAB_MINI_SIZE, FAB_MINI_SIZE) : new Rectangle(0, 0, FAB_SIZE, FAB_SIZE);
            fabBounds.Width -= 1;
            fabBounds.Height -= 1;
        }

        private void _showAnimationManager_OnAnimationFinished(object sender)
        {
            if (_isHiding)
            {
                Visible = false;
                _isHiding = false;
            }
        }

        private void drawShadowOnParent(object sender, PaintEventArgs e)
        {
            if (Parent == null)
            {
                RemoveShadowPaintEvent((Control)sender, drawShadowOnParent);
                return;
            }

            // paint shadow on parent
            Graphics gp = e.Graphics;
            Rectangle rect = new Rectangle(Location, fabBounds.Size);
            gp.SmoothingMode = SmoothingMode.AntiAlias;
            DrawHelper.DrawRoundShadow(gp, rect);
        }

        private Rectangle fabBounds;
        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;

            g.Clear(Parent.BackColor);
            g.SmoothingMode = SmoothingMode.AntiAlias;


            // Paint shadow on element to blend with the parent shadow
            DrawHelper.DrawRoundShadow(g, fabBounds);

            // draw fab
            g.FillEllipse(SkinManager.ColorScheme.AccentBrush, fabBounds);


            if (_animationManager.IsAnimating())
            {
                GraphicsPath regionPath = new GraphicsPath();
                regionPath.AddEllipse(new Rectangle(fabBounds.X - 1, fabBounds.Y - 1, fabBounds.Width + 3, fabBounds.Height + 2));
                Region fabRegion = new Region(regionPath);

                GraphicsContainer gcont = g.BeginContainer();
                g.SetClip(fabRegion, CombineMode.Replace);

                for (int i = 0; i < _animationManager.GetAnimationCount(); i++)
                {
                    var animationValue = _animationManager.GetProgress(i);
                    var animationSource = _animationManager.GetSource(i);
                    var rippleBrush = new SolidBrush(Color.FromArgb((int)(51 - (animationValue * 50)), Color.White));
                    var rippleSize = (int)(animationValue * Width * 2);
                    g.FillEllipse(rippleBrush, new Rectangle(animationSource.X - rippleSize / 2, animationSource.Y - rippleSize / 2, rippleSize, rippleSize));
                }

                g.EndContainer(gcont);
            }


            if (Icon != null)
            {
                g.DrawImage(Icon, new Rectangle(fabBounds.Width / 2 - 11, fabBounds.Height / 2 - 11, 24, 24));
            }


            if (_showAnimationManager.IsAnimating())
            {
                int target = Convert.ToInt32((_mini ? FAB_MINI_SIZE : FAB_SIZE) * _showAnimationManager.GetProgress());
                fabBounds.Width = target == 0 ? 1 : target;
                fabBounds.Height = target == 0 ? 1 : target;
                fabBounds.X = Convert.ToInt32(((_mini ? FAB_MINI_SIZE : FAB_SIZE) / 2) - (((_mini ? FAB_MINI_SIZE : FAB_SIZE) / 2) * _showAnimationManager.GetProgress()));
                fabBounds.Y = Convert.ToInt32(((_mini ? FAB_MINI_SIZE : FAB_SIZE) / 2) - (((_mini ? FAB_MINI_SIZE : FAB_SIZE) / 2) * _showAnimationManager.GetProgress()));
            }

            // Clip to a round shape with a 1px padding
            GraphicsPath clipPath = new GraphicsPath();
            clipPath.AddEllipse(new Rectangle(fabBounds.X - 1, fabBounds.Y - 1, fabBounds.Width + 3, fabBounds.Height + 3));
            Region = new Region(clipPath);
        }

        protected override void OnMouseClick(MouseEventArgs mevent)
        {
            base.OnMouseClick(mevent);
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
