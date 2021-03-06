using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using System.Text;
using System.Threading;

namespace Server_TTO
{
    public class ServerObject
    {
        public int player=0;
        public string Turn = "X";
        public bool won = false;
        public bool newgame = true;
        internal Button[] buttons = new Button[9];
        static TcpListener tcpListener; // сервер для прослушивания
        public List<ClientObject> clients = new List<ClientObject>(); // все подключения
        protected internal void CreateB()
        {
            for (int i = 0; i < buttons.Length; i++)
            {            
                    buttons[i]  = new Button();                          
            }
        }
        protected internal void AddConnection(ClientObject clientObject)
        {
            clients.Add(clientObject);

        }
        protected internal void RemoveConnection(string id)
        {
            // получаем по id закрытое подключение
            ClientObject client = clients.FirstOrDefault(c => c.Id == id);
            // и удаляем его из списка подключений
            clients.Remove(client);
        }
        // прослушивание входящих подключений
        protected internal void Listen()
        {           
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");
                while (true)
                {
                    if (player == 0) 
                    {
                        TcpClient tcpClient = tcpListener.AcceptTcpClient();
                        ClientObject clientObject = new ClientObject(tcpClient, this, "X");
                        Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                        clientThread.Start();                        
                        Console.WriteLine(clients[0].type);
                        player++;
                    }
                    if (player == 1)
                    {
                        TcpClient tcpClient = tcpListener.AcceptTcpClient();
                        ClientObject clientObject = new ClientObject(tcpClient, this, "O");
                        Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                        clientThread.Start();
                        Console.WriteLine(clients[1].type);
                        player++;
                    }
                    if (player == 2)
                    {
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //Disconnect();
            }
        }

        // трансляция сообщения подключенным клиентам
        protected internal void BroadcastMessage(string message, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id != id) // если id клиента не равно id отправляющего
                {
                    if (clients[i] != null)
                        clients[i].Stream.Write(data, 0, data.Length); //передача данных
                }
            }
        }
        protected internal void UniversalMessage(string message,int i)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            if (clients[i] != null)
                clients[i].Stream.Write(data, 0, data.Length); //передача данных
        }
        // отключение всех клиентов
        protected internal void Disconnect()
        {
            tcpListener.Stop(); //остановка сервера
            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close(); //отключение клиента
            }
            Environment.Exit(0); //завершение процесса
        }
        protected internal void ClearTable()
        {
            if (player != 0)
            {
                for (int i = 0; i < buttons.Length; i++)
                {
                    buttons[i].Text = "";
                }
                byte[] data = Encoding.Unicode.GetBytes("clear");
                for (int i = 0; i < clients.Count; i++)
                {
                        if(clients[i]!=null)
                             clients[i].Stream.Write(data, 0, data.Length); //передача данных
                }
                Thread.Sleep(50);
                if (won)
                {
                    data = Encoding.Unicode.GetBytes("Победили " + Turn);
                    for (int i = 0; i < clients.Count; i++)
                    {
                        if (clients[i] != null)
                            clients[i].Stream.Write(data, 0, data.Length); //передача данных
                    }
                }
                Thread.Sleep(50);
                won = false;
            }
        }
        protected internal void Win()
        {
            won = true;
            Console.WriteLine("Победили " + Turn);
            ClearTable();
        }
        protected internal void checkWin()
        {
            if (buttons[0].Text==buttons[1].Text && buttons[1].Text == buttons[2].Text)
            {
                if (buttons[0].Text != "")
                {
                    Win();
                }
            }
            if (buttons[3].Text == buttons[4].Text && buttons[4].Text == buttons[5].Text)
            {
                if (buttons[3].Text != "") 
                {
                    Win();
                }
            }
            if (buttons[6].Text == buttons[7].Text && buttons[7].Text == buttons[8].Text)
            {
                if (buttons[6].Text != "")
                {
                    Win();
                }
            }
            if (buttons[0].Text == buttons[3].Text && buttons[3].Text == buttons[6].Text)
            {
                if (buttons[0].Text != "")
                {
                    Win();
                }
            }
            if (buttons[1].Text == buttons[4].Text && buttons[4].Text == buttons[7].Text)
            {
                if (buttons[1].Text != "")
                {
                    Win();
                }
            }
            if (buttons[2].Text == buttons[5].Text && buttons[5].Text == buttons[8].Text)
            {
                if (buttons[2].Text != "")
                {
                    Win();
                }
            }
            if (buttons[0].Text == buttons[4].Text && buttons[4].Text == buttons[8].Text)
            {
                if (buttons[0].Text != "")
                {
                    Win();
                }
            }
            if (buttons[2].Text == buttons[4].Text && buttons[4].Text == buttons[6].Text)
            {
                if (buttons[2].Text != "")
                {
                    Win();
                }
            }
        }
    }
}
