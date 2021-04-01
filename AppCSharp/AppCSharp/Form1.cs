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
        private string type;
        private bool con=false;
        static string I;
        static string V;
        static string userName;
        private string host;
        private const int port = 8888;
        static TcpClient client;
        static NetworkStream stream;
        private Button[,] buttons =new Button[3,3];
        public Form1()
        {
            this.Icon = AppCSharp.Properties.Resources.tic_tac_toe_39453;
            this.BackgroundImage = AppCSharp.Properties.Resources.images;
            InitializeComponent();
        }
        static void SendMessage(object sender)
        {
                string message=sender.GetType().GetProperty("Text").GetValue(sender).ToString()+"|"+sender.GetType().GetProperty("Name").GetValue(sender).ToString();
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
                    switch (message)
                    {
                        case "clear":
                            {
                                clearButtons();
                                break;
                            }
                        case "Ваш ход":
                            {
                                label1.Text = message;
                                for (int i = 0; i < 3; i++)
                                {
                                    for (int j = 0; j < 3; j++)
                                    {
                                        buttons[i, j].Click += button1_Click;
                                    }
                                }
                                break;
                            }
                        case "Ждите":
                            {
                                label1.Text = message;
                                for (int i = 0; i < 3; i++)
                                {
                                    for (int j = 0; j < 3; j++)
                                    {
                                        buttons[i, j].Click -= button1_Click;             
                                    }
                                }
                                break;
                            }
                        case "X":
                            {
                                type = "X";
                                break;
                            }
                        case "O":
                            {
                                type = "O";
                                break;
                            }
                    }                 
                    if (message.Contains("Победили"))
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                buttons[i, j].Click -= button1_Click;
                            }
                        }
                        MessageBox.Show(message);
                    }
                    if (message.Contains('|'))
                    {
                        String[] words = message.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
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
        static void LogOut()
        {
            string message = "logout";
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length); 
        }
        private void setButtons()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j <3; j++)
                {
                    Console.WriteLine(buttons[i, j].Name);
                    buttons[i, j].Location = new Point(12 + 206 * j, 12 + 206 * i);
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
            sender.GetType().GetProperty("Text").SetValue(sender, type);
            sender.GetType().GetProperty("Enabled").SetValue(sender, false);
            SendMessage(sender);
        }       

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (con)
            {
                LogOut();
                Disconnect();
            }
        }
        private void Input_Click(object sender, EventArgs e)
        {
            userName=textBox1.Text;
            host=textBox2.Text;
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
                con = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            this.Height = 700;
            this.Width = 900;           
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
            if (con)
            {
                LogOut();
                Disconnect();
            }           
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
