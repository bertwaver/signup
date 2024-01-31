﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace signup
{
    public partial class MainWindow : HandyControl.Controls.GlowWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.KeyDown += OnKeyDown;
            DrawTabControl();

        }
        //禁用Alt+F4实现
        private void OnKeyDown(object sender,KeyEventArgs e)
        {
            if (e.Key == Key.System && e.SystemKey == Key.F4)
            {
                e.Handled = true;
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


        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void TabControl_SelectionChanged_1(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void TabControl_SelectionChanged_2(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
