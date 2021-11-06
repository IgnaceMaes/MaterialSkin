
namespace MaterialSkin.Controls
{
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MaterialSkin.Animations;

    public class MaterialMultiLineTextBox2 : Control, IMaterialControl
    {

        MaterialContextMenuStrip cms = new BaseTextBoxContextMenuStrip();
        ContextMenuStrip _lastContextMenuStrip = new ContextMenuStrip();

        //Properties for managing the material design properties
        
        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        [Browsable(false)]
        public MouseState MouseState { get; set; }
		
		//Unused properties
		
        [Browsable(false)]
        public override System.Drawing.Image BackgroundImage { get; set; }

        [Browsable(false)]
        public override System.Windows.Forms.ImageLayout BackgroundImageLayout { get; set; }

        [Browsable(false)]
        public string SelectedText { get { return baseTextBox.SelectedText; } set { baseTextBox.SelectedText = value; } }

        [Browsable(false)]
        public int SelectionStart { get { return baseTextBox.SelectionStart; } set { baseTextBox.SelectionStart = value; } }
        [Browsable(false)]
        public int SelectionLength { get { return baseTextBox.SelectionLength; } set { baseTextBox.SelectionLength = value; } }
        [Browsable(false)]
        public int TextLength { get { return baseTextBox.TextLength; } }

        [Browsable(false)]
        public override System.Drawing.Color ForeColor { get; set; }

        //Material Skin properties
        

        [Category("Material Skin"), DefaultValue(""), Localizable(true)]
        public string Hint 
        { 
            get { return baseTextBox.Hint; } 
            set 
            { 
                baseTextBox.Hint = value;
                hasHint = !String.IsNullOrEmpty(baseTextBox.Hint);
                Invalidate();
            }
        }

        [Category("Material Skin"), DefaultValue(true)]
        public bool UseAccent { get; set; }


        //TextBox properties

        public override ContextMenuStrip ContextMenuStrip
        {
            get { return baseTextBox.ContextMenuStrip; }
            set
            {
                if (value != null)
                {
                    baseTextBox.ContextMenuStrip = value;
                    base.ContextMenuStrip = value;
                }
                else
                {
                    baseTextBox.ContextMenuStrip = cms;
                    base.ContextMenuStrip = cms;
                }
                _lastContextMenuStrip = base.ContextMenuStrip;
            }
        }

        [Browsable(false)]
        public override Color BackColor { get { return Parent == null ? SkinManager.BackgroundColor : Parent.BackColor; } }

        public override string Text { get { return baseTextBox.Text; } set { baseTextBox.Text = value; } }

        [Category("Appearance")]
        public HorizontalAlignment TextAlign { get { return baseTextBox.TextAlign; } set { baseTextBox.TextAlign = value; } }

        [Category("Appearance")]
        public ScrollBars ScrollBars { get { return baseTextBox.ScrollBars; } set { baseTextBox.ScrollBars = value; } }

        [Category("Behavior")]
        public CharacterCasing CharacterCasing { get { return baseTextBox.CharacterCasing; } set { baseTextBox.CharacterCasing = value; } }

        [Category("Behavior")]
        public bool HideSelection { get { return baseTextBox.HideSelection; } set { baseTextBox.HideSelection = value; } }

        [Category("Behavior")]
        public int MaxLength { get { return baseTextBox.MaxLength; } set { baseTextBox.MaxLength = value; } }

        [Category("Behavior")]
        public char PasswordChar { get { return baseTextBox.PasswordChar; } set { baseTextBox.PasswordChar = value; } }

        [Category("Behavior")]
        public bool ShortcutsEnabled 
        { 
            get 
            { return baseTextBox.ShortcutsEnabled; } 
            set 
            { 
                baseTextBox.ShortcutsEnabled = value;
                if (value == false)
                {
                    baseTextBox.ContextMenuStrip = null;
                    base.ContextMenuStrip = null;
                }
                else
                {
                    baseTextBox.ContextMenuStrip = _lastContextMenuStrip;
                    base.ContextMenuStrip = _lastContextMenuStrip;
                }
            }
        }

