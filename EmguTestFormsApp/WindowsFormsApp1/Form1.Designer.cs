namespace WindowsFormsApp1
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button_turntogray = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.roi = new System.Windows.Forms.Button();
            this.throd = new System.Windows.Forms.Button();
            this.canndy = new System.Windows.Forms.Button();
            this.contours = new System.Windows.Forms.Button();
            this.txt_h = new System.Windows.Forms.TextBox();
            this.txt_w = new System.Windows.Forms.TextBox();
            this.label34 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.txt_y = new System.Windows.Forms.TextBox();
            this.txt_x = new System.Windows.Forms.TextBox();
            this.label36 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.save = new System.Windows.Forms.Button();
            this.smooth = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.erode_button = new System.Windows.Forms.Button();
            this.open_button = new System.Windows.Forms.Button();
            this.button_dft = new System.Windows.Forms.Button();
            this.imageBox2 = new Emgu.CV.UI.ImageBox();
            this.imageBox1 = new Emgu.CV.UI.ImageBox();
            this.button_CornerHarris = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // button_turntogray
            // 
            this.button_turntogray.Location = new System.Drawing.Point(136, 587);
            this.button_turntogray.Name = "button_turntogray";
            this.button_turntogray.Size = new System.Drawing.Size(100, 23);
            this.button_turntogray.TabIndex = 3;
            this.button_turntogray.Text = "turntogray";
            this.button_turntogray.UseVisualStyleBackColor = true;
            this.button_turntogray.Click += new System.EventHandler(this.button_turntogray_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(55, 587);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "open image";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // roi
            // 
            this.roi.Location = new System.Drawing.Point(242, 587);
            this.roi.Name = "roi";
            this.roi.Size = new System.Drawing.Size(45, 23);
            this.roi.TabIndex = 5;
            this.roi.Text = "roi";
            this.roi.UseVisualStyleBackColor = true;
            this.roi.Click += new System.EventHandler(this.roi_Click);
            // 
            // throd
            // 
            this.throd.Location = new System.Drawing.Point(354, 587);
            this.throd.Name = "throd";
            this.throd.Size = new System.Drawing.Size(75, 23);
            this.throd.TabIndex = 6;
            this.throd.Text = "throd";
            this.throd.UseVisualStyleBackColor = true;
            this.throd.Click += new System.EventHandler(this.throd_Click);
            // 
            // canndy
            // 
            this.canndy.Location = new System.Drawing.Point(435, 587);
            this.canndy.Name = "canndy";
            this.canndy.Size = new System.Drawing.Size(75, 23);
            this.canndy.TabIndex = 7;
            this.canndy.Text = "canndy";
            this.canndy.UseVisualStyleBackColor = true;
            this.canndy.Click += new System.EventHandler(this.canndy_Click);
            // 
            // contours
            // 
            this.contours.Location = new System.Drawing.Point(516, 587);
            this.contours.Name = "contours";
            this.contours.Size = new System.Drawing.Size(75, 23);
            this.contours.TabIndex = 8;
            this.contours.Text = "contours";
            this.contours.UseVisualStyleBackColor = true;
            this.contours.Click += new System.EventHandler(this.contours_Click);
            // 
            // txt_h
            // 
            this.txt_h.Font = new System.Drawing.Font("宋体", 12F);
            this.txt_h.Location = new System.Drawing.Point(1266, 121);
            this.txt_h.Name = "txt_h";
            this.txt_h.Size = new System.Drawing.Size(104, 26);
            this.txt_h.TabIndex = 181;
            this.txt_h.Text = "3027";
            // 
            // txt_w
            // 
            this.txt_w.Font = new System.Drawing.Font("宋体", 12F);
            this.txt_w.Location = new System.Drawing.Point(1266, 51);
            this.txt_w.Name = "txt_w";
            this.txt_w.Size = new System.Drawing.Size(104, 26);
            this.txt_w.TabIndex = 180;
            this.txt_w.Text = "2048";
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Font = new System.Drawing.Font("宋体", 12F);
            this.label34.Location = new System.Drawing.Point(1224, 125);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(24, 16);
            this.label34.TabIndex = 179;
            this.label34.Text = "H:";
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Font = new System.Drawing.Font("宋体", 12F);
            this.label35.Location = new System.Drawing.Point(1233, 52);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(24, 16);
            this.label35.TabIndex = 178;
            this.label35.Text = "W:";
            // 
            // txt_y
            // 
            this.txt_y.Font = new System.Drawing.Font("宋体", 12F);
            this.txt_y.Location = new System.Drawing.Point(1266, 83);
            this.txt_y.Name = "txt_y";
            this.txt_y.Size = new System.Drawing.Size(104, 26);
            this.txt_y.TabIndex = 177;
            this.txt_y.Text = "0";
            // 
            // txt_x
            // 
            this.txt_x.Font = new System.Drawing.Font("宋体", 12F);
            this.txt_x.Location = new System.Drawing.Point(1266, 13);
            this.txt_x.Name = "txt_x";
            this.txt_x.Size = new System.Drawing.Size(104, 26);
            this.txt_x.TabIndex = 176;
            this.txt_x.Text = "0";
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Font = new System.Drawing.Font("宋体", 12F);
            this.label36.Location = new System.Drawing.Point(1224, 87);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(24, 16);
            this.label36.TabIndex = 175;
            this.label36.Text = "y:";
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Font = new System.Drawing.Font("宋体", 12F);
            this.label37.Location = new System.Drawing.Point(1233, 14);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(24, 16);
            this.label37.TabIndex = 174;
            this.label37.Text = "x:";
            // 
            // save
            // 
            this.save.Location = new System.Drawing.Point(683, 874);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(100, 23);
            this.save.TabIndex = 182;
            this.save.Text = "save";
            this.save.UseVisualStyleBackColor = true;
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // smooth
            // 
            this.smooth.Location = new System.Drawing.Point(293, 587);
            this.smooth.Name = "smooth";
            this.smooth.Size = new System.Drawing.Size(55, 23);
            this.smooth.TabIndex = 183;
            this.smooth.Text = "smooth";
            this.smooth.UseVisualStyleBackColor = true;
            this.smooth.Click += new System.EventHandler(this.smooth_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(683, 587);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(100, 23);
            this.button3.TabIndex = 184;
            this.button3.Text = "change_throd";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(604, 588);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(73, 22);
            this.button4.TabIndex = 187;
            this.button4.Text = "writedata";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // erode_button
            // 
            this.erode_button.Location = new System.Drawing.Point(789, 587);
            this.erode_button.Name = "erode_button";
            this.erode_button.Size = new System.Drawing.Size(75, 23);
            this.erode_button.TabIndex = 188;
            this.erode_button.Text = "erode";
            this.erode_button.UseVisualStyleBackColor = true;
            this.erode_button.Click += new System.EventHandler(this.canny_button_Click);
            // 
            // open_button
            // 
            this.open_button.Location = new System.Drawing.Point(870, 587);
            this.open_button.Name = "open_button";
            this.open_button.Size = new System.Drawing.Size(75, 23);
            this.open_button.TabIndex = 189;
            this.open_button.Text = "open";
            this.open_button.UseVisualStyleBackColor = true;
            this.open_button.Click += new System.EventHandler(this.open_button_Click);
            // 
            // button_dft
            // 
            this.button_dft.Location = new System.Drawing.Point(951, 588);
            this.button_dft.Name = "button_dft";
            this.button_dft.Size = new System.Drawing.Size(75, 23);
            this.button_dft.TabIndex = 190;
            this.button_dft.Text = "dft";
            this.button_dft.UseVisualStyleBackColor = true;
            this.button_dft.Click += new System.EventHandler(this.button_dft_Click);
            // 
            // imageBox2
            // 
            this.imageBox2.Location = new System.Drawing.Point(604, 10);
            this.imageBox2.Name = "imageBox2";
            this.imageBox2.Size = new System.Drawing.Size(617, 553);
            this.imageBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imageBox2.TabIndex = 185;
            this.imageBox2.TabStop = false;
            // 
            // imageBox1
            // 
            this.imageBox1.Location = new System.Drawing.Point(12, 12);
            this.imageBox1.Name = "imageBox1";
            this.imageBox1.Size = new System.Drawing.Size(586, 551);
            this.imageBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imageBox1.TabIndex = 2;
            this.imageBox1.TabStop = false;
            // 
            // button_CornerHarris
            // 
            this.button_CornerHarris.Location = new System.Drawing.Point(1032, 587);
            this.button_CornerHarris.Name = "button_CornerHarris";
            this.button_CornerHarris.Size = new System.Drawing.Size(75, 23);
            this.button_CornerHarris.TabIndex = 191;
            this.button_CornerHarris.Text = "CornerHarris";
            this.button_CornerHarris.UseVisualStyleBackColor = true;
            this.button_CornerHarris.Click += new System.EventHandler(this.button_CornerHarris_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1388, 645);
            this.Controls.Add(this.button_CornerHarris);
            this.Controls.Add(this.button_dft);
            this.Controls.Add(this.open_button);
            this.Controls.Add(this.erode_button);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.imageBox2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.smooth);
            this.Controls.Add(this.save);
            this.Controls.Add(this.txt_h);
            this.Controls.Add(this.txt_w);
            this.Controls.Add(this.label34);
            this.Controls.Add(this.label35);
            this.Controls.Add(this.txt_y);
            this.Controls.Add(this.txt_x);
            this.Controls.Add(this.label36);
            this.Controls.Add(this.label37);
            this.Controls.Add(this.contours);
            this.Controls.Add(this.canndy);
            this.Controls.Add(this.throd);
            this.Controls.Add(this.roi);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button_turntogray);
            this.Controls.Add(this.imageBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imageBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Emgu.CV.UI.ImageBox imageBox1;
        private System.Windows.Forms.Button button_turntogray;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button roi;
        private System.Windows.Forms.Button throd;
        private System.Windows.Forms.Button canndy;
        private System.Windows.Forms.Button contours;
        public System.Windows.Forms.TextBox txt_h;
        public System.Windows.Forms.TextBox txt_w;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Label label35;
        public System.Windows.Forms.TextBox txt_y;
        public System.Windows.Forms.TextBox txt_x;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.Button save;
        private System.Windows.Forms.Button smooth;
        private System.Windows.Forms.Button button3;
        private Emgu.CV.UI.ImageBox imageBox2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button erode_button;
        private System.Windows.Forms.Button open_button;
        private System.Windows.Forms.Button button_dft;
        private System.Windows.Forms.Button button_CornerHarris;
    }
}

