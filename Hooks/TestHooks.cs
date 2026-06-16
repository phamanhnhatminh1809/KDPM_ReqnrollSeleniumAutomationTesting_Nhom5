
using ClosedXML.Excel;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Reqnroll;

namespace KDPM_ReqnrollSeleniumAutomationTesting_Nhom5.Hooks
{
    [Binding]
    public class TestHooks
    {
        public static IWebDriver? Driver;

        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;

        private static readonly List<TestReportRow> ReportRows = new();
        private static readonly object LockObj = new();

        private static readonly string ReportFolder =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestReports");

        private static readonly string ScreenshotFolder =
            Path.Combine(ReportFolder, "Screenshots");

        public TestHooks(ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            _scenarioContext = scenarioContext;
            _featureContext = featureContext;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            Directory.CreateDirectory(ReportFolder);
            Directory.CreateDirectory(ScreenshotFolder);

            lock (LockObj)
            {
                ReportRows.Clear();
            }
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            Driver = new ChromeDriver();
            Driver.Manage().Window.Maximize();

            _scenarioContext["StartTime"] = DateTime.Now;
        }

        [AfterScenario]
        public void AfterScenario()
        {
            DateTime startTime = _scenarioContext.ContainsKey("StartTime")
                ? (DateTime)_scenarioContext["StartTime"]
                : DateTime.Now;

            DateTime endTime = DateTime.Now;
            TimeSpan duration = endTime - startTime;

            string[] tags = _scenarioContext.ScenarioInfo.Tags;

            string testCaseId = tags.FirstOrDefault(t => t.StartsWith("TC_")) ?? "";
            string severity = GetTagValue(tags, "Severity_");
            string priority = GetTagValue(tags, "Priority_");

            string scenarioTitle = _scenarioContext.ScenarioInfo.Title;
            string testSummary = RemoveTestCaseIdFromTitle(scenarioTitle);
            string featureName = _featureContext.FeatureInfo.Title;

            string status;
            string actualResult;
            string screenshotPath = "";

            bool isSkipByTag = tags.Any(t => t.StartsWith("Skip", StringComparison.OrdinalIgnoreCase));

            if (_scenarioContext.TestError == null)
            {
                if (isSkipByTag)
                {
                    status = "SKIPPED";
                    actualResult = "Test được đánh dấu bỏ qua để tránh thay đổi dữ liệu thật.";
                }
                else
                {
                    status = "PASSED";
                    actualResult = "Đúng như mong đợi.";
                }
            }
            else
            {
                string errorType = _scenarioContext.TestError.GetType().Name;
                string errorMessage = _scenarioContext.TestError.Message;

                bool isIgnoredException =
                    errorType.Contains("Ignore", StringComparison.OrdinalIgnoreCase) ||
                    errorType.Contains("Ignored", StringComparison.OrdinalIgnoreCase) ||
                    errorMessage.Contains("bỏ qua", StringComparison.OrdinalIgnoreCase) ||
                    errorMessage.Contains("Đang tắt", StringComparison.OrdinalIgnoreCase);

                if (isSkipByTag || isIgnoredException)
                {
                    status = "SKIPPED";
                    actualResult = errorMessage;
                }
                else
                {
                    status = "FAILED";
                    actualResult = errorMessage;
                    screenshotPath = CaptureScreenshot(featureName, testCaseId);
                }
            }

            lock (LockObj)
            {
                ReportRows.Add(new TestReportRow
                {
                    TestCaseId = testCaseId,
                    Feature = featureName,
                    TestSummary = testSummary,
                    ExpectedResult = "Thực hiện đúng theo các bước Given/When/Then trong scenario.",
                    ActualResult = actualResult,
                    Status = status,
                    Severity = severity,
                    Priority = priority,
                    Duration = duration.TotalSeconds.ToString("0.00") + "s",
                    ExecutedAt = endTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    ScreenshotPath = screenshotPath
                });
            }

            if (Driver != null)
            {
                Driver.Quit();
                Driver.Dispose();
                Driver = null;
            }
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            ExportExcelReport();
        }

