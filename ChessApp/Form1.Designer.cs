namespace ChessApp
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.FEN_TEXT = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button2 = new System.Windows.Forms.Button();
            this.PlayComputer = new System.Windows.Forms.CheckBox();
            this.NextMoveLabel = new System.Windows.Forms.Label();
            this.CastlingOptionsTitle = new System.Windows.Forms.Label();
            this.WhiteCastlingOptions = new System.Windows.Forms.Label();
            this.BlackCastlingOptions = new System.Windows.Forms.Label();
            this.W_KingsideCastle = new System.Windows.Forms.CheckBox();
            this.W_QueensideCastle = new System.Windows.Forms.CheckBox();
            this.B_KingsideCastle = new System.Windows.Forms.CheckBox();
            this.B_QueensideCastle = new System.Windows.Forms.CheckBox();
            this.KingsideCastleLabel = new System.Windows.Forms.Label();
            this.QueensideCastleLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.VariantSelector = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.button15 = new System.Windows.Forms.Button();
            this.button14 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.button13 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.panel3 = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button1.Location = new System.Drawing.Point(13, 8);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 42);
            this.button1.TabIndex = 1;
            this.button1.Text = "Import game";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FEN_TEXT
            // 
            this.FEN_TEXT.Location = new System.Drawing.Point(126, 20);
            this.FEN_TEXT.Name = "FEN_TEXT";
            this.FEN_TEXT.Size = new System.Drawing.Size(386, 20);
            this.FEN_TEXT.TabIndex = 2;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(609, 22);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(73, 17);
            this.checkBox1.TabIndex = 3;
            this.checkBox1.Text = "Edit mode";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "White",
            "Black"});
            this.comboBox1.Location = new System.Drawing.Point(10, 29);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(73, 21);
            this.comboBox1.TabIndex = 4;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(519, 18);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "Undo";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // PlayComputer
            // 
            this.PlayComputer.AutoSize = true;
            this.PlayComputer.Location = new System.Drawing.Point(695, 21);
            this.PlayComputer.Name = "PlayComputer";
            this.PlayComputer.Size = new System.Drawing.Size(93, 17);
            this.PlayComputer.TabIndex = 4;
            this.PlayComputer.Text = "Play computer";
            this.PlayComputer.UseVisualStyleBackColor = true;
            this.PlayComputer.CheckedChanged += new System.EventHandler(this.PlayComputerCheckChange);
            // 
            // NextMoveLabel
            // 
            this.NextMoveLabel.AutoSize = true;
            this.NextMoveLabel.Location = new System.Drawing.Point(12, 10);
            this.NextMoveLabel.Name = "NextMoveLabel";
            this.NextMoveLabel.Size = new System.Drawing.Size(58, 13);
            this.NextMoveLabel.TabIndex = 5;
            this.NextMoveLabel.Text = "Next move";
            // 
            // CastlingOptionsTitle
            // 
            this.CastlingOptionsTitle.AutoSize = true;
            this.CastlingOptionsTitle.Font = new System.Drawing.Font("Microsoft YaHei", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CastlingOptionsTitle.Location = new System.Drawing.Point(13, 64);
            this.CastlingOptionsTitle.Name = "CastlingOptionsTitle";
            this.CastlingOptionsTitle.Size = new System.Drawing.Size(130, 19);
            this.CastlingOptionsTitle.TabIndex = 6;
            this.CastlingOptionsTitle.Text = "Castling options";
            // 
            // WhiteCastlingOptions
            // 
            this.WhiteCastlingOptions.AutoSize = true;
            this.WhiteCastlingOptions.Location = new System.Drawing.Point(25, 108);
            this.WhiteCastlingOptions.Name = "WhiteCastlingOptions";
            this.WhiteCastlingOptions.Size = new System.Drawing.Size(35, 13);
            this.WhiteCastlingOptions.TabIndex = 7;
            this.WhiteCastlingOptions.Text = "White";
            // 
            // BlackCastlingOptions
            // 
            this.BlackCastlingOptions.AutoSize = true;
            this.BlackCastlingOptions.Location = new System.Drawing.Point(25, 130);
            this.BlackCastlingOptions.Name = "BlackCastlingOptions";
            this.BlackCastlingOptions.Size = new System.Drawing.Size(34, 13);
            this.BlackCastlingOptions.TabIndex = 8;
            this.BlackCastlingOptions.Text = "Black";
            // 
            // W_KingsideCastle
            // 
            this.W_KingsideCastle.AutoSize = true;
            this.W_KingsideCastle.Location = new System.Drawing.Point(69, 108);
            this.W_KingsideCastle.Name = "W_KingsideCastle";
            this.W_KingsideCastle.Size = new System.Drawing.Size(15, 14);
            this.W_KingsideCastle.TabIndex = 9;
            this.W_KingsideCastle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.W_KingsideCastle, "White Kingside");
            this.W_KingsideCastle.UseVisualStyleBackColor = true;
            this.W_KingsideCastle.CheckedChanged += new System.EventHandler(this.CastleOptionChanged);
            // 
            // W_QueensideCastle
            // 
            this.W_QueensideCastle.AutoSize = true;
            this.W_QueensideCastle.Location = new System.Drawing.Point(99, 108);
            this.W_QueensideCastle.Name = "W_QueensideCastle";
            this.W_QueensideCastle.Size = new System.Drawing.Size(15, 14);
            this.W_QueensideCastle.TabIndex = 10;
            this.W_QueensideCastle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.W_QueensideCastle, "White Queenside");
            this.W_QueensideCastle.UseVisualStyleBackColor = true;
            this.W_QueensideCastle.CheckedChanged += new System.EventHandler(this.CastleOptionChanged);
            // 
            // B_KingsideCastle
            // 
            this.B_KingsideCastle.AutoSize = true;
            this.B_KingsideCastle.Location = new System.Drawing.Point(69, 130);
            this.B_KingsideCastle.Name = "B_KingsideCastle";
            this.B_KingsideCastle.Size = new System.Drawing.Size(15, 14);
            this.B_KingsideCastle.TabIndex = 11;
            this.B_KingsideCastle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.B_KingsideCastle, "Black Kingside");
            this.B_KingsideCastle.UseVisualStyleBackColor = true;
            this.B_KingsideCastle.CheckedChanged += new System.EventHandler(this.CastleOptionChanged);
            // 
            // B_QueensideCastle
            // 
            this.B_QueensideCastle.AutoSize = true;
            this.B_QueensideCastle.Location = new System.Drawing.Point(99, 130);
            this.B_QueensideCastle.Name = "B_QueensideCastle";
            this.B_QueensideCastle.Size = new System.Drawing.Size(15, 14);
            this.B_QueensideCastle.TabIndex = 12;
            this.B_QueensideCastle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.B_QueensideCastle, "Black Queenside");
            this.B_QueensideCastle.UseVisualStyleBackColor = true;
            this.B_QueensideCastle.CheckedChanged += new System.EventHandler(this.CastleOptionChanged);
            // 
            // KingsideCastleLabel
            // 
            this.KingsideCastleLabel.AutoSize = true;
            this.KingsideCastleLabel.Location = new System.Drawing.Point(69, 89);
            this.KingsideCastleLabel.Name = "KingsideCastleLabel";
            this.KingsideCastleLabel.Size = new System.Drawing.Size(14, 13);
            this.KingsideCastleLabel.TabIndex = 13;
            this.KingsideCastleLabel.Text = "K";
            this.toolTip1.SetToolTip(this.KingsideCastleLabel, "Kingside Castling");
            // 
            // QueensideCastleLabel
            // 
            this.QueensideCastleLabel.AutoSize = true;
            this.QueensideCastleLabel.Location = new System.Drawing.Point(99, 89);
            this.QueensideCastleLabel.Name = "QueensideCastleLabel";
            this.QueensideCastleLabel.Size = new System.Drawing.Size(15, 13);
            this.QueensideCastleLabel.TabIndex = 14;
            this.QueensideCastleLabel.Text = "Q";
            this.toolTip1.SetToolTip(this.QueensideCastleLabel, "Queenside Castling");
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.panel1.Controls.Add(this.VariantSelector);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.CastlingOptionsTitle);
            this.panel1.Controls.Add(this.QueensideCastleLabel);
            this.panel1.Controls.Add(this.comboBox1);
            this.panel1.Controls.Add(this.KingsideCastleLabel);
            this.panel1.Controls.Add(this.NextMoveLabel);
            this.panel1.Controls.Add(this.B_QueensideCastle);
            this.panel1.Controls.Add(this.WhiteCastlingOptions);
            this.panel1.Controls.Add(this.B_KingsideCastle);
            this.panel1.Controls.Add(this.BlackCastlingOptions);
            this.panel1.Controls.Add(this.W_QueensideCastle);
            this.panel1.Controls.Add(this.W_KingsideCastle);
            this.panel1.Location = new System.Drawing.Point(582, 47);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(155, 207);
            this.panel1.TabIndex = 15;
            this.panel1.Visible = false;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MovingPanel);
            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseUp);
            // 
            // VariantSelector
            // 
            this.VariantSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.VariantSelector.FormattingEnabled = true;
            this.VariantSelector.Items.AddRange(new object[] {
            "Standard",
            "Standard duck",
            "Duck Duck GOOSE",
            "Crazyhouse",
            "Crazy duck"});
            this.VariantSelector.Location = new System.Drawing.Point(15, 173);
            this.VariantSelector.Name = "VariantSelector";
            this.VariantSelector.Size = new System.Drawing.Size(121, 21);
            this.VariantSelector.TabIndex = 16;
            this.VariantSelector.SelectedIndexChanged += new System.EventHandler(this.VariantSelector_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft YaHei", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(46, 150);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 19);
            this.label1.TabIndex = 15;
            this.label1.Text = "Variant";
            // 
            // panel2
            // 
            this.panel2.AutoScroll = true;
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.panel2.Controls.Add(this.button15);
            this.panel2.Controls.Add(this.button14);
            this.panel2.Controls.Add(this.button12);
            this.panel2.Controls.Add(this.button13);
            this.panel2.Controls.Add(this.button10);
            this.panel2.Controls.Add(this.button11);
            this.panel2.Controls.Add(this.button8);
            this.panel2.Controls.Add(this.button9);
            this.panel2.Controls.Add(this.button7);
            this.panel2.Controls.Add(this.button6);
            this.panel2.Controls.Add(this.button5);
            this.panel2.Controls.Add(this.button4);
            this.panel2.Location = new System.Drawing.Point(582, 269);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(269, 242);
            this.panel2.TabIndex = 16;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // button15
            // 
            this.button15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.button15.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button15.Location = new System.Drawing.Point(134, 153);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(106, 23);
            this.button15.TabIndex = 12;
            this.button15.Text = "Resign";
            this.button15.UseVisualStyleBackColor = false;
            // 
            // button14
            // 
            this.button14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.button14.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button14.Location = new System.Drawing.Point(10, 153);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(106, 23);
            this.button14.TabIndex = 11;
            this.button14.Text = "Nxh8";
            this.button14.UseVisualStyleBackColor = false;
            // 
            // button12
            // 
            this.button12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.button12.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button12.Location = new System.Drawing.Point(134, 124);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(106, 23);
            this.button12.TabIndex = 10;
            this.button12.Text = "Qe7";
            this.button12.UseVisualStyleBackColor = false;
            // 
            // button13
            // 
            this.button13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.button13.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button13.Location = new System.Drawing.Point(10, 124);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(106, 23);
            this.button13.TabIndex = 9;
            this.button13.Text = "Nxf7";
            this.button13.UseVisualStyleBackColor = false;
            // 
            // button10
            // 
            this.button10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.button10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button10.Location = new System.Drawing.Point(134, 95);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(106, 23);
            this.button10.TabIndex = 8;
            this.button10.Text = "h6";
            this.button10.UseVisualStyleBackColor = false;
            // 
            // button11
            // 
            this.button11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.button11.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button11.Location = new System.Drawing.Point(10, 95);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(106, 23);
            this.button11.TabIndex = 7;
            this.button11.Text = "Ng5";
            this.button11.UseVisualStyleBackColor = false;
            // 
            // button8
            // 
            this.button8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.button8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button8.Location = new System.Drawing.Point(134, 66);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(106, 23);
            this.button8.TabIndex = 6;
            this.button8.Text = "Nc6";
            this.button8.UseVisualStyleBackColor = false;
            // 
            // button9
            // 
            this.button9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.button9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button9.Location = new System.Drawing.Point(10, 66);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(106, 23);
            this.button9.TabIndex = 5;
            this.button9.Text = "Bc4";
            this.button9.UseVisualStyleBackColor = false;
            // 
            // button7
            // 
            this.button7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.button7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button7.Location = new System.Drawing.Point(134, 37);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(106, 23);
            this.button7.TabIndex = 4;
            this.button7.Text = "Nf6";
            this.button7.UseVisualStyleBackColor = false;
            // 
            // button6
            // 
            this.button6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button6.Location = new System.Drawing.Point(10, 37);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(106, 23);
            this.button6.TabIndex = 3;
            this.button6.Text = "Nf3";
            this.button6.UseVisualStyleBackColor = false;
            // 
            // button5
            // 
            this.button5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.Location = new System.Drawing.Point(134, 8);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(106, 23);
            this.button5.TabIndex = 2;
            this.button5.Text = "e5";
            this.button5.UseVisualStyleBackColor = false;
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Location = new System.Drawing.Point(10, 8);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(106, 23);
            this.button4.TabIndex = 1;
            this.button4.Text = "e4";
            this.button4.UseVisualStyleBackColor = false;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(389, 520);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(97, 23);
            this.button3.TabIndex = 1;
            this.button3.Text = "Import PGN";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.pictureBox4);
            this.panel3.Controls.Add(this.pictureBox3);
            this.panel3.Controls.Add(this.pictureBox2);
            this.panel3.Controls.Add(this.pictureBox1);
            this.panel3.Location = new System.Drawing.Point(582, 517);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(269, 46);
            this.panel3.TabIndex = 17;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(188, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(76, 40);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Control;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(5, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(76, 40);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
            this.pictureBox3.Location = new System.Drawing.Point(139, 3);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(40, 40);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 2;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox4.Image")));
            this.pictureBox4.Location = new System.Drawing.Point(90, 3);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(40, 40);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox4.TabIndex = 3;
            this.pictureBox4.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(892, 575);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.PlayComputer);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.FEN_TEXT);
            this.Controls.Add(this.button1);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "Chess UI";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResizeBegin += new System.EventHandler(this.Form1_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.Form1_ResizeEnd);
            this.Click += new System.EventHandler(this.Form1_Click);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MovingPanel);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseUp);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button1;
        public System.Windows.Forms.TextBox FEN_TEXT;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox PlayComputer;
        private System.Windows.Forms.Label NextMoveLabel;
        private System.Windows.Forms.Label CastlingOptionsTitle;
        private System.Windows.Forms.Label WhiteCastlingOptions;
        private System.Windows.Forms.Label BlackCastlingOptions;
        private System.Windows.Forms.CheckBox W_KingsideCastle;
        private System.Windows.Forms.CheckBox W_QueensideCastle;
        private System.Windows.Forms.CheckBox B_KingsideCastle;
        private System.Windows.Forms.CheckBox B_QueensideCastle;
        private System.Windows.Forms.Label KingsideCastleLabel;
        private System.Windows.Forms.Label QueensideCastleLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ComboBox VariantSelector;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Button button13;
        private System.Windows.Forms.Button button14;
        private System.Windows.Forms.Button button15;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox4;
    }
}

