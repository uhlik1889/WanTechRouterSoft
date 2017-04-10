using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using PacketDotNet;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.Icmp;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;

namespace PSIP_projekt
{
    class RIP
    {
        public void posliRIP(String macCiel, String macZdroj, String ipecka, String ipeckaOdkial)
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

            //UdpPacket udp = new UdpPacket(520,520);

            UdpLayer udp = new UdpLayer();
            udp.DestinationPort = 520;
            udp.SourcePort = 520;

            PayloadLayer siete = new PayloadLayer();

               // siete.Data = new Datagram(//bytove pole plne sieti);

            // Create the builder that will build our packets
            PacketBuilder builder = new PacketBuilder(ethernetLayer, ipV4Layer, udp, siete);

            ipV4Layer.CurrentDestination = new IpV4Address(ipecka);
            ipV4Layer.Identification = 0;


            //// prilepit udp.payload s routovacou tabulkou
            /// 
            /// spravit aby funkcia vracala bytove pole ktore bude obsahovat sparsovany jeden riadok z routovacej tabulky
            /// v cykle ich potom pospajam a pridelim ako payload sem :D 
        }

        public void spracujRIPRequest() { }


    }
}
