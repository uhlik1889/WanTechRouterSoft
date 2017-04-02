using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
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

        public void Spojpocitace(Control f, int pc2, int pc1)
        {
            WinPcapDeviceList deviceswinp = WinPcapDeviceList.Instance;
            port2device = deviceswinp[pc2];
            port1device = deviceswinp[pc1];

            port1device.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrivalpc1);
            port2device.OnPacketArrival +=
                new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrivalpc2);

            // Open the device for capturing
            int readTimeoutMilliseconds = 100;
            port2device.Open(OpenFlags.NoCaptureLocal | OpenFlags.Promiscuous, readTimeoutMilliseconds);
            port1device.Open(OpenFlags.NoCaptureLocal | OpenFlags.Promiscuous, readTimeoutMilliseconds);

            f.UpdateText("-- Listening on {0}, hit 'Enter' to stop..." + port2device.Description);

            // Start the capturing process
            port2device.StartCapture();
            port1device.StartCapture();

            DataRow row1 = hlavnaforma.routingtabulka.NewRow();
            row1["Siet"] = IPAddressExtensions.GetNetworkAddress(IPAddress.Parse(hlavnaforma.port1IP()),
                IPAddress.Parse(hlavnaforma.port1MASKA()));
            row1["Maska"] = hlavnaforma.port1MASKA();
            row1["Typ"] = 'C';
            row1["NextHop"] = "null";
            row1["Interface"] = 1;
            hlavnaforma.routingtabulka.Rows.Add(row1);

            DataRow row2 = hlavnaforma.routingtabulka.NewRow();
            row2["Siet"] = IPAddressExtensions.GetNetworkAddress(IPAddress.Parse(hlavnaforma.port2IP()),
                IPAddress.Parse(hlavnaforma.port2MASKA()));
            row2["Maska"] = hlavnaforma.port2MASKA();
            row2["Typ"] = 'C';
            row2["NextHop"] = "null";
            row2["Interface"] = 2;
            hlavnaforma.routingtabulka.Rows.Add(row2);

            hlavnaforma.macnastav(port1device.MacAddress.ToString(), 1);
            hlavnaforma.macnastav(port2device.MacAddress.ToString(), 2);
        }

        public void PosliARPRequest(string IPKam, string IPOdkial, Int32 portkablu)
        {
            
            if (portkablu == 1)
            {
                var ethernetPacket = new EthernetPacket(PhysicalAddress.Parse(hlavnaforma.mac1daj()), PhysicalAddress.Parse("FFFFFFFFFFFF"),
                    EthernetPacketType.Arp);
                ARPPacket arp = new ARPPacket(ARPOperation.Request, PhysicalAddress.Parse("FFFFFFFFFFFF"), IPAddress.Parse(IPKam), port1device.MacAddress, IPAddress.Parse(IPOdkial));
                ethernetPacket.PayloadPacket = arp;
                port1device.SendPacket(ethernetPacket);
                updatujoutgoingtrafiku(null, arp, null, null, null, portkablu);
            }
            else
            {
                var ethernetPacket = new EthernetPacket(PhysicalAddress.Parse(hlavnaforma.mac2daj()), PhysicalAddress.Parse("FFFFFFFFFFFF"),
                    EthernetPacketType.Arp);
                ARPPacket arp = new ARPPacket(ARPOperation.Request, PhysicalAddress.Parse("FFFFFFFFFFFF"), IPAddress.Parse(IPKam), port2device.MacAddress, IPAddress.Parse(IPOdkial));
                ethernetPacket.PayloadPacket = arp;
                port2device.SendPacket(ethernetPacket);
                updatujoutgoingtrafiku(null, arp, null, null, null, portkablu);
            }  
        }

        public void PosliARPResponse(string IPKam, string IPOdkial, string MACKam, Int32 portkablu)
        {
            ARPPacket arp = new ARPPacket(ARPOperation.Response, PhysicalAddress.Parse(MACKam), IPAddress.Parse(IPKam), port1device.MacAddress, IPAddress.Parse(IPOdkial));
            if (portkablu == 1)
            {
                var ethernetPacket = new EthernetPacket(port1device.MacAddress, PhysicalAddress.Parse(MACKam),
                    EthernetPacketType.Arp);
                ethernetPacket.PayloadPacket = arp;
                port1device.SendPacket(ethernetPacket);
                updatujoutgoingtrafiku(null, arp, null, null, null, portkablu);
            }
            else
            {
                var ethernetPacket = new EthernetPacket(port2device.MacAddress, PhysicalAddress.Parse(MACKam),
                    EthernetPacketType.Arp);
                ethernetPacket.PayloadPacket = arp;
                port2device.SendPacket(ethernetPacket);
                updatujoutgoingtrafiku(null, arp, null, null, null, portkablu);
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

            if (icmp!=null)
            {

                var a = 7;
            }
            if (eth.DestinationHwAddress.ToString().Equals(hlavnaforma.mac1daj()) ||
                eth.DestinationHwAddress.Equals(PhysicalAddress.Parse("FFFFFFFFFFFF")))
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
            eth.DestinationHwAddress.Equals(PhysicalAddress.Parse("FFFFFFFFFFFF")))
            {
                updatujincomingtrafiku(icmp, arp, udp, tcp, ip, 2);
                spracujramec(eth, 2, icmp, arp, udp, tcp, ip, ipv4);
            }

        }

        public void spracujramec(EthernetPacket eth, Int32 portkablu, ICMPv4Packet icmp, ARPPacket arp, UdpPacket udp, TcpPacket tcp, IpPacket ip, IpPacket packet)

        {
            /*try
            {*/
                if (arp != null)
                {
                    spracujarp(arp, eth, portkablu, icmp, udp, tcp, ip);
                }
                else if ((icmp != null) && packet.DestinationAddress.Equals(hlavnaforma.port1IP()))
                {
                    if ((icmp.TypeCode == ICMPv4TypeCodes.EchoRequest))
                    {
                        //spracujicmp(); ako ziadost portom 2
                    }
                    else
                    {
                        //spracujicmp(); ako odpoved portom 2
                    }
                    
                }
                else if ((icmp != null) && packet.DestinationAddress.Equals(hlavnaforma.port2IP()))
                {
                    if ((icmp.TypeCode == ICMPv4TypeCodes.EchoRequest))
                    {
                        //spracujicmp(); ako ziadost portom 2
                    }
                    else
                    {
                        //spracujicmp(); ako odpoved portom 2
                    }
                }
                else
                {
                    DataRow row = najdiNajspecifickejsiuSiet(null, packet.DestinationAddress);
                    while (row != null)
                    {
                        // ak ma najspecifickejsia siet, interface tak dovi
                        if (Int32.Parse(row["Interface"].ToString()) != 0)
                        {
                            // PRACUJE S DESTIN IP V PAKETE
                            if (row["NextHop"].ToString().Equals("null"))   
                            {
                            // spracuj ramec a updatuj grafiku, len ak mame ARP inak posli ARP
                                spracujpacket(portkablu, null, eth, ip, icmp, udp, tcp, arp);
                                row = null;
                            }
                            // PRACUJE S NEXTHOPOM
                            else
                            {
                            // spracuj ramec a updatuj grafiku, len ak mame ARP inak posli ARP
                                spracujpacket(portkablu, row, eth, ip, icmp, udp, tcp, arp);
                                row = null;
                            }

                        }
                        else
                        {
                            row = najdiNajspecifickejsiuSiet(row, packet.DestinationAddress);
                        }

                    }

                }
            /*}
            catch
            {
            }*/
        }

        public void zastavkomunikaciu(Int32 pc2, Int32 pc1)
        {
            WinPcapDeviceList deviceswinp = WinPcapDeviceList.Instance;
            port2device = deviceswinp[pc2];
            port1device = deviceswinp[pc1];

            hlavnaforma.vycistiRoutingTabulku();

            try
            {
                port2device.StopCapture();
                port2device.Close();
                port1device.StopCapture();
                port1device.Close();
            }
            catch
            {
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
                                                                        foundRow["Timer"] = 10;
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
                                                                    rowcek["Timer"] = 10;
                                                                    hlavnaforma.arptabulka.Rows.Add(rowcek);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                //pozriem sa do ARP tabulky, ak viem kam mam poslat response dalej poslem, ak nie ...
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
                                                                        foundRow["Timer"] = 10;
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
                                                                    rowcek["Timer"] = 10;
                                                                    hlavnaforma.arptabulka.Rows.Add(rowcek);
                                                                }
                                                                PosliARPResponse(arp.SenderProtocolAddress.ToString(), hlavnaforma.Get_IPaddress(portkablu),
                                                                    arp.SenderHardwareAddress.ToString(), portkablu);
                                                            }
                                                            else
                                                            {
                                                                //V pripade ze to nieje mierene mne, teda musim najst kam to poslem
                                                            }
                                                        }
                            
        }

        private void spracujpacket(Int32 portkablu, DataRow row, EthernetPacket eth, IpPacket ip, ICMPv4Packet icmp, UdpPacket udp, TcpPacket tcp, ARPPacket arp)
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
                        port1device.SendPacket(eth);
                        updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, 1);
                    }
                    else
                    {
                        eth.DestinationHwAddress = PhysicalAddress.Parse(foundRow["Mac"].ToString());
                        eth.SourceHwAddress = PhysicalAddress.Parse(hlavnaforma.mac2daj());
                        port2device.SendPacket(eth);
                        updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, 2);
                    }
                }
                //dosiel mi paket a nemam jeho zaznam v arp tabulke, posielam request
                else
                {
                    DataRow riadok = najdiNajspecifickejsiuSiet(null, IPAddress.Parse(row["NextHop"].ToString()));
                    if ((int)riadok["Interface"] == 2)
                    {
                        PosliARPRequest(row["NextHop"].ToString(), hlavnaforma.port2IP(), 2);
                        updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, 2);
                    }
                    else
                    {
                        PosliARPRequest(row["NextHop"].ToString(), hlavnaforma.port1IP(), 1);
                        updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, 1);
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
                        port1device.SendPacket(eth);
                        updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, 1);
                    }
                    else
                    {
                        eth.DestinationHwAddress = PhysicalAddress.Parse(foundRow["Mac"].ToString());
                        eth.SourceHwAddress = PhysicalAddress.Parse(hlavnaforma.mac2daj());
                        port2device.SendPacket(eth);
                        updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, 2);
                    }
                }
                //dosiel mi paket a nemam jeho zaznam v arp tabulke, posielam request
                else
                {
                    DataRow riadok = najdiNajspecifickejsiuSiet(null, ip.DestinationAddress);
                    if ((int)riadok["Interface"] == 2)
                    {
                        PosliARPRequest(ip.DestinationAddress.ToString(), hlavnaforma.port2IP(), 2);
                        updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, 2);
                    }
                    else
                    {
                        PosliARPRequest(ip.DestinationAddress.ToString(), hlavnaforma.port1IP(), 1);
                        updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, 1);
                    }
                }
            }
        }

        private void spracujicmp(Int32 portkablu, DataRow row, EthernetPacket eth, IpPacket ip, ICMPv4Packet icmp,
            UdpPacket udp, TcpPacket tcp, ARPPacket arp)
        {
            // odpovedam na icmp
            
        }
    }
}


