using System.ComponentModel;

namespace WarningUnlock;

partial class sha_256
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private IContainer components = null;

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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(sha_256));
        label1 = new System.Windows.Forms.Label();
        label2 = new System.Windows.Forms.Label();
        button1 = new System.Windows.Forms.Button();
        textBox1 = new System.Windows.Forms.TextBox();
        SuspendLayout();
        // 
        // label1
        // 
        label1.Font = new System.Drawing.Font("思源黑体 CN Medium", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        label1.Location = new System.Drawing.Point(54, 85);
        label1.Name = "label1";
        label1.Size = new System.Drawing.Size(187, 49);
        label1.TabIndex = 0;
        label1.Text = "SHA256值：";
        label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        // 
        // label2
        // 
        label2.Font = new System.Drawing.Font("Microsoft YaHei UI", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        label2.Location = new System.Drawing.Point(213, 96);
        label2.Name = "label2";
        label2.Size = new System.Drawing.Size(551, 37);
        label2.TabIndex = 1;
        label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        // 
        // button1
        // 
        button1.Location = new System.Drawing.Point(642, 151);
        button1.Name = "button1";
        button1.Size = new System.Drawing.Size(121, 53);
        button1.TabIndex = 3;
        button1.Text = "校验";
        button1.UseVisualStyleBackColor = true;
        button1.Click += button1_Click;
        // 
        // textBox1
        // 
        textBox1.AcceptsReturn = true;
        textBox1.Location = new System.Drawing.Point(12, 151);
        textBox1.Multiline = true;
        textBox1.Name = "textBox1";
        textBox1.Size = new System.Drawing.Size(624, 53);
        textBox1.TabIndex = 4;
        // 
        // sha_256
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(800, 265);
        Controls.Add(textBox1);
        Controls.Add(button1);
        Controls.Add(label2);
        Controls.Add(label1);
        Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        Text = "SHA256值";
        ResumeLayout(false);
        PerformLayout();
    }

    private System.Windows.Forms.TextBox textBox1;

    private System.Windows.Forms.Button button1;

    private System.Windows.Forms.Label label2;

    private System.Windows.Forms.Label label1;

    #endregion
}