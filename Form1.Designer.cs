namespace NetDeepLearning
{
    partial class Form1
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.Start_IA = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.hSmartWindowControl1 = new HalconDotNet.HSmartWindowControl();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.Acq_Image = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Start_IA
            // 
            this.Start_IA.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Start_IA.Font = new System.Drawing.Font("Microsoft Sans Serif", 28.25F);
            this.Start_IA.Location = new System.Drawing.Point(825, 738);
            this.Start_IA.Name = "Start_IA";
            this.Start_IA.Size = new System.Drawing.Size(384, 52);
            this.Start_IA.TabIndex = 2;
            this.Start_IA.Text = "Piece 1";
            this.Start_IA.UseVisualStyleBackColor = true;
            this.Start_IA.Click += new System.EventHandler(this.Start_IA_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(517, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(252, 29);
            this.label1.TabIndex = 5;
            this.label1.Text = "Image postprocessed.";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // hSmartWindowControl1
            // 
            this.hSmartWindowControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hSmartWindowControl1.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.hSmartWindowControl1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.hSmartWindowControl1.HDoubleClickToFitContent = true;
            this.hSmartWindowControl1.HDrawingObjectsModifier = HalconDotNet.HSmartWindowControl.DrawingObjectsModifier.None;
            this.hSmartWindowControl1.HImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hSmartWindowControl1.HKeepAspectRatio = true;
            this.hSmartWindowControl1.HMoveContent = true;
            this.hSmartWindowControl1.HZoomContent = HalconDotNet.HSmartWindowControl.ZoomContent.WheelForwardZoomsIn;
            this.hSmartWindowControl1.Location = new System.Drawing.Point(9, -41);
            this.hSmartWindowControl1.Margin = new System.Windows.Forms.Padding(0);
            this.hSmartWindowControl1.Name = "hSmartWindowControl1";
            this.hSmartWindowControl1.Size = new System.Drawing.Size(1280, 1024);
            this.hSmartWindowControl1.TabIndex = 6;
            this.hSmartWindowControl1.WindowSize = new System.Drawing.Size(1280, 1024);
            this.hSmartWindowControl1.Load += new System.EventHandler(this.hSmartWindowControl1_Load_1);
            // 
            // button1
            // 
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 28.25F);
            this.button1.Location = new System.Drawing.Point(825, 796);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(384, 52);
            this.button1.TabIndex = 7;
            this.button1.Text = "Piece 2";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 28.25F);
            this.button2.Location = new System.Drawing.Point(825, 854);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(384, 52);
            this.button2.TabIndex = 8;
            this.button2.Text = "Piece 3";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Acq_Image
            // 
            this.Acq_Image.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Acq_Image.Font = new System.Drawing.Font("Microsoft Sans Serif", 28.25F);
            this.Acq_Image.Location = new System.Drawing.Point(373, 796);
            this.Acq_Image.Name = "Acq_Image";
            this.Acq_Image.Size = new System.Drawing.Size(384, 52);
            this.Acq_Image.TabIndex = 9;
            this.Acq_Image.Text = "Prend l\'image.";
            this.Acq_Image.UseVisualStyleBackColor = true;
            this.Acq_Image.Click += new System.EventHandler(this.Acq_Image_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1292, 996);
            this.Controls.Add(this.Acq_Image);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Start_IA);
            this.Controls.Add(this.hSmartWindowControl1);
            this.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button Start_IA;
        private System.Windows.Forms.Label label1;
        private HalconDotNet.HSmartWindowControl hSmartWindowControl1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button Acq_Image;
    }
}

