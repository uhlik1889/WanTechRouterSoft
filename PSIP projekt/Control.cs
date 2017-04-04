using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PacketDotNet;
using SharpPcap;
using SharpPcap.WinPcap;
using Timer = System.Threading.Timer;

namespace PSIP_projekt
{
    public partial class Control : Form
    {
        private Statistika statistikaprichadzajuca1 = new Statistika();
        private Statistika statistikaodchadzajuca1 = new Statistika();
        private Statistika statistikaprichadzajuca2 = new Statistika();
        private Statistika statistikaodchadzajuca2 = new Statistika();
        private FiltrovaciaObrazovka filtrikobrazovka;
        public kablovanie zapajaniekablov;

        public DataTable arptabulka = new DataTable();
        private DataColumn arpmacadresa = new DataColumn();
        private DataColumn arpipadresa = new DataColumn();
        private DataColumn arpport = new DataColumn();
        private DataColumn arptimer = new DataColumn();

        public DataTable routingtabulka = new DataTable();

        private DataColumn routsiet = new DataColumn();
        private DataColumn routmaska = new DataColumn();
        private DataColumn routtyp = new DataColumn(); // directly, static, RIP, 
        private DataColumn routnexthop = new DataColumn();
        private DataColumn routinterface = new DataColumn();

        private DataView viewARP;
        private DataView viewROUTING;

        public Control()
        {
            //filtrikobrazovka = new FiltrovaciaObrazovka(this);
            zapajaniekablov = new kablovanie(this, filtrikobrazovka);

            InitializeComponent();
            textBox1.AppendText("Vita ta switch naprogramovany Matejom Uhlikom :D\n");

            timerpc.Interval = (1*1000);
            timerpc.Tick += new EventHandler(timerpc_Tick);
            timerpc.Enabled = true;
            timerpc.Start();

            //   pridavanie stlpcov do ARP tabulky

            #region pridavanie stlpcov do ARP tabulky

            arpipadresa.DataType = System.Type.GetType("System.String");
            arpipadresa.ColumnName = "IP";
            arptabulka.Columns.Add(arpipadresa);
            arptabulka.PrimaryKey = new DataColumn[] {arptabulka.Columns["IP"]};

            arpmacadresa.DataType = System.Type.GetType("System.String");
            arpmacadresa.ColumnName = "Mac";
            arptabulka.Columns.Add(arpmacadresa);


            arpport.DataType = System.Type.GetType("System.Int32");
            arpport.ColumnName = "Port";
            arptabulka.Columns.Add(arpport);

            arptimer.DataType = System.Type.GetType("System.Int32");
            arptimer.ColumnName = "Timer";
            arptabulka.Columns.Add(arptimer);

            viewARP = new DataView(arptabulka);
            arpTabulkaView.DataSource = viewARP;

            #endregion

            //   pridavanie stlpcov do routovacej tabulky

            #region private DataView ROUTINGTABLE

            routsiet.DataType = System.Type.GetType("System.String");
            routsiet.ColumnName = "Siet";
            routingtabulka.Columns.Add(routsiet);
            //routingtabulka.PrimaryKey = new DataColumn[] {arptabulka.Columns["Siet"]};

            routmaska.DataType = System.Type.GetType("System.String");
            routmaska.ColumnName = "Maska";
            routingtabulka.Columns.Add(routmaska);


            routtyp.DataType = System.Type.GetType("System.Char");
            routtyp.ColumnName = "Typ";
            routingtabulka.Columns.Add(routtyp);

            routnexthop.DataType = System.Type.GetType("System.String");
            routnexthop.ColumnName = "NextHop";
            routingtabulka.Columns.Add(routnexthop);

            routinterface.DataType = System.Type.GetType("System.Int32");
            routinterface.ColumnName = "Interface";
            routingtabulka.Columns.Add(routinterface);

            viewROUTING = new DataView(routingtabulka);
            routovaciaTableView.DataSource = viewROUTING;

            #endregion

        }

        private void button1_Click(object sender, EventArgs e)
        {

            zapajaniekablov.Ukazkarty(this);
        }

        public string port1IP()
        {
            return port1IPadressText.Text;
        }

        public string port2IP()
        {
            return port2IPadressText.Text;
        }

        public string port1MASKA()
        {
            return port1Maska.Text;
        }

        public string port2MASKA()
        {
            return port2Maska.Text;
        }


    internal void UpdateText(string p)
        {
            textBox1.AppendText(p);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            zapajaniekablov.Spojpocitace(this, Convert.ToInt32(port2devlistnum.Text), Convert.ToInt32(port1devlistnum.Text));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            zapajaniekablov.zastavkomunikaciu(Convert.ToInt32(port2devlistnum.Text), Convert.ToInt32(port1devlistnum.Text));
        }

