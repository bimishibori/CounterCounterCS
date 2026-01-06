// CounterCounter/UI/Dialogs/ModernMessageBox.xaml.cs
using System.Windows;
using WpfWindow = System.Windows.Window;
using WpfMessageBoxButton = System.Windows.MessageBoxButton;
using WpfMessageBoxResult = System.Windows.MessageBoxResult;
using WpfMessageBoxImage = System.Windows.MessageBoxImage;
using WpfColor = System.Windows.Media.Color;
using WpfColorConverter = System.Windows.Media.ColorConverter;
using WpfSolidColorBrush = System.Windows.Media.SolidColorBrush;

namespace CounterCounter.UI.Dialogs
{
    public partial class ModernMessageBox : WpfWindow
    {
        public WpfMessageBoxResult Result { get; private set; }

        private ModernMessageBox()
        {
            InitializeComponent();
            Result = WpfMessageBoxResult.None;
        }

        public static WpfMessageBoxResult Show(
            string message,
            string title = "メッセージ",
            WpfMessageBoxButton button = WpfMessageBoxButton.OK,
            WpfMessageBoxImage icon = WpfMessageBoxImage.None)
        {
            var dialog = new ModernMessageBox
            {
                Title = title
            };

            dialog.TitleText.Text = title;
            dialog.MessageText.Text = message;

            dialog.SetIcon(icon);
            dialog.SetButtons(button);

            dialog.ShowDialog();
            return dialog.Result;
        }

        private void SetIcon(WpfMessageBoxImage icon)
        {
            switch (icon)
            {
                case WpfMessageBoxImage.Information:
                    IconText.Text = "ℹ️";
                    IconText.Foreground = new WpfSolidColorBrush(WpfColor.FromRgb(100, 149, 237));
                    break;
                case WpfMessageBoxImage.Warning:
                    IconText.Text = "⚠️";
                    IconText.Foreground = new WpfSolidColorBrush(WpfColor.FromRgb(255, 165, 0));
                    break;
                case WpfMessageBoxImage.Error:
                    IconText.Text = "❌";
                    IconText.Foreground = new WpfSolidColorBrush(WpfColor.FromRgb(255, 71, 87));
                    break;
                case WpfMessageBoxImage.Question:
                    IconText.Text = "❓";
                    IconText.Foreground = new WpfSolidColorBrush(WpfColor.FromRgb(144, 238, 144));
                    break;
                default:
                    IconText.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void SetButtons(WpfMessageBoxButton button)
        {
            switch (button)
            {
                case WpfMessageBoxButton.OK:
                    OkButton.Visibility = Visibility.Visible;
                    break;
                case WpfMessageBoxButton.OKCancel:
                    OkButton.Visibility = Visibility.Visible;
                    CancelButton.Visibility = Visibility.Visible;
                    break;
                case WpfMessageBoxButton.YesNo:
                    YesButton.Visibility = Visibility.Visible;
                    NoButton.Visibility = Visibility.Visible;
                    break;
                case WpfMessageBoxButton.YesNoCancel:
                    YesButton.Visibility = Visibility.Visible;
                    NoButton.Visibility = Visibility.Visible;
                    CancelButton.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Result = WpfMessageBoxResult.OK;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Result = WpfMessageBoxResult.Cancel;
            DialogResult = false;
            Close();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            Result = WpfMessageBoxResult.Yes;
            DialogResult = true;
            Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Result = WpfMessageBoxResult.No;
            DialogResult = false;
            Close();
        }
    }
}