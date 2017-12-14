using System.Windows;
using System.Windows.Input;

namespace ChatClient
{
    public partial class UserNameInput
    {
        public UserNameInput()
        {
            InitializeComponent();
            FocusManager.SetFocusedElement(this, NicknameBox);
            Closing += (sender, args) =>
            {
                if (string.IsNullOrEmpty(NicknameBox.Text.Trim())) args.Cancel = true;
            };

            NicknameBox.KeyUp += (sender, args) =>
            {
                if (args.Key == Key.Enter) Button_Click(null, null);
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NicknameBox.Text.Trim())) return;
            Close();
        }
    }
}