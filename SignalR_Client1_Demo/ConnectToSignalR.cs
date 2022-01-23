using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SignalR_Client1_Demo
{
    public class ConnectToSignalR
    {
        private HubConnection connection;
        private string token;

        public ConnectToSignalR()
        {

        }

        public async void Start(string token)
        {
            this.token = token;

            try
            {
                connection = new HubConnectionBuilder()
                    .WithUrl("http://localhost:64648/notificationhub", option =>
                    {
                        option.AccessTokenProvider = () => Task.FromResult(token);
                    })
                    .WithAutomaticReconnect()
                    .Build();
                
                //If theres no ping or message coming from server within the seconds setup
                connection.ServerTimeout = TimeSpan.FromSeconds(60);

                //It will send a ping to the server to maintain the connectivity
                connection.KeepAliveInterval = TimeSpan.FromSeconds(60);

                Connection_On_Listening_String();

                Connection_On_Listening_Object();

                connection.Closed += Connection_Closed;

                connection.Reconnected += Connection_Reconnected;

                connection.Reconnecting += Connection_Reconnecting;

                await connection.StartAsync();

                Console.WriteLine("You're now connected");
            }
            catch (HubException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if(ex.Message.Contains("No connection could be made because the target machine actively refused it."))
                {
                    ReconnectByRebuildingConnectionHub();
                }
            }
        }

        public async void Stop()
        {
            await connection.StopAsync();
        }

        public async void SendMessage(string message)
        {
            try
            {
                await connection.InvokeAsync("ClientMessageToAll", message);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Connection_On_Listening_String()
        {
            connection.On<string>("ReceiveMessage", (message) =>
            {
                Console.WriteLine($"{message}");
            });
        }

        private void Connection_On_Listening_Object()
        {
            connection.On<ServerMessage>("ReceiveMessageObject", (serverMessage) =>
            {
                Console.WriteLine($"{serverMessage.DateTime}");
                Console.WriteLine($"{serverMessage.Message}");
            });
        }

        private async Task Connection_Reconnecting(Exception arg)
        {
            Console.WriteLine($"Re-connecting. {arg.Message}");

            if (arg.Message.Contains("401 (Unauthorized)"))
            {
                await Connection_Closed(arg);
            }
        }

        private Task Connection_Reconnected(string arg)
        {
            Console.WriteLine($"Connected. {arg}");

            return Task.CompletedTask;
        }

        private async Task Connection_Closed(Exception arg)
        {
            if (arg == null) //Client manually closed the connection
            {
                Console.WriteLine("Connection closed without error.");
            }
            else
            {
                Console.WriteLine($"{arg.Message}");

                if (arg.Message.Contains("401 (Unauthorized)"))
                {
                    await connection.StopAsync();

                    Console.WriteLine($"Re-initialize Hub Connection Builder with the new token");

                    this.token = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJjbGllbnROYW1lIjoiTWlkZGxld2FyZSBBUEkiLCJnbG9iYWxJZCI6MTExMTExMSwicmVxdWVzdG9yIjoiMTExMTExMTExMSIsInJvbGUiOiJzdXBlckFkbWluIiwicmlnaHRzIjpbeyJtZXRob2QiOiJzaWduYWxyIiwiYWN0aW9ucyI6WyJnZXQiXX1dLCJuYmYiOjE1OTc3NDkzNTIsImV4cCI6MTYwNzc0OTM1MSwiaWF0IjoxNTk3NzQ5MzUyLCJpc3MiOiJodHRwczovL3d3dy50ZXN0aW5nLmNvbSIsImF1ZCI6Ijg4ODg4ODg4ODgifQ.j-p0f7LceNeF9J_vGVRLTtcHtSMSLzsTvL1U-mYiJCfYD6F7HkDZQyyM6jlflXMuJSIkOghZs-WMOE_xk-FgnSvJxf3QTFWEbPgZqreQxw1h8U78uPcDNAZl_69d7stITQ8hZGdpxrXOBDqwozIDv0aFLZPxIW2KflvXrXX3jZtQeD9A4l6KgbdCHcl4-PRtu8vlGF2WcY78IHOruu5OePhMcAWctbBLluGilMnMwABYbxSczwEm1gQr9yMwJPSmCD8XmdBJoooMVxN4iCHpCv19KlN_8nUd83g-HlfApMg5tixBh6ktfkdXo1BFocRQG_p-2aLSKcpgZWWL42FzBQ";

                    //Get the new token and rebuild the hubonnection
                    var reconnectToSignalR = new ConnectToSignalR();
                    reconnectToSignalR.Start(token);
                }
                else if (arg.Message.Contains("403 (Forbidden)"))
                {
                    Console.WriteLine("Stop Automatic Reconnect");
                }
                else //if server went down
                {
                    ReconnectByRebuildingConnectionHub();
                }
            }

            Console.WriteLine();

            //Clean up garbage in memory
            GC.Collect();
        }

        private async void ReconnectByRebuildingConnectionHub()
        {
            Console.WriteLine("Automatic Reconnect Initiated in 60secs");

            await Task.Delay(5000);

            //Rebuild the connection
            var reconnectToSignalR = new ConnectToSignalR();
            reconnectToSignalR.Start(token);
        }
    }
}
