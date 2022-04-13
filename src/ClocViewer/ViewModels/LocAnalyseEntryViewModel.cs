using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ClocAnalyzerLibrary;
using ClocViewer.Core;

namespace ClocViewer.ViewModels
{
    public class LocAnalyseEntryViewModel : ObservableObject
    {
        public ObservableCollection<LocAnalyseEntryViewModel> Entries { get; set; }

        public LocAnalyseEntryViewModel Parent { get; set; }

        public string Name
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public long FileCount
        {
            get => GetValue<long>();
            set => SetValue(value);
        }

        public string FileType
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public long BlankCount
        {
            get => GetValue<long>();
            set => SetValue(value);
        }

        public long CommentCount
        {
            get => GetValue<long>();
            set => SetValue(value);
        }

        public long CodeCount
        {
            get => GetValue<long>();
            set => SetValue(value);
        }

        public bool IsFolder
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        public bool IsIgnored
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        public string IgnoreReason
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string FullPath
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public LocFolder ModelFolder { get; }

        public LocAnalyseEntryViewModel(LocFolder folder, bool isRoot = false)
        {
            Entries = new ObservableCollection<LocAnalyseEntryViewModel>();
            IsFolder = true;
            ModelFolder = folder;

            if (!isRoot)
            {
                Entries.Add(new LocAnalyseEntryViewModel("..") { IsFolder = true, Parent = this });
            }

            foreach (var dir in folder.Folders)
            {
                Entries.Add(new LocAnalyseEntryViewModel(dir) { Parent = this });
            }

            foreach (var file in folder.Files)
            {
                Entries.Add(new LocAnalyseEntryViewModel(file));
            }

            Name = folder.Name;
            FullPath = folder.FullPath;
            FileCount = folder.TotalFilesCount;
            FillFromStats(folder.Stats);
        }

        public LocAnalyseEntryViewModel(string name)
        {
            Name = name;
        }

        public LocAnalyseEntryViewModel(LocFile file)
        {
            Name = file.Name;
            FullPath = file.FullPath;
            FillFromStats(file.Stats);
        }

        private void FillFromStats(LocStats stats)
        {
            FileType = stats.Type;
            BlankCount = stats.Blank;
            CommentCount = stats.Comment;
            CodeCount = stats.Code;
            IgnoreReason = stats.IgnoreReason;
            IsIgnored = stats.IsIgnored;
        }

        public IReadOnlyCollection<LocAnalyseEntryViewModel> DecendantsAndSelf()
        {
            return new ReadOnlyCollection<LocAnalyseEntryViewModel>(DecendantsRecursive(this));
        }

        private List<LocAnalyseEntryViewModel> DecendantsRecursive(LocAnalyseEntryViewModel entry)
        {
            List<LocAnalyseEntryViewModel> childrenEntries = new();
            if (entry == null || entry.Name == "..") return childrenEntries;
            if (!entry.IsFolder) // File
            {
                childrenEntries.Add(entry);
                return childrenEntries;
            }

            // Empty folder
            if (entry.FileCount == 0) return childrenEntries;

            var files = entry.Entries.Where(x => !x.IsFolder).OrderBy(x => x.Name);
            childrenEntries.AddRange(files);

            var folders = entry.Entries.Where(x => x.IsFolder).Where(x => x.Name != "..").OrderBy(x => x.Name);
            foreach (var e in folders)
            {
                childrenEntries.AddRange(DecendantsRecursive(e));
            }

            return childrenEntries;
        }
    }
}
