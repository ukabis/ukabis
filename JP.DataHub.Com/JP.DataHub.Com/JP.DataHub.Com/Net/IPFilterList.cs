using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Unity;
using NetTools;

namespace JP.DataHub.Com.Net
{
    public class IPFilterList
    {
        private Dictionary<string, IPAddressRange> _ipNetworks = new Dictionary<string, IPAddressRange>();
        private List<string> _allowIps = new List<string>();

        public IPFilterList()
        {
            var allowIpAdress = UnityCore.ResolveOrDefault<Hashtable>("allowIpAdressSection") as Hashtable;
            if (allowIpAdress != null)
            {
                foreach (var key in allowIpAdress.Keys)
                {
                    if (_ipNetworks.ContainsKey(key.ToString()))
                    {
                        _ipNetworks[key.ToString()] = IPAddressRange.Parse(allowIpAdress[key].ToString());
                    }
                    else
                    {
                        _ipNetworks.Add(key.ToString(), IPAddressRange.Parse(allowIpAdress[key].ToString()));
                    }
                }
            }
        }

        public IPFilterList(List<string> allowIps) : this()
        {
            this._allowIps = allowIps;
        }

        public bool Contain(string key, IPAddress address)
        {
            //IPV6の場合ははじく
            if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                return false;
            }

            if (_ipNetworks.ContainsKey(key))
            {
                return _ipNetworks[key].Contains(address);
            }
            return true;
        }

        public bool Contain(IPAddress address)
        {
            //IPV6の場合ははじく
            if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                return false;
            }

            if (_allowIps.Count <= 0 && _ipNetworks.Count <= 0)
            {
                return true;
            }
            foreach (var allowIp in _allowIps)
            {
                if (IPAddressRange.Parse(allowIp).Contains(address))
                {
                    return true;
                }
            }
            foreach (var key in _ipNetworks.Keys)
            {
                if (Contain(key, address))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Contain(string address, string filterAddress)
        {
            var ipaddress = IPAddress.Parse(address);
            //IPV6の場合ははじく
            if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                return false;
            }
            return IPAddressRange.Parse(filterAddress).Contains(ipaddress);
    }
}
}
