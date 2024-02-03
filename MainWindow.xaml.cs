using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.IO;

namespace signup
{
    public partial class MainWindow : HandyControl.Controls.GlowWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DrawTabControl();
        }
        
        //读取配置文件实现
        private void LoadSettingsFiles(string filePath)
        {
            if(!File.Exists(filePath))
            {
                MessageBox.Show("指定的配置文件不存在，本次导入操作将被终止。", "友情提醒：");
            }
            else
            {
                string Settings_Load = File.ReadAllText(filePath);
            }

        }

        //绘制tabcontrol边框实现
        private void DrawTabControl()
        {
            Style tabControlStyle = new Style(typeof(TabControl));
            Color borderColor = (Color)ColorConverter.ConvertFromString("#3F3F45");
            tabControlStyle.Setters.Add(new Setter(TabControl.BorderBrushProperty, new SolidColorBrush(borderColor)));
            tabControlStyle.Setters.Add(new Setter(TabControl.BorderThicknessProperty, new Thickness(1)));
            main_tabcontrol.Style = tabControlStyle;
        }

        //滚动条进行滚动操作时执行的操作
        private void ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            Main_RichTextBox.ScrollToVerticalOffset(e.NewValue);
        }

        //文本框内容更新时执行的事件
        private void Main_RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //调整滚动条的样式
            UpdateScrollBarProperties();
        }

        //调整滚动条样式方法实现
        private void UpdateScrollBarProperties()
        {
            // 确保控件已初始化
            if (Main_RichTextBox == null || Main_ScrollBar == null)
            {
                return;
            }

            // 获取文本框内容高度
            double contentHeight = Main_RichTextBox.ExtentHeight;

            // 设置滚动条的最大值
            Main_ScrollBar.Maximum = contentHeight - Main_RichTextBox.ViewportHeight;

            // 更新滚动条的ViewportSize（可视区域大小）
            Main_ScrollBar.ViewportSize = Main_RichTextBox.ViewportHeight;
        }

        //标题栏按钮“签到”被点击事件
        private void Bar_1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(main_tabcontrol.SelectedIndex==-1 || main_tabcontrol.SelectedIndex==0)
            {
                return;
            }
            else
            {
                Bar_1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#007CCC"));
                Bar_2.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D2D2F"));
                main_tabcontrol.SelectedIndex = 0;
            }
        }

        //标题栏按钮“设置”被点击事件
        private void Bar_2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (main_tabcontrol.SelectedIndex == -1 || main_tabcontrol.SelectedIndex == 1)
            {
                return;
            }
            else
            {
                Bar_1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D2D2F"));
                Bar_2.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#007CCC"));
                main_tabcontrol.SelectedIndex = 1;
            }
        }

        //访问博客按钮被点击事件
        private void Btn_Open_Blog_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://ixfish.cn/");
        }

        //创建新配置文件按钮被点击事件
        private void Btn_NewSettings_Click(object sender, RoutedEventArgs e)
        {
            ClassEditor CE = new ClassEditor();
            CE.Show();
        }
    }
}