        [Category("Behavior")]
        public bool UseSystemPasswordChar { get { return baseTextBox.UseSystemPasswordChar; } set { baseTextBox.UseSystemPasswordChar = value; } }

        public new object Tag { get { return baseTextBox.Tag; } set { baseTextBox.Tag = value; } }

        private bool _readonly;
        [Category("Behavior")]
        public bool ReadOnly
        {
            get { return _readonly; }
            set
            {
                _readonly = value;
                if (Enabled == true)
                {
                    baseTextBox.ReadOnly = _readonly;
                }
                this.Invalidate();
            }
        }

        private bool _animateReadOnly;

        [Category("Material Skin")]
        [Browsable(true)]
        public bool AnimateReadOnly
        {
            get => _animateReadOnly;
            set
            {
                _animateReadOnly = value;
                Invalidate();
            }
        }

        private bool _leaveOnEnterKey;

        [Category("Material Skin"), DefaultValue(false), Description("Select next control which have TabStop property set to True when enter key is pressed.")]
        public bool LeaveOnEnterKey
        {
            get => _leaveOnEnterKey;
            set
            {
                _leaveOnEnterKey = value;
                if (value)
                {
                    baseTextBox.KeyDown += new KeyEventHandler(LeaveOnEnterKey_KeyDown);
                }
                else
                {
                    baseTextBox.KeyDown -= LeaveOnEnterKey_KeyDown;
                }
                Invalidate();
            }
        }

        public void SelectAll() { baseTextBox.SelectAll(); }

        public void Clear() { baseTextBox.Clear(); }

        public void Copy() { baseTextBox.Copy(); }

        public void Cut() { baseTextBox.Cut(); }

        public void Undo() { baseTextBox.Undo(); }

        public void Paste() { baseTextBox.Paste(); }


        # region Forwarding events to baseTextBox
        
        public event EventHandler AcceptsTabChanged
        {
            add
            {
                baseTextBox.AcceptsTabChanged += value;
            }
            remove
            {
                baseTextBox.AcceptsTabChanged -= value;
            }
        }

        public new event EventHandler AutoSizeChanged
        {
            add
            {
                baseTextBox.AutoSizeChanged += value;
            }
            remove
            {
                baseTextBox.AutoSizeChanged -= value;
            }
        }

        public new event EventHandler BackgroundImageChanged
        {
            add
            {
                baseTextBox.BackgroundImageChanged += value;
            }
            remove
            {
                baseTextBox.BackgroundImageChanged -= value;
            }
        }

        public new event EventHandler BackgroundImageLayoutChanged
        {
            add
            {
                baseTextBox.BackgroundImageLayoutChanged += value;
            }
            remove
            {
                baseTextBox.BackgroundImageLayoutChanged -= value;
            }
        }

        public new event EventHandler BindingContextChanged
        {
            add
            {
                baseTextBox.BindingContextChanged += value;
            }
            remove
            {
                baseTextBox.BindingContextChanged -= value;
            }
        }

        public event EventHandler BorderStyleChanged
        {
            add
            {
                baseTextBox.BorderStyleChanged += value;
            }
            remove
            {
                baseTextBox.BorderStyleChanged -= value;
            }
        }

        public new event EventHandler CausesValidationChanged
        {
            add
            {
                baseTextBox.CausesValidationChanged += value;
            }
            remove
            {
                baseTextBox.CausesValidationChanged -= value;
            }
        }

        public new event UICuesEventHandler ChangeUICues
        {
            add
            {
                baseTextBox.ChangeUICues += value;
            }
            remove
            {
                baseTextBox.ChangeUICues -= value;
            }
        }

        public new event EventHandler Click
        {
            add
            {
                baseTextBox.Click += value;
            }
            remove
            {
                baseTextBox.Click -= value;
            }
        }

        public new event EventHandler ClientSizeChanged
        {
            add
            {
                baseTextBox.ClientSizeChanged += value;
            }
            remove
            {
                baseTextBox.ClientSizeChanged -= value;
            }
        }

