using ClosedXML.Excel;
using KDPM_ReqnrollSeleniumAutomationTesting_Nhom5.Hooks;
using OpenQA.Selenium;
using Reqnroll;

namespace KDPM_ReqnrollSeleniumAutomationTesting_Nhom5.Hooks
{
    [Binding]
    public class ExcelReportHooks
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;

        private static readonly List<TestReportRow> ReportRows = new();

        private static readonly string ResultDir =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestResults");

        private static readonly string ScreenshotDir =
            Path.Combine(ResultDir, "Screenshots");

        public ExcelReportHooks(ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            _scenarioContext = scenarioContext;
            _featureContext = featureContext;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            Directory.CreateDirectory(ResultDir);
            Directory.CreateDirectory(ScreenshotDir);

            ReportRows.Clear();
        }

        [AfterScenario]
        public void AfterScenario()
        {
            string featureName = _featureContext.FeatureInfo.Title;
            string scenarioName = _scenarioContext.ScenarioInfo.Title;
            string[] tags = _scenarioContext.ScenarioInfo.Tags;

            string tcId = GetTagValue(tags, "TC_");
            string severity = GetTagValue(tags, "Severity_");
            string priority = GetTagValue(tags, "Priority_");

            string status = "PASS";
            string errorMessage = "";
            string screenshotPath = "";

            if (_scenarioContext.TestError != null)
            {
                status = "FAIL";
                errorMessage = _scenarioContext.TestError.Message;
                screenshotPath = CaptureScreenshot(featureName, tcId, scenarioName);
            }
            else if (tags.Any(t => t.Contains("Skip", StringComparison.OrdinalIgnoreCase)))
            {
                status = "SKIP";
            }

            ReportRows.Add(new TestReportRow
            {
                Feature = featureName,
                TestCaseId = tcId,
                Scenario = scenarioName,
                Status = status,
                Severity = severity,
                Priority = priority,
                ErrorMessage = errorMessage,
                ScreenshotPath = screenshotPath,
                ExecutedAt = DateTime.Now
            });
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            Directory.CreateDirectory(ResultDir);

            string reportPath = Path.Combine(ResultDir, "Report.xlsx");

            using var workbook = new XLWorkbook();

            CreateSummarySheet(workbook);
            CreateFeatureSheets(workbook);

            workbook.SaveAs(reportPath);
        }

        private static void CreateSummarySheet(XLWorkbook workbook)
        {
            var ws = workbook.Worksheets.Add("Summary");

            ws.Cell(1, 1).Value = "Tổng số";
            ws.Cell(1, 2).Value = ReportRows.Count;

            ws.Cell(2, 1).Value = "PASS";
            ws.Cell(2, 2).Value = ReportRows.Count(x => x.Status == "PASS");

            ws.Cell(3, 1).Value = "FAIL";
            ws.Cell(3, 2).Value = ReportRows.Count(x => x.Status == "FAIL");

            ws.Cell(4, 1).Value = "SKIP";
            ws.Cell(4, 2).Value = ReportRows.Count(x => x.Status == "SKIP");

            ws.Cell(6, 1).Value = "Feature";
            ws.Cell(6, 2).Value = "Total";
            ws.Cell(6, 3).Value = "PASS";
            ws.Cell(6, 4).Value = "FAIL";
            ws.Cell(6, 5).Value = "SKIP";

            int row = 7;

            foreach (var group in ReportRows.GroupBy(x => x.Feature))
            {
                ws.Cell(row, 1).Value = group.Key;
                ws.Cell(row, 2).Value = group.Count();
                ws.Cell(row, 3).Value = group.Count(x => x.Status == "PASS");
                ws.Cell(row, 4).Value = group.Count(x => x.Status == "FAIL");
                ws.Cell(row, 5).Value = group.Count(x => x.Status == "SKIP");
                row++;
            }

            FormatWorksheet(ws);
        }

