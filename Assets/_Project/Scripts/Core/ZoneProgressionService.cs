using WheelDemo.Data;

namespace WheelDemo.Core
{
    public sealed class ZoneProgressionService
    {
        private readonly ZoneConfiguration configuration;

        public int CurrentZone { get; private set; }

        public ZoneProgressionService(
            ZoneConfiguration configuration,
            int startingZone = 1
        )
        {
            this.configuration = configuration;
            CurrentZone = startingZone < 1 ? 1 : startingZone;
        }

        public void Advance()
        {
            CurrentZone++;
        }

        public void Reset()
        {
            CurrentZone = 1;
        }

        public bool TryGetCurrentDefinition(out ZoneDefinition definition)
        {
            definition = null;

            return configuration != null &&
                configuration.TryGetDefinition(CurrentZone, out definition);
        }
    }
}
