using Microsoft.Win32;
using OpenCvSharp;
using OpenCvSharp.Extensions;
namespace WarningUnlock;
using System.Threading;
public partial class Form1 : Form
{
    private VideoCapture capture;
    private Thread cameraThread;
    private bool isRunning = false;
    static bool isLocked = false;
    static Timer lockTimer;
    private static bool shouldTakePhotos = false;
    public Form1()
    {
        InitializeComponent();
        if (!DesignMode)
        {
            // 注册锁屏事件
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
        }
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
    private void button1_Click(object sender, EventArgs e)
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
            cameraThread?.Join();
            capture?.Release();
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
                Bitmap map = BitmapConverter.ToBitmap(frame);
                pictureBox1.Image = map;
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