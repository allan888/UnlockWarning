using System.ServiceProcess;

namespace UnlockWarning;

public class Utility
{
    public static void PauseService(string serviceName)
    {
        try
        {
            using (ServiceController sc = new ServiceController(serviceName))
            {
                // 检查服务是否支持暂停
                if (!sc.CanPauseAndContinue)
                {
                    return;
                }

                // 若正在运行，则尝试暂停
                if (sc.Status == ServiceControllerStatus.Running)
                {
                    sc.Pause();
                    // 等待状态更新
                    sc.WaitForStatus(ServiceControllerStatus.Paused, TimeSpan.FromSeconds(10));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"操作失败：{ex.Message}");
        }
    }
    public static void DetectAndStartService(string serviceName)
    {
        try
        {
            using (ServiceController sc = new ServiceController(serviceName))
            {
                // 若正在运行，则尝试暂停
                if (sc.Status == ServiceControllerStatus.Stopped)
                {
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                }
                if (sc.Status == ServiceControllerStatus.Paused)
                {
                    sc.Continue();
                    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"操作失败：{ex.Message}");
        }
    }
}