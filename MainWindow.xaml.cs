using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MessageBox = System.Windows.Forms.MessageBox;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using UserControl = System.Windows.Forms.UserControl;

namespace UnlockWarning;

public partial class MainWindow : Window
{
    private Camera _camera = new Camera();
    private Sha256StringPage _sha256StringPage = new Sha256StringPage();
    private License _license = new License();
    public MainWindow()
    {
        InitializeComponent();
        RightPanel.Loaded += (s, e) =>
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Console.WriteLine($"Grid大小: {RightPanel.ActualWidth} x {RightPanel.ActualHeight}");
            }), System.Windows.Threading.DispatcherPriority.Render);
        };
    }
    private void Show(object sender, EventArgs e)
    {
        this.Visibility = System.Windows.Visibility.Visible;
        this.ShowInTaskbar = true;
        this.Activate();
    }
    
    private void Border_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            this.DragMove();
        }
    }

    private void Minimize_OnClick(object sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }

    private void Close_OnClick(object sender, RoutedEventArgs e)
    {
        this.Hide();
    }
    
    private void Camera_OnClick(object sender, RoutedEventArgs e)
    {
        ContentControl1.Content = null;
        ContentControl1.Content = new Frame()
        {
            Content = _camera
        };
    }

    private void Sha256String_OnClick(object sender, RoutedEventArgs e)
    {
        ContentControl1.Content = null;
        ContentControl1.Content = new Frame()
        {
            Content = _sha256StringPage
        };
    }

    private void Notice_OnClick(object sender, RoutedEventArgs e)
    {
        ContentControl1.Content = null;
    }

    private void License_Click(object sender, RoutedEventArgs e)
    {
        ContentControl1.Content = null;
        ContentControl1.Content = new Frame()
        {
            Content = _license
        };
    }
}