namespace WheelDemo.Core
{
    public static class ZoneService
    {
        private const int SafeZoneInterval = 5;
        private const int SuperZoneInterval = 30;

        public static ZoneType GetZoneType(int zoneNumber)
        {
            if (zoneNumber <= 0)
            {
                return ZoneType.Normal;
            }

            if (zoneNumber % SuperZoneInterval == 0)
            {
                return ZoneType.Super;
            }

            if (zoneNumber % SafeZoneInterval == 0)
            {
                return ZoneType.Safe;
            }

            return ZoneType.Normal;
        }
    }
}