        public void macnastav(string MAC, Int32 portkablu)
        {
            if(portkablu == 1)
            SynchronizedInvoke(macLabel1, delegate() { macLabel1.Text = MAC;  });
            else
            {
                SynchronizedInvoke(macLabel2, delegate() { macLabel2.Text = MAC; });
            }
        }

        public string mac1daj()
        {
            return macLabel1.Text;
        }

        public string mac2daj()
        {
            return macLabel2.Text;
        }

        public void notifylabelincoming(string pole, Int32 port)
        {
            if (port == 1)
            {
                switch (pole)
                {
                    case "icmp":
                        statistikaprichadzajuca1.icmp++;
                        SetTexticmp1in(statistikaprichadzajuca1.icmp.ToString());
                        break;
                    case "udp":
                        statistikaprichadzajuca1.udp++;
                        SetTextudp1in(statistikaprichadzajuca1.udp.ToString());
                        break;
                    case "tcp":
                        statistikaprichadzajuca1.tcp++;
                        SetTexttcp1in(statistikaprichadzajuca1.tcp.ToString());
                        break;
                    case "arp":
                        statistikaprichadzajuca1.arp++;
                        SetTextarp1in(statistikaprichadzajuca1.arp.ToString());
                        break;
                    case "ip":
                        statistikaprichadzajuca1.ip++;
                        SetTextip1in(statistikaprichadzajuca1.ip.ToString());
                        break;
                }
            }
            else
            {
                switch (pole)
                {
                    case "icmp":
                        statistikaprichadzajuca2.icmp++;
                        SetTexticmp2in(statistikaprichadzajuca2.icmp.ToString());
                        break;
                    case "udp":
                        statistikaprichadzajuca2.udp++;
                        SetTextudp2in(statistikaprichadzajuca2.udp.ToString());
                        break;
                    case "tcp":
                        statistikaprichadzajuca2.tcp++;
                        SetTexttcp2in(statistikaprichadzajuca2.tcp.ToString());
                        break;
                    case "arp":
                        statistikaprichadzajuca2.arp++;
                        SetTextarp2in(statistikaprichadzajuca2.arp.ToString());
                        break;
                    case "ip":
                        statistikaprichadzajuca2.ip++;
                        SetTextip2in(statistikaprichadzajuca2.ip.ToString());
                        break;
                }
            }
        }

        public void notifylabeloutgoing(string pole, Int32 port)
        {
            if (port == 1)
            {
                switch (pole)
                {
                    case "icmp":
                        statistikaodchadzajuca1.icmp++;
                        SetTexticmp1out(statistikaodchadzajuca1.icmp.ToString());
                        break;
                    case "udp":
                        statistikaodchadzajuca1.udp++;
                        SetTextudp1out(statistikaodchadzajuca1.udp.ToString());
                        break;
                    case "tcp":
                        statistikaodchadzajuca1.tcp++;
                        SetTexttcp1out(statistikaodchadzajuca1.tcp.ToString());
                        break;
                    case "arp":
                        statistikaodchadzajuca1.arp++;
                        SetTextarp1out(statistikaodchadzajuca1.arp.ToString());
                        break;
                    case "ip":
                        statistikaodchadzajuca1.ip++;
                        SetTextip1out(statistikaodchadzajuca1.ip.ToString());
                        break;
                }
            }
            else
            {
                switch (pole)
                {
                    case "icmp":
                        statistikaodchadzajuca2.icmp++;
                        SetTexticmp2out(statistikaodchadzajuca2.icmp.ToString());
                        break;
                    case "udp":
                        statistikaodchadzajuca2.udp++;
                        SetTextudp2out(statistikaodchadzajuca2.udp.ToString());
                        break;
                    case "tcp":
                        statistikaodchadzajuca2.tcp++;
                        SetTexttcp2out(statistikaodchadzajuca2.tcp.ToString());
                        break;
                    case "arp":
                        statistikaodchadzajuca2.arp++;
                        SetTextarp2out(statistikaodchadzajuca2.arp.ToString());
                        break;
                    case "ip":
                        statistikaodchadzajuca2.ip++;
                        SetTextip2out(statistikaodchadzajuca2.ip.ToString());
                        break;
                }
            }
        }


        #region LabelNastavovaciRegion
        private void SetTexticmp1in(string text)
        {
            SynchronizedInvoke(ICMPlabelIn1, delegate() { ICMPlabelIn1.Text = text; });
        }

        private void SetTextudp1in(string text)
        {
            SynchronizedInvoke(UDPLabelIn1, delegate() { UDPLabelIn1.Text = text; });
        }

        private void SetTexttcp1in(string text)
        {
            SynchronizedInvoke(TCPLabelIn1, delegate() { TCPLabelIn1.Text = text; });
        }

