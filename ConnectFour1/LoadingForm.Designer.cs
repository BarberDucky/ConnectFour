namespace ConnectFour1
{
    partial class LoadingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoadingForm));
            this.pbxTabla = new System.Windows.Forms.PictureBox();
            this.lblC4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTabla)).BeginInit();
            this.SuspendLayout();
            // 
            // pbxTabla
            // 
            this.pbxTabla.BackgroundImage = global::ConnectFour1.Resource.tabla;
            this.pbxTabla.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbxTabla.Location = new System.Drawing.Point(20, 20);
            this.pbxTabla.Name = "pbxTabla";
            this.pbxTabla.Size = new System.Drawing.Size(300, 300);
            this.pbxTabla.TabIndex = 3;
            this.pbxTabla.TabStop = false;
            // 
            // lblC4
            // 
            this.lblC4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblC4.AutoSize = true;
            this.lblC4.Font = new System.Drawing.Font("Calibri Light", 13F, System.Drawing.FontStyle.Bold);
            this.lblC4.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblC4.Location = new System.Drawing.Point(91, 325);
            this.lblC4.Name = "lblC4";
            this.lblC4.Size = new System.Drawing.Size(141, 27);
            this.lblC4.TabIndex = 4;
            this.lblC4.Text = "Connect Four";
            // 
            // LoadingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(139)))), ((int)(((byte)(205)))));
            this.ClientSize = new System.Drawing.Size(340, 361);
            this.Controls.Add(this.lblC4);
            this.Controls.Add(this.pbxTabla);
            this.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoadingForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)(this.pbxTabla)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pbxTabla;
        private System.Windows.Forms.Label lblC4;
    }
}