using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using MaterialSkin.Animations;

namespace MaterialSkin.Controls
{
    public class MaterialRaisedButton : Button, IMaterialControl
    {
        private readonly AnimationManager _animationManager;
        private Image _icon;
        private SizeF _textSize;

        public Image Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                if (AutoSize)
                {
                    Size = GetPreferredSize();
                }
                Invalidate();
            }
        }

        public bool Primary { get; set; }

        public override string Text
        {
            get => base.Text;
            set
            {
                base.Text = value;
                _textSize = CreateGraphics().MeasureString(value.ToUpper(), SkinManager.ROBOTO_MEDIUM_10);
                if (AutoSize)
                {
                    Size = GetPreferredSize();
                }
                Invalidate();
            }
        }

        public MaterialRaisedButton()
        {
            Primary = true;

            _animationManager = new AnimationManager(false)
            {
                Increment = 0.03,
                AnimationType = AnimationType.EaseOut
            };
            _animationManager.OnAnimationProgress += sender => Invalidate();

            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            AutoSize = true;
        }

        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        [Browsable(false)]
        public MouseState MouseState { get; set; }

        public override Size GetPreferredSize(Size proposedSize)
        {
            // Provides extra space for proper padding for content
            int extra = 16;

            if (Icon != null)
                // 24 is for icon size
                // 4 is for the space between icon & text
            {
                extra += 24 + 4;
            }

            return new Size((int) Math.Ceiling(_textSize.Width) + extra, 36);
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);

            _animationManager.StartNewAnimation(AnimationDirection.In, mevent.Location);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            g.Clear(Parent.BackColor);

            using (GraphicsPath backgroundPath = DrawHelper.CreateRoundRect(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width - 1, ClientRectangle.Height - 1, 1f))
            {
                g.FillPath(Primary ? SkinManager.ColorScheme.PrimaryBrush : SkinManager.GetRaisedButtonBackgroundBrush(), backgroundPath);
            }

            if (_animationManager.IsAnimating())
            {
                for (int i = 0; i < _animationManager.GetAnimationCount(); i++)
                {
                    double animationValue = _animationManager.GetProgress(i);
                    Point animationSource = _animationManager.GetSource(i);
                    SolidBrush rippleBrush = new SolidBrush(Color.FromArgb((int) (51 - animationValue * 50), Color.White));
                    int rippleSize = (int) (animationValue * Width * 2);
                    g.FillEllipse(rippleBrush, new Rectangle(animationSource.X - rippleSize / 2, animationSource.Y - rippleSize / 2, rippleSize, rippleSize));
                }
            }

            //Icon
            Rectangle iconRect = new Rectangle(8, 6, 24, 24);

            if (string.IsNullOrEmpty(Text))
                // Center Icon
            {
                iconRect.X += 2;
            }

            if (Icon != null)
            {
                g.DrawImage(Icon, iconRect);
            }

            //Text
            Rectangle textRect = ClientRectangle;

            if (Icon != null)
            {
                //
                // Resize and move Text container
                //

                // First 8: left padding
                // 24: icon width
                // Second 4: space between Icon and Text
                // Third 8: right padding
                textRect.Width -= 8 + 24 + 4 + 8;

                // First 8: left padding
                // 24: icon width
                // Second 4: space between Icon and Text
                textRect.X += 8 + 24 + 4;
            }

            g.DrawString(Text.ToUpper(), SkinManager.ROBOTO_MEDIUM_10, SkinManager.GetRaisedButtonTextBrush(Primary), textRect, new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            });
        }

        private Size GetPreferredSize()
        {
            return GetPreferredSize(new Size(0, 0));
        }
    }
}
