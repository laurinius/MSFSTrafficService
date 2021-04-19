
namespace TrafficService
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.startStopHttpButton = new System.Windows.Forms.Button();
            this.httpStatusLabel = new System.Windows.Forms.Label();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.autoStartCheckBox = new System.Windows.Forms.CheckBox();
            this.minimizeToTrayCheckBox = new System.Windows.Forms.CheckBox();
            this.startMinimizedCheckBox = new System.Windows.Forms.CheckBox();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.SuspendLayout();
            // 
            // startStopHttpButton
            // 
            this.startStopHttpButton.Location = new System.Drawing.Point(12, 12);
            this.startStopHttpButton.Name = "startStopHttpButton";
            this.startStopHttpButton.Size = new System.Drawing.Size(75, 23);
            this.startStopHttpButton.TabIndex = 1;
            this.startStopHttpButton.Text = "Start";
            this.startStopHttpButton.UseVisualStyleBackColor = true;
            this.startStopHttpButton.Click += new System.EventHandler(this.StartStopHttpButton_Click);
            // 
            // httpStatusLabel
            // 
            this.httpStatusLabel.Location = new System.Drawing.Point(12, 88);
            this.httpStatusLabel.Name = "httpStatusLabel";
            this.httpStatusLabel.Size = new System.Drawing.Size(175, 71);
            this.httpStatusLabel.TabIndex = 2;
            this.httpStatusLabel.Text = "Service stopped.";
            // 
            // portTextBox
            // 
            this.portTextBox.Location = new System.Drawing.Point(142, 12);
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(45, 23);
            this.portTextBox.TabIndex = 3;
            this.portTextBox.Text = "8383";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(107, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Port";
            // 
            // autoStartCheckBox
            // 
            this.autoStartCheckBox.AutoSize = true;
            this.autoStartCheckBox.Checked = true;
            this.autoStartCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoStartCheckBox.Location = new System.Drawing.Point(12, 41);
            this.autoStartCheckBox.Name = "autoStartCheckBox";
            this.autoStartCheckBox.Size = new System.Drawing.Size(78, 19);
            this.autoStartCheckBox.TabIndex = 5;
            this.autoStartCheckBox.Text = "Auto-Run";
            this.autoStartCheckBox.UseVisualStyleBackColor = true;
            this.autoStartCheckBox.CheckedChanged += new System.EventHandler(this.AutoStartCheckBox_CheckedChanged);
            // 
            // minimizeToTrayCheckBox
            // 
            this.minimizeToTrayCheckBox.AutoSize = true;
            this.minimizeToTrayCheckBox.Location = new System.Drawing.Point(12, 66);
            this.minimizeToTrayCheckBox.Name = "minimizeToTrayCheckBox";
            this.minimizeToTrayCheckBox.Size = new System.Drawing.Size(112, 19);
            this.minimizeToTrayCheckBox.TabIndex = 6;
            this.minimizeToTrayCheckBox.Text = "Minimize to tray";
            this.minimizeToTrayCheckBox.UseVisualStyleBackColor = true;
            this.minimizeToTrayCheckBox.CheckedChanged += new System.EventHandler(this.MinimizeToTrayCheckBox_CheckedChanged);
            // 
            // startMinimizedCheckBox
            // 
            this.startMinimizedCheckBox.AutoSize = true;
            this.startMinimizedCheckBox.Location = new System.Drawing.Point(87, 41);
            this.startMinimizedCheckBox.Name = "startMinimizedCheckBox";
            this.startMinimizedCheckBox.Size = new System.Drawing.Size(109, 19);
            this.startMinimizedCheckBox.TabIndex = 7;
            this.startMinimizedCheckBox.Text = "Start Minimized";
            this.startMinimizedCheckBox.UseVisualStyleBackColor = true;
            this.startMinimizedCheckBox.CheckedChanged += new System.EventHandler(this.StartMinimizedCheckBox_CheckedChanged);
            // 
            // notifyIcon
            // 
            this.notifyIcon.Text = "Traffic Service";
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon_MouseDoubleClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(199, 168);
            this.Controls.Add(this.startMinimizedCheckBox);
            this.Controls.Add(this.minimizeToTrayCheckBox);
            this.Controls.Add(this.autoStartCheckBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.portTextBox);
            this.Controls.Add(this.httpStatusLabel);
            this.Controls.Add(this.startStopHttpButton);
            this.Name = "Form1";
            this.Text = "Traffic";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button startStopHttpButton;
        private System.Windows.Forms.Label httpStatusLabel;
        private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox autoStartCheckBox;
        private System.Windows.Forms.CheckBox minimizeToTrayCheckBox;
        private System.Windows.Forms.CheckBox startMinimizedCheckBox;
        private System.Windows.Forms.NotifyIcon notifyIcon;
    }
}

