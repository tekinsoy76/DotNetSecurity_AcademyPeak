using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademyPeak_Cryptography
{
    public sealed class Kasa
    {
        private Kasa()
        {
            Key = Convert.FromBase64String("6w68TxeiyCCtvmA36ZxhyXfQkqJCHhKF+NZGSF+MHLE=");
            Vector = Convert.FromBase64String("KyJcmAD+OXY91mT9McK2mw==");
        }

        private static Kasa _instance;

        public static Kasa Instance
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = new Kasa();
                }
                return _instance; 
            }
        }


        public byte[] Key { get; set; }
        public byte[] Vector { get; set; }
    }
}
