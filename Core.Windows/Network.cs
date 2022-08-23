using System.Net.NetworkInformation;

namespace Jay.Windows
{
    /// <summary>
    /// Network related Utilities
    /// </summary>
    public static class Network
    {
        private static readonly string[] _virtualNames = {"virtual", "vmware"};

        /// <summary>
        /// Suggested minimum network speed
        /// </summary>
        public const long MINIMUM_SPEED = 10_000_000;

        /// <summary>
        /// Is there a valid, open Network Connection available?
        /// Filters connections below a specified speed, as well as virtual network cards.
        /// </summary>
        /// <param name="minimumSpeed"></param>
        /// <returns></returns>
        public static bool IsNetworkAvailable(long minimumSpeed = MINIMUM_SPEED)
        {
            //Check if we have any networks available
            if (!NetworkInterface.GetIsNetworkAvailable())
                return false;

            //Check the interfaces
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                //Discard for standard reasons
                if (ni.OperationalStatus != OperationalStatus.Up ||
                    ni.NetworkInterfaceType == NetworkInterfaceType.Loopback ||
                    ni.NetworkInterfaceType == NetworkInterfaceType.Tunnel)
                {
                    continue;
                }

                //Filter modems, serial modems, etc
                if (ni.Speed < minimumSpeed)
                    continue;

                //Discard virtual cards
                if (_virtualNames.Any(n => ni.Description.Contains(n, StringComparison.OrdinalIgnoreCase) ||
                                           ni.Name.Contains(n, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                //Discard 'Microsoft Loopback Adapter'
                if (ni.Description.Equals("Microsoft Loopback Adapter", StringComparison.OrdinalIgnoreCase))
                    continue;
				
                //Okay, this one qualifies
                return true;
            }

            //We didn't find any
            return false;
        }
    }
}