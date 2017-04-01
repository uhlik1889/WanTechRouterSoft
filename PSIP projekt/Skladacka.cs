using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketDotNet;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.Icmp;
using PcapDotNet.Packets.IpV4;
using Packet = PacketDotNet.Packet;

namespace PSIP_projekt
{
    class Skladacka
    {
        //ICMPv4Packet icmp = PacketDotNet.ICMPv4Packet.GetEncapsulated(eth);
        public PcapDotNet.Packets.Packet ping()
        {
            // Supposing to be on ethernet, set mac source to 01:01:01:01:01:01
            MacAddress source = new MacAddress("01:01:01:01:01:01");

            // set mac destination to 02:02:02:02:02:02
            MacAddress destination = new MacAddress("02:02:02:02:02:02");

            // Create the packets layers

            // Ethernet Layer
            EthernetLayer ethernetLayer = new EthernetLayer
            {
                Source = source,
                Destination = destination
            };

            // IPv4 Layer
            IpV4Layer ipV4Layer = new IpV4Layer
            {
                Source = new IpV4Address("1.2.3.4"),
                Ttl = 128,
                // The rest of the important parameters will be set for each packet
            };

            // ICMP Layer
            IcmpEchoLayer icmpLayer = new IcmpEchoLayer();

            // Create the builder that will build our packets
            PacketBuilder builder = new PacketBuilder(ethernetLayer, ipV4Layer, icmpLayer);

            PcapDotNet.Packets.Packet packet = builder.Build(DateTime.Now);

            return packet;
        }
    }
}
