using System.Threading;

namespace Fusion.Networking
{
    internal class BotNetworkingManager
    {
        /// <summary>
        /// Gets whether the BotNetworkingManager has initialized
        /// </summary>
        internal static bool Initialized { get; private set; }

        /// <summary>
        /// Returns true when everything is received from the server
        /// </summary>
        internal static bool IsReady
        {
            get
            {
                if (!initialized)
                {
                    System.Random rnd = new System.Random();
                    int Time = rnd.Next(700, 2500);
                    Thread.Sleep(Time);
                    initialized = true;
                    BotNetworkClient.Initialize();
                }

                return isReady;
            }
            set => isReady = value;
        }

        private static bool isReady;
        private static bool initialized;

        internal static string Name = string.Empty;

        internal static string UserID = string.Empty;
    }
}