namespace DefinetlyNotAFishingBot {
  partial class frmMain {
    /// <summary>
    /// Erforderliche Designervariable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Verwendete Ressourcen bereinigen.
    /// </summary>
    /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Vom Windows Form-Designer generierter Code

    /// <summary>
    /// Erforderliche Methode für die Designerunterstützung.
    /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.StartBtn = new System.Windows.Forms.Button();
      this.StopBtn = new System.Windows.Forms.Button();
      this.StatusLab = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.FishingKeyCB = new System.Windows.Forms.ComboBox();
      this.LureKeyCB = new System.Windows.Forms.ComboBox();
      this.BuffKeyCB = new System.Windows.Forms.ComboBox();
      this.OutfitKeyCB = new System.Windows.Forms.ComboBox();
      this.label5 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.RefishTimeSlider = new System.Windows.Forms.TrackBar();
      this.LootTimeSlider = new System.Windows.Forms.TrackBar();
      this.RefishTimeDisplayLab = new System.Windows.Forms.Label();
      this.LootTimeDisplayLab = new System.Windows.Forms.Label();
      this.FishingKeySetBtn = new System.Windows.Forms.Button();
      this.LureKeySetBtn = new System.Windows.Forms.Button();
      this.BuffKeySetBtn = new System.Windows.Forms.Button();
      this.OutfitKeySetBtn = new System.Windows.Forms.Button();
      this.ResetFormBtn = new System.Windows.Forms.Button();
      this.SelectColorPic = new System.Windows.Forms.PictureBox();
      this.ColorDisplayLab = new System.Windows.Forms.Label();
      this.GeneralTicker = new System.Windows.Forms.Timer(this.components);
      this.ColorTolleranceSlider = new System.Windows.Forms.TrackBar();
      this.ColorTollDisplayLab = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this.EscHintLab = new System.Windows.Forms.Label();
      this.CaughtCountLab = new System.Windows.Forms.Label();
      this.HideUiChk = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.RefishTimeSlider)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.LootTimeSlider)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.SelectColorPic)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ColorTolleranceSlider)).BeginInit();
      this.SuspendLayout();
      // 
      // StartBtn
      // 
      this.StartBtn.Location = new System.Drawing.Point(12, 12);
      this.StartBtn.Name = "StartBtn";
      this.StartBtn.Size = new System.Drawing.Size(75, 23);
      this.StartBtn.TabIndex = 0;
      this.StartBtn.Text = "Start";
      this.StartBtn.UseVisualStyleBackColor = true;
      // 
      // StopBtn
      // 
      this.StopBtn.Location = new System.Drawing.Point(93, 12);
      this.StopBtn.Name = "StopBtn";
      this.StopBtn.Size = new System.Drawing.Size(75, 23);
      this.StopBtn.TabIndex = 1;
      this.StopBtn.Text = "Stop";
      this.StopBtn.UseVisualStyleBackColor = true;
      // 
      // StatusLab
      // 
      this.StatusLab.AutoSize = true;
      this.StatusLab.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.StatusLab.ForeColor = System.Drawing.Color.DarkRed;
      this.StatusLab.Location = new System.Drawing.Point(174, 17);
      this.StatusLab.Name = "StatusLab";
      this.StatusLab.Size = new System.Drawing.Size(73, 13);
      this.StatusLab.TabIndex = 2;
      this.StatusLab.Text = "Not running";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(23, 70);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(64, 13);
      this.label1.TabIndex = 3;
      this.label1.Text = "Fishing Key:";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(35, 97);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(52, 13);
      this.label2.TabIndex = 4;
      this.label2.Text = "Lure Key:";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(37, 124);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(50, 13);
      this.label3.TabIndex = 5;
      this.label3.Text = "Buff Key:";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(31, 151);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(56, 13);
      this.label4.TabIndex = 6;
      this.label4.Text = "Outfit Key:";
      // 
      // FishingKeyCB
      // 
      this.FishingKeyCB.BackColor = System.Drawing.Color.White;
      this.FishingKeyCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.FishingKeyCB.ForeColor = System.Drawing.Color.Black;
      this.FishingKeyCB.FormattingEnabled = true;
      this.FishingKeyCB.Location = new System.Drawing.Point(93, 67);
      this.FishingKeyCB.Name = "FishingKeyCB";
      this.FishingKeyCB.Size = new System.Drawing.Size(107, 21);
      this.FishingKeyCB.TabIndex = 7;
      this.FishingKeyCB.SelectedIndexChanged += new System.EventHandler(this.FishingKeyCB_SelectedIndexChanged);
      this.FishingKeyCB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SetBtnKeyDown);
      // 
      // LureKeyCB
      // 
      this.LureKeyCB.BackColor = System.Drawing.Color.White;
      this.LureKeyCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.LureKeyCB.ForeColor = System.Drawing.Color.Black;
      this.LureKeyCB.FormattingEnabled = true;
      this.LureKeyCB.Location = new System.Drawing.Point(93, 94);
      this.LureKeyCB.Name = "LureKeyCB";
      this.LureKeyCB.Size = new System.Drawing.Size(107, 21);
      this.LureKeyCB.TabIndex = 8;
      this.LureKeyCB.SelectedIndexChanged += new System.EventHandler(this.LureKeyCB_SelectedIndexChanged);
      this.LureKeyCB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SetBtnKeyDown);
      // 
      // BuffKeyCB
      // 
      this.BuffKeyCB.BackColor = System.Drawing.Color.White;
      this.BuffKeyCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.BuffKeyCB.ForeColor = System.Drawing.Color.Black;
      this.BuffKeyCB.FormattingEnabled = true;
      this.BuffKeyCB.Location = new System.Drawing.Point(93, 121);
      this.BuffKeyCB.Name = "BuffKeyCB";
      this.BuffKeyCB.Size = new System.Drawing.Size(107, 21);
      this.BuffKeyCB.TabIndex = 9;
      this.BuffKeyCB.SelectedIndexChanged += new System.EventHandler(this.BuffKeyCB_SelectedIndexChanged);
      this.BuffKeyCB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SetBtnKeyDown);
      // 
      // OutfitKeyCB
      // 
      this.OutfitKeyCB.BackColor = System.Drawing.Color.White;
      this.OutfitKeyCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.OutfitKeyCB.ForeColor = System.Drawing.Color.Black;
      this.OutfitKeyCB.FormattingEnabled = true;
      this.OutfitKeyCB.Location = new System.Drawing.Point(93, 148);
      this.OutfitKeyCB.Name = "OutfitKeyCB";
      this.OutfitKeyCB.Size = new System.Drawing.Size(107, 21);
      this.OutfitKeyCB.TabIndex = 10;
      this.OutfitKeyCB.SelectedIndexChanged += new System.EventHandler(this.OutfitKeyCB_SelectedIndexChanged);
      this.OutfitKeyCB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SetBtnKeyDown);
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(249, 70);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(69, 13);
      this.label5.TabIndex = 11;
      this.label5.Text = "Refish Timer:";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(258, 151);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(60, 13);
      this.label6.TabIndex = 12;
      this.label6.Text = "Loot Timer:";
      // 
      // RefishTimeSlider
      // 
      this.RefishTimeSlider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.RefishTimeSlider.Location = new System.Drawing.Point(324, 67);
      this.RefishTimeSlider.Maximum = 60;
      this.RefishTimeSlider.Name = "RefishTimeSlider";
      this.RefishTimeSlider.Size = new System.Drawing.Size(509, 45);
      this.RefishTimeSlider.TabIndex = 13;
      this.RefishTimeSlider.Value = 3;
      this.RefishTimeSlider.Scroll += new System.EventHandler(this.RefishTimeSlider_Scroll);
      // 
      // LootTimeSlider
      // 
      this.LootTimeSlider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.LootTimeSlider.Location = new System.Drawing.Point(324, 148);
      this.LootTimeSlider.Maximum = 60;
      this.LootTimeSlider.Name = "LootTimeSlider";
      this.LootTimeSlider.Size = new System.Drawing.Size(509, 45);
      this.LootTimeSlider.TabIndex = 14;
      this.LootTimeSlider.Value = 1;
      this.LootTimeSlider.Scroll += new System.EventHandler(this.LootTimeSlider_Scroll);
      // 
      // RefishTimeDisplayLab
      // 
      this.RefishTimeDisplayLab.AutoSize = true;
      this.RefishTimeDisplayLab.ForeColor = System.Drawing.Color.Gray;
      this.RefishTimeDisplayLab.Location = new System.Drawing.Point(330, 99);
      this.RefishTimeDisplayLab.Name = "RefishTimeDisplayLab";
      this.RefishTimeDisplayLab.Size = new System.Drawing.Size(95, 13);
      this.RefishTimeDisplayLab.TabIndex = 15;
      this.RefishTimeDisplayLab.Text = "Refish Time: 3 sec";
      // 
      // LootTimeDisplayLab
      // 
      this.LootTimeDisplayLab.AutoSize = true;
      this.LootTimeDisplayLab.ForeColor = System.Drawing.Color.Gray;
      this.LootTimeDisplayLab.Location = new System.Drawing.Point(330, 180);
      this.LootTimeDisplayLab.Name = "LootTimeDisplayLab";
      this.LootTimeDisplayLab.Size = new System.Drawing.Size(86, 13);
      this.LootTimeDisplayLab.TabIndex = 16;
      this.LootTimeDisplayLab.Text = "Loot Time: 1 sec";
      // 
      // FishingKeySetBtn
      // 
      this.FishingKeySetBtn.BackColor = System.Drawing.Color.Gainsboro;
      this.FishingKeySetBtn.Location = new System.Drawing.Point(206, 66);
      this.FishingKeySetBtn.Name = "FishingKeySetBtn";
      this.FishingKeySetBtn.Size = new System.Drawing.Size(23, 23);
      this.FishingKeySetBtn.TabIndex = 17;
      this.FishingKeySetBtn.Text = "...";
      this.FishingKeySetBtn.UseVisualStyleBackColor = false;
      this.FishingKeySetBtn.Click += new System.EventHandler(this.SetKeyClicked);
      this.FishingKeySetBtn.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SetBtnKeyDown);
      // 
      // LureKeySetBtn
      // 
      this.LureKeySetBtn.BackColor = System.Drawing.Color.Gainsboro;
      this.LureKeySetBtn.Location = new System.Drawing.Point(206, 93);
      this.LureKeySetBtn.Name = "LureKeySetBtn";
      this.LureKeySetBtn.Size = new System.Drawing.Size(23, 23);
      this.LureKeySetBtn.TabIndex = 18;
      this.LureKeySetBtn.Text = "...";
      this.LureKeySetBtn.UseVisualStyleBackColor = false;
      this.LureKeySetBtn.Click += new System.EventHandler(this.SetKeyClicked);
      this.LureKeySetBtn.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SetBtnKeyDown);
      // 
      // BuffKeySetBtn
      // 
      this.BuffKeySetBtn.BackColor = System.Drawing.Color.Gainsboro;
      this.BuffKeySetBtn.Location = new System.Drawing.Point(206, 120);
      this.BuffKeySetBtn.Name = "BuffKeySetBtn";
      this.BuffKeySetBtn.Size = new System.Drawing.Size(23, 23);
      this.BuffKeySetBtn.TabIndex = 19;
      this.BuffKeySetBtn.Text = "...";
      this.BuffKeySetBtn.UseVisualStyleBackColor = false;
      this.BuffKeySetBtn.Click += new System.EventHandler(this.SetKeyClicked);
      this.BuffKeySetBtn.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SetBtnKeyDown);
      // 
      // OutfitKeySetBtn
      // 
      this.OutfitKeySetBtn.BackColor = System.Drawing.Color.Gainsboro;
      this.OutfitKeySetBtn.Location = new System.Drawing.Point(206, 147);
      this.OutfitKeySetBtn.Name = "OutfitKeySetBtn";
      this.OutfitKeySetBtn.Size = new System.Drawing.Size(23, 23);
      this.OutfitKeySetBtn.TabIndex = 20;
      this.OutfitKeySetBtn.Text = "...";
      this.OutfitKeySetBtn.UseVisualStyleBackColor = false;
      this.OutfitKeySetBtn.Click += new System.EventHandler(this.SetKeyClicked);
      this.OutfitKeySetBtn.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SetBtnKeyDown);
      // 
      // ResetFormBtn
      // 
      this.ResetFormBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.ResetFormBtn.ForeColor = System.Drawing.Color.DarkRed;
      this.ResetFormBtn.Location = new System.Drawing.Point(717, 12);
      this.ResetFormBtn.Name = "ResetFormBtn";
      this.ResetFormBtn.Size = new System.Drawing.Size(116, 23);
      this.ResetFormBtn.TabIndex = 21;
      this.ResetFormBtn.Text = "Back to Default";
      this.ResetFormBtn.UseVisualStyleBackColor = true;
      this.ResetFormBtn.Click += new System.EventHandler(this.ResetFormBtn_Click);
      // 
      // SelectColorPic
      // 
      this.SelectColorPic.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.SelectColorPic.BackColor = System.Drawing.Color.Black;
      this.SelectColorPic.Location = new System.Drawing.Point(0, 292);
      this.SelectColorPic.Name = "SelectColorPic";
      this.SelectColorPic.Size = new System.Drawing.Size(845, 301);
      this.SelectColorPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.SelectColorPic.TabIndex = 22;
      this.SelectColorPic.TabStop = false;
      this.SelectColorPic.MouseClick += new System.Windows.Forms.MouseEventHandler(this.SelectColorPic_MouseClick);
      // 
      // ColorDisplayLab
      // 
      this.ColorDisplayLab.BackColor = System.Drawing.Color.White;
      this.ColorDisplayLab.ForeColor = System.Drawing.Color.Black;
      this.ColorDisplayLab.Location = new System.Drawing.Point(4, 266);
      this.ColorDisplayLab.Name = "ColorDisplayLab";
      this.ColorDisplayLab.Size = new System.Drawing.Size(100, 23);
      this.ColorDisplayLab.TabIndex = 23;
      this.ColorDisplayLab.Text = "Bobber Color";
      this.ColorDisplayLab.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // GeneralTicker
      // 
      this.GeneralTicker.Interval = 250;
      this.GeneralTicker.Tick += new System.EventHandler(this.GeneralTicker_Tick);
      // 
      // ColorTolleranceSlider
      // 
      this.ColorTolleranceSlider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.ColorTolleranceSlider.Location = new System.Drawing.Point(324, 229);
      this.ColorTolleranceSlider.Maximum = 255;
      this.ColorTolleranceSlider.Name = "ColorTolleranceSlider";
      this.ColorTolleranceSlider.Size = new System.Drawing.Size(509, 45);
      this.ColorTolleranceSlider.TabIndex = 24;
      this.ColorTolleranceSlider.Value = 20;
      this.ColorTolleranceSlider.Scroll += new System.EventHandler(this.ColorTolleranceSlider_Scroll);
      // 
      // ColorTollDisplayLab
      // 
      this.ColorTollDisplayLab.AutoSize = true;
      this.ColorTollDisplayLab.ForeColor = System.Drawing.Color.Gray;
      this.ColorTollDisplayLab.Location = new System.Drawing.Point(330, 261);
      this.ColorTollDisplayLab.Name = "ColorTollDisplayLab";
      this.ColorTollDisplayLab.Size = new System.Drawing.Size(98, 13);
      this.ColorTollDisplayLab.TabIndex = 25;
      this.ColorTollDisplayLab.Text = "Color tollerance: 20";
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(235, 229);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(83, 13);
      this.label8.TabIndex = 26;
      this.label8.Text = "Color tollerance:";
      // 
      // EscHintLab
      // 
      this.EscHintLab.AutoSize = true;
      this.EscHintLab.ForeColor = System.Drawing.Color.Gray;
      this.EscHintLab.Location = new System.Drawing.Point(12, 43);
      this.EscHintLab.Name = "EscHintLab";
      this.EscHintLab.Size = new System.Drawing.Size(351, 13);
      this.EscHintLab.TabIndex = 27;
      this.EscHintLab.Text = "Hint: ESC stops the bot at any time — even while the game has the focus.";
      // 
      // CaughtCountLab
      // 
      this.CaughtCountLab.AutoSize = true;
      this.CaughtCountLab.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.CaughtCountLab.Location = new System.Drawing.Point(12, 229);
      this.CaughtCountLab.Name = "CaughtCountLab";
      this.CaughtCountLab.Size = new System.Drawing.Size(53, 13);
      this.CaughtCountLab.TabIndex = 28;
      this.CaughtCountLab.Text = "Loots: 0";
      // 
      // HideUiChk
      // 
      this.HideUiChk.AutoSize = true;
      this.HideUiChk.Checked = true;
      this.HideUiChk.CheckState = System.Windows.Forms.CheckState.Checked;
      this.HideUiChk.Location = new System.Drawing.Point(420, 41);
      this.HideUiChk.Name = "HideUiChk";
      this.HideUiChk.Size = new System.Drawing.Size(125, 17);
      this.HideUiChk.TabIndex = 29;
      this.HideUiChk.Text = "Hide game UI (Alt+Y)";
      this.HideUiChk.UseVisualStyleBackColor = true;
      // 
      // frmMain
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(845, 593);
      this.Controls.Add(this.HideUiChk);
      this.Controls.Add(this.CaughtCountLab);
      this.Controls.Add(this.EscHintLab);
      this.Controls.Add(this.label8);
      this.Controls.Add(this.ColorTollDisplayLab);
      this.Controls.Add(this.ColorTolleranceSlider);
      this.Controls.Add(this.ColorDisplayLab);
      this.Controls.Add(this.SelectColorPic);
      this.Controls.Add(this.ResetFormBtn);
      this.Controls.Add(this.OutfitKeySetBtn);
      this.Controls.Add(this.BuffKeySetBtn);
      this.Controls.Add(this.LureKeySetBtn);
      this.Controls.Add(this.FishingKeySetBtn);
      this.Controls.Add(this.LootTimeDisplayLab);
      this.Controls.Add(this.RefishTimeDisplayLab);
      this.Controls.Add(this.LootTimeSlider);
      this.Controls.Add(this.RefishTimeSlider);
      this.Controls.Add(this.label6);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.OutfitKeyCB);
      this.Controls.Add(this.BuffKeyCB);
      this.Controls.Add(this.LureKeyCB);
      this.Controls.Add(this.FishingKeyCB);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.StatusLab);
      this.Controls.Add(this.StopBtn);
      this.Controls.Add(this.StartBtn);
      this.Name = "frmMain";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Got any Fish?!";
      ((System.ComponentModel.ISupportInitialize)(this.RefishTimeSlider)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.LootTimeSlider)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.SelectColorPic)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ColorTolleranceSlider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button StartBtn;
    private System.Windows.Forms.Button StopBtn;
    private System.Windows.Forms.Label StatusLab;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.ComboBox FishingKeyCB;
    private System.Windows.Forms.ComboBox LureKeyCB;
    private System.Windows.Forms.ComboBox BuffKeyCB;
    private System.Windows.Forms.ComboBox OutfitKeyCB;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.TrackBar RefishTimeSlider;
    private System.Windows.Forms.TrackBar LootTimeSlider;
    private System.Windows.Forms.Label RefishTimeDisplayLab;
    private System.Windows.Forms.Label LootTimeDisplayLab;
    private System.Windows.Forms.Button FishingKeySetBtn;
    private System.Windows.Forms.Button LureKeySetBtn;
    private System.Windows.Forms.Button BuffKeySetBtn;
    private System.Windows.Forms.Button OutfitKeySetBtn;
    private System.Windows.Forms.Button ResetFormBtn;
    private System.Windows.Forms.PictureBox SelectColorPic;
    private System.Windows.Forms.Label ColorDisplayLab;
    private System.Windows.Forms.Timer GeneralTicker;
    private System.Windows.Forms.TrackBar ColorTolleranceSlider;
    private System.Windows.Forms.Label ColorTollDisplayLab;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.Label EscHintLab;
    private System.Windows.Forms.Label CaughtCountLab;
    private System.Windows.Forms.CheckBox HideUiChk;
  }
}

