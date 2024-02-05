using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

namespace signup
{
    /// <summary>
    /// ClassEditor.xaml 的交互逻辑
    /// </summary>
    public partial class ClassEditor : HandyControl.Controls.GlowWindow
    {
        private bool RichTextBox_ChangedByCode = false;
        public event EventHandler LoadSettingsFromEditor;
        public ClassEditor()
        {
            InitializeComponent();
        }

        //写入文件内容实现
        private void Editor_CreateFileContent(string filePath, string Content)
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

        //读取配置文件实现
        private void Editor_LoadSettings(string filePath, bool Editor_Init)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show("指定的配置文件不存在，本次导入操作将被终止。", "友情提醒：");
                return;
            }
            else
            {
                string Settings_Load = File.ReadAllText(filePath);

                //验证配置文件合法性
                for (int cptime = 1; cptime <= 63; cptime++)
                {
                    if (Settings_Load.Contains("[" + cptime.ToString() + "]"))
                    {
                        continue;
                    }
                    else
                    {
                        MessageBox.Show("在读入配置文件时出错:无法解析指定的配置文件。", "友情提醒:");
                        if (Editor_Init == true)
                        {
                            this.Close();
                        }
                        return;
                    }
                }

                Global.Editor_Temp_Settings[0] = "Done";
                for (int cptime = 1; cptime <= 63; cptime++)
                {
                    Global.Editor_Temp_Settings[cptime] = "";
                }
                //读取配置文件算法实现
                int Settings_Load_Length = Settings_Load.Length;
                string Target_Button = "";
                string Target_Type = "None";
                for (int cptime = 0; cptime <= Settings_Load_Length - 1; cptime++)
                {
                    if (Settings_Load[cptime] == '[')
                    {
                        Target_Button = "";
                        Target_Type = "Button";
                        continue;
                    }
                    else if (Settings_Load[cptime] == ']')
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
                        else if (Target_Type == "Content")
                        {
                            try
                            {
                                Global.Editor_Temp_Settings[int.Parse(Target_Button)] += Settings_Load[cptime];
                            }
                            catch (FormatException ex)
                            {
                                MessageBox.Show($"在读入配置文件时出错:对于{Target_Button}，无法在窗口中找到该按钮。", "友情提醒:");
                                return;
                            }
                            catch(IndexOutOfRangeException ex)
                            {
                                MessageBox.Show($"在读入配置文件时出错:无法解析指定的配置文件。", "友情提醒:");
                                return;
                            }
                            continue;
                        }
                    }
                }
                Global.Editor_SettingsNow = Settings_Load;

