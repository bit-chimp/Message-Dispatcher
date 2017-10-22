namespace btcp.MessageHandler.src
{

    public interface IMessageListener
    {
        void ReceiveMessage(Message message);
    }

}