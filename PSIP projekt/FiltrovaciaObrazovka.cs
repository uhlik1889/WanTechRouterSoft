using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PcapDotNet.Base;

namespace PSIP_projekt
{
    public partial class FiltrovaciaObrazovka : Form
    {
        public DataTable filtrovaciatabulka = new DataTable();
        public Filtrovanie filtracnaklasa = null;
        DataColumn dstmacadresapaketu = new DataColumn();
        DataColumn dstipadresapaketu = new DataColumn();
        DataColumn srcmacadresapaketu = new DataColumn();
        DataColumn srcipadresapaketu = new DataColumn();
        DataColumn dstportpaketu = new DataColumn();
        DataColumn srcportpaketu = new DataColumn();
        DataColumn inpaketu = new DataColumn();
        DataColumn permitpaketu = new DataColumn();
        DataColumn interfacepaketu = new DataColumn();

        DataColumn UDPcolumn = new DataColumn();
        DataColumn TCPcolumn = new DataColumn();
        DataColumn ARPcolumn = new DataColumn();
        DataColumn ICMPcolumn = new DataColumn();
        DataColumn IPcolumn = new DataColumn();

        public DataView view = null;
        private Control hlavnaforma = null;
        

        public FiltrovaciaObrazovka(Control form)
        {
            hlavnaforma = form;
            InitializeComponent();

            filtracnaklasa = new Filtrovanie(this);

            //   pridavanie stlpoc do filter tabulky
            dstmacadresapaketu.DataType = System.Type.GetType("System.String");
            dstmacadresapaketu.ColumnName = "DSTMacAdresa";
            filtrovaciatabulka.Columns.Add(dstmacadresapaketu);

            dstipadresapaketu.DataType = System.Type.GetType("System.String");
            dstipadresapaketu.ColumnName = "DSTIpAdresa";
            filtrovaciatabulka.Columns.Add(dstipadresapaketu);

            srcmacadresapaketu.DataType = System.Type.GetType("System.String");
            srcmacadresapaketu.ColumnName = "SRCMacAdresa";
            filtrovaciatabulka.Columns.Add(srcmacadresapaketu);

            srcipadresapaketu.DataType = System.Type.GetType("System.String");
            srcipadresapaketu.ColumnName = "SRCIpAdresa";
            filtrovaciatabulka.Columns.Add(srcipadresapaketu);

            dstportpaketu.DataType = System.Type.GetType("System.Int32");
            dstportpaketu.ColumnName = "DSTPort";
            filtrovaciatabulka.Columns.Add(dstportpaketu);

            srcportpaketu.DataType = System.Type.GetType("System.Int32");
            srcportpaketu.ColumnName = "SRCPort";
            filtrovaciatabulka.Columns.Add(srcportpaketu);

            inpaketu.DataType = System.Type.GetType("System.Boolean");
            inpaketu.ColumnName = "In";
            filtrovaciatabulka.Columns.Add(inpaketu);

            permitpaketu.DataType = System.Type.GetType("System.Boolean");
            permitpaketu.ColumnName = "Permit";
            filtrovaciatabulka.Columns.Add(permitpaketu);

            interfacepaketu.DataType = System.Type.GetType("System.Int32");
            interfacepaketu.ColumnName = "Interface";
            filtrovaciatabulka.Columns.Add(interfacepaketu);

            //////////////////////////////////////////////////////////////////
            UDPcolumn.DataType = System.Type.GetType("System.Boolean");
            UDPcolumn.ColumnName = "UDP";
            filtrovaciatabulka.Columns.Add(UDPcolumn);

            TCPcolumn.DataType = System.Type.GetType("System.Boolean");
            TCPcolumn.ColumnName = "TCP";
            filtrovaciatabulka.Columns.Add(TCPcolumn);

            ARPcolumn.DataType = System.Type.GetType("System.Boolean");
            ARPcolumn.ColumnName = "ARP";
            filtrovaciatabulka.Columns.Add(ARPcolumn);

            ICMPcolumn.DataType = System.Type.GetType("System.Boolean");
            ICMPcolumn.ColumnName = "ICMP";
            filtrovaciatabulka.Columns.Add(ICMPcolumn);

            IPcolumn.DataType = System.Type.GetType("System.Boolean");
            IPcolumn.ColumnName = "IP";
            filtrovaciatabulka.Columns.Add(IPcolumn);


            view = new DataView(filtrovaciatabulka);
            dataGridView1.DataSource = view;
        }

        public void AddException_Click(object sender, EventArgs e)
        {
            DataRow row = filtrovaciatabulka.NewRow();
            row["DSTMacAdresa"] = macDSTAdresaTextBox.Text;
            row["DSTIpAdresa"] = ipDSTAdresaTextBox.Text;
            row["SRCMacAdresa"] = macSRCAdresaTextBox.Text;
            row["SRCIpAdresa"] = ipSRCAdresaTextBox.Text;

            if (dstportTextBox.Text.ToString().IsNullOrEmpty())
            {
                row["DSTPort"] = 0;
            }
            else
            {
                row["DSTPort"] = Int32.Parse(dstportTextBox.Text);
            }

            if (srcportTextBox.Text.ToString().IsNullOrEmpty())
            {
                row["SRCPort"] = 0;
            }
            else
            {
                row["SRCPort"] = Int32.Parse(srcportTextBox.Text);
            }

            if (inRadioButtonTrue.Checked)
            {
                row["In"] = true;
            }
            else
            {
                row["In"] = false;
            }

            if (permitRadioButtonTrue.Checked)
            {
                row["Permit"] = true;
            }
            else
            {
                row["Permit"] = false;
            }

            if (interfaceTextBox.Text.ToString().IsNullOrEmpty())
            {
                row["Interface"] = 0;
            }
            else
            {
                row["Interface"] = interfaceTextBox.Text;
            }
            row["UDP"] = false;
            row["TCP"] = false;
            row["ICMP"] = false;
            row["ARP"] = false;
            row["IP"] = false;
            if (radioButton1.Checked)
            {
                row["UDP"] = true;
            }
            else if (radioButton2.Checked)
            {
                row["TCP"] = true;
            }
            else if (radioButton3.Checked)
            {
                row["ICMP"] = true;
            }
            else if (radioButton4.Checked)
            {
                row["ARP"] = true;
            }
            else if (radioButton5.Checked)
            {
                row["IP"] = true;
            }
            filtrovaciatabulka.Rows.Add(row);
        }

        private void FiltrovaciaObrazovka_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true; 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            filtrovaciatabulka.Clear();
        }

        

    }
}
