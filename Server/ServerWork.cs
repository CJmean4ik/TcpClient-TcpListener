using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Sevice;


namespace ServerTerminal
{
    class ServerWork
    {
        private TcpListener _listener;
        private bool _listenerIsALive;

        public ServerWork(int port) => _listener = new TcpListener(IPAddress.Loopback, port);

        /// <summary>
        /// Запускает ожидание входящих запросов
        /// </summary>
        public void StartListening()
        {
            if (_listenerIsALive == false)
            {
                _listener.Start();
                Console.WriteLine("Терминал запущен...");
                _listenerIsALive = true;
            }
        }

        /// <summary>
        /// Останавливает слушатель
        /// </summary>
        public void StopListening()
        {
            if (_listenerIsALive == true)
            {

                _listener.Stop();
                Console.WriteLine("Терминал отключен...");
                _listenerIsALive = false;
            }
        }

        /// <summary>
        /// Принимает асинхронно клиента, который подключился 
        /// </summary>
        /// <returns></returns>
        public async Task<TcpClient> AcceptClientAsync()
        {
            Console.WriteLine("Ожидаю клиентов...");

            TcpClient tcpClient = await _listener.AcceptTcpClientAsync();
            Console.WriteLine("Подключен клиент. Приступаю к обслуживанию...");
            return tcpClient;
        }

        /// <summary>
        /// Считывает сообщение которое отправил клиент
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>Сообщение клиента</returns>
        public string ReceiveClietnMsg(Stream stream)
        {
            byte[] receivebuffer = new byte[1024];
            int bytereceived = stream.Read(receivebuffer, 0, receivebuffer.Length);
            return Encoding.Default.GetString(receivebuffer, 0, bytereceived);
        }

        /// <summary>
        /// Отправляет сообщение клиенту
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SendClietnMsg(object sender, ServerEventArgs e)
        {
            byte[] sendbuffer = Encoding.Default.GetBytes(e.Message);
            e.stream.Write(sendbuffer, 0, sendbuffer.Length);
        }
    }
}

