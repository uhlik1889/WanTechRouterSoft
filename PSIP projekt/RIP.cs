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
using System.Data;
using System.Net;
using Knom.Helpers.Net;
using PacketDotNet.Utils;

namespace PSIP_projekt
{
    class RIP
    {
        private Control hlavnaforma = null;
        private FiltrovaciaObrazovka filtrovaciaobrazovkaforma = null;
        private Filtrovanie filtracia = null;
        public RIP(Control form, FiltrovaciaObrazovka form2)                 /********************************************/
        {
            hlavnaforma = form;
            filtrovaciaobrazovkaforma = form2;
            filtracia = new Filtrovanie(form2);
        }
        public EthernetPacket posielanieRIP(String macCiel, String macZdroj, String ipeckaCiel, String ipeckaOdkial, String ipeckaDruhyInterface, String macDruyhInterface, Int32 portkablu)
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

               // siete.Data = new Datagram(//bytove pole plne sieti);

            ipV4Layer.CurrentDestination = new IpV4Address(ipeckaCiel);
            ipV4Layer.Identification = 0;

            // vyhladaj v tabulke to rozhranie ktore chcem + to samotne rozhranie 

            List<int> indexes = hlavnaforma.indexiRoutovacejTabulkyPreOperacie( 
                System.Net.IPAddress.Parse(ipeckaDruhyInterface), 
                System.Net.IPAddress.Parse(macDruyhInterface),
                portkablu
                );
            indexes = indexes.Distinct().ToList();
            indexes.Sort();

            byte[] ripcasthlavicka = new byte[] { 0x02, 0x02, 0x00, 0x00 };                                
            
            
            byte[] siete = new byte[indexes.Count*20+4];  ////////////// !!!!!! viem velkost
            int a = 0;
            System.Buffer.BlockCopy(ripcasthlavicka, 0, siete, 0, ripcasthlavicka.Length);
            foreach (int i in indexes)
            {    
                byte[] dalsiasiet = dajDalsiaSietVBajtoch(i);
                System.Buffer.BlockCopy(dalsiasiet, 0, siete, (4+(a*20)), dalsiasiet.Length);
                a++;
            }

            PayloadLayer sietePL = new PayloadLayer
            {
                Data = new Datagram(siete)
            };

            // Create the builder that will build our packets
            PacketBuilder builder = new PacketBuilder(ethernetLayer, ipV4Layer, udp, sietePL);
            // vytvorit bajtove pole do ktoreho budem pridavat cyklom
            // pri kazdom cykle pridam na koniec pola novy retazec ktory mi vrati funkcia
            // funkcia vracia po zadani bajtove pole ktore reprezentuje jeden riadok v routovacej tabulke
            // na riadok ukazuje index z funkcia IVKA

            EthernetPacket packet = new EthernetPacket(new ByteArraySegment(builder.Build(DateTime.Now).Buffer));

            return packet;


            //// prilepit udp.payload s routovacou tabulkou
            /// 
            /// spravit aby funkcia vracala bytove pole ktore bude obsahovat sparsovany jeden riadok z routovacej tabulky
            /// v cykle ich potom pospajam a pridelim ako payload sem :D 
        }

        // posion
        public EthernetPacket posielanieRIPSingle(String macCiel, String macZdroj, String ipeckaCiel, String ipeckaOdkial, DataRow riadok)
        {
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

            // siete.Data = new Datagram(//bytove pole plne sieti);

            ipV4Layer.CurrentDestination = new IpV4Address(ipeckaCiel);
            ipV4Layer.Identification = 0;

            ////////////////////////////////////////////////////////////////////////////////////////////
            byte[] ripcasthlavicka = new byte[] { 0x02, 0x02, 0x00, 0x00 };
            byte[] siete = new byte[24];
            System.Buffer.BlockCopy(ripcasthlavicka, 0, siete, 0, ripcasthlavicka.Length);
            byte[] dalsiasiet = dajDalsiaSietVBajtochPoison(riadok);
            System.Buffer.BlockCopy(dalsiasiet, 0, siete, (4), dalsiasiet.Length);
            PayloadLayer sietePL = new PayloadLayer
            {
                Data = new Datagram(siete)
            };
            ////////////////////////////////////////////////////////////////////////////////////////////
            PacketBuilder builder = new PacketBuilder(ethernetLayer, ipV4Layer, udp, sietePL);
            EthernetPacket packet = new EthernetPacket(new ByteArraySegment(builder.Build(DateTime.Now).Buffer));
            return packet;
        }

