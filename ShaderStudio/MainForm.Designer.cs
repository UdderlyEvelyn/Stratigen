namespace ShaderStudio
{
    partial class MainForm
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
            this.tbRotX = new System.Windows.Forms.TrackBar();
            this.tbRotY = new System.Windows.Forms.TrackBar();
            this.tbRotZ = new System.Windows.Forms.TrackBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label16 = new System.Windows.Forms.Label();
            this.chkAuto = new System.Windows.Forms.CheckBox();
            this.lbZv = new System.Windows.Forms.Label();
            this.lbYv = new System.Windows.Forms.Label();
            this.lbXv = new System.Windows.Forms.Label();
            this.lbZ = new System.Windows.Forms.Label();
            this.lbY = new System.Windows.Forms.Label();
            this.lbX = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.lbZl = new System.Windows.Forms.Label();
            this.lbYl = new System.Windows.Forms.Label();
            this.lbXl = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tbXl = new System.Windows.Forms.TrackBar();
            this.tbZl = new System.Windows.Forms.TrackBar();
            this.tbYl = new System.Windows.Forms.TrackBar();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.lbZc = new System.Windows.Forms.Label();
            this.lbYc = new System.Windows.Forms.Label();
            this.lbXc = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.tbXc = new System.Windows.Forms.TrackBar();
            this.tbZc = new System.Windows.Forms.TrackBar();
            this.tbYc = new System.Windows.Forms.TrackBar();
            this.sceneControl = new ShaderStudio.SceneControl();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.lbZt = new System.Windows.Forms.Label();
            this.lbYt = new System.Windows.Forms.Label();
            this.lbXt = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.tbXt = new System.Windows.Forms.TrackBar();
            this.tbZt = new System.Windows.Forms.TrackBar();
            this.tbYt = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.tbRotX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRotY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRotZ)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbXl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbZl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbYl)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbXc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbZc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbYc)).BeginInit();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbXt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbZt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbYt)).BeginInit();
            this.SuspendLayout();
            // 
            // tbRotX
            // 
            this.tbRotX.LargeChange = 10;
            this.tbRotX.Location = new System.Drawing.Point(24, 26);
            this.tbRotX.Maximum = 360;
            this.tbRotX.Name = "tbRotX";
            this.tbRotX.Size = new System.Drawing.Size(104, 45);
            this.tbRotX.TabIndex = 1;
            this.tbRotX.TickFrequency = 90;
            this.tbRotX.Scroll += new System.EventHandler(this.tbRotX_Scroll);
            // 
            // tbRotY
            // 
            this.tbRotY.LargeChange = 10;
            this.tbRotY.Location = new System.Drawing.Point(24, 77);
            this.tbRotY.Maximum = 360;
            this.tbRotY.Name = "tbRotY";
            this.tbRotY.Size = new System.Drawing.Size(104, 45);
            this.tbRotY.TabIndex = 2;
            this.tbRotY.TickFrequency = 90;
            this.tbRotY.Scroll += new System.EventHandler(this.tbRotY_Scroll);
            // 
            // tbRotZ
            // 
            this.tbRotZ.LargeChange = 10;
            this.tbRotZ.Location = new System.Drawing.Point(24, 128);
            this.tbRotZ.Maximum = 360;
            this.tbRotZ.Name = "tbRotZ";
            this.tbRotZ.Size = new System.Drawing.Size(104, 45);
            this.tbRotZ.TabIndex = 3;
            this.tbRotZ.TickFrequency = 90;
            this.tbRotZ.Scroll += new System.EventHandler(this.tbRotZ_Scroll);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label16);
            this.panel1.Controls.Add(this.chkAuto);
            this.panel1.Controls.Add(this.lbZv);
            this.panel1.Controls.Add(this.lbYv);
            this.panel1.Controls.Add(this.lbXv);
            this.panel1.Controls.Add(this.lbZ);
            this.panel1.Controls.Add(this.lbY);
            this.panel1.Controls.Add(this.lbX);
            this.panel1.Controls.Add(this.tbRotX);
            this.panel1.Controls.Add(this.tbRotZ);
            this.panel1.Controls.Add(this.tbRotY);
            this.panel1.Location = new System.Drawing.Point(659, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(133, 203);
            this.panel1.TabIndex = 4;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(4, 4);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(88, 13);
            this.label16.TabIndex = 11;
            this.label16.Text = "Triangle Rotation";
            // 
            // chkAuto
            // 
            this.chkAuto.AutoSize = true;
            this.chkAuto.Checked = true;
            this.chkAuto.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAuto.Location = new System.Drawing.Point(7, 182);
            this.chkAuto.Name = "chkAuto";
            this.chkAuto.Size = new System.Drawing.Size(123, 17);
            this.chkAuto.TabIndex = 10;
            this.chkAuto.Text = "Automatically Rotate";
            this.chkAuto.UseVisualStyleBackColor = true;
            // 
            // lbZv
            // 
            this.lbZv.AutoSize = true;
            this.lbZv.Location = new System.Drawing.Point(24, 160);
            this.lbZv.Name = "lbZv";
            this.lbZv.Size = new System.Drawing.Size(13, 13);
            this.lbZv.TabIndex = 9;
            this.lbZv.Text = "0";
            // 
            // lbYv
            // 
            this.lbYv.AutoSize = true;
            this.lbYv.Location = new System.Drawing.Point(24, 109);
            this.lbYv.Name = "lbYv";
            this.lbYv.Size = new System.Drawing.Size(13, 13);
            this.lbYv.TabIndex = 8;
            this.lbYv.Text = "0";
            // 
            // lbXv
            // 
            this.lbXv.AutoSize = true;
            this.lbXv.Location = new System.Drawing.Point(24, 57);
            this.lbXv.Name = "lbXv";
            this.lbXv.Size = new System.Drawing.Size(13, 13);
            this.lbXv.TabIndex = 7;
            this.lbXv.Text = "0";
            // 
            // lbZ
            // 
            this.lbZ.AutoSize = true;
            this.lbZ.Location = new System.Drawing.Point(4, 128);
            this.lbZ.Name = "lbZ";
            this.lbZ.Size = new System.Drawing.Size(14, 13);
            this.lbZ.TabIndex = 6;
            this.lbZ.Text = "Z";
            // 
            // lbY
            // 
            this.lbY.AutoSize = true;
            this.lbY.Location = new System.Drawing.Point(4, 77);
            this.lbY.Name = "lbY";
            this.lbY.Size = new System.Drawing.Size(14, 13);
            this.lbY.TabIndex = 5;
            this.lbY.Text = "Y";
            // 
            // lbX
            // 
            this.lbX.AutoSize = true;
            this.lbX.Location = new System.Drawing.Point(4, 26);
            this.lbX.Name = "lbX";
            this.lbX.Size = new System.Drawing.Size(14, 13);
            this.lbX.TabIndex = 4;
            this.lbX.Text = "X";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.lbZl);
            this.panel2.Controls.Add(this.lbYl);
            this.panel2.Controls.Add(this.lbXl);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.tbXl);
            this.panel2.Controls.Add(this.tbZl);
            this.panel2.Controls.Add(this.tbYl);
            this.panel2.Location = new System.Drawing.Point(659, 209);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(133, 179);
            this.panel2.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Light Position";
            // 
            // lbZl
            // 
            this.lbZl.AutoSize = true;
            this.lbZl.Location = new System.Drawing.Point(22, 160);
            this.lbZl.Name = "lbZl";
            this.lbZl.Size = new System.Drawing.Size(13, 13);
            this.lbZl.TabIndex = 9;
            this.lbZl.Text = "0";
            // 
            // lbYl
            // 
            this.lbYl.AutoSize = true;
            this.lbYl.Location = new System.Drawing.Point(22, 109);
            this.lbYl.Name = "lbYl";
            this.lbYl.Size = new System.Drawing.Size(13, 13);
            this.lbYl.TabIndex = 8;
            this.lbYl.Text = "0";
            // 
            // lbXl
            // 
            this.lbXl.AutoSize = true;
            this.lbXl.Location = new System.Drawing.Point(22, 57);
            this.lbXl.Name = "lbXl";
            this.lbXl.Size = new System.Drawing.Size(13, 13);
            this.lbXl.TabIndex = 7;
            this.lbXl.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(2, 128);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Z";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(2, 77);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(14, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Y";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(2, 26);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(14, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "X";
            // 
            // tbXl
            // 
            this.tbXl.LargeChange = 10;
            this.tbXl.Location = new System.Drawing.Point(22, 26);
            this.tbXl.Maximum = 50;
            this.tbXl.Minimum = -50;
            this.tbXl.Name = "tbXl";
            this.tbXl.Size = new System.Drawing.Size(104, 45);
            this.tbXl.TabIndex = 1;
            this.tbXl.TickFrequency = 10;
            this.tbXl.Scroll += new System.EventHandler(this.tbXl_Scroll);
            // 
            // tbZl
            // 
            this.tbZl.LargeChange = 10;
            this.tbZl.Location = new System.Drawing.Point(22, 128);
            this.tbZl.Maximum = 0;
            this.tbZl.Minimum = -100;
            this.tbZl.Name = "tbZl";
            this.tbZl.Size = new System.Drawing.Size(104, 45);
            this.tbZl.TabIndex = 3;
            this.tbZl.TickFrequency = 10;
            this.tbZl.Scroll += new System.EventHandler(this.tbZl_Scroll);
            // 
            // tbYl
            // 
            this.tbYl.LargeChange = 10;
            this.tbYl.Location = new System.Drawing.Point(22, 77);
            this.tbYl.Maximum = 50;
            this.tbYl.Minimum = -50;
            this.tbYl.Name = "tbYl";
            this.tbYl.Size = new System.Drawing.Size(104, 45);
            this.tbYl.TabIndex = 2;
            this.tbYl.TickFrequency = 10;
            this.tbYl.Scroll += new System.EventHandler(this.tbYl_Scroll);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.lbZc);
            this.panel3.Controls.Add(this.lbYc);
            this.panel3.Controls.Add(this.lbXc);
            this.panel3.Controls.Add(this.label9);
            this.panel3.Controls.Add(this.label10);
            this.panel3.Controls.Add(this.label11);
            this.panel3.Controls.Add(this.tbXc);
            this.panel3.Controls.Add(this.tbZc);
            this.panel3.Controls.Add(this.tbYc);
            this.panel3.Location = new System.Drawing.Point(659, 394);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(133, 179);
            this.panel3.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Camera Position";
            // 
            // lbZc
            // 
            this.lbZc.AutoSize = true;
            this.lbZc.Location = new System.Drawing.Point(22, 160);
            this.lbZc.Name = "lbZc";
            this.lbZc.Size = new System.Drawing.Size(13, 13);
            this.lbZc.TabIndex = 9;
            this.lbZc.Text = "0";
            // 
            // lbYc
            // 
            this.lbYc.AutoSize = true;
            this.lbYc.Location = new System.Drawing.Point(22, 109);
            this.lbYc.Name = "lbYc";
            this.lbYc.Size = new System.Drawing.Size(13, 13);
            this.lbYc.TabIndex = 8;
            this.lbYc.Text = "0";
            // 
            // lbXc
            // 
            this.lbXc.AutoSize = true;
            this.lbXc.Location = new System.Drawing.Point(22, 57);
            this.lbXc.Name = "lbXc";
            this.lbXc.Size = new System.Drawing.Size(13, 13);
            this.lbXc.TabIndex = 7;
            this.lbXc.Text = "0";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(2, 128);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(14, 13);
            this.label9.TabIndex = 6;
            this.label9.Text = "Z";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(2, 77);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(14, 13);
            this.label10.TabIndex = 5;
            this.label10.Text = "Y";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(2, 26);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(14, 13);
            this.label11.TabIndex = 4;
            this.label11.Text = "X";
            // 
            // tbXc
            // 
            this.tbXc.LargeChange = 10;
            this.tbXc.Location = new System.Drawing.Point(22, 26);
            this.tbXc.Maximum = 50;
            this.tbXc.Minimum = -50;
            this.tbXc.Name = "tbXc";
            this.tbXc.Size = new System.Drawing.Size(104, 45);
            this.tbXc.TabIndex = 1;
            this.tbXc.TickFrequency = 10;
            this.tbXc.Scroll += new System.EventHandler(this.tbXc_Scroll);
            // 
            // tbZc
            // 
            this.tbZc.LargeChange = 10;
            this.tbZc.Location = new System.Drawing.Point(22, 128);
            this.tbZc.Maximum = 50;
            this.tbZc.Minimum = -50;
            this.tbZc.Name = "tbZc";
            this.tbZc.Size = new System.Drawing.Size(104, 45);
            this.tbZc.TabIndex = 3;
            this.tbZc.TickFrequency = 10;
            this.tbZc.Scroll += new System.EventHandler(this.tbZc_Scroll);
            // 
            // tbYc
            // 
            this.tbYc.LargeChange = 10;
            this.tbYc.Location = new System.Drawing.Point(22, 77);
            this.tbYc.Maximum = 50;
            this.tbYc.Minimum = -50;
            this.tbYc.Name = "tbYc";
            this.tbYc.Size = new System.Drawing.Size(104, 45);
            this.tbYc.TabIndex = 2;
            this.tbYc.TickFrequency = 10;
            this.tbYc.Scroll += new System.EventHandler(this.tbYc_Scroll);
            // 
            // sceneControl
            // 
            this.sceneControl.BackColor = System.Drawing.Color.Transparent;
            this.sceneControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sceneControl.ForeColor = System.Drawing.Color.White;
            this.sceneControl.Location = new System.Drawing.Point(0, 0);
            this.sceneControl.Name = "sceneControl";
            this.sceneControl.Size = new System.Drawing.Size(792, 573);
            this.sceneControl.TabIndex = 0;
            this.sceneControl.VSync = false;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.label3);
            this.panel4.Controls.Add(this.lbZt);
            this.panel4.Controls.Add(this.lbYt);
            this.panel4.Controls.Add(this.lbXt);
            this.panel4.Controls.Add(this.label13);
            this.panel4.Controls.Add(this.label14);
            this.panel4.Controls.Add(this.label15);
            this.panel4.Controls.Add(this.tbXt);
            this.panel4.Controls.Add(this.tbZt);
            this.panel4.Controls.Add(this.tbYt);
            this.panel4.Location = new System.Drawing.Point(520, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(133, 179);
            this.panel4.TabIndex = 13;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Triangle Position";
            // 
            // lbZt
            // 
            this.lbZt.AutoSize = true;
            this.lbZt.Location = new System.Drawing.Point(22, 160);
            this.lbZt.Name = "lbZt";
            this.lbZt.Size = new System.Drawing.Size(13, 13);
            this.lbZt.TabIndex = 9;
            this.lbZt.Text = "0";
            // 
            // lbYt
            // 
            this.lbYt.AutoSize = true;
            this.lbYt.Location = new System.Drawing.Point(22, 109);
            this.lbYt.Name = "lbYt";
            this.lbYt.Size = new System.Drawing.Size(13, 13);
            this.lbYt.TabIndex = 8;
            this.lbYt.Text = "0";
            // 
            // lbXt
            // 
            this.lbXt.AutoSize = true;
            this.lbXt.Location = new System.Drawing.Point(22, 57);
            this.lbXt.Name = "lbXt";
            this.lbXt.Size = new System.Drawing.Size(13, 13);
            this.lbXt.TabIndex = 7;
            this.lbXt.Text = "0";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(2, 128);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(14, 13);
            this.label13.TabIndex = 6;
            this.label13.Text = "Z";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(2, 77);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(14, 13);
            this.label14.TabIndex = 5;
            this.label14.Text = "Y";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(2, 26);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(14, 13);
            this.label15.TabIndex = 4;
            this.label15.Text = "X";
            // 
            // tbXt
            // 
            this.tbXt.LargeChange = 10;
            this.tbXt.Location = new System.Drawing.Point(22, 26);
            this.tbXt.Maximum = 50;
            this.tbXt.Minimum = -50;
            this.tbXt.Name = "tbXt";
            this.tbXt.Size = new System.Drawing.Size(104, 45);
            this.tbXt.TabIndex = 1;
            this.tbXt.TickFrequency = 10;
            this.tbXt.Scroll += new System.EventHandler(this.tbXt_Scroll);
            // 
            // tbZt
            // 
            this.tbZt.LargeChange = 10;
            this.tbZt.Location = new System.Drawing.Point(22, 128);
            this.tbZt.Maximum = 50;
            this.tbZt.Minimum = -50;
            this.tbZt.Name = "tbZt";
            this.tbZt.Size = new System.Drawing.Size(104, 45);
            this.tbZt.TabIndex = 3;
            this.tbZt.TickFrequency = 10;
            this.tbZt.Scroll += new System.EventHandler(this.tbZt_Scroll);
            // 
            // tbYt
            // 
            this.tbYt.LargeChange = 10;
            this.tbYt.Location = new System.Drawing.Point(22, 77);
            this.tbYt.Maximum = 50;
            this.tbYt.Minimum = -50;
            this.tbYt.Name = "tbYt";
            this.tbYt.Size = new System.Drawing.Size(104, 45);
            this.tbYt.TabIndex = 2;
            this.tbYt.TickFrequency = 10;
            this.tbYt.Scroll += new System.EventHandler(this.tbYt_Scroll);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 573);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.sceneControl);
            this.Name = "MainForm";
            this.Text = "WinForms Graphics Device";
            ((System.ComponentModel.ISupportInitialize)(this.tbRotX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRotY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRotZ)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbXl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbZl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbYl)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbXc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbZc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbYc)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbXt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbZt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbYt)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private SceneControl sceneControl;
        private System.Windows.Forms.TrackBar tbRotX;
        private System.Windows.Forms.TrackBar tbRotY;
        private System.Windows.Forms.TrackBar tbRotZ;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbZ;
        private System.Windows.Forms.Label lbY;
        private System.Windows.Forms.Label lbX;
        private System.Windows.Forms.Label lbZv;
        private System.Windows.Forms.Label lbYv;
        private System.Windows.Forms.Label lbXv;
        private System.Windows.Forms.CheckBox chkAuto;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lbZl;
        private System.Windows.Forms.Label lbYl;
        private System.Windows.Forms.Label lbXl;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TrackBar tbXl;
        private System.Windows.Forms.TrackBar tbZl;
        private System.Windows.Forms.TrackBar tbYl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbZc;
        private System.Windows.Forms.Label lbYc;
        private System.Windows.Forms.Label lbXc;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TrackBar tbXc;
        private System.Windows.Forms.TrackBar tbZc;
        private System.Windows.Forms.TrackBar tbYc;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbZt;
        private System.Windows.Forms.Label lbYt;
        private System.Windows.Forms.Label lbXt;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TrackBar tbXt;
        private System.Windows.Forms.TrackBar tbZt;
        private System.Windows.Forms.TrackBar tbYt;
    }
}