        private void SetTextarp1in(string text)
        {
            SynchronizedInvoke(ARPLabelIn1, delegate() { ARPLabelIn1.Text = text; });
        }

        private void SetTextip1in(string text)
        {
            SynchronizedInvoke(IPLabelIn1, delegate() { IPLabelIn1.Text = text; });
        }

        private void SetTexticmp2in(string text)
        {
            SynchronizedInvoke(ICMPLabelIn2, delegate() { ICMPLabelIn2.Text = text; });
        }

        private void SetTextudp2in(string text)
        {
            SynchronizedInvoke(UDPLabelIn2, delegate() { UDPLabelIn2.Text = text; });
        }

        private void SetTexttcp2in(string text)
        {
            SynchronizedInvoke(TCPLabelIn2, delegate() { TCPLabelIn2.Text = text; });
        }

        private void SetTextarp2in(string text)
        {
            SynchronizedInvoke(ARPLabelIn2, delegate() { ARPLabelIn2.Text = text; });
        }

        private void SetTextip2in(string text)
        {
            SynchronizedInvoke(IPLabelIn2, delegate() { IPLabelIn2.Text = text; });
        }

        private void SetTexticmp1out(string text)
        {
            SynchronizedInvoke(ICMPlabelOut1, delegate() { ICMPlabelOut1.Text = text; });
        }

        private void SetTextudp1out(string text)
        {
            SynchronizedInvoke(UDPLabelOut1, delegate() { UDPLabelOut1.Text = text; });
        }

        private void SetTexttcp1out(string text)
        {
            SynchronizedInvoke(TCPLabelOut1, delegate() { TCPLabelOut1.Text = text; });
        }

        private void SetTextarp1out(string text)
        {
            SynchronizedInvoke(ARPLabelOut1, delegate() { ARPLabelOut1.Text = text; });
        }

        private void SetTextip1out(string text)
        {
            SynchronizedInvoke(IPLabelOut1, delegate() { IPLabelOut1.Text = text; });
        }

        private void SetTexticmp2out(string text)
        {
            SynchronizedInvoke(ICMPLabelOut2, delegate() { ICMPLabelOut2.Text = text; });
        }

        private void SetTextudp2out(string text)
        {
            SynchronizedInvoke(UDPLabelOut2, delegate() { UDPLabelOut2.Text = text; });
        }

        private void SetTexttcp2out(string text)
        {
            SynchronizedInvoke(TCPLabelOut2, delegate() { TCPLabelOut2.Text = text; });
        }

        private void SetTextarp2out(string text)
        {
            SynchronizedInvoke(ARPLabelOut2, delegate() { ARPLabelOut2.Text = text; });
        }

        private void SetTextip2out(string text)
        {
            SynchronizedInvoke(IPLabelOut2, delegate() { IPLabelOut2.Text = text; });
        } 
        #endregion


        private void timerpc_Tick(object sender, EventArgs e)
        {
            try
            {
                foreach (DataRow riadok in arptabulka.Rows)
                {
                    int pom = 0;
                    pom = Int32.Parse(riadok["Timer"].ToString());
                    pom -= 1;
                    if (pom == 0)
                    {
                        arptabulka.Rows.Remove(riadok);
                    }
                    else
                    {
                        riadok["Timer"] = pom;
                    }
                }
            }
            catch
            {
            }
        }

        #region PridavanieDoArpTabulky
            public void pridajdoarptabulky(String stringac, Int32 integer)
            {
                SetTextARP(stringac, integer);
            }

            private void SetTextARP(string text, Int32 integer)
            {
                SynchronizedInvoke(arpTabulkaView, delegate() { arpTabulkaView.Rows.Add(text, integer); });
            }
        #endregion

        #region PridavanieDoRoutingTabulky
            public void pridajdoroutingtabulky(String stringac, Int32 integer)
            {
                SetTextRouting(stringac, integer);
            }

            private void SetTextRouting(string text, Int32 integer)
            {
                SynchronizedInvoke(routovaciaTableView, delegate() { routovaciaTableView.Rows.Add(text, integer); });
            }
        #endregion

        public void vymazzarptabulky(Int32 integer)
        {
            int rowindex = -1;
            foreach (DataGridViewRow row in arpTabulkaView.Rows)
            {
                if (row.Cells[2].Value.ToString().Equals(integer))
                {                    
                    arpTabulkaView.Rows.RemoveAt(row.Index);
                }
            }
        }

        static void SynchronizedInvoke(ISynchronizeInvoke sync, Action action)
        {
            // If the invoke is not required, then invoke here and get out.
            if (!sync.InvokeRequired)
            {
                // Execute action.
                action();

                // Get out.
                return;
            }

            // Marshal to the required context.
            sync.Invoke(action, new object[] { });
        } 

        public void posliARPport1(string KamIP)
        {
            zapajaniekablov.PosliARPRequest(KamIP, port1IPadressText.Text, 1);
        }

