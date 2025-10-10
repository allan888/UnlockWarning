using System.Security.Cryptography;
using System.Text;

namespace WarningUnlock;

public partial class sha_256 : Form
{
    private string Path = "";
    private string result = "";
    public sha_256(string path)
    {
        InitializeComponent();
        this.Path = path;
        Show_SHA256();
    }
    public string GetFileSHA256()
    {
        using (FileStream stream = File.OpenRead(Path))
        using (SHA256 sha = SHA256.Create())
        {
            byte[] hashBytes = sha.ComputeHash(stream);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
                sb.Append(b.ToString("x2")); // 转16进制字符串
            return sb.ToString();
        }
    }

    private void Show_SHA256()
    {
        if (Path != "")
        {
            result = this.GetFileSHA256();
            if (result!=null)
            {
                this.label2.Text = result;
            }
        }
        
    }

    private void button1_Click(object sender, EventArgs e)
    {
        if (this.textBox1.Text != ""&&result!="")
        {
            if (this.textBox1.Text == result)
            {
                MessageBox.Show("相同");
            }
            else
            {
                MessageBox.Show("不相同");
            }
        }
        else
        {
            MessageBox.Show("出现未知问题");
        }
    }
}