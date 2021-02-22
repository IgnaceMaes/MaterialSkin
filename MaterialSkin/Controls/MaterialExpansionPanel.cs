using MaterialSkin.Animations;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;


namespace MaterialSkin.Controls
{
    public class MaterialExpansionPanel : Panel, IMaterialControl
    {

        #region "Private members"

        private const int _expansionPanelDefaultPadding = 16;
        private const int _leftrightPadding = 24;
        private const int _buttonPadding = 8;
        private const int _expandcollapsbuttonsize = 24;
        private const int _textHeaderHeight = 24;
        private const int _headerHeightCollapse = 48;
        private const int _headerHeightExpand = 64;
        private const int _footerHeight = 68;
        private const int _footerButtonHeight = 36;
        private const int _minHeight = 200;
        private int _headerHeight ;

        private bool _collapse ;
        private bool _useAccentColor;
        private int _expandHeight;
									
									  
        private string _titleHeader;
        private string _descriptionHeader;
        private string _validationButtonText;
        private string _cancelButtonText;
										
																 
										  
        private bool _showValidationButtons;
        private bool _showCollapseExpand;
        private bool _drawShadows;
        private bool _shadowDrawEventSubscribed = false;
        private Rectangle _headerBounds;
        private Rectangle _expandcollapseBounds;
        private Rectangle _savebuttonBounds;
        private Rectangle _cancelbuttonBounds;
        private bool _savebuttonEnable;

        private enum ButtonState
        {
            SaveOver,
            CancelOver,
            ColapseExpandOver,
            HeaderOver,
            None
        }

        private ButtonState _buttonState = ButtonState.None;


        #endregion


        #region "Public Properties"

        [Browsable(false)]
        public int Depth { get; set; }
        [Browsable(false)]
        public MaterialSkinManager SkinManager { get { return MaterialSkinManager.Instance; } }
        [Browsable(false)]
        public MouseState MouseState { get; set; }

        [Category("Material Skin"), DefaultValue(false), DisplayName("Use Accent Color")]
        public bool UseAccentColor
        {
            get { return _useAccentColor; }
            set { _useAccentColor = value; Invalidate(); }
        }

        [DefaultValue(false)]
        [Description("Collapses the control when set to true")]
        [Category("Material Skin")]
        public bool Collapse
        {
            get { return _collapse; }
            set
            {
                _collapse = value;
                CollapseOrExpand();
                Invalidate();
            }
        }

        [DefaultValue("Title")]
        [Category("Material Skin"), DisplayName("Title")]
        [Description("Title to show in expansion panel's header")]
        public string Title
        {
            get { return _titleHeader; }
            set
            {
                _titleHeader = value;
                Invalidate();
            }
        }

        [DefaultValue("Description")]
        [Category("Material Skin"), DisplayName("Description")]
        [Description("Description to show in expansion panel's header")]
        public string Description
        {
            get { return _descriptionHeader; }
            set
            {
                _descriptionHeader = value;
                Invalidate();
            }
        }

        [DefaultValue(true)]
        [Category("Material Skin"), DisplayName("Draw Shadows")]
        [Description("Draw Shadows around control")]
        public bool DrawShadows
        {
            get { return _drawShadows; }
            set { _drawShadows = value; Invalidate(); }
        }

        [DefaultValue(240)]
        [Category("Material Skin"), DisplayName("Expand Height")]
        [Description("Define control height when expanded")]
        public int ExpandHeight
        {
            get { return _expandHeight; }
            set { if (value < _minHeight) value = _minHeight; _expandHeight = value; Invalidate(); }
        }

        [DefaultValue(true)]
        [Category("Material Skin"), DisplayName("Show collapse/expand")]
        [Description("Show collapse/expand indicator")]
        public bool ShowCollapseExpand
        {
            get { return _showCollapseExpand; }
            set { _showCollapseExpand = value; Invalidate(); }
        }

        [DefaultValue(true)]
        [Category("Material Skin"), DisplayName("Show validation buttons")]
        [Description("Show save/cancel button")]
        public bool ShowValidationButtons
        {
            get { return _showValidationButtons; }
            set { _showValidationButtons = value; Invalidate(); }
        }

        [DefaultValue("SAVE")]
        [Category("Material Skin"), DisplayName("Validation button text")]
        [Description("Set Validation button text")]
        public string ValidationButtonText
        {
            get { return _validationButtonText; }
            set { _validationButtonText = value; UpdateRects(); Invalidate(); }
        }

        [DefaultValue("CANCEL")]
        [Category("Material Skin"), DisplayName("Cancel button text")]
        [Description("Set Cancel button text")]
        public string CancelButtonText
        {
            get { return _cancelButtonText; }
            set { _cancelButtonText = value; UpdateRects(); Invalidate(); }
        }

