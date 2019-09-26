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
                Refresh();
            }
        }

        private const int TEXT_SPACING = 10;
        private const int TEXT_SMALL_SIZE = 18;
        private const int TEXT_SMALL_Y = 8;
        private const int HEIGHT = 60;

        private readonly AnimationManager _animationManager;

        public MaterialTextBox()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            base.AutoSize = false;
            Multiline = false;

            _animationManager = new AnimationManager
            {
                Increment = 0.08,
                AnimationType = AnimationType.EaseInOut
            };
            _animationManager.OnAnimationProgress += sender => Invalidate();

            BorderStyle = BorderStyle.None;
            Font = SkinManager.getFontByType(MaterialSkinManager.fontType.Body1);

            GotFocus += (sender, args) => _animationManager.StartNewAnimation(AnimationDirection.In);
            LostFocus += (sender, args) => _animationManager.StartNewAnimation(AnimationDirection.Out);

            MaterialContextMenuStrip cms = new TextBoxContextMenuStrip();
            cms.Opening += ContextMenuStripOnOpening;
            cms.OnItemClickStart += ContextMenuStripOnItemClickStart;

            ContextMenuStrip = cms;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            Size = new Size(Size.Width, HEIGHT);
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            return new Size(proposedSize.Width, HEIGHT);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            var g = pevent.Graphics;
            var lineY = ClientRectangle.Height - 3;

            g.Clear(Parent.BackColor);
            g.FillRectangle(Focused ? SkinManager.GetButtonPressedBackgroundBrush() : new SolidBrush(SkinManager.GetControlBackgroundColor()), ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, lineY);


            // HintText
            bool userTextPresent = !String.IsNullOrEmpty(Text);
            Color textColor = Enabled ? Focused ?
                            SkinManager.ColorScheme.PrimaryColor : // Focused
                            SkinManager.GetPrimaryTextColor() : // Inactive
                            SkinManager.GetDisabledOrHintColor(); // Disabled
            Rectangle hintRect = ClientRectangle;
            int textSize = 16;

            // bottom line base
            g.FillRectangle(SkinManager.GetDividersBrush(), 0, lineY, Width, 1);
            g.FillRectangle(SkinManager.GetDividersBrush(), 0, lineY, Width, 1);

            if (!_animationManager.IsAnimating())
            {
                // No animation
                // hint text
                hintRect = new Rectangle(TEXT_SPACING, Focused || userTextPresent ? TEXT_SMALL_Y : ClientRectangle.Y, Width, userTextPresent || Focused ? TEXT_SMALL_SIZE : lineY);
                textSize = userTextPresent || Focused ? 12 : 16;

                // bottom line
                if (Focused)
                {
                    g.FillRectangle(SkinManager.ColorScheme.PrimaryBrush, 0, lineY, Width, 2);
                }
            }
            else
            {
                // Animate - Focus got/lost
                double animationProgress = _animationManager.GetProgress();

                // hint Animation
                hintRect = new Rectangle(
                    TEXT_SPACING,
                    userTextPresent ? (TEXT_SMALL_Y) : ClientRectangle.Y + (int)((TEXT_SMALL_Y - ClientRectangle.Y) * animationProgress),
                    Width,
                    userTextPresent ? (TEXT_SMALL_SIZE) : (int)(lineY + (TEXT_SMALL_SIZE - lineY) * animationProgress));
                textSize = userTextPresent ? 12 : (int)(16 + (12 - 16) * animationProgress);

                // Line Animation
                int LineAnimationWidth = (int)(Width * animationProgress);
                int LineAnimationX = (Width / 2) - (LineAnimationWidth / 2);
                g.FillRectangle(SkinManager.ColorScheme.PrimaryBrush, LineAnimationX, lineY, LineAnimationWidth, 2);
            }

            // Draw hint text
            using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
            {
                NativeText.DrawTransparentText(
                    Hint,
                    SkinManager.getTextBoxFontBySize(textSize),
                    textColor,
                    hintRect.Location,
                    hintRect.Size,
                    NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Middle);
            }



            Rectangle textRect = new Rectangle(
                hintRect.X,
                (hintRect.Y + hintRect.Height) - 2,
                hintRect.Width - hintRect.X * 2,
                lineY - (hintRect.Y + hintRect.Height));

            // Draw user text
            using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
            {
                NativeText.DrawTransparentText(
                    Password ? Text.ToSecureString() : Text,
                    SkinManager.getLogFontByType(MaterialSkinManager.fontType.Body1),
                    SkinManager.GetPrimaryTextColor(),
                    textRect.Location,
                    textRect.Size,
                    NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Middle);
            }


        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            Refresh();
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
                case "Undo":
                    Undo();
                    break;
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
                strip.Undo.Enabled = CanUndo;
                strip.Cut.Enabled = !string.IsNullOrEmpty(SelectedText);
                strip.Copy.Enabled = !string.IsNullOrEmpty(SelectedText);
                strip.Paste.Enabled = Clipboard.ContainsText();
                strip.Delete.Enabled = !string.IsNullOrEmpty(SelectedText);
                strip.SelectAll.Enabled = !string.IsNullOrEmpty(Text);
            }
        }


        // Hide IBeam
        [DllImport("user32.dll", EntryPoint = "HideCaret")]
        public static extern long HideCaret(IntPtr hwnd);

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            HideCaret(this.Handle);
        }
    }

        public class TextBoxContextMenuStrip : MaterialContextMenuStrip
    {
        public readonly ToolStripItem Undo = new MaterialToolStripMenuItem { Text = "Undo" };
        public readonly ToolStripItem Seperator1 = new ToolStripSeparator();
        public readonly ToolStripItem Cut = new MaterialToolStripMenuItem { Text = "Cut" };
        public readonly ToolStripItem Copy = new MaterialToolStripMenuItem { Text = "Copy" };
        public readonly ToolStripItem Paste = new MaterialToolStripMenuItem { Text = "Paste" };
        public readonly ToolStripItem Delete = new MaterialToolStripMenuItem { Text = "Delete" };
        public readonly ToolStripItem Seperator2 = new ToolStripSeparator();
        public readonly ToolStripItem SelectAll = new MaterialToolStripMenuItem { Text = "Select All" };

        public TextBoxContextMenuStrip()
        {
            Items.AddRange(new[]
            {
                    Undo,
                    Seperator1,
                    Cut,
                    Copy,
                    Paste,
                    Delete,
                    Seperator2,
                    SelectAll
                });
        }
    }
}
