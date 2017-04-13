using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using PacketDotNet;
using PacketDotNet.Utils;
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
        public EthernetPacket ping(String macCiel, String macZdroj, String ipecka, String ipeckaOdkial)
        {
            // Supposing to be on ethernet, set mac source to 01:01:01:01:01:01
            string sourcik = string.Join(":", (from z in PhysicalAddress.Parse(macZdroj).GetAddressBytes() select z.ToString("X2")).ToArray());
            MacAddress source = new MacAddress(sourcik);

            string destinationik = string.Join(":", (from z in PhysicalAddress.Parse(macCiel).GetAddressBytes() select z.ToString("X2")).ToArray());
            // set mac destination to 02:02:02:02:02:02
            MacAddress destination = new MacAddress(destinationik);

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
                Source = new IpV4Address(ipeckaOdkial),     // source IP address
                Ttl = 128,
                // The rest of the important parameters will be set for each packet
            };

            // ICMP Layer
            IcmpEchoLayer icmpLayer = new IcmpEchoLayer();

            // Create the builder that will build our packets
            PacketBuilder builder = new PacketBuilder(ethernetLayer, ipV4Layer, icmpLayer);

            ipV4Layer.CurrentDestination = new IpV4Address(ipecka);   
            ipV4Layer.Identification = 0;

            // Set ICMP parameters
            icmpLayer.SequenceNumber = 0;
            icmpLayer.Identifier = 0;

            // Build the packet
            EthernetPacket packet = new EthernetPacket(new ByteArraySegment(builder.Build(DateTime.Now).Buffer));           

            return packet;
        }

        public EthernetPacket pingReply(String macCiel, String macZdroj, String ipecka, String ipeckaOdkial, byte[] data, ushort sequence, ushort identifier)
        {
            // Supposing to be on ethernet, set mac source to 01:01:01:01:01:01
            string sourcik = string.Join(":", (from z in PhysicalAddress.Parse(macZdroj).GetAddressBytes() select z.ToString("X2")).ToArray());
            MacAddress source = new MacAddress(sourcik);

            string destinationik = string.Join(":", (from z in PhysicalAddress.Parse(macCiel).GetAddressBytes() select z.ToString("X2")).ToArray());
            // set mac destination to 02:02:02:02:02:02
            MacAddress destination = new MacAddress(destinationik);

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
                Source = new IpV4Address(ipeckaOdkial),     // source IP address
                Ttl = 128,
                // The rest of the important parameters will be set for each packet
            };

            // ICMP Layer
            IcmpEchoReplyLayer icmpLayer = new IcmpEchoReplyLayer();

            PayloadLayer payload = new PayloadLayer();
            payload.Data = new Datagram(data);

            // Create the builder that will build our packets
            PacketBuilder builder = new PacketBuilder(ethernetLayer, ipV4Layer, icmpLayer, payload);

            ipV4Layer.CurrentDestination = new IpV4Address(ipecka);
            ipV4Layer.Identification = 0;

            // Set ICMP parameters
            icmpLayer.SequenceNumber = sequence;
            icmpLayer.Identifier = identifier;

            // Build the packet
            EthernetPacket packet = new EthernetPacket(new ByteArraySegment(builder.Build(DateTime.Now).Buffer));

            return packet;
        }
    }
}
