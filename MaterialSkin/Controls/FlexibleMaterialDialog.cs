﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace MaterialSkin.Controls
{
    /// <summary>
    /// The form to show the customized message box.
    /// It is defined as an internal class to keep the public interface of the FlexibleMessageBox clean.
    /// </summary>
    public class FlexibleMaterialForm : MaterialForm, IMaterialControl
    {
        private readonly MaterialSkinManager materialSkinManager;

        /// <summary>
        /// Defines the font for all FlexibleMessageBox instances.
        ///
        /// Default is: SystemFonts.MessageBoxFont
        /// </summary>
        public static Font FONT;

        /// <summary>
        /// Defines the maximum width for all FlexibleMessageBox instances in percent of the working area.
        ///
        /// Allowed values are 0.2 - 1.0 where:
        /// 0.2 means:  The FlexibleMessageBox can be at most half as wide as the working area.
        /// 1.0 means:  The FlexibleMessageBox can be as wide as the working area.
        ///
        /// Default is: 70% of the working area width.
        /// </summary>
        public static double MAX_WIDTH_FACTOR = 0.7;

        /// <summary>
        /// Defines the maximum height for all FlexibleMessageBox instances in percent of the working area.
        ///
        /// Allowed values are 0.2 - 1.0 where:
        /// 0.2 means:  The FlexibleMessageBox can be at most half as high as the working area.
        /// 1.0 means:  The FlexibleMessageBox can be as high as the working area.
        ///
        /// Default is: 90% of the working area height.
        /// </summary>
        public static double MAX_HEIGHT_FACTOR = 0.9;

        private MaterialMultiLineTextBox richTextBoxMessage;

        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.leftButton = new MaterialSkin.Controls.MaterialButton();
            this.FlexibleMaterialFormBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.messageContainer = new System.Windows.Forms.Panel();
            this.pictureBoxForIcon = new System.Windows.Forms.PictureBox();
            this.richTextBoxMessage = new MaterialSkin.Controls.MaterialMultiLineTextBox();
            this.middleButton = new MaterialSkin.Controls.MaterialButton();
            this.rightButton = new MaterialSkin.Controls.MaterialButton();
            ((System.ComponentModel.ISupportInitialize)(this.FlexibleMaterialFormBindingSource)).BeginInit();
            this.messageContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxForIcon)).BeginInit();
            this.SuspendLayout();
            //
            // leftButton
            //
            this.leftButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.leftButton.AutoSize = false;
            this.leftButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.leftButton.Depth = 0;
            this.leftButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.leftButton.DrawShadows = true;
            this.leftButton.HighEmphasis = false;
            this.leftButton.Icon = null;
            this.leftButton.Location = new System.Drawing.Point(44, 163);
            this.leftButton.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.leftButton.MinimumSize = new System.Drawing.Size(0, 24);
            this.leftButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.leftButton.Name = "leftButton";
            this.leftButton.Size = new System.Drawing.Size(108, 36);
            this.leftButton.TabIndex = 2;
            this.leftButton.Text = "OK";
            this.leftButton.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Text;
            this.leftButton.UseAccentColor = false;
            this.leftButton.UseVisualStyleBackColor = true;
            this.leftButton.Visible = false;
            //
            // messageContainer
            //
            this.messageContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.messageContainer.BackColor = System.Drawing.Color.White;
            this.messageContainer.Controls.Add(this.pictureBoxForIcon);
            this.messageContainer.Controls.Add(this.richTextBoxMessage);
            this.messageContainer.Location = new System.Drawing.Point(0, 65);
            this.messageContainer.Name = "messageContainer";
            this.messageContainer.Size = new System.Drawing.Size(388, 81);
            this.messageContainer.TabIndex = 1;
            //
            // pictureBoxForIcon
            //
            this.pictureBoxForIcon.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxForIcon.Location = new System.Drawing.Point(15, 19);
            this.pictureBoxForIcon.Name = "pictureBoxForIcon";
            this.pictureBoxForIcon.Size = new System.Drawing.Size(32, 32);
            this.pictureBoxForIcon.TabIndex = 8;
            this.pictureBoxForIcon.TabStop = false;
            //
            // richTextBoxMessage
            //
            this.richTextBoxMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxMessage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(237)))), ((int)(((byte)(237)))));
            this.richTextBoxMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxMessage.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.FlexibleMaterialFormBindingSource, "MessageText", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.richTextBoxMessage.Depth = 0;
            this.richTextBoxMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxMessage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.richTextBoxMessage.Hint = "";
            this.richTextBoxMessage.Location = new System.Drawing.Point(47, 2);
            this.richTextBoxMessage.Margin = new System.Windows.Forms.Padding(0);
            this.richTextBoxMessage.MouseState = MaterialSkin.MouseState.HOVER;
            this.richTextBoxMessage.Name = "richTextBoxMessage";
            this.richTextBoxMessage.ReadOnly = true;
            this.richTextBoxMessage.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBoxMessage.Size = new System.Drawing.Size(338, 78);
            this.richTextBoxMessage.TabIndex = 0;
            this.richTextBoxMessage.TabStop = false;
            this.richTextBoxMessage.Text = "<Message>";
            this.richTextBoxMessage.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richTextBoxMessage_LinkClicked);
            //
            // middleButton
            //
            this.middleButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.middleButton.AutoSize = false;
            this.middleButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.middleButton.Depth = 0;
            this.middleButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.middleButton.DrawShadows = true;
            this.middleButton.HighEmphasis = true;
            this.middleButton.Icon = null;
            this.middleButton.Location = new System.Drawing.Point(160, 163);
            this.middleButton.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.middleButton.MinimumSize = new System.Drawing.Size(0, 24);
            this.middleButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.middleButton.Name = "middleButton";
            this.middleButton.Size = new System.Drawing.Size(102, 36);
            this.middleButton.TabIndex = 3;
            this.middleButton.Text = "OK";
            this.middleButton.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Text;
            this.middleButton.UseAccentColor = false;
            this.middleButton.UseVisualStyleBackColor = true;
            this.middleButton.Visible = false;
            //
            // rightButton
            //
            this.rightButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.rightButton.AutoSize = false;
            this.rightButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.rightButton.Depth = 0;
            this.rightButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.rightButton.DrawShadows = true;
            this.rightButton.HighEmphasis = true;
            this.rightButton.Icon = null;
            this.rightButton.Location = new System.Drawing.Point(270, 163);
            this.rightButton.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.rightButton.MinimumSize = new System.Drawing.Size(0, 24);
            this.rightButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.rightButton.Name = "rightButton";
            this.rightButton.Size = new System.Drawing.Size(106, 36);
            this.rightButton.TabIndex = 0;
            this.rightButton.Text = "OK";
            this.rightButton.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.rightButton.UseAccentColor = false;
            this.rightButton.UseVisualStyleBackColor = true;
            this.rightButton.Visible = false;
            //
            // FlexibleMaterialForm
            //
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(388, 208);
            this.Controls.Add(this.rightButton);
            this.Controls.Add(this.middleButton);
            this.Controls.Add(this.messageContainer);
            this.Controls.Add(this.leftButton);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.FlexibleMaterialFormBindingSource, "CaptionText", true));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(276, 140);
            this.Name = "FlexibleMaterialForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "<Caption>";
            this.Load += new System.EventHandler(this.FlexibleMaterialForm_Load);
            this.Shown += new System.EventHandler(this.FlexibleMaterialForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.FlexibleMaterialFormBindingSource)).EndInit();
            this.messageContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxForIcon)).EndInit();
            this.ResumeLayout(false);
        }

        private MaterialButton leftButton;

        /// <summary>
        /// Defines the FlexibleMaterialFormBindingSource
        /// </summary>
        private System.Windows.Forms.BindingSource FlexibleMaterialFormBindingSource;

        /// <summary>
        /// Defines the panel1
        /// </summary>
        private System.Windows.Forms.Panel messageContainer;

        /// <summary>
        /// Defines the pictureBoxForIcon
        /// </summary>
        private System.Windows.Forms.PictureBox pictureBoxForIcon;

        private MaterialButton middleButton;
        private MaterialButton rightButton;

        //These separators are used for the "copy to clipboard" standard operation, triggered by Ctrl + C (behavior and clipboard format is like in a standard MessageBox)
        /// <summary>
        /// Defines the STANDARD_MESSAGEBOX_SEPARATOR_LINES
        /// </summary>
        private static readonly String STANDARD_MESSAGEBOX_SEPARATOR_LINES = "---------------------------\n";

        /// <summary>
        /// Defines the STANDARD_MESSAGEBOX_SEPARATOR_SPACES
        /// </summary>
        private static readonly String STANDARD_MESSAGEBOX_SEPARATOR_SPACES = "   ";

        //These are the possible buttons (in a standard MessageBox)
        private enum ButtonID
        { /// <summary>
          /// Defines the OK
          /// </summary>
            OK = 0,

            /// <summary>
            /// Defines the CANCEL
            /// </summary>
            CANCEL,

            /// <summary>
            /// Defines the YES
            /// </summary>
            YES,

            /// <summary>
            /// Defines the NO
            /// </summary>
            NO,

            /// <summary>
            /// Defines the ABORT
            /// </summary>
            ABORT,

            /// <summary>
            /// Defines the RETRY
            /// </summary>
            RETRY,

            /// <summary>
            /// Defines the IGNORE
            /// </summary>
            IGNORE
        };

        //These are the buttons texts for different languages.
        //If you want to add a new language, add it here and in the GetButtonText-Function
        private enum TwoLetterISOLanguageID
        { /// <summary>
          /// Defines the en
          /// </summary>
            en,

            /// <summary>
            /// Defines the de
            /// </summary>
            de,

            /// <summary>
            /// Defines the es
            /// </summary>
            es,

            /// <summary>
            /// Defines the it
            /// </summary>
            it
        };

        /// <summary>
        /// Defines the BUTTON_TEXTS_ENGLISH_EN
        /// </summary>
        private static readonly String[] BUTTON_TEXTS_ENGLISH_EN = { "OK", "Cancel", "&Yes", "&No", "&Abort", "&Retry", "&Ignore" };//Note: This is also the fallback language

        /// <summary>
        /// Defines the BUTTON_TEXTS_GERMAN_DE
        /// </summary>
        private static readonly String[] BUTTON_TEXTS_GERMAN_DE = { "OK", "Abbrechen", "&Ja", "&Nein", "&Abbrechen", "&Wiederholen", "&Ignorieren" };

        /// <summary>
        /// Defines the BUTTON_TEXTS_SPANISH_ES
        /// </summary>
        private static readonly String[] BUTTON_TEXTS_SPANISH_ES = { "Aceptar", "Cancelar", "&Sí", "&No", "&Abortar", "&Reintentar", "&Ignorar" };

        /// <summary>
        /// Defines the BUTTON_TEXTS_ITALIAN_IT
        /// </summary>
        private static readonly String[] BUTTON_TEXTS_ITALIAN_IT = { "OK", "Annulla", "&Sì", "&No", "&Interrompi", "&Riprova", "&Ignora" };

        /// <summary>
        /// Defines the defaultButton
        /// </summary>
        private MessageBoxDefaultButton defaultButton;

        /// <summary>
        /// Defines the visibleButtonsCount
        /// </summary>
        private int visibleButtonsCount;

        /// <summary>
        /// Defines the languageID
        /// </summary>
        private TwoLetterISOLanguageID languageID = TwoLetterISOLanguageID.en;

        /// <summary>
        /// Prevents a default instance of the <see cref="FlexibleMaterialForm"/> class from being created.
        /// </summary>
        private FlexibleMaterialForm()
        {
            InitializeComponent();

            //Try to evaluate the language. If this fails, the fallback language English will be used
            Enum.TryParse<TwoLetterISOLanguageID>(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, out this.languageID);

            this.KeyPreview = true;
            this.KeyUp += FlexibleMaterialForm_KeyUp;

            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            FONT = materialSkinManager.getFontByType(MaterialSkinManager.fontType.Body1);
        }

        /// <summary>
        /// Gets the string rows.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The string rows as 1-dimensional array</returns>
        private static string[] GetStringRows(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return null;
            }

            var messageRows = message.Split(new char[] { '\n' }, StringSplitOptions.None);
            return messageRows;
        }

        /// <summary>
        /// Gets the button text for the CurrentUICulture language.
        /// Note: The fallback language is English
        /// </summary>
        /// <param name="buttonID">The ID of the button.</param>
        /// <returns>The button text</returns>
        private string GetButtonText(ButtonID buttonID)
        {
            var buttonTextArrayIndex = Convert.ToInt32(buttonID);

            switch (this.languageID)
            {
                case TwoLetterISOLanguageID.de: return BUTTON_TEXTS_GERMAN_DE[buttonTextArrayIndex];
                case TwoLetterISOLanguageID.es: return BUTTON_TEXTS_SPANISH_ES[buttonTextArrayIndex];
                case TwoLetterISOLanguageID.it: return BUTTON_TEXTS_ITALIAN_IT[buttonTextArrayIndex];

                default: return BUTTON_TEXTS_ENGLISH_EN[buttonTextArrayIndex];
            }
        }

        /// <summary>
        /// Ensure the given working area factor in the range of  0.2 - 1.0 where:
        ///
        /// 0.2 means:  20 percent of the working area height or width.
        /// 1.0 means:  100 percent of the working area height or width.
        /// </summary>
        /// <param name="workingAreaFactor">The given working area factor.</param>
        /// <returns>The corrected given working area factor.</returns>
        private static double GetCorrectedWorkingAreaFactor(double workingAreaFactor)
        {
            const double MIN_FACTOR = 0.2;
            const double MAX_FACTOR = 1.0;

            if (workingAreaFactor < MIN_FACTOR)
            {
                return MIN_FACTOR;
            }

            if (workingAreaFactor > MAX_FACTOR)
            {
                return MAX_FACTOR;
            }

            return workingAreaFactor;
        }

        /// <summary>
        /// Set the dialogs start position when given.
        /// Otherwise center the dialog on the current screen.
        /// </summary>
        /// <param name="FlexibleMaterialForm">The FlexibleMessageBox dialog.</param>
        /// <param name="owner">The owner.</param>
        private static void SetDialogStartPosition(FlexibleMaterialForm FlexibleMaterialForm, IWin32Window owner)
        {
            //If no owner given: Center on current screen
            if (owner == null)
            {
                var screen = Screen.FromPoint(Cursor.Position);
                FlexibleMaterialForm.StartPosition = FormStartPosition.Manual;
                FlexibleMaterialForm.Left = screen.Bounds.Left + screen.Bounds.Width / 2 - FlexibleMaterialForm.Width / 2;
                FlexibleMaterialForm.Top = screen.Bounds.Top + screen.Bounds.Height / 2 - FlexibleMaterialForm.Height / 2;
            }
        }

        /// <summary>
        /// Calculate the dialogs start size (Try to auto-size width to show longest text row).
        /// Also set the maximum dialog size.
        /// </summary>
        /// <param name="FlexibleMaterialForm">The FlexibleMessageBox dialog.</param>
        /// <param name="text">The text (the longest text row is used to calculate the dialog width).</param>
        /// <param name="caption">The caption<see cref="string"/></param>
        private static void SetDialogSizes(FlexibleMaterialForm FlexibleMaterialForm, string text, string caption)
        {
            //First set the bounds for the maximum dialog size
            FlexibleMaterialForm.MaximumSize = new Size(Convert.ToInt32(SystemInformation.WorkingArea.Width * FlexibleMaterialForm.GetCorrectedWorkingAreaFactor(MAX_WIDTH_FACTOR)),
                                                          Convert.ToInt32(SystemInformation.WorkingArea.Height * FlexibleMaterialForm.GetCorrectedWorkingAreaFactor(MAX_HEIGHT_FACTOR)));

            //Get rows. Exit if there are no rows to render...
            var stringRows = GetStringRows(text);
            if (stringRows == null)
            {
                return;
            }

            //Calculate whole text height
            var textHeight = Math.Min(TextRenderer.MeasureText(text, FONT).Height, 600);

            //Calculate width for longest text line
            const int SCROLLBAR_WIDTH_OFFSET = 15;
            var longestTextRowWidth = stringRows.Max(textForRow => TextRenderer.MeasureText(textForRow, FONT).Width);
            var captionWidth = TextRenderer.MeasureText(caption, SystemFonts.CaptionFont).Width;
            var textWidth = Math.Max(longestTextRowWidth + SCROLLBAR_WIDTH_OFFSET, captionWidth);

            //Calculate margins
            var marginWidth = FlexibleMaterialForm.Width - FlexibleMaterialForm.richTextBoxMessage.Width;
            var marginHeight = FlexibleMaterialForm.Height - FlexibleMaterialForm.richTextBoxMessage.Height;

            //Set calculated dialog size (if the calculated values exceed the maximums, they were cut by windows forms automatically)
            FlexibleMaterialForm.Size = new Size(textWidth + marginWidth,
                                                   textHeight + marginHeight);
        }

        /// <summary>
        /// Set the dialogs icon.
        /// When no icon is used: Correct placement and width of rich text box.
        /// </summary>
        /// <param name="FlexibleMaterialForm">The FlexibleMessageBox dialog.</param>
        /// <param name="icon">The MessageBoxIcon.</param>
        private static void SetDialogIcon(FlexibleMaterialForm FlexibleMaterialForm, MessageBoxIcon icon)
        {
            switch (icon)
            {
                case MessageBoxIcon.Information:
                    FlexibleMaterialForm.pictureBoxForIcon.Image = SystemIcons.Information.ToBitmap();
                    break;

                case MessageBoxIcon.Warning:
                    FlexibleMaterialForm.pictureBoxForIcon.Image = SystemIcons.Warning.ToBitmap();
                    break;

                case MessageBoxIcon.Error:
                    FlexibleMaterialForm.pictureBoxForIcon.Image = SystemIcons.Error.ToBitmap();
                    break;

                case MessageBoxIcon.Question:
                    FlexibleMaterialForm.pictureBoxForIcon.Image = SystemIcons.Question.ToBitmap();
                    break;

                default:
                    //When no icon is used: Correct placement and width of rich text box.
                    FlexibleMaterialForm.pictureBoxForIcon.Visible = false;
                    FlexibleMaterialForm.richTextBoxMessage.Left -= FlexibleMaterialForm.pictureBoxForIcon.Width;
                    FlexibleMaterialForm.richTextBoxMessage.Width += FlexibleMaterialForm.pictureBoxForIcon.Width;
                    break;
            }
        }

        /// <summary>
        /// Set dialog buttons visibilities and texts.
        /// Also set a default button.
        /// </summary>
        /// <param name="FlexibleMaterialForm">The FlexibleMessageBox dialog.</param>
        /// <param name="buttons">The buttons.</param>
        /// <param name="defaultButton">The default button.</param>
        private static void SetDialogButtons(FlexibleMaterialForm FlexibleMaterialForm, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        {
            //Set the buttons visibilities and texts
            switch (buttons)
            {
                case MessageBoxButtons.AbortRetryIgnore:
                    FlexibleMaterialForm.visibleButtonsCount = 3;

                    FlexibleMaterialForm.leftButton.Visible = true;
                    FlexibleMaterialForm.leftButton.Text = FlexibleMaterialForm.GetButtonText(ButtonID.ABORT);
                    FlexibleMaterialForm.leftButton.DialogResult = DialogResult.Abort;

                    FlexibleMaterialForm.middleButton.Visible = true;
                    FlexibleMaterialForm.middleButton.Text = FlexibleMaterialForm.GetButtonText(ButtonID.RETRY);
                    FlexibleMaterialForm.middleButton.DialogResult = DialogResult.Retry;

                    FlexibleMaterialForm.rightButton.Visible = true;
                    FlexibleMaterialForm.rightButton.Text = FlexibleMaterialForm.GetButtonText(ButtonID.IGNORE);
                    FlexibleMaterialForm.rightButton.DialogResult = DialogResult.Ignore;

                    FlexibleMaterialForm.ControlBox = false;
                    break;

                case MessageBoxButtons.OKCancel:
                    FlexibleMaterialForm.visibleButtonsCount = 2;

                    FlexibleMaterialForm.middleButton.Visible = true;
                    FlexibleMaterialForm.middleButton.Text = FlexibleMaterialForm.GetButtonText(ButtonID.CANCEL);
                    FlexibleMaterialForm.middleButton.DialogResult = DialogResult.Cancel;

                    FlexibleMaterialForm.rightButton.Visible = true;
                    FlexibleMaterialForm.rightButton.Text = FlexibleMaterialForm.GetButtonText(ButtonID.OK);
                    FlexibleMaterialForm.rightButton.DialogResult = DialogResult.OK;

                    FlexibleMaterialForm.CancelButton = FlexibleMaterialForm.middleButton;
                    break;

                case MessageBoxButtons.RetryCancel:
                    FlexibleMaterialForm.visibleButtonsCount = 2;

                    FlexibleMaterialForm.middleButton.Visible = true;
                    FlexibleMaterialForm.middleButton.Text = FlexibleMaterialForm.GetButtonText(ButtonID.CANCEL);
                    FlexibleMaterialForm.middleButton.DialogResult = DialogResult.Cancel;

                    FlexibleMaterialForm.rightButton.Visible = true;
                    FlexibleMaterialForm.rightButton.Text = FlexibleMaterialForm.GetButtonText(ButtonID.RETRY);
                    FlexibleMaterialForm.rightButton.DialogResult = DialogResult.Retry;

                    FlexibleMaterialForm.CancelButton = FlexibleMaterialForm.middleButton;
                    break;

                case MessageBoxButtons.YesNo:
                    FlexibleMaterialForm.visibleButtonsCount = 2;

                    FlexibleMaterialForm.middleButton.Visible = true;
                    FlexibleMaterialForm.middleButton.Text = FlexibleMaterialForm.GetButtonText(ButtonID.NO);
                    FlexibleMaterialForm.middleButton.DialogResult = DialogResult.No;

                    FlexibleMaterialForm.rightButton.Visible = true;
                    FlexibleMaterialForm.rightButton.Text = FlexibleMaterialForm.GetButtonText(ButtonID.YES);
                    FlexibleMaterialForm.rightButton.DialogResult = DialogResult.Yes;

                    FlexibleMaterialForm.ControlBox = false;
                    break;

                case MessageBoxButtons.YesNoCancel:
                    FlexibleMaterialForm.visibleButtonsCount = 3;

                    FlexibleMaterialForm.rightButton.Visible = true;
                    FlexibleMaterialForm.rightButton.Text = FlexibleMaterialForm.GetButtonText(ButtonID.YES);
                    FlexibleMaterialForm.rightButton.DialogResult = DialogResult.Yes;

                    FlexibleMaterialForm.middleButton.Visible = true;
                    FlexibleMaterialForm.middleButton.Text = FlexibleMaterialForm.GetButtonText(ButtonID.NO);
                    FlexibleMaterialForm.middleButton.DialogResult = DialogResult.No;

                    FlexibleMaterialForm.leftButton.Visible = true;
                    FlexibleMaterialForm.leftButton.Text = FlexibleMaterialForm.GetButtonText(ButtonID.CANCEL);
                    FlexibleMaterialForm.leftButton.DialogResult = DialogResult.Cancel;

                    FlexibleMaterialForm.CancelButton = FlexibleMaterialForm.leftButton;
                    break;

                case MessageBoxButtons.OK:
                default:
                    FlexibleMaterialForm.visibleButtonsCount = 1;
                    FlexibleMaterialForm.rightButton.Visible = true;
                    FlexibleMaterialForm.rightButton.Text = FlexibleMaterialForm.GetButtonText(ButtonID.OK);
                    FlexibleMaterialForm.rightButton.DialogResult = DialogResult.OK;

                    FlexibleMaterialForm.CancelButton = FlexibleMaterialForm.rightButton;
                    break;
            }

            //Set default button (used in FlexibleMaterialForm_Shown)
            FlexibleMaterialForm.defaultButton = defaultButton;
        }

        /// <summary>
        /// Handles the Shown event of the FlexibleMaterialForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void FlexibleMaterialForm_Shown(object sender, EventArgs e)
        {
            int buttonIndexToFocus = 1;
            Button buttonToFocus;

            //Set the default button...
            switch (this.defaultButton)
            {
                case MessageBoxDefaultButton.Button1:
                default:
                    buttonIndexToFocus = 1;
                    break;

                case MessageBoxDefaultButton.Button2:
                    buttonIndexToFocus = 2;
                    break;

                case MessageBoxDefaultButton.Button3:
                    buttonIndexToFocus = 3;
                    break;
            }

            if (buttonIndexToFocus > this.visibleButtonsCount)
            {
                buttonIndexToFocus = this.visibleButtonsCount;
            }

            if (buttonIndexToFocus == 3)
            {
                buttonToFocus = this.rightButton;
            }
            else if (buttonIndexToFocus == 2)
            {
                buttonToFocus = this.middleButton;
            }
            else
            {
                buttonToFocus = this.leftButton;
            }

            buttonToFocus.Focus();
        }

        /// <summary>
        /// Handles the LinkClicked event of the richTextBoxMessage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkClickedEventArgs"/> instance containing the event data.</param>
        private void richTextBoxMessage_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                Process.Start(e.LinkText);
            }
            catch (Exception)
            {
                //Let the caller of FlexibleMaterialForm decide what to do with this exception...
                throw;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>
        /// Handles the KeyUp event of the richTextBoxMessage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        internal void FlexibleMaterialForm_KeyUp(object sender, KeyEventArgs e)
        {
            //Handle standard key strikes for clipboard copy: "Ctrl + C" and "Ctrl + Insert"
            if (e.Control && (e.KeyCode == Keys.C || e.KeyCode == Keys.Insert))
            {
                var buttonsTextLine = (this.leftButton.Visible ? this.leftButton.Text + STANDARD_MESSAGEBOX_SEPARATOR_SPACES : string.Empty)
                                    + (this.middleButton.Visible ? this.middleButton.Text + STANDARD_MESSAGEBOX_SEPARATOR_SPACES : string.Empty)
                                    + (this.rightButton.Visible ? this.rightButton.Text + STANDARD_MESSAGEBOX_SEPARATOR_SPACES : string.Empty);

                //Build same clipboard text like the standard .Net MessageBox
                var textForClipboard = STANDARD_MESSAGEBOX_SEPARATOR_LINES
                                     + this.Text + Environment.NewLine
                                     + STANDARD_MESSAGEBOX_SEPARATOR_LINES
                                     + this.richTextBoxMessage.Text + Environment.NewLine
                                     + STANDARD_MESSAGEBOX_SEPARATOR_LINES
                                     + buttonsTextLine.Replace("&", string.Empty) + Environment.NewLine
                                     + STANDARD_MESSAGEBOX_SEPARATOR_LINES;

                //Set text in clipboard
                Clipboard.SetText(textForClipboard);
            }
        }

        /// <summary>
        /// Gets or sets the CaptionText
        /// The text that is been used for the heading.
        /// </summary>
        public string CaptionText { get; set; }

        /// <summary>
        /// Gets or sets the MessageText
        /// The text that is been used in the FlexibleMaterialForm.
        /// </summary>
        public string MessageText { get; set; }

        /// <summary>
        /// Shows the specified message box.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="text">The text.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="buttons">The buttons.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="defaultButton">The default button.</param>
        /// <returns>The dialog result.</returns>
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
        {
            //Create a new instance of the FlexibleMessageBox form
            var FlexibleMaterialForm = new FlexibleMaterialForm();
            FlexibleMaterialForm.ShowInTaskbar = false;

            //Bind the caption and the message text
            FlexibleMaterialForm.CaptionText = caption;
            FlexibleMaterialForm.MessageText = text;
            FlexibleMaterialForm.FlexibleMaterialFormBindingSource.DataSource = FlexibleMaterialForm;

            //Set the buttons visibilities and texts. Also set a default button.
            SetDialogButtons(FlexibleMaterialForm, buttons, defaultButton);

            //Set the dialogs icon. When no icon is used: Correct placement and width of rich text box.
            SetDialogIcon(FlexibleMaterialForm, icon);

            //Set the font for all controls
            FlexibleMaterialForm.Font = FONT;
            FlexibleMaterialForm.richTextBoxMessage.Font = FONT;

            //Calculate the dialogs start size (Try to auto-size width to show longest text row). Also set the maximum dialog size.
            SetDialogSizes(FlexibleMaterialForm, text, caption);

            //Set the dialogs start position when given. Otherwise center the dialog on the current screen.
            SetDialogStartPosition(FlexibleMaterialForm, owner);

            //Show the dialog
            return FlexibleMaterialForm.ShowDialog(owner);
        }

        private void FlexibleMaterialForm_Load(object sender, EventArgs e)
        {
        }
    }
}