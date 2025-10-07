using System.Security.Principal;
using Microsoft.Win32;

namespace WarningUnlock;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        if (args.Length > 0)
        {
            if (args.Length == 2)
            {
                if (args[0] == "-S")
                {
                    sha SHA = new sha(args[1]);
                    string result = SHA.GetFileSHA256();
                    if (result!=null)
                    {
                        MessageBox.Show(result, "SHA256结果");
                    }
                }
            }
        }
        else
        {
            System.Security.Principal.WindowsIdentity Identity = WindowsIdentity.GetCurrent();
            System.Security.Principal.WindowsPrincipal Principal = new System.Security.Principal.WindowsPrincipal(Identity);
            if (Principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
            {
                try
                {
                    bool isRegistered = false;
                    string menuName = "WarningUnlockSHA256";  // 菜单名称
                    string command = Application.ExecutablePath + " -S \"%1\""; // 执行命令，带参数 -S Path
                    string regPath = @"*\shell\" + menuName;

                    // 打开根目录
                    using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(regPath))
                    {
                        // 如果键存在则不重复注册
                        if (key != null)
                        {
                            isRegistered = true;
                        }
                    }

                    if (!isRegistered)
                    {
                        // 创建菜单项
                        using (RegistryKey shellKey = Registry.ClassesRoot.CreateSubKey(regPath))
                        {
                            shellKey.SetValue(null, "显示SHA256值"); // 显示的右键菜单名称
                            shellKey.SetValue("Icon", Application.ExecutablePath); // 图标
                        }

                        // 创建 command 子键，指定执行命令
                        using (RegistryKey commandKey = Registry.ClassesRoot.CreateSubKey(regPath + @"\command"))
                        {
                            commandKey.SetValue(null, command);
                        }
                    }
                
                }
                catch (Exception ex)
                {
                    MessageBox.Show("注册右键菜单时出错：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Application.Run(new Form1()); //正常的图形化界面
            }
            else
            {
                //创建启动对象
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.FileName = Application.ExecutablePath;
                //设置启动动作,确保以管理员身份运行
                startInfo.Verb = "runas";
                try
                {
                    System.Diagnostics.Process.Start(startInfo);
                } 
                catch
                {
                    return; 
                }
                //退出
                Application.Exit();
            }
        }
    }
}