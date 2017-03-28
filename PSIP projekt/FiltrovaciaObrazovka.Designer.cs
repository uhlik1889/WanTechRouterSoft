namespace PSIP_projekt
{
    partial class FiltrovaciaObrazovka
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.macSRCAdresaTextBox = new System.Windows.Forms.TextBox();
            this.ipSRCAdresaTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dstportTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.AddException = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.macDSTAdresaTextBox = new System.Windows.Forms.TextBox();
            this.ipDSTAdresaTextBox = new System.Windows.Forms.TextBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.radioButton5 = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.In = new System.Windows.Forms.GroupBox();
            this.inRadioButtonFalse = new System.Windows.Forms.RadioButton();
            this.inRadioButtonTrue = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.permitRadioButtonFalse = new System.Windows.Forms.RadioButton();
            this.permitRadioButtonTrue = new System.Windows.Forms.RadioButton();
            this.interfaceTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.srcportTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.radioButton6 = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.In.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(13, 13);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(641, 300);
            this.dataGridView1.TabIndex = 0;
            // 
            // macSRCAdresaTextBox
            // 
            this.macSRCAdresaTextBox.Location = new System.Drawing.Point(13, 363);
            this.macSRCAdresaTextBox.Name = "macSRCAdresaTextBox";
            this.macSRCAdresaTextBox.Size = new System.Drawing.Size(100, 20);
            this.macSRCAdresaTextBox.TabIndex = 1;
            // 
            // ipSRCAdresaTextBox
            // 
            this.ipSRCAdresaTextBox.Location = new System.Drawing.Point(13, 408);
            this.ipSRCAdresaTextBox.Name = "ipSRCAdresaTextBox";
            this.ipSRCAdresaTextBox.Size = new System.Drawing.Size(100, 20);
            this.ipSRCAdresaTextBox.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 341);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "SRC Mac adresa";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 389);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "SRC IP adresa";
            // 
            // dstportTextBox
            // 
            this.dstportTextBox.Location = new System.Drawing.Point(593, 408);
            this.dstportTextBox.Name = "dstportTextBox";
            this.dstportTextBox.Size = new System.Drawing.Size(68, 20);
            this.dstportTextBox.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(590, 389);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "DSTPort";
            // 
            // AddException
            // 
            this.AddException.Location = new System.Drawing.Point(536, 448);
            this.AddException.Name = "AddException";
            this.AddException.Size = new System.Drawing.Size(118, 23);
            this.AddException.TabIndex = 12;
            this.AddException.Text = "Pridaj vynimku";
            this.AddException.UseVisualStyleBackColor = true;
            this.AddException.Click += new System.EventHandler(this.AddException_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(150, 341);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "DST Mac adresa";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(150, 390);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "DST IP adresa";
            // 
            // macDSTAdresaTextBox
            // 
            this.macDSTAdresaTextBox.Location = new System.Drawing.Point(153, 363);
            this.macDSTAdresaTextBox.Name = "macDSTAdresaTextBox";
            this.macDSTAdresaTextBox.Size = new System.Drawing.Size(100, 20);
            this.macDSTAdresaTextBox.TabIndex = 15;
            // 
            // ipDSTAdresaTextBox
            // 
            this.ipDSTAdresaTextBox.Location = new System.Drawing.Point(153, 408);
            this.ipDSTAdresaTextBox.Name = "ipDSTAdresaTextBox";
            this.ipDSTAdresaTextBox.Size = new System.Drawing.Size(100, 20);
            this.ipDSTAdresaTextBox.TabIndex = 16;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(19, 25);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(48, 17);
            this.radioButton1.TabIndex = 17;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "UDP";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(19, 47);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(46, 17);
            this.radioButton2.TabIndex = 18;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "TCP";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(19, 71);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(51, 17);
            this.radioButton3.TabIndex = 19;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "ICMP";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton4
            // 
            this.radioButton4.AutoSize = true;
            this.radioButton4.Location = new System.Drawing.Point(19, 95);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(47, 17);
            this.radioButton4.TabIndex = 20;
            this.radioButton4.TabStop = true;
            this.radioButton4.Text = "ARP";
            this.radioButton4.UseVisualStyleBackColor = true;
            // 
            // radioButton5
            // 
            this.radioButton5.AutoSize = true;
            this.radioButton5.Location = new System.Drawing.Point(19, 119);
            this.radioButton5.Name = "radioButton5";
            this.radioButton5.Size = new System.Drawing.Size(35, 17);
            this.radioButton5.TabIndex = 21;
            this.radioButton5.TabStop = true;
            this.radioButton5.Text = "IP";
            this.radioButton5.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton6);
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.radioButton5);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Controls.Add(this.radioButton4);
            this.groupBox1.Controls.Add(this.radioButton3);
            this.groupBox1.Location = new System.Drawing.Point(259, 319);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(134, 152);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Protocoly";
            // 
            // In
            // 
            this.In.Controls.Add(this.inRadioButtonFalse);
            this.In.Controls.Add(this.inRadioButtonTrue);
            this.In.Location = new System.Drawing.Point(400, 320);
            this.In.Name = "In";
            this.In.Size = new System.Drawing.Size(110, 63);
            this.In.TabIndex = 23;
            this.In.TabStop = false;
            this.In.Text = "In";
            // 
            // inRadioButtonFalse
            // 
            this.inRadioButtonFalse.AutoSize = true;
            this.inRadioButtonFalse.Location = new System.Drawing.Point(36, 38);
            this.inRadioButtonFalse.Name = "inRadioButtonFalse";
            this.inRadioButtonFalse.Size = new System.Drawing.Size(41, 17);
            this.inRadioButtonFalse.TabIndex = 1;
            this.inRadioButtonFalse.TabStop = true;
            this.inRadioButtonFalse.Text = "Nie";
            this.inRadioButtonFalse.UseVisualStyleBackColor = true;
            // 
            // inRadioButtonTrue
            // 
            this.inRadioButtonTrue.AutoSize = true;
            this.inRadioButtonTrue.Location = new System.Drawing.Point(36, 15);
            this.inRadioButtonTrue.Name = "inRadioButtonTrue";
            this.inRadioButtonTrue.Size = new System.Drawing.Size(44, 17);
            this.inRadioButtonTrue.TabIndex = 0;
            this.inRadioButtonTrue.TabStop = true;
            this.inRadioButtonTrue.Text = "Ano";
            this.inRadioButtonTrue.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.permitRadioButtonFalse);
            this.groupBox2.Controls.Add(this.permitRadioButtonTrue);
            this.groupBox2.Location = new System.Drawing.Point(400, 400);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(110, 71);
            this.groupBox2.TabIndex = 24;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Permit";
            // 
            // permitRadioButtonFalse
            // 
            this.permitRadioButtonFalse.AutoSize = true;
            this.permitRadioButtonFalse.Location = new System.Drawing.Point(36, 44);
            this.permitRadioButtonFalse.Name = "permitRadioButtonFalse";
            this.permitRadioButtonFalse.Size = new System.Drawing.Size(41, 17);
            this.permitRadioButtonFalse.TabIndex = 1;
            this.permitRadioButtonFalse.TabStop = true;
            this.permitRadioButtonFalse.Text = "Nie";
            this.permitRadioButtonFalse.UseVisualStyleBackColor = true;
            // 
            // permitRadioButtonTrue
            // 
            this.permitRadioButtonTrue.AutoSize = true;
            this.permitRadioButtonTrue.Location = new System.Drawing.Point(36, 20);
            this.permitRadioButtonTrue.Name = "permitRadioButtonTrue";
            this.permitRadioButtonTrue.Size = new System.Drawing.Size(44, 17);
            this.permitRadioButtonTrue.TabIndex = 0;
            this.permitRadioButtonTrue.TabStop = true;
            this.permitRadioButtonTrue.Text = "Ano";
            this.permitRadioButtonTrue.UseVisualStyleBackColor = true;
            // 
            // interfaceTextBox
            // 
            this.interfaceTextBox.Location = new System.Drawing.Point(538, 357);
            this.interfaceTextBox.Name = "interfaceTextBox";
            this.interfaceTextBox.Size = new System.Drawing.Size(100, 20);
            this.interfaceTextBox.TabIndex = 25;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(538, 335);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(49, 13);
            this.label6.TabIndex = 26;
            this.label6.Text = "Interface";
            // 
            // srcportTextBox
            // 
            this.srcportTextBox.Location = new System.Drawing.Point(519, 408);
            this.srcportTextBox.Name = "srcportTextBox";
            this.srcportTextBox.Size = new System.Drawing.Size(68, 20);
            this.srcportTextBox.TabIndex = 27;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(516, 389);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(48, 13);
            this.label7.TabIndex = 28;
            this.label7.Text = "SRCPort";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(16, 447);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(237, 23);
            this.button1.TabIndex = 29;
            this.button1.Text = "Vymazať filtre";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // radioButton6
            // 
            this.radioButton6.AutoSize = true;
            this.radioButton6.Location = new System.Drawing.Point(77, 70);
            this.radioButton6.Name = "radioButton6";
            this.radioButton6.Size = new System.Drawing.Size(49, 17);
            this.radioButton6.TabIndex = 22;
            this.radioButton6.TabStop = true;
            this.radioButton6.Text = "none";
            this.radioButton6.UseVisualStyleBackColor = true;
            // 
            // FiltrovaciaObrazovka
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(666, 483);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.srcportTextBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.interfaceTextBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.In);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ipDSTAdresaTextBox);
            this.Controls.Add(this.macDSTAdresaTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.AddException);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dstportTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ipSRCAdresaTextBox);
            this.Controls.Add(this.macSRCAdresaTextBox);
            this.Controls.Add(this.dataGridView1);
            this.Name = "FiltrovaciaObrazovka";
            this.Text = "IngoingObrazovka";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FiltrovaciaObrazovka_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.In.ResumeLayout(false);
            this.In.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox macSRCAdresaTextBox;
        private System.Windows.Forms.TextBox ipSRCAdresaTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox dstportTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button AddException;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox macDSTAdresaTextBox;
        private System.Windows.Forms.TextBox ipDSTAdresaTextBox;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.RadioButton radioButton5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox In;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton inRadioButtonFalse;
        private System.Windows.Forms.RadioButton inRadioButtonTrue;
        private System.Windows.Forms.RadioButton permitRadioButtonFalse;
        private System.Windows.Forms.RadioButton permitRadioButtonTrue;
        private System.Windows.Forms.TextBox interfaceTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox srcportTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RadioButton radioButton6;
    }
}