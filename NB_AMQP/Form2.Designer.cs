
namespace NB_AMQP
{
    partial class Form2
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
            this.text_humidity_threshold = new System.Windows.Forms.TextBox();
            this.text_temperature_threshold = new System.Windows.Forms.TextBox();
            this.湿度阈值 = new System.Windows.Forms.Label();
            this.温度阈值 = new System.Windows.Forms.Label();
            this.button_cancel = new System.Windows.Forms.Button();
            this.button_save = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // text_humidity_threshold
            // 
            this.text_humidity_threshold.Location = new System.Drawing.Point(144, 86);
            this.text_humidity_threshold.Name = "text_humidity_threshold";
            this.text_humidity_threshold.Size = new System.Drawing.Size(100, 25);
            this.text_humidity_threshold.TabIndex = 14;
            // 
            // text_temperature_threshold
            // 
            this.text_temperature_threshold.Location = new System.Drawing.Point(144, 39);
            this.text_temperature_threshold.Name = "text_temperature_threshold";
            this.text_temperature_threshold.Size = new System.Drawing.Size(100, 25);
            this.text_temperature_threshold.TabIndex = 15;
            // 
            // 湿度阈值
            // 
            this.湿度阈值.AutoSize = true;
            this.湿度阈值.Location = new System.Drawing.Point(56, 86);
            this.湿度阈值.Name = "湿度阈值";
            this.湿度阈值.Size = new System.Drawing.Size(67, 15);
            this.湿度阈值.TabIndex = 12;
            this.湿度阈值.Text = "湿度阈值";
            // 
            // 温度阈值
            // 
            this.温度阈值.AutoSize = true;
            this.温度阈值.Location = new System.Drawing.Point(56, 44);
            this.温度阈值.Name = "温度阈值";
            this.温度阈值.Size = new System.Drawing.Size(67, 15);
            this.温度阈值.TabIndex = 13;
            this.温度阈值.Text = "温度阈值";
            // 
            // button_cancel
            // 
            this.button_cancel.Location = new System.Drawing.Point(197, 143);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(75, 23);
            this.button_cancel.TabIndex = 16;
            this.button_cancel.Text = "取消";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(51, 143);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(75, 23);
            this.button_save.TabIndex = 17;
            this.button_save.Text = "保存";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(315, 202);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.text_humidity_threshold);
            this.Controls.Add(this.text_temperature_threshold);
            this.Controls.Add(this.湿度阈值);
            this.Controls.Add(this.温度阈值);
            this.Name = "Form2";
            this.Text = "温湿度阈值设置";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox text_humidity_threshold;
        private System.Windows.Forms.TextBox text_temperature_threshold;
        private System.Windows.Forms.Label 湿度阈值;
        private System.Windows.Forms.Label 温度阈值;
        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.Button button_save;
    }
}