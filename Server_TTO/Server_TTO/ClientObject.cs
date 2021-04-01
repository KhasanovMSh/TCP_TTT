using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Threading;

namespace Server_TTO
{
    public class ClientObject
    {     
        protected internal string Id { get; private set; }
        protected internal string type { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        string userName;
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
                Console.WriteLine(message);
                // в бесконечном цикле получаем сообщения от клиента
                while (true)
                {
                    try
                    {
                        Console.WriteLine("Кол-во игроков"+server.player);
                        if (message.Contains("logout"))
                        {
                            server.ClearTable();
                            Thread.Sleep(50);
                            message = String.Format("{0}: покинул игру", userName);
                            Console.WriteLine(message);
                            // в случае выхода из цикла закрываем ресурсы
                            if (server.player == 2 && this.Id == server.clients[0].Id)
                            {
                                server.clients[0] = server.clients[1];
                                server.clients[1] = null;
                                server.clients.RemoveAt(1);
                            }
                            if (this.Id == server.clients[0].Id)
                            {
                                server.clients.RemoveAt(0);
                            }
                            if (this.Id == server.clients[1].Id)
                            {
                                server.clients.RemoveAt(1);
                            }
                            server.newgame = true;
                            server.player--;
                            Console.WriteLine("Кол-во игроков" + server.player);
                            server.RemoveConnection(this.Id);
                            Close();
                        }
                        if (server.player == 2 && server.newgame)
                        {
                            server.UniversalMessage("Ваш ход", 0);
                            Thread.Sleep(50);
                            server.UniversalMessage("Ждите", 1);
                            Thread.Sleep(50);
                            server.UniversalMessage("X", 0);
                            Thread.Sleep(50);
                            server.UniversalMessage("O", 1);
                            Thread.Sleep(50);
                            server.newgame = false;
                        }
                        message = GetMessage();
                        if (message.Contains('|'))
                        {
                            String[] words = message.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                            I = Convert.ToInt32(words[1]);
                            V = words[0];
                            server.buttons[I].Text = V;
                            server.BroadcastMessage(message, this.Id);
                            server.checkWin();
                            if(server.buttons[0].Text!="" && server.buttons[1].Text != "" && server.buttons[2].Text != "" 
                                && server.buttons[3].Text != "" && server.buttons[4].Text != "" && server.buttons[5].Text != "" 
                                && server.buttons[6].Text != "" && server.buttons[7].Text != "" && server.buttons[8].Text != "")
                            {
                                server.ClearTable();
                            }
                            switch (server.Turn)
                            {
                                case "X":
                                    {
                                        server.UniversalMessage("Ждите", 0);
                                        Thread.Sleep(50);
                                        server.UniversalMessage("Ваш ход", 1);
                                        Thread.Sleep(50);
                                        server.Turn = "O";
                                        break;
                                    }
                                case "O":
                                    {
                                        server.UniversalMessage("Ждите", 1);
                                        Thread.Sleep(50);
                                        server.UniversalMessage("Ваш ход", 0);
                                        Thread.Sleep(50);
                                        server.Turn = "X";
                                        break;
                                    }
                            }
                        }
                    }
                    catch
                    {
                        server.ClearTable();
                        message = String.Format("{0}: покинул игру", userName);
                        Console.WriteLine(message);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            /*finally
            {
                server.ClearTable();
                Console.WriteLine("АААААААААА");
                // в случае выхода из цикла закрываем ресурсы
                if (server.player == 2 && this.Id == server.clients[0].Id)
                {
                    server.clients[0] = server.clients[1];
                    //server.clients.RemoveAt(1);
                    //server.clients[0].type = "X";
                    Console.WriteLine(server.clients[0].userName);
                    Console.WriteLine(server.clients[0].type);                  
                }               
                //server.newgame = true;
                server.player--;
                server.RemoveConnection(this.Id);
                Close();
            }*/
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
