namespace ChatModel
{
    /// <summary>
    /// Общий интерфейс сообщений
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string Text { get; set; }
    }
}