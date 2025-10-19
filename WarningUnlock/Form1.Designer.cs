namespace WarningUnlock;

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
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
        button1 = new System.Windows.Forms.Button();
        pictureBox1 = new System.Windows.Forms.PictureBox();
        ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
        SuspendLayout();
        // 
        // button1
        // 
        button1.Font = new System.Drawing.Font("思源黑体 CN Medium", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        button1.Location = new System.Drawing.Point(258, 12);
        button1.Name = "button1";
        button1.Size = new System.Drawing.Size(280, 89);
        button1.TabIndex = 0;
        button1.Text = "打开/关上";
        button1.UseVisualStyleBackColor = true;
        button1.Click += button1_Click;
        // 
        // pictureBox1
        // 
        pictureBox1.Location = new System.Drawing.Point(57, 133);
        pictureBox1.Name = "pictureBox1";
        pictureBox1.Size = new System.Drawing.Size(691, 237);
        pictureBox1.TabIndex = 1;
        pictureBox1.TabStop = false;
        // 
        // Form1
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(800, 450);
        Controls.Add(pictureBox1);
        Controls.Add(button1);
        Icon = ((System.Drawing.Icon)resources.GetObject("$this.Icon"));
        Text = "CJY安全卫士";
        ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
        ResumeLayout(false);
    }

    private System.Windows.Forms.PictureBox pictureBox1;

    private System.Windows.Forms.Button button1;

    #endregion
}