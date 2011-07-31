﻿using System.Windows;
using Inscribe.Configuration;
using Inscribe.Configuration.KeyAssignment;
using System.Windows.Input;

namespace Mystique.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // ウィンドウ設定の読み込みと保存

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("IME:" + this.IsInputMethodEnabled);
            if (!Setting.Instance.StateProperty.WindowPosition.IsEmpty)
            {
                this.Left = Setting.Instance.StateProperty.WindowPosition.Left;
                this.Top = Setting.Instance.StateProperty.WindowPosition.Top;
                this.Width = Setting.Instance.StateProperty.WindowPosition.Width;
                this.Height = Setting.Instance.StateProperty.WindowPosition.Height;
                this.WindowState = Setting.Instance.StateProperty.WindowState;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Setting.IsInitialized)
            {
                var rect = Nightmare.WinAPI.NativeWindowControl.GetWindowPlacement(this);
                Setting.Instance.StateProperty.WindowPosition = rect;
                Setting.Instance.StateProperty.WindowState = this.WindowState;
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            KeyAssign.HandleEvent(e, AssignRegion.General);
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            KeyAssign.HandlePreviewEvent(e, AssignRegion.General);
        }
    }
}