        [DefaultValue(true)]
        [Category("Material Skin"), DisplayName("Validation button enable")]
        [Description("Enable validation button")]
        public bool ValidationButtonEnable
        {
            get { return _savebuttonEnable; }
            set { _savebuttonEnable = value; Invalidate(); }
        }


        #endregion


        #region "Events"

        [Category("Action")]
        [Description("Fires when Save button is clicked")]
        public event EventHandler SaveClick;

        [Category("Action")]
        [Description("Fires when Cancel button is clicked")]
        public event EventHandler CancelClick;

        [Category("Disposition")]
        [Description("Fires when Panel Collapse")]
        public event EventHandler PanelCollapse;

        [Category("Disposition")]
        [Description("Fires when Panel Expand")]
        public event EventHandler PanelExpand;


        #endregion

        public MaterialExpansionPanel()
        {
            ShowValidationButtons = true;
            ValidationButtonEnable = false;
            ValidationButtonText = "SAVE";
            CancelButtonText = "CANCEL";
            ShowCollapseExpand = true;
            Collapse = false;
            Title = "Title";
            Description = "Description";
            DrawShadows = true;
            ExpandHeight = 240;
            AutoScroll = false;

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            BackColor = SkinManager.BackgroundColor;
            ForeColor = SkinManager.TextHighEmphasisColor;

            Padding = new Padding(24, 64, 24, 16);
            Margin = new Padding( 3, 16,  3, 16);
            Size = new Size(480, ExpandHeight);
            							 
        }


        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            Font = SkinManager.getFontByType(MaterialSkinManager.fontType.Body1);
        }

        protected override void InitLayout()
        {
            LocationChanged += (sender, e) => { Parent?.Invalidate(); };
            ForeColor = SkinManager.TextHighEmphasisColor;
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            if (Parent != null) AddShadowPaintEvent(Parent, drawShadowOnParent);
            if (_oldParent != null) RemoveShadowPaintEvent(_oldParent, drawShadowOnParent);
            _oldParent = Parent;
        }

