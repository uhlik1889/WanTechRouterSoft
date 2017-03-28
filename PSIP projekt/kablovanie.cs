using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        }

        public void PosliARPRequest(string IPKam, string IPOdkial, Int32 portkablu)
        {
            ARPPacket arp = new ARPPacket(ARPOperation.Request, PhysicalAddress.Parse("FFFFFFFFFFFF"), IPAddress.Parse(IPKam), port1device.MacAddress, IPAddress.Parse(IPOdkial));
            if (portkablu == 1)
            {
                var ethernetPacket = new EthernetPacket(port1device.MacAddress, PhysicalAddress.Parse("FFFFFFFFFFFF"),
                    EthernetPacketType.Arp);
                ethernetPacket.PayloadPacket = arp;
                port1device.SendPacket(ethernetPacket);
                updatujoutgoingtrafiku(null, arp, null, null, null, portkablu);
            }
            else
            {
                var ethernetPacket = new EthernetPacket(port2device.MacAddress, PhysicalAddress.Parse("FFFFFFFFFFFF"),
                    EthernetPacketType.Arp);
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


                if (arp != null)
                {
                    updatujincomingtrafiku(icmp, arp, udp, tcp, ip, 1);
                    spracujramec(eth, 1, icmp, arp, udp, tcp, ip);                    
                }
                              
   
        }

        public void spracujramec(EthernetPacket eth, Int32 portkablu, ICMPv4Packet icmp, ARPPacket arp, UdpPacket udp, TcpPacket tcp, IpPacket ip)
        // skontroluj ci sa nachadza source v tabulke
        // skontroluj ci sa zhoduje port s source v tabulke

        {
            try
            {
                if ((arp != null) && (ARPOperation.Response == arp.Operation))
                {
                    if (arp.TargetProtocolAddress.ToString().Equals(hlavnaforma.Get_IPaddress(portkablu)))
                    {
                        if (hlavnaforma.arptabulka.Rows.Contains(arp.SenderProtocolAddress.ToString()))
                        {
                            DataRow foundRow = hlavnaforma.arptabulka.Rows.Find(arp.SenderProtocolAddress.ToString());
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
                            row["IP"] = arp.SenderProtocolAddress.ToString();
                            row["Mac"] = eth.SourceHwAddress.ToString();
                            row["Port"] = portkablu;
                            row["Timer"] = 10;
                            hlavnaforma.arptabulka.Rows.Add(row);
                        }
                    }
                    else
                    {
                        //pozriem sa do ARP tabulky, ak viem kam mam poslat response dalej poslem, ak nie dropnem
                    }
                }
                else if(arp != null)
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
                            DataRow row = hlavnaforma.arptabulka.NewRow();
                            row["IP"] = arp.SenderProtocolAddress.ToString();
                            row["Mac"] = eth.SourceHwAddress.ToString();
                            row["Port"] = portkablu;
                            row["Timer"] = 10;
                            hlavnaforma.arptabulka.Rows.Add(row);
                        }
                        PosliARPResponse(arp.SenderProtocolAddress.ToString(), hlavnaforma.Get_IPaddress(portkablu),
                            arp.SenderHardwareAddress.ToString(), portkablu);
                    }
                    else
                    {
                        //V pripade ze to nieje mierene mne, teda musim najst kam to poslem
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
            }
            catch
            {
            }
        }

        private void device_OnPacketArrivalpc2(object sender, CaptureEventArgs e)
        {
            
                //byte[] ramec = PacketDotNet.EthernetPacket.ParsePacket(LinkLayers.Ethernet, e.Packet.Data).Bytes;
                EthernetPacket eth = (EthernetPacket) Packet.ParsePacket(LinkLayers.Ethernet, e.Packet.Data);
                ICMPv4Packet icmp = PacketDotNet.ICMPv4Packet.GetEncapsulated(eth);
                ARPPacket arp = PacketDotNet.ARPPacket.GetEncapsulated(eth);
                UdpPacket udp = PacketDotNet.UdpPacket.GetEncapsulated(eth);
                TcpPacket tcp = PacketDotNet.TcpPacket.GetEncapsulated(eth);
                IpPacket ip = PacketDotNet.IpPacket.GetEncapsulated(eth);
                //IPv4Packet ipv4 = PacketDotNet.IPv4Packet.GetEncapsulated(eth);

                //foreach zaznam vo filtrovacej tabulke

                if (arp != null)
                {
                    updatujincomingtrafiku(icmp, arp, udp, tcp, ip, 2);
                    spracujramec(eth, 2, icmp, arp, udp, tcp, ip);
                }
            
        }


        public void zastavkomunikaciu(Int32 pc2, Int32 pc1)
        {
            WinPcapDeviceList deviceswinp = WinPcapDeviceList.Instance;
            port2device = deviceswinp[pc2];
            port1device = deviceswinp[pc1];

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
    }
}
