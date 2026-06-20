using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using Microsoft.Win32;

namespace GenerateLicense;

public partial class MainWindow : Window
{
    private string? _currentLicense;

    public MainWindow()
    {
        InitializeComponent();
        ExpiryDatePicker.SelectedDate = DateTime.Now.AddYears(1);
    }

    private void GenerateButton_Click(object sender, RoutedEventArgs e)
    {
        string machineCode = MachineCodeTextBox.Text.Trim();
        if (string.IsNullOrEmpty(machineCode))
        {
            MessageLabel.Text = "请输入机器码";
            return;
        }

        if (ExpiryDatePicker.SelectedDate == null)
        {
            MessageLabel.Text = "请选择到期日期";
            return;
        }

        DateTime expiry = ExpiryDatePicker.SelectedDate.Value;
        DateTime expiryUtc = expiry.ToUniversalTime();
        long ticks = expiryUtc.Ticks;

        byte[] secretKey = Encoding.UTF8.GetBytes("CJY@AnquanWeishi#License2025");

        string payloadStr = machineCode + "|" + ticks;
        byte[] payload = Encoding.UTF8.GetBytes(payloadStr);
        string payloadBase64 = Convert.ToBase64String(payload);

        byte[] hmac = HMACSHA256.HashData(secretKey, payload);
        string hmacBase64 = Convert.ToBase64String(hmac);

        _currentLicense = payloadBase64 + ":" + hmacBase64;
        ResultTextBox.Text = _currentLicense;
        ExportButton.IsEnabled = true;
        MessageLabel.Text = "";
    }

    private void ExportButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_currentLicense))
            return;

        SaveFileDialog dialog = new SaveFileDialog
        {
            Title = "导出许可证文件",
            Filter = "许可证文件 (*.lic)|*.lic",
            DefaultExt = ".lic",
            FileName = "license.lic"
        };

        if (dialog.ShowDialog() == true)
        {
            try
            {
                File.WriteAllText(dialog.FileName, _currentLicense);
                MessageLabel.Text = "导出成功：" + dialog.FileName;
            }
            catch (Exception ex)
            {
                MessageLabel.Text = "导出失败：" + ex.Message;
            }
        }
    }
}
