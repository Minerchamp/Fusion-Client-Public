namespace Fusion.Networking
{
    using Fusion.Networking.Serializable;
    using FusionClient;
    using FC;
    using FC.Utils;
    using FusionClient.API.QM;
    using FusionClient.Core;
    #region Imports

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using UnityEngine;

    #endregion

    public class BotServer
    {
        private static readonly int maxConnections = 1000;
        internal static bool Started;
        internal static Dictionary<int, QMSingleButton> botbuttons = new Dictionary<int, QMSingleButton>();

        public static List<BotClient> Bots { get; private set; }

        public BotServer()
        {
            Console.WriteLine("Starting Bot Server..");
            Bots = new List<BotClient>();
            StartServer();
        }

        private static void StartServer()
        {
            Started = true;
            TcpListener serverSocket = new TcpListener(new IPEndPoint(IPAddress.Any, 12000));
            serverSocket.Start();
            Console.WriteLine("Bot Server Started..");

            Task task = new Task(() =>
            {
                while (true)
                {
                    TcpClient clientSocket = serverSocket.AcceptTcpClient();
                    BotClient client = new BotClient();

                    client.Connected += OnConnected;
                    client.Disconnected += OnDisconnected;
                    client.ReceivedPacket += OnReceivedPacket;

                    client.StartClient(clientSocket, GetNewClientID());
                    //MelonCoroutines.Start(PingLoop());
                }
            });
            task.Start();
        }

        private static IEnumerator PingLoop()
        {
            SendAll(new PacketData(PacketBotServerType.KEEP_ALIVE));
            yield return new WaitForSeconds(5f);
        }

        private static void ProcessInputAsync(object sender, PacketData packetData)
        {
            var networkEventID = packetData.NetworkEventID;
            BotClient client = sender as BotClient;

            //if (networkEventID != 100)
            //{
            //    ModConsole.Log($"R [{client.ClientID}] <- {packetData.NetworkEventID}: {packetData.TextData}");
            //}

            switch (networkEventID)
            {
                case PacketBotType.CONNECTED:
                    client.Send(new PacketData(PacketBotServerType.CONNECTION_FINISHED, client.ClientID.ToString()));
                    break;
                case PacketBotType.DISCONNECT:
                    client.Disconnect();
                    break;
                case PacketBotType.LEVEL_CHANGED:
                    MainThreadRunner.Enqueue(() =>
                    {
                        if (FusionClient.Modules.BotModule.Captcha)
                        {
                            Logs.Hud($"<color=cyan>FusionClient</color> | Bot [{client.ClientID}] Is Stuck On VRC Captcha");
                            Logs.Log($"Bot [{client.ClientID}] Is Stuck On VRC Captcha", ConsoleColor.Red);
                        }
                        else
                        {
                            Logs.Hud($"<color=cyan>FusionClient</color> | Bot [{client.ClientID}] Is Loading");
                            Logs.Log($"Bot [{client.ClientID}] Is Loading", ConsoleColor.Red);
                        }
                    });
                    break;
                case PacketBotType.ROOM_JOINED:
                    MainThreadRunner.Enqueue(() =>
                    {
                        Logs.Hud($"<color=cyan>FusionClient</color> | Bot [{client.ClientID}] Joined Room: {packetData.TextData}");
                    });
                    break;
                case PacketBotType.KEEP_ALIVE:
                    // No need to do anything here, we only catch this because it's a valid packet type.
                    break;
                default:
                    Console.WriteLine($"Received unknown packet type {networkEventID}");
                    break;
            }
        }

        public static void SendClient(int id, PacketData packetData)
        {
            BotServer.Bots.Where(s => s.ClientID == id).FirstOrDefault().Send(packetData);
        }

        public static void SendAll(PacketData packetData)
        {
            foreach (BotClient client in Bots.Where(b => b.IsConnected))
            {
                client.Send(packetData);
                Logs.Log($"S [{client.ClientID}] -> {packetData.NetworkEventID}: {packetData.TextData}", ConsoleColor.Cyan);
            }
        }

        private static void OnDisconnected(object sender, EventArgs e)
        {
            BotClient client = sender as BotClient;
            if (Bots.Contains(client))
            {
                Bots.Remove(client);
            }
            Console.WriteLine($"Bot {client.ClientID} Disconnected: There are now {Bots.Count} bots online");
            botbuttons.TryGetValue(client.ClientID, out QMSingleButton button);
            GameObject.Destroy(button.GetGameObject());
            botbuttons.Remove(client.ClientID);
        }

        internal static int GetNewClientID()
        {
            return Bots.Count + 1;
        }

        private static void OnConnected(object sender, EventArgs e)
        {
            BotClient client = sender as BotClient;

            if (Bots.Count < maxConnections)
            {
                if (!Bots.Contains(client))
                {
                    Bots.Add(client);
                    client.Send(new PacketData(PacketBotServerType.CONNECTED));
                }
                Console.WriteLine($"Bot {client.ClientID} Connected: There is now {Bots.Count} bots online");
                if (!botbuttons.ContainsKey(client.ClientID))
                {
                    if (client.ClientID > 3)
                    {
                        var BotButton = new QMSingleButton(UI.BotOneMovment, client.ClientID, 1, $"Bot #{client.ClientID}", delegate
                        {
                            UI.BotTarget = client.ClientID;
                        }, $"Target Bot {client.ClientID}");
                        botbuttons.Add(client.ClientID, BotButton);
                    }
                    else
                    {
                        var BotButton = new QMSingleButton(UI.BotOneMovment, client.ClientID, 0, $"Bot #{client.ClientID}", delegate
                        {
                            UI.BotTarget = client.ClientID;
                        }, $"Target Bot {client.ClientID}");
                        botbuttons.Add(client.ClientID, BotButton);
                    }
                }
            }
            else
            {
                Console.WriteLine($"Bot Connection Failed");
                client.Disconnect();
            }
        }

        private static void OnReceivedPacket(object sender, ReceivedPacketEventArgs e)
        {
            ProcessInputAsync(sender, e.Data);
        }
    }
}