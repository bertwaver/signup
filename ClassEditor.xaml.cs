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

        private void LayCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        //"直接编辑本机方案"按钮被按下事件
        private void Btn_EditSelf_Click(object sender, RoutedEventArgs e)
        {
            Editor_TabContrl.SelectedIndex = 1;
        }

        private void Btn2_Click(object sender, RoutedEventArgs e)
        {

        }
        
        //滚动条进行滚动时执行的操作
        private void Editor_ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {

        }
    }
}
