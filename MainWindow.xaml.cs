using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MessageBox = System.Windows.Forms.MessageBox;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace UnlockWarning;

public partial class MainWindow : Window
{
    
    public MainWindow()
    {
        InitializeComponent();
        
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
}