using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Security;
namespace AppCSharp
{
    public partial class Form1 : Form
    {
        private bool Turn = false;
        static string I;
        static string V;
        static string userName;
        private const string host = "127.0.0.1";
        private const int port = 8888;
        static TcpClient client;
        static NetworkStream stream;
        private int x = 12, y = 12;
        private Button[,] buttons =new Button[3,3];
        private int player;
        public Form1()
        {
            InitializeComponent();
            string ip = new WebClient().DownloadString("https://api.ipify.org");
            textBox3.Text = ip.ToString(); ;
        }
        static void SendMessage(object sender)
        {
            //Console.WriteLine("Введите сообщение: ");
           
                string message=sender.GetType().GetProperty("Text").GetValue(sender).ToString()+" "+sender.GetType().GetProperty("Name").GetValue(sender).ToString();
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);
            
        }
        // получение сообщений
        private void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64]; // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);
                    string message = builder.ToString();
                    Console.WriteLine(message);
                    if (message == "clear")
                    {
                        clearButtons();
                    }
                    else if (message == "won")
                    {
                        MessageBox.Show("Вы выиграли");
                        clearButtons();
                    }
                    else if (message == "lose")
                    {
                        MessageBox.Show("Вы проиграли");
                        clearButtons();
                    }
                    else
                    {
                        String[] words = message.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        //message = String.Format("{0}: {1}", userName, message);
                        I = words[1];
                        V = words[0];
                        for (int i = 0; i < buttons.Length / 3; i++)
                        {
                            for (int j = 0; j < buttons.Length / 3; j++)
                            {
                                if (buttons[i, j].Name == I)
                                {
                                    buttons[i, j].Text = V;
                                    buttons[i, j].Enabled = false;
                                }
                            }

                        }
                        Console.WriteLine(message);//вывод сообщения
                    }
                }
                catch
                {
                    MessageBox.Show("Подключение прервано!"); //соединение было прервано
                    Disconnect();
                }
            }
        }

        static void Disconnect()
        {
            if (stream != null)
                stream.Close();//отключение потока
            if (client != null)
                client.Close();//отключение клиента
            Environment.Exit(0); //завершение процесса
        }
        private void setButtons()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j <3; j++)
                {
                    Console.WriteLine(buttons[i, j].Name);
                    buttons[i, j].Location = new Point(12 + 206 * j, 12 + 206 * i);
                    buttons[i, j].Click += button1_Click;
                    buttons[i, j].Font = new Font(new FontFamily("Microsoft Sans Serif"), 138);
                    buttons[i, j].Text = "";
                    this.Controls.Add(buttons[i, j]);
                }
            }
        }
        private void clearButtons()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    buttons[i, j].Text = "";
                    buttons[i, j].Enabled = true;
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            switch (player)
            {
                case 1:
                    sender.GetType().GetProperty("Text").SetValue(sender, "x");                    
                    player = 0;
                    label1.Text = "Текущий ход: Игрок 2";
                    break;
                case 0:
                    sender.GetType().GetProperty("Text").SetValue(sender, "o");
                    player = 1;
                    label1.Text = "Текущий ход: Игрок 1";
                    break;
            }
            sender.GetType().GetProperty("Enabled").SetValue(sender, false);
            SendMessage(sender);
            //checkWin();
        }       

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Disconnect();
        }

        private void Input_Click(object sender, EventArgs e)
        {
            userName=textBox1.Text;
            //host=textBox2.Text;
            client = new TcpClient();
            try
            {
                client.Connect(host, port); //подключение клиента
                stream = client.GetStream(); // получаем поток

                string message = userName;
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);

                // запускаем новый поток для получения данных
                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start(); //старт потока
                //MessageBox.Show("Добро пожаловать, {0}", userName);
                //SendMessage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // Disconnect();
            }
            this.Height = 700;
            this.Width = 900;
            player = 1;
            label1.Text = "Текущий ход: Игрок 1";
            for (int i = 0, id = 0; i < buttons.Length / 3; i++)
            {
                for (int j = 0; j < buttons.Length / 3; j++)
                {
                    buttons[i, j] = new Button();
                    buttons[i, j].Size = new Size(200, 200);
                    buttons[i, j].Name = id.ToString();
                    id++;
                }
            }
            setButtons();
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    buttons[i, j].Text = "";
                    buttons[i, j].Enabled = true;
                }
            }
        }
    }
}
