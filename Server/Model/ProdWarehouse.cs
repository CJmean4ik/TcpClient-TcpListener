using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Model
{
    class ProdWarehouse
    {
        public string NameProduct { get; set; }
        public double PriceProduct { get; set; }
        public ProdWarehouse(string nameprod, double price) => (NameProduct, PriceProduct) = (nameprod, price);                
    }
}
