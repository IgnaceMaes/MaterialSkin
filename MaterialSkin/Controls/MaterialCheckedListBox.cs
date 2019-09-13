namespace MaterialSkin.Controls
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// Defines the <see cref="MaterialCheckedListBox" />
    /// </summary>
    public class MaterialCheckedListBox : Panel, IMaterialControl
    {
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
        /// Gets or sets a value indicating whether Striped
        /// </summary>
        public bool Striped { get; set; }

        /// <summary>
        /// Gets or sets the StripeDarkColor
        /// </summary>
        public Color StripeDarkColor { get; set; }

        /// <summary>
        /// Gets or sets the Items
        /// </summary>
        public ItemsList Items { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialCheckedListBox"/> class.
        /// </summary>
        public MaterialCheckedListBox() : base()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Items = new ItemsList(this);
            this.AutoScroll = true;
            BackColorChanged += (sender, args) => BackColor = SkinManager.GetControlBackgroundColor();
            ForeColorChanged += (sender, args) => ForeColor = SkinManager.GetPrimaryTextColor();
            
        }

        /// <summary>
        /// The InitializeComponent
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ResumeLayout(false);
        }

        /// <summary>
        /// The GetItemCheckState
        /// </summary>
        /// <param name="Index">The Index<see cref="int"/></param>
        /// <returns>The <see cref="CheckState"/></returns>
        public CheckState GetItemCheckState(int Index)
        {
            return Items[Index].CheckState;
        }

        /// <summary>
        /// Defines the <see cref="ItemsList" />
        /// </summary>
        public class ItemsList : List<MaterialSkin.Controls.MaterialCheckbox>
        {
            /// <summary>
            /// Defines the _parent
            /// </summary>
            private Panel _parent;

            /// <summary>
            /// Initializes a new instance of the <see cref="ItemsList"/> class.
            /// </summary>
            /// <param name="parent">The parent<see cref="Panel"/></param>
            public ItemsList(Panel parent)
            {
                _parent = parent;
            }

            /// <summary>
            /// Defines the SelectedIndexChanged
            /// </summary>
            public event SelectedIndexChangedEventHandler SelectedIndexChanged;

            /// <summary>
            /// The SelectedIndexChangedEventHandler
            /// </summary>
            /// <param name="Index">The Index<see cref="int"/></param>
            public delegate void SelectedIndexChangedEventHandler(int Index);

            /// <summary>
            /// Add a new checkbox item by string only , defaults checked state to false
            /// </summary>
            /// <param name="text">The text<see cref="string"/></param>
            public void Add(string text)
            {
                Add(text, false);
            }

            /// <summary>
            /// Create a new checkbox item based on the string value passed, checked state defaults to false
            /// </summary>
            /// <param name="text">The text<see cref="string"/></param>
            /// <param name="@checked">The checked<see cref="bool"/></param>
            public void Add(string text, bool @checked)
            {
                MaterialSkin.Controls.MaterialCheckbox cb = new MaterialSkin.Controls.MaterialCheckbox();
                cb.Checked = @checked;
                cb.Text = text;
                // cb.Width = _parent.Width * 0.75
                Add(cb);
            }

            /// <summary>
            /// Add a new checkbox item to the list
            /// </summary>
            /// <param name="value">The value<see cref="MaterialSkin.Controls.MaterialCheckbox"/></param>
            public new void Add(MaterialSkin.Controls.MaterialCheckbox value)
            {
                value.Dock = DockStyle.Top;
                base.Add(value);
                _parent.Controls.Add(value);
            }

            /// <summary>
            /// Remove a checkedbox item control from the list
            /// </summary>
            /// <param name="value">The value<see cref="MaterialSkin.Controls.MaterialCheckbox"/></param>
            public new void Remove(MaterialSkin.Controls.MaterialCheckbox value)
            {
                base.Remove(value);
                _parent.Controls.Remove(value);
            }
        }
    }
}
