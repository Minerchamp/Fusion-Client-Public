namespace Fusion.Networking
{
    public static class PacketBotType
    {
        public const byte CONNECTED = 1;
        public const byte DISCONNECT = 2;
        public const byte LEVEL_CHANGED = 10;
        public const byte ROOM_JOINED = 11;
        public const byte KEEP_ALIVE = 100;
    }

    public static class PacketBotServerType
    {
        public const byte EXIT = 0;
        public const byte CONNECTED = 1;
        public const byte DISCONNECT = 2;
        public const byte CONNECTION_FINISHED = 20;
        public const byte JOIN_WORLD = 30;
        public const byte CLICK_TP = 31;
        public const byte CHANGE_AVATAR = 32;
        public const byte SPINBOT_TOGGLE = 33;
        public const byte UNMUTE = 34;
        public const byte VOLUME = 35;
        public const byte ALIGN_CHANGE = 36;
        public const byte FOLLOW = 37;
        public const byte ORBIT = 38;
        public const byte PARROT = 39;
        public const byte STOP_FOLLOW = 40;
        public const byte MOVE_LIKE = 41;
        public const byte DEBUGTEST = 42;
        public const byte KEEP_ALIVE = 100;
        public const byte E6 = 99;
        public const byte RPC_DC_MASTER = 101;
        public const byte E1 = 79;
        public const byte E1_MIMIC = 80;
        public const byte TARGET_E7 = 70;
        public const byte TARGET = 77;
        public const byte EDIT_VALUES = 75;
        public const byte TP = 78;
        public const byte MOVEMENT = 76;
        public const byte PLACEMENT = 102;
    }
}