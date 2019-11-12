using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace practice_1
{
    class User
    {
        private Socket Socket { get; set; }
        private Server Server;
        private int Id { get; set; }
        private int Tasks = 0;
        //private int number = 0;

        public User(Server Server, Socket Socket, int Id)
        {
            this.Socket = Socket;
            this.Server = Server;
            this.Id = Id;
        }

        public void Processing()
        {
            try
            {
                while (Server.Work)
                {
                    int number = Reading();

                    if (Tasks == 0)
                    {
                        Tasks = 1;
                        Task task = new Task(() => Working(number));
                        task.Start();
                    }
                    else
                    {
                        Tasks++;
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("User " + Id +" Error: " + e.Message);
            }

            Exit();
        }

        private void Task()
        {
            string text = " дней до конца света!";
            byte[] receiveBuffer = new byte[4];
            byte[] sendBuffer;
            byte[] sendSize = new byte[4];

            Socket.Receive(receiveBuffer, 4, SocketFlags.None);
            Console.WriteLine("User " + Id + " получен запрос.");
            text = BitConverter.ToInt32(receiveBuffer, 0).ToString() + text;
            sendBuffer = Encoding.Unicode.GetBytes(text);
            sendSize = BitConverter.GetBytes(sendBuffer.Length);

            Thread.Sleep(6000);

            Socket.Send(sendSize);
            Socket.Send(sendBuffer);

            Console.WriteLine("User " + Id + " отправлен ответ на запрос.");
        }

        private int Reading()
        {
            byte[] receiveBuffer = new byte[4];
            Socket.Receive(receiveBuffer, 4, SocketFlags.None);
            Console.WriteLine("User " + Id + " получен запрос.");
            return BitConverter.ToInt32(receiveBuffer, 0);
        }


        private void Working(int number)
        {
            Thread.Sleep(6000);
            string text = " дней до конца света!";
            text = number + text;
            Answer(Encoding.Unicode.GetBytes(text));
        }

        private void Answer(byte[] answer)
        {
            for (int i = 0; i < Tasks; i++)
            {
                Socket.Send(BitConverter.GetBytes(answer.Length));
                Socket.Send(answer);
                Console.WriteLine("User " + Id + " отправлен ответ на запрос.");
            }
            Tasks = 0;
        }

        private void Exit()
        {
            Socket.Close();
        }
    }
}
