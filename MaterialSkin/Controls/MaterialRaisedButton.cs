using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using MaterialSkin.Animations;
using System;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;

namespace MaterialSkin.Controls
{
    public enum Shades
    {
        Primary,
        PrimaryDark,
        PrimaryLight,
        Accent,
        Danger,
        Warning,
        Success
    }
    public class MaterialRaisedButton : Button, IMaterialControl
    {
        [Browsable(false)]
        public int Depth { get; set; }
        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;
        [Browsable(false)]
        public MouseState MouseState { get; set; }
        public bool Primary { get; set; }
        public Shades Shade { get; set; }
        public bool IsSmall { get; set; }
        public Shades BorderShade { get; set; }
        
        public bool IsWidget
        {
            get => _isWidget;
            set
            {
                _isWidget = value;
                Text = Text;
            }
        }

        private readonly AnimationManager _animationManager;

        private SizeF _textSize;

        private Image _icon;
        private bool _isWidget;

        public Image Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
                if (AutoSize)
                    Size = GetPreferredSize();
                Invalidate();
            }
        }

        public MaterialRaisedButton()
        {
            Primary = true;
            Shade = Shades.PrimaryDark;

            _animationManager = new AnimationManager(false)
            {
                Increment = 0.03,
                AnimationType = AnimationType.EaseOut
            };
            _animationManager.OnAnimationProgress += sender => Invalidate();

            // AutoSizeMode = AutoSizeMode.GrowAndShrink;
            // AutoSize = true;
        }

        public override string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                var font = IsWidget ? SkinManager.ROBOTO_TITLE : SkinManager.ROBOTO_MEDIUM_10;

                _textSize = CreateGraphics().MeasureString(value.ToUpper(), font);
                if (AutoSize)
                    Size = GetPreferredSize();
                Invalidate();
            }
        }

        protected override void OnClick(EventArgs e)
        {
            var mevent = e as MouseEventArgs;
            if (mevent != null)
            {
                _animationManager.StartNewAnimation(AnimationDirection.In, mevent.Location);
            }            
            base.OnClick(e);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
           
            g.Clear(Parent.BackColor);
            var radius = IsSmall ? 3f : 6f;
            using (var backgroundPath = DrawHelper.CreateRoundRect(ClientRectangle.X,
                ClientRectangle.Y,
                ClientRectangle.Width - 1,
                ClientRectangle.Height - 1,
                radius))
            {
                // g.FillPath(Primary ? SkinManager.ColorScheme.PrimaryBrush : SkinManager.GetRaisedButtonBackgroundBrush(), backgroundPath);
                var fillBrush = MaterialSkinManager.GetMaterialBrush(Shade);
                g.FillPath(fillBrush, backgroundPath);
                g.DrawPath(MaterialSkinManager.GetMaterialPen(BorderShade), backgroundPath);
            }

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

            //Icon

            if (Icon != null)
            {
                var iconRect = new Rectangle(8, (Height/2)- Icon.Height/2, Icon.Width, Icon.Height);

                //create a color matrix object  & set the opacity
                var matrix = new ColorMatrix { Matrix33 = (float) 0.75 };

                //set the color(opacity) of the image                  
                var attributes = new ImageAttributes();                 
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);                    

                // Draw the image
                g.DrawImage(Icon, iconRect, 0, 0, Icon.Width, Icon.Height, GraphicsUnit.Pixel, attributes );
            }

            //Text
            var textRect = ClientRectangle;

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

            textRect.Y =  Height / 2 - (int) Math.Round(_textSize.Height / 2)+2;
            textRect.Height = (int)Math.Round(_textSize.Height);
            var font = IsWidget ? SkinManager.ROBOTO_TITLE : SkinManager.ROBOTO_MEDIUM_10;
            g.DrawString(
                Text.ToUpper(),
                font,
                SkinManager.GetRaisedButtonTextBrush(Primary),
                textRect,
                new StringFormat { Alignment = ContentToTextHAlignment(TextAlign), LineAlignment = ContentToTextVAlignment(TextAlign) });

            if (Enabled == false)
            {
                
            }
        }        

        public static StringAlignment ContentToTextHAlignment(ContentAlignment textAlign)
        {
            switch (textAlign)
            {
                case ContentAlignment.TopLeft:
                    return StringAlignment.Near;                    
                case ContentAlignment.TopCenter:
                    return StringAlignment.Center;                    
                case ContentAlignment.TopRight:
                    return StringAlignment.Far;
                case ContentAlignment.MiddleLeft:
                    return StringAlignment.Near;
                case ContentAlignment.MiddleCenter:
                    return StringAlignment.Center;
                case ContentAlignment.MiddleRight:
                    return StringAlignment.Far;
                case ContentAlignment.BottomLeft:
                    return StringAlignment.Near;
                case ContentAlignment.BottomCenter:
                    return StringAlignment.Center;
                case ContentAlignment.BottomRight:
                    return StringAlignment.Far;
                default:
                    throw new ArgumentOutOfRangeException(nameof(textAlign), textAlign, null);
            }
        }

        public static StringAlignment ContentToTextVAlignment(ContentAlignment textAlign)
        {
            switch (textAlign)
            {
                case ContentAlignment.TopLeft:
                    return StringAlignment.Near;                    
                case ContentAlignment.TopCenter:
                    return StringAlignment.Near;
                case ContentAlignment.TopRight:
                    return StringAlignment.Near;
                case ContentAlignment.MiddleLeft:
                    return StringAlignment.Center;
                case ContentAlignment.MiddleCenter:
                    return StringAlignment.Center;
                case ContentAlignment.MiddleRight:
                    return StringAlignment.Center;
                case ContentAlignment.BottomLeft:
                    return StringAlignment.Far;
                case ContentAlignment.BottomCenter:
                    return StringAlignment.Far;
                case ContentAlignment.BottomRight:
                    return StringAlignment.Far;
                default:
                    throw new ArgumentOutOfRangeException(nameof(textAlign), textAlign, null);
            }
        }

        private Size GetPreferredSize()
        {
            return GetPreferredSize(new Size(0, 0));
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            // Provides extra space for proper padding for content
            var extra = 16;

            if (Icon != null)
                // 24 is for icon size
                // 4 is for the space between icon & text
                extra += 24 + 4;

            return new Size((int)Math.Ceiling(_textSize.Width) + extra, IsSmall ? 24 : 36);
        }
    }
}
