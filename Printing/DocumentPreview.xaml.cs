/************************************************************************
 * Copyright: Hans Wolff
 *
 * License:  This software abides by the LGPL license terms. For further
 *           licensing information please see the top level LICENSE.txt 
 *           file found in the root directory of CodeReason Reports.
 *
 * Author:   Hans Wolff
 *
 ************************************************************************/

using CodeReason.Reports;
using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Xps.Packaging;

namespace Lieferliste_WPF.Printing
{
    /// <summary>
    /// Application's main form
    /// </summary>
    public partial class DocumentPreview : Window
    {
        private bool _firstActivated = true;
        public String ReportTitle { get; set; }
        ReportData data = new ReportData();
        public void addTable(DataTable table)
        {
            data.DataTables.Add(table);
            //_reportTables.Add(table);
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public DocumentPreview()
        {
           // InitializeComponent();
        }

        /// <summary>
        /// Window has been activated
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event details</param>
        private void Window_Activated(object sender, EventArgs e)
        {
            if (!_firstActivated) return;
            _firstActivated = false;

            try
            {
                ReportDocument reportDocument = new ReportDocument();

                StreamReader reader = new StreamReader(new FileStream(@"Templates\AllocationReport.xaml", FileMode.Open, FileAccess.Read));
                reportDocument.XamlData = reader.ReadToEnd();
                reportDocument.XamlImagePath = Path.Combine(Environment.CurrentDirectory, @"Templates\");
                reader.Close();



                // set constant document values
                data.ReportDocumentValues.Add("PrintDate", DateTime.Now); // print date is now
                data.ReportDocumentValues.Add("ReportTitle", ReportTitle);
                // sample table "Ean"
                //DataTable table = new DataTable("Ean");
                //table.Columns.Add("Position", typeof(string));
                //table.Columns.Add("Item", typeof(string));
                //table.Columns.Add("EAN", typeof(string));
                //table.Columns.Add("Count", typeof(int));
                //Random rnd = new Random(1234);
                //for (int i = 1; i <= 100; i++)
                //{
                //    // randomly create some articles
                //    table.Rows.Add(new object[] { i, "Item " + i.ToString("0000"), "123456790123", rnd.Next(9) + 1 });
                //}
                //data.DataTables.Add(table);


                DateTime dateTimeStart = DateTime.Now; // start time measure here

                XpsDocument xps = reportDocument.CreateXpsDocument(data);
               // documentViewer.Document = xps.GetFixedDocumentSequence();

                // show the elapsed time in window title
                Title += " - generated in " + (DateTime.Now - dateTimeStart).TotalMilliseconds + "ms";
            }
            catch (Exception ex)
            {
                // show exception
                MessageBox.Show(ex.Message + "\r\n\r\n" + ex.GetType() + "\r\n" + ex.StackTrace, ex.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
    }
}
