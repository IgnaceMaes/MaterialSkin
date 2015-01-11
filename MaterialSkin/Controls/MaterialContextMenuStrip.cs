using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using MaterialSkin.Animations;

namespace MaterialSkin.Controls
{
    public class MaterialContextMenuStrip : ContextMenuStrip, IMaterialControl
    {
        //Properties for managing the material design properties
        public int Depth { get; set; }
        public MaterialSkinManager SkinManager { get { return MaterialSkinManager.Instance; } }
        public MouseState MouseState { get; set; }


        internal AnimationManager animationManager;
        internal Point animationSource;

        public MaterialContextMenuStrip()
        {
            Renderer = new MaterialToolStripRender();

            animationManager = new AnimationManager()
            {
                Increment = 0.05,
                AnimationType = AnimationType.Linear,
                InterruptAnimation = true
            };
            animationManager.OnAnimationProgress += sender => Invalidate();
            animationManager.OnAnimationFinished += sender => OnItemClicked(delayesArgs);
        }

        protected override void OnMouseUp(MouseEventArgs mea)
        {
            base.OnMouseUp(mea);

            animationSource = mea.Location;
        }

        private ToolStripItemClickedEventArgs delayesArgs;
        protected override void OnItemClicked(ToolStripItemClickedEventArgs e)
        {
            if (delayesArgs != null && e == delayesArgs && e.ClickedItem != null)
            {
                //The event has been fired manualy because the args are the ones we saved for delay
                base.OnItemClicked(e);
            }
            else
            {
                //Interrupt the default on click, saving the args for the delay which is needed to display the animaton
                delayesArgs = e;
                animationManager.StartNewAnimation(AnimationDirection.In);
            }
        }
    }

    public class MaterialToolStripMenuItem : ToolStripMenuItem
    {

        public MaterialToolStripMenuItem()
        {
            AutoSize = false;
            Size = new Size(120, 32);
        }

        protected override ToolStripDropDown CreateDefaultDropDown()
        {
            var baseDropDown = base.CreateDefaultDropDown();
            if (DesignMode) return baseDropDown;

            var defaultDropDown = new MaterialContextMenuStrip();
            defaultDropDown.Items.AddRange(baseDropDown.Items);

            return defaultDropDown;
        }
    }

    internal class MaterialToolStripRender : ToolStripProfessionalRenderer, IMaterialControl
    {
        //Properties for managing the material design properties
        public int Depth { get; set; }
        public MaterialSkinManager SkinManager { get { return MaterialSkinManager.Instance; } }
        public MouseState MouseState { get; set; }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            var g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            var itemRect = GetItemRect(e.Item);
            var textRect = new Rectangle(24, itemRect.Y, itemRect.Width - (24 + 16), itemRect.Height);
            g.DrawString(e.Text, SkinManager.ROBOTO_MEDIUM_10, e.Item.Enabled ? SkinManager.GetMainTextBrush() : SkinManager.GetDisabledOrHintBrush(), textRect, new StringFormat() { LineAlignment = StringAlignment.Center });
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(SkinManager.GetApplicationBackgroundColor());

            //Draw background
            var itemRect = GetItemRect(e.Item);
            g.FillRectangle(e.Item.Selected ? SkinManager.GetCmsSelectedItemBrush() : new SolidBrush(SkinManager.GetApplicationBackgroundColor()),itemRect);

            //Ripple animation
            var toolStrip = e.ToolStrip as MaterialContextMenuStrip;
            if (toolStrip != null)
            {
                var animationManager = toolStrip.animationManager;
                var animationSource = toolStrip.animationSource;
                if (toolStrip.animationManager.IsAnimating() && e.Item.Bounds.Contains(animationSource))
                {
                    var animationValue = animationManager.GetProgress();
                    var rippleBrush = new SolidBrush(Color.FromArgb((int)(51 - (animationValue * 50)), Color.Black));
                    var rippleSize = (int)(animationValue * itemRect.Width * 3);
                    g.FillEllipse(rippleBrush, new Rectangle(animationSource.X - rippleSize / 2, itemRect.Y - itemRect.Height, rippleSize, itemRect.Height * 3));
                }
            }
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            Graphics g = e.Graphics;

            g.DrawLine(new Pen(Color.FromArgb(226, 227, 227)), new Point(e.Item.Bounds.Left + 22, e.Item.Bounds.Height / 2 - 1), new Point(e.Item.Bounds.Right, e.Item.Bounds.Height / 2 - 1));
            g.DrawLine(Pens.White, new Point(e.Item.Bounds.Left + 22, e.Item.Bounds.Height / 2), new Point(e.Item.Bounds.Right, e.Item.Bounds.Height / 2));
        }

        private Rectangle GetItemRect(ToolStripItem item)
        {
            return  new Rectangle(0, item.ContentRectangle.Y, item.ContentRectangle.Width + 4, item.ContentRectangle.Height);
        }
    }
}
