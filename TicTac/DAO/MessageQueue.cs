using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TicTac.DAO;

namespace TicTac.DAO
{
    class Message
    {
        public string Action { get; set; }
        public string Data { get; set; }
    }

    class MessageQueue
    {
        public void StoreMessage(Message msg)
        {
            throw new NotImplementedException();
        }

        public Message LoadNextMessage()
        {
            throw new NotImplementedException();
        }

        // This method checks if messages were stored but not sent while offline
        public void ProcessWaitingMessages()
        {
            throw new NotImplementedException();

            Message msg = LoadNextMessage();
            while (msg != null)
            {
                ProcessMessage(msg);
                msg = LoadNextMessage();
            }
        }


        public void ProcessMessage(Message msg)
        {
            // get msg.action and msg.data
            throw new NotImplementedException();
        }
    }
}
