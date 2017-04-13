using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Knom.Helpers.Net;
using PacketDotNet;
using SharpPcap;
using PcapDotNet;
using PcapDotNet.Packets.IpV6;
using SharpPcap.WinPcap;


namespace PSIP_projekt
{
    public class kablovanie
    {
        private WinPcapDevice port2device = null;
        private WinPcapDevice port1device = null;
        private System.Object lockThis2 = new System.Object();
        private System.Object lockThis1 = new System.Object();
        Skladacka pingskladanie = new Skladacka();
       
        private Control hlavnaforma = null;
        private FiltrovaciaObrazovka filtrovaciaobrazovkaforma = null;
        private Filtrovanie filtracia = null;

        public kablovanie(Control form, FiltrovaciaObrazovka form2)                 /********************************************/
        {
            hlavnaforma = form;
            filtrovaciaobrazovkaforma = form2;
            filtracia = new Filtrovanie(form2);
        }

        // Retrieve the device list
        CaptureDeviceList devices = CaptureDeviceList.Instance;
        
        public void Ukazkarty(Control f)
        {

            // ked ziadne devices hod error
            if (devices.Count < 1)
            {
                f.UpdateText( "\n" + "No devices were found on this machine");
                return;
            }

            f.UpdateText("\n" + "The following devices are available on this machine:");
            f.UpdateText("----------------------------------------------------" + "\n");

            int kolkaty = 0;

            // Vypis devices a pripis ku nim int ako poradove cislo
            foreach (ICaptureDevice dev in devices)
                f.UpdateText("{0}  " + kolkaty++ + "   " + dev.Description.ToString() + "\n" + "\n" + "\n");

                      
        }

        public void zapniInterface1(Control f, int pc1)
        {
            WinPcapDeviceList deviceswinp = WinPcapDeviceList.Instance;
            int readTimeoutMilliseconds = 100;
            if (port1device!= null && port1device.Opened && hlavnaforma.beziport1)
            {

                DataRow[] riadok = hlavnaforma.routingtabulka.Select("Interface = 1 and Typ = 'C'");
                if (hlavnaforma.bezirip2)
                {
                    hlavnaforma.posliRIPPoison(2, riadok[0]);
                }
                hlavnaforma.routingtabulka.Rows.Remove(riadok[0]);
                                 
                port1device = deviceswinp[pc1];
                port1device.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrivalpc1);

                DataRow row1 = hlavnaforma.routingtabulka.NewRow();
                row1["Siet"] = IPAddressExtensions.GetNetworkAddress(IPAddress.Parse(hlavnaforma.port1IP()),
                    IPAddress.Parse(hlavnaforma.port1MASKA()));
                row1["Maska"] = hlavnaforma.port1MASKA();
                row1["Typ"] = 'C';
                row1["NextHop"] = "null";
                row1["Interface"] = 1;
                hlavnaforma.routingtabulka.Rows.Add(row1);

                hlavnaforma.macnastav(port1device.MacAddress.ToString(), 1);
                //hlavnaforma.bezirip1 = true;
                hlavnaforma.beziport1 = true;
                //gratious
                PosliARPRequest(hlavnaforma.port1IP(), hlavnaforma.port1IP(), 1);
                DataRow[] riadoknovy = hlavnaforma.routingtabulka.Select("Interface = 1 and Typ = 'C'");
                if(hlavnaforma.bezirip1)
                    hlavnaforma.posliRIPPoison(2, riadoknovy[0], true);


            }
            else
            {
                port1device = deviceswinp[pc1];
                port1device.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrivalpc1);

                port1device.Open(OpenFlags.NoCaptureLocal | OpenFlags.Promiscuous, readTimeoutMilliseconds);
                port1device.StartCapture();

                DataRow row1 = hlavnaforma.routingtabulka.NewRow();
                row1["Siet"] = IPAddressExtensions.GetNetworkAddress(IPAddress.Parse(hlavnaforma.port1IP()),
                    IPAddress.Parse(hlavnaforma.port1MASKA()));
                row1["Maska"] = hlavnaforma.port1MASKA();
                row1["Typ"] = 'C';
                row1["NextHop"] = "null";
                row1["Interface"] = 1;
                hlavnaforma.routingtabulka.Rows.Add(row1);

