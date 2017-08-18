#if NET452
#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    internal static class DateTimeOffsetExtensions
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long ToUnixTimeSeconds(this DateTimeOffset dto) {
            return (long)(dto.UtcDateTime - UnixEpoch).TotalSeconds;
        }

        public static long ToUnixTimeMilliseconds(this DateTimeOffset dto)
        {
            return (long)(dto.UtcDateTime - UnixEpoch).TotalMilliseconds;
        }        
    }
}
#endif