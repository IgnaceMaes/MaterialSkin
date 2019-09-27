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

        [Category("Material Skin")]
        public bool Password { get; set; }

        private string _hint = string.Empty;
        [Category("Material Skin")]
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

        private const int TEXT_MARGIN = 10;
        private const int TEXT_SMALL_SIZE = 18;
        private const int TEXT_SMALL_Y = 4;
        private const int HEIGHT = 50;
        private const int BOTTOM_PADDING = 3;
        private const int LINE_Y = HEIGHT - BOTTOM_PADDING;

        private bool hasHint;

        private readonly AnimationManager _animationManager;

        public MaterialTextBox()
        {
            Font = new Font(SkinManager.getFontByType(MaterialSkinManager.fontType.Body2).FontFamily, 12f, FontStyle.Regular);

            Cursor = Cursors.IBeam;

            // Properties
            base.AutoSize = false;
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

            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            if (Password) SendMessage(Handle, EM_SETPASSWORDCHAR, 'T', 0);

            Font = new Font(SkinManager.getFontByType(MaterialSkinManager.fontType.Body2).FontFamily, 12f, FontStyle.Regular);

            // Size and padding
            Size = new Size(Size.Width, HEIGHT);

            var rect = new Rectangle(TEXT_MARGIN, hasHint ? (TEXT_SMALL_Y + TEXT_SMALL_SIZE) : LINE_Y / 3, ClientSize.Width - (TEXT_MARGIN * 2), LINE_Y);
            RECT rc = new RECT(rect);
            SendMessageRefRect(Handle, EM_SETRECT, 0, ref rc);

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

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            var g = pevent.Graphics;

            g.Clear(Parent.BackColor);
            g.FillRectangle(Focused ?
                SkinManager.GetButtonPressedBackgroundBrush() :  // Focused
                MouseState == MouseState.HOVER ? SkinManager.GetButtonHoverBackgroundBrush() : // Hover
                new SolidBrush(SkinManager.GetControlBackgroundColor()), // Normal
                ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, LINE_Y);


            // HintText
            bool userTextPresent = !String.IsNullOrEmpty(Text);
            Color textColor = Enabled ? Focused ?
                            SkinManager.ColorScheme.PrimaryColor : // Focused
                            SkinManager.GetPrimaryTextColor() : // Inactive
                            SkinManager.GetDisabledOrHintColor(); // Disabled
            Rectangle hintRect = ClientRectangle;
            int hintTextSize = 16;

            // bottom line base
            g.FillRectangle(SkinManager.GetDividersBrush(), 0, LINE_Y, Width, 1);
            g.FillRectangle(SkinManager.GetDividersBrush(), 0, LINE_Y, Width, 1);

            if (!_animationManager.IsAnimating())
            {
                // No animation

                if (hasHint)
                {
                    // hint text
                    hintRect = new Rectangle(TEXT_MARGIN, Focused || userTextPresent ? TEXT_SMALL_Y : ClientRectangle.Y, Width, userTextPresent || Focused ? TEXT_SMALL_SIZE : LINE_Y);
                    hintTextSize = userTextPresent || Focused ? 12 : 16;
                }

                // bottom line
                if (Focused)
                {
                    g.FillRectangle(SkinManager.ColorScheme.PrimaryBrush, 0, LINE_Y, Width, 2);
                }
            }
            else
            {
                // Animate - Focus got/lost
                double animationProgress = _animationManager.GetProgress();

                // hint Animation
                if (hasHint)
                {
                    hintRect = new Rectangle(
                        TEXT_MARGIN,
                        userTextPresent ? (TEXT_SMALL_Y) : ClientRectangle.Y + (int)((TEXT_SMALL_Y - ClientRectangle.Y) * animationProgress),
                        Width,
                        userTextPresent ? (TEXT_SMALL_SIZE) : (int)(LINE_Y + (TEXT_SMALL_SIZE - LINE_Y) * animationProgress));
                    hintTextSize = userTextPresent ? 12 : (int)(16 + (12 - 16) * animationProgress);
                }

                // Line Animation
                int LineAnimationWidth = (int)(Width * animationProgress);
                int LineAnimationX = (Width / 2) - (LineAnimationWidth / 2);
                g.FillRectangle(SkinManager.ColorScheme.PrimaryBrush, LineAnimationX, LINE_Y, LineAnimationWidth, 2);
            }

            // Text stuff:
            string textToDisplay = Password ? Text.ToSecureString() : Text;
            string textSelected;
            Rectangle textSelectRect;

            // Calc text Rect
            Rectangle textRect = new Rectangle(
                TEXT_MARGIN,
                hasHint ? (hintRect.Y + hintRect.Height) - 2 : ClientRectangle.Y,
                ClientRectangle.Width - TEXT_MARGIN * 2 + scrollPos.X,
                hasHint ? LINE_Y - (hintRect.Y + hintRect.Height) : LINE_Y);

            g.Clip = new Region(textRect);
            textRect.X -= scrollPos.X;

            using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
            {
                // Selection rects calc
                string textBeforeSelection = textToDisplay.Substring(0, SelectionStart);
                textSelected = textToDisplay.Substring(SelectionStart, SelectionLength);

                int selectX = NativeText.MeasureLogString(textBeforeSelection, SkinManager.getLogFontByType(MaterialSkinManager.fontType.Body1)).Width;
                int selectWidth = NativeText.MeasureLogString(textSelected, SkinManager.getLogFontByType(MaterialSkinManager.fontType.Body1)).Width;

                textSelectRect = new Rectangle(
                    textRect.X + selectX,
                    hasHint ? textRect.Y + BOTTOM_PADDING : LINE_Y / 3 - BOTTOM_PADDING,
                    selectWidth,
                    hasHint ? textRect.Height - BOTTOM_PADDING * 2 : LINE_Y / 2);

                // Draw user text
                NativeText.DrawTransparentText(
                    textToDisplay,
                    SkinManager.getLogFontByType(MaterialSkinManager.fontType.Body1),
                    SkinManager.GetPrimaryTextColor(),
                    textRect.Location,
                    textRect.Size,
                    NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Middle);
            }

            if (Focused)
            {
                // Draw Selection Rectangle
                g.FillRectangle(SkinManager.ColorScheme.DarkPrimaryBrush, textSelectRect);

                // Draw Selected Text
                using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
                {
                    NativeText.DrawTransparentText(
                        textSelected,
                        SkinManager.getLogFontByType(MaterialSkinManager.fontType.Body1),
                        SkinManager.ColorScheme.TextColor,
                        textSelectRect.Location,
                        textSelectRect.Size,
                        NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Middle);
                }
            }

            g.Clip = new Region(ClientRectangle);

            // Draw hint text
            if (hasHint)
            {
                using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
                {
                    NativeText.DrawTransparentText(
                    Hint,
                    SkinManager.getTextBoxFontBySize(hintTextSize),
                    textColor,
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

        protected override void OnSelectionChanged(EventArgs e)
        {
            base.OnSelectionChanged(e);
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Size = new Size(Width, HEIGHT);
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
        const int WM_SETCURSOR = 0x0020;
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

            public RECT(Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom) { }
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
