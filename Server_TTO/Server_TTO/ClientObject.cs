using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Server_TTO
{
    public class ClientObject
    {
      
        protected internal string Id { get; private set; }
        protected internal string type { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        string userName;
        private string playerType;
        int I;
        string V;
        TcpClient client;
        ServerObject server; // объект сервера

        public ClientObject(TcpClient tcpClient, ServerObject serverObject,string Type)
        {           
             Id = Guid.NewGuid().ToString();
             client = tcpClient;
             server = serverObject;
             type = Type;
             serverObject.AddConnection(this);
        }
        public void Process()
        {
            try
            {
                Stream = client.GetStream();
                // получаем имя пользователя
                string message = GetMessage();
                userName = message;
                message = userName + " вошел в игру";
                // посылаем сообщение о входе в чат всем подключенным пользователям
                server.BroadcastMessage(message, this.Id);
                Console.WriteLine(message);
                // в бесконечном цикле получаем сообщения от клиента
                while (true)
                {

                    try
                    {
                        message = GetMessage();
                        String[] words = message.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        //message = String.Format("{0}: {1}", userName, message);
                        I = Convert.ToInt32(words[1]);
                        V = words[0];
                        server.buttons[I].Text = V;
                        Console.WriteLine(message);
                       // server.BroadcastMessage(message, this.Id);
                        server.checkWin();
                    }
                    catch
                    {
                        server.ClearTable();
                        message = String.Format("{0}: покинул игру", userName);
                        Console.WriteLine(message);
                        //server.BroadcastMessage(message, this.Id);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // в случае выхода из цикла закрываем ресурсы
                if (server.player == 2 && this.Id == server.clients[0].Id)
                {
                    server.clients[0] = server.clients[1];
                    server.clients[0].type = "X";
                    Console.WriteLine(server.clients[0].userName);
                    Console.WriteLine(server.clients[0].type);
                }
                server.player--;
                server.RemoveConnection(this.Id);
                Close();
            }
        }

        // чтение входящего сообщения и преобразование в строку
        private string GetMessage()
        {
            byte[] data = new byte[64]; // буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        // закрытие подключения
        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();         
        }
    }
}
