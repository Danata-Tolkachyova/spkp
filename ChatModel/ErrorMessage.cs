using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatModel
{
    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public class ErrorMessage : IMessage
    {
        /// <summary>
        /// Текст сообщение
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Создать экземпляр сообщения об Ошибке
        /// </summary>
        /// <param name="text">Текст сообщения</param>
        public ErrorMessage(string text)
        {
            Text = text;
        }
    }
}
