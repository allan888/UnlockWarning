using System.Windows;
using Point = System.Windows.Point;
namespace UnlockWarning;

public partial class NotificationWindow : Window
{
    private  MainWindow mainWindow;
    public NotificationWindow(MainWindow mainn)
    {
        InitializeComponent();
        // this.ShowActivated = false;
        this.mainWindow = mainn;
        this.MouseDown += (s, e) =>
        {
            if (!this.IsMouseOver)
                this.Close();
        };
    }

    private void Show_Click(object sender, RoutedEventArgs e)
    {
        mainWindow.Show();
        mainWindow.Topmost = true; //显示在最顶层
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        AskForPassword ask = new AskForPassword();
        ask.Show();
    }
}