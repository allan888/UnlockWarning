using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace UnlockWarning;

public partial class License : Page
{
    private string? _licContent;
    private bool _serialValid;
    private bool _licValid;

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

    private void SerialTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        string serial = SerialTextBox.Text.Trim();
        if (string.IsNullOrEmpty(serial))
        {
            SerialStatusLabel.Text = "";
            _serialValid = false;
        }
        else
        {
            var (valid, expiry) = LicenseManager.VerifyLicense(serial);
            _serialValid = valid && expiry.HasValue;
            if (_serialValid)
                SerialStatusLabel.Text = "✓ 有效，到期 " + expiry!.Value.ToLocalTime().ToString("yyyy-MM-dd");
            else if (expiry.HasValue)
                SerialStatusLabel.Text = "✗ 已过期";
            else
                SerialStatusLabel.Text = "✗ 序列号无效";
            SerialStatusLabel.Foreground = new System.Windows.Media.SolidColorBrush(
                _serialValid ? System.Windows.Media.Color.FromRgb(0x5c, 0xb8, 0x5c) :
                System.Windows.Media.Color.FromRgb(0xd9, 0x53, 0x4f));
        }
        UpdateActivateButton();
    }

    private void ImportFile_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new System.Windows.Forms.OpenFileDialog
        {
            Title = "选择许可证文件",
            Filter = "许可证文件 (*.lic)|*.lic|所有文件 (*.*)|*.*",
            DefaultExt = "lic"
        };

        IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(
            Window.GetWindow(this)).EnsureHandle();
        var wrapper = new Win32Window(hwnd);

        if (dialog.ShowDialog(wrapper) == System.Windows.Forms.DialogResult.OK)
        {
            try
            {
                _licContent = File.ReadAllText(dialog.FileName).Trim();
                LicPathTextBox.Text = dialog.FileName;

                var (valid, expiry) = LicenseManager.VerifyLicense(_licContent);
                _licValid = valid && expiry.HasValue;
                if (_licValid)
                    LicStatusLabel.Text = "✓ 有效，到期 " + expiry!.Value.ToLocalTime().ToString("yyyy-MM-dd");
                else if (expiry.HasValue)
                    LicStatusLabel.Text = "✗ 已过期";
                else
                    LicStatusLabel.Text = "✗ 文件无效或与当前机器不匹配";
                LicStatusLabel.Foreground = new System.Windows.Media.SolidColorBrush(
                    _licValid ? System.Windows.Media.Color.FromRgb(0x5c, 0xb8, 0x5c) :
                    System.Windows.Media.Color.FromRgb(0xd9, 0x53, 0x4f));
            }
            catch (Exception ex)
            {
                LicStatusLabel.Text = "读取失败：" + ex.Message;
                LicStatusLabel.Foreground = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(0xd9, 0x53, 0x4f));
                _licValid = false;
            }
        }
        UpdateActivateButton();
    }

    private void UpdateActivateButton()
    {
        ActivateButton.IsEnabled = _serialValid && _licValid;
    }

    private void ActivateButton_Click(object sender, RoutedEventArgs e)
    {
        string serial = SerialTextBox.Text.Trim();
        if (!_serialValid || !_licValid || _licContent == null)
        {
            MessageLabel.Text = "请先确保序列号和许可证文件均有效";
            return;
        }

        var (sValid, sExpiry) = LicenseManager.VerifyLicense(serial);
        var (lValid, lExpiry) = LicenseManager.VerifyLicense(_licContent);

        if (!sValid || !lValid || sExpiry == null || lExpiry == null)
        {
            MessageLabel.Text = "验证失败，请重新检查序列号和文件";
            return;
        }

        if (sExpiry.Value.Ticks != lExpiry.Value.Ticks)
        {
            MessageLabel.Text = "序列号与许可证文件的到期时间不一致";
            return;
        }

        LicenseManager.SaveActivation(serial, sExpiry.Value);
        RefreshStatus();
        MessageLabel.Text = "激活成功！";
    }
}

internal class Win32Window : System.Windows.Forms.IWin32Window
{
    public IntPtr Handle { get; }
    public Win32Window(IntPtr handle) => Handle = handle;
}