        #if NETFRAMEWORK
        public new event EventHandler ContextMenuChanged
        {
            add
            {
                baseTextBox.ContextMenuChanged += value;
            }
            remove
            {
                baseTextBox.ContextMenuChanged -= value;
            }
        }
        #endif

        public new event EventHandler ContextMenuStripChanged
        {
            add
            {
                baseTextBox.ContextMenuStripChanged += value;
            }
            remove
            {
                baseTextBox.ContextMenuStripChanged -= value;
            }
        }

        public new event ControlEventHandler ControlAdded
        {
            add
            {
                baseTextBox.ControlAdded += value;
            }
            remove
            {
                baseTextBox.ControlAdded -= value;
            }
        }

        public new event ControlEventHandler ControlRemoved
        {
            add
            {
                baseTextBox.ControlRemoved += value;
            }
            remove
            {
                baseTextBox.ControlRemoved -= value;
            }
        }

        public new event EventHandler CursorChanged
        {
            add
            {
                baseTextBox.CursorChanged += value;
            }
            remove
            {
                baseTextBox.CursorChanged -= value;
            }
        }

        public new event EventHandler Disposed
        {
            add
            {
                baseTextBox.Disposed += value;
            }
            remove
            {
                baseTextBox.Disposed -= value;
            }
        }

        public new event EventHandler DockChanged
        {
            add
            {
                baseTextBox.DockChanged += value;
            }
            remove
            {
                baseTextBox.DockChanged -= value;
            }
        }

        public new event EventHandler DoubleClick
        {
            add
            {
                baseTextBox.DoubleClick += value;
            }
            remove
            {
                baseTextBox.DoubleClick -= value;
            }
        }

        public new event DragEventHandler DragDrop
        {
            add
            {
                baseTextBox.DragDrop += value;
            }
            remove
            {
                baseTextBox.DragDrop -= value;
            }
        }

        public new event DragEventHandler DragEnter
        {
            add
            {
                baseTextBox.DragEnter += value;
            }
            remove
            {
                baseTextBox.DragEnter -= value;
            }
        }

        public new event EventHandler DragLeave
        {
            add
            {
                baseTextBox.DragLeave += value;
            }
            remove
            {
                baseTextBox.DragLeave -= value;
            }
        }

        public new event DragEventHandler DragOver
        {
            add
            {
                baseTextBox.DragOver += value;
            }
            remove
            {
                baseTextBox.DragOver -= value;
            }
        }

        public new event EventHandler EnabledChanged
        {
            add
            {
                baseTextBox.EnabledChanged += value;
            }
            remove
            {
                baseTextBox.EnabledChanged -= value;
            }
        }

        public new event EventHandler Enter
        {
            add
            {
                baseTextBox.Enter += value;
            }
            remove
            {
                baseTextBox.Enter -= value;
            }
        }

        public new event EventHandler FontChanged
        {
            add
            {
                baseTextBox.FontChanged += value;
            }
            remove
            {
                baseTextBox.FontChanged -= value;
            }
        }

        public new event EventHandler ForeColorChanged
        {
            add
            {
                baseTextBox.ForeColorChanged += value;
            }
            remove
            {
                baseTextBox.ForeColorChanged -= value;
            }
        }

        public new event GiveFeedbackEventHandler GiveFeedback
        {
            add
            {
                baseTextBox.GiveFeedback += value;
            }
            remove
            {
                baseTextBox.GiveFeedback -= value;
            }
        }

        public new event EventHandler GotFocus
        {
            add
            {
                baseTextBox.GotFocus += value;
            }
            remove
            {
                baseTextBox.GotFocus -= value;
            }
        }

        public new event EventHandler HandleCreated
        {
            add
            {
                baseTextBox.HandleCreated += value;
            }
            remove
            {
                baseTextBox.HandleCreated -= value;
            }
        }

