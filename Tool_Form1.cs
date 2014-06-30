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
namespace exportresstrings
{
    public partial class Form1 : Form
    {
        int column = 1, row = 1;
        Dictionary<string, string> resDictionary = new Dictionary<string, string>();
        public Form1()
        {
            InitializeComponent();
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
                        resDLLName = slnFile[0].Substring(slnFile[0].LastIndexOf(@"\")+1);
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
                readAllResourceFiles(txtBoxPath.Text.ToString());
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
}