        private static string CaptureScreenshot(string featureName, string testCaseId)
        {
            try
            {
                if (Driver == null)
                    return "";

                if (Driver is not ITakesScreenshot screenshotDriver)
                    return "";

                Directory.CreateDirectory(ScreenshotFolder);

                string fileName =
                    $"{MakeSafeFileName(featureName)}_{MakeSafeFileName(testCaseId)}_{DateTime.Now:yyyyMMdd_HHmmss}.png";

                string fullPath = Path.Combine(ScreenshotFolder, fileName);

                Screenshot screenshot = screenshotDriver.GetScreenshot();
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
            string tag = tags.FirstOrDefault(t => t.StartsWith(prefix)) ?? "";
            return tag.Replace(prefix, "");
        }

        private static string RemoveTestCaseIdFromTitle(string title)
        {
            int index = title.IndexOf(" - ");

            if (index >= 0 && index + 3 < title.Length)
            {
                return title.Substring(index + 3).Trim();
            }

            return title.Trim();
        }

        private static void ExportExcelReport()
        {
            if (ReportRows.Count == 0)
                return;

            Directory.CreateDirectory(ReportFolder);

            string fileName = $"Reqnroll_Test_Report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            string filePath = Path.Combine(ReportFolder, fileName);

            using var workbook = new XLWorkbook();

            CreateSummarySheet(workbook);
            CreateResultSheet(workbook, "All Test Results", ReportRows);

            var failedRows = ReportRows
                .Where(r => r.Status == "FAILED")
                .ToList();

            CreateResultSheet(workbook, "Failed Cases", failedRows);

            foreach (var group in ReportRows.GroupBy(r => r.Feature))
            {
                string sheetName = MakeSafeSheetName(group.Key);
                CreateResultSheet(workbook, sheetName, group.ToList());
            }

            workbook.SaveAs(filePath);

            Console.WriteLine("Đã xuất report Excel:");
            Console.WriteLine(filePath);
        }

        private static void CreateSummarySheet(XLWorkbook workbook)
        {
            var ws = workbook.Worksheets.Add("Summary");

            ws.Cell(1, 1).Value = "Tổng số test";
            ws.Cell(1, 2).Value = ReportRows.Count;

            ws.Cell(2, 1).Value = "PASSED";
            ws.Cell(2, 2).Value = ReportRows.Count(r => r.Status == "PASSED");

            ws.Cell(3, 1).Value = "FAILED";
            ws.Cell(3, 2).Value = ReportRows.Count(r => r.Status == "FAILED");

            ws.Cell(4, 1).Value = "SKIPPED";
            ws.Cell(4, 2).Value = ReportRows.Count(r => r.Status == "SKIPPED");

            ws.Cell(6, 1).Value = "Feature";
            ws.Cell(6, 2).Value = "Total";
            ws.Cell(6, 3).Value = "PASSED";
            ws.Cell(6, 4).Value = "FAILED";
            ws.Cell(6, 5).Value = "SKIPPED";

            int row = 7;

            foreach (var group in ReportRows.GroupBy(r => r.Feature))
            {
                ws.Cell(row, 1).Value = group.Key;
                ws.Cell(row, 2).Value = group.Count();
                ws.Cell(row, 3).Value = group.Count(r => r.Status == "PASSED");
                ws.Cell(row, 4).Value = group.Count(r => r.Status == "FAILED");
                ws.Cell(row, 5).Value = group.Count(r => r.Status == "SKIPPED");
                row++;
            }

            FormatSheet(ws);
        }

        private static void CreateResultSheet(XLWorkbook workbook, string sheetName, List<TestReportRow> rows)
        {
            var ws = workbook.Worksheets.Add(MakeSafeSheetName(sheetName));

            string[] headers =
            {
                "Test case Id",
                "Feature",
                "Test summary",
                "Expected Result",
                "Actual Result",
                "Status",
                "Severity",
                "Priority",
                "Duration",
                "Executed At",
                "Screenshot"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(1, i + 1).Value = headers[i];
            }

            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                int row = i + 2;

                ws.Cell(row, 1).Value = r.TestCaseId;
                ws.Cell(row, 2).Value = r.Feature;
                ws.Cell(row, 3).Value = r.TestSummary;
                ws.Cell(row, 4).Value = r.ExpectedResult;
                ws.Cell(row, 5).Value = r.ActualResult;
                ws.Cell(row, 6).Value = r.Status;
                ws.Cell(row, 7).Value = r.Severity;
                ws.Cell(row, 8).Value = r.Priority;
                ws.Cell(row, 9).Value = r.Duration;
                ws.Cell(row, 10).Value = r.ExecutedAt;

                if (!string.IsNullOrWhiteSpace(r.ScreenshotPath) && File.Exists(r.ScreenshotPath))
                {
                    ws.Cell(row, 11).Value = "Open screenshot";
                    ws.Cell(row, 11).SetHyperlink(new XLHyperlink(r.ScreenshotPath));
                    ws.Cell(row, 11).Style.Font.FontColor = XLColor.Blue;
                    ws.Cell(row, 11).Style.Font.Underline = XLFontUnderlineValues.Single;
                }

                if (r.Status == "PASSED")
                {
                    ws.Cell(row, 6).Style.Fill.BackgroundColor = XLColor.LightGreen;
                }
                else if (r.Status == "FAILED")
                {
                    ws.Cell(row, 6).Style.Fill.BackgroundColor = XLColor.LightPink;
                }
                else if (r.Status == "SKIPPED")
                {
                    ws.Cell(row, 6).Style.Fill.BackgroundColor = XLColor.LightYellow;
                }
            }

            FormatSheet(ws);
        }

