using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Documents;
using System.ComponentModel;
using System.IO;
using Microsoft.Win32;

namespace signup
{
    public partial class MainWindow : HandyControl.Controls.GlowWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DrawTabControl();
            this.Closing += MainWindow_Closing;
            LoadSettingsFiles(@"D:\signup\settings.ini");

            //向签到信息添加当前日期
            string now = DateTime.Now.ToString("yyyy年MM月dd日。");
            Global.Saves += "\n[快速签到]现在是"+now;
            UpdateRichTextBox(Global.Saves);

            //设置定时器，在指定时间显示窗口
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Tick += SetWindowsVisible;
            timer.Start();
        }

        //从对象编辑器加载配置
        private void MainWindow_LoadSettingsFromEditor(object sender,EventArgs e)
        {
            LoadSettingsFiles(@"D:\signup\settings.ini");
        }

        //更新文本框内容实现
        private void UpdateRichTextBox(string content)
        {
            Main_RichTextBox.Document.Blocks.Clear();

            //解析content内容算法实现
            string Temp_Saves = "";
            bool isnewlines = false;
            int content_length = content.Length;
            bool oneline = true;
            for(int cptime=0;cptime<=content_length-1 ;cptime++)
            {
                if(content[cptime]=='\\')
                {
                    isnewlines = true;
                    continue;
                }
                else if(content[cptime]=='n')
                {
                    if(isnewlines==true)
                    {
                        Paragraph paragraph = new Paragraph();
                        paragraph.Inlines.Add(new Run(Temp_Saves));
                        Main_RichTextBox.Document.Blocks.Add(paragraph);

                        oneline = false;
                        Temp_Saves = "";
                        isnewlines = false;
                        continue;
                    }
                    else if(isnewlines==false)
                    {
                        Temp_Saves += content[cptime];
                        continue;
                    }
                }
                else
                {
                    Temp_Saves += content[cptime];
                    continue;
                }
            }
            if (oneline==true|| (Temp_Saves!="" && oneline==false))
            {
                Paragraph paragraph = new Paragraph();
                paragraph.Inlines.Add(new Run(Temp_Saves));
                Main_RichTextBox.Document.Blocks.Add(paragraph);
                Temp_Saves = "";
            }
            return;
        }

        //设置窗口可视状态实现
        private void SetWindowsVisible(object sender,EventArgs e)
        {
            DateTime now = DateTime.Now;
            if(now.Hour>=5 && now.Hour<6)
            {
                if(WindowState==WindowState.Minimized)
                {
                    return;
                }
                else
                {
                    this.Show();
                }
            }
        }

        //程序保持后台运行实现
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = WindowState.Normal;
            this.Hide();
        }

        //读取配置文件实现
        private void LoadSettingsFiles(string filePath)
        {
            if(!File.Exists(filePath))
            {
                MessageBox.Show("指定的配置文件不存在，本次导入操作将被终止。", "友情提醒：");
                return;
            }
            else
            {
                string Settings_Load = File.ReadAllText(filePath);
                Global.Temp_Settings[0] = "Done";
                for(int cptime=1; cptime<=63;cptime++)
                {
                    Global.Temp_Settings[cptime] = "";
                }
                //读取配置文件算法实现
                int Settings_Load_Length = Settings_Load.Length;
                string Target_Button = "";
                string Target_Type = "None";
                for(int cptime=0;cptime<=Settings_Load_Length-1;cptime++)
                {
                    if(Settings_Load[cptime]=='[')
                    {
                        Target_Button = "";
                        Target_Type = "Button";
                        continue;
                    }
                    else if(Settings_Load[cptime]==']')
                    {
                        Target_Type = "Content";
                        continue;
                    }
                    else
                    {
                        if (Target_Type == "Button")
                        {
                            Target_Button += Settings_Load[cptime];
                            continue;
                        }
                        else if(Target_Type=="Content")
                        {
                            try
                            {
                                Global.Temp_Settings[int.Parse(Target_Button)] += Settings_Load[cptime];
                            }
                            catch(FormatException ex)
                            {
                                MessageBox.Show($"在读入配置文件时出错:对于{Target_Button}，无法在窗口中找到该按钮。", "友情提醒:");
                                return;
                            }
                            catch (IndexOutOfRangeException ex)
                            {
                                MessageBox.Show($"在读入配置文件时出错:无法解析指定的配置文件。", "友情提醒:");
                                return;
                            }
                            continue;
                        }
                    }
                }
                Global.SettingsNow = Settings_Load;

                //更新按钮
                for(int cptime=1;cptime<=63;cptime++)
                {
                    Button button = this.FindName("Btn" + cptime) as Button;
                    if(button!=null)
                    {
                        button.Content = Global.Temp_Settings[cptime];
                    }
                    else
                    {
                        MessageBox.Show("在查找按钮控件时发生错误！","友情提醒:");
                        return;
                    }
                }

                //更新默认配置文件
                CreateFileContent(@"D:\signup\settings.ini", Global.SettingsNow);
            }
        }

        //写入文件内容实现
        private void CreateFileContent(string filePath,string Content)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.Write(Content);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"写入文件时出错:{ex.Message}。", "友情提醒：");
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
            CE.LoadSettingsFromEditor += MainWindow_LoadSettingsFromEditor;
            CE.Show();
        }

        //打开已有配置文件按钮被点击事件
        private void Btn_LoadSettingsFromFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter = "配置文件 (*.ini)|*.ini";
            if(openFileDialog.ShowDialog()==true)
            {
                string filePath = openFileDialog.FileName;
                LoadSettingsFiles(filePath);
                InformationBar.Text = "已读入配置文件。";
            }
            else
            {
                InformationBar.Text = "无效的文件。";
            }
        }

        //查看历史记录按钮被点击事件
        private void Btn_History_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"D:\signup\");
        }

        //立即保存按钮被点击事件
        private void Btn_SaveNow_Click(object sender, RoutedEventArgs e)
        {
            string now = DateTime.Now.ToString("yyyy.M.d");
            CreateFileContent($@"D:\signup\{now}.txt", Global.Saves);
            InformationBar.Text = "保存成功。";
        }

        //签到按钮被点击事件
        private void Btn_Signup_Click(object sender,RoutedEventArgs e)
        {
            Button ClickedButton = sender as Button;
            if(ClickedButton!=null)
            {
                string name = (string)ClickedButton.Content;
                if(Global.Saves.Contains(name+"签到成功。"))
                {
                    InformationBar.Text = "你已经签到过了！";
                    return;
                }
                else
                {
                    DateTime now = DateTime.Now;
                    string now_strings = now.ToString("yyyy.M.d");
                    InformationBar.Text = name + "签到成功！";
                    Global.member_count++;
                    Global.Saves += "\n"+ "[" + now.Hour.ToString() + ":" + now.Minute.ToString() + "]" + Global.member_count.ToString() + ":"+ name + "签到成功。";
                    UpdateRichTextBox(Global.Saves);
                    CreateFileContent($@"D:\signup\{now_strings}.txt",Global.Saves);
                    return;
                }
            }
        }
    }
}