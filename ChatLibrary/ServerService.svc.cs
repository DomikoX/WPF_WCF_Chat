using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace ChatLibrary
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ServerService : IServer
    {
        private static IClient ClientCallback => OperationContext.Current.GetCallbackChannel<IClient>();
        public Dictionary<string, IClient> AllConnectedUsers { get; set; } = new Dictionary<string, IClient>();

        public bool Register(string name)
        {
            if (AllConnectedUsers.ContainsKey(name)) return false;
            AllConnectedUsers.Add(name, ClientCallback);
            return true;
        }

        public void Unregister(string name)
        {
            CreateDisconnectMessage(name);
            AllConnectedUsers.Remove(name);
        }

        public void BroadcastMessageToAll(Message message)
        {
            var listOfUsers = AllConnectedUsers.Values.ToList();

            foreach (var user in listOfUsers)
                try
                {
                    user.ReceiveMessage(message);
                }
                catch
                {
                    var item = AllConnectedUsers.First(kvp => kvp.Value == user);
                    AllConnectedUsers.Remove(item.Key);
                    CreateDisconnectMessage(item.Key);
                }
        }

        public void SendMessageToUser(string userName, Message message)
        {
            try
            {
                AllConnectedUsers[userName].ReceiveMessage(message);
            }
            catch (Exception e)
            {
                if (e is KeyNotFoundException) return;

                CreateDisconnectMessage(userName);
            }
        }

        private void CreateDisconnectMessage(string username)
        {
            var msg = new Message
            {
                Type = MessageType.DisconnectMessage,
                SenderUserName = username
            };
            BroadcastMessageToAll(msg);
        }
    }
}