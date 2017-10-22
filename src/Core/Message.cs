
using System;
using System.Collections.Generic;
using UnityEngine;

namespace btcp.MessageHandler.src
{
    public class Message
    {


        private Dictionary<string, object> m_valueDict;

        private object m_messageID;
        private float m_messageDelay;
        public object MessageID { get { return m_messageID; } private set { m_messageID = value; } }
        public float MessageTime { get { return m_messageDelay; } set { m_messageDelay = value; } }


        public Message(object id)
        {
            m_messageID = id;

            m_valueDict = new Dictionary<string, object>();
        }


        public void SetArgFloat(string id, float obj)
        {
            m_valueDict[id] = obj;
        }

        public void SetArgInt(string id, int obj)
        {
            m_valueDict[id] = obj;
        }

        public void SetArgString(string id, string obj)
        {
            m_valueDict[id] = obj;
        }

        public float GetArgFloat(string id)
        {
            if (!(m_valueDict[id] is float))
            {
                OnWrongCastType(id);
            }

            return (float)m_valueDict[id];
        }

        public string GetArgString(string id)
        {
            if (!(m_valueDict[id] is string))
            {
                OnWrongCastType(id);
            }

            return (string)m_valueDict[id];
        }

        public int GetArgInt(string id)
        {
            if (!(m_valueDict[id] is int))
            {
                OnWrongCastType(id);
            }
            return (int)m_valueDict[id];
        }

        private void OnWrongCastType(string id)
        {
            Debug.LogError("[Message - cast type] " + id + " is of type " + m_valueDict[id].GetType().Name + "!");
        }

    }
}