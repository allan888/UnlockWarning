using System.Drawing;
using System.Windows;
using Microsoft.Win32;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using Window = System.Windows.Window;

namespace UnlockWarning;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class CameraShot : Window
{
    private VideoCapture capture;
    private Thread cameraThread;
    private bool isRunning = false;
    static bool isLocked = false;
    static System.Threading.Timer lockTimer;
    private static bool shouldTakePhotos = false;
    public CameraShot()
    {
        InitializeComponent();
        // 注册锁屏事件
        SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
    }
    private static void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
    {
        if (e.Reason == SessionSwitchReason.SessionLock)
        {
            isLocked = true;

            // 启动1分钟计时器
            lockTimer = new System.Threading.Timer(
                new TimerCallback(OnLockTimerElapsed),
                null,
                20000,
                Timeout.Infinite
            );
        }
        else if (e.Reason == SessionSwitchReason.SessionUnlock)
        {
            isLocked = false;
            lockTimer?.Dispose();
        }
    }
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (!isRunning)
        {
            capture = new VideoCapture(0); // 0 表示默认摄像头
            cameraThread = new Thread(new ThreadStart(CaptureCameraCallback));
            cameraThread.Start();
            isRunning = true;
        }
        else
        {
            isRunning = false;
            // 异步关闭摄像头，避免UI阻塞
            Task.Run(() =>
            {
                cameraThread?.Join(); // 在后台等待线程结束
                capture?.Release();
                capture?.Dispose();
            });
        }    
    }
    private static void OnLockTimerElapsed(object state)
    {
        if (isLocked)
        {
            shouldTakePhotos = true;
        }
    }
    private void CaptureCameraCallback()
    {
        Mat frame = new Mat();
        while (isRunning)
        {
            capture.Read(frame);
            if (!frame.Empty())
            {
                var imageSource = BitmapSourceConverter.ToBitmapSource(frame);
                imageSource.Freeze();
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    pictureBox1.Source = imageSource;
                }));
                if (shouldTakePhotos)
                {
                    string filename = $"capture_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
                    Cv2.ImWrite(filename, frame);
                    shouldTakePhotos = false;
                }
            }
        }
    }
}