using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.IO;

namespace signup
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        //防止程序多开
        static Mutex mutex = new Mutex(true, "6298E476-CB3A-455B-9630-23C0CC2DB92C");
        [STAThread]
        public static void Main()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                App app = new App();
                app.InitializeComponent();
                //窗口加载前的初始化操作
                app.App_Init();
                //加载主窗口
                MainWindow mainwindow = new MainWindow();
                mainwindow.Show();
                //启动应用程序消息循环
                app.MainWindow = mainwindow;
                app.Run();
                //当程序准备退出时释放互斥体
                mutex.ReleaseMutex();
            }
            else
            {
                MessageBox.Show("本程序已在运行！","友情提醒：");
            }
        }

        //窗口加载前的初始化操作
        private void App_Init()
        {
            //禁用Alt+F4
            EventManager.RegisterClassHandler(typeof(Window), Window.KeyDownEvent, new KeyEventHandler(OnKeyDown), true);
            //检测是否有配置文件并且创建文件
            CreateSettings();
        }

        //禁用Alt+F4实现
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.System && e.SystemKey == Key.F4)
            {
                e.Handled = true;
            }
        }

        //创建配置文件实现
        private void CreateSettings()
        {
            string filePath = @"D:\signup\settings.ini";

            if(!File.Exists(filePath))
            {
                try
                {
                    using (FileStream fs=File.Create(filePath))
                    {

                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"创建文件时出错:{ex.Message}。程序将退出。", "友情提醒：");
                    mutex.ReleaseMutex();
                    Application.Current.Shutdown();
                }
            }
        }
    }
}
