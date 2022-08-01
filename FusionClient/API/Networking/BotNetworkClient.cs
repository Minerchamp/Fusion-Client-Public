namespace Fusion.Networking
{
    #region Imports

    using Fusion.Networking.Serializable;
    using FC;
    using MelonLoader;
    using Newtonsoft.Json;
    using System;
    using System.Collections;
    using System.IO;
    using System.Net.Sockets;
    using UnityEngine;
    using FC.Utils;
    using FC.Components;
    using FusionClient.Modules;
    using FusionClient.Core;
    using System.Linq;
    using FusionClient;

    #endregion Imports

    internal class BotNetworkClient
    {
        public static HandleBotClient Client;

        internal static void Initialize()
        {
            Logs.Log("Bot Client Connecting..", ConsoleColor.Red);

            Connect();
        }

        private static IEnumerator PingLoop()
        {
            for (; Client.IsConnected;)
            {
                Client.Send(new PacketData(PacketBotServerType.KEEP_ALIVE));
                yield return new WaitForSeconds(5f);
            }
        }

        private static void Connect()
        {
            Client = null;
            TcpClient tcpClient = new TcpClient("localhost", 12000);

            Client = new BotClient();

            Client.Connected += OnConnected;
            Client.Disconnected += OnDisconnect;
            Client.ReceivedPacket += OnPacketReceived;

            Client.StartClient(tcpClient, 0);
            //MelonCoroutines.Start(PingLoop());
            BotModule.ReceiveAction(() =>
            {
                BotModule.Spinbot = false;
            });
        }

        private static void ProcessInput(PacketData packetData)
        {
            var networkEventID = packetData.NetworkEventID;

            //if (networkEventID != PacketBotServerType.KEEP_ALIVE)
            //{
            //    Logs.Log($"R <- {packetData.NetworkEventID}: {packetData.TextData}", ConsoleColor.Red);
            //}

            switch (networkEventID)
            {
                case PacketBotServerType.EXIT:
                    Application.Quit();
                    break;

                case PacketBotServerType.CONNECTED:
                    Client.Send(new PacketData(PacketBotType.CONNECTED));
                    break;

                case PacketBotServerType.DISCONNECT:
                    Client.Disconnect();
                    break;

                case PacketBotServerType.CONNECTION_FINISHED:
                    BotNetworkingManager.IsReady = true;
                    BotModule.BotID = int.Parse(packetData.TextData);
                    break;

                case PacketBotServerType.JOIN_WORLD:
                    BotModule.ReceiveAction(() =>
                    {
                        MainThreadRunner.Enqueue(() =>
                        {
                            try
                            {
                                Logs.Log($"Going To World {packetData.TextData}", ConsoleColor.Red);
                                Logs.Hud($"Going To World {packetData.TextData}");
                                VRC.SDKBase.Networking.GoToRoom(packetData.TextData);
                            }
                            catch (Exception e)
                            {
                                Logs.Error("BotNetworkClient", e);
                            }
                        });
                    });
                    break;

                case PacketBotServerType.CLICK_TP:
                    BotModule.ReceiveAction(() =>
                    {
                        MainThreadRunner.Enqueue(() =>
                        {
                            if (PlayerUtils.GetCurrentUser() != null)
                            {
                                string[] Split = packetData.TextData.Split(':');
                                float X = float.Parse(Split[0]);
                                float Y = float.Parse(Split[1]);
                                float Z = float.Parse(Split[2]);
                                PlayerUtils.GetCurrentUser().transform.position = new Vector3(X, Y, Z);
                            }
                        });
                    });
                    break;

                case PacketBotServerType.CHANGE_AVATAR:
                    BotModule.ReceiveAction(() =>
                    {
                        MainThreadRunner.Enqueue(() =>
                        {
                            if (PlayerUtils.GetCurrentUser() != null)
                            {
                                ModulesFunctions.ChangeAvatar(packetData.TextData);
                            }
                        });
                    });
                    break;
                
                case PacketBotServerType.SPINBOT_TOGGLE:
                    BotModule.ReceiveAction(() =>
                    {
                        MainThreadRunner.Enqueue(() =>
                        {
                            BotModule.Spinbot = packetData.TextData != string.Empty;
                        });
                    });
                    break;
                
                case PacketBotServerType.UNMUTE:
                    BotModule.ReceiveAction(() =>
                    {
                        MainThreadRunner.Enqueue(() =>
                        {
                            //AudioUtils.ToggleMute();
                        });
                    });
                    break;
                
                case PacketBotServerType.VOLUME:
                    BotModule.ReceiveAction(() =>
                    {
                        MainThreadRunner.Enqueue(() =>
                        {
                            USpeaker.field_Internal_Static_Single_1 = (float.Parse(packetData.TextData));
                        });
                    });
                    break;

                case PacketBotServerType.ALIGN_CHANGE:
                    BotModule.ReceiveAction(() =>
                    {
                        if (BotModule.axismode < 13)
                        {
                            BotModule.axismode += 1;
                        }
                        else
                        {
                            BotModule.axismode = 0;
                        }
                    });
                    break;

                case PacketBotServerType.E1:
                    BotModule.ReceiveAction(() =>
                    {
                        MainThreadRunner.Enqueue(() =>
                        {
                            if (packetData.TextData == "true")
                            {
                                ModulesFunctions.earrapeExploitRunning = true;
                                MelonCoroutines.Start(ModulesFunctions.EarrapeExploit());
                            }
                            else
                            {
                                if (packetData.TextData == "false")
                                {
                                    ModulesFunctions.earrapeExploitRunning = false;
                                }
                            }
                        });
                    });
                    break;

                case PacketBotServerType.RPC_DC_MASTER:
                    BotModule.ReceiveAction(() =>
                    {
                        MainThreadRunner.Enqueue(() =>
                        {
                            if (packetData.TextData == "true")
                            {
                                ModulesFunctions.MasterDC = true;
                                MelonCoroutines.Start(ModulesFunctions.DisconnectMaster());
                            }
                            else
                            {
                                if (packetData.TextData == "false")
                                {
                                    ModulesFunctions.MasterDC = false;
                                }
                            }
                        });
                    });
                    break;

                case PacketBotServerType.E1_MIMIC:
                    BotModule.ReceiveAction(() =>
                    {
                        MainThreadRunner.Enqueue(() =>
                        {
                            BotModule.SelectedTarget = PlayerUtils.GetPlayerByUserID(packetData.TextData);
                            BotModule.VoiceMimic = true;
                            if (packetData.TextData.Contains(""))
                            {
                                BotModule.VoiceMimic = false;
                            }
                        });
                    });
                    break;

                case PacketBotServerType.FOLLOW:
                    BotModule.ReceiveAction(() =>
                    {
                        MainThreadRunner.Enqueue(() =>
                        {
                            BotModule.FollowMyGuy = true;
                            PlayerUtils.GetCurrentUser().GetComponent<VRC.Networking.FlatBufferNetworkSerializer>().enabled = false;
                            if (BotModule.FollowMyGuy)
                            {
                                BotModule.SelectedTarget = PlayerUtils.GetPlayerByUserID(packetData.TextData);
                                Logs.Log($"Following {BotModule.SelectedTarget.GetDisplayName()}", ConsoleColor.Red);
                                try
                                {
                                    ModulesFunctions.ChangeAvatar(BotModule.SelectedTarget.GetApiAvatar().id);
                                }
                                catch (Exception e)
                                {
                                    Logs.Log($"[{PlayerUtils.GetCurrentUser()._player.GetDisplayName()}] Could not clone Avatar {BotModule.SelectedTarget.GetApiAvatar().id}. Is private?", ConsoleColor.Red);
                                }
                            }
                        });
                    });
                    break;
                
                case PacketBotServerType.ORBIT:
                    BotModule.ReceiveAction(() =>
                    {
                        MainThreadRunner.Enqueue(() =>
                        {
                            BotModule.OrbitTarget = PlayerUtils.GetPlayerByUserID(packetData.TextData);
                        });
                    });
                    break;
                
                case PacketBotServerType.STOP_FOLLOW:
                    BotModule.ReceiveAction(() =>
                    {
                        MainThreadRunner.Enqueue(() =>
                        {
                            BotModule.FollowMyGuy = false;
                            BotModule.SelectedTarget = null;
                            BotModule.OrbitTarget = null;
                            BotModule.DEBUGTEST = false;
                            PlayerUtils.GetCurrentUser().GetComponent<VRC.Networking.FlatBufferNetworkSerializer>().enabled = true;
                        });
                    });
                    break;
                
                case PacketBotServerType.MOVE_LIKE:
                    BotModule.ReceiveAction(() =>
                    {
                        MainThreadRunner.Enqueue(() =>
                        {
                            var movementdata = Convert.FromBase64String(packetData.TextData);
                            byte[] viewIDOut = BitConverter.GetBytes(int.Parse(PlayerUtils.GetCurrentUser().GetVRCPlayerApi().playerId + "00001"));
                            Buffer.BlockCopy(viewIDOut, 0, movementdata, 0, 4);

                            byte[] vecbytes = BufferRW.Vector3ToBytes(BotModule.GetOffset(BufferRW.ReadVector3(movementdata, 48)));
                            Buffer.BlockCopy(vecbytes, 0, movementdata, 48, vecbytes.Length);
                            MiscUtils.OpRaiseEvent(7, movementdata, BotModule.UnreliableEventOptions, BotModule.UnreliableOptions);
                        });
                    });
                    break;

                case PacketBotServerType.DEBUGTEST:
                    BotModule.ReceiveAction(() =>
                    {
                        MainThreadRunner.Enqueue(() =>
                        {
                            BotModule.DEBUGTEST = true;
                            PlayerUtils.GetCurrentUser().GetComponent<VRC.Networking.FlatBufferNetworkSerializer>().enabled = false;
                        });
                    });
                    break;

                case PacketBotServerType.EDIT_VALUES:
                    BotModule.ReceiveAction(() =>
                    {
                        var values = Newtonsoft.Json.JsonConvert.DeserializeObject<EditValues>(packetData.TextData);
                        if (values.OrbitSpeed > 0)
                        {
                            BotModule.OrbitSpeed = values.OrbitSpeed;
                        }
                        if (values.BotDistance > 0)
                        {
                            BotModule.BotDistance = values.BotDistance;
                        }
                        if (values.SpinbotSpeed > 0)
                        {
                            BotModule.SpinbotSpeed = values.SpinbotSpeed;
                        }
                    });
                    break;

                case PacketBotServerType.TP:
                    BotModule.ReceiveAction(() =>
                    {
                        MainThreadRunner.Enqueue(() =>
                        {
                            PlayerUtils.GetCurrentUser().gameObject.transform.position = PlayerUtils.GetPlayerByUserID(packetData.TextData)._vrcplayer.gameObject.transform.position;
                        });
                    });
                    break;

                case PacketBotServerType.MOVEMENT:
                    BotModule.ReceiveAction(() =>
                    {
                        MainThreadRunner.Enqueue(() =>
                        {
                            var values = Newtonsoft.Json.JsonConvert.DeserializeObject<Movement>(packetData.TextData);
                            if (values.Move == Movement.MovementInfo.Forword)
                            {
                                MovePlayer(GameObject.Find("Camera (eye)").transform.forward * 1);
                            }
                            if (values.Move == Movement.MovementInfo.Backwards)
                            {
                                MovePlayer(GameObject.Find("Camera (eye)").transform.forward * -1);
                            }
                            if (values.Move == Movement.MovementInfo.Right)
                            {
                                MovePlayer(GameObject.Find("Camera (eye)").transform.right * 1);
                            }
                            if (values.Move == Movement.MovementInfo.Left)
                            {
                                MovePlayer(GameObject.Find("Camera (eye)").transform.right * -1);
                            }
                            if (values.Move2 == Movement.MovementInfo2.LeftAndRight)
                            {
                                BotModule.axismode = 2;
                            }
                            if (values.Move2 == Movement.MovementInfo2.Zigzag)
                            {
                                BotModule.axismode = 3;
                            }
                            if (values.Move2 == Movement.MovementInfo2.Line)
                            {
                                BotModule.axismode = 4;
                            }
                            if (values.Move2 == Movement.MovementInfo2.FrontLine)
                            {
                                BotModule.axismode = 5;
                            }
                            if (values.Move2 == Movement.MovementInfo2.AllLeftBack)
                            {
                                BotModule.axismode = 6;
                            }
                            if (values.Move2 == Movement.MovementInfo2.LineBack)
                            {
                                BotModule.axismode = 7;
                            }
                            if (values.Move2 == Movement.MovementInfo2.AllRightBack)
                            {
                                BotModule.axismode = 8;
                            }
                            if (values.Move2 == Movement.MovementInfo2.HeadStack)
                            {
                                BotModule.axismode = 9;
                            }
                            if (values.Move2 == Movement.MovementInfo2.LineRight)
                            {
                                BotModule.axismode = 10;
                            }
                            if (values.Move2 == Movement.MovementInfo2.LineLeft)
                            {
                                BotModule.axismode = 11;
                            }
                            if (values.Move2 == Movement.MovementInfo2.FrontLeftAndRight)
                            {
                                BotModule.axismode = 12;
                            }
                            if (values.Move2 == Movement.MovementInfo2.RightAndLeftBack)
                            {
                                BotModule.axismode = 13;
                            }
                        });
                    });
                    break;

                case PacketBotServerType.KEEP_ALIVE:
                    // No need to do anything here, we only catch this because it's a valid packet type.
                    break;

                case PacketBotServerType.TARGET_E7:
                    BotModule.ReceiveAction(() =>
                    {
                        if (packetData.TextData.Equals("true"))
                        {
                            BotModule.DumpNextEvent = true;
                        }
                        if (packetData.TextData.Equals("false"))
                        {
                            BotModule.DumpNextEvent = false;
                        }
                    });
                    break;

                case PacketBotServerType.TARGET:
                    BotModule.ReceiveAction(() =>
                    {
                        MainThreadRunner.Enqueue(() =>
                        {
                            BotModule.SelectedTarget = PlayerUtils.GetPlayerByUserID(packetData.TextData);
                        });
                    });
                    break;

                case PacketBotServerType.E6:
                    BotModule.ReceiveAction(() =>
                    {
                        MainThreadRunner.Enqueue(() =>
                        {
                            if (packetData.TextData == "true")
                            {
                                ModulesFunctions.Event6 = true;
                                MelonCoroutines.Start(ModulesFunctions.StartE6());
                            }
                            else
                            {
                                if (packetData.TextData == "false")
                                {
                                    ModulesFunctions.Event6 = false;
                                }
                            }
                        });
                    });
                    break;

                case PacketBotServerType.PLACEMENT:
                    BotModule.ReceiveAction(() =>
                    {
                        MainThreadRunner.Enqueue(() =>
                        {
                            if (packetData.TextData == "Reset")
                            {
                                PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(0, 0, 0);
                            }
                            if (packetData.TextData == "Midnight Top")
                            {
                                if (BotServer.Bots.Count >= 2)
                                {
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=1")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(-2.5971f, -2.9532f, 6.4015f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=2")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(5.47f, -2.6774f, 6.5422f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=3")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(1.6873f, -2.7318f, 0.0594f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=4")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(4.1626f, -1.9743f, -7.0984f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=5")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(-2.0841f, -1.3346f, -8.8021f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                }
                                else
                                {
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=1")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(2.0137f, 4.692f, 5.3955f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=2")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(1.8556f, 1.5602f, -8.808f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                }
                            }
                            if (packetData.TextData == "Midnight Bottom")
                            {
                                if (BotServer.Bots.Count >= 2)
                                {
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=1")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(6.8544f, -24.4761f, 7.2777f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=2")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(-2.8891f, -23.7914f, 7.2766f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=3")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(-2.9327f, -23.1294f, -7.8744f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=4")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(3.7055f, -26.8831f, -6.2983f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=5")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(3.0998f, -29.0715f, 1.1464f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                }
                                else
                                {
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=1")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(-2.8479f, -23.9159f, 7.2867f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=2")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(3.7378f, -23.8388f, -5.8261f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                }
                            }
                            if (packetData.TextData == "BlackCat Main")
                            {
                                if (BotServer.Bots.Count >= 2)
                                {
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=1")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(20.8987f, -3.1092f, -4.4995f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=2")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(21.0454f, -2.2333f, 11.2658f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=3")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(2.9419f, -2.3585f, -8.469f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=4")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(-3.1507f, -2.4608f, 14.9273f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=5")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(-12.1703f, -0.2001f, -23.0032f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                }
                                else
                                {
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=1")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(20.8987f, -3.1092f, -4.4995f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=2")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(21.0454f, -2.2333f, 11.2658f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                }
                            }
                            if (packetData.TextData == "BlackCat Top")
                            {
                                if (BotServer.Bots.Count >= 2)
                                {
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=1")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(32.5475f, 1.0156f, -18.484f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=2")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(16.9041f, 6.7099f, -32.9345f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=3")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(37.3079f, 6.7099f, -33.4044f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=4")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(37.26f, 6.7099f, -7.6421f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=5")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(24.3793f, 14.0949f, -16.3509f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                }
                                else
                                {
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=1")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(32.5475f, 1.0156f, -18.484f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=2")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(20.3197f, 1.3657f, -30.6809f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                }
                            }
                            if (packetData.TextData == "Rain")
                            {
                                if (BotServer.Bots.Count >= 2)
                                {
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=1")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(9.9569f, -4.1775f, 2.5177f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=2")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(10.2155f, -4.4255f, -14.1955f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=3")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(-5.5795f, -4.6015f, -13.8254f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=4")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(-7.5189f, -5.7367f, 2.5991f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=5")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(2.3942f, 10.5486f, -7.3941f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                }
                                else
                                {
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=1")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(-8.001f, -15.54f, 21.7897f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                    if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=2")))
                                    {
                                        PlayerUtils.GetCurrentUser().gameObject.transform.position = new Vector3(-9.3107f, -9.4371f, 23.8386f);
                                        PlayerUtils.GetCurrentUser().gameObject.GetComponent<GamelikeInputController>().enabled = false;
                                    }
                                }
                            }
                        });
                    });
                    break;

                default:
                    break;
            }
        }

        private static void MovePlayer(Vector3 pos)
        {
            if (PlayerUtils.GetCurrentUser() != null)
                PlayerUtils.GetCurrentUser().transform.position += pos;
        }

        private static void OnConnected(object sender, EventArgs e)
        {
            Logs.Log("Bot Connected.", ConsoleColor.Red);
            return;
        }

        private static void OnDisconnect(object sender, EventArgs e)
        {
            BotModule.ReceiveAction(() =>
            {
                BotModule.Spinbot = true;
            });
            MelonCoroutines.Start(TryReconnect());
        }

        private static IEnumerator TryReconnect()
        {
            for (; ; )
            {
                try
                {
                    Connect();
                    yield break;
                }
                catch { }

                Logs.Log("Bot reconnection attempt in 5 seconds..", ConsoleColor.Cyan);
                yield return new WaitForSeconds(5f);
            }
        }

        private static void OnPacketReceived(object sender, ReceivedPacketEventArgs e)
        {
            ProcessInput(e.Data);
        }
    }
}