        public new event EventHandler HandleDestroyed
        {
            add
            {
                baseTextBox.HandleDestroyed += value;
            }
            remove
            {
                baseTextBox.HandleDestroyed -= value;
            }
        }

        public new event HelpEventHandler HelpRequested
        {
            add
            {
                baseTextBox.HelpRequested += value;
            }
            remove
            {
                baseTextBox.HelpRequested -= value;
            }
        }

        public event EventHandler HideSelectionChanged
        {
            add
            {
                baseTextBox.HideSelectionChanged += value;
            }
            remove
            {
                baseTextBox.HideSelectionChanged -= value;
            }
        }

        public new event EventHandler ImeModeChanged
        {
            add
            {
                baseTextBox.ImeModeChanged += value;
            }
            remove
            {
                baseTextBox.ImeModeChanged -= value;
            }
        }

        public new event InvalidateEventHandler Invalidated
        {
            add
            {
                baseTextBox.Invalidated += value;
            }
            remove
            {
                baseTextBox.Invalidated -= value;
            }
        }

        public new event KeyEventHandler KeyDown
        {
            add
            {
                baseTextBox.KeyDown += value;
            }
            remove
            {
                baseTextBox.KeyDown -= value;
            }
        }

        public new event KeyPressEventHandler KeyPress
        {
            add
            {
                baseTextBox.KeyPress += value;
            }
            remove
            {
                baseTextBox.KeyPress -= value;
            }
        }

        public new event KeyEventHandler KeyUp
        {
            add
            {
                baseTextBox.KeyUp += value;
            }
            remove
            {
                baseTextBox.KeyUp -= value;
            }
        }

        public new event LayoutEventHandler Layout
        {
            add
            {
                baseTextBox.Layout += value;
            }
            remove
            {
                baseTextBox.Layout -= value;
            }
        }

        public new event EventHandler Leave
        {
            add
            {
                baseTextBox.Leave += value;
            }
            remove
            {
                baseTextBox.Leave -= value;
            }
        }

        public new event EventHandler LocationChanged
        {
            add
            {
                baseTextBox.LocationChanged += value;
            }
            remove
            {
                baseTextBox.LocationChanged -= value;
            }
        }

        public new event EventHandler LostFocus
        {
            add
            {
                baseTextBox.LostFocus += value;
            }
            remove
            {
                baseTextBox.LostFocus -= value;
            }
        }

        public new event EventHandler MarginChanged
        {
            add
            {
                baseTextBox.MarginChanged += value;
            }
            remove
            {
                baseTextBox.MarginChanged -= value;
            }
        }

        public event EventHandler ModifiedChanged
        {
            add
            {
                baseTextBox.ModifiedChanged += value;
            }
            remove
            {
                baseTextBox.ModifiedChanged -= value;
            }
        }

        public new event EventHandler MouseCaptureChanged
        {
            add
            {
                baseTextBox.MouseCaptureChanged += value;
            }
            remove
            {
                baseTextBox.MouseCaptureChanged -= value;
            }
        }

        public new event MouseEventHandler MouseClick
        {
            add
            {
                baseTextBox.MouseClick += value;
            }
            remove
            {
                baseTextBox.MouseClick -= value;
            }
        }

        public new event MouseEventHandler MouseDoubleClick
        {
            add
            {
                baseTextBox.MouseDoubleClick += value;
            }
            remove
            {
                baseTextBox.MouseDoubleClick -= value;
            }
        }

        public new event MouseEventHandler MouseDown
        {
            add
            {
                baseTextBox.MouseDown += value;
            }
            remove
            {
                baseTextBox.MouseDown -= value;
            }
        }

        public new event EventHandler MouseEnter
        {
            add
            {
                baseTextBox.MouseEnter += value;
            }
            remove
            {
                baseTextBox.MouseEnter -= value;
            }
        }

        public new event EventHandler MouseHover
        {
            add
            {
                baseTextBox.MouseHover += value;
            }
            remove
            {
                baseTextBox.MouseHover -= value;
            }
        }

