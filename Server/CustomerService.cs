using Server.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sevice
{

    class CustomerService
    {
        #region Fields

   
        private Dictionary<string, ProdWarehouse> _products;
        private Stream _networkStream;
        private EventHandler<ServerEventArgs> _eventHandler;
        private Func<Stream, string> _receiveClientMsg;
        #endregion

        #region Full Properties 

        public event EventHandler<ServerEventArgs> EventSendMessage
        {
            add { _eventHandler += value; }
            remove { _eventHandler -= value; }
        }
        public event Func<Stream, string> EventReceiveClietnMsg
        {
            add { _receiveClientMsg += value; }
            remove { _receiveClientMsg -= value; }
        }

       
        public Stream stream
        {
            get { return _networkStream == null ? new MemoryStream() : _networkStream; }
            set { _networkStream = value == null ? new MemoryStream() : value; }
        }
        #endregion

        public CustomerService(Dictionary<string, ProdWarehouse> dict) => _products = dict;

        public void Greetings()
        {
            string msg = "Добро пожаловать на кассу самообслуживания." +
              " Пожалуйста, просканируйте свой товар либо введите его товарный номер! " +
              "После того как просканируете все продукты, введите <1> что-бы приступить к оплате!";

            _eventHandler?.Invoke(this, new ServerEventArgs { Message = msg, stream = stream });

        }
        public void ChekingPaymentMethod()
        {

            double sumproduct = 0;

            while (true)
            {
                string clientmsg = _receiveClientMsg?.Invoke(stream);

                if (clientmsg ==  "1")
                {
                    Console.WriteLine("Клиент приступил к оплате");

                    string totalAmount = $"Итоговая сумма за продукты {sumproduct} грн.\nДалее выберите способ опаты:\n" +
                                $"Нажмите <0>, если оплата будет производится по карте\n" +
                                $"Нажмите <1>, если оплата будет производится наличными средствами\n";
                    _eventHandler?.Invoke(this, new ServerEventArgs { Message = totalAmount, stream = stream });

                    PaymentMethodProcessing(sumproduct);
                    break;
                }

                double scanresult = ChekingProductNumb(clientmsg);
                sumproduct += scanresult;
            }

        }

        private double ChekingProductNumb(string msg)
        {
            string basket = "";
            double sum = 0;

            if (!int.TryParse(msg, out int numbprod))
                _eventHandler?.Invoke(this, new ServerEventArgs { Message = "Вы ввели не коректый номер продукта!", stream = stream });


            foreach (var item in _products)
            {
                if (item.Key == msg)
                {
                    sum += item.Value.PriceProduct;
                    basket += $"Ваш продукт:\n Название: {item.Value.NameProduct}. Цена: {item.Value.PriceProduct} грн.\n";

                    _eventHandler?.Invoke(this, new ServerEventArgs { Message = basket, stream = stream });

                    Console.WriteLine($"Клиент ввёл/сканировал продукт по номеру: {msg} ");
                    break;
                }
            }

            if (basket == "" || sum == 0)
                _eventHandler?.Invoke(this, new ServerEventArgs { Message = "Нет такого продукта с таким номером", stream = stream });

            return sum;
        }
        private bool IsPaymentSuccessful(string pay, double amountTOpaid)
        {
            if (pay != "0" || pay != "1")
            {
                _eventHandler?.Invoke(this, new ServerEventArgs
                {
                    Message = "Такого способа оплаты нет\n",
                    stream = stream
                });
                return false;
            }


            if (pay == "0")
            {
                _eventHandler?.Invoke(this, new ServerEventArgs
                {
                    Message = "Вы выбрали способ оплаты картой. Вставьте карточку..." +
                    $"С вашего счета списано {amountTOpaid} грн.\n",
                    stream = stream
                });
                return true;
            }

            if (pay == "1")
            {
                _eventHandler?.Invoke(this, new ServerEventArgs
                {
                    Message = "Вы выбрали оплату наличными средствами." +
                    $" С вас {amountTOpaid} грн.\n...200 Оплата прошла успешно. Спасибо за покупки\n",
                    stream = stream
                });
                return true;
            }
            return true;
        }
        private void PaymentMethodProcessing(double sumproduct)
        {
            string paymethdclient = string.Empty;
            while (true)
            {
                paymethdclient = _receiveClientMsg?.Invoke(stream);

                if (IsPaymentSuccessful(paymethdclient, sumproduct))
                {
                    Console.WriteLine("Клиент оплатил товар!");
                    break;
                }
            }


        }

    }
}