        private static void FormatSheet(IXLWorksheet ws)
        {
            var usedRange = ws.RangeUsed();

            if (usedRange != null)
            {
                usedRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                usedRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                usedRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                usedRange.Style.Alignment.WrapText = true;
            }

            var headerRange = ws.Range(1, 1, 1, ws.LastColumnUsed()?.ColumnNumber() ?? 1);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            for (int row = 2; row <= ws.LastRowUsed()?.RowNumber(); row++)
            {
                string status = ws.Cell(row, 6).GetString();

                if (status == "PASSED")
                {
                    ws.Cell(row, 6).Style.Fill.BackgroundColor = XLColor.LightGreen;
                }
                else if (status == "FAILED")
                {
                    ws.Cell(row, 6).Style.Fill.BackgroundColor = XLColor.LightPink;
                }
                else if (status == "SKIPPED")
                {
                    ws.Cell(row, 6).Style.Fill.BackgroundColor = XLColor.LightYellow;
                }
            }

            ws.Columns().AdjustToContents();

            ws.Column(3).Width = 35;
            ws.Column(4).Width = 45;
            ws.Column(5).Width = 60;
            ws.Column(11).Width = 25;

            ws.SheetView.FreezeRows(1);
        }

        private static string MakeSafeSheetName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "Sheet";

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
                return "NoName";

            foreach (char c in Path.GetInvalidFileNameChars())
            {
                value = value.Replace(c, '_');
            }

            value = value.Replace(" ", "_");

            return value;
        }
    }

    public class TestReportRow
    {
        public string TestCaseId { get; set; } = "";
        public string Feature { get; set; } = "";
        public string TestSummary { get; set; } = "";
        public string ExpectedResult { get; set; } = "";
        public string ActualResult { get; set; } = "";
        public string Status { get; set; } = "";
        public string Severity { get; set; } = "";
        public string Priority { get; set; } = "";
        public string Duration { get; set; } = "";
        public string ExecutedAt { get; set; } = "";
        public string ScreenshotPath { get; set; } = "";
    }
}

