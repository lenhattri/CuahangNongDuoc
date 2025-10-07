using Microsoft.Reporting.WinForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CuahangNongduoc.Utils.Functions
{
    public class ReportHanler
    {
        public static string reportsFolder = Application.StartupPath.Replace("bin\\Debug", "Report");

        public static void LoadReport(
            ReportViewer reportViewer,
            IEnumerable dataSource,
            string reportName,
            string datasetName,
            IList<ReportParameter> parameters = null
        )
        {
            var reportDataSource = new ReportDataSource
            {
                Name = datasetName,
                Value = dataSource
            };

            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.DataSources.Add(reportDataSource);
            reportViewer.LocalReport.ReportPath = Path.Combine(reportsFolder, reportName);
            if (parameters != null && parameters.Count > 0)
            {
                reportViewer.LocalReport.SetParameters(parameters);
            }
            reportViewer.SetDisplayMode(DisplayMode.PrintLayout);
            reportViewer.ZoomMode = ZoomMode.Percent;
            reportViewer.ZoomPercent = 100;
            reportViewer.RefreshReport();
        }

    }
}
