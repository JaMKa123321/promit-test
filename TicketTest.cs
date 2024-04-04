using System.Text.RegularExpressions;

namespace promit_test
{
    [TestFixture]
    public class TicketTest
    {

        // Определяем номера файлов-билетов в директории
        public static int[] ReceiptNums()
        {
            string directory = "../../../promit";

            string[] txtFiles = Directory.GetFiles(directory, "*.txt");
            int[] toReturn = new int[txtFiles.Length];
        
            for (int i = 0; i < txtFiles.Length; i++)
            {
                string pattern = @"\d{3}[.]";
                int pos = Regex.Match(txtFiles[i], pattern).Index;
                toReturn[i] = Int32.Parse($"{txtFiles[i][pos]}{txtFiles[i][pos+1]}{txtFiles[i][pos+2]}");
            }

            return toReturn;
        }

        [TestCaseSource(nameof(ReceiptNums))]
        public void Test(int current)
        {
            StreamReader sr = new StreamReader("../../../promit/Receipt_" + current.ToString().PadLeft(3, '0') + ".txt");
            sr.ReadLine();
            sr.ReadLine();

            // Паттерны для даты, отправления и прибытия
            string[] patterns1 = {@"на\s*\d\s\d.*\d\s\d.*\d\s\d\s\d", @"от\s*.*[a-zA-Z]*", @"до\s*.*[a-zA-Z]*"};
            string? line;
            foreach (string pattern in patterns1)
            {
                line = sr.ReadLine();
                if (line == null)
                    Assert.Fail("Недостаточно строк");
                Assert.True(CheckLine(line!, pattern));
            }

            // Номер билета
            string read = "";
            while (read.Length < 5 && ((char) sr.Peek()) != '\r')
            {
                char next = (char) sr.Read();
                read += Char.IsDigit(next) ? next : "";
            }
            Assert.That(read.Length, Is.EqualTo(5));
            Assert.That(Int32.Parse(read), Is.GreaterThan(0));
            sr.Read();

            // Системный номер
            read = "";
            while (((char) sr.Peek()) != '\r')
            {
                char next = (char) sr.Read();
                read += Char.IsDigit(next) ? next : "";
            }
            Assert.That(read.Length, Is.EqualTo(13));
            Assert.That(Int64.Parse(read), Is.GreaterThan(0));
            sr.ReadLine();

            // Паттерны для перевозки, стоимости по тарифу и итога
            string[] patterns2 = {@"Перевозка.*->.*\d*$", @"Стоимость по тарифу:\s*=\d*[.,]\d*$", @"ИТОГ: \d*[.,]\d*"};
            foreach (string pattern in patterns2)
            {
                line = sr.ReadLine();
                if (line == null)
                    Assert.Fail("Недостаточно строк");
                Assert.True(CheckLine(line!, pattern));
            }
        }

        private bool CheckLine(string line, string pattern)
        {
            return Regex.Match(line, pattern).Index == 0;
        }
    }
}