using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatModel
{
    public class Message
    {
        public string UserName { get; set; }

        public string UserId { get; set; }

        public string Text { get; set; }

        public DateTime SendDate { get; }

        public bool IsMyMessage { get; } = false;

        public bool IsServiceMessage { get; } = false;

        public Message(string userName, string text)
        {
            UserName = userName;
            Text = text;
            SendDate = DateTime.Now;
        }

        public Message(string servertext, bool isMyMessage = false, bool isServiceMessage = false)
        {
            Text = servertext.TrimEnd('\n');

            IsMyMessage = isMyMessage;
            IsServiceMessage = isServiceMessage;
        }
    }
}
