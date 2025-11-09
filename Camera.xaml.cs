using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using MessageBox = System.Windows.MessageBox;

namespace UnlockWarning;

public partial class Camera : Page
{
    private VideoCapture capture;
    private Thread cameraThread;
    private bool isRunning = false;
    static bool isLocked = false;
    static System.Threading.Timer lockTimer;
    private static bool shouldTakePhotos = false;
    public Camera()
    {
        InitializeComponent();
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
            capture = new VideoCapture(1); // 0 表示默认摄像头
            if (!capture.IsOpened())
            {
                MessageBox.Show("无法打开摄像头！");
                return;
            }
            cameraThread = new Thread(CaptureCameraCallback);
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
        var frame = new Mat();
        while (isRunning)
        {
            capture.Read(frame);
            if (!frame.Empty())
            {
                var imageSource = BitmapSourceConverter.ToBitmapSource(frame);
                imageSource.Freeze();
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    CameraPicture.Source = imageSource;
                }));
                if (shouldTakePhotos)
                {
                    string filename = $"capture_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
                    Cv2.ImWrite(filename, frame);
                    shouldTakePhotos = false;
                }
            }
            else
            {
                Console.WriteLine("Frame empty");
            }
        }
    }
}