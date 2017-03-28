using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PSIP_projekt
{
    public class Filter
    {
        public int Icmp = 0;
        public int Tcp = 0;
        public int Arp = 0;
        public int Udp = 0;
        public int Ip = 0;

        public string DSTMacAdresa = "";
        public string DSTIpAdresa = "";

        public string SRCMacAdresa = "";
        public string SRCIpAdresa = "";

        public int DSTPort = 0;
        public int SRCPort = 0;
        public int Interface = 0;   //-

        // Permit = true povolene, false nepovolene, takze ako keby deny = true
        public bool Permit = false;
       
        // In = true smer in, ked flase, to iste ako keby Out = true
        public bool In = false;   //-
    }
}

