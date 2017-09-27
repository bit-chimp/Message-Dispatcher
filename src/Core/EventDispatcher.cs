using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Utilities.MessageHandler
{
    public class MessageDispatcher
    {

        private static MessageDispatcher m_instance;
        public static MessageDispatcher Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new MessageDispatcher();
                }

                return m_instance;
            }
        }

        private List<Message> m_messageList;
        private List<Message> m_messageListQueue;

        private List<MessageBind> m_messageBinds;

        public MessageDispatcher()
        {
            m_messageList = new List<Message>();
            m_messageListQueue = new List<Message>();
            m_messageBinds = new List<MessageBind>();
        }

        public void BindListener(IMessageListener listener, int messageID)
        {
            MessageBind bind = GetOrCreateBind(listener);
            Debug.Assert(bind.MessageTypes.Contains(messageID) == false, "Listener " + listener.GetType().Name + " already has bind with message type " + messageID);
            bind.MessageTypes.Add(messageID);
        }

        public void UnBindListenerFromMessage(IMessageListener listener, int messageID)
        {
            Debug.Assert(HasBind(listener), "Listener does not have any binds associated with it");
            MessageBind bind = GetOrCreateBind(listener);

            Debug.Assert(bind.MessageTypes.Contains(messageID), "Listener " + listener.GetType().Name + " is not binded to " + messageID);
            bind.MessageTypes.Remove(messageID);
        }

        public void UnBindListener(IMessageListener listener)
        {
            Debug.Assert(HasBind(listener), "Listener does not have any binds associated with it");
            MessageBind bind = GetOrCreateBind(listener);
            m_messageBinds.Remove(bind);
        }



        private bool HasBind(IMessageListener listener)
        {
            foreach (MessageBind bind in m_messageBinds)
            {
                if (bind.Listener == listener)
                {
                    return true;
                }
            }

            return false;
        }


        private MessageBind GetOrCreateBind(IMessageListener listener)
        {
            foreach (MessageBind bind in m_messageBinds)
            {
                if (bind.Listener == listener)
                {
                    return bind;
                }
            }

            MessageBind newBind = new MessageBind(listener);
            m_messageBinds.Add(newBind);
            return newBind;
        }

        public void Update()
        {
            SendMessages();
            QueueWaitingMessages();
        }

        private void SendMessages()
        {
            List<Message> messages = GetMessagesToSend();

            while (messages.Count > 0)
            {
                Message msg = messages[0];

                SendMessage(msg);
                messages.Remove(msg);
                m_messageList.Remove(msg);
            }
        }

        private List<Message> GetMessagesToSend()
        {
            List<Message> messages = new List<Message>();

            for (int i = 0; i < m_messageList.Count; i++)
            {
                if (m_messageList[i].MessageTime > Time.time)
                {
                    break;
                }

                messages.Add(m_messageList[i]);
            }


            return messages;
        }

        private void QueueWaitingMessages()
        {
            while (m_messageListQueue.Count > 0)
            {
                m_messageList.Add(m_messageListQueue[0]);
                m_messageListQueue.RemoveAt(0);
            }

            OnMessagesUpdated();
        }

        private void OnMessagesUpdated()
        {
            SortMessagesByTime();
        }

        public void QueueMessage(Message msg, float delayInSeconds = 0)
        {
            if (delayInSeconds > 0)
            {
                msg.MessageTime = Time.time + delayInSeconds;
                m_messageListQueue.Add(msg);
            }
            else
            {
                SendMessage(msg);
            }
        }



        //TODO : Optimize search for binds with message type?
        private void SendMessage(Message msg)
        {
            foreach (MessageBind bind in m_messageBinds)
            {
                if (bind.MessageTypes.Contains(msg.MessageID))
                {
                    bind.Listener.ReceiveMessage(msg);
                }
            }

        }

        private void SortMessagesByTime()
        {
            m_messageList.Sort(SortByTime);
        }


        private int SortByTime(Message a, Message b)
        {
            if (a.MessageTime < b.MessageTime)
            {
                return -1;
            }
            else
            if (a.MessageTime > b.MessageTime)
            {
                return 1;
            }

            return 0;
        }



        private class MessageBind
        {

            private IMessageListener m_listener;
            private List<int> m_messageTypes;

            public IMessageListener Listener { get { return m_listener; } private set { m_listener = value; } }
            public List<int> MessageTypes { get { return m_messageTypes; } set { m_messageTypes = value; } }

            public MessageBind(IMessageListener listener)
            {
                Listener = listener;

                m_messageTypes = new List<int>();
            }
        }

    }
}