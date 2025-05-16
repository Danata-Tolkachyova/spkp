using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class ChatClient
{
    private const string SERVER_IP = "127.0.0.1";
    private const int SERVER_PORT = 8888;
    private const int BUFFER_SIZE = 1024;

    static void Main()
    {
        try
        {
            Console.Write("Введите ваше имя: ");
            string userName = Console.ReadLine();
            if (string.IsNullOrEmpty(userName))
            {
                Console.WriteLine("Имя не может быть пустым");
                return;
            }

            // Создание TCP-клиента и подключение к серверу
            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Parse(SERVER_IP), SERVER_PORT);
            
            // Отправляем имя серверу
            byte[] nameData = Encoding.UTF8.GetBytes(userName);
            client.GetStream().Write(nameData, 0, nameData.Length);

            Console.WriteLine("Connected to chat server. Type your messages:");

            // Запуск потока для чтения сообщений от сервера
            Thread receiveThread = new Thread(() => ReceiveMessages(client));
            receiveThread.Start();

            // Основной цикл для отправки сообщений
            while (true)
            {
                string message = Console.ReadLine();
                if (string.IsNullOrEmpty(message)) continue;

                byte[] data = Encoding.UTF8.GetBytes(message);
                NetworkStream stream = client.GetStream();
                stream.Write(data, 0, data.Length);

                if (message.Equals("/quit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Disconnecting...");
                    break;
                }
            }

            // Закрытие соединения
            client.Close();
            receiveThread.Join();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static void ReceiveMessages(TcpClient client)
    {
        try
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[BUFFER_SIZE];
            
            while (true)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    Console.WriteLine("Server disconnected");
                    Environment.Exit(0);
                }
                
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine(message);
            }
        }
        catch (Exception)
        {
            // Игнорируем ошибки при закрытии соединения
        }
    }
}
