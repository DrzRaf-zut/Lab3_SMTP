using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMTP
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                SMTPClient smtpClient = new SMTPClient();

                Console.WriteLine("Podaj adres odbiorcy:\n");
                string recipent = Console.ReadLine();
                string subject = "PS LAB N2 ZIMA 2018 N2 14B";
                string content = "Rafał Drzewowski";

                smtpClient.sendEmail(recipent, subject, content);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
