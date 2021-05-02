#region Imports

using System;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Collections.Specialized;
using System.Runtime.InteropServices;
//using System.Collections;
//using System.Windows.Forms.Design;
//using System.ComponentModel.Design;

#endregion

namespace MaterialSkin.Controls
{
    #region MaterialListBoxDesignerDesign

    //internal class MaterialListBoxDesigner : ControlDesigner
    //{
    //    private readonly string[] _propertiesToRemove =
    //    {
    //        "BackgroundImage", "BackgroundImageLayout",
    //        "RightToLeft","ImeMode"
    //    };

    //    protected override void PostFilterProperties(IDictionary properties)
    //    {
    //        foreach (string property in _propertiesToRemove)
    //        {
    //            properties.Remove(property);
    //        }

    //        base.PostFilterProperties(properties);
    //    }

    //    private DesignerActionListCollection _actionListCollection;

    //    public override DesignerActionListCollection ActionLists => _actionListCollection ?? (_actionListCollection = new DesignerActionListCollection { new MaterialListBoxActionList(Component) });
    //}
    #endregion

    #region MaterialListBox

    //[ToolboxItem(true)]
    //[ToolboxBitmap(typeof(MetroListBox), "Bitmaps.ListBox.bmp")]
    //[Designer(typeof(MaterialListBoxDesigner))]
    [DefaultProperty("Items")]
    [DefaultEvent("SelectedIndexChanged")]
    [ComVisible(true)]
    public class MaterialListBox : Control, IMaterialControl
    {
        #region Interfaces

        //[Category("Metro"), Description("Gets or sets the style associated with the control.")]
        //public Style Style
        //{
        //    get => StyleManager?.Style ?? _style;
        //    set
        //    {
        //        _style = value;
        //        switch (value)
        //        {
        //            case Style.Light:
        //                ApplyTheme();
        //                break;
        //            case Style.Dark:
        //                ApplyTheme(Style.Dark);
        //                break;
        //            case Style.Custom:
        //                ApplyTheme(Style.Custom);
        //                break;
        //            default:
        //                ApplyTheme();
        //                break;
        //        }
        //        _scrollBar.Style = value;
        //        Invalidate();
        //    }
        //}

        //[Category("Metro"), Description("Gets or sets the Style Manager associated with the control.")]
        //public MetroStyleManager StyleManager
        //{
        //    get => _styleManager;
        //    set
        //    {
        //        _styleManager = value;
        //        Invalidate();
        //    }
        //}

        //[Category("Metro"), Description("Gets or sets the The Author name associated with the theme.")]
        //public string ThemeAuthor { get; set; }

        //[Category("Metro"), Description("Gets or sets the The Theme name associated with the theme.")]
        //public string ThemeName { get; set; }

        #endregion Interfaces

        #region Internal Vars

        private ObservableCollection<MaterialListBoxItem> _items = new ObservableCollection<MaterialListBoxItem>();
        private List<object> _selectedItems;
        private List<object> _indicates;
        private bool _multiSelect;
        private int _selectedIndex;
        private MaterialListBoxItem _selectedItem;
        private string _selectedText;
        private bool _showScrollBar;
        private bool _multiKeyDown;
        private int _hoveredItem;
        private MaterialScrollBar _scrollBar;
        private object _selectedValue;

        private bool _updating=false;
        private int _itemHeight;
        private bool _showBorder;
        private Color _borderColor;
        private Font _primaryFont;
        private Font _secondaryFont;

        private const int _leftrightPadding = 16;
        private int _primaryTextBottomPadding = 0;
        private int _secondaryTextTopPadding = 0;
        private int _secondaryTextBottomPadding = 0;

        public enum ListBoxStyle
        {
            SingleLine,
            TwoLine,
            ThreeLine
        }
        private ListBoxStyle _style = ListBoxStyle.SingleLine;

        public enum MaterialItemDensity
        {
            Default,
            Dense
        }

