namespace EmailDataApp
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.CheckBox checkBoxEmailId;
        private System.Windows.Forms.CheckBox checkBoxParentId;
        private System.Windows.Forms.CheckBox checkBoxSubject;
        private System.Windows.Forms.CheckBox checkBoxTextBody;
        private System.Windows.Forms.CheckBox checkBoxMessageDate;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.checkBoxEmailId = new System.Windows.Forms.CheckBox();
            this.checkBoxParentId = new System.Windows.Forms.CheckBox();
            this.checkBoxSubject = new System.Windows.Forms.CheckBox();
            this.checkBoxTextBody = new System.Windows.Forms.CheckBox();
            this.checkBoxMessageDate = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(16, 15);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 28);
            this.button1.TabIndex = 0;
            this.button1.Text = "Fetch Emails";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(124, 15);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 28);
            this.button2.TabIndex = 1;
            this.button2.Text = "Save as PDF";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(16, 50);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.Size = new System.Drawing.Size(750, 489);
            this.dataGridView1.TabIndex = 2;
            // 
            // checkBoxEmailId
            // 
            this.checkBoxEmailId.AutoSize = true;
            this.checkBoxEmailId.Location = new System.Drawing.Point(232, 20);
            this.checkBoxEmailId.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxEmailId.Name = "checkBoxEmailId";
            this.checkBoxEmailId.Size = new System.Drawing.Size(77, 20);
            this.checkBoxEmailId.TabIndex = 3;
            this.checkBoxEmailId.Text = "Email Id";
            this.checkBoxEmailId.UseVisualStyleBackColor = true;
            // 
            // checkBoxParentId
            // 
            this.checkBoxParentId.AutoSize = true;
            this.checkBoxParentId.Location = new System.Drawing.Point(324, 20);
            this.checkBoxParentId.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxParentId.Name = "checkBoxParentId";
            this.checkBoxParentId.Size = new System.Drawing.Size(82, 20);
            this.checkBoxParentId.TabIndex = 4;
            this.checkBoxParentId.Text = "Parent Id";
            this.checkBoxParentId.UseVisualStyleBackColor = true;
            // 
            // checkBoxSubject
            // 
            this.checkBoxSubject.AutoSize = true;
            this.checkBoxSubject.Location = new System.Drawing.Point(425, 20);
            this.checkBoxSubject.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxSubject.Name = "checkBoxSubject";
            this.checkBoxSubject.Size = new System.Drawing.Size(74, 20);
            this.checkBoxSubject.TabIndex = 5;
            this.checkBoxSubject.Text = "Subject";
            this.checkBoxSubject.UseVisualStyleBackColor = true;
            // 
            // checkBoxTextBody
            // 
            this.checkBoxTextBody.AutoSize = true;
            this.checkBoxTextBody.Location = new System.Drawing.Point(516, 20);
            this.checkBoxTextBody.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxTextBody.Name = "checkBoxTextBody";
            this.checkBoxTextBody.Size = new System.Drawing.Size(90, 20);
            this.checkBoxTextBody.TabIndex = 6;
            this.checkBoxTextBody.Text = "Text Body";
            this.checkBoxTextBody.UseVisualStyleBackColor = true;
            // 
            // checkBoxMessageDate
            // 
            this.checkBoxMessageDate.AutoSize = true;
            this.checkBoxMessageDate.Location = new System.Drawing.Point(621, 20);
            this.checkBoxMessageDate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxMessageDate.Name = "checkBoxMessageDate";
            this.checkBoxMessageDate.Size = new System.Drawing.Size(118, 20);
            this.checkBoxMessageDate.TabIndex = 7;
            this.checkBoxMessageDate.Text = "Message Date";
            this.checkBoxMessageDate.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 554);
            this.Controls.Add(this.checkBoxMessageDate);
            this.Controls.Add(this.checkBoxTextBody);
            this.Controls.Add(this.checkBoxSubject);
            this.Controls.Add(this.checkBoxParentId);
            this.Controls.Add(this.checkBoxEmailId);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Form1";
            this.Text = "Email Data App";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
