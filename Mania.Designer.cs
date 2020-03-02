namespace Mania
{
    partial class Mania
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
            this.Countdown = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Countdown
            // 
            this.Countdown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Countdown.Font = new System.Drawing.Font("Microsoft JhengHei", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Countdown.ForeColor = System.Drawing.Color.Red;
            this.Countdown.Location = new System.Drawing.Point(0, 0);
            this.Countdown.Name = "Countdown";
            this.Countdown.Size = new System.Drawing.Size(178, 744);
            this.Countdown.TabIndex = 0;
            this.Countdown.Text = "3";
            this.Countdown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Countdown.Visible = false;
            // 
            // Mania
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(178, 744);
            this.Controls.Add(this.Countdown);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Mania";
            this.Text = "Mania by palapapa";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Mania_FormClosing);
            this.Load += new System.EventHandler(this.Mania_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Mania_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Mania_KeyUp);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label Countdown;
    }
}