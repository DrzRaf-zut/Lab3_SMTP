using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Xml;

namespace SMTP
{
    class SMTPClient
    {
        private string senderServerAddress;
        private string senderAddress;
        private string login;
        private string pass;
        private int senderPort;


        public SMTPClient()
        {
            XmlReader xmlReader = XmlReader.Create("App.config");
            xmlReader.MoveToContent();
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    if (xmlReader.Name == "senderServerAddress")
                        senderServerAddress = xmlReader.ReadElementString();
                    else if (xmlReader.Name == "senderAddress")
                        senderAddress = xmlReader.ReadElementString();
                    else if (xmlReader.Name == "user")
                        login = xmlReader.ReadElementString();
                    else if (xmlReader.Name == "password")
                        pass = xmlReader.ReadElementString();
                    else if (xmlReader.Name == "senderPort")
                        senderPort = Int32.Parse(xmlReader.ReadElementString());
                }
            }
        }

        public void sendEmail(string recipent, string subject, string content)
        {
            using (TcpClient tcpClient = new TcpClient(senderServerAddress, senderPort))
            {
                using (StreamReader sr = new StreamReader(tcpClient.GetStream()))
                {
                    using (StreamWriter sw = new StreamWriter(tcpClient.GetStream()))
                    {
                        string responseLine = string.Empty;
                        responseLine = sr.ReadLine();

                        sw.WriteLine("HELO " + senderServerAddress);
                        sw.Flush();
                        responseLine = sr.ReadLine();
                        if(responseLine.Substring(0, 3) != "250")
                            throw new Exception("Wystapil blad w komunikacji z serwerem.\n" + responseLine);

                        sw.WriteLine("AUTH PLAIN");
                        sw.Flush();
                        responseLine = sr.ReadLine();
                        if (responseLine.Substring(0, 3) != "334")
                            throw new Exception("Wystapil blad w komunikacji z serwerem.\n" + responseLine);


                        sw.WriteLine(System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("\0" + login + "\0" + pass)));
                        sw.Flush();
                        responseLine = sr.ReadLine();
                        if (responseLine.Substring(0, 3) != "235")
                            throw new Exception("Wystapil blad w komunikacji z serwerem.\n" + responseLine);


                        sw.WriteLine("MAIL FROM: <" + senderAddress + ">");
                        sw.Flush();
                        responseLine = sr.ReadLine();
                        if (responseLine.Substring(0, 3) != "250")
                            throw new Exception("Wystapil blad w komunikacji z serwerem.\n" + responseLine);


                        sw.WriteLine("RCPT TO: <" + recipent + ">");
                        sw.Flush();
                        responseLine = sr.ReadLine();
                        if (responseLine.Substring(0, 3) != "250")
                            throw new Exception("Wystapil blad w komunikacji z serwerem.\n" + responseLine);


                        sw.WriteLine("DATA");
                        sw.Flush();
                        responseLine = sr.ReadLine();
                        if (responseLine.Substring(0, 3) != "354")
                            throw new Exception("Wystapil blad w komunikacji z serwerem.\n" + responseLine);

                        string message = "Subject: " + subject + "\r\n"
                                       + "To: " + recipent + "\r\n"
                                       + "From: " + senderAddress + "\r\n\r\n"
                                       + content + "\r\n"
                                       + ".\r\n";
                        sw.Write(message);
                        sw.Flush();
                        responseLine = sr.ReadLine();
                        if (responseLine.Substring(0, 3) != "250")
                            throw new Exception("Wystapil blad w komunikacji z serwerem.\n" + responseLine);


                        sw.WriteLine("QUIT");
                        sw.Flush();
                        responseLine = sr.ReadLine();
                        if (responseLine.Substring(0, 3) != "221")
                            throw new Exception("Wystapil blad w komunikacji z serwerem.\n" + responseLine);

                        Console.WriteLine("Wiadomosc zostala pomyslnie wyslana\n");
                    }
                }
            }
        }
    }
}
