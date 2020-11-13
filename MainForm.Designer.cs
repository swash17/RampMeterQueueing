namespace QueueCalcs
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
            this.btnStart = new System.Windows.Forms.Button();
            this.btnWriteDataFile = new System.Windows.Forms.Button();
            this.btnReadDataFile = new System.Windows.Forms.Button();
            this.rtbMessages = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(63, 90);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(186, 48);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Run Queuing Analysis";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnWriteDataFile
            // 
            this.btnWriteDataFile.Location = new System.Drawing.Point(63, 156);
            this.btnWriteDataFile.Name = "btnWriteDataFile";
            this.btnWriteDataFile.Size = new System.Drawing.Size(186, 48);
            this.btnWriteDataFile.TabIndex = 2;
            this.btnWriteDataFile.Text = "Write Input File";
            this.btnWriteDataFile.UseVisualStyleBackColor = true;
            this.btnWriteDataFile.Click += new System.EventHandler(this.btnWriteDataFile_Click);
            // 
            // btnReadDataFile
            // 
            this.btnReadDataFile.Location = new System.Drawing.Point(63, 25);
            this.btnReadDataFile.Name = "btnReadDataFile";
            this.btnReadDataFile.Size = new System.Drawing.Size(186, 48);
            this.btnReadDataFile.TabIndex = 0;
            this.btnReadDataFile.Text = "Read Input File";
            this.btnReadDataFile.UseVisualStyleBackColor = true;
            this.btnReadDataFile.Click += new System.EventHandler(this.btnReadDataFile_Click);
            // 
            // rtbMessages
            // 
            this.rtbMessages.Location = new System.Drawing.Point(12, 246);
            this.rtbMessages.Name = "rtbMessages";
            this.rtbMessages.Size = new System.Drawing.Size(288, 96);
            this.rtbMessages.TabIndex = 3;
            this.rtbMessages.Text = "";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(312, 354);
            this.Controls.Add(this.rtbMessages);
            this.Controls.Add(this.btnReadDataFile);
            this.Controls.Add(this.btnWriteDataFile);
            this.Controls.Add(this.btnStart);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "Macroscopic Queuing Calculations";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnWriteDataFile;
        private System.Windows.Forms.Button btnReadDataFile;
        private System.Windows.Forms.RichTextBox rtbMessages;
    }
}