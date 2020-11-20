using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Sheets
{
    public class ChangeParameter_PopUp : Form 
    {
        public string[] parametre = new string[] { "" };
        public List<string> chosenTypes = new List<string>();
        public List<string> sheetTyper = new List<string>();
        public List<string> paramTypes = new List<string>();
        public int omfang = 1;
        public string parameterNavn = "";
        public string paramValue = "";
        //public bool isBoolParameter = false;
        public string parameterType = "";
        //public bool boolValue = false;    

        public bool cancelled = true;
        public CheckedListBox omfangCheckedList;
        public ComboBox omfangDropdown;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        
        public CheckBox cb_boolValue;
        public ComboBox comboBox_parameterNavn;



        /// Required designer variable.
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }


        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        public void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.AfbrydKnap = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_stringValue = new System.Windows.Forms.TextBox();
            this.Beskrivelse = new System.Windows.Forms.Label();
            this.overskrift = new System.Windows.Forms.Label();
            this.GodkendKnap = new System.Windows.Forms.Button();
            this.omfangCheckedList = new System.Windows.Forms.CheckedListBox();
            this.omfangDropdown = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cb_boolValue = new System.Windows.Forms.CheckBox();
            this.comboBox_parameterNavn = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 14);
            this.label1.TabIndex = 4;
            this.label1.Text = "Angiv parameternavn";
            // 
            // AfbrydKnap
            // 
            this.AfbrydKnap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.AfbrydKnap.BackColor = System.Drawing.Color.Silver;
            this.AfbrydKnap.Cursor = System.Windows.Forms.Cursors.Default;
            this.AfbrydKnap.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.AfbrydKnap.FlatAppearance.BorderSize = 0;
            this.AfbrydKnap.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightCoral;
            this.AfbrydKnap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AfbrydKnap.Font = new System.Drawing.Font("Century Gothic", 8.25F);
            this.AfbrydKnap.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.AfbrydKnap.Location = new System.Drawing.Point(277, 297);
            this.AfbrydKnap.Name = "AfbrydKnap";
            this.AfbrydKnap.Size = new System.Drawing.Size(80, 39);
            this.AfbrydKnap.TabIndex = 3;
            this.AfbrydKnap.Text = "AFBRYD";
            this.AfbrydKnap.UseVisualStyleBackColor = false;
            this.AfbrydKnap.Click += new System.EventHandler(this.LukVindue);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 14);
            this.label2.TabIndex = 4;
            this.label2.Text = "Angiv parameterværdi";
            // 
            // tb_stringValue
            // 
            this.tb_stringValue.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tb_stringValue.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_stringValue.Location = new System.Drawing.Point(156, 47);
            this.tb_stringValue.Name = "tb_stringValue";
            this.tb_stringValue.Size = new System.Drawing.Size(241, 20);
            this.tb_stringValue.TabIndex = 5;
            this.tb_stringValue.Text = "2018/07/19";
            this.tb_stringValue.TextChanged += new System.EventHandler(this.parameterValue_TextChanged);
            // 
            // Beskrivelse
            // 
            this.Beskrivelse.AutoEllipsis = true;
            this.Beskrivelse.AutoSize = true;
            this.Beskrivelse.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Beskrivelse.Location = new System.Drawing.Point(12, 36);
            this.Beskrivelse.Name = "Beskrivelse";
            this.Beskrivelse.Size = new System.Drawing.Size(333, 14);
            this.Beskrivelse.TabIndex = 6;
            this.Beskrivelse.Text = "Dette script ændrer en parameterværdi for valgte sheets i projektet.";
            // 
            // overskrift
            // 
            this.overskrift.AutoEllipsis = true;
            this.overskrift.AutoSize = true;
            this.overskrift.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Bold);
            this.overskrift.Location = new System.Drawing.Point(12, 9);
            this.overskrift.Name = "overskrift";
            this.overskrift.Size = new System.Drawing.Size(185, 18);
            this.overskrift.TabIndex = 6;
            this.overskrift.Text = "ÆNDR SHEET PARAMETER";
            // 
            // GodkendKnap
            // 
            this.GodkendKnap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.GodkendKnap.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.GodkendKnap.Cursor = System.Windows.Forms.Cursors.Default;
            this.GodkendKnap.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.GodkendKnap.FlatAppearance.BorderSize = 0;
            this.GodkendKnap.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LimeGreen;
            this.GodkendKnap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.GodkendKnap.Font = new System.Drawing.Font("Century Gothic", 8.25F);
            this.GodkendKnap.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.GodkendKnap.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.GodkendKnap.Location = new System.Drawing.Point(357, 297);
            this.GodkendKnap.Margin = new System.Windows.Forms.Padding(0);
            this.GodkendKnap.Name = "GodkendKnap";
            this.GodkendKnap.Size = new System.Drawing.Size(80, 39);
            this.GodkendKnap.TabIndex = 3;
            this.GodkendKnap.Text = "GODKEND";
            this.GodkendKnap.UseVisualStyleBackColor = false;
            this.GodkendKnap.Click += new System.EventHandler(this.LukVindue);
            // 
            // omfangCheckedList
            // 
            this.omfangCheckedList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.omfangCheckedList.CheckOnClick = true;
            this.omfangCheckedList.FormattingEnabled = true;
            this.omfangCheckedList.Items.AddRange(new object[] {
            "A4 - Liggende",
            "A4 - Stående",
            "A3 - Stående"});
            this.omfangCheckedList.Location = new System.Drawing.Point(156, 19);
            this.omfangCheckedList.Name = "omfangCheckedList";
            this.omfangCheckedList.Size = new System.Drawing.Size(245, 105);
            this.omfangCheckedList.TabIndex = 14;
            this.omfangCheckedList.Visible = false;
            // 
            // omfangDropdown
            // 
            this.omfangDropdown.BackColor = System.Drawing.SystemColors.Menu;
            this.omfangDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.omfangDropdown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.omfangDropdown.Font = new System.Drawing.Font("Arial", 8.25F);
            this.omfangDropdown.FormattingEnabled = true;
            this.omfangDropdown.Items.AddRange(new object[] {
            "Alle sheets",
            "Efter type ..."});
            this.omfangDropdown.Location = new System.Drawing.Point(9, 19);
            this.omfangDropdown.Name = "omfangDropdown";
            this.omfangDropdown.Size = new System.Drawing.Size(137, 22);
            this.omfangDropdown.TabIndex = 7;
            this.omfangDropdown.SelectedIndexChanged += new System.EventHandler(this.omfangDropdown_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.omfangDropdown);
            this.groupBox1.Controls.Add(this.omfangCheckedList);
            this.groupBox1.Location = new System.Drawing.Point(15, 62);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(407, 135);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Vælg omfang";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.comboBox_parameterNavn);
            this.groupBox2.Controls.Add(this.cb_boolValue);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.tb_stringValue);
            this.groupBox2.Location = new System.Drawing.Point(15, 203);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(407, 79);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Vælg parameter";
            // 
            // cb_boolValue
            // 
            this.cb_boolValue.AutoSize = true;
            this.cb_boolValue.Location = new System.Drawing.Point(156, 51);
            this.cb_boolValue.Name = "cb_boolValue";
            this.cb_boolValue.Size = new System.Drawing.Size(15, 14);
            this.cb_boolValue.TabIndex = 17;
            this.cb_boolValue.UseVisualStyleBackColor = true;
            this.cb_boolValue.Visible = false;
            // 
            // comboBox_parameterNavn
            // 
            this.comboBox_parameterNavn.BackColor = System.Drawing.SystemColors.Menu;
            this.comboBox_parameterNavn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_parameterNavn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox_parameterNavn.Font = new System.Drawing.Font("Arial", 8.25F);
            this.comboBox_parameterNavn.FormattingEnabled = true;
            this.comboBox_parameterNavn.Location = new System.Drawing.Point(156, 18);
            this.comboBox_parameterNavn.Name = "comboBox_parameterNavn";
            this.comboBox_parameterNavn.Size = new System.Drawing.Size(241, 22);
            this.comboBox_parameterNavn.TabIndex = 18;
            this.comboBox_parameterNavn.SelectedIndexChanged += new System.EventHandler(this.parameterNavn_SelectedIndexChanged);
            this.comboBox_parameterNavn.Leave += new System.EventHandler(this.comboBox_parameterNavn_Leave);
            // 
            // ChangeParameter_PopUp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.AfbrydKnap;
            this.ClientSize = new System.Drawing.Size(437, 336);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.GodkendKnap);
            this.Controls.Add(this.overskrift);
            this.Controls.Add(this.Beskrivelse);
            this.Controls.Add(this.AfbrydKnap);
            this.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.HelpButton = true;
            this.Name = "ChangeParameter_PopUp";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sheets - Ændr parameter";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.Red;
            this.Load += new System.EventHandler(this.PopUpVindue_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button AfbrydKnap;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox tb_stringValue;
        private System.Windows.Forms.Label Beskrivelse;
        private System.Windows.Forms.Label overskrift;
        private System.Windows.Forms.Button GodkendKnap;


        #region Klikfunktioner
        // CONSTRUCTOR
        public ChangeParameter_PopUp(SortedDictionary<string,string> parameterNamesAndValues, List<string> sheetTyperX)
        {
            paramTypes = parameterNamesAndValues.Values.ToList();
            parametre = parameterNamesAndValues.Keys.ToArray();
            InitializeComponent();

            this.comboBox_parameterNavn.Items.Clear();
            this.comboBox_parameterNavn.Items.AddRange(parametre);
            this.comboBox_parameterNavn.SelectedIndex = 0;


            this.omfangDropdown.SelectedIndex = 0;
            this.omfangCheckedList.Items.Clear();
            this.omfangCheckedList.Items.AddRange(sheetTyperX.ToArray());

            this.sheetTyper = sheetTyperX;
        }

        private void PopUpVindue_Load(object sender, EventArgs e)
        {
            
        }

        private void LukVindue(object sender, EventArgs e)
        {
            // FUNKTIONEN SØRGER FOR AT SENDE OUTPUT UD, NÅR VINDUE LUKKES VED 'GODKEND' ELLER 'AFBRYD'

            // Tilføjer indtastning til output
            // Laver objektet 'sender' til en knap, så .Name mm. kan læses
            Button btSender = (Button)sender;

            if (btSender.Name == "GodkendKnap")
            { 
                cancelled = false;

                //parameterNavn = tb_parameterNavn.Text;
                parameterNavn = comboBox_parameterNavn.SelectedItem.ToString();

                if (parameterType != "YesNo")
                {
                    paramValue = tb_stringValue.Text;
                }
                else
                {
                    paramValue = cb_boolValue.Checked ? "1" : "0";
                }

                omfang = omfangDropdown.SelectedIndex;

                if (omfang == 1)
                {
                    foreach (int i in omfangCheckedList.CheckedIndices)
                    {
                        chosenTypes.Add(sheetTyper[i]);
                    }
                }
                Close();

            }
            else if(btSender.Name == "AfbrydKnap")
            {
                // Sætter afbrudt til true:
                cancelled = true;
                // Lukker vindue
                Close();
            }
            

        }
        
        private void parameterNavn_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            //tb_parameterNavn.Text = cb.Text;

            cb_boolValue.Visible = false;
            tb_stringValue.Visible = true;

            // Viser advarsel, hvis parameternavn ikke er fundet
            //paramIkkeFundet.Visible = !parametre.ToList().Contains(cb.Text);

            //if (!paramIkkeFundet.Visible)
            //{
            parameterType = paramTypes[parametre.ToList().IndexOf(comboBox_parameterNavn.SelectedItem.ToString())];
            if (parameterType == "YesNo")
            {
                cb_boolValue.Visible = true;
                tb_stringValue.Visible = false;
                //isBoolParameter = true;
            }
            //}
        }
        /*
        private void parameterNavn_Leave(object sender, EventArgs e)
        {
            cb_paramYesNo.Visible = false;
            tb_parameterValue.Visible = true;

            TextBox tb = (TextBox)sender;

            // Viser advarsel, hvis parameternavn ikke er fundet
            paramIkkeFundet.Visible = !parametre.ToList().Contains(tb.Text);

            if (!paramIkkeFundet.Visible)
            {
                if (paramTypes[parametre.ToList().IndexOf(tb.Text)] == "YesNo")
                {
                    cb_paramYesNo.Visible      = true;
                    tb_parameterValue.Visible  = false;
                    isTextParameter         = false;
                }

            }
        }
        */
    
        private void parameterValue_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb_stringValue.Text = tb.Text;
        }
        #endregion

        private void omfangDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox btSender = (ComboBox)sender;

            this.omfangCheckedList.Visible = false;
            this.omfangCheckedList.Enabled = false;

            switch (btSender.SelectedItem.ToString())
            {
                case "Efter type ...":
                    omfangCheckedList.Visible = true;
                    omfangCheckedList.Enabled = true;

                    break;

                default:

                    break;
            }
            omfangCheckedList.Update();
        }

        private void comboBox_parameterNavn_Leave(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            try
            {
                cb.SelectedIndex = cb.Items.IndexOf(cb.Text);
            }
            catch
            {
                cb.SelectedIndex = 0;
            }
        }
    }
}
