namespace FinancePlus
{
    partial class CommentForm
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
            this.cancelButton = new System.Windows.Forms.Button();
            this.OkButton = new System.Windows.Forms.Button();
            this.businessNameLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.commentTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(145, 227);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "ביטול";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // OkButton
            // 
            this.OkButton.Location = new System.Drawing.Point(64, 227);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(75, 23);
            this.OkButton.TabIndex = 2;
            this.OkButton.Text = "אישור";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // businessNameLabel
            // 
            this.businessNameLabel.AutoSize = true;
            this.businessNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.businessNameLabel.Location = new System.Drawing.Point(58, 31);
            this.businessNameLabel.Name = "businessNameLabel";
            this.businessNameLabel.Size = new System.Drawing.Size(99, 16);
            this.businessNameLabel.TabIndex = 9;
            this.businessNameLabel.Text = "שם בית העסק:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "שם בית העסק:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.commentTextBox);
            this.groupBox1.Location = new System.Drawing.Point(15, 66);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(257, 58);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "הערה";
            // 
            // commentTextBox
            // 
            this.commentTextBox.Location = new System.Drawing.Point(26, 23);
            this.commentTextBox.Name = "commentTextBox";
            this.commentTextBox.Size = new System.Drawing.Size(205, 20);
            this.commentTextBox.TabIndex = 0;
            // 
            // CommentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.businessNameLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.OkButton);
            this.Name = "CommentForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Text = "עריכה";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.Label businessNameLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox commentTextBox;
    }
}