using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UnlockWarning;

public partial class Sha256StringPage : Page
{
    public Sha256StringPage()
    {
        InitializeComponent();
    }

    private void ComputeButton_Click(object sender, RoutedEventArgs e)
    {
        string input = InputTextBox.Text;
        if (string.IsNullOrEmpty(input))
        {
            ResultLabel.Text = "请输入字符串";
            return;
        }

        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] hashBytes = SHA256.HashData(inputBytes);
        StringBuilder sb = new StringBuilder();
        foreach (byte b in hashBytes)
            sb.Append(b.ToString("x2"));
        ResultLabel.Text = sb.ToString();
    }
}
