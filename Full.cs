using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop;
using System.Reflection;
using Microsoft.Office.Interop.Excel;
using System.Threading;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
namespace exportresstrings
{
    public partial class Form1 : Form
    {
        int column = 1, row = 1;
        Dictionary<string, string> resDictionary = new Dictionary<string, string>();
        public Form1()
        {
            InitializeComponent();
            //readRCfiles();
        }

        public void readAllResourceFiles(string pathfromclient)
        {
            List<Details> resList = new List<Details>();
            List<LogInfo> logList = new List<LogInfo>();
            Details details = new Details();
            LogInfo logInfo = new LogInfo();
            int logCount = 0;
            string requiredPath = "";
            string path = "";
            string resDLLName = "Resource DLL Name";
            string logPath = @"\resourceToExcelLog.txt";

            string[] subFolderArray = Directory.GetDirectories(pathfromclient);

            if (subFolderArray.Length == 0)
            {
                subFolderArray = new string[1];
                subFolderArray[0] = pathfromclient;
            }

            for (int folderCount = 0; folderCount < subFolderArray.Length; folderCount++)
            {
                requiredPath = null;
                string folderName = subFolderArray[folderCount];//.Substring(0, subFolderArray[folderCount].IndexOf("\\"));
                try
                {
                    string[] files = Directory.GetFiles(subFolderArray[folderCount], "Resources.resx", SearchOption.AllDirectories);
                    string[] slnFile = Directory.GetFiles(subFolderArray[folderCount], "*.sln", SearchOption.AllDirectories);
                    if (slnFile.Length > 0)
                    {
                        resDLLName = slnFile[0].Substring(slnFile[0].LastIndexOf(@"\") + 1);
                    }
                    if (files.Length > 0)
                    {
                        for (int count = 0; count < files.Length; count++)
                        {
                            if (files[count].EndsWith(@"Localization\Resources.resx", StringComparison.InvariantCultureIgnoreCase))
                            {
                                requiredPath = files[count];
                            }
                        }

                        path = requiredPath;

                        if (path != null)
                        {
                            //resDLLName = folderName;
                            if (File.Exists(path))
                            {
                                ResXResourceReader rsxr = new ResXResourceReader(path);

                                IDictionaryEnumerator id = rsxr.GetEnumerator();

                                foreach (DictionaryEntry d in rsxr)
                                {
                                    details.ResourceID = d.Key.ToString();
                                    details.ResourceDLL = resDLLName;
                                    details.EngText = d.Value.ToString();
                                    resList.Add(details);
                                    details = new Details();
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    logCount++;
                    logInfo.sNo = logCount;
                    logInfo.exceptionInfo = e.Message;
                    logList.Add(logInfo);
                    DateTime currentDateTime = DateTime.Now;
                    using (StreamWriter sw = File.AppendText(pathfromclient + logPath))
                    {
                        sw.WriteLine(logInfo.sNo + " - " + currentDateTime + " - " + logInfo.exceptionInfo);
                    }
                    logInfo = new LogInfo();
                }
            }
            if (resList.Count > 0)
                exportToExcel(resList);
            else
                MessageBox.Show("There are no files by the name Resources.resx in Localization folder in " + pathfromclient);
        }

        public void readRCfiles(string pathforrcfiles)
        {

            List<UnManagedDetails> unmanList = new List<UnManagedDetails>();
            List<Details> resList = new List<Details>();
            Details d = new Details();
            UnManagedDetails unmanDetails = new UnManagedDetails();
            string rcPath = "";
            //string rcPath = @"\\hydzbhifs\BHRDSSEZ\Daily\WorkArea\Sriram\joaJewelSuiteResources_en_us\joaJewelSuiteResources_en_us.rc";

            string[] subFolderArray = Directory.GetDirectories(pathforrcfiles);

            if (subFolderArray.Length == 0)
            {
                subFolderArray = new string[1];
                subFolderArray[0] = pathforrcfiles;
            }

            for (int folderCount = 0; folderCount < subFolderArray.Length; folderCount++)
            {
                try
                {

                    string folderName = subFolderArray[folderCount];

                    string[] files = Directory.GetFiles(subFolderArray[folderCount], "*_en_us.rc", SearchOption.AllDirectories);
                    rcPath = files[0].ToString();
                    if (rcPath != null)
                    {
                        TextReader reader = File.OpenText(rcPath);
                        string line;

                        //string[] files = Directory.GetFiles(@"D:\", "*_en_us.resx", SearchOption.AllDirectories);

                        while ((line = reader.ReadLine()) != null)
                        {
                            if ((line = reader.ReadLine()) == "STRINGTABLE")
                            {
                                line = reader.ReadLine();
                                while (!((line = reader.ReadLine()) == "END"))
                                {
                                    string[] s1 = line.Split('"');
                                    d.ResourceDLL = "UNMANAGED";
                                    d.ResourceID = s1[0].Trim();
                                    d.EngText = s1[1].Trim();
                                    resList.Add(d);
                                    d = new Details();
                                }
                            }
                        }
                    }
                }
                catch (Exception exe)
                {

                }
            }





            //string rcPath = @"D:\joaBasicsResources_en_us.rc";
            //string[] subFolderArray = Directory.GetDirectories(rcPath);

            exportToExcel(resList);

        }

        public void exportToExcel(IList<Details> dict)
        {
            Microsoft.Office.Interop.Excel.Application oXL = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook oWB = (Microsoft.Office.Interop.Excel._Workbook)(oXL.Workbooks.Add(Missing.Value));
            Microsoft.Office.Interop.Excel._Worksheet oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;
            Range cellRange = (Range)oSheet.Cells[row, column];
            cellRange.Font.Bold = true;
            oSheet.Cells[row, column] = "ResourceID";
            column++;
            cellRange = (Range)oSheet.Cells[row, column];
            cellRange.Font.Bold = true;
            oSheet.Cells[row, column] = "ResourceDLL";
            column++;
            cellRange = (Range)oSheet.Cells[row, column];
            cellRange.Font.Bold = true;
            oSheet.Cells[row, column] = "EnglishText";
            row++;

            foreach (var p in dict)
            {
                column = 1;
                oSheet.Cells[row, column] = p.ResourceID.ToString();
                column++;
                oSheet.Cells[row, column] = p.ResourceDLL.ToString();
                column++;
                oSheet.Cells[row, column] = p.EngText.ToString();
                row++;
            }

            oXL.Visible = true;
            oXL.UserControl = true;
        }

        public void button1_Click(object sender, EventArgs e)
        {
            Loading l = new Loading();
            try
            {
                button1.Enabled = false;
                button2.Enabled = false;
                this.Hide();
                l.Show();
                l.Refresh();
                //readAllResourceFiles(txtBoxPath.Text.ToString());
                readRCfiles(txtBoxPath.Text);
                l.Close();
                this.Close();
            }
            catch (Exception exce)
            {
                MessageBox.Show(exce.Message);
                this.Show();
                l.Close();
                button1.Enabled = true;
                button2.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();
            txtBoxPath.Text = fbd.SelectedPath;
        }


    }
    public class Details
    {
        public string ResourceID { get; set; }
        public string ResourceDLL { get; set; }
        public string EngText { get; set; }
    }
    public class LogInfo
    {
        public int sNo { get; set; }
        public string exceptionInfo { get; set; }
        //public int sNo { get; set; }
    }

    public class UnManagedDetails
    {
        public string ResourceID { get; set; }
        public string ResourceDLL { get; set; }
        public string EngText { get; set; }
    }
}
