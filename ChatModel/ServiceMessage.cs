using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatModel
{
    /// <summary>
    /// Системное сообщение
    /// </summary>
    public class ServiceMessage : IMessage
    {
        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Создать экземпляр сообщения
        /// </summary>
        /// <param name="text">Текст сообщения</param>
        public ServiceMessage(string text)
        {
            Text = text;
        }
    }
}
