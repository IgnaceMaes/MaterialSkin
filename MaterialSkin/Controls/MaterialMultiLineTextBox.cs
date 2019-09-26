﻿namespace MaterialSkin.Controls
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    /// <summary>
    /// Defines the <see cref="MaterialMultiLineTextBox" />
    /// </summary>
    public class MaterialMultiLineTextBox : RichTextBox , IMaterialControl
    {
        //Properties for managing the material design properties
        /// <summary>
        /// Gets or sets the Depth
        /// </summary>
        [Browsable(false)]
        public int Depth { get; set; }

        /// <summary>
        /// Gets the SkinManager
        /// </summary>
        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        /// <summary>
        /// Gets or sets the MouseState
        /// </summary>
        [Browsable(false)]
        public MouseState MouseState { get; set; }

        /// <summary>
        /// The SendMessage
        /// </summary>
        /// <param name="hWnd">The hWnd<see cref="IntPtr"/></param>
        /// <param name="msg">The msg<see cref="int"/></param>
        /// <param name="wParam">The wParam<see cref="int"/></param>
        /// <param name="lParam">The lParam<see cref="string"/></param>
        /// <returns>The <see cref="IntPtr"/></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);

        /// <summary>
        /// Defines the EM_SETCUEBANNER
        /// </summary>
        private const int EM_SETCUEBANNER = 0x1501;

        /// <summary>
        /// Defines the EmptyChar
        /// </summary>
        private const char EmptyChar = (char)0;

        /// <summary>
        /// Defines the VisualStylePasswordChar
        /// </summary>
        private const char VisualStylePasswordChar = '\u25CF';

        /// <summary>
        /// Defines the NonVisualStylePasswordChar
        /// </summary>
        private const char NonVisualStylePasswordChar = '\u002A';

        /// <summary>
        /// Defines the hint
        /// </summary>
        private string hint = string.Empty;

        /// <summary>
        /// Gets or sets the Hint
        /// </summary>
        public string Hint
        {
            get { return hint; }
            set
            {
                hint = value;
                SendMessage(Handle, EM_SETCUEBANNER, (int)IntPtr.Zero, Hint);
            }
        }

      
       

        /// <summary>
        /// The SelectAll
        /// </summary>
        public new void SelectAll()
        {
            BeginInvoke((MethodInvoker)delegate ()
            {
                base.Focus();
                base.SelectAll();
            });
        }

        /// <summary>
        /// The Focus
        /// </summary>
        public new void Focus()
        {
            BeginInvoke((MethodInvoker)delegate ()
            {
                base.Focus();
            });
        }

       

              
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialMultiLineTextBox"/> class.
        /// </summary>
        public MaterialMultiLineTextBox()
        {

            base.OnCreateControl();

            MaterialContextMenuStrip cms = new MaterialTextBox.TextBoxContextMenuStrip();
            cms.Opening += ContextMenuStripOnOpening;
            cms.OnItemClickStart += ContextMenuStripOnItemClickStart;
            this.Multiline = true;

            ContextMenuStrip = cms;
            BorderStyle = BorderStyle.None;
            Font = SkinManager.ROBOTO_REGULAR_11;
            BackColor = SkinManager.GetControlBackgroundColor();
            ForeColor = SkinManager.GetPrimaryTextColor();
            BackColorChanged += (sender, args) => BackColor = SkinManager.GetControlBackgroundColor();
            ForeColorChanged += (sender, args) => ForeColor = SkinManager.GetPrimaryTextColor();
        }

        /// <summary>
        /// The ContextMenuStripOnItemClickStart
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="toolStripItemClickedEventArgs">The toolStripItemClickedEventArgs<see cref="ToolStripItemClickedEventArgs"/></param>
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

        /// <summary>
        /// The ContextMenuStripOnOpening
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="cancelEventArgs">The cancelEventArgs<see cref="CancelEventArgs"/></param>
        private void ContextMenuStripOnOpening(object sender, CancelEventArgs cancelEventArgs)
        {
            var strip = sender as MaterialTextBox.TextBoxContextMenuStrip;
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
    }
}