        public new event EventHandler MouseLeave
        {
            add
            {
                baseTextBox.MouseLeave += value;
            }
            remove
            {
                baseTextBox.MouseLeave -= value;
            }
        }

        public new event MouseEventHandler MouseMove
        {
            add
            {
                baseTextBox.MouseMove += value;
            }
            remove
            {
                baseTextBox.MouseMove -= value;
            }
        }

        public new event MouseEventHandler MouseUp
        {
            add
            {
                baseTextBox.MouseUp += value;
            }
            remove
            {
                baseTextBox.MouseUp -= value;
            }
        }

        public new event MouseEventHandler MouseWheel
        {
            add
            {
                baseTextBox.MouseWheel += value;
            }
            remove
            {
                baseTextBox.MouseWheel -= value;
            }
        }

        public new event EventHandler Move
        {
            add
            {
                baseTextBox.Move += value;
            }
            remove
            {
                baseTextBox.Move -= value;
            }
        }

        public event EventHandler MultilineChanged
        {
            add
            {
                baseTextBox.MultilineChanged += value;
            }
            remove
            {
                baseTextBox.MultilineChanged -= value;
            }
        }

        public new event EventHandler PaddingChanged
        {
            add
            {
                baseTextBox.PaddingChanged += value;
            }
            remove
            {
                baseTextBox.PaddingChanged -= value;
            }
        }

        public new event PaintEventHandler Paint
        {
            add
            {
                baseTextBox.Paint += value;
            }
            remove
            {
                baseTextBox.Paint -= value;
            }
        }

        public new event EventHandler ParentChanged
        {
            add
            {
                baseTextBox.ParentChanged += value;
            }
            remove
            {
                baseTextBox.ParentChanged -= value;
            }
        }

        public new event PreviewKeyDownEventHandler PreviewKeyDown
        {
            add
            {
                baseTextBox.PreviewKeyDown += value;
            }
            remove
            {
                baseTextBox.PreviewKeyDown -= value;
            }
        }

        public new event QueryAccessibilityHelpEventHandler QueryAccessibilityHelp
        {
            add
            {
                baseTextBox.QueryAccessibilityHelp += value;
            }
            remove
            {
                baseTextBox.QueryAccessibilityHelp -= value;
            }
        }

        public new event QueryContinueDragEventHandler QueryContinueDrag
        {
            add
            {
                baseTextBox.QueryContinueDrag += value;
            }
            remove
            {
                baseTextBox.QueryContinueDrag -= value;
            }
        }

        public event EventHandler ReadOnlyChanged
        {
            add
            {
                baseTextBox.ReadOnlyChanged += value;
            }
            remove
            {
                baseTextBox.ReadOnlyChanged -= value;
            }
        }

        public new event EventHandler RegionChanged
        {
            add
            {
                baseTextBox.RegionChanged += value;
            }
            remove
            {
                baseTextBox.RegionChanged -= value;
            }
        }

        public new event EventHandler Resize
        {
            add
            {
                baseTextBox.Resize += value;
            }
            remove
            {
                baseTextBox.Resize -= value;
            }
        }

        public new event EventHandler RightToLeftChanged
        {
            add
            {
                baseTextBox.RightToLeftChanged += value;
            }
            remove
            {
                baseTextBox.RightToLeftChanged -= value;
            }
        }

        public new event EventHandler SizeChanged
        {
            add
            {
                baseTextBox.SizeChanged += value;
            }
            remove
            {
                baseTextBox.SizeChanged -= value;
            }
        }

        public new event EventHandler StyleChanged
        {
            add
            {
                baseTextBox.StyleChanged += value;
            }
            remove
            {
                baseTextBox.StyleChanged -= value;
            }
        }

        public new event EventHandler SystemColorsChanged
        {
            add
            {
                baseTextBox.SystemColorsChanged += value;
            }
            remove
            {
                baseTextBox.SystemColorsChanged -= value;
            }
        }

        public new event EventHandler TabIndexChanged
        {
            add
            {
                baseTextBox.TabIndexChanged += value;
            }
            remove
            {
                baseTextBox.TabIndexChanged -= value;
            }
        }

