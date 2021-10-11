namespace MaterialSkin.Controls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    [ToolboxItem(false)]
    public class BaseTextBox : TextBox, IMaterialControl
    {
        #region "Public Properties"

        //Properties for managing the material design properties
        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        [Browsable(false)]
        public MouseState MouseState { get; set; }

        private string hint = string.Empty;
        public string Hint
        {
            get { return hint; }
            set
            {
                hint = value;
                Invalidate();
            }
        }

        public new void SelectAll()
        {
            BeginInvoke((MethodInvoker)delegate ()
            {
                base.Focus();
                base.SelectAll();
            });
        }

        #endregion

        public BaseTextBox()
        {
        }


        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate();
        }

        private const int WM_ENABLE = 0x0A;
        private const int WM_PAINT = 0xF;
        private const UInt32 WM_USER = 0x0400;
        private const UInt32 EM_SETBKGNDCOLOR = (WM_USER + 67);
        private const UInt32 WM_KILLFOCUS = 0x0008;
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);


            if (m.Msg == WM_PAINT)
            {
                if (m.Msg == WM_ENABLE)
                {
                    Graphics g = Graphics.FromHwnd(Handle);
                    Rectangle bounds = new Rectangle(0, 0, Width, Height);
                    g.FillRectangle(SkinManager.BackgroundDisabledBrush, bounds);
                }
            }

            if (m.Msg == WM_PAINT && String.IsNullOrEmpty(Text) && !Focused)
            {
                using (NativeTextRenderer NativeText = new NativeTextRenderer(Graphics.FromHwnd(m.HWnd)))
                {
                    NativeText.DrawTransparentText(
                    Hint,
                    SkinManager.getFontByType(MaterialSkinManager.fontType.Subtitle1),
                    !this.ReadOnly ?
                    ColorHelper.RemoveAlpha(SkinManager.TextMediumEmphasisColor, BackColor) : // not focused
                    ColorHelper.RemoveAlpha(SkinManager.TextDisabledOrHintColor, BackColor), // Disabled
                    ClientRectangle.Location,
                    ClientRectangle.Size,
                    NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Top);
                }
            }

            if (m.Msg == EM_SETBKGNDCOLOR)
            {
                Invalidate();
            }

            if (m.Msg == WM_KILLFOCUS) //set border back to normal on lost focus
            {
                Invalidate();
            }

        }

    }

    [ToolboxItem(false)]
    public class BaseMaskedTextBox : MaskedTextBox, IMaterialControl
    {
        //Properties for managing the material design properties
        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        [Browsable(false)]
        public MouseState MouseState { get; set; }

        private string hint = string.Empty;
        public string Hint
        {
            get { return hint; }
            set
            {
                hint = value;
                Invalidate();
            }
        }

        public new void SelectAll()
        {
            BeginInvoke((MethodInvoker)delegate ()
            {
                base.Focus();
                base.SelectAll();
            });
        }


        public BaseMaskedTextBox()
        {
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate();
        }

        private const int WM_ENABLE = 0x0A;
        private const int WM_PAINT = 0xF;
        private const UInt32 WM_USER = 0x0400;
        private const UInt32 EM_SETBKGNDCOLOR = (WM_USER + 67);
        private const UInt32 WM_KILLFOCUS = 0x0008;
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);


            if (m.Msg == WM_PAINT)
            {
                if (m.Msg == WM_ENABLE)
                {
                    Graphics g = Graphics.FromHwnd(Handle);
                    Rectangle bounds = new Rectangle(0, 0, Width, Height);
                    g.FillRectangle(SkinManager.BackgroundDisabledBrush, bounds);
                }
            }

            if (m.Msg == WM_PAINT && String.IsNullOrEmpty(Text) && !Focused)
            {
                using (NativeTextRenderer NativeText = new NativeTextRenderer(Graphics.FromHwnd(m.HWnd)))
                {
                    NativeText.DrawTransparentText(
                    Hint,
                    SkinManager.getFontByType(MaterialSkinManager.fontType.Subtitle1),
                    !this.ReadOnly ?
                    ColorHelper.RemoveAlpha(SkinManager.TextMediumEmphasisColor, BackColor) : // not focused
                    ColorHelper.RemoveAlpha(SkinManager.TextDisabledOrHintColor, BackColor), // Disabled
                    ClientRectangle.Location,
                    ClientRectangle.Size,
                    NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Top);
                }
            }

            if (m.Msg == EM_SETBKGNDCOLOR)
            {
                Invalidate();
            }

            if (m.Msg == WM_KILLFOCUS) //set border back to normal on lost focus
            {
                Invalidate();
            }

        }
    }

    [ToolboxItem(false)]
    public class BaseTextBoxContextMenuStrip : MaterialContextMenuStrip
    {
        public readonly ToolStripItem undo = new MaterialToolStripMenuItem { Text = "Undo" };
        public readonly ToolStripItem seperator1 = new ToolStripSeparator();
        public readonly ToolStripItem cut = new MaterialToolStripMenuItem { Text = "Cut" };
        public readonly ToolStripItem copy = new MaterialToolStripMenuItem { Text = "Copy" };
        public readonly ToolStripItem paste = new MaterialToolStripMenuItem { Text = "Paste" };
        public readonly ToolStripItem delete = new MaterialToolStripMenuItem { Text = "Delete" };
        public readonly ToolStripItem seperator2 = new ToolStripSeparator();
        public readonly ToolStripItem selectAll = new MaterialToolStripMenuItem { Text = "Select All" };

        public BaseTextBoxContextMenuStrip()
        {
            Items.AddRange(new[]
            {
                    undo,
                    seperator1,
                    cut,
                    copy,
                    paste,
                    delete,
                    seperator2,
                    selectAll
                });
        }
    }

}
