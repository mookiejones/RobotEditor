using System.Collections.Generic;

namespace RobotEditor.Controls.TextEditor.Snippets.CompletionData
{
    public static class CodeCompletionDataUsageCache
    {
        private struct UsageStruct
        {
            public int Uses;
            public int ShowCount;
            public UsageStruct(int uses, int showCount)
            {
                Uses = uses;
                ShowCount = showCount;
            }
        }
        private class SaveItemsComparer : IComparer<KeyValuePair<string, UsageStruct>>
        {
            public int Compare(KeyValuePair<string, UsageStruct> x, KeyValuePair<string, UsageStruct> y) => -(x.Value.Uses / (double)x.Value.ShowCount).CompareTo(y.Value.Uses / (double)y.Value.ShowCount);
        }
        private const long magic = 7306916068411589443L;
        private const short version = 1;
        private const int MinUsesForSave = 2;
        private static Dictionary<string, UsageStruct> dict;
        private static bool dataUsageCacheEnabled = true;
        public static bool DataUsageCacheEnabled
        {
            get => dataUsageCacheEnabled;
            set => dataUsageCacheEnabled = value;
        }
        public static void ResetCache() => dict = new Dictionary<string, UsageStruct>();
        public static double GetPriority(string dotnetName, bool incrementShowCount)
        {
            if (!DataUsageCacheEnabled)
            {
                return 0.0;
            }
            if (dict == null)
            {
                dict = new Dictionary<string, UsageStruct>();
            }
            UsageStruct value;
            if (!dict.TryGetValue(dotnetName, out value))
            {
                return 0.0;
            }
            var num = value.Uses / (double)value.ShowCount;
            if (value.Uses < 2)
            {
                num *= 0.2;
            }
            if (incrementShowCount)
            {
                value.ShowCount++;
                dict[dotnetName] = value;
            }
            return num;
        }
        public static void IncrementUsage(string dotnetName)
        {
            if (!DataUsageCacheEnabled)
            {
                return;
            }
            if (dict == null)
            {
                dict = new Dictionary<string, UsageStruct>();
            }
            UsageStruct value;
            if (!dict.TryGetValue(dotnetName, out value))
            {
                value = new UsageStruct(0, 2);
            }
            value.Uses++;
            dict[dotnetName] = value;
        }
    }
}