using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.ServiceModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using ChatLibrary;
using Microsoft.Win32;

namespace ChatClient
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Client = new Client();
            var registered = false;
            do
            {
                var input = new UserNameInput();
                input.ShowDialog();
                var name = input.NicknameBox.Text.Trim();

                try
                {
                    registered = Client.Register(name);
                }
                catch (EndpointNotFoundException)
                {
                    MessageBox.Show("Cannot connect to server, are you sure it is running?",
                        "No server found",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }

                if (!registered)
                    MessageBox.Show("There is already a user with selected username, please choose another one!",
                        "Username already exist",
                        MessageBoxButton.OK, MessageBoxImage.Information);
            } while (!registered);

            Client.SendConnectionMessage();
            Client.MessageIncomeEvent += ClientOnMessageIncomeEvent;
            Client.NewUserJoinedEvent += ClientOnNewUserJoinedEvent;
            Client.UserDisconnectEvent += ClientOnUserDisconnectEvent;
            Client.UserIsOnlineEvent += ClientOnUserIsOnlineEvent;
            Client.ImageMessageIncomeEvent += ClientOnImageMessageIncomeEvent;

            ChatBox.IsReadOnly = true;
            ReceiverComboBox.Items.Add("All");
            ReceiverComboBox.SelectedIndex = 0;

            Title += $": {Client.Name}";

            FocusManager.SetFocusedElement(this, InputBox);
            ChatBox.Document = new FlowDocument();
        }

        public Client Client { get; }
        public ObservableCollection<string> ConnectedUsers { get; set; } = new ObservableCollection<string>();

        private void ClientOnUserIsOnlineEvent(string username)
        {
            ModifyConntedusers(username);
            ChatBox.AppendText(username, "is online");
        }

        private void ClientOnUserDisconnectEvent(string username)
        {
            ModifyConntedusers(username, false);
            ChatBox.AppendText(username, "disconnected");
        }


        private void ClientOnNewUserJoinedEvent(string username)
        {
            ModifyConntedusers(username);
            ChatBox.AppendText(username, "connected");
        }

        private void ModifyConntedusers(string username, bool add = true)
        {
            if (add)
            {
                if (username != Client.Name)
                {
                    ConnectedUsers.Add(username);
                    ReceiverComboBox.Items.Add(username);
                    return;
                }

                ConnectedUsers.Add($"{username} (You)");
            }
            else
            {
                ConnectedUsers.Remove(username);
                ReceiverComboBox.Items.Remove(username);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            try
            {
                Client.Unregister();
            }
            catch
            {
                // ignored
            }
        }

        private void ClientOnImageMessageIncomeEvent(Message msg)
        {
            var usr = FormatUsername(msg, out bool privateMsg);
            ChatBox.AppendImage(usr, msg.ImageData, privateMsg);
        }

        private static string FormatUsername(Message msg, out bool privateMessage)
        {
            privateMessage = false;
            if (msg.ReceiverUserName == null) return msg.SenderUserName;

            privateMessage = true;
            return $"{msg.SenderUserName} > {msg.ReceiverUserName}";
        }

        private void ClientOnMessageIncomeEvent(Message msg)
        {
            var usr = FormatUsername(msg, out bool privateMsg);
            ChatBox.AppendText(usr, msg.Content, privateMsg);
        }

        private void SendMessage()
        {
            var receiver = ReceiverComboBox.SelectedItem as string == "All"
                ? null
                : ReceiverComboBox.SelectedItem as string;
            if (string.IsNullOrEmpty(InputBox.Text)) return;
            Client.SendMessage(InputBox.Text, receiver);
            InputBox.Text = string.Empty;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void InputBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) SendMessage();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            };

            var result = fileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var fi = new FileInfo(fileDialog.FileName);
                if (!File.Exists(fi.FullName)) return;


                var fs = File.ReadAllBytes(fi.FullName);
                if (fi.Length > int.MaxValue)
                {
                    MessageBox.Show("Too big file", "The selected picture is too big", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                var receiver = ReceiverComboBox.SelectedItem as string == "All"
                    ? null
                    : ReceiverComboBox.SelectedItem as string;
                Client.SendMessage(null, receiver, fs);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var settings = new SettingsWindow {Owner = this};
            settings.ShowDialog();
        }
    }
}