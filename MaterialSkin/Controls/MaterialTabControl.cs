﻿namespace MaterialSkin.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    public class MaterialTabControl : TabControl, IMaterialControl
    {
        public MaterialTabControl()
        {
            Multiline = true;
        }

        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        [Browsable(false)]
        public MouseState MouseState { get; set; }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x1328 && !DesignMode) m.Result = (IntPtr)1;
            else base.WndProc(ref m);
        }
        
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            e.Control.BackColor = System.Drawing.Color.White;
        }
    }
}
