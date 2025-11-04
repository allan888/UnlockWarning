using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows;
using Microsoft.Win32;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
namespace UnlockWarning;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private MainWindow mainWindow = null;
    private System.Windows.Forms.NotifyIcon notifyIcon;
    private NotificationWindow notificationWindow;
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool GetCursorPos(out POINT pt);
    public struct POINT
    {
        public int X;
        public int Y;
        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

    }
    private void Application_Startup(object sender, StartupEventArgs e)
        {
            // 初始化托盘图标
            this.notifyIcon = new NotifyIcon();
            this.notifyIcon.BalloonTipText = "系统监控中... ...";
            this.notifyIcon.ShowBalloonTip(2000);
            this.notifyIcon.Text = "系统监控中... ...";
            using (var ms = new MemoryStream(Resources1.icon))
            {
                Icon icon = new Icon(ms);
                this.notifyIcon.Icon = icon;
            }
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseClick += Show_NotificationWindow;
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler((o, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (mainWindow == null)
                    {
                        mainWindow = new MainWindow();
                        mainWindow.Show();
                    }
                    else
                    {
                        mainWindow.Show();
                    }
                }
            });
            if (e.Args.Length > 0)
            {
                if (e.Args.Length == 2 && e.Args[0] == "-S")
                {
                    // 右键调用，显示 SHA256 窗口
                    var shaWindow = new sha_256(e.Args[1]);
                    shaWindow.Show();
                    return;
                }
                if (e.Args[0] == "-N")
                {
                    Main_Window(true);
                    return;
                }
            }
            Main_Window(false);
        }

    private void Main_Window(bool hide)
    {
        // 普通启动路径
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);

            if (principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                try
                {
                    bool isRegistered = false;
                    string menuName = "WarningUnlockSHA256";
                    string command = $"\"{Process.GetCurrentProcess().MainModule.FileName}\" -S \"%1\"";
                    string regPath = @"*\shell\" + menuName;

                    // 检查是否已经注册
                    using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(regPath))
                    {
                        if (key != null)
                            isRegistered = true;
                    }

                    // 如果未注册，则注册右键菜单
                    if (!isRegistered)
                    {
                        using (RegistryKey shellKey = Registry.ClassesRoot.CreateSubKey(regPath))
                        {
                            shellKey.SetValue(null, "显示SHA256值");
                            shellKey.SetValue("Icon", Process.GetCurrentProcess().MainModule.FileName);
                        }

                        using (RegistryKey commandKey = Registry.ClassesRoot.CreateSubKey(regPath + @"\command"))
                        {
                            commandKey.SetValue(null, command);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("注册右键菜单时出错：" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                Utility.DetectAndStartService("Windows Hanging Up");
                // 启动主界面
                mainWindow = new MainWindow();
                if (hide)
                {
                    mainWindow.Visibility = Visibility.Hidden;
                }
                mainWindow.Show();
            }
            else
            {
                // 重新以管理员身份运行
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    UseShellExecute = true,
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName = Process.GetCurrentProcess().MainModule.FileName,
                    Verb = "runas"
                };
                if (hide)
                {
                    startInfo.Arguments = "-N";
                }
                try
                {
                    System.Diagnostics.Process.Start(startInfo);
                }
                catch
                {
                    // 用户拒绝 UAC 提升
                }

                // 退出当前实例
                Shutdown();
            }
    }
    public void NotificationWindow_Loaded()
    {
        if (notificationWindow == null || !notificationWindow.IsVisible)
        {
            POINT p = new POINT();
            if (GetCursorPos(out p))
            {
                Console.WriteLine($"X = {p.X}, Y = {p.Y}");  
                notificationWindow = new NotificationWindow(mainWindow);
                notificationWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                // 获取缩放因子
                PresentationSource source = PresentationSource.FromVisual(System.Windows.Application.Current.MainWindow);
                double dpiX = 1.0, dpiY = 1.0;
                if (source != null)
                {
                    dpiX = source.CompositionTarget.TransformToDevice.M11;
                    dpiY = source.CompositionTarget.TransformToDevice.M22;
                }

                // 转换为 WPF 单位 
                // 转换为 WPF 单位
                notificationWindow.Left = p.X / dpiX ;
                notificationWindow.Top = p.Y / dpiY - notificationWindow.Height;
                // 防止窗体跑出屏幕
                var screen = System.Windows.Forms.Screen.FromPoint(new System.Drawing.Point(p.X, p.Y));
                var workArea = screen.WorkingArea;
                if (notificationWindow.Left < workArea.Left) notificationWindow.Left = workArea.Left + 5;
                if (notificationWindow.Left + notificationWindow.Width > workArea.Right)
                    notificationWindow.Left = workArea.Right - notificationWindow.Width - 5;
                if (notificationWindow.Top < workArea.Top)
                    notificationWindow.Top = p.Y + 10;
                notificationWindow.Show();
                notificationWindow.Deactivated += (s, e) => notificationWindow.Close();
            }
            
        }
    }
    public void Show_NotificationWindow(object sender, MouseEventArgs e)
    {
        NotificationWindow_Loaded();
    }
}