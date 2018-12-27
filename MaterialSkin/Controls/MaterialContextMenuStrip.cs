namespace MaterialSkin.Controls
{
    using MaterialSkin.Animations;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    /// <summary>
    /// Defines the <see cref="MaterialContextMenuStrip" />
    /// </summary>
    public class MaterialContextMenuStrip : ContextMenuStrip, IMaterialControl
    {
        //Properties for managing the material design properties
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
        /// Defines the AnimationManager
        /// </summary>
        internal AnimationManager AnimationManager;

        /// <summary>
        /// Defines the AnimationSource
        /// </summary>
        internal Point AnimationSource;

        /// <summary>
        /// The ItemClickStart
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="ToolStripItemClickedEventArgs"/></param>
        public delegate void ItemClickStart(object sender, ToolStripItemClickedEventArgs e);

        /// <summary>
        /// Defines the OnItemClickStart
        /// </summary>
        public event ItemClickStart OnItemClickStart;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialContextMenuStrip"/> class.
        /// </summary>
        public MaterialContextMenuStrip()
        {
            Renderer = new MaterialToolStripRender();

            AnimationManager = new AnimationManager(false)
            {
                Increment = 0.07,
                AnimationType = AnimationType.Linear
            };
            AnimationManager.OnAnimationProgress += sender => Invalidate();
            AnimationManager.OnAnimationFinished += sender => OnItemClicked(_delayesArgs);

            BackColor = SkinManager.GetApplicationBackgroundColor();
        }

        /// <summary>
        /// The OnMouseUp
        /// </summary>
        /// <param name="mea">The mea<see cref="MouseEventArgs"/></param>
        protected override void OnMouseUp(MouseEventArgs mea)
        {
            base.OnMouseUp(mea);

            AnimationSource = mea.Location;
        }

        /// <summary>
        /// Defines the _delayesArgs
        /// </summary>
        private ToolStripItemClickedEventArgs _delayesArgs;

        /// <summary>
        /// The OnItemClicked
        /// </summary>
        /// <param name="e">The e<see cref="ToolStripItemClickedEventArgs"/></param>
        protected override void OnItemClicked(ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem != null && !(e.ClickedItem is ToolStripSeparator))
            {
                if (e == _delayesArgs)
                {
                    //The event has been fired manualy because the args are the ones we saved for delay
                    base.OnItemClicked(e);
                }
                else
                {
                    //Interrupt the default on click, saving the args for the delay which is needed to display the animaton
                    _delayesArgs = e;

                    //Fire custom event to trigger actions directly but keep cms open
                    OnItemClickStart?.Invoke(this, e);

                    //Start animation
                    AnimationManager.StartNewAnimation(AnimationDirection.In);
                }
            }
        }
    }

    /// <summary>
    /// Defines the <see cref="MaterialToolStripMenuItem" />
    /// </summary>
    public class MaterialToolStripMenuItem : ToolStripMenuItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialToolStripMenuItem"/> class.
        /// </summary>
        public MaterialToolStripMenuItem()
        {
            AutoSize = false;
            Size = new Size(120, 30);
        }

        /// <summary>
        /// The CreateDefaultDropDown
        /// </summary>
        /// <returns>The <see cref="ToolStripDropDown"/></returns>
        protected override ToolStripDropDown CreateDefaultDropDown()
        {
            var baseDropDown = base.CreateDefaultDropDown();
            if (DesignMode) return baseDropDown;

            var defaultDropDown = new MaterialContextMenuStrip();
            defaultDropDown.Items.AddRange(baseDropDown.Items);

            return defaultDropDown;
        }
    }

    /// <summary>
    /// Defines the <see cref="MaterialToolStripRender" />
    /// </summary>
    internal class MaterialToolStripRender : ToolStripProfessionalRenderer, IMaterialControl
    {
        //Properties for managing the material design properties
        /// <summary>
        /// Gets or sets the Depth
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Gets the SkinManager
        /// </summary>
        public MaterialSkinManager SkinManager=> MaterialSkinManager.Instance;

        /// <summary>
        /// Gets or sets the MouseState
        /// </summary>
        public MouseState MouseState { get; set; }

        /// <summary>
        /// The OnRenderItemText
        /// </summary>
        /// <param name="e">The e<see cref="ToolStripItemTextRenderEventArgs"/></param>
        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            var g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            var itemRect = GetItemRect(e.Item);
            var textRect = new Rectangle(24, itemRect.Y, itemRect.Width - (24 + 16), itemRect.Height);
            g.DrawString(
                e.Text,
                SkinManager.ROBOTO_MEDIUM_10,
                e.Item.Enabled ? SkinManager.GetPrimaryTextBrush() : SkinManager.GetDisabledOrHintBrush(),
                textRect,
                new StringFormat { LineAlignment = StringAlignment.Center });
        }

        /// <summary>
        /// The OnRenderMenuItemBackground
        /// </summary>
        /// <param name="e">The e<see cref="ToolStripItemRenderEventArgs"/></param>
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(SkinManager.GetApplicationBackgroundColor());

            //Draw background
            var itemRect = GetItemRect(e.Item);
            g.FillRectangle(e.Item.Selected && e.Item.Enabled ? SkinManager.GetCmsSelectedItemBrush() : new SolidBrush(SkinManager.GetApplicationBackgroundColor()), itemRect);

            //Ripple animation
            var toolStrip = e.ToolStrip as MaterialContextMenuStrip;
            if (toolStrip != null)
            {
                var animationManager = toolStrip.AnimationManager;
                var animationSource = toolStrip.AnimationSource;
                if (toolStrip.AnimationManager.IsAnimating() && e.Item.Bounds.Contains(animationSource))
                {
                    for (int i = 0; i < animationManager.GetAnimationCount(); i++)
                    {
                        var animationValue = animationManager.GetProgress(i);
                        var rippleBrush = new SolidBrush(Color.FromArgb((int)(51 - (animationValue * 50)), Color.Black));
                        var rippleSize = (int)(animationValue * itemRect.Width * 2.5);
                        g.FillEllipse(rippleBrush, new Rectangle(animationSource.X - rippleSize / 2, itemRect.Y - itemRect.Height, rippleSize, itemRect.Height * 3));
                    }
                }
            }
        }

        /// <summary>
        /// The OnRenderImageMargin
        /// </summary>
        /// <param name="e">The e<see cref="ToolStripRenderEventArgs"/></param>
        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
        }

        /// <summary>
        /// The OnRenderSeparator
        /// </summary>
        /// <param name="e">The e<see cref="ToolStripSeparatorRenderEventArgs"/></param>
        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            var g = e.Graphics;

            g.FillRectangle(new SolidBrush(SkinManager.GetApplicationBackgroundColor()), e.Item.Bounds);
            g.DrawLine(
                new Pen(SkinManager.GetDividersColor()),
                new Point(e.Item.Bounds.Left, e.Item.Bounds.Height / 2),
                new Point(e.Item.Bounds.Right, e.Item.Bounds.Height / 2));
        }

        /// <summary>
        /// The OnRenderToolStripBorder
        /// </summary>
        /// <param name="e">The e<see cref="ToolStripRenderEventArgs"/></param>
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            var g = e.Graphics;

            g.DrawRectangle(
                new Pen(SkinManager.GetDividersColor()),
                new Rectangle(e.AffectedBounds.X, e.AffectedBounds.Y, e.AffectedBounds.Width - 1, e.AffectedBounds.Height - 1));
        }

        /// <summary>
        /// The OnRenderArrow
        /// </summary>
        /// <param name="e">The e<see cref="ToolStripArrowRenderEventArgs"/></param>
        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            var g = e.Graphics;
            const int ARROW_SIZE = 4;

            var arrowMiddle = new Point(e.ArrowRectangle.X + e.ArrowRectangle.Width / 2, e.ArrowRectangle.Y + e.ArrowRectangle.Height / 2);
            var arrowBrush = e.Item.Enabled ? SkinManager.GetPrimaryTextBrush() : SkinManager.GetDisabledOrHintBrush();
            using (var arrowPath = new GraphicsPath())
            {
                arrowPath.AddLines(
                    new[] {
                        new Point(arrowMiddle.X - ARROW_SIZE, arrowMiddle.Y - ARROW_SIZE),
                        new Point(arrowMiddle.X, arrowMiddle.Y),
                        new Point(arrowMiddle.X - ARROW_SIZE, arrowMiddle.Y + ARROW_SIZE) });
                arrowPath.CloseFigure();

                g.FillPath(arrowBrush, arrowPath);
            }
        }

        /// <summary>
        /// The GetItemRect
        /// </summary>
        /// <param name="item">The item<see cref="ToolStripItem"/></param>
        /// <returns>The <see cref="Rectangle"/></returns>
        private Rectangle GetItemRect(ToolStripItem item)
        {
            return new Rectangle(0, item.ContentRectangle.Y, item.ContentRectangle.Width + 4, item.ContentRectangle.Height);
        }
    }
}
