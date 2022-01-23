using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SignalR_Client1_Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var accessToken = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJjbGllbnROYW1lIjoiTWlkZGxld2FyZSBBUEkiLCJnbG9iYWxJZCI6MTExMTExMSwicmVxdWVzdG9yIjoiMTExMTExMTExMSIsInJvbGUiOiJzdXBlckFkbWluIiwicmlnaHRzIjpbeyJtZXRob2QiOiJzaWduYWxyIiwiYWN0aW9ucyI6WyJnZXQiXX1dLCJuYmYiOjE1OTg0NDc0MjcsImV4cCI6MTU5ODQ0NzQ4NywiaWF0IjoxNTk4NDQ3NDI3LCJpc3MiOiJodHRwczovL3d3dy50ZXN0aW5nLmNvbSIsImF1ZCI6Ijg4ODg4ODg4ODgifQ.fx74VbN9mB1pvSqfkqhTNAu3WVIq5pR5lGqm2Tn-uEeTzN4XQHbT3QVGbzvNtbRXmFRYeNX5oKLn6Q4xqAcVfmWHA-ELL8yzVTK8zzN4wJpiEooruqleEKjXl1diCEXokIIjNcQ9cnGYBP578owDRiu94bIZzUc_MM0ztH9B6_iKtQpNzanNiFerJ4p7KH6zDMwTXraAQGXh11ed7iEaXgwfXjWv4gkshYQM1kGOP-Rmz25d9a3KBDYAxMNtCg3Es7dCqzZAP_DVoZG6SIHVLbYpf1_B4db22jlY3vGdI4frzMkmMrDuzlz2dcLLGx9y4hRKy548T4dCYbwpO3oVdQ";

            var sr = new ConnectToSignalR();
            sr.Start(accessToken);

            do
            {
                Console.WriteLine("Please enter your message to server");
                var e = Console.ReadLine();
                if (e.ToLower() == "stop")
                {
                    sr.Stop();
                    break;
                }
                else
                {
                    sr.SendMessage(e);
                }
            } while (true);

            Console.ReadLine();
        }
    }
}
