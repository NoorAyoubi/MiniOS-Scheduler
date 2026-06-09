using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Scheduling
{
    class Code
    {
        protected List<string> m_lLines;
        public int LineCount
        {
            get { return m_lLines.Count; }
        }

        protected Dictionary<string, int> m_dLables;

        public string this[int iLine]
        {
            get { return GetLine(iLine); }
        }

        public Code()
        {
            m_lLines = new List<string>();
            m_dLables = new Dictionary<string, int>();
        }

        public Code(string sCodeFile) : this()
        {
            List<string> rawLines = new List<string>();
            StreamReader sr = new StreamReader(sCodeFile);
            string sLine = "";

            while (!sr.EndOfStream)
            {
                sLine = sr.ReadLine().Trim();
                if (string.IsNullOrEmpty(sLine)) continue;

                // Check if line is a label (ends with ":")
                if (sLine.EndsWith(":"))
                {
                    string label = sLine.Substring(0, sLine.Length - 1).Trim();
                    if (string.IsNullOrEmpty(label)) continue;

                    if (!m_dLables.ContainsKey(label))
                    {
                        m_dLables.Add(label, rawLines.Count); // index after removing labels
                    }
                }
                else
                {
                    rawLines.Add(sLine);
                }
            }
            sr.Close();
            m_lLines = rawLines;
        }

        private string GetLine(int iLine)
        {
            // תיקון 1: הגנה מפני IndexOutOfRange
            if (iLine < 0 || iLine >= m_lLines.Count)
            {
                throw new IndexOutOfRangeException($"Invalid line index: {iLine}. Valid range: 0-{m_lLines.Count - 1}");
            }

            string sLine = m_lLines[iLine];

            // תיקון 2: Split robust יותר
            string[] asTokens = sLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Handle goto instruction
            if (asTokens.Length > 0 && asTokens[0] == "goto" && asTokens.Length > 1)
            {
                string target = asTokens[1];
                if (m_dLables.ContainsKey(target))
                {
                    return $"goto {m_dLables[target]}";
                }
                else
                {
                    // תיקון 3: טיפול ב-label לא קיים
                    throw new KeyNotFoundException($"Label '{target}' not found in code file");
                }
            }

            return sLine;
        }
    }
}