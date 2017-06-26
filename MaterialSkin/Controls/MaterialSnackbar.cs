using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin.Animations;
using System.Drawing.Text;
using System.Drawing.Drawing2D;

namespace MaterialSkin.Controls
{
    public class MaterialSnackbar : Control, IMaterialControl
    {
        [Browsable(false)]
        public int Depth { get; set; }
        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;
        [Browsable(false)]
        public MouseState MouseState { get; set; }

        private AnimationManager _animationManager;

        private MaterialLabel _textLabel;
        private MaterialSnackbarFlatButton _button;
        private Timer _idleTimer;
        private bool _isShowing = false;
        private EventHandler _currentClickHandler;

        public override string Text
        {
            get => _textLabel.Text;
            set => _textLabel.Text = value;
        }
        public string ButtonText
        {
            get => _button.Text;
            set => _button.Text = value;
        }

        public bool ShowButton
        {
            get => _button.Visible;
            set => _button.Visible = value;
        }

        public EventHandler OnButtonClick
        {
            set => SetClickEventHandler(value);
        }

        private const int SNACKBAR_HEIGHT = 48;
        private const int SNACKBAR_PADDING_LEFT_RIGHT = 24;
        private const int SNACKBAR_LABEL_PADDING_TOP_BOTTOM = 14;
        private const int SNACKBAR_BUTTON_PADDING_TOP_BOTTOM = 7;
        private const int SNACKBAR_DURATION_LONG = 3500;
        private const int SNACKBAR_DURATION_SHORT = 2000;
        private const int SNACKBAR_MIN_WIDTH = 288;
        private const int SNACKBAR_MAX_WIDTH = 568;
        private const int SNACKBAR_CHAR_BREAK_POSITION = 67;

        public MaterialSnackbar()
        {
            _animationManager = new AnimationManager
            {
                Increment = 0.03,
                AnimationType = AnimationType.EaseOut
            };
            _animationManager.OnAnimationProgress += sender => Invalidate();
            Invalidated += MaterialSnackbar_Invalidated;

            _animationManager.OnAnimationFinished += _animationManager_OnAnimationFinished;

            Visible = false;
            _textLabel = new MaterialLabel();
            Controls.Add(_textLabel);
            _textLabel.Location = new Point(SNACKBAR_PADDING_LEFT_RIGHT, SNACKBAR_LABEL_PADDING_TOP_BOTTOM);
            _textLabel.Font = SkinManager.ROBOTO_REGULAR_14;
            _textLabel.BackColor = SkinManager.GetSnackbarBackgroundColor();
            _textLabel.ForeColor = SkinManager.GetSnackbarTextColor();

            _button = new MaterialSnackbarFlatButton();
            Controls.Add(_button);
            _button.Visible = false;
            _button.Click += _button_Click;

            _idleTimer = new Timer()
            {
                Interval = SNACKBAR_DURATION_SHORT
            };
            _idleTimer.Tick += _idleTimer_Tick;
            this.DoubleBuffered = true;
        }

        private void _button_Click(object sender, EventArgs e)
        {
            _animationManager.StartNewAnimation(AnimationDirection.Out);
            _isShowing = true;
        }

        private void SetClickEventHandler(EventHandler handler)
        {
            if(_currentClickHandler != null)
            {
                _button.Click -= _currentClickHandler;
            }
            _button.Click += handler;
            _currentClickHandler = handler;
        }

        private void MaterialSnackbar_Invalidated(object sender, InvalidateEventArgs e)
        {
            Size = new Size(Parent.Width, SNACKBAR_HEIGHT);
            int textWidth = Convert.ToInt32(CreateGraphics().MeasureString(_textLabel.Text, _textLabel.Font).Width);
            int requiredWidth = SNACKBAR_PADDING_LEFT_RIGHT * 3 + textWidth + _button.Width;
            if(requiredWidth < SNACKBAR_MIN_WIDTH)
            {
                Size = new Size(SNACKBAR_MIN_WIDTH, SNACKBAR_HEIGHT);
            }
            else if(requiredWidth > SNACKBAR_MAX_WIDTH)
            {
                Size = new Size(SNACKBAR_MAX_WIDTH, SNACKBAR_HEIGHT);
                _textLabel.Text = _textLabel.Text.Substring(0, SNACKBAR_CHAR_BREAK_POSITION) + "...";
            }
            else
            {
                Size = new Size(requiredWidth, SNACKBAR_HEIGHT);
            }
            _textLabel.Width = textWidth;
            _textLabel.ForeColor = SkinManager.GetSnackbarTextColor();
            _button.Location = new Point(Width - _button.Width - SNACKBAR_PADDING_LEFT_RIGHT, SNACKBAR_BUTTON_PADDING_TOP_BOTTOM);

            if (_animationManager.IsAnimating())
            {
                var value = _animationManager.GetProgress();
                Location = new Point((Parent.Width / 2) - (Width / 2), Parent.Height - Convert.ToInt32((SNACKBAR_HEIGHT * value)));

            }
        }