        public EthernetPacket posielanieRIPSingle(String macCiel, String macZdroj, String ipeckaCiel, String ipeckaOdkial, DataRow riadok, Boolean TU)
        {
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

            // siete.Data = new Datagram(//bytove pole plne sieti);

            ipV4Layer.CurrentDestination = new IpV4Address(ipeckaCiel);
            ipV4Layer.Identification = 0;

            ////////////////////////////////////////////////////////////////////////////////////////////
            byte[] ripcasthlavicka = new byte[] { 0x02, 0x02, 0x00, 0x00 };
            byte[] siete = new byte[24];
            System.Buffer.BlockCopy(ripcasthlavicka, 0, siete, 0, ripcasthlavicka.Length);
            byte[] dalsiasiet = dajDalsiaSietVBajtochPoison(riadok, TU);
            System.Buffer.BlockCopy(dalsiasiet, 0, siete, (4), dalsiasiet.Length);
            PayloadLayer sietePL = new PayloadLayer
            {
                Data = new Datagram(siete)
            };
            ////////////////////////////////////////////////////////////////////////////////////////////
            PacketBuilder builder = new PacketBuilder(ethernetLayer, ipV4Layer, udp, sietePL);
            EthernetPacket packet = new EthernetPacket(new ByteArraySegment(builder.Build(DateTime.Now).Buffer));
            return packet;
        }

        private byte[] dajDalsiaSietVBajtoch(Int32 i)
        {            
            DataRow riadok = hlavnaforma.routingtabulka.Rows[i];
            /*if (riadok.IsNull(hlavnaforma.routmetrika) || 
                (((int)riadok["Metrika"] != 16) && 
                (riadok["Lokacia"].ToString().Equals("ROUT+RIP") 
                || riadok["Lokacia"].ToString().Equals("")))
                )
            {*/
                IPAddress adresa = IPAddress.Parse((string)riadok["Siet"]);
                byte[] ipaddress = adresa.GetAddressBytes();

                IPAddress maska = IPAddress.Parse((string)riadok["Maska"]);
                byte[] netmask = maska.GetAddressBytes();
                IPAddress nexthopik = null;
                if (riadok["NextHop"].Equals("null"))
                {
                    nexthopik = IPAddress.Parse("0.0.0.0");
                }
                else
                {
                    nexthopik = IPAddress.Parse("0.0.0.0");
                }
                byte[] nexthop = nexthopik.GetAddressBytes();
                byte[] metrikaB;
                if (riadok.IsNull(hlavnaforma.routmetrika))
                {
                    metrikaB = BitConverter.GetBytes(1);
                }
                else
                {
                    int metrika = (int)riadok["Metrika"];
                    metrikaB = BitConverter.GetBytes(metrika == 16 ? 16 : ++metrika);
                }

                byte[] celasiet = new Byte[ipaddress.Length + netmask.Length + nexthop.Length + metrikaB.Length + 4];

                byte[] ripcastsiet = new byte[] { 0x00, 0x02, 0x00, 0x00 };
                System.Buffer.BlockCopy(ripcastsiet, 0, celasiet, 0, ripcastsiet.Length);
                System.Buffer.BlockCopy(ipaddress, 0, celasiet, 4, ripcastsiet.Length);
                System.Buffer.BlockCopy(netmask, 0, celasiet, 8, ripcastsiet.Length);
                System.Buffer.BlockCopy(nexthop, 0, celasiet, 12, ripcastsiet.Length);
                System.Buffer.BlockCopy(metrikaB, 0, celasiet, 19, 1);
                return celasiet;
            /*}
            return null;*/
        }

