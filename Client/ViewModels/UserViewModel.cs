using ChatModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace Client.ViewModels
{
    internal class UserViewModel : ViewModelBase
    {
        private string _IpAddress = "127.0.0.1";
        public string IpAddress
        {
            get { return _IpAddress; }
            set { _IpAddress = value; OnPropertyChanged(); }
        }


        private int _Port = 8888;
        public int Port
        {
            get { return _Port; }
            set { _Port = value; OnPropertyChanged(); }
        }


        //private int _BufferSize = 1024;
        //public int BufferSize
        //{
        //    get { return _BufferSize; }
        //    set { _BufferSize = value; OnPropertyChanged(); }
        //}


        private User _User;
        public User User
        {
            get { return _User; }
            set { _User = value; OnPropertyChanged(); }
        }


        private Chat _Chat;
        public Chat Chat
        {
            get { return _Chat; }
            set { _Chat = value; OnPropertyChanged(); }
        }


        private TcpClient _TcpClient;
        public TcpClient TcpClient
        {
            get { return _TcpClient; }
            set { _TcpClient = value; OnPropertyChanged(); }
        }


        private bool _CanSendMessages = false;
        public bool CanSendMessages
        {
            get { return _CanSendMessages; }
            set { _CanSendMessages = value; OnPropertyChanged(); }
        }


        private string _Message;
        public string Message
        {
            get { return _Message; }
            set { _Message = value; OnPropertyChanged(); }
        }


        public UserViewModel()
        {
            Chat = new Chat();
            ConnectToServer();
        }


        public void ConnectToServer()
        {
            try
            {
                Chat.Messages.Add(new Message($"Выполняется подключение к серверу {IpAddress}:{Port}", false, true));

                // Создание TCP-клиента и подключение к серверу
                TcpClient = new TcpClient();
                TcpClient.Connect(IPAddress.Parse(IpAddress), Port);

                Chat.Messages.Add(new Message("Подключен к серверу!", false, true));

                // Запуск потока для чтения сообщений от сервера
                Thread receiveThread = new Thread(() => ReceiveMessages(TcpClient, Chat));
                receiveThread.Start();

                // Основной цикл для отправки сообщений
                CanSendMessages = true;
            }
            catch (Exception ex)
            {
                Chat.Messages.Add(new Message($"Error: {ex.Message}", false, true));
            }
        }
        

        public void OpenSettingsWindow()
        {
            new SettingsWindow() { DataContext = this }.Show();
        }


        public void ApplySettings()
        {
            ConnectToServer();
        }


        //[RelayCommand(CanExecute = nameof(CanSendMessages))]
        public void SendMessage()
        {
            Console.WriteLine($"SENDING MESSAGE: {Message}");

            if (string.IsNullOrEmpty(Message)) return;

            try
            {
                byte[] data = Encoding.UTF8.GetBytes(Message);
                NetworkStream stream = TcpClient.GetStream();
                stream.Write(data, 0, data.Length);

                Chat.Messages.Add(new Message(Message));

                Message = "";
            }
            catch (Exception ex)
            {
                Chat.Messages.Add(new Message($"Error: {ex.Message}", false, true));
            }
        }


        private static void ReceiveMessages(TcpClient client, Chat chat)
        {
            var bufferSize = 1024;

            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[bufferSize];

                while (true)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        Console.WriteLine("Server disconnected");
                        Environment.Exit(0);
                    }

                    chat.Messages.Add(new Message(Encoding.UTF8.GetString(buffer, 0, bytesRead)));
                    
                }
            }
            catch (Exception)
            {
                // Игнорируем ошибки при закрытии соединения
            }
        }
    }
}
