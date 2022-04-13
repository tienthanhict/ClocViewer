using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ClocAnalyzerLibrary;

namespace ClocViewer.ViewModels
{
    public partial class LocAnalyseViewModel
    {
        private const char _csvDelimiter = ',';
        private readonly FileStreamOptions _saveFileOptions = new FileStreamOptions()
        {
            Mode = FileMode.Create,
            Access = FileAccess.Write,
            Share = FileShare.Read
        };

        private void SaveSummaryReport(IEnumerable<LocAnalyseEntryViewModel> fileEntries)
        {
            string prefixSourceFolder = new DirectoryInfo(CurrentPath).Parent.FullName + "\\";
            string reportFile = GenerateReportFilePath("SUMMARY");
            
            long fileCount = 0;
            long codeCount = 0;
            long commentCount = 0;
            long blankCount = 0;
            var languageDictCount = new Dictionary<string, (long, long, long, long)>();

            foreach (var item in fileEntries.Where(x => !x.IsIgnored))
            {
                fileCount += 1;
                codeCount += item.CodeCount;
                commentCount += item.CommentCount;
                blankCount += item.BlankCount;

                if (item.IsIgnored) continue;
                languageDictCount.IncrementBy(item.FileType, 1, item.CodeCount, item.CommentCount, item.BlankCount);
            }

            var sb = new StringBuilder();
            sb.Append("Language").Append(_csvDelimiter)
                .Append("File count").Append(_csvDelimiter)
                .Append("Code").Append(_csvDelimiter)
                .Append("Comment").Append(_csvDelimiter)
                .Append("Blank").Append(_csvDelimiter)
                .Append("Total lines")
                .AppendLine();

            foreach (var lang in languageDictCount)
            {
                sb.Append(lang.Key).Append(_csvDelimiter)
                .Append(lang.Value.Item1).Append(_csvDelimiter)
                .Append(lang.Value.Item2).Append(_csvDelimiter)
                .Append(lang.Value.Item3).Append(_csvDelimiter)
                .Append(lang.Value.Item4).Append(_csvDelimiter)
                .Append(lang.Value.Item2 + lang.Value.Item3 + lang.Value.Item4)
                .AppendLine();
            }
            sb.AppendLine();
            sb.Append("Grand total").Append(_csvDelimiter)
                .Append(fileCount).Append(_csvDelimiter)
                .Append(codeCount).Append(_csvDelimiter)
                .Append(commentCount).Append(_csvDelimiter)
                .Append(blankCount).Append(_csvDelimiter)
                .Append(codeCount + commentCount + blankCount)
                .AppendLine();


            using (TextWriter writer = new StreamWriter(reportFile, Encoding.UTF8, _saveFileOptions))
            {
                writer.Write(sb.ToString());
            }
        }

        private void SaveDetailReport(IEnumerable<LocAnalyseEntryViewModel> fileEntries)
        {
            string prefixSourceFolder = new DirectoryInfo(CurrentPath).Parent.FullName + "\\";            
            string reportFile = GenerateReportFilePath("DETAIL");

            var sb = new StringBuilder();
            sb.Append("Language").Append(_csvDelimiter)
                .Append("Filename").Append(_csvDelimiter)
                .Append("Code").Append(_csvDelimiter)
                .Append("Comment").Append(_csvDelimiter)
                .Append("Blank").Append(_csvDelimiter)
                .Append("Grand total")
                .AppendLine();

            foreach (var item in fileEntries.Where(x => !x.IsIgnored))
            {
                sb.Append(item.FileType).Append(_csvDelimiter)
                    .Append(NormalizeCsvValue(item.FullPath.Replace(prefixSourceFolder, ""))).Append(_csvDelimiter)
                    .Append(item.CodeCount).Append(_csvDelimiter)
                    .Append(item.CommentCount).Append(_csvDelimiter)
                    .Append(item.BlankCount).Append(_csvDelimiter)
                    .Append(item.CodeCount + item.CommentCount + item.BlankCount)
                    .AppendLine();
            }

            sb.AppendLine();
            sb.Append("Grand total").Append(_csvDelimiter)
                .Append(fileEntries.Count()).Append(_csvDelimiter)
                .Append(fileEntries.Sum(x => x.CodeCount)).Append(_csvDelimiter)
                .Append(fileEntries.Sum(x => x.CommentCount)).Append(_csvDelimiter)
                .Append(fileEntries.Sum(x => x.BlankCount)).Append(_csvDelimiter)
                .Append(fileEntries.Sum(x => x.CodeCount + x.CommentCount + x.BlankCount))
                .AppendLine();

            using (TextWriter writer = new StreamWriter(reportFile, Encoding.UTF8, _saveFileOptions))
            {
                writer.Write(sb.ToString());
            }
        }

        private string GenerateReportFilePath(string reportName)
        {
            string reportFolder = ReportPath;
            string sourceFolderName = new DirectoryInfo(Root.FullPath).Name;

            if (string.IsNullOrEmpty(ReportPath))
            {
                reportFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            }
            if (!Directory.Exists(reportFolder))
            {
                Directory.CreateDirectory(reportFolder);
            }
            return Path.Combine(reportFolder, $"{sourceFolderName}_{reportName}_{DateTime.Now.ToString("yyyyMMdd_hhmm")}.csv");
        }

        private string NormalizeCsvValue(string str)
        {
            return str.Contains(_csvDelimiter) ? $"\"{str}\"" : str;
        }
    }
}
