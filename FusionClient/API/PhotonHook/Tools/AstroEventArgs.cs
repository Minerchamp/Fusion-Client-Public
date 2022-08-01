namespace FusionClient.AstroEventArgs
{
    using System;
    using Photon.Realtime;
    internal class PhotonPlayerEventArgs : EventArgs
    {
        internal Player player;
        internal PhotonPlayerEventArgs(Player player)
        {
            this.player = player;
        }
    }
}