using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;               
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Simbag
{
    public partial class Form1 : Form
    {
        //instance variables
        string SelectedElement = "";
        string IdTransport = "T0";
        string IdCheckin = "C0";
        string IdConveyorMerge = "CM0";
        string IdConveyorSplit = "CS0";
        string IdSecurity = "S0";
        string IdGate = "G0";
        string lastIssuedId = "Bag A-1";
        int clicks = 1;
        int state = 0;
        int drawdelay = 0;
        int bagindex = 0;
        bool saved = true;
        bool secondclick = false;
        Node one = null;
        Node two = null;
        Node three = null;
        Builder builder;
        BHSReport report;
        List<Bag> bagList;
        List<Bag> placeholderBagList;
        
        public Form1()
        {
            InitializeComponent();
            builder = new Builder();
            bagList = new List<Bag>();
            placeholderBagList = new List<Bag>();
            report = new BHSReport(0, 0, 0, 0, null, 0, 0, 0, 0, 0);

            // Create the ToolTip and associate with the Form container.
            ToolTip toolTip1 = new ToolTip();

            // Set up the delays for the ToolTip.
            toolTip1.AutoPopDelay = 1000;
            toolTip1.InitialDelay = 500;
            toolTip1.ReshowDelay = 100;
            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip1.ShowAlways = true;

            // Set up the ToolTip text for the Button and Checkbox.
            toolTip1.SetToolTip(this.btnCheckIn, "Check-in");
            toolTip1.SetToolTip(this.btnSecCheck, "Security check");
            toolTip1.SetToolTip(this.btnConvBelt, "Conveyor belt");
            toolTip1.SetToolTip(this.btnConvMerge, "Conveyor merge");
            toolTip1.SetToolTip(this.btnConvSplit, "Conveyor split");
            toolTip1.SetToolTip(this.btnGate, "Gate");

            //Disable all tab controls and empty the properties tab
            DisablePropertiesTab();
        }

        // Helper methods to reduce code repetition
        private void SelectButtons(string buttonType)
        {
            if (SelectedElement == buttonType)
            {
                SelectedElement = "";
                lblSelectedElem.Text = SelectedElement;
                three = null;
                DisablePropertiesTab();
                pictureBoxBHSBuild.Focus();
                pictureBoxBHSBuild.Invalidate();
            }
            else
            {
                SelectedElement = buttonType;
                lblSelectedElem.Text = SelectedElement;
                lblSelectedElem.Visible = true;
            }
        }
        private void FillDestinationBox()
        {
            cbFinalDestination.Items.Clear();
            foreach (Node n in builder.mymap.Nodes)
            {
                if (n is Gate)
                {
                    cbFinalDestination.Items.Add(n.id);
                }
            }
        }
        private void ConfigureAllBags()
        {
            Random r = new Random();
            CheckIn ci = null;
            bagList = new List<Bag>();
            foreach (Node n in builder.mymap.Nodes)
            {
                if (n is CheckIn)
                {
                    ci = (CheckIn)n;
                    for (int i = 0; i < ci.NumberOfBags; i++)
                    {
                        bool suspicious = false;
                        if (r.Next(1, 100) <= ci.ChanceSuspicious) { suspicious = true; }
                        bagList.Add(new Bag(MakeId(), n, ci.FinalDestination, suspicious));
                    }
                }
            }
        }
        private void DisablePropertiesTab()
        {
            //Disable all tabcontrols
            tpCheckIn.Enabled = false;
            tpSecCheck.Enabled = false;
            tpConvBelt.Enabled = false;
            tpConvSplit.Enabled = false;
            tpConvMerge.Enabled = false;
            tpGate.Enabled = false;

            //empty all textboxes
            tbCheckInName.Clear();
            tbCheckInX.Clear();
            tbCheckInY.Clear();
            tbSecChkName.Clear();
            tbSecChkX.Clear();
            tbSecChkY.Clear();
            tbConvSplitName.Clear();
            tbConvSplitX.Clear();
            tbConvSplitY.Clear();
            tbConvMergeName.Clear();
            tbConvMergeX.Clear();
            tbConvMergeY.Clear();
            tbGateName.Clear();
            tbGateX.Clear();
            tbGateY.Clear();
            cbFinalDestination.SelectedIndex = -1;
            nudBagPerCheckIn.Value = 0;
            nudChanceSuspicious.Value = 0;
        }
        private void SaveBHS()
        {
            DialogResult result = saveFileDialog1.ShowDialog();
            FileStream fs = null;
            BinaryFormatter bf = null;

            try
            {



                if (result == DialogResult.OK)
                {
                    fs = new FileStream(this.saveFileDialog1.FileName, FileMode.Create, FileAccess.Write);
                    bf = new BinaryFormatter();
                    bf.Serialize(fs, builder);
                    saved = true;
                }

            }
            catch (SerializationException)
            { MessageBox.Show("something wrong with Serialization"); }
            catch (IOException)
            { MessageBox.Show("something wrong with IO"); }
            finally
            {
                if (fs != null) fs.Close();
            }
        }
        private void LoadBHS()
        {
            openFileDialog1.FileName = "";
            DialogResult result = openFileDialog1.ShowDialog();
            FileStream fs = null;
            BinaryFormatter bf = null;

            try
            {
                if (result == DialogResult.OK)
                {
                    fs = new FileStream(this.openFileDialog1.FileName, FileMode.Open, FileAccess.Read);
                    bf = new BinaryFormatter();
                    builder = (Builder)(bf.Deserialize(fs));
                    pictureBoxBHSSim.Invalidate();
                    pictureBoxBHSBuild.Invalidate();
                }
            }
            catch (SerializationException)
            { MessageBox.Show("Invalid file type"); }
            catch (IOException)
            { MessageBox.Show("Something went wrong with IO"); }
            finally
            {
                if (fs != null) fs.Close();
            }
        }
        private string MakeId()
        {
            int number = Convert.ToInt32(lastIssuedId.Substring(5));
            char letter = Convert.ToChar(lastIssuedId.Substring(4, 1));
            if (number < 9)
            {
                number++;
            }
            else
            {
                letter++;
                number = 0;
            }
            lastIssuedId = "Bag " + letter.ToString() + number.ToString();
            return lastIssuedId;
        }
        private void UpdateTransportLength(Node selectedNode)
        {
            foreach (Transport t in selectedNode.Connections)
            {
                t.Length = selectedNode.StraightLineDistanceTo(t.ConnectedNode);
            }
            foreach (Node n in builder.mymap.Nodes)
            {
                foreach (Transport t in n.Connections)
                {
                    if (t.ConnectedNode.id == selectedNode.id)
                    {
                        t.Length = n.StraightLineDistanceTo(t.ConnectedNode);
                    }
                }
            }
        }

        // Form events
        // Toolbox panel events
        private void btnCheckIn_Click(object sender, EventArgs e)
        {
            SelectButtons("CHECKIN");
        }
        private void btnSecCheck_Click(object sender, EventArgs e)
        {
            SelectButtons("SECURITY");
        }
        private void btnConvBelt_Click(object sender, EventArgs e)
        {
            SelectButtons("CONVEYOR");
        }
        private void btnConvMerge_Click(object sender, EventArgs e)
        {
            SelectButtons("CONVEYORMERGE");
        }
        private void btnGate_Click(object sender, EventArgs e)
        {
            SelectButtons("GATE");
        }
        private void btnConvSplit_Click(object sender, EventArgs e)
        {
            SelectButtons("CONVEYORSPLIT");
        }
        private void btnDeleteNode_Click(object sender, EventArgs e)
        {
            SelectButtons("DELETE");
        }
        private void btnSimStart_Click(object sender, EventArgs e)
        {
            double suspiciosavg = 0;
            double normalavg = 0;
            ConfigureAllBags();

            state = 0;
            AStarSearch astar;

            foreach (Bag b in bagList)
            {

                builder.mymap.StartNode = b.StartingPoint;
                builder.mymap.EndNode = b.EndPoint;

                astar = new AStarSearch(builder.mymap, b);
                List<Node> tempNodeList = astar.Search();
                b.GetNextLocation(tempNodeList);
                if (b.Suspicious)
                {
                    report.SuspiciousBags++;
                    suspiciosavg += astar.DistanceTraveled;
                    report.SuspiciosDistance = astar.DistanceTraveled;
                }
                else
                {
                    report.NormalBags++;
                    normalavg += astar.DistanceTraveled;
                    report.NormalDistance = astar.DistanceTraveled;
                }

            }

            switch (trackBarSpeed.Value)
            {
                case 1:
                    timer1.Interval = 30;
                    break;
                case 2:
                    timer1.Interval = 15;
                    break;
                case 3:
                    timer1.Interval = 1;
                    break;
            }


            timer1.Enabled = true;
            btnSimStart.Enabled = false;
            btnLoadBHS.Enabled = false;

            report.AvgDistance = (suspiciosavg + normalavg) / bagList.Count;
        }
        private void btnLoadBHS_Click(object sender, EventArgs e)
        {
            LoadBHS();
        }
        private void btnSimPause_Click(object sender, EventArgs e)
        {


        }
        private void btnSimPause_MouseClick(object sender, MouseEventArgs e)

        {
            if (e.Button == MouseButtons.Left && secondclick == false)
            {
                state = 1;
                btnSimPause.Text = "Unpause";
                //timer1.Enabled = true;
                //secondclick = true;
            }
            else
            {
                state = 0;
                btnSimPause.Text = "Pause";
                timer1.Enabled = true;

            }
            secondclick = !secondclick;
        }
        private void btnSimStop_Click(object sender, EventArgs e)
        {
            state = 2;

            drawdelay = 0;
            bagindex = 0;



            btnSimStart.Enabled = true;
            btnLoadBHS.Enabled = true;
            timer1.Enabled = false;
            Graphics gr = pictureBoxBHSSim.CreateGraphics();
            foreach (Bag b in this.placeholderBagList)
            {
                b.Draw(gr, state);

            }
            this.placeholderBagList = new List<Bag>();
            pictureBoxBHSSim.Invalidate();
        }
        private void btnSimStop_MouseClick(object sender, MouseEventArgs e)
        {
            //timer1.Enabled = false;
        }
        private void trackBarSpeed_Scroll(object sender, EventArgs e)
        {
            switch (trackBarSpeed.Value)
            {
                case 1:
                    timer1.Interval = 30;
                    break;
                case 2:
                    timer1.Interval = 15;
                    break;
                case 3:
                    timer1.Interval = 1;
                    break;
            }
        }

        // PictureBox build/sim events
        private void pictureBoxBHSBuild_Click(object sender, MouseEventArgs e)
        {
            try
            {
                lblError.Text = "";
                if (e.Button == MouseButtons.Left)
                {
                    if (SelectedElement == "CONVEYOR" /*|| SelectedElement == "TROLLEY"*/ && clicks == 1)

                    {
                        Point p = new Point(e.X, e.Y);

                        one = builder.CheckNode(p);

                        if (one != null)
                        {
                            if (one.CheckConnections() != true)
                            {
                                clicks = 1;
                                one = null;
                                two = null;
                                lblError.Text = "There isn't a free output";
                            }
                            else
                            {
                                clicks = 2;
                            }
                        }
                        else
                        {
                            clicks = 1;
                            one = null;
                            two = null;
                            lblError.Text = "Select a component";
                        }
                    }
                    else if (clicks == 2)
                    {

                        Point p = new Point(e.X, e.Y);
                        two = builder.CheckNode(p);
                        if (two != null)
                        {
                            if (two == one)
                            {
                                clicks = 1;
                                one = null;
                                two = null;
                                lblError.Text = "Can't connect to the same element";
                            }
                            else
                            {
                                double difX = two.Location.X - one.Location.X;
                                double difY = two.Location.Y - one.Location.Y;
                                double lengthsquared = (difX * difX) + (difY * difY);
                                double length = Math.Sqrt(lengthsquared);

                                IdTransport = Regex.Replace(IdTransport, "\\d+", m => (int.Parse(m.Value) + 1).ToString(new string('0', m.Value.Length)));
                                if (two is ConveyorMerge || two is Gate)
                                {
                                    one.MakeConnection(two);
                                    two.Connected = true;
                                }
                                else
                                {
                                    if (two.Connected) { lblError.Text = "There isn't a free input"; }
                                    else { one.MakeConnection(two); two.Connected = true; }
                                }
                            }

                            one = null;
                            two = null;
                            clicks = 1;
                        }
                        else
                        {
                            lblError.Text = "Select an element";
                            clicks = 1;
                            one = null;
                            two = null;
                        }
                    }

                    if (SelectedElement == "CHECKIN")
                    {
                        IdCheckin = Regex.Replace(IdCheckin, "\\d+", m => (int.Parse(m.Value) + 1).ToString(new string('0', m.Value.Length)));
                        builder.AddToList(SelectedElement, IdCheckin, new Point(e.X, e.Y));
                    }
                    if (SelectedElement == "GATE")
                    {
                        IdGate = Regex.Replace(IdGate, "\\d+", m => (int.Parse(m.Value) + 1).ToString(new string('0', m.Value.Length)));
                        builder.AddToList(SelectedElement, IdGate, new Point(e.X, e.Y));
                    }
                    if (SelectedElement == "SECURITY")
                    {
                        IdSecurity = Regex.Replace(IdSecurity, "\\d+", m => (int.Parse(m.Value) + 1).ToString(new string('0', m.Value.Length)));
                        builder.AddToList(SelectedElement, IdSecurity, new Point(e.X, e.Y));
                    }
                    if (SelectedElement == "CONVEYORSPLIT")
                    {
                        IdConveyorSplit = Regex.Replace(IdConveyorSplit, "\\d+", m => (int.Parse(m.Value) + 1).ToString(new string('0', m.Value.Length)));
                        builder.AddToList(SelectedElement, IdConveyorSplit, new Point(e.X, e.Y));
                    }
                    if (SelectedElement == "CONVEYORMERGE")
                    {
                        IdConveyorMerge = Regex.Replace(IdConveyorMerge, "\\d+", m => (int.Parse(m.Value) + 1).ToString(new string('0', m.Value.Length)));
                        builder.AddToList(SelectedElement, IdConveyorMerge, new Point(e.X, e.Y));
                    }
                    if (SelectedElement == "DELETE")
                    {
                        builder.DeleteAnything(new Point(e.X, e.Y));



                    }
                    if (SelectedElement == "")
                    {
                        Point p = new Point(e.X, e.Y);
                        three = builder.CheckNode(p);
                        if (three != null)
                        {
                            if (three is CheckIn)
                            {
                                DisablePropertiesTab();
                                tabsParameters.SelectedIndex = 0;
                                tpCheckIn.Enabled = true;
                                tbCheckInName.Text = three.id;
                                tbCheckInX.Text = three.Location.X.ToString();
                                tbCheckInY.Text = three.Location.Y.ToString();
                                CheckIn ci = (CheckIn)three;
                                nudBagPerCheckIn.Value = ci.NumberOfBags;
                                nudChanceSuspicious.Value = ci.ChanceSuspicious;
                                for (int i = 0; i < cbFinalDestination.Items.Count; i++)
                                {
                                    if (ci.FinalDestination == null) { cbFinalDestination.SelectedIndex = -1; }
                                    else if (cbFinalDestination.Items[i].ToString() == ci.FinalDestination.id)
                                    {
                                        cbFinalDestination.SelectedIndex = i;
                                    }
                                }
                            }
                            if (three is SecurityCheckPoint)
                            {
                                DisablePropertiesTab();
                                tabsParameters.SelectedIndex = 1;
                                tpSecCheck.Enabled = true;
                                tbSecChkName.Text = three.id;
                                tbSecChkX.Text = three.Location.X.ToString();
                                tbSecChkY.Text = three.Location.Y.ToString();
                            }
                            //if (three is Transport)
                            //{
                            //    // not yet implemented, might not need any properties
                            //}
                            if (three is ConveyorSplit)
                            {
                                DisablePropertiesTab();
                                tabsParameters.SelectedIndex = 3;
                                tpConvSplit.Enabled = true;
                                tbConvSplitName.Text = three.id;
                                tbConvSplitX.Text = three.Location.X.ToString();
                                tbConvSplitY.Text = three.Location.Y.ToString();
                            }
                            if (three is ConveyorMerge)
                            {
                                DisablePropertiesTab();
                                tabsParameters.SelectedIndex = 4;
                                tpConvMerge.Enabled = true;
                                tbConvMergeName.Text = three.id;
                                tbConvMergeX.Text = three.Location.X.ToString();
                                tbConvMergeY.Text = three.Location.Y.ToString();
                            }
                            if (three is Gate)
                            {
                                DisablePropertiesTab();
                                tabsParameters.SelectedIndex = 5;
                                tpGate.Enabled = true;
                                tbGateName.Text = three.id;
                                tbGateX.Text = three.Location.X.ToString();
                                tbGateY.Text = three.Location.Y.ToString();
                            }
                        }
                    }

                    // builder.CalculateFlow();
                    pictureBoxBHSBuild.Invalidate();
                }


                else if (e.Button == MouseButtons.Right)
                {
                    SelectedElement = "";
                    lblSelectedElem.Text = SelectedElement;
                    //logic for selecting a node in the future
                    three = null;
                    DisablePropertiesTab();
                    pictureBoxBHSBuild.Invalidate();
                }
                saved = false;
            }
            catch (StackOverflowException)
            {
                lblError.Text = "You can't connect those together";
            }
        }
        private void pictureBoxBHSBuild_Paint(object sender, PaintEventArgs e)
        {
            if (saved) { lblSaved.Text = "Saved"; }
            else { lblSaved.Text = "Unsaved changes"; }
            builder.mymap.Draw(e.Graphics);
            FillDestinationBox();
        }
        private void pictureBoxBHSSim_Paint(object sender, PaintEventArgs e)
        {
            builder.mymap.Draw(e.Graphics);


            if (bagList.Count == 1)
            {
                placeholderBagList = bagList;

            }
            else
            {
                if (bagindex < bagList.Count && (drawdelay == 25 || drawdelay == 0))
                {
                    placeholderBagList.Add(bagList[bagindex]);
                    bagindex++;
                    drawdelay = 0;
                }
                drawdelay++;

            }
            foreach (Bag b in placeholderBagList)
            {
                b.Draw(e.Graphics, state);

            }


        }

        // Not-visible element events
        private void timer1_Tick(object sender, EventArgs e)
        {
            pictureBoxBHSSim.Refresh();
            bool done = true;
            foreach (Bag b in bagList)
            {
                if (b.finished == false) { done = false; }
            }
            if (done)
            {
                btnSimStop_Click(this, new EventArgs());
            }
        }

        // Menustrip events
        private void newDesignToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!saved)
            {
                DialogResult result = MessageBox.Show("You have unsaved changes. do you want to save?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes) { SaveBHS(); }
                else if (result == DialogResult.No) { Application.Restart(); }
                else if (result == DialogResult.Cancel) { }
            }
            else
                Application.Restart();
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!saved)
            {
                DialogResult result = MessageBox.Show("You have unsaved changes. do you want to save?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes) { SaveBHS(); }
                else if (result == DialogResult.No) { Application.Exit(); }
                else if (result == DialogResult.Cancel) { }
            }
            else
                Application.Exit();
        }
        private void saveDesignToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveBHS();
        }
        private void loadDesignToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadBHS();
        }
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, Application.StartupPath + "\\help.chm");
        }

        // Properties panel events
        private void tbCheckInName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrWhiteSpace(tbCheckInName.Text))
                {
                    MessageBox.Show("Invalid name!");
                    tbCheckInName.Clear();
                    tbCheckInName.Focus();
                }
                else
                {
                    three.id = tbCheckInName.Text;
                    e.SuppressKeyPress = true;
                }
            }
        }
        private void tbCheckInX_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrWhiteSpace(tbCheckInX.Text))
                {
                    MessageBox.Show("Invalid value!");
                    tbCheckInX.Clear();
                    tbCheckInX.Focus();
                }
                else
                {
                    try
                    {
                        three.location.X = Convert.ToInt32(tbCheckInX.Text);
                        e.SuppressKeyPress = true;
                        UpdateTransportLength(three);
                        pictureBoxBHSBuild.Invalidate();
                    }
                    catch
                    {
                        MessageBox.Show("Invalid value!");
                        tbCheckInX.Text = three.location.X.ToString();
                        tbCheckInX.Focus();
                    }
                }
            }
        }
        private void tbCheckInY_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrWhiteSpace(tbCheckInY.Text))
                {
                    MessageBox.Show("Invalid value!");
                    tbCheckInY.Clear();
                    tbCheckInY.Focus();
                }
                else
                {
                    try
                    {
                        three.location.Y = Convert.ToInt32(tbCheckInY.Text);
                        e.SuppressKeyPress = true;
                        UpdateTransportLength(three);
                        pictureBoxBHSBuild.Invalidate();
                    }
                    catch
                    {
                        MessageBox.Show("Invalid value!");
                        tbCheckInY.Text = three.location.Y.ToString();
                        tbCheckInY.Focus();
                    }
                }
            }
        }
        private void tbSecChkName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrWhiteSpace(tbSecChkName.Text))
                {
                    MessageBox.Show("Invalid name!");
                    tbSecChkName.Clear();
                    tbSecChkName.Focus();
                }
                else
                {
                    three.id = tbSecChkName.Text;
                    e.SuppressKeyPress = true;
                }
            }
        }
        private void tbSecChkX_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrWhiteSpace(tbSecChkX.Text))
                {
                    MessageBox.Show("Invalid value!");
                    tbSecChkX.Clear();
                    tbSecChkX.Focus();
                }
                else
                {
                    try
                    {
                        three.location.X = Convert.ToInt32(tbSecChkX.Text);
                        e.SuppressKeyPress = true;
                        UpdateTransportLength(three);
                        pictureBoxBHSBuild.Invalidate();
                    }
                    catch
                    {
                        MessageBox.Show("Invalid value!");
                        tbSecChkX.Text = three.location.X.ToString();
                        tbSecChkX.Focus();
                    }
                }
            }
        }
        private void tbSecChkY_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrWhiteSpace(tbSecChkY.Text))
                {
                    MessageBox.Show("Invalid value!");
                    tbSecChkY.Clear();
                    tbSecChkY.Focus();
                }
                else
                {
                    try
                    {
                        three.location.Y = Convert.ToInt32(tbSecChkY.Text);
                        e.SuppressKeyPress = true;
                        UpdateTransportLength(three);
                        pictureBoxBHSBuild.Invalidate();
                    }
                    catch
                    {
                        MessageBox.Show("Invalid value!");
                        tbSecChkY.Text = three.location.Y.ToString();
                        tbSecChkY.Focus();
                    }
                }
            }
        }
        private void tbConvSplitName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrWhiteSpace(tbConvSplitName.Text))
                {
                    MessageBox.Show("Invalid name!");
                    tbConvSplitName.Clear();
                    tbConvSplitName.Focus();
                }
                else
                {
                    three.id = tbConvSplitName.Text;
                    e.SuppressKeyPress = true;
                }
            }
        }
        private void tbConvSplitX_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrWhiteSpace(tbConvSplitX.Text))
                {
                    MessageBox.Show("Invalid value!");
                    tbConvSplitX.Clear();
                    tbConvSplitX.Focus();
                }
                else
                {
                    try
                    {
                        three.location.X = Convert.ToInt32(tbConvSplitX.Text);
                        e.SuppressKeyPress = true;
                        UpdateTransportLength(three);
                        pictureBoxBHSBuild.Invalidate();
                    }
                    catch
                    {
                        MessageBox.Show("Invalid value!");
                        tbConvSplitX.Text = three.location.X.ToString();
                        tbConvSplitX.Focus();
                    }
                }
            }
        }
        private void tbConvSplitY_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrWhiteSpace(tbConvSplitY.Text))
                {
                    MessageBox.Show("Invalid value!");
                    tbConvSplitY.Clear();
                    tbConvSplitY.Focus();
                }
                else
                {
                    try
                    {
                        three.location.Y = Convert.ToInt32(tbConvSplitY.Text);
                        e.SuppressKeyPress = true;
                        UpdateTransportLength(three);
                        pictureBoxBHSBuild.Invalidate();
                    }
                    catch
                    {
                        MessageBox.Show("Invalid value!");
                        tbConvSplitY.Text = three.location.Y.ToString();
                        tbConvSplitY.Focus();
                    }
                }
            }
        }
        private void tbConvMergeName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrWhiteSpace(tbConvMergeName.Text))
                {
                    MessageBox.Show("Invalid name!");
                    tbConvMergeName.Clear();
                    tbConvMergeName.Focus();
                }
                else
                {
                    three.id = tbConvMergeName.Text;
                    e.SuppressKeyPress = true;
                }
            }
        }
        private void tbConvMergeX_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrWhiteSpace(tbConvMergeX.Text))
                {
                    MessageBox.Show("Invalid value!");
                    tbConvMergeX.Clear();
                    tbConvMergeX.Focus();
                }
                else
                {
                    try
                    {
                        three.location.X = Convert.ToInt32(tbConvMergeX.Text);
                        e.SuppressKeyPress = true;
                        UpdateTransportLength(three);
                        pictureBoxBHSBuild.Invalidate();
                    }
                    catch
                    {
                        MessageBox.Show("Invalid value!");
                        tbConvMergeX.Text = three.location.X.ToString();
                        tbConvMergeX.Focus();
                    }
                }
            }
        }
        private void tbConvMergeY_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrWhiteSpace(tbConvMergeY.Text))
                {
                    MessageBox.Show("Invalid value!");
                    tbConvMergeY.Clear();
                    tbConvMergeY.Focus();
                }
                else
                {
                    try
                    {
                        three.location.Y = Convert.ToInt32(tbConvMergeY.Text);
                        e.SuppressKeyPress = true;
                        UpdateTransportLength(three);
                        pictureBoxBHSBuild.Invalidate();
                    }
                    catch
                    {
                        MessageBox.Show("Invalid value!");
                        tbConvMergeY.Text = three.location.Y.ToString();
                        tbConvMergeY.Focus();
                    }
                }
            }
        }
        private void tbGateName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrWhiteSpace(tbGateName.Text))
                {
                    MessageBox.Show("Invalid name!");
                    tbGateName.Clear();
                    tbGateName.Focus();
                }
                else
                {
                    three.id = tbGateName.Text;
                    e.SuppressKeyPress = true;
                }
            }
        }
        private void tbGateX_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrWhiteSpace(tbGateX.Text))
                {
                    MessageBox.Show("Invalid value!");
                    tbGateX.Clear();
                    tbGateX.Focus();
                }
                else
                {
                    try
                    {
                        three.location.X = Convert.ToInt32(tbGateX.Text);
                        e.SuppressKeyPress = true;
                        UpdateTransportLength(three);
                        pictureBoxBHSBuild.Invalidate();
                    }
                    catch
                    {
                        MessageBox.Show("Invalid value!");
                        tbGateX.Text = three.location.X.ToString();
                        tbGateX.Focus();
                    }
                }
            }
        }
        private void tbGateY_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrWhiteSpace(tbGateY.Text))
                {
                    MessageBox.Show("Invalid value!");
                    tbGateY.Clear();
                    tbGateY.Focus();
                }
                else
                {
                    try
                    {
                        three.location.Y = Convert.ToInt32(tbGateY.Text);
                        e.SuppressKeyPress = true;
                        UpdateTransportLength(three);
                        pictureBoxBHSBuild.Invalidate();
                    }
                    catch
                    {
                        MessageBox.Show("Invalid value!");
                        tbGateY.Text = three.location.Y.ToString();
                        tbGateY.Focus();
                    }
                }
            }
        }
        private void cbFinalDestination_SelectedIndexChanged(object sender, EventArgs e)
        {
            Node n = null;
            if (cbFinalDestination.SelectedIndex == -1) { return; }
            foreach (Node node in builder.mymap.Nodes)
            {
                if (node.id == cbFinalDestination.SelectedItem.ToString())
                {
                    n = node;
                }
            }
            if (n != null && three is CheckIn)
            {
                CheckIn c = (CheckIn)three;
                c.FinalDestination = n;
            }
        }
        private void nudChangeSuspicious_ValueChanged(object sender, EventArgs e)
        {
            if (three != null)
            {
                Node n = null;
                foreach (Node node in builder.mymap.Nodes)
                {
                    if (node.id == three.id)
                    {
                        n = node;
                    }
                }
                if (n != null)
                {
                    CheckIn c = (CheckIn)three;
                    c.ChanceSuspicious = (int)nudChanceSuspicious.Value;
                }
            }
        }
        private void nudBagPerCheckIn_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (three != null && three is CheckIn)
                {
                    Node n = null;
                    foreach (Node node in builder.mymap.Nodes)
                    {
                        if (node.id == three.id)
                        {
                            n = node;
                        }
                    }
                    if (n != null)
                    {
                        CheckIn c = (CheckIn)three;
                        c.NumberOfBags = (int)nudBagPerCheckIn.Value;
                    }
                }
            }
        }
        private void nudChanceSuspicious_KeyUp(object sender, KeyEventArgs e)
        {

        }

        // Miscellaneous events
        private void btexport_Click(object sender, EventArgs e)
        {

            foreach (Bag b in bagList)
            {
                if (b.finished)
                {
                    report.AvgTime += b.traveltime;
                    if (b.Suspicious)
                    {
                        report.SuspiciosTime = b.traveltime;
                    }
                    else
                    {
                        report.NormalTime = b.traveltime;
                    }
                }

            }
            saveFileDialog2.ShowDialog();

            Document doc = new Document(iTextSharp.text.PageSize.LETTER, 10, 10, 42, 35);
            PdfWriter wri = PdfWriter.GetInstance(doc, new FileStream(this.saveFileDialog2.FileName, FileMode.Create, FileAccess.Write));
            doc.Open();
            //Open document to write  Write content
            Paragraph paragraph = new Paragraph(string.Format("\n Normal Bags: {0} \n Suspicious Bags: {1} \n Avg. Time: {2} \n Avg. Dis" +
                "tance: {3} \n Suspicious: {4} % \n Normal Bag Distance: {5} \n Suspicios Bag Distance: {6}\n Normal Bag time: {7} \n Suspicious Bag time: {8}", report.NormalBags,
                report.SuspiciousBags, report.AvgTime / bagList.Count, report.AvgDistance, nudChanceSuspicious.Value, report.NormalDistance, report.SuspiciosDistance, report.NormalTime, report.SuspiciosTime));
            //Add this to pdf
            doc.Add(paragraph);
            foreach (Node n in builder.mymap.Nodes)
            {
                if (n is Gate)
                {
                    Gate g = (Gate)n;
                    foreach (Bag b in bagList)
                    {
                        if (b.EndPoint.id == n.id)
                        {
                            g.CountBag++;

                        }
                    }
                    Paragraph paragraph2 = new Paragraph(string.Format("Gate {0} has {1} bags\n", n.id, g.CountBag));
                    doc.Add(paragraph2);

                }
            }


            doc.Close();
            MessageBox.Show(string.Format("\n Normal Bags: {0} \n Suspicious Bags: {1} \n Avg. Time: {2} \n Avg. Dis" +
                "tance: {3} \n Suspicious: {4} % \n Normal Bag Distance: {5} \n Suspicios Bag Distance: {6}\n Normal Bag time: {7} \n Suspicious Bag time: {8}", report.NormalBags,
                report.SuspiciousBags, report.AvgTime / bagList.Count, report.AvgDistance, nudChanceSuspicious.Value, report.NormalDistance, report.SuspiciosDistance, report.NormalTime, report.SuspiciosTime));
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!saved)
            {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    DialogResult result = MessageBox.Show("You have unsaved changes. do you want to save?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes) { SaveBHS(); }
                    else if (result == DialogResult.Cancel) { e.Cancel = true; }
                }
            }
        }
        
      
    }
}
