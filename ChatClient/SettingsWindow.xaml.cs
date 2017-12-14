using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ChatClient.Properties;

namespace ChatClient
{
    /// <summary>
    ///     Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
            var colors = typeof(Colors).GetProperties().ToList();
            FontColorComboBox.ItemsSource = colors;
            FontColorComboBox.SelectedItem = colors.First(info => info.Name == Settings.Default.FontColor);
            BackColorComboBox.ItemsSource = colors;
            BackColorComboBox.SelectedItem = colors.First(info => info.Name == Settings.Default.BackColor);
            FontSizeTextBox.Text = Settings.Default.FontSize.ToString();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.FontColor = (FontColorComboBox.SelectedItem as PropertyInfo).Name;
            Settings.Default.BackColor = (BackColorComboBox.SelectedItem as PropertyInfo).Name;
            Settings.Default.FontSize = int.Parse(FontSizeTextBox.Text);
            Settings.Default.Save();
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void FontSizeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }
    }
}