        private void _idleTimer_Tick(object sender, EventArgs e)
        {
            _idleTimer.Stop();
            _animationManager.StartNewAnimation(AnimationDirection.Out);
            _isShowing = true;
        }

        private void _animationManager_OnAnimationFinished(object sender)
        {
            if(_isShowing)
            {
                _isShowing = false;
                Visible = false;
            }
            else
            {
                if (!_button.Visible)
                {
                    _idleTimer.Start();
                }
            }
        }

        public void ShowSnackbar(int duration = SNACKBAR_DURATION_SHORT)
        {
            if(!Visible)
            {
                _idleTimer.Interval = duration;
                _animationManager.StartNewAnimation(AnimationDirection.In);
                Visible = true;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            using (var path = DrawHelper.CreateRoundRect(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height, 1f))
            {
                g.FillPath(SkinManager.GetSnackbarBackgroundBrush(), path);
            }
        }
    }

    // This is the only way to get the correct background color for the button AND keep the rounded edges on the snackbar.
    internal class MaterialSnackbarFlatButton : MaterialFlatButton
    {
        private readonly AnimationManager _animationManager;
        private readonly AnimationManager _hoverAnimationManager;

        private SizeF _textSize;

        private Image _icon;

        public MaterialSnackbarFlatButton()
        {
            Primary = false;

            _animationManager = new AnimationManager(false)
            {
                Increment = 0.03,
                AnimationType = AnimationType.EaseOut
            };
            _hoverAnimationManager = new AnimationManager
            {
                Increment = 0.07,
                AnimationType = AnimationType.Linear
            };

            _hoverAnimationManager.OnAnimationProgress += sender => Invalidate();
            _animationManager.OnAnimationProgress += sender => Invalidate();

            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            AutoSize = true;
            Margin = new Padding(4, 6, 4, 6);
            Padding = new Padding(0);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            g.Clear(SkinManager.GetSnackbarBackgroundColor());

            //Hover
            Color c = SkinManager.GetSnackbarBackgroundColor();
            using (Brush b = new SolidBrush(Color.FromArgb((int)(_hoverAnimationManager.GetProgress() * c.A), c.RemoveAlpha())))
                g.FillRectangle(b, ClientRectangle);

            //Ripple
            if (_animationManager.IsAnimating())
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                for (var i = 0; i < _animationManager.GetAnimationCount(); i++)
                {
                    var animationValue = _animationManager.GetProgress(i);
                    var animationSource = _animationManager.GetSource(i);

                    using (Brush rippleBrush = new SolidBrush(Color.FromArgb((int)(101 - (animationValue * 100)), Color.Black)))
                    {
                        var rippleSize = (int)(animationValue * Width * 2);
                        g.FillEllipse(rippleBrush, new Rectangle(animationSource.X - rippleSize / 2, animationSource.Y - rippleSize / 2, rippleSize, rippleSize));
                    }
                }
                g.SmoothingMode = SmoothingMode.None;
            }

            //Icon
            var iconRect = new Rectangle(8, 6, 24, 24);

            if (string.IsNullOrEmpty(Text))
                // Center Icon
                iconRect.X += 2;

            if (Icon != null)
                g.DrawImage(Icon, iconRect);

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

            g.DrawString(
                Text.ToUpper(),
                SkinManager.ROBOTO_MEDIUM_10,
                Enabled ? (Primary ? SkinManager.ColorScheme.PrimaryBrush : SkinManager.GetSnackbarTextBrush()) : SkinManager.GetFlatButtonDisabledTextBrush(),
                textRect,
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center }
                );
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (DesignMode) return;

            MouseState = MouseState.OUT;
            MouseEnter += (sender, args) =>
            {
                MouseState = MouseState.HOVER;
                _hoverAnimationManager.StartNewAnimation(AnimationDirection.In);
                Invalidate();
            };
            MouseLeave += (sender, args) =>
            {
                MouseState = MouseState.OUT;
                _hoverAnimationManager.StartNewAnimation(AnimationDirection.Out);
                Invalidate();
            };
            MouseDown += (sender, args) =>
            {
                if (args.Button == MouseButtons.Left)
                {
                    MouseState = MouseState.DOWN;

                    _animationManager.StartNewAnimation(AnimationDirection.In, args.Location);
                    Invalidate();
                }
            };
            MouseUp += (sender, args) =>
            {
                MouseState = MouseState.HOVER;

                Invalidate();
            };
        }
    }
}