                hlavnaforma.macnastav(port1device.MacAddress.ToString(), 1);
                //hlavnaforma.bezirip1 = true;
                hlavnaforma.beziport1 = true;
                //gratious
                PosliARPRequest(hlavnaforma.port1IP(), hlavnaforma.port1IP(), 1);
                DataRow[] riadoknovy = hlavnaforma.routingtabulka.Select("Interface = 1 and Typ = 'C'");
                if (hlavnaforma.bezirip1)
                    hlavnaforma.posliRIPPoison(2, riadoknovy[0], true);
            } 
        }

        public void zapniInterface2(Control f, int pc2)
        {
            WinPcapDeviceList deviceswinp = WinPcapDeviceList.Instance;
            int readTimeoutMilliseconds = 100;
            if (hlavnaforma.beziport2 && port2device != null && port2device.Opened)
            {                

                DataRow[] riadok = hlavnaforma.routingtabulka.Select("Interface = 2 and Typ = 'C'");
                if (hlavnaforma.bezirip1)
                {
                    hlavnaforma.posliRIPPoison(1, riadok[0]);
                }
                hlavnaforma.routingtabulka.Rows.Remove(riadok[0]);

                port2device = deviceswinp[pc2];
                port2device.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrivalpc2);
                
                DataRow row1 = hlavnaforma.routingtabulka.NewRow();
                row1["Siet"] = IPAddressExtensions.GetNetworkAddress(IPAddress.Parse(hlavnaforma.port2IP()),
                    IPAddress.Parse(hlavnaforma.port2MASKA()));
                row1["Maska"] = hlavnaforma.port2MASKA();
                row1["Typ"] = 'C';
                row1["NextHop"] = "null";
                row1["Interface"] = 2;
                hlavnaforma.routingtabulka.Rows.Add(row1);

                hlavnaforma.macnastav(port2device.MacAddress.ToString(), 2);
                //hlavnaforma.bezirip2 = true;
                hlavnaforma.beziport2 = true;
                //gratious
                PosliARPRequest(hlavnaforma.port2IP(), hlavnaforma.port2IP(), 2);
                DataRow[] riadoknovy = hlavnaforma.routingtabulka.Select("Interface = 2 and Typ = 'C'");
                if (hlavnaforma.bezirip2)
                    hlavnaforma.posliRIPPoison(1, riadoknovy[0], true);

            }
            else
            {
                port2device = deviceswinp[pc2];
                port2device.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrivalpc2);

                port2device.Open(OpenFlags.NoCaptureLocal | OpenFlags.Promiscuous, readTimeoutMilliseconds);
                port2device.StartCapture();

                DataRow row1 = hlavnaforma.routingtabulka.NewRow();
                row1["Siet"] = IPAddressExtensions.GetNetworkAddress(IPAddress.Parse(hlavnaforma.port2IP()),
                    IPAddress.Parse(hlavnaforma.port2MASKA()));
                row1["Maska"] = hlavnaforma.port2MASKA();
                row1["Typ"] = 'C';
                row1["NextHop"] = "null";
                row1["Interface"] = 2;
                hlavnaforma.routingtabulka.Rows.Add(row1);

                hlavnaforma.macnastav(port2device.MacAddress.ToString(), 2);
                //hlavnaforma.bezirip2 = true;
                hlavnaforma.beziport2 = true;
                //gratious
                PosliARPRequest(hlavnaforma.port2IP(), hlavnaforma.port2IP(), 2);
                DataRow[] riadoknovy = hlavnaforma.routingtabulka.Select("Interface = 2 and Typ = 'C'");
                if (hlavnaforma.bezirip2)
                    hlavnaforma.posliRIPPoison(1, riadoknovy[0]);
            }
        }

        public void zastavkomunikaciuInterface1(Int32 pc1)
        {
            WinPcapDeviceList deviceswinp = WinPcapDeviceList.Instance;
            port1device = deviceswinp[pc1];

            if (port1device.Opened)
            {
                DataRow[] riadok = hlavnaforma.routingtabulka.Select("Interface = 1 and Typ = 'C'");
                if (hlavnaforma.bezirip2)
                {
                    hlavnaforma.posliRIPPoison(2, riadok[0]);
                }
                hlavnaforma.routingtabulka.Rows.Remove(riadok[0]);
                hlavnaforma.bezirip1 = false;
                hlavnaforma.beziport1 = false;
                hlavnaforma.RIP1 = '0';
                try
                {
                    port1device.StopCapture();
                    port1device.Close();
                }
                catch
                {
                }
            }
        }

        public void zastavkomunikaciuInterface2(Int32 pc2)
        {
            WinPcapDeviceList deviceswinp = WinPcapDeviceList.Instance;
            port2device = deviceswinp[pc2];

            if (port2device.Opened)
            {
                DataRow[] riadok = hlavnaforma.routingtabulka.Select("Interface = 2 and Typ = 'C'");
                if (hlavnaforma.bezirip1)
                {
                    hlavnaforma.posliRIPPoison(1, riadok[0]);
                }
                hlavnaforma.routingtabulka.Rows.Remove(riadok[0]);
                hlavnaforma.bezirip2 = false;
                hlavnaforma.beziport2 = false;
                hlavnaforma.RIP2 = '0';
                try
                {
                    port2device.StopCapture();
                    port2device.Close();
                }
                catch
                {
                }
            }
        }

        public void PosliARPRequest(string IPKam, string IPOdkial, Int32 portkablu)
        {
            
            if (portkablu == 1)
            {
                var ethernetPacket = new EthernetPacket(PhysicalAddress.Parse(hlavnaforma.mac1daj()), PhysicalAddress.Parse("FFFFFFFFFFFF"),
                    EthernetPacketType.Arp);
                ARPPacket arp = new ARPPacket(ARPOperation.Request, PhysicalAddress.Parse("FFFFFFFFFFFF"), IPAddress.Parse(IPKam), port1device.MacAddress, IPAddress.Parse(IPOdkial));
                ethernetPacket.PayloadPacket = arp;
                if (hlavnaforma.beziport1 && port1device.Opened)
                {
                    port1device.SendPacket(ethernetPacket);
                    updatujoutgoingtrafiku(null, arp, null, null, null, portkablu);
                }
            }
            else
            {
                var ethernetPacket = new EthernetPacket(PhysicalAddress.Parse(hlavnaforma.mac2daj()), PhysicalAddress.Parse("FFFFFFFFFFFF"),
                    EthernetPacketType.Arp);
                ARPPacket arp = new ARPPacket(ARPOperation.Request, PhysicalAddress.Parse("FFFFFFFFFFFF"), IPAddress.Parse(IPKam), port2device.MacAddress, IPAddress.Parse(IPOdkial));
                ethernetPacket.PayloadPacket = arp;
                if (hlavnaforma.beziport2 && port2device.Opened)
                {
                    port2device.SendPacket(ethernetPacket);
                    updatujoutgoingtrafiku(null, arp, null, null, null, portkablu);
                }
            }  
        }

        public void PosliARPResponse(string IPKam, string IPOdkial, string MACKam, Int32 portkablu)
        {
            
            if (portkablu == 1)
            {
                ARPPacket arp = new ARPPacket(ARPOperation.Response, PhysicalAddress.Parse(MACKam), IPAddress.Parse(IPKam), port1device.MacAddress, IPAddress.Parse(IPOdkial));
                var ethernetPacket = new EthernetPacket(port1device.MacAddress, PhysicalAddress.Parse(MACKam),
                    EthernetPacketType.Arp);
                ethernetPacket.PayloadPacket = arp;
                if (hlavnaforma.beziport1 && port1device.Opened)
                {
                    port1device.SendPacket(ethernetPacket);
                    updatujoutgoingtrafiku(null, arp, null, null, null, portkablu);
                }
            }
            else
            {
                ARPPacket arp = new ARPPacket(ARPOperation.Response, PhysicalAddress.Parse(MACKam), IPAddress.Parse(IPKam), port2device.MacAddress, IPAddress.Parse(IPOdkial));
                var ethernetPacket = new EthernetPacket(port2device.MacAddress, PhysicalAddress.Parse(MACKam),
                    EthernetPacketType.Arp);
                ethernetPacket.PayloadPacket = arp;
                if (hlavnaforma.beziport2 && port2device.Opened)
                {
                    port2device.SendPacket(ethernetPacket);
                    updatujoutgoingtrafiku(null, arp, null, null, null, portkablu);
                }
            }
        }

        private void device_OnPacketArrivalpc1(object sender, CaptureEventArgs e)
        {
            
                //byte[] ramec = PacketDotNet.EthernetPacket.ParsePacket(LinkLayers.Ethernet, e.Packet.Data).Bytes;
                EthernetPacket eth = (EthernetPacket) Packet.ParsePacket(LinkLayers.Ethernet, e.Packet.Data);
                ICMPv4Packet icmp = PacketDotNet.ICMPv4Packet.GetEncapsulated(eth);
                ARPPacket arp = PacketDotNet.ARPPacket.GetEncapsulated(eth);
                UdpPacket udp = PacketDotNet.UdpPacket.GetEncapsulated(eth);
                TcpPacket tcp = PacketDotNet.TcpPacket.GetEncapsulated(eth);
                IpPacket ip = PacketDotNet.IpPacket.GetEncapsulated(eth);
                IpPacket ipv4 = IPv4Packet.GetEncapsulated(eth);

            if (icmp != null)
            {
                    var a = 7;  
            }            
            if (eth.DestinationHwAddress.ToString().Equals(hlavnaforma.mac1daj()) ||
                eth.DestinationHwAddress.Equals(PhysicalAddress.Parse("FFFFFFFFFFFF")) || 
                eth.DestinationHwAddress.Equals(PhysicalAddress.Parse("01005E000009")))
            {
                updatujincomingtrafiku(icmp, arp, udp, tcp, ip, 1);
                spracujramec(eth, 1, icmp, arp, udp, tcp, ip, ipv4);
            }
        }

        private void device_OnPacketArrivalpc2(object sender, CaptureEventArgs e)
        {

            //byte[] ramec = PacketDotNet.EthernetPacket.ParsePacket(LinkLayers.Ethernet, e.Packet.Data).Bytes;
            EthernetPacket eth = (EthernetPacket)Packet.ParsePacket(LinkLayers.Ethernet, e.Packet.Data);
            ICMPv4Packet icmp = PacketDotNet.ICMPv4Packet.GetEncapsulated(eth);
            ARPPacket arp = PacketDotNet.ARPPacket.GetEncapsulated(eth);
            UdpPacket udp = PacketDotNet.UdpPacket.GetEncapsulated(eth);
            TcpPacket tcp = PacketDotNet.TcpPacket.GetEncapsulated(eth);
            IpPacket ip = PacketDotNet.IpPacket.GetEncapsulated(eth);
            IpPacket ipv4 = IPv4Packet.GetEncapsulated(eth);

            if (icmp != null)
            {

                var i = 5;

            }
            if (eth.DestinationHwAddress.ToString().Equals(hlavnaforma.mac2daj()) ||
            eth.DestinationHwAddress.Equals(PhysicalAddress.Parse("FFFFFFFFFFFF")) || 
            eth.DestinationHwAddress.Equals(PhysicalAddress.Parse("01005E000009")))
            {
                updatujincomingtrafiku(icmp, arp, udp, tcp, ip, 2);
                spracujramec(eth, 2, icmp, arp, udp, tcp, ip, ipv4);
            }

        }

        public void spracujramec(EthernetPacket eth, Int32 portkablu, ICMPv4Packet icmp, ARPPacket arp, UdpPacket udp, TcpPacket tcp, IpPacket ip, IpPacket packetIPV4)

        {
            /*try
            {*/
                if (arp != null)
                {
                    spracujarp(arp, eth, portkablu, icmp, udp, tcp, ip);
                }
                else if ((icmp != null) && packetIPV4.DestinationAddress.ToString().Equals(hlavnaforma.port1IP()))
                {
                    if (hlavnaforma.beziport1 && (icmp.TypeCode == ICMPv4TypeCodes.EchoRequest))
                    {
                        //spracujicmp(); ako ziadost portom 1
                        EthernetPacket pong = pingskladanie.pingReply(eth.SourceHwAddress.ToString(), eth.DestinationHwAddress.ToString(), ip.SourceAddress.ToString(), ip.DestinationAddress.ToString(), icmp.Data, icmp.Sequence, icmp.ID);
                        port1device.SendPacket(pong);
                        updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, 1);
                }
                    else
                    {
                        //spracujicmp(); ako odpoved portom 1                        
                        hlavnaforma.pingButton.BackColor = Color.Wheat;
                    }
                    
                }
                else if ((icmp != null) && packetIPV4.DestinationAddress.ToString().Equals(hlavnaforma.port2IP()))
                {
                    if (hlavnaforma.beziport2 && (icmp.TypeCode == ICMPv4TypeCodes.EchoRequest))
                    {
                        //spracujicmp(); ako ziadost portom 2
                        EthernetPacket pong = pingskladanie.pingReply(eth.SourceHwAddress.ToString(), eth.DestinationHwAddress.ToString(), ip.SourceAddress.ToString(), ip.DestinationAddress.ToString(), icmp.Data, icmp.Sequence, icmp.ID);
                        port2device.SendPacket(pong);
                        updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, 2);
                    }
                    else
                    {
                        //spracujicmp(); ako odpoved portom 2                        
                        hlavnaforma.pingButton.BackColor = Color.Wheat;
                    }
                }
                else if (hlavnaforma.RIP1.Equals('1') && portkablu == 1 && packetIPV4.DestinationAddress.Equals(IPAddress.Parse("224.0.0.9")) &&
                    eth.DestinationHwAddress.Equals(PhysicalAddress.Parse("01005E000009")) &&
                    udp != null &&
                    udp.DestinationPort == 520 && 
                    udp.PayloadData[0].Equals(1))
                {
                    hlavnaforma.posliRIP(1);
                }
                else if (hlavnaforma.RIP2.Equals('1') && portkablu == 2 && packetIPV4.DestinationAddress.Equals(IPAddress.Parse("224.0.0.9")) &&
                eth.DestinationHwAddress.Equals(PhysicalAddress.Parse("01005E000009")) &&
                udp != null &&
                udp.DestinationPort == 520 &&
                udp.PayloadData[0].Equals(1))
                {
                    hlavnaforma.posliRIP(2);
                }
                else if (hlavnaforma.RIP1.Equals('1') && portkablu == 1 && packetIPV4.DestinationAddress.Equals(IPAddress.Parse("224.0.0.9")) && 
                    eth.DestinationHwAddress.Equals(PhysicalAddress.Parse("01005E000009")) &&
                    udp != null &&
                    udp.DestinationPort == 520)
                {
                    spracujrip(arp, eth, portkablu, icmp, udp, tcp, ip);
                }
                else if (hlavnaforma.RIP2.Equals('1') && portkablu == 2 && packetIPV4.DestinationAddress.Equals(IPAddress.Parse("224.0.0.9")) &&
                    eth.DestinationHwAddress.Equals(PhysicalAddress.Parse("01005E000009")) &&
                    udp != null &&
                    udp.DestinationPort == 520)
                {
                    spracujrip(arp, eth, portkablu, icmp, udp, tcp, ip);
                }
                else
                {
                    smerujpacket(eth, portkablu, icmp, arp, udp, tcp, ip, packetIPV4.DestinationAddress, packetIPV4.SourceAddress);
                }
            /*}
            catch
            {
            }*/
        }

        private void smerujpacket(EthernetPacket eth, Int32 portkablu, ICMPv4Packet icmp, ARPPacket arp, UdpPacket udp, TcpPacket tcp, IpPacket ip, IPAddress ipeckaDST, IPAddress ipeckaSRC)
        {
            if (portkablu == 1 && IPAddressExtensions.IsInSameSubnet(ipeckaDST, ipeckaSRC, IPAddress.Parse(hlavnaforma.port1MASKA()))) {  }
            else if (portkablu == 2 && IPAddressExtensions.IsInSameSubnet(ipeckaDST, ipeckaSRC, IPAddress.Parse(hlavnaforma.port2MASKA()))) { }
            else { 
            DataRow row = najdiNajspecifickejsiuSiet(null, ipeckaDST);
                while (row != null)
                {
                    // ak ma najspecifickejsia siet, interface tak dovi
                    if (Int32.Parse(row["Interface"].ToString()) != 0)
                    {
                        // PRACUJE S DESTIN IP V PAKETE
                        if (row["NextHop"].ToString().Equals("null"))
                        {
                            // spracuj ramec a updatuj grafiku, len ak mame ARP inak posli ARP
                            spracujpacket(null, eth, ip, icmp, udp, tcp, arp);
                            row = null;
                        }
                        // PRACUJE S NEXTHOPOM
                        else
                        {
                            // spracuj ramec a updatuj grafiku, len ak mame ARP inak posli ARP
                            spracujpacket(row, eth, ip, icmp, udp, tcp, arp);
                            row = null;
                        }

                    }
                    else
                    {
                        row = najdiNajspecifickejsiuSiet(row, ipeckaDST);
                    }
                }                
            }
        }

        private void updatujoutgoingtrafiku(ICMPv4Packet icmp, ARPPacket arp, UdpPacket udp, TcpPacket tcp, IpPacket ip, Int32 port)
        {
            if (port == 1)
            {
                if (icmp != null)
                {
                    hlavnaforma.notifylabeloutgoing("icmp", 1);
                }
                else if (arp != null)
                {
                    hlavnaforma.notifylabeloutgoing("arp", 1);
                }
                else if (udp != null)
                {
                    hlavnaforma.notifylabeloutgoing("udp", 1);
                }
                else if (tcp != null)
                {
                    hlavnaforma.notifylabeloutgoing("tcp", 1);
                }
                else if (ip != null)
                {
                    hlavnaforma.notifylabeloutgoing("ip", 1);
                }
            }
            else
            {
                if (icmp != null)
                {
                    hlavnaforma.notifylabeloutgoing("icmp", 2);
                }
                else if (arp != null)
                {
                    hlavnaforma.notifylabeloutgoing("arp", 2);
                }
                else if (udp != null)
                {
                    hlavnaforma.notifylabeloutgoing("udp", 2);
                }
                else if (tcp != null)
                {
                    hlavnaforma.notifylabeloutgoing("tcp", 2);
                }
                else if (ip != null)
                {
                    hlavnaforma.notifylabeloutgoing("ip", 2);
                }
            }
        }

        private void updatujincomingtrafiku(ICMPv4Packet icmp, ARPPacket arp, UdpPacket udp, TcpPacket tcp, IpPacket ip, Int32 port)
        {
            if (port == 1)
            {
                if (icmp != null)
                {
                    hlavnaforma.notifylabelincoming("icmp", 1);
                }
                else if (arp != null)
                {
                    hlavnaforma.notifylabelincoming("arp", 1);
                }
                else if (udp != null)
                {
                    hlavnaforma.notifylabelincoming("udp", 1);
                }
                else if (tcp != null)
                {
                    hlavnaforma.notifylabelincoming("tcp", 1);
                }
                else if (ip != null)
                {
                    hlavnaforma.notifylabelincoming("ip", 1);
                }
            }
            else
            {
                if (icmp != null)
                {
                    hlavnaforma.notifylabelincoming("icmp", 2);
                }
                else if (arp != null)
                {
                    hlavnaforma.notifylabelincoming("arp", 2);
                }
                else if (udp != null)
                {
                    hlavnaforma.notifylabelincoming("udp", 2);
                }
                else if (tcp != null)
                {
                    hlavnaforma.notifylabelincoming("tcp", 2);
                }
                else if (ip != null)
                {
                    hlavnaforma.notifylabelincoming("ip", 2);
                } 
            }
        }

        private DataRow najdiNajspecifickejsiuSiet(DataRow riadok, IPAddress adresa)
        {
            DataRow pomrow = null;            
            if (riadok == null)
            {
                foreach (DataRow row in hlavnaforma.routingtabulka.Rows)
                {
                    if (IPAddressExtensions.IsInSameSubnet(adresa,
                        IPAddress.Parse(row["Siet"].ToString()),
                        IPAddress.Parse(row["Maska"].ToString())))
                    {
                        if ((pomrow == null) || (IPAddressExtensions.ConvertIPToUint(pomrow["Maska"].ToString()) <
                            IPAddressExtensions.ConvertIPToUint(row["Maska"].ToString())))
                        {
                            pomrow = row;
                        }
                    }
                }
                return pomrow;
            }
            else
            {
                foreach (DataRow row in hlavnaforma.routingtabulka.Rows)
                {                    
                        if (IPAddressExtensions.IsInSameSubnet(IPAddress.Parse(riadok["NextHop"].ToString()),
                            ////////////////////////////
                            IPAddress.Parse(row["Siet"].ToString()),
                            IPAddress.Parse(row["Maska"].ToString())))
                        {
                            if ((pomrow == null) || (IPAddressExtensions.ConvertIPToUint(pomrow["Maska"].ToString()) <
                                                     IPAddressExtensions.ConvertIPToUint(row["Maska"].ToString())))
                            {
                                pomrow = row;
                            }
                        }                    
                }
                return pomrow;
            }    
        }

        private void spracujarp(ARPPacket arp, EthernetPacket eth, Int32 portkablu, ICMPv4Packet icmp, UdpPacket udp, TcpPacket tcp, IpPacket ip)
        {
            if ((arp != null) && (ARPOperation.Response == arp.Operation)) // paket je ARP a je RESPONSE
            {
                if (arp.TargetProtocolAddress.ToString().Equals(hlavnaforma.Get_IPaddress(portkablu)))
                {
                    if (hlavnaforma.arptabulka.Rows.Contains(arp.SenderProtocolAddress.ToString()))
                    {
                        DataRow foundRow = hlavnaforma.arptabulka.Rows.Find(arp.SenderProtocolAddress.ToString());
                        if ((int)(foundRow["Port"]) == portkablu)
                        {
                            foundRow["Timer"] = 30;
                        }
                        else
                        {
                            foundRow["Port"] = portkablu;
                        }
                    }
                    else
                    {
                        DataRow rowcek = hlavnaforma.arptabulka.NewRow();
                        rowcek["IP"] = arp.SenderProtocolAddress.ToString();
                        rowcek["Mac"] = eth.SourceHwAddress.ToString();
                        rowcek["Port"] = portkablu;
                        rowcek["Timer"] = 30;
                        hlavnaforma.arptabulka.Rows.Add(rowcek);
                    }
                }
                else
                {
                    // jediny response co sa moze ku mne blizit a chcem ho je ten co je miereny mne, ostatne ma nazaujimaju
                }
            }
            else if (arp != null)    // paket je ARP a je REQUEST
            {
                // V pripade ze to nieje response, a je to mierene mne, teda je to request na ktory musim odpovedat
                if (arp.TargetProtocolAddress.ToString().Equals(hlavnaforma.Get_IPaddress(portkablu)))
                {
                    if (hlavnaforma.arptabulka.Rows.Contains(arp.SenderProtocolAddress.ToString()))
                    {
                        DataRow foundRow = hlavnaforma.arptabulka.Rows.Find(arp.SenderProtocolAddress.ToString());
                        if ((int)(foundRow["Port"]) == portkablu)
                        {
                            foundRow["Timer"] = 30;
                        }
                        else
                        {
                            foundRow["Port"] = portkablu;
                        }
                    }
                    else
                    {
                        DataRow rowcek = hlavnaforma.arptabulka.NewRow();
                        rowcek["IP"] = arp.SenderProtocolAddress.ToString();
                        rowcek["Mac"] = eth.SourceHwAddress.ToString();
                        rowcek["Port"] = portkablu;
                        rowcek["Timer"] = 30;
                        hlavnaforma.arptabulka.Rows.Add(rowcek);
                    }
                    PosliARPResponse(arp.SenderProtocolAddress.ToString(), hlavnaforma.Get_IPaddress(portkablu),
                        arp.SenderHardwareAddress.ToString(), portkablu);
                }
                else
                {
                    proxyarp(eth, arp, icmp, udp, tcp, ip, portkablu);
                    //V pripade ze to nieje mierene mne, teda musim najst kam to poslem
                }
            }
        }

        private void proxyarp(EthernetPacket eth, ARPPacket arp, ICMPv4Packet icmp, UdpPacket udp, TcpPacket tcp, IpPacket ip, Int32 portkablu)
        {
            // odpoviem pokial vidim ziadanu IP adresu v routing tabulke
            foreach(DataRow riadok in hlavnaforma.routingtabulka.Rows)
            {
                if (IPAddressExtensions.IsInSameSubnet(arp.TargetProtocolAddress, IPAddress.Parse(riadok["Siet"].ToString()), IPAddress.Parse(riadok["Maska"].ToString()))) {
                    
                    if (hlavnaforma.beziport1 && portkablu == 1)
                    {
                        PhysicalAddress pomPA = eth.SourceHwAddress;    // poprehadzujem ip a mac adresy
                        eth.SourceHwAddress = PhysicalAddress.Parse(hlavnaforma.mac1daj()); // ako source MAC dam seba teda princip proxy
                        eth.DestinationHwAddress = pomPA;
                        IPAddress pomIP = arp.SenderProtocolAddress;
                        arp.SenderProtocolAddress = arp.TargetProtocolAddress;
                        arp.TargetProtocolAddress = pomIP;
                        port1device.SendPacket(eth);
                        updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, portkablu);
                    }
                    else if (hlavnaforma.beziport2)
                    {
                        PhysicalAddress pomPA = eth.SourceHwAddress;    // poprehadzujem ip a mac adresy
                        eth.SourceHwAddress = PhysicalAddress.Parse(hlavnaforma.mac2daj()); // ako source MAC dam seba teda princip proxy
                        eth.DestinationHwAddress = pomPA;
                        IPAddress pomIP = arp.SenderProtocolAddress;
                        arp.SenderProtocolAddress = arp.TargetProtocolAddress;
                        arp.TargetProtocolAddress = pomIP;
                        port2device.SendPacket(eth);
                        updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, portkablu);
                    }
                }
            }
        }

        private void spracujrip(ARPPacket arp, EthernetPacket eth, Int32 portkablu, ICMPv4Packet icmp, UdpPacket udp,
            TcpPacket tcp, IpPacket ip)
        {
            DataRow[] zoznamNovychSieti = null;
            if (portkablu == 1)
            {
                zoznamNovychSieti = hlavnaforma.prelozpaketrip(udp.PayloadData, portkablu,
                    IPAddress.Parse(hlavnaforma.port1IP()));
            }
            else
            {
                zoznamNovychSieti = hlavnaforma.prelozpaketrip(udp.PayloadData, portkablu,
                    IPAddress.Parse(hlavnaforma.port2IP()));
            }
            // pre kazdy riadok sa pozriem ci ho mam v tabulke
            foreach (DataRow riadok in zoznamNovychSieti)
            {
                // pokial je to riadok co ma metriku 16 vyhodim tu cestu ak mam z routovacej tabulky
                
                // ak siet a maska rovnaka
                // ak je ripko
                DataRow[] zhodneriadky = hlavnaforma.routingtabulka.Select("Siet = '" + riadok["Siet"].ToString() + "' and Maska = '" + riadok["Maska"].ToString() +
                       "' and Typ = 'R'");
                    if (zhodneriadky.Count() != 0)
                    {
                            // ak interface && nexthop
                            // tak prepisem
                            foreach (DataRow zhodnyriadok in zhodneriadky)
                            {
                                if ((int) zhodnyriadok["Metrika"] != 16)
                                {
                                    if ((int) riadok["Metrika"] == 16)
                                    {
                                        zhodnyriadok["Flag"] = '1';
                                        zhodnyriadok["Metrika"] = 16;
                                        zhodnyriadok["Lokacia"] = "RIP";
                                        zhodnyriadok["Invalid+Flush"] = 0;
                                        // triggered update poison
                                        if (hlavnaforma.beziport1 && portkablu == 1)
                                        {
                                            hlavnaforma.posliRIPPoison(2, zhodnyriadok);
                                            updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, portkablu);
                                        }
                                        else if(hlavnaforma.beziport2)
                                        {
                                            hlavnaforma.posliRIPPoison(1, zhodnyriadok);
                                            updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, portkablu);
                                        }

                                    }
                                    else if (zhodnyriadok["Interface"].Equals(riadok["Interface"]) &&
                                             zhodnyriadok["NextHop"].Equals(ip.SourceAddress.ToString()))
                                    {
                                        if (zhodnyriadok["Lokacia"].ToString().Equals("ROUT+RIP"))
                                        {
                                            zhodnyriadok["Invalid+Flush"] = 180;
                                            zhodnyriadok["Lokacia"] = "ROUT+RIP";
                                            zhodnyriadok["Metrika"] = riadok["Metrika"];
                                            // ak iny interface || nexthop
                                        }
                                    }
                                    else if (zhodnyriadok["Interface"].Equals(riadok["Interface"]) ||
                                             zhodnyriadok["NextHop"].Equals(ip.SourceAddress.ToString()))
                                    {
                                        // pozriem ci ma lepsiu metriku
                                        if ((int) riadok["Metrika"] < (int) zhodnyriadok["Metrika"])
                                        {
                                            if (zhodnyriadok["Lokacia"].ToString().Equals("ROUT+RIP"))
                                            {
                                                // ak ano preipisem v rout tabulke
                                                zhodnyriadok["Interface"] = riadok["Interface"];
                                                zhodnyriadok["NextHop"] = ip.SourceAddress.ToString();
                                                zhodnyriadok["Metrika"] = riadok["Metrika"];
                                            }
                                        }
                                        else
                                        {
                                            // ak nie zapisem do rip tabulky only
                                        }
                                    }
                                }
                                else if(zhodnyriadok["Lokacia"].ToString().Equals("ROUT+RIP"))
                                {
                                    // ma metriku 16 zhodny riadok
                                    if (zhodnyriadok["Interface"].Equals(riadok["Interface"]) &&
                                             zhodnyriadok["NextHop"].Equals(ip.SourceAddress.ToString()))
                                    {
                                        if (zhodnyriadok["Lokacia"].ToString().Equals("ROUT+RIP"))
                                        {
                                            zhodnyriadok["Metrika"] = riadok["Metrika"];
                                            zhodnyriadok["Invalid+Flush"] = 180;
                                            // ak iny interface || nexthop
                                        }
                                    }
                                    else if (zhodnyriadok["Interface"].Equals(riadok["Interface"]) ||
                                             zhodnyriadok["NextHop"].Equals(ip.SourceAddress.ToString()))
                                    {
                                        // pozriem ci ma lepsiu metriku
                                        if ((int)riadok["Metrika"] < (int)zhodnyriadok["Metrika"])
                                        {
                                            if (zhodnyriadok["Lokacia"].ToString().Equals("ROUT+RIP"))
                                            {
                                                // ak ano preipisem v rout tabulke
                                                zhodnyriadok["Interface"] = riadok["Interface"];
                                                zhodnyriadok["NextHop"] = ip.SourceAddress.ToString();
                                                zhodnyriadok["Metrika"] = riadok["Metrika"];
                                    }
                                        }
                                        else
                                        {
                                            // ak nie zapisem do rip tabulky only
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if ((int) riadok["Metrika"] == 16)
                            {
                                riadok["Flag"] = '1';
                                riadok["Metrika"] = 16;
                                riadok["Lokacia"] = "RIP";
                                riadok["Invalid+Flush"] = 0;
                                // triggered update poison
                                if (hlavnaforma.beziport1 && portkablu == 1)
                                {
                                    hlavnaforma.posliRIPPoison(2, riadok);
                                    updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, portkablu);
                                }
                                else if(hlavnaforma.beziport2)
                                {
                                    hlavnaforma.posliRIPPoison(1, riadok);
                                    updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, portkablu);
                                }

                            }
                            else
                            {
                                riadok["NextHop"] = ip.SourceAddress.ToString();
                                riadok["Lokacia"] = "ROUT+RIP";
                                hlavnaforma.routingtabulka.Rows.Add(riadok);
                            }
                        }                                    
            }
        }

        private void spracujpacket(DataRow row, EthernetPacket eth, IpPacket ip, ICMPv4Packet icmp, UdpPacket udp, TcpPacket tcp, ARPPacket arp)
        {
            if (row != null)
            {
                //dosiel mi paket a mam jeho zaznam v arp tabulke
                if (hlavnaforma.arptabulka.Rows.Contains(row["NextHop"].ToString()))
                {
                    DataRow foundRow = hlavnaforma.arptabulka.Rows.Find(row["NextHop"].ToString());
                    if ((int)(foundRow["Port"]) == 1)
                    {
                        eth.DestinationHwAddress = PhysicalAddress.Parse(foundRow["Mac"].ToString());
                        eth.SourceHwAddress = PhysicalAddress.Parse(hlavnaforma.mac1daj());
                        if (hlavnaforma.beziport1 && port1device.Opened)
                        {
                            port1device.SendPacket(eth);
                            updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, 1);
                        }
                    }
                    else
                    {
                        eth.DestinationHwAddress = PhysicalAddress.Parse(foundRow["Mac"].ToString());
                        eth.SourceHwAddress = PhysicalAddress.Parse(hlavnaforma.mac2daj());
                        if (hlavnaforma.beziport2 && port2device.Opened)
                        {
                            port2device.SendPacket(eth);
                            updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, 2);
                        }
                    }
                }
                //dosiel mi paket a nemam jeho zaznam v arp tabulke, posielam request
                else
                {
                    DataRow riadok = najdiNajspecifickejsiuSiet(null, IPAddress.Parse(row["NextHop"].ToString()));
                    if ((int)riadok["Interface"] == 2)
                    {
                        if (hlavnaforma.beziport2 && port2device.Opened)
                        {
                            PosliARPRequest(row["NextHop"].ToString(), hlavnaforma.port2IP(), 2);
                            updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, 2);
                        }
                    }
                    else
                    {
                        if (hlavnaforma.beziport1 && port1device.Opened)
                        {
                            PosliARPRequest(row["NextHop"].ToString(), hlavnaforma.port1IP(), 1);
                            updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, 1);
                        }
                    }
                }
                // nemam NEXTHOP takze cez packet.destionationipaddress
            }
            else
            {
                //dosiel mi paket a mam jeho zaznam v arp tabulke
                if (hlavnaforma.arptabulka.Rows.Contains(ip.DestinationAddress))
                {
                    DataRow foundRow = hlavnaforma.arptabulka.Rows.Find(ip.DestinationAddress);
                    if ((int)(foundRow["Port"]) == 1)
                    {
                        eth.DestinationHwAddress = PhysicalAddress.Parse(foundRow["Mac"].ToString());
                        eth.SourceHwAddress = PhysicalAddress.Parse(hlavnaforma.mac1daj());
                        if (hlavnaforma.beziport1 && port1device.Opened)
                        {
                            port1device.SendPacket(eth);
                            updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, 1);
                        }
                    }
                    else
                    {
                        eth.DestinationHwAddress = PhysicalAddress.Parse(foundRow["Mac"].ToString());
                        eth.SourceHwAddress = PhysicalAddress.Parse(hlavnaforma.mac2daj());
                        if (hlavnaforma.beziport2 && port2device.Opened)
                        {
                            port2device.SendPacket(eth);
                            updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, 2);
                        }
                    }
                }
                //dosiel mi paket a nemam jeho zaznam v arp tabulke, posielam request
                else
                {
                    DataRow riadok = najdiNajspecifickejsiuSiet(null, ip.DestinationAddress);
                    if ((int)riadok["Interface"] == 2)
                    {
                        if (hlavnaforma.beziport2 && port2device.Opened)
                        {
                            PosliARPRequest(ip.DestinationAddress.ToString(), hlavnaforma.port2IP(), 2);
                            updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, 2);
                        }
                    }
                    else
                    {
                        if (hlavnaforma.beziport1 && port1device.Opened)
                        {
                            PosliARPRequest(ip.DestinationAddress.ToString(), hlavnaforma.port1IP(), 1);
                            updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, 1);
                        }
                    }
                }
            }
        }

        public void posliPacket(EthernetPacket packet, Int32 portkablu)
        {
            ICMPv4Packet icmp = PacketDotNet.ICMPv4Packet.GetEncapsulated(packet);
            ARPPacket arp = PacketDotNet.ARPPacket.GetEncapsulated(packet);
            UdpPacket udp = PacketDotNet.UdpPacket.GetEncapsulated(packet);
            TcpPacket tcp = PacketDotNet.TcpPacket.GetEncapsulated(packet);
            IpPacket ip = PacketDotNet.IpPacket.GetEncapsulated(packet);
            IpPacket ipv4 = IPv4Packet.GetEncapsulated(packet);
            if (portkablu == 1)
            {
                if (hlavnaforma.beziport1)
                {
                    port1device.SendPacket(packet);
                    updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, portkablu);
                }
            }
            else
            {
                if (hlavnaforma.beziport2)
                {
                    port2device.SendPacket(packet);
                    updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, portkablu);
                }
            }
        }

        public void posliPing(String ipeckaString)
        {
            checkIP(ipeckaString, true);
            Skladacka skladanie = new Skladacka();

            DataRow row = najdiNajspecifickejsiuSiet(null, IPAddress.Parse(ipeckaString));
            while (row != null)
            {
                // ak ma najspecifickejsia siet, interface tak dovi
                if (Int32.Parse(row["Interface"].ToString()) != 0)
                {
                    // PRACUJE S DESTIN IP V PAKETE
                    if (row["NextHop"].ToString().Equals("null"))
                    {
                        // spracuj ramec a updatuj grafiku, len ak mame ARP inak posli ARP
                        // potrebujem nastavit zdroj a ciel MAC
                        if (hlavnaforma.arptabulka.Rows.Contains(IPAddress.Parse(ipeckaString)))
                        {
                            DataRow foundRow = hlavnaforma.arptabulka.Rows.Find(IPAddress.Parse(ipeckaString));
                            if ((int)(foundRow["Port"]) == 1)
                            {
                                // vytvorenie pingu ako navratovu hodnotu do eth paketu                           
                                EthernetPacket eth = skladanie.ping(foundRow["Mac"].ToString(), hlavnaforma.mac1daj(), ipeckaString, hlavnaforma.port1IP());
                                ICMPv4Packet icmp = PacketDotNet.ICMPv4Packet.GetEncapsulated(eth); // pre statistiku vytvorim icmp paket :D 
                                if (hlavnaforma.beziport1 && port1device.Opened)
                                {
                                    port1device.SendPacket(eth);
                                    updatujoutgoingtrafiku(icmp, null, null, null, null, 1);
                                }
                            }
                            else
                            {
                                // vytvorenie pingu ako navratovu hodnotu do eth paketu 
                                EthernetPacket eth = skladanie.ping(foundRow["Mac"].ToString(), hlavnaforma.mac2daj(), ipeckaString, hlavnaforma.port2IP());
                                ICMPv4Packet icmp = PacketDotNet.ICMPv4Packet.GetEncapsulated(eth);  // pre statistiku vytvorim icmp paket :D 
                                if (hlavnaforma.beziport2 && port2device.Opened)
                                {
                                    port2device.SendPacket(eth);
                                    updatujoutgoingtrafiku(icmp, null, null, null, null, 2);
                                }
                            }
                        }
                        //dosiel mi paket a nemam jeho zaznam v arp tabulke, posielam request
                        else
                        {
                            DataRow riadok = najdiNajspecifickejsiuSiet(null, IPAddress.Parse(ipeckaString));
                            if ((int)riadok["Interface"] == 2)
                            {
                                if (hlavnaforma.beziport2 && port2device.Opened)
                                {
                                    PosliARPRequest(ipeckaString, hlavnaforma.port2IP(), 2);
                                }
                            }
                            else
                            {
                                if (hlavnaforma.beziport1 && port1device.Opened)
                                {
                                    PosliARPRequest(ipeckaString, hlavnaforma.port1IP(), 1);
                                }
                            }
                        }
                        row = null;
                    }
                    // PRACUJE S NEXTHOPOM
                    else
                    {
                        // spracuj ramec a updatuj grafiku, len ak mame ARP inak posli ARP
                        // potrebujem nastavit zdroj a ciel MAC
                        if (hlavnaforma.arptabulka.Rows.Contains(IPAddress.Parse(row["NextHop"].ToString())))
                        {
                            DataRow foundRow = hlavnaforma.arptabulka.Rows.Find(IPAddress.Parse(row["NextHop"].ToString()));
                            if ((int)(foundRow["Port"]) == 1)
                            {
                                // vytvorenie pingu ako navratovu hodnotu do eth paketu                           
                                EthernetPacket eth = skladanie.ping(foundRow["Mac"].ToString(), hlavnaforma.mac1daj(), ipeckaString, hlavnaforma.port1IP());
                                ICMPv4Packet icmp = PacketDotNet.ICMPv4Packet.GetEncapsulated(eth); // pre statistiku vytvorim icmp paket :D 
                                if (hlavnaforma.beziport1 && port1device.Opened)
                                {
                                    port1device.SendPacket(eth);
                                    updatujoutgoingtrafiku(icmp, null, null, null, null, 1);
                                }
                            }
                            else
                            {
                                // vytvorenie pingu ako navratovu hodnotu do eth paketu 
                                EthernetPacket eth = skladanie.ping(foundRow["Mac"].ToString(), hlavnaforma.mac2daj(), ipeckaString, hlavnaforma.port2IP());
                                ICMPv4Packet icmp = PacketDotNet.ICMPv4Packet.GetEncapsulated(eth);  // pre statistiku vytvorim icmp paket :D 
                                if (hlavnaforma.beziport2 && port2device.Opened)
                                {
                                    port2device.SendPacket(eth);
                                    updatujoutgoingtrafiku(icmp, null, null, null, null, 2);
                                }
                            }
                        }
                        //dosiel mi paket a nemam jeho zaznam v arp tabulke, posielam request
                        else
                        {
                            DataRow riadok = najdiNajspecifickejsiuSiet(null, IPAddress.Parse(row["NextHop"].ToString()));
                            if ((int)riadok["Interface"] == 2)
                            {
                                if (hlavnaforma.beziport2 && port2device.Opened)
                                {
                                    PosliARPRequest(row["NextHop"].ToString(), hlavnaforma.port2IP(), 2);
                                }
                            }
                            else
                            {
                                if (hlavnaforma.beziport1 && port1device.Opened)
                                {
                                    PosliARPRequest(row["NextHop"].ToString(), hlavnaforma.port1IP(), 1);
                                }
                            }
                        }
                        row = null;
                    }

                }
                else
                {
                    row = najdiNajspecifickejsiuSiet(row, IPAddress.Parse(ipeckaString));
                }

            }
                        
            // prerutuj cez routing table

        }

        public void vytvorStatickuCestu(string Siet, string Maska, string NextHop, Int32 Interface)
        {
            if (checkIP(Siet, true) && checkIP(Maska, false) && !(NextHop.Equals(hlavnaforma.port1IP()) || NextHop.Equals(hlavnaforma.port2IP())))
            {
                
                    DataRow row2 = hlavnaforma.routingtabulka.NewRow();
                    row2["Siet"] = IPAddress.Parse(Siet);
                    row2["Maska"] = Maska;
                    row2["Typ"] = 'S';
                    if(!NextHop.Equals(""))
                        row2["NextHop"] = NextHop.ToString();
                    row2["Interface"] = Interface;
                    hlavnaforma.routingtabulka.Rows.Add(row2);
                
            }
        }

        private Boolean checkIP(string ip, bool ipTRUE_maskFALSE)
        {
            string patternIP = @"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b";
            string mask = @"(255|254|252|248|240|224|192|128|0+)";
            Regex maskRegex = new Regex("^" + mask + @"\." + mask + @"\." + mask + @"\." + mask + "$");

            if (ipTRUE_maskFALSE)
                return Regex.IsMatch(ip, patternIP);
            else
                return maskRegex.IsMatch(ip);
        }
    }
}

