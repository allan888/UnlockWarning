using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows;
using Microsoft.Win32;

namespace UnlockWarning;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length > 0)
            {
                if (e.Args.Length == 2 && e.Args[0] == "-S")
                {
                    // 右键调用，显示 SHA256 窗口
                    var shaWindow = new sha_256(e.Args[1]);
                    shaWindow.Show();
                    return;
                }
            }

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

                // 启动主界面
                var mainWindow = new MainWindow();
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
}