using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatModel
{
    /// <summary>
    /// Пользовательское сообщение
    /// </summary>
    public class Message : IMessage
    {
        /// <summary>
        /// Имя отправителя
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Id отправителя
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Время отправки
        /// </summary>
        public DateTime SendDate { get; }

        /// <summary>
        /// Принадлежит ли сообщение текущему пользователю
        /// </summary>
        public bool IsMyMessage { get; } = false;

        /// <summary>
        /// Создать экземпляр сообщения
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="text"></param>
        public Message(string userName, string text)
        {
            UserName = userName;
            Text = text;
            SendDate = DateTime.Now;
        }

        /// <summary>
        /// Создать экземпляр сообщения
        /// </summary>
        /// <param name="servertext"></param>
        /// <param name="isMyMessage"></param>
        public Message(string servertext, bool isMyMessage = false)
        {
            var data = servertext.TrimEnd('\n').Split(' ');
            SendDate = DateTime.Parse(data[0]);
            Text = data[1];
            IsMyMessage = isMyMessage;
        }
    }
}
