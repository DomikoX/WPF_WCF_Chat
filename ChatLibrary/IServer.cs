using System.Runtime.Serialization;
using System.ServiceModel;

namespace ChatLibrary
{
    [ServiceContract(CallbackContract = typeof(IClient), SessionMode = SessionMode.Required)]
    public interface IServer
    {
        [OperationContract]
        bool Register(string name);

        [OperationContract(IsOneWay = true)]
        void Unregister(string name);

        [OperationContract(IsOneWay = true)]
        void BroadcastMessageToAll(Message message);

        [OperationContract(IsOneWay = true)]
        void SendMessageToUser(string userId, Message message);
    }

    [DataContract]
    public class Message
    {
        [DataMember]
        public MessageType Type { get; set; }

        [DataMember]
        public string SenderUserName { get; set; }

        [DataMember]
        public string ReceiverUserName { get; set; }

        [DataMember]
        public string Content { get; set; }

        [DataMember]
        public byte[] ImageData { get; set; }
    }

    [DataContract]
    public enum MessageType
    {
        [EnumMember] TextMessage,
        [EnumMember] ImageMessage,
        [EnumMember] ConnectMessage,
        [EnumMember] IsOnlineMessage,
        [EnumMember] DisconnectMessage
    }
}