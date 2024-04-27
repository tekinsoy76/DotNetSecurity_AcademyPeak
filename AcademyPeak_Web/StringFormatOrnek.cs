using System.Text;

namespace AcademyPeak_Web
{
    public class StringFormatOrnek
    {
        void Ornek()
        {
            string a = "";
            string b = "";
            string sonuc = "ornek" + a + b; // asla kullanmiyoruz
            sonuc = string.Format("ornek {0} {1}", a, b);
            sonuc = $"ornek {a} {b}";
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(a);
        }
    }
}
