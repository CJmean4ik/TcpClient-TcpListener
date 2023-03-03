using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpClient_TcpListener
{
    class Program
    {
        static void Main(string[] args)
        {
            //Порт для подключение 
            int port = 3340;
            //Формируем конечную точку, то-есть адресс на котором работает сервер
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, port);

            TcpClient tcpClient = new TcpClient();

            //Подключаемся 
            tcpClient.Connect(endPoint.Address, port);

            //Поток для передачи сообщений 
            using (NetworkStream stream = tcpClient.GetStream())
            {
                while (true)
                {
                    
                    string msg = ReceiveServerMsg(stream);
                    Console.WriteLine(msg);
                    string str = Console.ReadLine();
                    if (str == "1")
                    {
                        //Отправка серверу, что клиент готов оплатить продукты
                        SendServerMsg(stream, str);

                        //Сообщение от сервера. Выбор способа оплаты
                        Console.WriteLine(ReceiveServerMsg(stream));
                        string paymentM = string.Empty;
                        do
                        {
                            //Вводим 0 или 1. Что-бы отправить серверу о способе оплаты
                            paymentM = Console.ReadLine();

                            //Отправляем способ оплаты
                            SendServerMsg(stream, paymentM);

                            //Получаем сообщение от сервера
                            string receivemsg = ReceiveServerMsg(stream);

                            if (receivemsg.Contains("200"))
                            {
                                Console.WriteLine(receivemsg.Replace("200", ""));
                                break;
                            }
                            else
                            {
                                Console.WriteLine(receivemsg);
                            }


                        } while (paymentM != "0" || paymentM != "1");

                        break;
                    }
                    else SendServerMsg(stream, str);
                }
                int opt = int.Parse(Console.ReadLine());

                if (opt == 333)
                {
                    SendServerMsg(stream, "Exit");
                    tcpClient.Close();
                }
                
            }
            Console.ReadKey();

        }
        //Метод для получения сообщения от сервера 
        static string ReceiveServerMsg(Stream stream)
        {           
            byte[] receivebuffer = new byte[1024];
            int bytereceived = stream.Read(receivebuffer, 0, receivebuffer.Length);
            return Encoding.Default.GetString(receivebuffer, 0, bytereceived);                      
        }

        //Метод для отправки сообщение серверу
        static void SendServerMsg(Stream stream, string msg)
        {          
            byte[] sendbuffer = Encoding.Default.GetBytes(msg);
            stream.Write(sendbuffer, 0, sendbuffer.Length);
        }
    }
}
