using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace AppCSharp
{
    public class ServerObject
    {
        static int CN = 2;
        internal string[,] buttons = new string[3, 3];
        static TcpListener tcpListener; // сервер для прослушивания
        List<ClientObject> clients = new List<ClientObject>(); // все подключения

        protected internal void AddConnection(ClientObject clientObject)
        {
            clients.Add(clientObject);
        }
        protected internal void RemoveConnection(string id)
        {
            // получаем по id закрытое подключение
            ClientObject client = clients.FirstOrDefault(c => c.Id == id);
            // и удаляем его из списка подключений
            if (client != null)
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
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
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
                    clients[i].Stream.Write(data, 0, data.Length); //передача данных
                }
            }
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
        protected internal void checkWin()
        {
            if (buttons[0, 0] == buttons[0, 1] && buttons[0, 1] == buttons[0, 2])
            {
                if (buttons[0, 0] != "")
                {
                    //MessageBox.Show("Вы победили!");
                    return;
                }
            }
            if (buttons[1, 0]  == buttons[1, 1]  && buttons[1, 1]  == buttons[1, 2] )
            {
                if (buttons[1, 0]  != "")
                     Console.WriteLine();
                    //MessageBox.Show("Вы победили!");
            }
            if (buttons[2, 0]  == buttons[2, 1]  && buttons[2, 1]  == buttons[2, 2] )
            {
                if (buttons[2, 0]  != "")
                    Console.WriteLine();
                //MessageBox.Show("Вы победили!");
            }
            if (buttons[0, 0]  == buttons[1, 0]  && buttons[1, 0]  == buttons[2, 0] )
            {
                if (buttons[0, 0]  != "")
                    Console.WriteLine();
                //MessageBox.Show("Вы победили!");
            }
            if (buttons[0, 1]  == buttons[1, 1]  && buttons[1, 1]  == buttons[2, 1] )
            {
                if (buttons[0, 1]  != "")
                    Console.WriteLine();
                //MessageBox.Show("Вы победили!");
            }
            if (buttons[0, 2]  == buttons[1, 2]  && buttons[1, 2]  == buttons[2, 2] )
            {
                if (buttons[0, 2]  != "")
                    Console.WriteLine();
                // MessageBox.Show("Вы победили!");
            }
            if (buttons[0, 0]  == buttons[1, 1]  && buttons[1, 1]  == buttons[2, 2] )
            {
                if (buttons[0, 0]  != "")
                    Console.WriteLine();
                // MessageBox.Show("Вы победили!");
            }
            if (buttons[2, 0] == buttons[1, 1] && buttons[1, 1] == buttons[0, 2])
            {
                if (buttons[2, 0] != "")
                    Console.WriteLine();
            }
        }
    }
}
