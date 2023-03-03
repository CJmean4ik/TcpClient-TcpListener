using Server.Model;
using ServerTerminal;
using Sevice;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;


namespace Server
{
    class Server
    {
       
        private Dictionary<string, ProdWarehouse> product = new Dictionary<string, ProdWarehouse>
        {
            ["78291"] = new ProdWarehouse("Хлеб", 11.50),
            ["53221"] = new ProdWarehouse("Сыр", 25.89),
            ["90101"] = new ProdWarehouse("Колбаса", 45.5),
            ["56722"] = new ProdWarehouse("Яйца (Лоток 10 шт.)", 60.0),
            ["12462"] = new ProdWarehouse("Зелень", 15.75)
        };

 
        static async Task Main(string[] args)
        {
           
            Server server = new Server();
           
            ServerWork serverWork = new ServerWork(3340);           
               
            serverWork.StartListening();
         
            TcpClient tcpClient = await serverWork.AcceptClientAsync();



            CustomerService service = new CustomerService(server.product);
            service.EventSendMessage += serverWork.SendClietnMsg;
            service.EventReceiveClietnMsg += serverWork.ReceiveClietnMsg;

          
            using (NetworkStream stream = tcpClient.GetStream())
            {
                service.stream = stream;


                service.Greetings();
                
                service.ChekingPaymentMethod();
               
                System.Console.WriteLine("Клиент забирает покупки ;)");
                Thread.Sleep(2000);

                 System.Console.WriteLine("Клиент забрал покупки");
                 tcpClient.Close();
                  serverWork.StopListening();

                 
            }
             
        }
        
    }
}
