namespace MaterialSkin.Controls
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class MaterialCheckedListBox : Panel, IMaterialControl
    {
        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        [Browsable(false)]
        public MouseState MouseState { get; set; }
       

        public bool Striped { get; set; }

        public Color StripeDarkColor { get; set; }

        public ItemsList Items { get; set; }

        public MaterialCheckedListBox() : base()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Items = new ItemsList(this);
            this.AutoScroll = true;
            BackColorChanged += (sender, args) => BackColor = SkinManager.GetControlBackgroundColor();
            ForeColorChanged += (sender, args) => ForeColor = SkinManager.GetPrimaryTextColor();
            
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ResumeLayout(false);
        }

        public CheckState GetItemCheckState(int Index)
        {
            return Items[Index].CheckState;
        }

        public class ItemsList : List<MaterialSkin.Controls.MaterialCheckbox>
        {
            private Panel _parent;

            public ItemsList(Panel parent)
            {
                _parent = parent;
            }

            public delegate void SelectedIndexChangedEventHandler(int Index);

            public void Add(string text)
            {
                Add(text, false);
            }

            public void Add(string text, bool @checked)
            {
                MaterialSkin.Controls.MaterialCheckbox cb = new MaterialSkin.Controls.MaterialCheckbox();
                cb.Checked = @checked;
                cb.Text = text;
                // cb.Width = _parent.Width * 0.75
                Add(cb);
            }

             public new void Add(MaterialSkin.Controls.MaterialCheckbox value)
            {
                value.Dock = DockStyle.Top;
                base.Add(value);
                _parent.Controls.Add(value);
            }

 public new void Remove(MaterialSkin.Controls.MaterialCheckbox value)
            {
                base.Remove(value);
                _parent.Controls.Remove(value);
            }
        }
    }
}
