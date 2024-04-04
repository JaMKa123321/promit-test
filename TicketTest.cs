using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using System.Globalization;

namespace promit_test
{
    [TestFixture]
    public class TicketTest
    {
        string path = AppDomain.CurrentDomain.BaseDirectory;

        public static int[] receiptNums = {6, 7};


        [TestCaseSource(nameof(receiptNums))]
        public void Test1(int current)
        {
            StreamReader sr = new StreamReader("../../../promit/Receipt_" + current.ToString().PadLeft(3, '0') + ".txt");
            sr.ReadLine();
            sr.ReadLine();

            // Дата
            string read = "";
            while ( ((char) sr.Peek()) != '\r')
            {
                char next = (char) sr.Read();
                read += (Char.IsDigit(next) || next == '.') ? next : "";
            }
            DateTime? dt = null;
            try
            {
                dt = DateTime.ParseExact(read, "dd.MM.yyyy", null);
            }
            catch
            {
                Assert.Fail("Некорректная дата");
            }
            // Assert.That(dt!, Is.LessThanOrEqualTo(DateTime.Now));
            sr.ReadLine();

            // Отправление
            read = "";
            while ( ((char) sr.Peek()) != '\r')
            {
                char next = (char) sr.Read();
                read += (next != ' ') ? next : "";
            }
            Assert.That(read.Length, Is.GreaterThan(2));
            sr.ReadLine();

            // Прибытие
            read = "";
            while ( ((char) sr.Peek()) != '\r')
            {
                char next = (char) sr.Read();
                read += (next != ' ') ? next : "";
            }
            Assert.That(read.Length, Is.GreaterThan(2));
            sr.ReadLine();

            // Номер билета
            read = "";
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

            // Перевозка
            read = "";
            string number = "";
            while (((char) sr.Peek()) != '\r')
            {
                char next = (char) sr.Read();
                read += next != ' ' ? next : "";
                number += Char.IsDigit(next) ? next : "";
            }
            Assert.That(number.Length, Is.GreaterThanOrEqualTo(4));
            Assert.That(read.Length, Is.GreaterThan(9));
            sr.ReadLine();

            // Стоимость по тарифу
            while (((char) sr.Peek()) != '=' || ((char) sr.Peek()) != '\r')
                sr.Read();
            
            read = "";
            while (((char) sr.Peek()) != '\r')
            {
                char next = (char) sr.Read();
                read += Char.IsDigit(next) ? next : "";
            }
            Assert.That((float) Int32.Parse(read) / 100, Is.GreaterThan(0));
            sr.ReadLine();

            // ИТОГ
            read = "";
            for (int i = 0; i < 4; i++)
            {
                read += (char) sr.Read();
            }
            Assert.That(read, Is.EqualTo("ИТОГ"));

            read = "";
            while ( ((char) sr.Peek()) != '\r' )
            {
                char next = (char) sr.Read();
                read += Char.IsDigit(next) ? next : "";
            }
            Assert.That(read.Length, Is.GreaterThan(0));
        }
    }
}