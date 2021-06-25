namespace MaterialSkin.Controls
{
    using MaterialSkin.Animations;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;
    using System.Runtime.InteropServices;

    public class MaterialSnackBar : MaterialForm
    {

        private const int TOP_PADDING_SINGLE_LINE = 6;
        private const int LEFT_RIGHT_PADDING = 16;
        private const int BUTTON_PADDING = 8;
        private const int BUTTON_HEIGHT = 36;

        private MaterialButton _actionButton = new MaterialButton();
        private Timer _duration = new Timer();      // Timer that checks when the drop down is fully visible

        private AnimationManager _AnimationManager;
        private bool _closingAnimationDone = false;
        private bool _useAccentColor;
        private bool CloseAnimation = false;

        #region "Events"

        [Category("Action")]
        [Description("Fires when Action button is clicked")]
        public event EventHandler ActionButtonClick;

        #endregion


        [Category("Material Skin"), DefaultValue(false), DisplayName("Use Accent Color")]
        public bool UseAccentColor
        {
            get { return _useAccentColor; }
            set { _useAccentColor = value; Invalidate(); }
        }


        /// <summary>
        /// Get or Set SnackBar show duration in milliseconds
        /// </summary>
        [Category("Material Skin"), DefaultValue(2000)]
        public int Duration
        {
            get
            {
                return _duration.Interval;
            }
            set
            {
                _duration.Interval = value;
            }
        }

        private String _text;
        /// <summary>
        /// The Text which gets displayed as the Content
        /// </summary>
        [Category("Material Skin"), DefaultValue("SnackBar text")]
        public new String Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                UpdateRects();
                Invalidate();
            }
        }

        private bool _showActionButton;
        [Category("Material Skin"), DefaultValue(false), DisplayName("Show Action Button")]
        public bool ShowActionButton
        {
            get { return _showActionButton; }
            set { _showActionButton = value; UpdateRects(); Invalidate(); }
        }

        private String _actionButtonText;
        /// <summary>
        /// The Text which gets displayed as the Content
        /// </summary>
        [Category("Material Skin"), DefaultValue("OK")]
        public String ActionButtonText
        {
            get
            {
                return _actionButtonText;
            }
            set
            {
                _actionButtonText = value;
                Invalidate();
            }
        }

        /// <summary>
        /// The Collection for the Buttons
        /// </summary>
        //public ObservableCollection<MaterialButton> Buttons { get; set; }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );

        /// <summary>
        /// Constructer Setting up the Layout
        /// </summary>
        public MaterialSnackBar(string Text, int Duration, bool ShowActionButton, string ActionButtonText, bool UseAccentColor)
        {
            this.Text = Text;
            this.Duration = Duration;
            TopMost = true;
            ShowInTaskbar = false;
            Sizable = false;

            BackColor = SkinManager.SnackBarBackgroundColor;
            FormStyle = FormStyles.StatusAndActionBar_None;

            this.ActionButtonText = ActionButtonText;
            this.UseAccentColor = UseAccentColor;
            //gPath = GetRoundedRP(rect, 30); //The 30 behind is the angle of the circle, the greater the value, the greater the circle angle
            Height = 48;
            MinimumSize = new System.Drawing.Size(344, 48);
            MaximumSize = new System.Drawing.Size(568, 48);

            this.ShowActionButton = ShowActionButton;

            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 6, 6));

            _AnimationManager = new AnimationManager();
            _AnimationManager.AnimationType = AnimationType.EaseOut;
            _AnimationManager.Increment = 0.03;
            _AnimationManager.OnAnimationProgress += _AnimationManager_OnAnimationProgress;

            _duration.Tick += new EventHandler(duration_Tick);

            _actionButton = new MaterialButton
            {
                AutoSize = false,
                NoAccentTextColor = SkinManager.SnackBarTextButtonNoAccentTextColor,
                DrawShadows = false,
                Type = MaterialButton.MaterialButtonType.Text,
                UseAccentColor = _useAccentColor,
                Visible = _showActionButton,
                Text = _actionButtonText
            };
            _actionButton.Click += (sender, e) =>
            {
                ActionButtonClick?.Invoke(this, new EventArgs());
                _closingAnimationDone = false;
                Close();
            };

            if (!Controls.Contains(_actionButton))
            {
                Controls.Add(_actionButton);
            }

            UpdateRects();

        }

        public MaterialSnackBar() : this("SnackBar Text", 3000, false, "OK", false)
        {
        }

        public MaterialSnackBar(string Text) : this(Text, 3000, false, "OK", false)
        {
        }

        public MaterialSnackBar(string Text, int Duration) : this(Text, Duration, false, "OK", false)
        {
        }

        public MaterialSnackBar(string Text, string ActionButtonText) : this(Text, 3000, true, ActionButtonText, false)
        {
        }

        public MaterialSnackBar(string Text, string ActionButtonText, bool UseAccentColor) : this(Text, 3000, true, ActionButtonText, UseAccentColor)
        {
        }

        public MaterialSnackBar(string Text, int Duration, string ActionButtonText) : this(Text, Duration, true, ActionButtonText, false)
        {
        }

        public MaterialSnackBar(string Text, int Duration, string ActionButtonText, bool UseAccentColor) : this(Text, Duration, true, ActionButtonText, UseAccentColor)
        {
        }

        private void UpdateRects()
        {
            if (_showActionButton == true)
            {
                int _buttonWidth = ((TextRenderer.MeasureText(ActionButtonText, SkinManager.getFontByType(MaterialSkinManager.fontType.Button))).Width + 32);
                Rectangle _actionbuttonBounds = new Rectangle((Width) - BUTTON_PADDING - _buttonWidth, TOP_PADDING_SINGLE_LINE, _buttonWidth, BUTTON_HEIGHT);
                _actionButton.Width = _actionbuttonBounds.Width;
                _actionButton.Height = _actionbuttonBounds.Height;
                _actionButton.Text = _actionButtonText;
                _actionButton.Top = _actionbuttonBounds.Top;
                _actionButton.UseAccentColor = _useAccentColor;
            }
            else
            {
                _actionButton.Width = 0;
            }
            _actionButton.Left = Width - BUTTON_PADDING - _actionButton.Width;  //Button minimum width management
            _actionButton.Visible = _showActionButton;

            Width = TextRenderer.MeasureText(_text, SkinManager.getFontByType(MaterialSkinManager.fontType.Body2)).Width + (2 * LEFT_RIGHT_PADDING) + _actionButton.Width + 48;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 6, 6));

        }

        private void duration_Tick(object sender, EventArgs e)
        {
            _duration.Stop();
            _closingAnimationDone = false;
            Close();
        }

         protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateRects();

        }

        /// <summary>
        /// Sets up the Starting Location and starts the Animation
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Location = new Point(Convert.ToInt32(Owner.Location.X + (Owner.Width / 2) - (Width / 2)), Convert.ToInt32(Owner.Location.Y + Owner.Height - 60));
            _AnimationManager.StartNewAnimation(AnimationDirection.In);
            _duration.Start();
        }

        /// <summary>
        /// Animates the Form slides
        /// </summary>
        void _AnimationManager_OnAnimationProgress(object sender)
        {
            if (CloseAnimation)
            {
                Opacity = _AnimationManager.GetProgress();
            }
        }

        /// <summary>
        /// Ovverides the Paint to create the solid colored backcolor
        /// </summary>
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            e.Graphics.Clear(BackColor);

            
            // Calc text Rect
            Rectangle textRect = new Rectangle(
                LEFT_RIGHT_PADDING,
                0,
                Width - (2 * LEFT_RIGHT_PADDING) - _actionButton.Width,
                Height);

            //Draw  Text
            using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
            {
                // Draw header text
                NativeText.DrawTransparentText(
                    _text,
                    SkinManager.getLogFontByType(MaterialSkinManager.fontType.Body2),
                    SkinManager.SnackBarTextHighEmphasisColor,
                    textRect.Location,
                    textRect.Size,
                    NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Middle);
            }

        }

        /// <summary>
        /// Overrides the Closing Event to Animate the Slide Out
        /// </summary>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !_closingAnimationDone;
            if (!_closingAnimationDone)
            {
                CloseAnimation = true;
                _AnimationManager.Increment = 0.06;
                _AnimationManager.OnAnimationFinished += _AnimationManager_OnAnimationFinished;
                _AnimationManager.StartNewAnimation(AnimationDirection.Out);
            }
            base.OnClosing(e);
        }

        /// <summary>
        /// Closes the Form after the pull out animation
        /// </summary>
        void _AnimationManager_OnAnimationFinished(object sender)
        {
            _closingAnimationDone = true;
            Close();
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            _closingAnimationDone = false;
            Close();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(344, 48);
            this.Name = "SnackBar";
            this.ResumeLayout(false);

        }

        /// <summary>
        /// Prevents the Form from beeing dragged
        /// </summary>
        protected override void WndProc(ref Message message)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MOVE = 0xF010;

            switch (message.Msg)
            {
                case WM_SYSCOMMAND:
                    int command = message.WParam.ToInt32() & 0xfff0;
                    if (command == SC_MOVE)
                        return;
                    break;
            }

            base.WndProc(ref message);
        }

    }
}
