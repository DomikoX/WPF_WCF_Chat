using System;
using System.ServiceModel;
using ChatLibrary;

namespace ChatClient
{
    public class Client : IClient
    {
        public string Name { get; set; }
        public string PassPhrase { get; set; }
        private readonly IServer _channel;

        public delegate void MessageIcomeHandler(Message msg);

        public event MessageIcomeHandler MessageIncomeEvent;
        public event MessageIcomeHandler ImageMessageIncomeEvent;

        public delegate void UserStatusHandler(string username);

        public event UserStatusHandler NewUserJoinedEvent;
        public event UserStatusHandler UserDisconnectEvent;
        public event UserStatusHandler UserIsOnlineEvent;


        public Client()
        {
            DuplexChannelFactory<IServer> channelFactory =
                new DuplexChannelFactory<IServer>(this, "MyChatNetTcpEndpoint");
            _channel = channelFactory.CreateChannel();
        }

        public bool Register(string name)
        {
            Name = name;
            return _channel.Register(name);
        }


        public void Unregister()
        {
            _channel.Unregister(Name);
        }

        public void SendConnectionMessage()
        {
            var msg = new Message() {SenderUserName = Name, Type = MessageType.ConnectMessage};
            _channel.BroadcastMessageToAll(msg);
        }

        public void SendMessage(string message, string userName, byte[] image = null)
        {
            var msg = new Message()
            {
                Type = image == null ? MessageType.TextMessage : MessageType.ImageMessage,
                SenderUserName = Name,
                Content = message,
                ReceiverUserName = userName,
                ImageData = image,
            };

            if (userName == null) _channel.BroadcastMessageToAll(msg);
            else
            {
                _channel.SendMessageToUser(userName, msg);

                if (image == null)
                    MessageIncomeEvent?.Invoke(msg);
                else
                    ImageMessageIncomeEvent?.Invoke(msg);
            }
        }

        public void ReceiveMessage(Message message)
        {
            switch (message.Type)
            {
                case MessageType.TextMessage:
                    MessageIncomeEvent?.Invoke(message);
                    break;
                case MessageType.ImageMessage:
                    ImageMessageIncomeEvent?.Invoke(message);
                    break;
                case MessageType.ConnectMessage:
                    NewUserJoinedEvent?.Invoke(message.SenderUserName);
                    if (message.SenderUserName != Name)
                        _channel.SendMessageToUser(message.SenderUserName,
                            new Message() {SenderUserName = Name, Type = MessageType.IsOnlineMessage});
                    break;
                case MessageType.DisconnectMessage:
                    UserDisconnectEvent?.Invoke(message.SenderUserName);
                    break;
                case MessageType.IsOnlineMessage:
                    UserIsOnlineEvent?.Invoke(message.SenderUserName);
                    break;
            }
        }
    }
}