        private static void CreateFeatureSheets(XLWorkbook workbook)
        {
            foreach (var group in ReportRows.GroupBy(x => x.Feature))
            {
                string sheetName = MakeSafeSheetName(group.Key);
                var ws = workbook.Worksheets.Add(sheetName);

                ws.Cell(1, 1).Value = "TC-ID";
                ws.Cell(1, 2).Value = "Scenario";
                ws.Cell(1, 3).Value = "Status";
                ws.Cell(1, 4).Value = "Severity";
                ws.Cell(1, 5).Value = "Priority";
                ws.Cell(1, 6).Value = "Error Message";
                ws.Cell(1, 7).Value = "Screenshot";
                ws.Cell(1, 8).Value = "Executed At";

                int row = 2;

                foreach (var item in group)
                {
                    ws.Cell(row, 1).Value = item.TestCaseId;
                    ws.Cell(row, 2).Value = item.Scenario;
                    ws.Cell(row, 3).Value = item.Status;
                    ws.Cell(row, 4).Value = item.Severity;
                    ws.Cell(row, 5).Value = item.Priority;
                    ws.Cell(row, 6).Value = item.ErrorMessage;
                    ws.Cell(row, 8).Value = item.ExecutedAt.ToString("yyyy-MM-dd HH:mm:ss");

                    if (!string.IsNullOrWhiteSpace(item.ScreenshotPath) && File.Exists(item.ScreenshotPath))
                    {
                        ws.Cell(row, 7).Value = "Open screenshot";
                        ws.Cell(row, 7).SetHyperlink(new XLHyperlink(item.ScreenshotPath));
                    }

                    if (item.Status == "PASS")
                    {
                        ws.Cell(row, 3).Style.Fill.BackgroundColor = XLColor.LightGreen;
                    }
                    else if (item.Status == "FAIL")
                    {
                        ws.Cell(row, 3).Style.Fill.BackgroundColor = XLColor.LightPink;
                    }
                    else if (item.Status == "SKIP")
                    {
                        ws.Cell(row, 3).Style.Fill.BackgroundColor = XLColor.LightYellow;
                    }

                    row++;
                }

                FormatWorksheet(ws);
            }
        }

        private static string CaptureScreenshot(string featureName, string tcId, string scenarioName)
        {
            try
            {
                var driver = TestHooks.Driver;

                if (driver == null)
                {
                    return "";
                }

                if (driver is not ITakesScreenshot screenshotDriver)
                {
                    return "";
                }

                string fileName =
                    $"{MakeSafeFileName(featureName)}_{MakeSafeFileName(tcId)}_{DateTime.Now:yyyyMMdd_HHmmss}.png";

                string fullPath = Path.Combine(ScreenshotDir, fileName);

                var screenshot = screenshotDriver.GetScreenshot();
                screenshot.SaveAsFile(fullPath);

                return fullPath;
            }
            catch
            {
                return "";
            }
        }

        private static string GetTagValue(string[] tags, string prefix)
        {
            var tag = tags.FirstOrDefault(t => t.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrWhiteSpace(tag))
            {
                return "";
            }

            return tag;
        }

        private static string MakeSafeSheetName(string value)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                value = value.Replace(c, '_');
            }

            value = value.Replace(":", "_")
                         .Replace("\\", "_")
                         .Replace("/", "_")
                         .Replace("?", "_")
                         .Replace("*", "_")
                         .Replace("[", "_")
                         .Replace("]", "_");

            if (value.Length > 31)
            {
                value = value.Substring(0, 31);
            }

            return value;
        }

        private static string MakeSafeFileName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "NoName";
            }

            foreach (char c in Path.GetInvalidFileNameChars())
            {
                value = value.Replace(c, '_');
            }

            return value.Replace(" ", "_");
        }

        private static void FormatWorksheet(IXLWorksheet ws)
        {
            ws.Row(1).Style.Font.Bold = true;
            ws.Row(1).Style.Fill.BackgroundColor = XLColor.LightGray;

            ws.Columns().AdjustToContents();

            foreach (var col in ws.Columns())
            {
                if (col.Width > 60)
                {
                    col.Width = 60;
                }
            }

            ws.SheetView.FreezeRows(1);
        }

        private class TestReportRow
        {
            public string Feature { get; set; } = "";
            public string TestCaseId { get; set; } = "";
            public string Scenario { get; set; } = "";
            public string Status { get; set; } = "";
            public string Severity { get; set; } = "";
            public string Priority { get; set; } = "";
            public string ErrorMessage { get; set; } = "";
            public string ScreenshotPath { get; set; } = "";
            public DateTime ExecutedAt { get; set; }
        }
    }
}

