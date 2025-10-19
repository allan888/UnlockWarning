using OpenCvSharp;

namespace WarningUnlock;

public partial class Form1 : Form
{
    private VideoCapture capture;
    private Thread cameraThread;
    private bool isRunning = false;
    public Form1()
    {
        InitializeComponent();
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
    private void CaptureCameraCallback()
    {
        Mat frame = new Mat();
        while (isRunning)
        {
            capture.Read(frame);
        }
    }
}