        private Control _oldParent;

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Parent == null) return;
            if (Visible)
                AddShadowPaintEvent(Parent, drawShadowOnParent);
            else
                RemoveShadowPaintEvent(Parent, drawShadowOnParent);
        }

        private void drawShadowOnParent(object sender, PaintEventArgs e)
        {
            if (Parent == null)
            {
                RemoveShadowPaintEvent((Control)sender, drawShadowOnParent);
                return;
            }

            if (!_drawShadows || Parent == null) return;

            // paint shadow on parent
            Graphics gp = e.Graphics;
            Rectangle rect = new Rectangle(Location, ClientRectangle.Size);
            gp.SmoothingMode = SmoothingMode.AntiAlias;
            DrawHelper.DrawSquareShadow(gp, rect);
        }


        private void AddShadowPaintEvent(Control control, PaintEventHandler shadowPaintEvent)
        {
            if (_shadowDrawEventSubscribed) return;
            control.Paint += shadowPaintEvent;
            control.Invalidate();
            _shadowDrawEventSubscribed = true;
        }

        private void RemoveShadowPaintEvent(Control control, PaintEventHandler shadowPaintEvent)
        {
            if (!_shadowDrawEventSubscribed) return;
            control.Paint -= shadowPaintEvent;
            control.Invalidate();
            _shadowDrawEventSubscribed = false;
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            BackColor = SkinManager.BackgroundColor;
        }

        protected override void OnResize(EventArgs e)
        {
            if (!_collapse)
            {
                if (DesignMode)
                {
                    _expandHeight = Height;
                }
                if (Height < _minHeight) Height = _minHeight;
            }
            else
            {
                Height = _headerHeightCollapse;
            }

            base.OnResize(e);

            _headerBounds = new Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, _headerHeight);
            _expandcollapseBounds = new Rectangle((Width) - _leftrightPadding - _expandcollapsbuttonsize, (int)((_headerHeight - _expandcollapsbuttonsize) / 2), _expandcollapsbuttonsize, _expandcollapsbuttonsize);

            UpdateRects();

            if (Parent != null)
            { 
                RemoveShadowPaintEvent(Parent, drawShadowOnParent);
                AddShadowPaintEvent(Parent, drawShadowOnParent);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (DesignMode)
                return;

            var oldState = _buttonState;

            if (_savebuttonBounds.Contains(e.Location))
                _buttonState = ButtonState.SaveOver;
            else if (_cancelbuttonBounds.Contains(e.Location))
                _buttonState = ButtonState.CancelOver;
            else if (_expandcollapseBounds.Contains(e.Location))
            {
                Cursor = Cursors.Hand;
                _buttonState = ButtonState.ColapseExpandOver;
            }
            else if (_headerBounds.Contains(e.Location))
            {
                Cursor = Cursors.Hand;
                _buttonState = ButtonState.HeaderOver;
            }
            else
            {
                Cursor = Cursors.Default;
                _buttonState = ButtonState.None;
            }

            if (oldState != _buttonState) Invalidate();

        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (Enabled && _buttonState == ButtonState.SaveOver && _savebuttonEnable)
            {
                // Raise the event
                SaveClick?.Invoke(this, new EventArgs());
                Collapse = true;
                CollapseOrExpand();
            }
            else if (Enabled && _buttonState == ButtonState.CancelOver)
            {
                // Raise the event
                CancelClick?.Invoke(this, new EventArgs());		
                Collapse = true;
                CollapseOrExpand();
            }
															  
            else if (Enabled && (_buttonState == ButtonState.HeaderOver | _buttonState == ButtonState.ColapseExpandOver))
            {
                Collapse = !Collapse;
                CollapseOrExpand();
            }
            else
            {
                if (DesignMode)
                    return;
            }

             base.OnMouseDown(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (DesignMode)
                return;

            Cursor = Cursors.Arrow;
            _buttonState = ButtonState.None;
            Invalidate();
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            g.Clear(Parent.BackColor);

            // card rectangle path
            RectangleF expansionPanelRectF = new RectangleF(ClientRectangle.Location, ClientRectangle.Size);
            expansionPanelRectF.X -= 0.5f;
            expansionPanelRectF.Y -= 0.5f;
            GraphicsPath expansionPanelPath = DrawHelper.CreateRoundRect(expansionPanelRectF, 2);

            // button shadow (blend with form shadow)
            DrawHelper.DrawSquareShadow(g, ClientRectangle);

            // Draw expansion panel
            // Disabled
            if (!Enabled)
            {
                using (SolidBrush disabledBrush = new SolidBrush(DrawHelper.BlendColor(Parent.BackColor, SkinManager.BackgroundDisabledColor, SkinManager.BackgroundDisabledColor.A)))
                {
                    g.FillPath(disabledBrush, expansionPanelPath);
                }
            }
            // Mormal
            else
            {
                if ((_buttonState == ButtonState.HeaderOver | _buttonState == ButtonState.ColapseExpandOver) && _collapse)
                {
                    RectangleF expansionPanelBorderRectF = new RectangleF(ClientRectangle.X + 1, ClientRectangle.Y + 1, ClientRectangle.Width - 2, ClientRectangle.Height - 2);
                    expansionPanelBorderRectF.X -= 0.5f;
                    expansionPanelBorderRectF.Y -= 0.5f;
                    GraphicsPath expansionPanelBoarderPath = DrawHelper.CreateRoundRect(expansionPanelBorderRectF, 2);
																					   
                    g.FillPath(SkinManager.ExpansionPanelFocusBrush, expansionPanelBoarderPath);
                }
                else
                {
                    using (SolidBrush normalBrush = new SolidBrush(SkinManager.BackgroundColor))
                    {
                        g.FillPath(normalBrush, expansionPanelPath);
                    }
                }
            }

            // Calc text Rect
            Rectangle headerRect = new Rectangle(
                _leftrightPadding,
                (_headerHeight - _textHeaderHeight) / 2,
                TextRenderer.MeasureText(_titleHeader, Font).Width + _expansionPanelDefaultPadding,
                _textHeaderHeight);

            //Draw  headers
            using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
            {
                // Draw header text
                NativeText.DrawTransparentText(
                    _titleHeader,
                    SkinManager.getLogFontByType(MaterialSkinManager.fontType.Body1),
                    Enabled ? SkinManager.TextHighEmphasisColor : SkinManager.TextDisabledOrHintColor,
                    headerRect.Location,
                    headerRect.Size,
                    NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Middle);
            }

            if (!String.IsNullOrEmpty(_descriptionHeader))
	    {
                //Draw description header text 

                Rectangle headerDescriptionRect = new Rectangle(
                    headerRect.Right + _expansionPanelDefaultPadding,
                    (_headerHeight - _textHeaderHeight) / 2,
                    _expandcollapseBounds.Left - (headerRect.Right + _expansionPanelDefaultPadding ) - _expansionPanelDefaultPadding,
                    _textHeaderHeight);

                using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
                {
                    // Draw description header text 
                    NativeText.DrawTransparentText(
                    _descriptionHeader,
                    SkinManager.getLogFontByType(MaterialSkinManager.fontType.Body2),
                     SkinManager.TextDisabledOrHintColor,
                    headerDescriptionRect.Location,
                    headerDescriptionRect.Size,
                    NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Middle);
                }
            }

            if (_showCollapseExpand==true)
            {
                using (var formButtonsPen = new Pen(_useAccentColor ? SkinManager.ColorScheme.AccentColor : SkinManager.TextDisabledOrHintColor, 2))
                {
                    if (_collapse)
                    {
                        //Draw Expand button
                        System.Drawing.Drawing2D.GraphicsPath pth = new System.Drawing.Drawing2D.GraphicsPath();
                        PointF TopLeft = new PointF(_expandcollapseBounds.X + 6, _expandcollapseBounds.Y + 9);
                        PointF MidBottom = new PointF(_expandcollapseBounds.X + 12, _expandcollapseBounds.Y + 15);
                        PointF TopRight = new PointF(_expandcollapseBounds.X + 18, _expandcollapseBounds.Y + 9);
                        pth.AddLine(TopLeft, MidBottom);
                        pth.AddLine(TopRight, MidBottom);
                        g.DrawPath(formButtonsPen, pth);
                    }
                    else
                    {
                        // Draw Collapse button
                        System.Drawing.Drawing2D.GraphicsPath pth = new System.Drawing.Drawing2D.GraphicsPath();
                        PointF BottomLeft = new PointF(_expandcollapseBounds.X + 6, _expandcollapseBounds.Y + 15);
                        PointF MidTop = new PointF(_expandcollapseBounds.X + 12, _expandcollapseBounds.Y + 9);
                        PointF BottomRight = new PointF(_expandcollapseBounds.X + 18, _expandcollapseBounds.Y + 15);
                        pth.AddLine(BottomLeft, MidTop);
                        pth.AddLine(BottomRight, MidTop);
                        g.DrawPath(formButtonsPen, pth);
                    }
                }
            }

            if (!_collapse && _showValidationButtons)
            {
                //Draw divider
                g.DrawLine(new Pen(SkinManager.DividersColor, 1), new Point(0, Height - _footerHeight), new Point(Width, Height - _footerHeight));

                //Validation Button
                using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
                {
                    Point _textLocation = new Point(_savebuttonBounds.X + 8, _savebuttonBounds.Y);

                    NativeText.DrawTransparentText(
                    ValidationButtonText,
                    SkinManager.getLogFontByType(MaterialSkinManager.fontType.Button),
                    Enabled && _savebuttonEnable ? (_buttonState == ButtonState.SaveOver) ? SkinManager.ColorScheme.AccentColor : SkinManager.TextHighEmphasisColor : SkinManager.TextDisabledOrHintColor,
                    _textLocation,
                    _savebuttonBounds.Size,
                    NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Middle);
                }

                //Cancel Button
                using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
                {
                    Point _textLocation = new Point(_cancelbuttonBounds.X + 8, _cancelbuttonBounds.Y);

                    NativeText.DrawTransparentText(
                    CancelButtonText,
                    SkinManager.getLogFontByType( MaterialSkinManager.fontType.Button),
                    Enabled ? (_buttonState == ButtonState.CancelOver) ? SkinManager.ColorScheme.AccentColor : SkinManager.TextHighEmphasisColor : SkinManager.TextDisabledOrHintColor,
                    _textLocation,
                    _cancelbuttonBounds.Size,
                    NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Middle);
                }
            }
        }

        private void CollapseOrExpand()
        {
            //if (!useAnimation)
            //{
            if (_collapse)
            {
                _headerHeight = _headerHeightCollapse;
                this.Height = _headerHeightCollapse;
                Margin = new Padding(16, 1, 16, 0);

                // Is the event registered?
                if (PanelCollapse != null)
                    // Raise the event
                    this.PanelCollapse(this, new EventArgs());
            }
            else
            {
                _headerHeight = _headerHeightExpand;
                this.Height = _expandHeight;
                Margin = new Padding(16, 16, 16, 16);

                // Is the event registered?
                if (PanelExpand != null)
                    // Raise the event
                    this.PanelExpand(this, new EventArgs());
            }

            Refresh();
        }

        private void UpdateRects()
        {
            if (!_collapse && _showValidationButtons)
            {
                int _buttonWidth = ((TextRenderer.MeasureText(ValidationButtonText, SkinManager.getFontByType(MaterialSkinManager.fontType.Button))).Width + 16);
                _savebuttonBounds = new Rectangle((Width) - _buttonPadding - _buttonWidth, Height - _expansionPanelDefaultPadding - _footerButtonHeight, _buttonWidth, _footerButtonHeight);
                _buttonWidth = ((TextRenderer.MeasureText(CancelButtonText, SkinManager.getFontByType(MaterialSkinManager.fontType.Button))).Width + 16);
                _cancelbuttonBounds = new Rectangle(_savebuttonBounds.Left - _buttonPadding - _buttonWidth, Height - _expansionPanelDefaultPadding - _footerButtonHeight, _buttonWidth, _footerButtonHeight);
            }
        }

    }
}
