using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin.Animations;
using System.ComponentModel;

namespace MaterialSkin.Controls
{
    public class MaterialCheckedListBox : Panel, IMaterialControl

    {
        [Browsable(false)]
        public int Depth { get; set; }
        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;
        [Browsable(false)]
        public MouseState MouseState { get; set; }

        private ItemsList _itemsList;
        public bool Striped { get; set; }
        public Color StripeDarkColor { get; set; }
        public ItemsList Items { get; set; }

        public MaterialCheckedListBox() : base()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Items = new ItemsList(this);
            this.AutoScroll = true;
            return;
        }



        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ResumeLayout(false);
        }

        public class ItemsList : List<MaterialSkin.Controls.MaterialCheckBox>
        {
            private Panel _parent;


            public ItemsList(Panel parent)
            {
                _parent = parent;
            }

            public event SelectedIndexChangedEventHandler SelectedIndexChanged;

            public delegate void SelectedIndexChangedEventHandler(int Index);

            public new void Add(string text)
            {
                Add(text, false);
            }

            public void Add(string text, bool @checked)
            {
                MaterialSkin.Controls.MaterialCheckBox cb = new MaterialSkin.Controls.MaterialCheckBox();
                cb.Checked = @checked;
                cb.Text = text;
                // cb.Width = _parent.Width * 0.75
                Add(cb);
            }

            public  void Add(MaterialSkin.Controls.MaterialCheckBox value)
            {
                value.Dock = DockStyle.Bottom;
                base.Add(value);
                _parent.Controls.Add(value);
            }


            public  void Remove(MaterialSkin.Controls.MaterialCheckBox value)
            {
                base.Remove(value);
                _parent.Controls.Remove(value);
            }
        }

    }
}