        public void posliARPport2(string KamIP)
        {
            zapajaniekablov.PosliARPRequest(KamIP, port2IPadressText.Text, 2);
        }
        
        private void button5_Click(object sender, EventArgs e)
        {
            filtrikobrazovka.Show();
        }

        public string Get_IPaddress(Int32 portkablu)
        {
            if (portkablu == 1)
            {
                return port1IPadressText.Text;    
            }
            else if (portkablu == 2)
            {
                return port2IPadressText.Text; 
            }
            return null;

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            arptabulka.Clear();
        }

        public void vycistiRoutingTabulku()
        {
            routingtabulka.Clear();
        }

        private void button6_Click(object sender, EventArgs e)
        {
          
                statistikaprichadzajuca1.icmp = 0;
                SetTexticmp1in(statistikaprichadzajuca1.icmp.ToString());
                statistikaprichadzajuca1.udp = 0;
                SetTextudp1in(statistikaprichadzajuca1.udp.ToString());
                statistikaprichadzajuca1.tcp = 0;
                SetTexttcp1in(statistikaprichadzajuca1.tcp.ToString());
                statistikaprichadzajuca1.arp = 0;
                SetTextarp1in(statistikaprichadzajuca1.arp.ToString());
                statistikaprichadzajuca1.ip = 0;
                SetTextip1in(statistikaprichadzajuca1.ip.ToString());
                statistikaprichadzajuca2.icmp = 0;
                SetTexticmp2in(statistikaprichadzajuca2.icmp.ToString());
                statistikaprichadzajuca2.udp = 0;
                SetTextudp2in(statistikaprichadzajuca2.udp.ToString());
                statistikaprichadzajuca2.tcp = 0;
                SetTexttcp2in(statistikaprichadzajuca2.tcp.ToString());
                statistikaprichadzajuca2.arp = 0;
                SetTextarp2in(statistikaprichadzajuca2.arp.ToString());
                statistikaprichadzajuca2.ip = 0;
                SetTextip2in(statistikaprichadzajuca2.ip.ToString());
                statistikaodchadzajuca1.icmp = 0;
                SetTexticmp1out(statistikaodchadzajuca1.icmp.ToString());
                statistikaodchadzajuca1.udp = 0;
                SetTextudp1out(statistikaodchadzajuca1.udp.ToString());
                statistikaodchadzajuca1.tcp = 0;
                SetTexttcp1out(statistikaodchadzajuca1.tcp.ToString());
                statistikaodchadzajuca1.arp = 0;
                SetTextarp1out(statistikaodchadzajuca1.arp.ToString());
                statistikaodchadzajuca1.ip = 0;
                SetTextip1out(statistikaodchadzajuca1.ip.ToString());
                statistikaodchadzajuca2.icmp = 0;
                SetTexticmp2out(statistikaodchadzajuca2.icmp.ToString());
                statistikaodchadzajuca2.udp = 0;
                SetTextudp2out(statistikaodchadzajuca2.udp.ToString());
                statistikaodchadzajuca2.tcp = 0;
                SetTexttcp2out(statistikaodchadzajuca2.tcp.ToString());
                statistikaodchadzajuca2.arp = 0;
                SetTextarp2out(statistikaodchadzajuca2.arp.ToString());
                statistikaodchadzajuca2.ip = 0;
                SetTextip2out(statistikaodchadzajuca2.ip.ToString());
            
        }

        private void sendARPtoIPButton1_Click(object sender, EventArgs e)
        {
            zapajaniekablov.PosliARPRequest(sendARPtoIPText1.Text, port1IPadressText.Text, 1);
        }

        private void sendARPtoIPButton2_Click(object sender, EventArgs e)
        {
            zapajaniekablov.PosliARPRequest(sendARPtoIPText2.Text, port2IPadressText.Text, 2);
        }

        private void label32_Click(object sender, EventArgs e)
        {

        }
        // tlacitko pre vytvorenie statickej cesty
        private void button7_Click(object sender, EventArgs e)
        {
            zapajaniekablov.vytvorStatickuCestu(sietTextBoxSC.Text, maskaTextBoxSC.Text, nextHopTextBoxSC.Text, interfaceTextBoxSC.Text);
        }
        // tlacitko pre zrusenie statickej cesty
        private void buttonSCZrus_Click(object sender, EventArgs e)
        {
            DataRow row = ((DataRowView)routovaciaTableView.CurrentRow.DataBoundItem).Row;
            if ((row != null) && ((char)row["Typ"]=='S'))
            {
                routingtabulka.Rows.Remove(row);
            }
        }

        private void pingButton_Click(object sender, EventArgs e)
        {
            zapajaniekablov.posliPing(pingTextBox.Text);
        }
        
    }
}
