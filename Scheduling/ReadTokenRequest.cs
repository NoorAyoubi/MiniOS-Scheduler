using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduling
{
    public class ReadTokenRequest
    {
        public string FileName { get; set; }
        public int TokenNumber { get; set; }
        public string Token { get; set; }
        public int ProcessId { get; set; }
        public string TargetVariable { get; set; }
        public bool EndOfStreamReached { get; set; }

        public ReadTokenRequest(int processId, string fileName, int tokenNumber, string targetVariable)
        {
            ProcessId = processId;
            FileName = fileName;
            TokenNumber = tokenNumber;
            TargetVariable = targetVariable;
            Token = null;
            EndOfStreamReached = false;
        }

        public ReadTokenRequest()
        {
        }
    }
}