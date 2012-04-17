namespace FinancePlus
{
    partial class CategoryForm
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
            this.OkButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.primaryCategoryComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.secondaryCategoryComboBox = new System.Windows.Forms.ComboBox();
            this.businessNameLabel = new System.Windows.Forms.Label();
            this.ApplyCategoryCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // OkButton
            // 
            this.OkButton.Location = new System.Drawing.Point(86, 227);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(75, 23);
            this.OkButton.TabIndex = 0;
            this.OkButton.Text = "אישור";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(167, 227);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 1;
            this.CancelButton.Text = "ביטול";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "שם בית העסק:";
            // 
            // primaryCategoryComboBox
            // 
            this.primaryCategoryComboBox.FormattingEnabled = true;
            this.primaryCategoryComboBox.Location = new System.Drawing.Point(121, 77);
            this.primaryCategoryComboBox.Name = "primaryCategoryComboBox";
            this.primaryCategoryComboBox.Size = new System.Drawing.Size(121, 21);
            this.primaryCategoryComboBox.TabIndex = 3;
            this.primaryCategoryComboBox.SelectedIndexChanged += new System.EventHandler(this.primaryCategoryComboBox_SelectedIndexChanged);
            this.primaryCategoryComboBox.TextChanged += new System.EventHandler(this.primaryCategoryComboBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "קטגוריה ראשית:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 113);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "קטגוריה משנית:";
            // 
            // secondaryCategoryComboBox
            // 
            this.secondaryCategoryComboBox.FormattingEnabled = true;
            this.secondaryCategoryComboBox.Location = new System.Drawing.Point(121, 110);
            this.secondaryCategoryComboBox.Name = "secondaryCategoryComboBox";
            this.secondaryCategoryComboBox.Size = new System.Drawing.Size(121, 21);
            this.secondaryCategoryComboBox.TabIndex = 5;
            // 
            // businessNameLabel
            // 
            this.businessNameLabel.AutoSize = true;
            this.businessNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.businessNameLabel.Location = new System.Drawing.Point(58, 31);
            this.businessNameLabel.Name = "businessNameLabel";
            this.businessNameLabel.Size = new System.Drawing.Size(99, 16);
            this.businessNameLabel.TabIndex = 7;
            this.businessNameLabel.Text = "שם בית העסק:";
            // 
            // ApplyCategoryCheckBox
            // 
            this.ApplyCategoryCheckBox.AutoSize = true;
            this.ApplyCategoryCheckBox.Checked = true;
            this.ApplyCategoryCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ApplyCategoryCheckBox.Location = new System.Drawing.Point(28, 140);
            this.ApplyCategoryCheckBox.Name = "ApplyCategoryCheckBox";
            this.ApplyCategoryCheckBox.Size = new System.Drawing.Size(278, 17);
            this.ApplyCategoryCheckBox.TabIndex = 8;
            this.ApplyCategoryCheckBox.Text = "שייך בית עסק זה לקטגוריות שנקבעו באופן קבוע";
            this.ApplyCategoryCheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(12, 56);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(298, 116);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "קטגוריה";
            // 
            // CategoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(322, 262);
            this.Controls.Add(this.ApplyCategoryCheckBox);
            this.Controls.Add(this.businessNameLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.secondaryCategoryComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.primaryCategoryComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.groupBox1);
            this.Name = "CategoryForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Text = "עריכה";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox primaryCategoryComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox secondaryCategoryComboBox;
        private System.Windows.Forms.Label businessNameLabel;
        private System.Windows.Forms.CheckBox ApplyCategoryCheckBox;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}