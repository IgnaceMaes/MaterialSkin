namespace MaterialSkin.Controls
{
    using MaterialSkin.Animations;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class MaterialTextBox : RichTextBox, IMaterialControl
    {
        //Properties for managing the material design properties
        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        [Browsable(false)]
        public MouseState MouseState { get; set; }

        [Category("Material Skin"), DefaultValue(false)]
        public bool Password { get; set; }

        private bool _UseTallSize;

        [Category("Material Skin"), DefaultValue(true), Description("Using a larger size enables the hint to always be visible")]
        public bool UseTallSize
        {
            get { return _UseTallSize; }
            set
            {
                _UseTallSize = value;
                HEIGHT = UseTallSize ? 50 : 36;
                Size = new Size(Size.Width, HEIGHT);
                Invalidate();
            }
        }

        [Category("Material Skin"), DefaultValue(true)]
        public bool UseAccent { get; set; }

        private string _hint = string.Empty;

        [Category("Material Skin"), DefaultValue(""), Localizable(true)]
        public string Hint
        {
            get { return _hint; }
            set
            {
                _hint = value;
                hasHint = !String.IsNullOrEmpty(Hint);
                Invalidate();
            }
        }

        private Image _leadingIcon;

        [Category("Material Skin"), Browsable(true), Localizable(false)]
        /// <summary>
        /// Gets or sets the leading Icon
        /// </summary>
        public Image LeadingIcon
        {
            get { return _leadingIcon; }
            set
            {
                _leadingIcon = value;
                UpdateRects();
                //if (_leadingIcon != null) 
                //    _left_padding = SkinManager.FORM_PADDING+ ICON_SIZE;
                //else
                //    _left_padding = SkinManager.FORM_PADDING;
                if (AutoSize)
                {
                    Refresh();
                }
                else
                {
                    Invalidate();
                }
            }
        }

        private Image _trailingIcon;

        [Category("Material Skin"), Browsable(true), Localizable(false)]
        /// <summary>
        /// Gets or sets the trailing Icon
        /// </summary>
        public Image TrailingIcon
        {
            get { return _trailingIcon; }
            set
            {
                _trailingIcon = value;
                UpdateRects();
                //if (_trailingIcon != null)
                //    _right_padding = SkinManager.FORM_PADDING + ICON_SIZE;
                //else
                //    _right_padding = SkinManager.FORM_PADDING;
                if (AutoSize)
                {
                    Refresh();
                }
                else
                {
                    Invalidate();
                }
            }
        }

        private const int ICON_SIZE = 24;
        private const int HINT_TEXT_SMALL_SIZE = 18;
        private const int HINT_TEXT_SMALL_Y = 4;
        private const int BOTTOM_PADDING = 3;
        private int HEIGHT = 50;
        private int LINE_Y;

        private bool hasHint;
        private int _left_padding ;
        private int _right_padding ;
        private Rectangle _leadingIconBounds;
        private Rectangle _trailingIconBounds;
        private Rectangle _textfieldBounds;

        private readonly AnimationManager _animationManager;

        #region "Events"

        [Category("Action")]
        [Description("Fires when Leading Icon is clicked")]
        public event EventHandler LeadingIconClick;

        [Category("Action")]
        [Description("Fires when Trailing Icon is clicked")]
        public event EventHandler TrailingIconClick;

        #endregion

        public MaterialTextBox()
        {
            // Material Properties
            Hint = "";
            Password = false;
            UseAccent = true;
            UseTallSize = true;

            // Properties
            TabStop = true;
            Multiline = false;
            BorderStyle = BorderStyle.None;

            // Animations
            _animationManager = new AnimationManager
            {
                Increment = 0.08,
                AnimationType = AnimationType.EaseInOut
            };
            _animationManager.OnAnimationProgress += sender => Invalidate();

            MaterialContextMenuStrip cms = new TextBoxContextMenuStrip();
            cms.Opening += ContextMenuStripOnOpening;
            cms.OnItemClickStart += ContextMenuStripOnItemClickStart;

            ContextMenuStrip = cms;

            MaxLength = 50;
        }

        private const int EM_SETPASSWORDCHAR = 0x00cc;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            base.Font = new Font(SkinManager.getFontByType(MaterialSkinManager.fontType.Subtitle1).FontFamily, 12f, FontStyle.Regular);
            base.AutoSize = false;

            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            if (Password) SendMessage(Handle, EM_SETPASSWORDCHAR, 'T', 0);

            // Size and padding
            HEIGHT = UseTallSize ? 50 : 36;
            Size = new Size(Size.Width, HEIGHT);
            LINE_Y = HEIGHT - BOTTOM_PADDING;
            UpdateRects();

            // events
            MouseState = MouseState.OUT;
            LostFocus += (sender, args) => _animationManager.StartNewAnimation(AnimationDirection.Out);
            GotFocus += (sender, args) =>
            {
                _animationManager.StartNewAnimation(AnimationDirection.In);
            };
            MouseEnter += (sender, args) =>
            {
                MouseState = MouseState.HOVER;
                Invalidate();
            };
            MouseLeave += (sender, args) =>
            {
                MouseState = MouseState.OUT;
                Invalidate();
            };
            HScroll += (sender, args) =>
            {
                SendMessage(this.Handle, EM_GETSCROLLPOS, 0, ref scrollPos);
                Invalidate();
            };
            KeyDown += (sender, args) =>
            {
                SendMessage(this.Handle, EM_GETSCROLLPOS, 0, ref scrollPos);
            };
        }

        private Point scrollPos = Point.Empty;
        private const int EM_GETSCROLLPOS = WM_USER + 221;
        private const int WM_USER = 0x400;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, Int32 wMsg, Int32 wParam, ref Point lParam);

        public override Size GetPreferredSize(Size proposedSize)
        {
            return new Size(proposedSize.Width, HEIGHT);
        }

        private void UpdateRects()
        {
            if (LeadingIcon != null)
                _left_padding = SkinManager.FORM_PADDING + ICON_SIZE;
            else
                _left_padding = SkinManager.FORM_PADDING;

            if (_trailingIcon != null)
                _right_padding = SkinManager.FORM_PADDING + ICON_SIZE;
            else
                _right_padding = SkinManager.FORM_PADDING;

            _leadingIconBounds = new Rectangle(8, (HEIGHT / 2) - (ICON_SIZE / 2), ICON_SIZE, ICON_SIZE);
            _trailingIconBounds = new Rectangle(Width - (ICON_SIZE + 8), (HEIGHT / 2) - (ICON_SIZE / 2), ICON_SIZE, ICON_SIZE);
            _textfieldBounds = new Rectangle(_left_padding, ClientRectangle.Y, Width - _left_padding - _right_padding, LINE_Y);

            var rect = new Rectangle(_left_padding, UseTallSize ? hasHint ?
        (HINT_TEXT_SMALL_Y + HINT_TEXT_SMALL_SIZE) : // Has hint and it's tall
        (int)(LINE_Y / 3.5) : // No hint and tall
        HEIGHT / 5, // not tall
        ClientSize.Width - _left_padding - _right_padding, LINE_Y);
            RECT rc = new RECT(rect);
            SendMessageRefRect(Handle, EM_SETRECT, 0, ref rc);

        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            var g = pevent.Graphics;

            g.Clear(Parent.BackColor);

            SolidBrush backBrush = new SolidBrush(DrawHelper.BlendColor(Parent.BackColor, SkinManager.BackgroundAlternativeColor, SkinManager.BackgroundAlternativeColor.A));

            g.FillRectangle(
                !Enabled ? SkinManager.BackgroundDisabledBrush : // Disabled
                Focused ? SkinManager.BackgroundFocusBrush :  // Focused
                MouseState == MouseState.HOVER ? SkinManager.BackgroundHoverBrush : // Hover
                backBrush, // Normal
                ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, LINE_Y);

            //Leading Icon
            if (LeadingIcon != null)
            {
                g.DrawImage(LeadingIcon, _leadingIconBounds);
            }

            //Trailing Icon
            if (TrailingIcon != null)
            {
                g.DrawImage(TrailingIcon, _trailingIconBounds);
            }

            // HintText
            bool userTextPresent = !String.IsNullOrEmpty(Text);
            Color textColor = Enabled ? Focused ?
                            UseAccent ? SkinManager.ColorScheme.AccentColor : SkinManager.ColorScheme.PrimaryColor : // Focused
                            SkinManager.TextHighEmphasisColor : // Inactive
                            SkinManager.TextDisabledOrHintColor; // Disabled
            Rectangle hintRect = new Rectangle(_left_padding, ClientRectangle.Y, Width - _left_padding - _right_padding, LINE_Y);
            int hintTextSize = 16;

            // bottom line base
            g.FillRectangle(SkinManager.DividersAlternativeBrush, 0, LINE_Y, Width, 1);

            if (!_animationManager.IsAnimating())
            {
                // No animation
                if (hasHint && UseTallSize && (Focused || userTextPresent))
                {
                    // hint text
                    hintRect = new Rectangle(_left_padding, HINT_TEXT_SMALL_Y, Width - _left_padding - _right_padding, HINT_TEXT_SMALL_SIZE);
                    hintTextSize = 12;
                }

                // bottom line
                if (Focused)
                {
                    g.FillRectangle(UseAccent ? SkinManager.ColorScheme.AccentBrush : SkinManager.ColorScheme.PrimaryBrush, 0, LINE_Y, Width, 2);
                }
            }
            else
            {
                // Animate - Focus got/lost
                double animationProgress = _animationManager.GetProgress();

                // hint Animation
                if (hasHint && UseTallSize)
                {
                    hintRect = new Rectangle(
                        _left_padding,
                        userTextPresent ? (HINT_TEXT_SMALL_Y) : ClientRectangle.Y + (int)((HINT_TEXT_SMALL_Y - ClientRectangle.Y) * animationProgress),
                        Width - _left_padding - _right_padding,
                        userTextPresent ? (HINT_TEXT_SMALL_SIZE) : (int)(LINE_Y + (HINT_TEXT_SMALL_SIZE - LINE_Y) * animationProgress));
                    hintTextSize = userTextPresent ? 12 : (int)(16 + (12 - 16) * animationProgress);
                }

                // Line Animation
                int LineAnimationWidth = (int)(Width * animationProgress);
                int LineAnimationX = (Width / 2) - (LineAnimationWidth / 2);
                g.FillRectangle(UseAccent ? SkinManager.ColorScheme.AccentBrush : SkinManager.ColorScheme.PrimaryBrush, LineAnimationX, LINE_Y, LineAnimationWidth, 2);
            }

            // Text stuff:
            string textToDisplay = Password ? Text.ToSecureString() : Text;
            string textSelected;
            Rectangle textSelectRect;

            // Calc text Rect
            Rectangle textRect = new Rectangle(
                hintRect.X,
                hasHint && UseTallSize ? (hintRect.Y + hintRect.Height) - 2 : ClientRectangle.Y,
                ClientRectangle.Width - _left_padding - _right_padding + scrollPos.X,
                hasHint && UseTallSize ? LINE_Y - (hintRect.Y + hintRect.Height) : LINE_Y);

            g.Clip = new Region(textRect);
            textRect.X -= scrollPos.X;

            using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
            {
                // Selection rects calc
                string textBeforeSelection = textToDisplay.Substring(0, SelectionStart);
                textSelected = textToDisplay.Substring(SelectionStart, SelectionLength);

                int selectX = NativeText.MeasureLogString(textBeforeSelection, SkinManager.getLogFontByType(MaterialSkinManager.fontType.Subtitle1)).Width;
                int selectWidth = NativeText.MeasureLogString(textSelected, SkinManager.getLogFontByType(MaterialSkinManager.fontType.Subtitle1)).Width;

                textSelectRect = new Rectangle(
                    textRect.X + selectX, UseTallSize ? hasHint ?
                     textRect.Y + BOTTOM_PADDING : // tall and hint
                     LINE_Y / 3 - BOTTOM_PADDING : // tall and no hint
                     BOTTOM_PADDING, // not tall
                    selectWidth,
                    UseTallSize ? hasHint ?
                    textRect.Height - BOTTOM_PADDING * 2 : // tall and hint
                    (int)(LINE_Y / 2) : // tall and no hint
                    LINE_Y - BOTTOM_PADDING * 2); // not tall

                // Draw user text
                NativeText.DrawTransparentText(
                    textToDisplay,
                    SkinManager.getLogFontByType(MaterialSkinManager.fontType.Subtitle1),
                    Enabled ? SkinManager.TextHighEmphasisColor : SkinManager.TextDisabledOrHintColor,
                    textRect.Location,
                    textRect.Size,
                    NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Middle);
            }

            if (Focused)
            {
                // Draw Selection Rectangle
                g.FillRectangle(UseAccent ? SkinManager.ColorScheme.AccentBrush : SkinManager.ColorScheme.DarkPrimaryBrush, textSelectRect);

                // Draw Selected Text
                using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
                {
                    NativeText.DrawTransparentText(
                        textSelected,
                        SkinManager.getLogFontByType(MaterialSkinManager.fontType.Subtitle1),
                        SkinManager.ColorScheme.TextColor,
                        textSelectRect.Location,
                        textSelectRect.Size,
                        NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Middle);
                }
            }

            g.Clip = new Region(ClientRectangle);

            // Draw hint text
            if (hasHint && (UseTallSize || String.IsNullOrEmpty(Text)))
            {
                using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
                {
                    NativeText.DrawTransparentText(
                    Hint,
                    SkinManager.getTextBoxFontBySize(hintTextSize),
                    Enabled ? Focused ? UseAccent ?
                    SkinManager.ColorScheme.AccentColor : // Focus Accent
                    SkinManager.ColorScheme.PrimaryColor : // Focus Primary
                    SkinManager.TextMediumEmphasisColor : // not focused
                    SkinManager.TextDisabledOrHintColor, // Disabled
                    hintRect.Location,
                    hintRect.Size,
                    NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Middle);
                }
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (DesignMode)
                return;

            if (_textfieldBounds.Contains(e.Location))
            {
                Cursor = Cursors.IBeam;
            }
            else if (LeadingIcon != null && _leadingIconBounds.Contains(e.Location) && LeadingIconClick != null)
            {
                Cursor = Cursors.Hand;
            }
            else if (TrailingIcon != null && _trailingIconBounds.Contains(e.Location) && TrailingIconClick != null)
            {
                Cursor = Cursors.Hand;
            }
            else
            {
                Cursor = Cursors.Default;
            }

        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (LeadingIcon != null && _leadingIconBounds.Contains(e.Location))
            {
                LeadingIconClick?.Invoke(this, new EventArgs());
            }
            else if (TrailingIcon != null && _trailingIconBounds.Contains(e.Location))
            {
                TrailingIconClick?.Invoke(this, new EventArgs());
            }
            else
            {
                if (DesignMode)
                    return;
            }
            base.OnMouseDown(e);
        }

        protected override void OnSelectionChanged(EventArgs e)
        {
            base.OnSelectionChanged(e);
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Size = new Size(Width, HEIGHT);
            LINE_Y = HEIGHT - BOTTOM_PADDING;

            if (DesignMode)
            {
                //Below code helps to redraw images in design mode only
                Image _tmpimage;
                _tmpimage = LeadingIcon;
                LeadingIcon = null;
                LeadingIcon = _tmpimage;
            }
        }

        private void ContextMenuStripOnItemClickStart(object sender, ToolStripItemClickedEventArgs toolStripItemClickedEventArgs)
        {
            switch (toolStripItemClickedEventArgs.ClickedItem.Text)
            {
                case "Cut":
                    Cut();
                    break;

                case "Copy":
                    Copy();
                    break;

                case "Paste":
                    Paste();
                    break;

                case "Delete":
                    SelectedText = string.Empty;
                    break;

                case "Select All":
                    SelectAll();
                    break;
            }
        }

        private void ContextMenuStripOnOpening(object sender, CancelEventArgs cancelEventArgs)
        {
            var strip = sender as TextBoxContextMenuStrip;
            if (strip != null)
            {
                strip.Cut.Enabled = !string.IsNullOrEmpty(SelectedText);
                strip.Copy.Enabled = !string.IsNullOrEmpty(SelectedText);
                strip.Paste.Enabled = Clipboard.ContainsText();
                strip.Delete.Enabled = !string.IsNullOrEmpty(SelectedText);
                strip.SelectAll.Enabled = !string.IsNullOrEmpty(Text);
            }
        }

        // Cursor flickering fix
        private const int WM_SETCURSOR = 0x0020;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_SETCURSOR)
                Cursor.Current = this.Cursor;
            else
                base.WndProc(ref m);
        }

        // Padding
        private const int EM_SETRECT = 0xB3;

        [DllImport(@"User32.dll", EntryPoint = @"SendMessage", CharSet = CharSet.Auto)]
        private static extern int SendMessageRefRect(IntPtr hWnd, uint msg, int wParam, ref RECT rect);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public readonly int Left;
            public readonly int Top;
            public readonly int Right;
            public readonly int Bottom;

            private RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public RECT(Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom)
            {
            }
        }
    }

    public class TextBoxContextMenuStrip : MaterialContextMenuStrip
    {
        public readonly ToolStripItem SelectAll = new MaterialToolStripMenuItem { Text = "Select All" };
        public readonly ToolStripItem Separator2 = new ToolStripSeparator();
        public readonly ToolStripItem Paste = new MaterialToolStripMenuItem { Text = "Paste" };
        public readonly ToolStripItem Copy = new MaterialToolStripMenuItem { Text = "Copy" };
        public readonly ToolStripItem Cut = new MaterialToolStripMenuItem { Text = "Cut" };
        public readonly ToolStripItem Delete = new MaterialToolStripMenuItem { Text = "Delete" };

        public TextBoxContextMenuStrip()
        {
            Items.AddRange(new[]
                {
                    Cut,
                    Copy,
                    Paste,
                    Delete,
                    Separator2,
                    SelectAll
                }
            );
        }
    }
}