        private MaterialItemDensity _density;

        #endregion Internal Vars


        #region Properties

        //Properties for managing the material design properties
        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        [Browsable(false)]
        public MouseState MouseState { get; set; }

        //private bool highEmphasis;

        //[Category("Material Skin")]
        ///// <summary>
        ///// Gets or sets a value indicating whether HighEmphasis
        ///// </summary>
        //public bool HighEmphasis
        //{
        //    get { return highEmphasis; }
        //    set { highEmphasis = value; Invalidate(); }
        //}

        private bool useAccentColor;

        [Category("Material Skin")]
        public bool UseAccentColor
        {
            get { return useAccentColor; }
            set { useAccentColor = value; Invalidate(); }
        }

        //[Editor(typeof(CustomCollectionEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(CollectionConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        //[Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
        [Editor(typeof(MaterialItemCollectionEditor), typeof(UITypeEditor))]
        [Category("Material Skin"), Description("Gets the items of the ListBox.")]
        public ObservableCollection<MaterialListBoxItem> Items => _items;
        //public MaterialItemCollection<MaterialListBoxItem> Items => _items;
        //public MaterialItemCollection Items => _items;

        [Browsable(false)]
        [Category("Material Skin"), Description("Gets a collection containing the currently selected items in the ListBox.")]
        public List<object> SelectedItems => _selectedItems;