                //更新按钮
                for (int cptime = 1; cptime <= 63; cptime++)
                {
                    Button button = this.FindName("Btn" + cptime) as Button;
                    if (button != null)
                    {
                        button.Content = Global.Editor_Temp_Settings[cptime];
                    }
                    else
                    {
                        MessageBox.Show("在查找按钮控件时发生错误！", "友情提醒:");
                        return;
                    }
                }

            }
        }

        //"直接编辑本机方案"按钮被按下事件
        private void Btn_EditSelf_Click(object sender, RoutedEventArgs e)
        {
            Editor_CreateFileContent(@"D:\signup\Editor_Settings.ini", Global.SettingsNow);
            Editor_LoadSettings(@"D:\signup\Editor_Settings.ini",true);
            try
            {
                File.Delete(@"D:\signup\Editor_Settings.ini");
            }
            catch(IOException ex)
            {
                MessageBox.Show($"在处理编辑器临时文件时出错：{ex.Message}。", "友情提醒:");
            }
            RichTextBox_ChangedByCode = true;
            Editor_RichTextBox.Document.Blocks.Clear();
            Paragraph paragraph = new Paragraph();
            paragraph.Inlines.Add(new Run(Global.SettingsNow));
            RichTextBox_ChangedByCode = true;
            Editor_RichTextBox.Document.Blocks.Add(paragraph);
            Editor_TabContrl.SelectedIndex = 1;
        }

        //"导入其它配置文件"按钮被按下事件
        private void Btn_EditOthers_Click(object sender,RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter="配置文件 (*.ini)|*.ini";
            if(openFileDialog.ShowDialog()==true)
            {
                string filePath = openFileDialog.FileName;
                Editor_LoadSettings(filePath,true);

                RichTextBox_ChangedByCode = true;
                Editor_RichTextBox.Document.Blocks.Clear();
                Paragraph paragraph = new Paragraph();
                paragraph.Inlines.Add(new Run(Global.Editor_SettingsNow));
                RichTextBox_ChangedByCode = true;
                Editor_RichTextBox.Document.Blocks.Add(paragraph);
                Editor_TabContrl.SelectedIndex = 1;
            }
            else
            {
                return;
            }
        }

        //滚动条进行滚动时执行的操作
        private void Editor_ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            Editor_RichTextBox.ScrollToVerticalOffset(e.NewValue);
        }

        //文本框内容更新时执行的事件
        private void Editor_RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //调整滚动条的样式
            UpdateScrollBarProperties();

            //检查是否是由后端代码更新内容
            if(RichTextBox_ChangedByCode==true)
            {
                RichTextBox_ChangedByCode = false;
                return;
            }

            //验证更改内容合法性
            FlowDocument document = Editor_RichTextBox.Document;
            TextRange textRange = new TextRange(document.ContentStart, document.ContentEnd);
            string text = textRange.Text;
            for(int cptime=1;cptime<=63;cptime++)
            {
                if(text.Contains("["+cptime.ToString()+"]"))
                {
                    continue;
                }
                else
                {
                    RichTextBox_ChangedByCode = true;
                    Editor_RichTextBox.Document.Blocks.Clear();
                    Paragraph paragraph = new Paragraph();
                    paragraph.Inlines.Add(new Run(Global.Editor_SettingsNow));
                    RichTextBox_ChangedByCode = true;
                    Editor_RichTextBox.Document.Blocks.Add(paragraph);
                    MessageBox.Show("更改配置文件时出错：更改内容不合法。", "友情提醒:");
                    return;
                }
            }

            //更新按钮和临时配置文件
            Editor_CreateFileContent(@"D:\signup\Temp_Editor_Settings.ini", text);
            Editor_LoadSettings(@"D:\signup\Temp_Editor_Settings.ini",false);
            try
            {
                File.Delete(@"D:\signup\Temp_Editor_Settings.ini");
            }
            catch (IOException ex)
            {
                MessageBox.Show($"在处理编辑器临时文件时出错：{ex.Message}。", "友情提醒:");
            }
            Global.Editor_SettingsNow = text;
        }

        private void UpdateScrollBarProperties()
        {
            // 确保控件已初始化
            if (Editor_RichTextBox == null || Editor_ScrollBar == null)
            {
                return;
            }

            // 获取文本框内容高度
            double contentHeight = Editor_RichTextBox.ExtentHeight;

            // 设置滚动条的最大值
            Editor_ScrollBar.Maximum = contentHeight - Editor_RichTextBox.ViewportHeight;

            // 更新滚动条的ViewportSize（可视区域大小）
            Editor_ScrollBar.ViewportSize = Editor_RichTextBox.ViewportHeight;
        }

        //签到按钮被点击事件
        private void Editor_Btn_Signup_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.Content = Editor_TextBox.Text;

            string SelectedButton = "";
            for(int cptime=4;cptime<=button.Name.Length ;cptime++)
            {
                SelectedButton +=button.Name[cptime - 1];
            }

            string input = Global.Editor_SettingsNow;
            int startpoint = input.IndexOf("["+SelectedButton+"]");
            int endpoint = input.IndexOf("[" + (int.Parse(SelectedButton)+1).ToString() + "]");

            if((startpoint!=-1 && endpoint!=-1)||(SelectedButton=="63"))
            {
                if(SelectedButton=="63")
                {
                    if(startpoint!=-1)
                    {
                        input=input.Remove(startpoint+4);
                        input += button.Content;
                        Global.Editor_SettingsNow = input;
                        RichTextBox_ChangedByCode = true;
                        Editor_RichTextBox.Document.Blocks.Clear();
                        Paragraph paragraph = new Paragraph();
                        paragraph.Inlines.Add(new Run(input));
                        RichTextBox_ChangedByCode = true;
                        Editor_RichTextBox.Document.Blocks.Add(paragraph);
                        Editor_TabContrl.SelectedIndex = 1;
                        return;
                    }
                    else
                    {
                        MessageBox.Show($"在配置文件中查找[{SelectedButton}]时出错，请重新载入配置文件！", "友情提醒:");
                        return;
                    }
                }
                else
                {
                    string beforestr = input.Remove(startpoint + 2 + SelectedButton.Length);
                    string afterstr = input.Substring(endpoint);
                    string sum=beforestr+button.Content+afterstr;
                    Global.Editor_SettingsNow = sum;
                    RichTextBox_ChangedByCode = true;
                    Editor_RichTextBox.Document.Blocks.Clear();
                    Paragraph paragraph = new Paragraph();
                    paragraph.Inlines.Add(new Run(sum));
                    RichTextBox_ChangedByCode = true;
                    Editor_RichTextBox.Document.Blocks.Add(paragraph);
                    return;
                }
            }
            else
            {
                MessageBox.Show($"在配置文件中查找[{SelectedButton}]时出错，请重新载入配置文件！","友情提醒:");
                return;
            }
        }

        //保存到本机使用按钮被点击事件
        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            Editor_CreateFileContent(@"D:\signup\settings.ini", Global.Editor_SettingsNow);
            LoadSettingsFromEditor.Invoke(this, EventArgs.Empty);
            MessageBox.Show("保存成功！", "友情提醒:");
        }

        //导出到本地使用按钮被点击事件
        private void Btn_ToLocal_Click(object sender,RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "选择要保存的路径";
            saveFileDialog.Filter = "配置文件(*.ini)|*.ini";
            saveFileDialog.DefaultExt = "ini";
            saveFileDialog.AddExtension = true;

            if(saveFileDialog.ShowDialog()==true)
            {
                string Filepath = saveFileDialog.FileName;
                Editor_CreateFileContent(Filepath, Global.Editor_SettingsNow);
                MessageBox.Show("保存成功！", "友情提醒:");
            }
        }
    }
}