        public new event EventHandler TabStopChanged
        {
            add
            {
                baseTextBox.TabStopChanged += value;
            }
            remove
            {
                baseTextBox.TabStopChanged -= value;
            }
        }

       public event EventHandler TextAlignChanged
        {
            add
            {
                baseTextBox.TextAlignChanged += value;
            }
            remove
            {
                baseTextBox.TextAlignChanged -= value;
            }
        }

        public new event EventHandler TextChanged
        {
            add
            {
                baseTextBox.TextChanged += value;
            }
            remove
            {
                baseTextBox.TextChanged -= value;
            }
        }

        public new event EventHandler Validated
        {
            add
            {
                baseTextBox.Validated += value;
            }
            remove
            {
                baseTextBox.Validated -= value;
            }
        }

        public new event CancelEventHandler Validating
        {
            add
            {
                baseTextBox.Validating += value;
            }
            remove
            {
                baseTextBox.Validating -= value;
            }
        }

        public new event EventHandler VisibleChanged
        {
            add
            {
                baseTextBox.VisibleChanged += value;
            }
            remove
            {
                baseTextBox.VisibleChanged -= value;
            }
        }
        # endregion

        //private readonly AnimationManager animationManager;
        private readonly AnimationManager _animationManager;

        public bool isFocused = false;
        private const int HINT_TEXT_SMALL_SIZE = 18;
        private const int HINT_TEXT_SMALL_Y = 4;
        private const int LINE_BOTTOM_PADDING = 3;
        private const int TOP_PADDING = 10;
        private const int BOTTOM_PADDING = 10;
        private const int LEFT_PADDING = 16;
        private const int RIGHT_PADDING = 12;
        private int LINE_Y;
        private bool hasHint;

        protected readonly BaseTextBox baseTextBox;
        public MaterialMultiLineTextBox2()
        {
            // Material Properties
            UseAccent = true;
            MouseState = MouseState.OUT;

            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.DoubleBuffer, true);

            // Animations
            _animationManager = new AnimationManager
            {
                Increment = 0.06,
                AnimationType = AnimationType.EaseInOut,
                InterruptAnimation = false
            };
            _animationManager.OnAnimationProgress += sender => Invalidate();

            baseTextBox = new BaseTextBox
            {
                BorderStyle = BorderStyle.None,
                Font = SkinManager.getFontByType(MaterialSkinManager.fontType.Subtitle1),
                ForeColor = SkinManager.TextHighEmphasisColor,
                Multiline = true
            };

            Cursor = Cursors.IBeam;
            Enabled = true;
            ReadOnly = false;
            ScrollBars = ScrollBars.None;
            Size = new Size(250, 100);

            if (!Controls.Contains(baseTextBox) && !DesignMode)
            {
                Controls.Add(baseTextBox);
            }

            baseTextBox.GotFocus += (sender, args) =>
            {
                if (Enabled)
                {
                    isFocused = true;
                    _animationManager.StartNewAnimation(AnimationDirection.In);
                }
                else
                    base.Focus();
            };
            baseTextBox.LostFocus += (sender, args) =>
            {
                isFocused = false;
                _animationManager.StartNewAnimation(AnimationDirection.Out);
            };

            baseTextBox.TextChanged += new EventHandler(Redraw);
            baseTextBox.BackColorChanged += new EventHandler(Redraw);

            baseTextBox.TabStop = true;
            this.TabStop = false;

            cms.Opening += ContextMenuStripOnOpening;
            cms.OnItemClickStart += ContextMenuStripOnItemClickStart;
            ContextMenuStrip = cms;
        }