        private byte[] dajDalsiaSietVBajtochPoison(DataRow riadok, Boolean TU)
        {            
            
                IPAddress adresa = IPAddress.Parse((string)riadok["Siet"]);
                byte[] ipaddress = adresa.GetAddressBytes();

                IPAddress maska = IPAddress.Parse((string)riadok["Maska"]);
                byte[] netmask = maska.GetAddressBytes();
                IPAddress nexthopik = null;
                if (riadok["NextHop"].Equals("null"))
                {
                    nexthopik = IPAddress.Parse("0.0.0.0");
                }
                else
                {
                    nexthopik = IPAddress.Parse("0.0.0.0");
                }
                byte[] nexthop = nexthopik.GetAddressBytes();
                byte[] metrikaB;
                int metrika;                
                    if (TU)
                    {
                        metrikaB = BitConverter.GetBytes(1);                        
                    }
                    else
                    {
                        metrika = (int)riadok["Metrika"];
                        metrikaB = BitConverter.GetBytes(metrika);
                    }
                    
                

                byte[] celasiet = new Byte[ipaddress.Length + netmask.Length + nexthop.Length + metrikaB.Length + 4];

                byte[] ripcastsiet = new byte[] { 0x00, 0x02, 0x00, 0x00 };
                System.Buffer.BlockCopy(ripcastsiet, 0, celasiet, 0, ripcastsiet.Length);
                System.Buffer.BlockCopy(ipaddress, 0, celasiet, 4, ripcastsiet.Length);
                System.Buffer.BlockCopy(netmask, 0, celasiet, 8, ripcastsiet.Length);
                System.Buffer.BlockCopy(nexthop, 0, celasiet, 12, ripcastsiet.Length);
                System.Buffer.BlockCopy(metrikaB, 0, celasiet, 19, 1);
                return celasiet;
            }

        private byte[] dajDalsiaSietVBajtochPoison(DataRow riadok)
        {

            IPAddress adresa = IPAddress.Parse((string)riadok["Siet"]);
            byte[] ipaddress = adresa.GetAddressBytes();

            IPAddress maska = IPAddress.Parse((string)riadok["Maska"]);
            byte[] netmask = maska.GetAddressBytes();
            IPAddress nexthopik = null;
            if (riadok["NextHop"].Equals("null"))
            {
                nexthopik = IPAddress.Parse("0.0.0.0");
            }
            else
            {
                nexthopik = IPAddress.Parse("0.0.0.0");
            }
            byte[] nexthop = nexthopik.GetAddressBytes();
            byte[] metrikaB;
            int metrika;
            
                metrikaB = BitConverter.GetBytes(16);
            

            byte[] celasiet = new Byte[ipaddress.Length + netmask.Length + nexthop.Length + metrikaB.Length + 4];

            byte[] ripcastsiet = new byte[] { 0x00, 0x02, 0x00, 0x00 };
            System.Buffer.BlockCopy(ripcastsiet, 0, celasiet, 0, ripcastsiet.Length);
            System.Buffer.BlockCopy(ipaddress, 0, celasiet, 4, ripcastsiet.Length);
            System.Buffer.BlockCopy(netmask, 0, celasiet, 8, ripcastsiet.Length);
            System.Buffer.BlockCopy(nexthop, 0, celasiet, 12, ripcastsiet.Length);
            System.Buffer.BlockCopy(metrikaB, 0, celasiet, 19, 1);
            return celasiet;
        }

        public DataRow[] GetRipResponse(byte[] ripBytes, Int32 portkablu, IPAddress nextHop)
        {
            DataRow[] zaznamy = null;
            if ((ripBytes.Length - 4) % 20 == 0)
            {
                ripBytes = ripBytes.ReadBytes(4, ripBytes.Length - 4);
                int count = ripBytes.Length / 20;
                zaznamy = new DataRow[count];
                for (int i = 0; i < count; i++)
                {
                    byte[] net = new byte[4];
                    Array.Copy(ripBytes, 20 * i + 4, net, 0, 4);
                    byte[] mask = new byte[4];
                    Array.Copy(ripBytes, 20 * i + 8, mask, 0, 4);
                    byte metric = ripBytes[20 * i + 19];

                    DataRow riadok = hlavnaforma.routingtabulka.NewRow();
                    riadok["Siet"] = translateBytetoIP(net);
                    riadok["Maska"] = translateBytetoIP(mask);
                    riadok["Typ"] = 'R';
                    riadok["NextHop"] = nextHop.ToString();
                    riadok["Interface"] = portkablu;
                    riadok["Metrika"] = metric;
                    riadok["Invalid+Flush"] = 180;
                    riadok["Lokacia"] = "Rout + RIP";

                    zaznamy[i] = riadok;
                }
                return zaznamy;
            }
            return null;
        }

        private IPAddress translateBytetoIP(byte[] ip)
        {
            string ipString = ip[0].ToString() + "." + ip[1].ToString() + "." + ip[2].ToString() + "." +
                              ip[3].ToString();
            return IPAddress.Parse(ipString);
        }
    }
}
