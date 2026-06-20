using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace UnlockWarning;

public partial class License : Page
{
    public License()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        MachineCodeTextBox.Text = LicenseManager.GetMachineCode();
        RefreshStatus();
    }

    private void RefreshStatus()
    {
        var (activated, _, expiry) = LicenseManager.LoadActivation();
        if (activated && expiry.HasValue)
        {
            StatusLabel.Text = "已激活";
            StatusLabel.Foreground = System.Windows.Media.Brushes.Green;
            ExpiryLabel.Text = expiry.Value.ToLocalTime().ToString("yyyy-MM-dd");
        }
        else if (expiry.HasValue && expiry.Value < DateTime.UtcNow)
        {
            StatusLabel.Text = "已过期";
            StatusLabel.Foreground = System.Windows.Media.Brushes.OrangeRed;
            ExpiryLabel.Text = expiry.Value.ToLocalTime().ToString("yyyy-MM-dd");
        }
        else
        {
            StatusLabel.Text = "未激活";
            StatusLabel.Foreground = System.Windows.Media.Brushes.Red;
            ExpiryLabel.Text = "—";
        }
    }

    private void CopyMachineCode_Click(object sender, RoutedEventArgs e)
    {
        System.Windows.Clipboard.SetText(MachineCodeTextBox.Text);
        MessageLabel.Text = "机器码已复制到剪贴板";
    }

    private void ActivateButton_Click(object sender, RoutedEventArgs e)
    {
        string serial = SerialTextBox.Text.Trim();
        if (string.IsNullOrEmpty(serial))
        {
            MessageLabel.Text = "请输入序列号";
            return;
        }

        var (valid, expiry) = LicenseManager.VerifyLicense(serial);
        if (valid && expiry.HasValue)
        {
            LicenseManager.SaveActivation(serial, expiry.Value);
            RefreshStatus();
            MessageLabel.Text = "激活成功！";
        }
        else if (expiry.HasValue)
        {
            MessageLabel.Text = "序列号已过期";
        }
        else
        {
            MessageLabel.Text = "序列号无效或与当前机器不匹配";
        }
    }

    private void ImportFile_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Title = "选择许可证文件",
            Filter = "许可证文件 (*.lic)|*.lic|所有文件 (*.*)|*.*",
            DefaultExt = ".lic"
        };

        if (dialog.ShowDialog() == true)
        {
            try
            {
                string content = File.ReadAllText(dialog.FileName).Trim();
                var (valid, expiry) = LicenseManager.VerifyLicense(content);
                if (valid && expiry.HasValue)
                {
                    LicenseManager.SaveActivation(content, expiry.Value);
                    SerialTextBox.Text = content;
                    RefreshStatus();
                    MessageLabel.Text = "许可证文件导入成功！";
                }
                else if (expiry.HasValue)
                {
                    MessageLabel.Text = "许可证已过期";
                }
                else
                {
                    MessageLabel.Text = "许可证文件无效或与当前机器不匹配";
                }
            }
            catch (Exception ex)
            {
                MessageLabel.Text = "读取文件失败：" + ex.Message;
            }
        }
    }
}