        [Browsable(false), Category("Material Skin"), Description("Gets or sets the currently selected item in the ListBox.")]
        public MaterialListBoxItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                _selectedIndex = _items.IndexOf(_selectedItem);
                update_selection();
                Invalidate();
            }
        }

        [Browsable(false), Category("Material Skin"),
         Description("Gets the currently selected Text in the ListBox.")]
        public string SelectedText
        {
            get => _selectedText;
            //set
            //{
            //    _selectedText = value;
            //    Invalidate();
            //}
        }

        [Browsable(false), Category("Material Skin"), Description("Gets or sets the zero-based index of the currently selected item in a ListBox.")]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                update_selection();
                Invalidate();
            }
        }

        [Browsable(true), Category("Material Skin"), Description("Gets the value of the member property specified by the ValueMember property.")]
        public object SelectedValue
        {
            get => _selectedValue;
            set
            {
                _selectedValue = value;
                Invalidate();
            }
        }

        [Category("Material Skin"), Description("Gets or sets a value indicating whether the ListBox supports multiple rows.")]
        public bool MultiSelect
        {
            get => _multiSelect;
            set
            {
                _multiSelect = value;

                if (_selectedItems.Count > 1)
                {
                    _selectedItems.RemoveRange(1, _selectedItems.Count - 1);
                }

                Invalidate();
            }
        }

        [Browsable(false)]
        public int Count => _items.Count;

        [Category("Material Skin"), Description("Gets or sets a value indicating whether the vertical scroll bar be shown or not.")]
        public bool ShowScrollBar
        {
            get => _showScrollBar;
            set
            {
                _showScrollBar = value;
                _scrollBar.Visible = value;
                Invalidate();
            }
        }

        [Category("Material Skin"), Description("Gets or sets a value indicating whether the border shown or not.")]
        public bool ShowBorder
        {
            get => _showBorder;
            set
            {
                _showBorder = value;
                Refresh();
            }
        }

        [Category("Material Skin"), Description("Gets or sets backcolor used by the control.")]
        public override Color BackColor { get; set; }

        [Category("Material Skin"), Description("Gets or sets forecolor used by the control.")]
        public override Color ForeColor { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string Text { get => base.Text; set => base.Text = value; }

        //[Category("Material Skin"), Description("Gets or sets selected item used by the control.")]
        //public Color SelectedItemColor
        //{
        //    get => _selectedItemColor;
        //    set
        //    {
        //        _selectedItemColor = value;
        //        Refresh();
        //    }
        //}

        //[Category("Material Skin"), Description("Gets or sets selected item backcolor used by the control.")]
        //public Color SelectedItemBackColor
        //{
        //    get => _selectedItemBackColor;
        //    set
        //    {
        //        _selectedItemBackColor = value;
        //        Refresh();
        //    }
        //}

        //[Category("Material Skin"), Description("Gets or sets hovered item used by the control.")]
        //public Color HoveredItemColor
        //{
        //    get => _hoveredItemColor;
        //    set
        //    {
        //        _hoveredItemColor = value;
        //        Refresh();
        //    }
        //}

        //[Category("Material Skin"), Description("Gets or sets hovered item backcolor used by the control.")]
        //public Color HoveredItemBackColor
        //{
        //    get => _hoveredItemBackColor;
        //    set
        //    {
        //        _hoveredItemBackColor = value;
        //        Refresh();
        //    }
        //}

        //[Category("Material Skin"), Description("Gets or sets disabled forecolor used by the control.")]
        //public Color DisabledForeColor
        //{
        //    get => _disabledForeColor;
        //    set
        //    {
        //        _disabledForeColor = value;
        //        Refresh();
        //    }
        //}

        //[Category("Material Skin"), Description("Gets or sets disabled backcolor used by the control.")]
        //public Color DisabledBackColor
        //{
        //    get => _disabledBackColor;
        //    set
        //    {
        //        _disabledBackColor = value;
        //        Refresh();
        //    }
        //}

        [Category("Material Skin"), Description("Gets or sets border color used by the control.")]
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                Refresh();
            }
        }

        //[Category("Material Skin")]
        //[Description("Gets or sets the whether this control reflect to parent(s) style. \n " +
        //             "Set it to false if you want the style of this control be independent. ")]
        //public bool IsDerivedStyle
        //{
        //    get => _isDerivedStyle;
        //    set
        //    {
        //        _isDerivedStyle = value;
        //        Refresh();
        //    }
        //}

        [Category("Material Skin"), DefaultValue(ListBoxStyle.SingleLine)]
        [Description("Gets or sets the control style.")]
        public ListBoxStyle Style
        {
            get => _style;
            set
            {
                _style = value;
                UpdateItemSpecs();

                InvalidateScroll(this, null);
                Refresh();
            }
        }

        [Category("Material Skin"), DefaultValue(MaterialItemDensity.Dense)]
        [Description("Gets or sets list density")]
        public MaterialItemDensity Density
        {
            get { return _density; }
            set
            {
                _density = value;
                //if (_density == MaterialItemDensity.Dense)
                //    Size = new Size(Size.Width, HEIGHTDENSE);
                //else
                //    Size = new Size(Size.Width, HEIGHTDEFAULT);

                UpdateItemSpecs();
                Invalidate();
            }
        }

        #endregion Properties

        #region Constructors

        public MaterialListBox()
        {
            SetStyle
            (
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.Selectable |
                ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor,
                    true
            );
            UpdateStyles();
            base.BackColor = Color.Transparent;
            base.Font = SkinManager.getFontByType(MaterialSkinManager.fontType.Subtitle1);
            _secondaryFont = SkinManager.getFontByType(MaterialSkinManager.fontType.Body1);
            //_utl = new Utilites();
            //ApplyTheme();

            ForeColor = SkinManager.TextHighEmphasisColor; // Color.Black;
            BackColor = Color.White;
            //SelectedItemBackColor = Color.FromArgb(65, 177, 225);
            //SelectedItemColor = Color.White;
            //HoveredItemColor = Color.DimGray;
            //HoveredItemBackColor = SkinManager.ColorScheme.AccentColor;  // Color.LightGray;
            //DisabledBackColor = SkinManager.BackgroundDisabledColor;  // Color.FromArgb(204, 204, 204);
            //DisabledForeColor = Color.FromArgb(136, 136, 136);
            BorderColor = Color.LightGray;
            //ThemeAuthor = "Taiizor";
            //ThemeName = "MetroLight";
            UpdateProperties();

            SetDefaults();
        }

        private void SetDefaults()
        {
            SelectedIndex = -1;
            _hoveredItem = -1;
            _showScrollBar = false;
            //_items = new MaterialItemCollection();
            //_items.ItemUpdated += InvalidateScroll;
            _items.CollectionChanged += InvalidateScroll;
            _selectedItems = new List<object>();
            _indicates = new List<object>();
            //ItemHeight = 30;
            _multiKeyDown = false;
            //VScrollBar vScrollBar1 = new VScrollBar();
            _scrollBar = new MaterialScrollBar()
            {
                Orientation = MaterialScrollOrientation.Vertical,
                Size = new Size(12, Height),
                Maximum = _items.Count * _itemHeight,
                SmallChange = _itemHeight,
                LargeChange = _itemHeight
            };
            _scrollBar.Scroll += HandleScroll;
            _scrollBar.MouseDown += VS_MouseDown;
            _scrollBar.BackColor = Color.Transparent;
            if (!Controls.Contains(_scrollBar))
            {
                Controls.Add(_scrollBar);
            }

            Style = ListBoxStyle.SingleLine;
            Density = MaterialItemDensity.Dense;
        }

        #endregion Constructors

        #region ApplyTheme


        private void UpdateProperties()
        {
            Invalidate();
        }

        private void UpdateItemSpecs()
        {
            //_leftrightPadding = 16;
            //if (_style == ListBoxStyle.SingleLine)
            //{
            //    if (_density == MaterialItemDensity.Dense)
            //        _itemHeight = 40;
            //    else
            //        _itemHeight = 48;
            //    base.Font = SkinManager.getFontByType(MaterialSkinManager.fontType.Subtitle1);
            //    _secondaryFont = SkinManager.getFontByType(MaterialSkinManager.fontType.Body1);
            //}
            if (_style == ListBoxStyle.TwoLine)
            {
                _secondaryTextTopPadding = 4;
                if (_density == MaterialItemDensity.Dense)
                {
                    _itemHeight = 60;
                    _secondaryTextBottomPadding = 10;
                    _primaryTextBottomPadding = 2;
                    _primaryFont = SkinManager.getFontByType(MaterialSkinManager.fontType.Body1);
                    _secondaryFont = SkinManager.getFontByType(MaterialSkinManager.fontType.Body2);
                }
                else
                {
                    _itemHeight = 72;
                    _secondaryTextBottomPadding = 16;
                    _primaryTextBottomPadding = 4;
                    _primaryFont = SkinManager.getFontByType(MaterialSkinManager.fontType.Subtitle1);
                    _secondaryFont = SkinManager.getFontByType(MaterialSkinManager.fontType.Body1);
                }
            }
            else if (_style == ListBoxStyle.ThreeLine)
            {
                _primaryTextBottomPadding = 4;
                _secondaryTextTopPadding = 4;
                if (_density == MaterialItemDensity.Dense)
                {
                    _itemHeight = 76;
                    _secondaryTextBottomPadding = 16;
                    _primaryFont = SkinManager.getFontByType(MaterialSkinManager.fontType.Body1);
                    _secondaryFont = SkinManager.getFontByType(MaterialSkinManager.fontType.Body2);
                }
                else
                {
                    _itemHeight = 88;
                    _secondaryTextBottomPadding = 12;
                    _primaryFont = SkinManager.getFontByType(MaterialSkinManager.fontType.Subtitle1);
                    _secondaryFont = SkinManager.getFontByType(MaterialSkinManager.fontType.Body1);
                }
            }
            else
            {
                //SingleLine
                if (_density == MaterialItemDensity.Dense)
                    _itemHeight = 40;
                else
                    _itemHeight = 48;
                _primaryFont = SkinManager.getFontByType(MaterialSkinManager.fontType.Subtitle1);
                _secondaryFont = SkinManager.getFontByType(MaterialSkinManager.fontType.Body1);
            }

        }



        #endregion ApplyTheme



        #region Draw Control

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_updating == true) return;

            Graphics g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            Rectangle mainRect = new Rectangle(0, 0, Width - (ShowBorder ? 1 : 0), Height - (ShowBorder ? 1 : 0));

            int lastItem = (_scrollBar.Value / _itemHeight) + (Height / _itemHeight) + 1 > Items.Count ? Items.Count : (_scrollBar.Value / _itemHeight) + (Height / _itemHeight) + 1;
            int firstItem = _scrollBar.Value / _itemHeight < 0 ? 0 : (_scrollBar.Value / _itemHeight);
            //int firstItem = _scrollBar.Value / _itemHeight < 0 ? 0 : lastItem-(Height / _itemHeight) > 0 ? lastItem - (Height / _itemHeight) : 0;

            g.FillRectangle(Enabled ? SkinManager.BackgroundBrush : SkinManager.BackgroundDisabledBrush, mainRect);

            //Set TextAlignFlags
            NativeTextRenderer.TextAlignFlags primaryTextAlignFlags;
            NativeTextRenderer.TextAlignFlags secondaryTextAlignFlags = NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Top;
            if (_style == ListBoxStyle.TwoLine || _style == ListBoxStyle.ThreeLine)
            {
                primaryTextAlignFlags = NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Bottom;
            }
            else
            {
                //SingleLine
                primaryTextAlignFlags = NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Middle;
            }

            //Draw items
            for (int i = firstItem; i < lastItem; i++)
            {
                //string itemText = (string)Items[i];
                string itemText = Items[i].Text;
                string itemSecondaryText = Items[i].SecondaryText;

                Rectangle itemRect = new Rectangle(0, (i - firstItem) * _itemHeight, Width, _itemHeight);

                //g.DrawString(itemText, Font, usic, itemRect, sf);

                if (MultiSelect && _indicates.Count != 0)
                {
                    if (i == _hoveredItem && !_indicates.Contains(i))
                    {
                        //g.FillRectangle(hibc, itemRect);
                        g.FillRectangle(SkinManager.BackgroundHoverBrush, itemRect);
                        //g.DrawString(itemText, Font, hic, itemRect, sf);
                    }
                    else if (_indicates.Contains(i))
                    {
                        //g.FillRectangle(sibc, itemRect);
                        g.FillRectangle(UseAccentColor ? SkinManager.ColorScheme.AccentBrush : SkinManager.ColorScheme.PrimaryBrush, itemRect);
                        //g.DrawString(itemText, Font, sic, itemRect, sf);
                    }
                }
                else
                {
                    if (i == _hoveredItem && i != SelectedIndex)
                    {
                        //g.FillRectangle(hibc, itemRect);
                        g.FillRectangle(SkinManager.BackgroundHoverBrush, itemRect);
                        //g.DrawString(itemText, Font, hic, itemRect, sf);
                    }
                    else if (i == SelectedIndex)
                    {
                        //g.FillRectangle(sibc, itemRect);
                        g.FillRectangle(UseAccentColor ? SkinManager.ColorScheme.AccentBrush : SkinManager.ColorScheme.PrimaryBrush, itemRect);
                        //g.DrawString(itemText, Font, sic, itemRect, sf);
                    }
                }

                //Define primary & secondary Text Rect
                Rectangle primaryTextRect = new Rectangle(itemRect.X + _leftrightPadding, itemRect.Y, itemRect.Width - (2 * _leftrightPadding), itemRect.Height);
                Rectangle secondaryTextRect = new Rectangle();

                if (_style == ListBoxStyle.TwoLine)
                {
                    primaryTextRect.Height = (primaryTextRect.Height / 2) - _primaryTextBottomPadding;
                }
                else if (_style == ListBoxStyle.ThreeLine)
                {
                    if (_density == MaterialItemDensity.Default)
                    {
                        primaryTextRect.Height = 36 - _primaryTextBottomPadding;
                    }
                    else
                    {
                        primaryTextRect.Height = 30 - _primaryTextBottomPadding;
                    }
                }
                secondaryTextRect = new Rectangle(primaryTextRect.X, primaryTextRect.Y + primaryTextRect.Height + (_primaryTextBottomPadding + _secondaryTextTopPadding), primaryTextRect.Width, _itemHeight - _secondaryTextBottomPadding - primaryTextRect.Height - (_primaryTextBottomPadding + _secondaryTextTopPadding));

                using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
                {
                    if (i != SelectedIndex)
                        NativeText.DrawTransparentText(
                        itemText,
                        _primaryFont,
                        SkinManager.TextHighEmphasisColor,
                        primaryTextRect.Location,
                        primaryTextRect.Size,
                        primaryTextAlignFlags);
                    else
                        NativeText.DrawTransparentText(
                        itemText,
                        _primaryFont,
                        UseAccentColor ? SkinManager.TextHighEmphasisColor : SkinManager.ColorScheme.TextColor,
                        primaryTextRect.Location,
                        primaryTextRect.Size,
                        primaryTextAlignFlags);
                    if (_style == ListBoxStyle.TwoLine)
                    {
                        NativeText.DrawTransparentText(
                        itemSecondaryText,
                        _secondaryFont,
                        SkinManager.TextDisabledOrHintColor,
                        secondaryTextRect.Location,
                        secondaryTextRect.Size,
                        secondaryTextAlignFlags);
                    }
                    else if (_style == ListBoxStyle.ThreeLine)
                    {
                        NativeText.DrawMultilineTransparentText(
                        itemSecondaryText,
                        _secondaryFont,
                        SkinManager.TextDisabledOrHintColor,
                        secondaryTextRect.Location,
                        secondaryTextRect.Size,
                        secondaryTextAlignFlags);
                    }
                }

            }
            if (ShowBorder)
            {
                g.DrawRectangle(Pens.LightGray, mainRect);
            }
            //using (SolidBrush bg = new SolidBrush(Enabled ? BackColor : DisabledBackColor))
            //{
            //    using (SolidBrush usic = new SolidBrush(Enabled ? ForeColor : DisabledForeColor))
            //    {
            //        using (SolidBrush sic = new SolidBrush(SelectedItemColor))
            //        {
            //            using (SolidBrush sibc = new SolidBrush(SelectedItemBackColor))
            //            {
            //                using (SolidBrush hic = new SolidBrush(HoveredItemColor))
            //                {
            //                    using (SolidBrush hibc = new SolidBrush(HoveredItemBackColor))
            //                    {
            //                        using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center })
            //                        {
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
        }

        #endregion Draw Control

        #region Methods

        public void AddItem(MaterialListBoxItem newItem)
        {
            _items.Add(newItem);
            InvalidateScroll(this, null);
            ItemsCountChanged?.Invoke(this, new EventArgs());
        }

        public void AddItem(string newItem)
        {
            MaterialListBoxItem _newitemMLBI = new MaterialListBoxItem(newItem);
            _items.Add(_newitemMLBI);
            InvalidateScroll(this, null);
            ItemsCountChanged?.Invoke(this, new EventArgs());
        }

        public void AddItems(MaterialListBoxItem[] newItems)
        {
            _updating = true;
            foreach (MaterialListBoxItem str in newItems)
            {
                AddItem(str);
            }
            _updating = false;

            InvalidateScroll(this, null);
            ItemsCountChanged?.Invoke(this, new EventArgs());
        }

        public void AddItems(string[] newItems)
        {
            _updating = true;
            foreach (string str in newItems)
            {
                AddItem(str);
            }
            _updating = false;

            InvalidateScroll(this, null);
            ItemsCountChanged?.Invoke(this, new EventArgs());
        }

        public void RemoveItemAt(int index)
        {
           if (index<= _selectedIndex)
            {
                _selectedIndex -=1;
                update_selection();
            }
            _items.RemoveAt(index);
            InvalidateScroll(this, null);
            ItemsCountChanged?.Invoke(this, new EventArgs());
        }

        public void RemoveItem(MaterialListBoxItem item)
        {
            if (_items.IndexOf(item)<= _selectedIndex)
            {
                _selectedIndex -= 1;
                update_selection();
            }
            _items.Remove(item);
            InvalidateScroll(this, null);
            ItemsCountChanged?.Invoke(this, new EventArgs());
        }

        public int IndexOf(MaterialListBoxItem value)
        {
            return _items.IndexOf(value);
        }

        //public bool Contains(object item)
        //{
        //    return _items.Contains(item.ToString());
        //}

        public void RemoveItems(MaterialListBoxItem[] itemsToRemove)
        {
            _updating = true;
            foreach (MaterialListBoxItem item in itemsToRemove)
            {
                if (_items.IndexOf(item) <= _selectedIndex)
                {
                    _selectedIndex -= 1;
                    update_selection();
                }
                _items.Remove(item);
            }
            _updating = false;

            InvalidateScroll(this, null);
            ItemsCountChanged?.Invoke(this, new EventArgs());

        }

        private void update_selection()
        {
            if (_selectedIndex >= 0)
            {
                _selectedItem = _items[_selectedIndex];
                _selectedValue = _items[_selectedIndex];
                _selectedText = _items[_selectedIndex].ToString();
            }
            else
            {
                _selectedItem = null;
                _selectedValue = null;
                _selectedText = null;
            }
        }

        public void Clear()
        {
            _updating = true;
            for (int i = _items.Count - 1; i >= 0; i += -1)
            {
                _items.RemoveAt(i);
            }
            _updating = false;
            _selectedIndex = -1;
            update_selection();

            InvalidateScroll(this, null);
            ItemsCountChanged?.Invoke(this, new EventArgs());

        }

        public void BeginUpdate()
        {
            _updating = true;
        }

        public void EndUpdate()
        {
            _updating = false;
        }

        #endregion Methods

        #region Events

        [Category("Behavior")]
        [Description("Occurs when selected index change.")]
        public event SelectedIndexChangedEventHandler SelectedIndexChanged;

        public delegate void SelectedIndexChangedEventHandler(object sender, MaterialListBoxItem selectedItem);

        [Category("Behavior")]
        [Description("Occurs when selected value change.")]
        public event SelectedValueEventHandler SelectedValueChanged;

        public delegate void SelectedValueEventHandler(object sender, MaterialListBoxItem selectedItem);

        [Category("Behavior")]
        [Description("Occurs when item is added or removed.")]
        public event EventHandler ItemsCountChanged;

        #endregion Events

        protected override void OnSizeChanged(EventArgs e)
        {
            InvalidateScroll(this, e);
            InvalidateLayout();
            base.OnSizeChanged(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Focus();
            if (e.Button == MouseButtons.Left)
            {
                int index = _scrollBar.Value / _itemHeight + e.Location.Y / _itemHeight;
                //int index = e.Location.Y / _itemHeight;
                if (index >= 0 && index < _items.Count)
                {
                    if (MultiSelect && _multiKeyDown)
                    {
                        _indicates.Add(index);
                        _selectedItems.Add(Items[index]);
                    }
                    else
                    {
                        _indicates.Clear();
                        _selectedItems.Clear();
                        _selectedItem = Items[index];
                        _selectedIndex = index;
                        _selectedValue = Items[index];
                        _selectedText = Items[index].ToString();
                        SelectedIndexChanged?.Invoke(this, _selectedItem);
                        SelectedValueChanged?.Invoke(this, _selectedItem);
                    }
                }
                Invalidate();
            }
            base.OnMouseDown(e);
        }

        private void HandleScroll(object sender, ScrollEventArgs e)
        {
            Invalidate();
        }

        private void InvalidateScroll(object sender, EventArgs e)
        {
            _scrollBar.Maximum = _items.Count * _itemHeight;
            _scrollBar.SmallChange = _itemHeight;
            _scrollBar.LargeChange = Height;
            _scrollBar.Visible = (_items.Count * _itemHeight) > Height;
            if (_items.Count == 0)
            { _scrollBar.Value = 0; }
            Invalidate();
        }

        private void VS_MouseDown(object sender, MouseEventArgs e)
        {
            Focus();
        }

        private void InvalidateLayout()
        {
            _scrollBar.Size = new Size(12, Height - (ShowBorder ? 2 : 0));
            _scrollBar.Location = new Point(Width - (_scrollBar.Width + (ShowBorder ? 1 : 0)), ShowBorder ? 1 : 0);
            Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (_scrollBar.Visible == true)
            {
                //_scrollBar.Value -= e.Delta / 4;
                if (_scrollBar.Minimum > _scrollBar.Value - e.Delta / 2)
                    _scrollBar.Value = _scrollBar.Minimum;
                else if (_scrollBar.Maximum < _scrollBar.Value + Height)// - e.Delta/2 )
                {
                    if (e.Delta>0)
                        _scrollBar.Value -= e.Delta / 2;
                    else
                    { } //Do nothing, maximum reached
                }
                else
                    _scrollBar.Value -= e.Delta / 2;

                _updateHoveredItem(e);

                Invalidate();
                base.OnMouseWheel(e);
            }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Down:
                    try
                    {
                        _selectedItems.Remove(_items[SelectedIndex]);
                        SelectedIndex += 1;
                        _selectedItems.Add(_items[SelectedIndex]);
                    }
                    catch
                    {
                        //
                    }
                    break;

                case Keys.Up:
                    try
                    {
                        _selectedItems.Remove(_items[SelectedIndex]);
                        SelectedIndex -= 1;
                        _selectedItems.Add(_items[SelectedIndex]);
                    }
                    catch
                    {
                        //
                    }
                    break;
            }
            Invalidate();
            return base.IsInputKey(keyData);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Cursor = Cursors.Hand;
            //int index = _scrollBar.Value / _itemHeight + e.Location.Y / _itemHeight;
            ////int index = e.Location.Y / _itemHeight;

            //if (index >= Items.Count)
            //{
            //    index = -1;
            //}

            //if (index >= 0 && index < Items.Count)
            //{
            //    _hoveredItem = index;
            //}
            _updateHoveredItem(e);

            Invalidate();
        }

        private void _updateHoveredItem(MouseEventArgs e)
        {
            int index = _scrollBar.Value / _itemHeight + e.Location.Y / _itemHeight;
            //int index = e.Location.Y / _itemHeight;

            if (index >= Items.Count)
            {
                index = -1;
            }

            if (index >= 0 && index<Items.Count)
            {
                _hoveredItem = index;
            }

        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _hoveredItem = -1;
            Cursor = Cursors.Default;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            _scrollBar.Size = new Size(12, Height - (ShowBorder ? 2 : 0));
            _scrollBar.Location = new Point( Width - (_scrollBar.Width + (ShowBorder ? 1 : 0)), ShowBorder ? 1 : 0);
            InvalidateScroll(this, e);
        }

        public const int WM_SETCURSOR = 0x0020;
        public const int IDC_HAND = 32649;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetCursor(IntPtr hCursor);

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_SETCURSOR)
            {
                SetCursor(LoadCursor(IntPtr.Zero, IDC_HAND));
                m.Result = IntPtr.Zero;
                return;
            }
            base.WndProc(ref m);
        }


    }

    #endregion

}
