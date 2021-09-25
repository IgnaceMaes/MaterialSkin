namespace MaterialSkin.Controls
{
    using MaterialSkin.Animations;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;
    using System.Runtime.InteropServices;

    public class MaterialDialog : MaterialForm
    {

        private const int LEFT_RIGHT_PADDING = 24;
        private const int BUTTON_PADDING = 8;
        private const int BUTTON_HEIGHT = 36;
        private const int TEXT_TOP_PADDING = 17;
        private const int TEXT_BOTTOM_PADDING = 28;
        private int _header_Height = 40;

        private MaterialButton _validationButton = new MaterialButton();
        private MaterialButton _cancelButton = new MaterialButton();
        private AnimationManager _AnimationManager;
        private bool CloseAnimation = false;
        private Form _formOverlay;
        private String _text;
        private String _title;

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
        public MaterialDialog(Form ParentForm, string Title, string Text, string ValidationButtonText, bool ShowCancelButton, string CancelButtonText, bool UseAccentColor)
        {
            _formOverlay = new Form
            {
                BackColor = Color.Black,
                Opacity = 0.5,
                MinimizeBox = false,
                MaximizeBox = false,
                Text = "",
                ShowIcon = false,
                ControlBox = false,
                FormBorderStyle = FormBorderStyle.None,
                Size = new Size(ParentForm.Width, ParentForm.Height),
                ShowInTaskbar = false,
                Owner = ParentForm,
                Visible = true,
                Location = new Point(ParentForm.Location.X, ParentForm.Location.Y),
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom,
            };

            _title = Title;
            if (Title.Length == 0)
                _header_Height = 0;
            else
                _header_Height = 40;

            _text = Text;
            ShowInTaskbar = false;
            Sizable = false;

            BackColor = SkinManager.BackgroundColor;
            FormStyle = FormStyles.StatusAndActionBar_None;

            _AnimationManager = new AnimationManager();
            _AnimationManager.AnimationType = AnimationType.EaseOut;
            _AnimationManager.Increment = 0.03;
            _AnimationManager.OnAnimationProgress += _AnimationManager_OnAnimationProgress;

            _validationButton = new MaterialButton
            {
                AutoSize = false,
                DialogResult = DialogResult.OK,
                DrawShadows = false,
                Type = MaterialButton.MaterialButtonType.Text,
                UseAccentColor = UseAccentColor,
                Text = ValidationButtonText
            };
            _cancelButton = new MaterialButton
            {
                AutoSize = false,
                DialogResult = DialogResult.Cancel,
                DrawShadows = false,
                Type = MaterialButton.MaterialButtonType.Text,
                UseAccentColor = UseAccentColor,
                Visible = ShowCancelButton,
                Text = CancelButtonText
            };

            this.AcceptButton = _validationButton;
            this.CancelButton = _cancelButton;

            if (!Controls.Contains(_validationButton))
                Controls.Add(_validationButton);
            if (!Controls.Contains(_cancelButton))
                Controls.Add(_cancelButton);

            Width = 560;
            int TextWidth = TextRenderer.MeasureText(_text, SkinManager.getFontByType(MaterialSkinManager.fontType.Body1)).Width;
            int RectWidth = Width - (2 * LEFT_RIGHT_PADDING) - BUTTON_PADDING;
            int RectHeight = ((TextWidth / RectWidth) + 1) * 19;
            Rectangle textRect = new Rectangle(
                LEFT_RIGHT_PADDING,
                _header_Height + TEXT_TOP_PADDING,
                RectWidth,
                RectHeight + 9);

            Height = _header_Height + TEXT_TOP_PADDING + textRect.Height + TEXT_BOTTOM_PADDING + 52; //560;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 6, 6));

            int _buttonWidth = ((TextRenderer.MeasureText(ValidationButtonText, SkinManager.getFontByType(MaterialSkinManager.fontType.Button))).Width + 32);
            Rectangle _validationbuttonBounds = new Rectangle((Width) - BUTTON_PADDING - _buttonWidth, Height - BUTTON_PADDING - BUTTON_HEIGHT, _buttonWidth, BUTTON_HEIGHT);
            _validationButton.Width = _validationbuttonBounds.Width;
            _validationButton.Height = _validationbuttonBounds.Height;
            _validationButton.Top = _validationbuttonBounds.Top;
            _validationButton.Left = _validationbuttonBounds.Left;  //Button minimum width management
            _validationButton.Visible = true;

            _buttonWidth = ((TextRenderer.MeasureText(CancelButtonText, SkinManager.getFontByType(MaterialSkinManager.fontType.Button))).Width + 32);
            Rectangle _cancelbuttonBounds = new Rectangle((_validationbuttonBounds.Left) - BUTTON_PADDING - _buttonWidth, Height - BUTTON_PADDING - BUTTON_HEIGHT, _buttonWidth, BUTTON_HEIGHT);
            _cancelButton.Width = _cancelbuttonBounds.Width;
            _cancelButton.Height = _cancelbuttonBounds.Height;
            _cancelButton.Top = _cancelbuttonBounds.Top;
            _cancelButton.Left = _cancelbuttonBounds.Left;  //Button minimum width management


            //this.ShowDialog();
            //this Dispose();
            //return materialDialogResult;
        }

        public MaterialDialog(Form ParentForm) : this(ParentForm, "Title", "Dialog box", "OK", false, "Cancel", false)
        {
        }

        public MaterialDialog(Form ParentForm, string Text) : this(ParentForm, "Title", Text, "OK", false, "Cancel", false)
        {
        }

        public MaterialDialog(Form ParentForm, string Title, string Text) : this(ParentForm, Title, Text, "OK", false, "Cancel", false)
        {
        }

        public MaterialDialog(Form ParentForm, string Title, string Text, string ValidationButtonText) : this(ParentForm, Title, Text, ValidationButtonText, false, "Cancel", false)
        {
        }

        public MaterialDialog(Form ParentForm, string Title, string Text, bool ShowCancelButton) : this(ParentForm, Title, Text, "OK", ShowCancelButton, "Cancel", false)
        {
        }

        public MaterialDialog(Form ParentForm, string Title, string Text, bool ShowCancelButton, string CancelButtonText) : this(ParentForm, Title, Text, "OK", ShowCancelButton, CancelButtonText, false)
        {
        }

       public MaterialDialog(Form ParentForm, string Title, string Text, string ValidationButtonText, bool ShowCancelButton, string CancelButtonText) : this(ParentForm, Title, Text, ValidationButtonText, ShowCancelButton, CancelButtonText, false)
        {
        }


        /// <summary>
        /// Sets up the Starting Location and starts the Animation
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Location = new Point(Convert.ToInt32(Owner.Location.X + (Owner.Width / 2) - (Width / 2)), Convert.ToInt32(Owner.Location.Y + (Owner.Height/2) - (Height / 2)));
            _AnimationManager.StartNewAnimation(AnimationDirection.In);
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

            
            // Calc title Rect
            Rectangle titleRect = new Rectangle(
                LEFT_RIGHT_PADDING,
                0,
                Width - (2 * LEFT_RIGHT_PADDING) ,
                _header_Height);

            //Draw title
            using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
            {
                // Draw header text
                NativeText.DrawTransparentText(
                    _title,
                    SkinManager.getLogFontByType(MaterialSkinManager.fontType.H6),
                    SkinManager.TextHighEmphasisColor,
                    titleRect.Location,
                    titleRect.Size,
                    NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Bottom);
            }

            // Calc text Rect

            int TextWidth = TextRenderer.MeasureText(_text, SkinManager.getFontByType(MaterialSkinManager.fontType.Body1)).Width;
            int RectWidth = Width - (2 * LEFT_RIGHT_PADDING) - BUTTON_PADDING;
            int RectHeight = ((TextWidth / RectWidth) + 1) * 19;

            Rectangle textRect = new Rectangle(
                LEFT_RIGHT_PADDING,
                _header_Height+17,
                RectWidth,
                RectHeight +19);

            //Draw  Text
            using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
            {
                // Draw header text
                NativeText.DrawMultilineTransparentText(
                    _text,
                    SkinManager.getLogFontByType(MaterialSkinManager.fontType.Body1),
                    SkinManager.TextHighEmphasisColor,
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
            _formOverlay.Visible = false;
            _formOverlay.Close();
            _formOverlay.Dispose();

            DialogResult res = this.DialogResult;

            base.OnClosing(e);
        }

        /// <summary>
        /// Closes the Form after the pull out animation
        /// </summary>
        void _AnimationManager_OnAnimationFinished(object sender)
        {
            Close();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(560, 182);
            this.Name = "Dialog";
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
