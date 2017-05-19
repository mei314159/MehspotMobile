using System;
namespace Mehspot.Core
{
    public class Constants
    {
#if DEBUG
        public const string ApiHost = "http://10.211.55.3:59483";
#else
        public const string ApiHost = "http://mehspot.com";
        //public const string ApiHost = "http://mehspot-dev.mehspot.com";
        //public const string ApiHost = "http://192.168.0.105";
#endif

    }
}
