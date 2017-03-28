using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PacketDotNet;
using PcapDotNet.Base;

namespace PSIP_projekt
{
    public class Filtrovanie
    {
        private FiltrovaciaObrazovka hlavnaforma = null;
        private FiltrovaciaObrazovka filtrovaciaObrazovka;

        public Filtrovanie(FiltrovaciaObrazovka form)
        {
            hlavnaforma = form;
        }


        //   porzi sa ci sa interface cez cyklus, ked nenajdes interface 1 tak pokracuj
        public bool porovnajinterface(Int32 portkablu, Int32 portfiltru)
        {
            if (portkablu == portfiltru)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //   porovnaj smer filtru
        public bool porovnajsmertrafiku(bool smerdnu, bool smerfiltra)
        {
            if (smerdnu == smerfiltra)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //   porovnaj adresy v paket a filtri, alebo ked je empty vrat true, prve 4 idu paketu druhe idu 4 filtra
        public bool porovnajadresyprefilter(String DSTMACpaket, String DSTIPpaket, String SRCMACpaket, String SRCIPpaket,
            String DSTMACfilter, String DSTIPfilter, String SRCMACfilter, String SRCIPfilter)
        {
            if ((DSTMACpaket.Equals(DSTMACfilter) || DSTMACfilter.IsNullOrEmpty()) &&
                (DSTIPpaket.Equals(DSTIPfilter) || DSTIPfilter.IsNullOrEmpty()) &&
                (SRCMACpaket.Equals(SRCMACfilter) || SRCMACfilter.IsNullOrEmpty()) &&
                (SRCIPpaket.Equals(SRCIPfilter) || SRCIPfilter.IsNullOrEmpty())
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //   porovnaj port(http napr) paketu a filtra, alebo vo filtri je nic vrat true
        public bool porovnajportyprefilter(Int32 portpaketu, Int32 portfiltra)
        {
            if ((portpaketu == portfiltra) || (portfiltra == 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool mamtopustit(EthernetPacket eth, ICMPv4Packet icmp, ARPPacket arp, UdpPacket udp, TcpPacket tcp, IpPacket ip, Int32 port, Boolean incoming)
        {
            Boolean permitaktivovanyin = false;
            Boolean permitaktivovanyout = false;
            Boolean permit = false;
            Boolean pomocna = true;  // aby som vedel ci prechadzal iterator cez permity alebo deny

            DataTable pomtabulka = null;
            pomtabulka = hlavnaforma.filtrovaciatabulka;

            //foreach zaznam vo filtrovacej tabulke hladam permit
            if (pomtabulka.Rows.Count > 0)
            {
                try
                {
                    foreach (DataRow riadok in pomtabulka.Rows)
                    {
                        if ((bool) riadok["Permit"])
                        {
                            if (permitaktivovanyin) // uz sa nasiel permit na smer in
                            {
                                if (!(bool) riadok["In"])  // ked sa najde aj na smer out nastav permit
                                {
                                    permit = true;               
                                }
                            }
                            else if (permitaktivovanyout)   // uz sa nasiel permit na smer out
                            {
                                if ((bool)riadok["In"]) // ked sa najde aj na smer in nastav permit
                                {
                                    permit = true;
                                }
                            }
                            else
                            {
                                if ((bool) riadok["In"])    // ked sa najde permit na smer in alebo out
                                {
                                    permitaktivovanyin = true;
                                }
                                else
                                {
                                    permitaktivovanyout = true;
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
                    try
                    {
                        foreach (DataRow riadok in pomtabulka.Rows)
                        {
                            if (incoming == (bool) riadok["In"])
                            {
                                //ked nieje permit idem na DENY a ked nenajdem pustim
                                if (porovnajprichadzamjepermit(permitaktivovanyin, incoming) && incoming)  // ked je PERMITIN a je prichadzajuci !!!!!!!!!!!
                                {
                                    if ((bool)riadok["Permit"])
                                    {                                       
                                        #region prepermitaktivovany                                  

                                        // ak je zhodny interface s tym po ktorom prisiel paket
                                        if (porovnajinterface(port, (int) riadok["Interface"]))
                                        {
                                            //  ked som tu znamena, je permit, a zhoduje sa mi s interfacom
                                            pomocna = false;
                                            //  ked sa kombinacie s adresami zhoduju
                                            if (porovnajadresyprefilter(eth.DestinationHwAddress.ToString(),                                                
                                                ip.DestinationAddress.ToString(), eth.SourceHwAddress.ToString(),
                                                ip.SourceAddress.ToString(), riadok["DSTMacAdresa"].ToString(),
                                                riadok["DSTIpAdresa"].ToString(),
                                                riadok["SRCMacAdresa"].ToString(), riadok["SRCIpAdresa"].ToString()))
                                            {
                                                // len ICMP je blokovane bez portu konkretneho atd pre arp,udp,tcp,ip
                                                if ((icmp != null))
                                                {
                                                    if (((bool)riadok["ICMP"]) && ((int)riadok["DSTPort"] == 0) &&
                                                    ((int)riadok["SRCPort"] == 0))
                                                    {
                                                        return true;
                                                    }
                                                    pomocna = false;
                                                }
                                                else if ((arp != null))
                                                {
                                                    if (((bool)riadok["ARP"]) && ((int)riadok["DSTPort"] == 0) &&
                                                        ((int)riadok["SRCPort"] == 0))
                                                    {
                                                        return true;
                                                    }
                                                    pomocna = false;
                                                }
                                                else if ((udp != null))   // paket bol udp a presiel napriek tomu ze som povolil len ICMP
                                                {
                                                    if (((bool)riadok["UDP"]) && ((int)riadok["DSTPort"] == 0) &&
                                                        ((int)riadok["SRCPort"] == 0))
                                                    {
                                                        return true;
                                                    }
                                                    else
                                                    {
                                                        if (((int)riadok["DSTPort"] != 0) || ((int)riadok["SRCPort"] != 0))
                                                        {
                                                            if (udp.DestinationPort == (int)riadok["DSTPort"])
                                                            {
                                                                return false;
                                                            }
                                                            else if (udp.SourcePort == (int)riadok["SRCPort"])
                                                            {
                                                                return false;
                                                            }
                                                            else
                                                            {
                                                                return true;
                                                            }
                                                        }
                                                    }
                                                }
                                                else if ((tcp != null))
                                                {
                                                    if (((bool)riadok["TCP"]) && ((int)riadok["DSTPort"] == 0) &&
                                                        ((int)riadok["SRCPort"] == 0))
                                                    {
                                                        return true;
                                                    }
                                                    else
                                                    {
                                                        if (((int)riadok["DSTPort"] != 0) || ((int)riadok["SRCPort"] != 0))
                                                        {
                                                            if (tcp.DestinationPort == (int)riadok["DSTPort"])
                                                            {
                                                                return false;
                                                            }
                                                            else if (tcp.SourcePort == (int)riadok["SRCPort"])
                                                            {
                                                                return false;
                                                            }
                                                            else
                                                            {
                                                                return true;
                                                            }
                                                        }
                                                    }
                                                    pomocna = false;
                                                }
                                                else if ((ip != null))
                                                {
                                                    if (((bool)riadok["IP"]) && ((int)riadok["DSTPort"] == 0) &&
                                                        ((int)riadok["SRCPort"] == 0))
                                                    {
                                                        return true;
                                                    }
                                                    pomocna = false;
                                                }                                                
                                            }
                                            return true;
                                        }
                                        

                                        #endregion preprmitaktivovany
                                    }
                                }
                                else if (porovnajodchadzamajepermit(!permitaktivovanyout, incoming) && !incoming)  // ked je PERMITOUT a je odchadzajuci !!!!!!!!!!!
                                {
                                    if ((bool)riadok["permit"])
                                    {
                                        //pomocna = true;
                                        #region prepermitaktivovany

                                        // ak je zhodny interface s tym po ktorom prisiel paket
                                        if (porovnajinterface(port, (int) riadok["Interface"]))
                                        {   
                                            //  zase situacia, existuje permit, zhoduje sa mi s interfacom, cize bud zhoda alebo papa :D 
                                            pomocna = false;
                                            //  ked sa kombinacie s adresami zhoduju
                                            if (porovnajadresyprefilter(eth.DestinationHwAddress.ToString(),
                                                ip.DestinationAddress.ToString(), eth.SourceHwAddress.ToString(),
                                                ip.SourceAddress.ToString(), riadok["DSTMacAdresa"].ToString(),
                                                riadok["DSTIpAdresa"].ToString(),
                                                riadok["SRCMacAdresa"].ToString(), riadok["SRCIpAdresa"].ToString()))
                                            {
                                                // len ICMP je blokovane bez portu konkretneho atd pre arp,udp,tcp,ip
                                                if ((icmp != null))
                                                {
                                                    if (((bool)riadok["ICMP"]) && ((int)riadok["DSTPort"] == 0) &&
                                                    ((int)riadok["SRCPort"] == 0))
                                                    {
                                                        return true;
                                                    }
                                                    pomocna = false;
                                                }
                                                else if ((arp != null))
                                                {
                                                    if (((bool)riadok["ARP"]) && ((int)riadok["DSTPort"] == 0) &&
                                                        ((int)riadok["SRCPort"] == 0))
                                                    {
                                                        return true;
                                                    }
                                                    pomocna = false;
                                                }
                                                else if ((udp != null))   // paket bol udp a presiel napriek tomu ze som povolil len ICMP
                                                {
                                                    if (((bool)riadok["UDP"]) && ((int)riadok["DSTPort"] == 0) &&
                                                        ((int)riadok["SRCPort"] == 0))
                                                    {
                                                        return true;
                                                    }
                                                    else
                                                    {
                                                        if (((int)riadok["DSTPort"] != 0) || ((int)riadok["SRCPort"] != 0))
                                                        {
                                                            if (udp.DestinationPort == (int)riadok["DSTPort"])
                                                            {
                                                                return false;
                                                            }
                                                            else if (udp.SourcePort == (int)riadok["SRCPort"])
                                                            {
                                                                return false;
                                                            }
                                                            else
                                                            {
                                                                return true;
                                                            }
                                                        }
                                                    }
                                                }
                                                else if ((tcp != null))
                                                {
                                                    if (((bool)riadok["TCP"]) && ((int)riadok["DSTPort"] == 0) &&
                                                        ((int)riadok["SRCPort"] == 0))
                                                    {
                                                        return true;
                                                    }
                                                    else
                                                    {
                                                        if (((int)riadok["DSTPort"] != 0) || ((int)riadok["SRCPort"] != 0))
                                                        {
                                                            if (tcp.DestinationPort == (int)riadok["DSTPort"])
                                                            {
                                                                return false;
                                                            }
                                                            else if (tcp.SourcePort == (int)riadok["SRCPort"])
                                                            {
                                                                return false;
                                                            }
                                                            else
                                                            {
                                                                return true;
                                                            }
                                                        }
                                                    }
                                                    pomocna = false;
                                                }
                                                else if ((ip != null))
                                                {
                                                    if (((bool)riadok["IP"]) && ((int)riadok["DSTPort"] == 0) &&
                                                        ((int)riadok["SRCPort"] == 0))
                                                    {
                                                        return true;
                                                    }
                                                    pomocna = false;
                                                }                                                
                                            }
                                            return true;
                                        }

                                        #endregion preprmitaktivovany
                                    }
                                } 
                                else if (porovnajpermit(permit))  // ked je PERMIT obojstranne !!!!!!!!!!
                                {
                                    if ((bool)riadok["Permit"])
                                    {
                                        pomocna = true;
                                        #region prepermitaktivovany

                                        // ak je zhodny interface s tym po ktorom prisiel paket
                                        if (porovnajinterface(port, (int) riadok["Interface"]))
                                        {
                                            //  ked sa kombinacie s adresami zhoduju
                                            if (porovnajadresyprefilter(eth.DestinationHwAddress.ToString(),
                                                ip.DestinationAddress.ToString(), eth.SourceHwAddress.ToString(),
                                                ip.SourceAddress.ToString(), riadok["DSTMacAdresa"].ToString(),
                                                riadok["DSTIpAdresa"].ToString(),
                                                riadok["SRCMacAdresa"].ToString(), riadok["SRCIpAdresa"].ToString()))
                                            {
                                                // len ICMP je blokovane bez portu konkretneho atd pre arp,udp,tcp,ip
                                                if ((icmp != null))
                                                {
                                                    if (((bool)riadok["ICMP"]) && ((int)riadok["DSTPort"] == 0) &&
                                                    ((int)riadok["SRCPort"] == 0))
                                                    {
                                                        return true;
                                                    }
                                                    pomocna = false;
                                                }
                                                else if ((arp != null))
                                                {
                                                    if (((bool)riadok["ARP"]) && ((int)riadok["DSTPort"] == 0) &&
                                                        ((int)riadok["SRCPort"] == 0))
                                                    {
                                                        return true;
                                                    }
                                                    pomocna = false;
                                                }
                                                else if ((udp != null))   // paket bol udp a presiel napriek tomu ze som povolil len ICMP
                                                {
                                                    if (((bool)riadok["UDP"]) && ((int)riadok["DSTPort"] == 0) &&
                                                        ((int)riadok["SRCPort"] == 0))
                                                    {
                                                        return true;
                                                    }
                                                    else
                                                    {
                                                        if (((int)riadok["DSTPort"] != 0) || ((int)riadok["SRCPort"] != 0))
                                                        {
                                                            if (udp.DestinationPort == (int)riadok["DSTPort"])
                                                            {
                                                                return false;
                                                            }
                                                            else if (udp.SourcePort == (int)riadok["SRCPort"])
                                                            {
                                                                return false;
                                                            }
                                                            else
                                                            {
                                                                return true;
                                                            }
                                                        }
                                                    }
                                                }
                                                else if ((tcp != null))
                                                {
                                                    if (((bool)riadok["TCP"]) && ((int)riadok["DSTPort"] == 0) &&
                                                        ((int)riadok["SRCPort"] == 0))
                                                    {
                                                        return true;
                                                    }
                                                    else
                                                    {
                                                        if (((int)riadok["DSTPort"] != 0) || ((int)riadok["SRCPort"] != 0))
                                                        {
                                                            if (tcp.DestinationPort == (int)riadok["DSTPort"])
                                                            {
                                                                return false;
                                                            }
                                                            else if (tcp.SourcePort == (int)riadok["SRCPort"])
                                                            {
                                                                return false;
                                                            }
                                                            else
                                                            {
                                                                return true;
                                                            }
                                                        }
                                                    }
                                                    pomocna = false;
                                                }
                                                else if ((ip != null))
                                                {
                                                    if (((bool)riadok["IP"]) && ((int)riadok["DSTPort"] == 0) &&
                                                        ((int)riadok["SRCPort"] == 0))
                                                    {
                                                        return true;
                                                    }
                                                    pomocna = false;
                                                }
                                                
                                            }
                                            return true;
                                        }

                                        #endregion preprmitaktivovany
                                    }
                                } 
                                else  // ked je DENY !!!!!!!!!!!!
                                {
                                    #region predenyaktivovany
                                    // ak je zhodny interface s tym po ktorom prisiel paket
                                    if (porovnajinterface(port, (int) riadok["Interface"]))
                                    {
                                        //  ked sa kombinacie s adresami zhoduju
                                        if (porovnajadresyprefilter(eth.DestinationHwAddress.ToString(),
                                            ip.DestinationAddress.ToString(), eth.SourceHwAddress.ToString(),
                                            ip.SourceAddress.ToString(), riadok["DSTMacAdresa"].ToString(),
                                            riadok["DSTIpAdresa"].ToString(),
                                            riadok["SRCMacAdresa"].ToString(), riadok["SRCIpAdresa"].ToString()))
                                        {
                                            // len ICMP je blokovane bez portu konkretneho atd pre arp,udp,tcp,ip
                                            if ((icmp != null))
                                            {
                                                if (((bool)riadok["ICMP"]) && ((int)riadok["DSTPort"] == 0) &&
                                                ((int)riadok["SRCPort"] == 0))
                                                {
                                                    return false;
                                                }
                                                pomocna = true;
                                            }
                                            else if ((arp != null))
                                            {
                                                if (((bool)riadok["ARP"]) && ((int)riadok["DSTPort"] == 0) &&
                                                    ((int)riadok["SRCPort"] == 0))
                                                {
                                                    return true;
                                                }
                                                pomocna = true;
                                            }
                                            else if ((udp != null))   // paket bol udp a presiel napriek tomu ze som povolil len ICMP
                                            {
                                                if (((bool) riadok["UDP"]) && ((int) riadok["DSTPort"] == 0) &&
                                                    ((int) riadok["SRCPort"] == 0))
                                                {
                                                    return false;
                                                }
                                                else
                                                {
                                                    if (((int)riadok["DSTPort"] != 0) || ((int)riadok["SRCPort"] != 0))
                                                    {
                                                        if (udp.DestinationPort == (int)riadok["DSTPort"])
                                                        {
                                                            return false;
                                                        }
                                                        else if (udp.SourcePort == (int)riadok["SRCPort"])
                                                        {
                                                            return false;
                                                        }
                                                        else
                                                        {
                                                            return true;
                                                        }
                                                    }
                                                }
                                            }
                                            else if ((tcp != null))
                                            {
                                                if (((bool) riadok["TCP"]) && ((int) riadok["DSTPort"] == 0) &&
                                                    ((int) riadok["SRCPort"] == 0))
                                                {
                                                    return false;
                                                }
                                                else
                                                {
                                                    if (((int)riadok["DSTPort"] != 0) || ((int)riadok["SRCPort"] != 0))
                                                {
                                                    if (tcp.DestinationPort == (int) riadok["DSTPort"])
                                                    {
                                                        return false;
                                                    }
                                                    else if (tcp.SourcePort == (int) riadok["SRCPort"])
                                                    {
                                                        return false;
                                                    }
                                                    else
                                                    {
                                                        return true;
                                                    }
                                                }
                                                }
                                                pomocna = false;
                                            }
                                            else if ((ip != null))
                                            {
                                                if (((bool)riadok["IP"]) && ((int)riadok["DSTPort"] == 0) &&
                                                    ((int)riadok["SRCPort"] == 0))
                                                {
                                                    return false;
                                                }
                                                pomocna = true;
                                            }
                                            else
                                            {                                              
                                                // kvoli pripadu ze mi filter presiel, aj ked sa paket zhodoval s filtrom
                                            }
                                        }
                                        else
                                        {
                                            if (incoming)
                                            {
                                                if (!(permitaktivovanyin || permit))
                                                    return true;
                                            }
                                            else
                                                if (!(permitaktivovanyout || permit))
                                                    return true;
                                        }
                                        return false;
                                    }
                                    #endregion predenyaktivovany
                                }
                            }
                        }

                        if (pomocna)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    }
                    catch
                    {
                    }
                //}
                // sem sa dostane ked dojde filter, takze pre DENY nepustim dalej, nedam true
                
            }
            else
            {
                return true;
            }
            
            return false;
        }

        private bool porovnajprichadzamjepermit(Boolean permit, Boolean prichadzajuci)
        {
            if (permit == prichadzajuci)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool porovnajodchadzamajepermit(Boolean permit, Boolean odchadzajuci)
        {
            if (permit == odchadzajuci)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool porovnajpermit(Boolean permit)
        {
            if (permit)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
        
    }
}
