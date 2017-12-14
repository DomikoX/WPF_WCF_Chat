using System.ServiceModel;

namespace ChatLibrary
{
    [ServiceContract]
    public interface IClient
    {
        [OperationContract(IsOneWay = true)]
        void ReceiveMessage(Message message);
    }
}