        private void Redraw(object sencer, EventArgs e)
        {
            SuspendLayout();
            Invalidate();
            ResumeLayout(false);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.Clear(Parent.BackColor);
            SolidBrush backBrush = new SolidBrush(DrawHelper.BlendColor(Parent.BackColor, SkinManager.BackgroundAlternativeColor, SkinManager.BackgroundAlternativeColor.A));

            //backColor
            g.FillRectangle(
                !Enabled ? SkinManager.BackgroundDisabledBrush : // Disabled
                isFocused ? SkinManager.BackgroundFocusBrush :  // Focused
                MouseState == MouseState.HOVER && (!ReadOnly || (ReadOnly && !AnimateReadOnly)) ? SkinManager.BackgroundHoverBrush : // Hover
                backBrush, // Normal
                ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, LINE_Y);

            baseTextBox.BackColor = !Enabled ? ColorHelper.RemoveAlpha(SkinManager.BackgroundDisabledColor, BackColor) : //Disabled
                isFocused ? DrawHelper.BlendColor(BackColor, SkinManager.BackgroundFocusColor, SkinManager.BackgroundFocusColor.A) : //Focused
                MouseState == MouseState.HOVER && (!ReadOnly || (ReadOnly && !AnimateReadOnly)) ? DrawHelper.BlendColor(BackColor, SkinManager.BackgroundHoverColor, SkinManager.BackgroundHoverColor.A) : // Hover
                DrawHelper.BlendColor(BackColor, SkinManager.BackgroundAlternativeColor, SkinManager.BackgroundAlternativeColor.A); // Normal

            // bottom line base
            g.FillRectangle(SkinManager.DividersAlternativeBrush, 0, LINE_Y, Width, 1);

            if (ReadOnly == false || (ReadOnly && AnimateReadOnly))
            {
                if (!_animationManager.IsAnimating())
                {
                    // bottom line
                    if (isFocused)
                    {
                        //No animation
                        g.FillRectangle(isFocused ? UseAccent ? SkinManager.ColorScheme.AccentBrush : SkinManager.ColorScheme.PrimaryBrush : SkinManager.DividersBrush, 0, LINE_Y, Width, isFocused ? 2 : 1);
                    }
                }
                else
                {
                    // Animate - Focus got/lost
                    double animationProgress = _animationManager.GetProgress();

                    // Line Animation
                    int LineAnimationWidth = (int)(Width * animationProgress);
                    int LineAnimationX = (Width / 2) - (LineAnimationWidth / 2);
                    g.FillRectangle(UseAccent ? SkinManager.ColorScheme.AccentBrush : SkinManager.ColorScheme.PrimaryBrush, LineAnimationX, LINE_Y, LineAnimationWidth, 2);
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (DesignMode)
                return;

            base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (DesignMode)
                return;

            baseTextBox?.Focus();
            base.OnMouseDown(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            if (DesignMode)
                return;

            base.OnMouseEnter(e);
            MouseState = MouseState.HOVER;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (DesignMode)
                return;

            if (this.ClientRectangle.Contains(this.PointToClient(Control.MousePosition)))
                return;
            else
            {
                base.OnMouseLeave(e);
                MouseState = MouseState.OUT;
                Invalidate();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            baseTextBox.Location = new Point(LEFT_PADDING, TOP_PADDING);
            baseTextBox.Width = Width - (LEFT_PADDING + RIGHT_PADDING);
            baseTextBox.Height = Height - (TOP_PADDING + BOTTOM_PADDING);

            LINE_Y = Height - LINE_BOTTOM_PADDING;

        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            // events
            MouseState = MouseState.OUT;
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
            var strip = sender as BaseTextBoxContextMenuStrip;
            if (strip != null)
            {
                strip.undo.Enabled = baseTextBox.CanUndo && !ReadOnly;
                strip.cut.Enabled = !string.IsNullOrEmpty(SelectedText) && !ReadOnly;
                strip.copy.Enabled = !string.IsNullOrEmpty(SelectedText);
                strip.paste.Enabled = Clipboard.ContainsText() && !ReadOnly;
                strip.delete.Enabled = !string.IsNullOrEmpty(SelectedText) && !ReadOnly;
                strip.selectAll.Enabled = !string.IsNullOrEmpty(Text);
            }
        }

        private void LeaveOnEnterKey_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter && e.Control == false)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                SendKeys.Send("{TAB}");
            }
        }
    }
}
