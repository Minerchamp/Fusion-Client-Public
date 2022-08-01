namespace FusionLoader
{
    public class AssemblyResponse
    {
        public byte[] Assembly { get; private set; }
        public Response Response { get; private set; }
        public string Message { get; private set; }
        public string BanReason { get; private set; }

        public static AssemblyResponse Error(string message)
        {
            return new AssemblyResponse
            {
                Assembly = null,
                Response = Response.Error,
                Message = message,
                BanReason = null
            };
        }

        public static AssemblyResponse OK(byte[] assembly, string message)
        {
            return new AssemblyResponse
            {
                Assembly = assembly,
                Response = Response.OK,
                Message = message,
                BanReason = null
            };
        }

        public static AssemblyResponse Banned(string message, string reason)
        {
            return new AssemblyResponse
            {
                Assembly = null,
                BanReason = reason,
                Message = message,
                Response = Response.Banned
            };
        }

        public static AssemblyResponse Outdated()
        {
            return new AssemblyResponse
            {
                Assembly = null,
                BanReason = null,
                Message = null,
                Response = Response.Outdated
            };
        }

        public static AssemblyResponse HWID()
        {
            return new AssemblyResponse
            {
                Assembly = null,
                BanReason = null,
                Message = null,
                Response = Response.HWID
            };
        }
    }

    public enum Response
    {
        Error,
        OK,
        Banned,
        Outdated,
        HWID,
    }
}
