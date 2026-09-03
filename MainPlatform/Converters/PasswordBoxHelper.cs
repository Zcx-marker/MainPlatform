using System.Windows;
using System.Windows.Controls;

namespace MainPlatform.Converters
{
    public static class PasswordBoxHelper
    {
        public static readonly DependencyProperty BoundPasswordProperty =
            DependencyProperty.RegisterAttached(
                "BoundPassword",
                typeof(string),
                typeof(PasswordBoxHelper),
                new FrameworkPropertyMetadata(string.Empty, OnBoundPasswordChanged));

        // 触发器：XAML 中显式设为 True，false→true 必然触发回调，
        // 在回调里挂载 PasswordChanged，保证首次加载即可双向同步。
        public static readonly DependencyProperty IsMonitoringProperty =
            DependencyProperty.RegisterAttached(
                "IsMonitoring",
                typeof(bool),
                typeof(PasswordBoxHelper),
                new PropertyMetadata(false, OnIsMonitoringChanged));

        public static string GetBoundPassword(DependencyObject d)
            => (string)d.GetValue(BoundPasswordProperty);

        public static void SetBoundPassword(DependencyObject d, string value)
            => d.SetValue(BoundPasswordProperty, value);

        public static bool GetIsMonitoring(DependencyObject d)
            => (bool)d.GetValue(IsMonitoringProperty);

        public static void SetIsMonitoring(DependencyObject d, bool value)
            => d.SetValue(IsMonitoringProperty, value);

        private static void OnIsMonitoringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = d as PasswordBox;
            if (box == null) return;

            if ((bool)e.NewValue)
            {
                box.PasswordChanged -= OnPasswordChanged;
                box.PasswordChanged += OnPasswordChanged;

                // 首次挂载时从 VM 同步初始值到 PasswordBox
                var bound = GetBoundPassword(box) ?? string.Empty;
                if (box.Password != bound)
                    box.Password = bound;
            }
            else
            {
                box.PasswordChanged -= OnPasswordChanged;
            }
        }

        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = d as PasswordBox;
            if (box == null) return;

            var newValue = (string)e.NewValue ?? string.Empty;
            if (box.Password != newValue)
                box.Password = newValue;
        }

        private static void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            var box = (PasswordBox)sender;
            SetBoundPassword(box, box.Password);
        }
    }
}