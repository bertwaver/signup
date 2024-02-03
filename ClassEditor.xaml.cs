using System.Windows;
using System.Windows.Controls.Primitives;

namespace signup
{
    /// <summary>
    /// ClassEditor.xaml 的交互逻辑
    /// </summary>
    public partial class ClassEditor : HandyControl.Controls.GlowWindow
    {
        public ClassEditor()
        {
            InitializeComponent();
        }

        //"直接编辑本机方案"按钮被按下事件
        private void Btn_EditSelf_Click(object sender, RoutedEventArgs e)
        {
            Editor_TabContrl.SelectedIndex = 1;
        }
        
        //滚动条进行滚动时执行的操作
        private void Editor_ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            Editor_RichTextBox.ScrollToVerticalOffset(e.NewValue);
        }

        //文本框内容更新时执行的事件
        private void Editor_RichTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            //调整滚动条的样式
            UpdateScrollBarProperties();
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
    }
}
