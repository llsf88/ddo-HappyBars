namespace UiRuler
{
    partial class IngameUI
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            chkEnabled = new System.Windows.Forms.CheckBox();
            nudGridSpacing = new System.Windows.Forms.NumericUpDown();
            lblGrid = new System.Windows.Forms.Label();
            nudOffsetX = new System.Windows.Forms.NumericUpDown();
            nudOffsetY = new System.Windows.Forms.NumericUpDown();
            lblOffsetX = new System.Windows.Forms.Label();
            lblOffsetY = new System.Windows.Forms.Label();
            lblTarget = new System.Windows.Forms.Label();
            cboTargetElement = new System.Windows.Forms.ComboBox();
            btnLogMeasurement = new System.Windows.Forms.Button();
            btnCaptureSnapshot = new System.Windows.Forms.Button();
            chkShowLabels = new System.Windows.Forms.CheckBox();
            chkShowGrid = new System.Windows.Forms.CheckBox();
            chkShowCrosshair = new System.Windows.Forms.CheckBox();
            lblStatus = new System.Windows.Forms.Label();
            txtConversation = new System.Windows.Forms.TextBox();
            txtLog = new System.Windows.Forms.TextBox();
            btnCopyDetails = new System.Windows.Forms.Button();
            btnCopyLog = new System.Windows.Forms.Button();
            btnSelectDetails = new System.Windows.Forms.Button();
            btnSetHotbars = new System.Windows.Forms.Button();
            btnSaveHotbars = new System.Windows.Forms.Button();
            btnLoadHotbars = new System.Windows.Forms.Button();
            cboHotbarSaveFiles = new System.Windows.Forms.ComboBox();
            txtHotbarLayout = new System.Windows.Forms.TextBox();
            lnkHotbarSavePath = new System.Windows.Forms.LinkLabel();
            tabTools = new System.Windows.Forms.TabControl();
            tabUiRuler = new System.Windows.Forms.TabPage();
            tabHappyBars = new System.Windows.Forms.TabPage();
            chkSnapHotbars = new System.Windows.Forms.CheckBox();
            lblHorizontalGap = new System.Windows.Forms.Label();
            nudHorizontalGap = new System.Windows.Forms.NumericUpDown();
            lblVerticalGap = new System.Windows.Forms.Label();
            nudVerticalGap = new System.Windows.Forms.NumericUpDown();
            tabHowToUse = new System.Windows.Forms.TabPage();
            txtHowToUse = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)nudGridSpacing).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudOffsetX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudOffsetY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudHorizontalGap).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudVerticalGap).BeginInit();
            tabTools.SuspendLayout();
            tabUiRuler.SuspendLayout();
            tabHappyBars.SuspendLayout();
            tabHowToUse.SuspendLayout();
            SuspendLayout();
            // 
            // chkEnabled
            // 
            chkEnabled.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            chkEnabled.AutoSize = true;
            chkEnabled.Location = new System.Drawing.Point(12, 12);
            chkEnabled.Name = "chkEnabled";
            chkEnabled.Size = new System.Drawing.Size(133, 29);
            chkEnabled.TabIndex = 0;
            chkEnabled.Text = "Enable ruler";
            chkEnabled.UseVisualStyleBackColor = true;
            chkEnabled.CheckedChanged += chkEnabled_CheckedChanged;
            // 
            // nudGridSpacing
            // 
            nudGridSpacing.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            nudGridSpacing.Increment = new decimal(new int[] { 5, 0, 0, 0 });
            nudGridSpacing.Location = new System.Drawing.Point(123, 50);
            nudGridSpacing.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            nudGridSpacing.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            nudGridSpacing.Name = "nudGridSpacing";
            nudGridSpacing.Size = new System.Drawing.Size(80, 31);
            nudGridSpacing.TabIndex = 1;
            nudGridSpacing.Value = new decimal(new int[] { 10, 0, 0, 0 });
            nudGridSpacing.ValueChanged += nudGridSpacing_ValueChanged;
            // 
            // lblGrid
            // 
            lblGrid.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            lblGrid.AutoSize = true;
            lblGrid.Location = new System.Drawing.Point(12, 52);
            lblGrid.Name = "lblGrid";
            lblGrid.Size = new System.Drawing.Size(105, 25);
            lblGrid.TabIndex = 2;
            lblGrid.Text = "Grid spacing";
            // 
            // nudOffsetX
            // 
            nudOffsetX.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            nudOffsetX.Location = new System.Drawing.Point(295, 12);
            nudOffsetX.Minimum = new decimal(new int[] { 2000, 0, 0, int.MinValue });
            nudOffsetX.Name = "nudOffsetX";
            nudOffsetX.Size = new System.Drawing.Size(80, 31);
            nudOffsetX.TabIndex = 3;
            nudOffsetX.ValueChanged += nudOffset_ValueChanged;
            // 
            // nudOffsetY
            // 
            nudOffsetY.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            nudOffsetY.Location = new System.Drawing.Point(473, 12);
            nudOffsetY.Minimum = new decimal(new int[] { 2000, 0, 0, int.MinValue });
            nudOffsetY.Name = "nudOffsetY";
            nudOffsetY.Size = new System.Drawing.Size(80, 31);
            nudOffsetY.TabIndex = 4;
            nudOffsetY.ValueChanged += nudOffset_ValueChanged;
            // 
            // lblOffsetX
            // 
            lblOffsetX.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            lblOffsetX.AutoSize = true;
            lblOffsetX.Location = new System.Drawing.Point(221, 14);
            lblOffsetX.Name = "lblOffsetX";
            lblOffsetX.Size = new System.Drawing.Size(68, 25);
            lblOffsetX.TabIndex = 5;
            lblOffsetX.Text = "X offset";
            // 
            // lblOffsetY
            // 
            lblOffsetY.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            lblOffsetY.AutoSize = true;
            lblOffsetY.Location = new System.Drawing.Point(399, 14);
            lblOffsetY.Name = "lblOffsetY";
            lblOffsetY.Size = new System.Drawing.Size(68, 25);
            lblOffsetY.TabIndex = 6;
            lblOffsetY.Text = "Y offset";
            // 
            // lblTarget
            // 
            lblTarget.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            lblTarget.AutoSize = true;
            lblTarget.Location = new System.Drawing.Point(221, 53);
            lblTarget.Name = "lblTarget";
            lblTarget.Size = new System.Drawing.Size(59, 25);
            lblTarget.TabIndex = 7;
            lblTarget.Text = "Target";
            // 
            // cboTargetElement
            // 
            cboTargetElement.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            cboTargetElement.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboTargetElement.FormattingEnabled = true;
            cboTargetElement.Location = new System.Drawing.Point(286, 49);
            cboTargetElement.Name = "cboTargetElement";
            cboTargetElement.Size = new System.Drawing.Size(343, 33);
            cboTargetElement.TabIndex = 8;
            // 
            // btnLogMeasurement
            // 
            btnLogMeasurement.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnLogMeasurement.Location = new System.Drawing.Point(633, 47);
            btnLogMeasurement.Name = "btnLogMeasurement";
            btnLogMeasurement.Size = new System.Drawing.Size(136, 38);
            btnLogMeasurement.TabIndex = 9;
            btnLogMeasurement.Text = "Capture Click";
            btnLogMeasurement.UseVisualStyleBackColor = true;
            btnLogMeasurement.Click += btnLogMeasurement_Click;
            // 
            // btnCaptureSnapshot
            // 
            btnCaptureSnapshot.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnCaptureSnapshot.Location = new System.Drawing.Point(775, 47);
            btnCaptureSnapshot.Name = "btnCaptureSnapshot";
            btnCaptureSnapshot.Size = new System.Drawing.Size(141, 38);
            btnCaptureSnapshot.TabIndex = 10;
            btnCaptureSnapshot.Text = "Save Snapshot";
            btnCaptureSnapshot.UseVisualStyleBackColor = true;
            btnCaptureSnapshot.Click += btnCaptureSnapshot_Click;
            // 
            // chkShowLabels
            // 
            chkShowLabels.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            chkShowLabels.AutoSize = true;
            chkShowLabels.Location = new System.Drawing.Point(12, 95);
            chkShowLabels.Name = "chkShowLabels";
            chkShowLabels.Size = new System.Drawing.Size(132, 29);
            chkShowLabels.TabIndex = 11;
            chkShowLabels.Text = "Show labels";
            chkShowLabels.UseVisualStyleBackColor = true;
            chkShowLabels.CheckedChanged += chkShowLabels_CheckedChanged;
            // 
            // chkShowGrid
            // 
            chkShowGrid.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            chkShowGrid.AutoSize = true;
            chkShowGrid.Checked = true;
            chkShowGrid.CheckState = System.Windows.Forms.CheckState.Checked;
            chkShowGrid.Location = new System.Drawing.Point(150, 95);
            chkShowGrid.Name = "chkShowGrid";
            chkShowGrid.Size = new System.Drawing.Size(112, 29);
            chkShowGrid.TabIndex = 12;
            chkShowGrid.Text = "Show grid";
            chkShowGrid.UseVisualStyleBackColor = true;
            chkShowGrid.CheckedChanged += chkShowGrid_CheckedChanged;
            // 
            // chkShowCrosshair
            // 
            chkShowCrosshair.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            chkShowCrosshair.AutoSize = true;
            chkShowCrosshair.Checked = true;
            chkShowCrosshair.CheckState = System.Windows.Forms.CheckState.Checked;
            chkShowCrosshair.Location = new System.Drawing.Point(268, 95);
            chkShowCrosshair.Name = "chkShowCrosshair";
            chkShowCrosshair.Size = new System.Drawing.Size(146, 29);
            chkShowCrosshair.TabIndex = 13;
            chkShowCrosshair.Text = "Show crosshair";
            chkShowCrosshair.UseVisualStyleBackColor = true;
            chkShowCrosshair.CheckedChanged += chkShowCrosshair_CheckedChanged;
            // 
            // btnSetHotbars
            // 
            btnSetHotbars.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            btnSetHotbars.Location = new System.Drawing.Point(12, 12);
            btnSetHotbars.Name = "btnSetHotbars";
            btnSetHotbars.Size = new System.Drawing.Size(118, 36);
            btnSetHotbars.TabIndex = 27;
            btnSetHotbars.Text = "Set Hotbars";
            btnSetHotbars.UseVisualStyleBackColor = true;
            btnSetHotbars.Click += btnSetHotbars_Click;
            // 
            // btnSaveHotbars
            // 
            btnSaveHotbars.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            btnSaveHotbars.Location = new System.Drawing.Point(136, 12);
            btnSaveHotbars.Name = "btnSaveHotbars";
            btnSaveHotbars.Size = new System.Drawing.Size(128, 36);
            btnSaveHotbars.TabIndex = 28;
            btnSaveHotbars.Text = "Save Hotbars";
            btnSaveHotbars.UseVisualStyleBackColor = true;
            btnSaveHotbars.Click += btnSaveHotbars_Click;
            // 
            // btnLoadHotbars
            // 
            btnLoadHotbars.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            btnLoadHotbars.Location = new System.Drawing.Point(270, 12);
            btnLoadHotbars.Name = "btnLoadHotbars";
            btnLoadHotbars.Size = new System.Drawing.Size(128, 36);
            btnLoadHotbars.TabIndex = 29;
            btnLoadHotbars.Text = "Load Hotbars";
            btnLoadHotbars.UseVisualStyleBackColor = true;
            btnLoadHotbars.Click += btnLoadHotbars_Click;
            // 
            // cboHotbarSaveFiles
            // 
            cboHotbarSaveFiles.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            cboHotbarSaveFiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboHotbarSaveFiles.FormattingEnabled = true;
            cboHotbarSaveFiles.Location = new System.Drawing.Point(404, 14);
            cboHotbarSaveFiles.Name = "cboHotbarSaveFiles";
            cboHotbarSaveFiles.Size = new System.Drawing.Size(260, 33);
            cboHotbarSaveFiles.TabIndex = 37;
            cboHotbarSaveFiles.DropDown += cboHotbarSaveFiles_DropDown;
            // 
            // lblStatus
            // 
            lblStatus.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lblStatus.AutoSize = true;
            lblStatus.Location = new System.Drawing.Point(12, 136);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new System.Drawing.Size(153, 25);
            lblStatus.TabIndex = 15;
            lblStatus.Text = "Waiting for popup.";
            // 
            // txtConversation
            // 
            txtConversation.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtConversation.Font = new System.Drawing.Font("Consolas", 9F);
            txtConversation.HideSelection = false;
            txtConversation.Location = new System.Drawing.Point(12, 336);
            txtConversation.Multiline = true;
            txtConversation.Name = "txtConversation";
            txtConversation.ReadOnly = true;
            txtConversation.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            txtConversation.ShortcutsEnabled = true;
            txtConversation.Size = new System.Drawing.Size(884, 214);
            txtConversation.TabIndex = 17;
            // 
            // txtLog
            // 
            txtLog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtLog.Font = new System.Drawing.Font("Consolas", 9F);
            txtLog.HideSelection = false;
            txtLog.Location = new System.Drawing.Point(12, 166);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            txtLog.ShortcutsEnabled = true;
            txtLog.Size = new System.Drawing.Size(904, 80);
            txtLog.TabIndex = 16;
            // 
            // btnCopyDetails
            // 
            btnCopyDetails.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            btnCopyDetails.Location = new System.Drawing.Point(12, 294);
            btnCopyDetails.Name = "btnCopyDetails";
            btnCopyDetails.Size = new System.Drawing.Size(122, 36);
            btnCopyDetails.TabIndex = 18;
            btnCopyDetails.Text = "Copy Details";
            btnCopyDetails.UseVisualStyleBackColor = true;
            btnCopyDetails.Click += btnCopyDetails_Click;
            // 
            // btnCopyLog
            // 
            btnCopyLog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            btnCopyLog.Location = new System.Drawing.Point(140, 294);
            btnCopyLog.Name = "btnCopyLog";
            btnCopyLog.Size = new System.Drawing.Size(100, 36);
            btnCopyLog.TabIndex = 19;
            btnCopyLog.Text = "Copy Log";
            btnCopyLog.UseVisualStyleBackColor = true;
            btnCopyLog.Click += btnCopyLog_Click;
            // 
            // btnSelectDetails
            // 
            btnSelectDetails.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            btnSelectDetails.Location = new System.Drawing.Point(246, 294);
            btnSelectDetails.Name = "btnSelectDetails";
            btnSelectDetails.Size = new System.Drawing.Size(126, 36);
            btnSelectDetails.TabIndex = 20;
            btnSelectDetails.Text = "Select Details";
            btnSelectDetails.UseVisualStyleBackColor = true;
            btnSelectDetails.Click += btnSelectDetails_Click;
            // 
            // txtHotbarLayout
            // 
            txtHotbarLayout.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtHotbarLayout.Font = new System.Drawing.Font("Consolas", 9F);
            txtHotbarLayout.Location = new System.Drawing.Point(12, 124);
            txtHotbarLayout.Multiline = true;
            txtHotbarLayout.Name = "txtHotbarLayout";
            txtHotbarLayout.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            txtHotbarLayout.Size = new System.Drawing.Size(884, 424);
            txtHotbarLayout.TabIndex = 30;
            txtHotbarLayout.Text = "1,5\r\n2,6\r\n3,7\r\n4,8";
            // 
            // lnkHotbarSavePath
            // 
            lnkHotbarSavePath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lnkHotbarSavePath.AutoEllipsis = true;
            lnkHotbarSavePath.Font = new System.Drawing.Font("Segoe UI", 8F);
            lnkHotbarSavePath.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            lnkHotbarSavePath.Location = new System.Drawing.Point(12, 98);
            lnkHotbarSavePath.Name = "lnkHotbarSavePath";
            lnkHotbarSavePath.Size = new System.Drawing.Size(884, 20);
            lnkHotbarSavePath.TabIndex = 36;
            lnkHotbarSavePath.TabStop = true;
            lnkHotbarSavePath.Text = "Saved hotbars path";
            lnkHotbarSavePath.Visible = false;
            lnkHotbarSavePath.LinkClicked += lnkHotbarSavePath_LinkClicked;
            // 
            // tabTools
            // 
            tabTools.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tabTools.Controls.Add(tabUiRuler);
            tabTools.Controls.Add(tabHappyBars);
            tabTools.Controls.Add(tabHowToUse);
            tabTools.Location = new System.Drawing.Point(0, 0);
            tabTools.Name = "tabTools";
            tabTools.SelectedIndex = 0;
            tabTools.Size = new System.Drawing.Size(928, 600);
            tabTools.TabIndex = 31;
            // 
            // tabUiRuler
            // 
            tabUiRuler.Controls.Add(btnSelectDetails);
            tabUiRuler.Controls.Add(btnCopyLog);
            tabUiRuler.Controls.Add(btnCopyDetails);
            tabUiRuler.Controls.Add(txtConversation);
            tabUiRuler.Controls.Add(txtLog);
            tabUiRuler.Controls.Add(lblStatus);
            tabUiRuler.Controls.Add(chkShowCrosshair);
            tabUiRuler.Controls.Add(chkShowGrid);
            tabUiRuler.Controls.Add(chkShowLabels);
            tabUiRuler.Controls.Add(btnCaptureSnapshot);
            tabUiRuler.Controls.Add(btnLogMeasurement);
            tabUiRuler.Controls.Add(cboTargetElement);
            tabUiRuler.Controls.Add(lblTarget);
            tabUiRuler.Controls.Add(lblOffsetY);
            tabUiRuler.Controls.Add(lblOffsetX);
            tabUiRuler.Controls.Add(nudOffsetY);
            tabUiRuler.Controls.Add(nudOffsetX);
            tabUiRuler.Controls.Add(lblGrid);
            tabUiRuler.Controls.Add(nudGridSpacing);
            tabUiRuler.Controls.Add(chkEnabled);
            tabUiRuler.Location = new System.Drawing.Point(4, 34);
            tabUiRuler.Name = "tabUiRuler";
            tabUiRuler.Padding = new System.Windows.Forms.Padding(3);
            tabUiRuler.Size = new System.Drawing.Size(920, 562);
            tabUiRuler.TabIndex = 0;
            tabUiRuler.Text = "uiRuler";
            tabUiRuler.UseVisualStyleBackColor = true;
            // 
            // tabHappyBars
            // 
            tabHappyBars.Controls.Add(txtHotbarLayout);
            tabHappyBars.Controls.Add(lnkHotbarSavePath);
            tabHappyBars.Controls.Add(nudVerticalGap);
            tabHappyBars.Controls.Add(lblVerticalGap);
            tabHappyBars.Controls.Add(nudHorizontalGap);
            tabHappyBars.Controls.Add(lblHorizontalGap);
            tabHappyBars.Controls.Add(chkSnapHotbars);
            tabHappyBars.Controls.Add(cboHotbarSaveFiles);
            tabHappyBars.Controls.Add(btnLoadHotbars);
            tabHappyBars.Controls.Add(btnSaveHotbars);
            tabHappyBars.Controls.Add(btnSetHotbars);
            tabHappyBars.Location = new System.Drawing.Point(4, 34);
            tabHappyBars.Name = "tabHappyBars";
            tabHappyBars.Padding = new System.Windows.Forms.Padding(3);
            tabHappyBars.Size = new System.Drawing.Size(920, 562);
            tabHappyBars.TabIndex = 1;
            tabHappyBars.Text = "HappyBars";
            tabHappyBars.UseVisualStyleBackColor = true;
            // 
            // chkSnapHotbars
            // 
            chkSnapHotbars.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            chkSnapHotbars.AutoSize = true;
            chkSnapHotbars.Location = new System.Drawing.Point(420, 62);
            chkSnapHotbars.Name = "chkSnapHotbars";
            chkSnapHotbars.Size = new System.Drawing.Size(149, 29);
            chkSnapHotbars.TabIndex = 31;
            chkSnapHotbars.Text = "Snap hotbars";
            chkSnapHotbars.UseVisualStyleBackColor = true;
            chkSnapHotbars.CheckedChanged += chkSnapHotbars_CheckedChanged;
            // 
            // lblHorizontalGap
            // 
            lblHorizontalGap.AutoSize = true;
            lblHorizontalGap.Location = new System.Drawing.Point(12, 64);
            lblHorizontalGap.Name = "lblHorizontalGap";
            lblHorizontalGap.Size = new System.Drawing.Size(92, 25);
            lblHorizontalGap.TabIndex = 32;
            lblHorizontalGap.Text = "H gap px";
            // 
            // nudHorizontalGap
            // 
            nudHorizontalGap.Location = new System.Drawing.Point(110, 60);
            nudHorizontalGap.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
            nudHorizontalGap.Name = "nudHorizontalGap";
            nudHorizontalGap.Size = new System.Drawing.Size(80, 31);
            nudHorizontalGap.TabIndex = 33;
            nudHorizontalGap.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // lblVerticalGap
            // 
            lblVerticalGap.AutoSize = true;
            lblVerticalGap.Location = new System.Drawing.Point(214, 64);
            lblVerticalGap.Name = "lblVerticalGap";
            lblVerticalGap.Size = new System.Drawing.Size(88, 25);
            lblVerticalGap.TabIndex = 34;
            lblVerticalGap.Text = "V gap px";
            // 
            // nudVerticalGap
            // 
            nudVerticalGap.Location = new System.Drawing.Point(308, 60);
            nudVerticalGap.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
            nudVerticalGap.Name = "nudVerticalGap";
            nudVerticalGap.Size = new System.Drawing.Size(80, 31);
            nudVerticalGap.TabIndex = 35;
            nudVerticalGap.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // tabHowToUse
            // 
            tabHowToUse.Controls.Add(txtHowToUse);
            tabHowToUse.Location = new System.Drawing.Point(4, 34);
            tabHowToUse.Name = "tabHowToUse";
            tabHowToUse.Padding = new System.Windows.Forms.Padding(3);
            tabHowToUse.Size = new System.Drawing.Size(920, 562);
            tabHowToUse.TabIndex = 2;
            tabHowToUse.Text = "How to use";
            tabHowToUse.UseVisualStyleBackColor = true;
            // 
            // txtHowToUse
            // 
            txtHowToUse.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtHowToUse.Font = new System.Drawing.Font("Segoe UI", 10F);
            txtHowToUse.Location = new System.Drawing.Point(12, 12);
            txtHowToUse.Name = "txtHowToUse";
            txtHowToUse.ReadOnly = true;
            txtHowToUse.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            txtHowToUse.ShortcutsEnabled = true;
            txtHowToUse.Size = new System.Drawing.Size(896, 538);
            txtHowToUse.TabIndex = 0;
            txtHowToUse.Text = "Set Hotbars\r\nOrganiza as hotbars usando o texto da aba HappyBars. Cada linha vira uma linha no layout, e cada numero separado por virgula vira uma coluna.\r\n\r\nExemplo:\r\n1,5\r\n2,6\r\n3,7\r\n\r\nUse x ou 0 para deixar um espaco vazio:\r\nx,3\r\n4,6\r\n9,5\r\n\r\nUse V ao lado do numero para pedir aquela hotbar na vertical:\r\n1,5v\r\n2,6v\r\n\r\nA primeira hotbar numerica da primeira coluna vira a ancora. Se o layout nao couber perto da borda da janela do jogo, o plugin move a ancora primeiro e ajusta o bloco inteiro antes de mover as outras barras.\r\n\r\nSave Hotbars\r\nSalva as posicoes atuais das hotbars, a orientacao horizontal/vertical e o personagem atual. O arquivo de save usa o nome e o ID do personagem.\r\n\r\nLoad Hotbars\r\nCarrega o save do personagem atual. Ele procura primeiro pelo ID do personagem e depois pelo nome. Se uma hotbar precisar mudar de horizontal para vertical, ou o contrario, ele tenta rotacionar antes de mover.\r\n\r\nSnap Hotbars\r\nQuando ligado, ao arrastar uma hotbar manualmente o plugin mostra uma previa visual da barra vizinha escolhida e da posicao final. Ele escolhe a hotbar vizinha mais perto do mouse, tenta encaixar acima, abaixo, esquerda ou direita, e evita sobrepor outras hotbars.\r\n\r\nESC\r\nDurante Set Hotbars ou Load Hotbars, pressione ESC para parar o movimento das barras.";
            // 
            // IngameUI
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(928, 600);
            Controls.Add(tabTools);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Name = "IngameUI";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "UiRuler";
            tabTools.ResumeLayout(false);
            tabUiRuler.ResumeLayout(false);
            tabUiRuler.PerformLayout();
            tabHappyBars.ResumeLayout(false);
            tabHappyBars.PerformLayout();
            tabHowToUse.ResumeLayout(false);
            tabHowToUse.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudGridSpacing).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudOffsetX).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudOffsetY).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudHorizontalGap).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudVerticalGap).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.CheckBox chkEnabled;
        private System.Windows.Forms.NumericUpDown nudGridSpacing;
        private System.Windows.Forms.Label lblGrid;
        private System.Windows.Forms.NumericUpDown nudOffsetX;
        private System.Windows.Forms.NumericUpDown nudOffsetY;
        private System.Windows.Forms.Label lblOffsetX;
        private System.Windows.Forms.Label lblOffsetY;
        private System.Windows.Forms.Label lblTarget;
        private System.Windows.Forms.ComboBox cboTargetElement;
        private System.Windows.Forms.Button btnLogMeasurement;
        private System.Windows.Forms.Button btnCaptureSnapshot;
        private System.Windows.Forms.CheckBox chkShowLabels;
        private System.Windows.Forms.CheckBox chkShowGrid;
        private System.Windows.Forms.CheckBox chkShowCrosshair;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TextBox txtConversation;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button btnCopyDetails;
        private System.Windows.Forms.Button btnCopyLog;
        private System.Windows.Forms.Button btnSelectDetails;
        private System.Windows.Forms.Button btnSetHotbars;
        private System.Windows.Forms.Button btnSaveHotbars;
        private System.Windows.Forms.Button btnLoadHotbars;
        private System.Windows.Forms.ComboBox cboHotbarSaveFiles;
        private System.Windows.Forms.TextBox txtHotbarLayout;
        private System.Windows.Forms.TabControl tabTools;
        private System.Windows.Forms.TabPage tabUiRuler;
        private System.Windows.Forms.TabPage tabHappyBars;
        private System.Windows.Forms.CheckBox chkSnapHotbars;
        private System.Windows.Forms.Label lblHorizontalGap;
        private System.Windows.Forms.NumericUpDown nudHorizontalGap;
        private System.Windows.Forms.Label lblVerticalGap;
        private System.Windows.Forms.NumericUpDown nudVerticalGap;
        private System.Windows.Forms.LinkLabel lnkHotbarSavePath;
        private System.Windows.Forms.TabPage tabHowToUse;
        private System.Windows.Forms.RichTextBox txtHowToUse;
    }
}