/*{
                   if (hlavnaforma.arptabulka.Rows.Contains(eth.SourceHwAddress.ToString()))
                   {
                       DataRow foundRow = hlavnaforma.arptabulka.Rows.Find(eth.SourceHwAddress.ToString());
                       if ((int) (foundRow["Port"]) == portkablu)
                       {
                           foundRow["Timer"] = 10;
                       }
                       else
                       {
                           foundRow["Port"] = portkablu;
                       }
                   }
                   else
                   {
                        
                           DataRow row = hlavnaforma.arptabulka.NewRow();
                           row["Mac"] = eth.SourceHwAddress.ToString();
                           row["Port"] = portkablu;
                           row["Timer"] = 10;
                           hlavnaforma.arptabulka.Rows.Add(row);
                        
                   }
                   // ak existuje destin mac v tabulke posli len tam
                   if (hlavnaforma.arptabulka.Rows.Contains(eth.DestinationHwAddress.ToString()))
                   {
                       DataRow foundRow = hlavnaforma.arptabulka.Rows.Find(eth.DestinationHwAddress.ToString());
                       if (Int32.Parse(foundRow["Port"].ToString()) != portkablu)
                       {
                           if (Int32.Parse(foundRow["Port"].ToString()) == 1)
                           {
                               if (filtracia.mamtopustit(eth, icmp, arp, udp, tcp, ip, 1, false))
                               {
                                   port1device.SendPacket(eth);
                                   updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, 1);
                               }
                           }
                           else if (Int32.Parse(foundRow["Port"].ToString()) == 2)
                           {
                               if (filtracia.mamtopustit(eth, icmp, arp, udp, tcp, ip, 2, false))
                               {
                                   port2device.SendPacket(eth);
                                   updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, 2);
                               }
                           }
                       }
                   }
                   else // ak neexistuje destin mac v tabulke posli vsade okrem odkial prislo
                   {
                       if (hlavnaforma.arptabulka.Rows.Contains(eth.SourceHwAddress.ToString()))
                       {
                           DataRow foundRow = hlavnaforma.arptabulka.Rows.Find(eth.SourceHwAddress.ToString());
                           if ((Int32.Parse(foundRow["Port"].ToString()) == 1) && (portkablu != 2))
                           {
                               if (filtracia.mamtopustit(eth, icmp, arp, udp, tcp, ip, 2, false))
                               {
                                   port2device.SendPacket(eth);
                                   updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, 2);
                               }
                           }
                           else if ((Int32.Parse(foundRow["Port"].ToString()) == 2) && (portkablu != 1))
                           {
                               if (filtracia.mamtopustit(eth, icmp, arp, udp, tcp, ip, 1, false))
                               {
                                   port1device.SendPacket(eth);
                                   updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, 1);
                               }
                           }
                       }
                   }


                   // ak je destin FFFF.FFFF.FFFF.FFFF tak posli vsade okrem odkial prislo
                   if (eth.DestinationHwAddress.ToString().Equals("FFFFFFFFFFFF"))
                   {
                       if (hlavnaforma.arptabulka.Rows.Contains(eth.SourceHwAddress.ToString()))
                       {
                           DataRow foundRow = hlavnaforma.arptabulka.Rows.Find(eth.SourceHwAddress.ToString());
                           if ((Int32.Parse(foundRow["Port"].ToString()) == 1) && (portkablu != 2))
                           {
                               if (filtracia.mamtopustit(eth, icmp, arp, udp, tcp, ip, 2, false))
                               {
                                   port2device.SendPacket(eth);
                                   updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, 2);
                               }
                           }
                           else if ((Int32.Parse(foundRow["Port"].ToString()) == 2) && (portkablu != 1))
                           {
                               if (filtracia.mamtopustit(eth, icmp, arp, udp, tcp, ip, 1, false))
                               {
                                   port1device.SendPacket(eth);
                                   updatujoutgoingtrafiku(icmp, arp, udp, tcp, ip, 1);
                               }
                           }
                       }
                   }
               }*/
