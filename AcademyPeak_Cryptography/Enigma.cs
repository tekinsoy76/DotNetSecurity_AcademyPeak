using System.Security.Cryptography;
using System.Text;

namespace AcademyPeak_Cryptography
{
    public class Enigma
    {
        // Asagidaki sadece hassas olmayan ama hizlica gozlerden saklanmak istenen veriler icin kullanilabilir
        public string SimpleEncryptor(string data)
        {
            byte[] dataArray = Encoding.UTF8.GetBytes(data);
            return Convert.ToBase64String(dataArray);
        }

        public string SimpleDecriptor(string data)
        {
            byte[] dataArray = Convert.FromBase64String(data);
            return Encoding.UTF8.GetString(dataArray);
        }


        //Hash: tek yonlu sifreleme
        public string HashEncryptor<T>(string data) where T:HashAlgorithm
        {
            byte[] dataArray = Encoding.UTF8.GetBytes(data);
            T provider = Activator.CreateInstance<T>();
            byte[] encryptedArray = provider.ComputeHash(dataArray);
            return Convert.ToBase64String(encryptedArray);
        }

        public string HashEncryptor(string data, string password)
        {
            byte[] dataArray = Encoding.UTF8.GetBytes(data);
            KeyedHashAlgorithm algorithm = KeyedHashAlgorithm.Create();
            byte[] passArray = Encoding.UTF8.GetBytes(password);
            algorithm.Key = passArray;
            byte[] encryptedArray = algorithm.ComputeHash(dataArray);
            return Convert.ToBase64String(encryptedArray);
        }


        // Symetric Algoritmalar: Bir anahtar ve bir vektor degeri ile sifrelenebilen ve acilabilen yapilar
        public string SymmetricEncryptor<T>(string data) where T: SymmetricAlgorithm
        {
            string encryptedData = string.Empty;
            // profesor
            T provider = Activator.CreateInstance<T>();
            // asistan
            ICryptoTransform transform = provider.CreateEncryptor(Kasa.Instance.Key, Kasa.Instance.Vector);

            //Key ve vektor provider tarafindan (bir kereye mahsus) uretilebilir
            //T providerBirSefer = Activator.CreateInstance<T>();
            //string birKerelikAnahtar = Convert.ToBase64String(providerBirSefer.Key); //Ornek: xDfR3A@G9OwNf7l6
            //string birKerelikVector = Convert.ToBase64String(providerBirSefer.IV); //Ornek: r9Qw5Ug1Ap8S!

            // calisma ortami
            using (MemoryStream ms = new MemoryStream())
            {
                //yonetici
                using (CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Write))
                {
                    //yazici
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(data);
                    }// yazici gitti
                }//yonetici gitti
                encryptedData = Convert.ToBase64String(ms.ToArray());
            }
            return encryptedData;
        }

        public string SymmetricDecryptor<T>(string data) where T: SymmetricAlgorithm
        {
            string decryptedData = string.Empty;
            T provider = Activator.CreateInstance<T>();
            ICryptoTransform transform = provider.CreateDecryptor(Kasa.Instance.Key, Kasa.Instance.Vector);
            byte[] encryptedData = Convert.FromBase64String(data);
            using (MemoryStream ms = new MemoryStream(encryptedData))
            {
                using (CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        decryptedData = sr.ReadToEnd();
                    }
                }
            }
            return decryptedData;
        }
    }
}