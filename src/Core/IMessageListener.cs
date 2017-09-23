namespace Assets.Scripts.Utilities.MessageHandler
{

    public interface IMessageListener
    {
        int ListenerPriority { get; }

        void ReceiveMessage(Message message);
    }

}