using System;
using System.Collections.Generic;
using System.IO;

namespace Scheduling
{
    class Disk
    {
        public ReadTokenRequest ActiveRequest { get; set; }
        public OperatingSystem OperatingSystem { get; set; }

        private Queue<ReadTokenRequest> m_lReadRequests;
        private StreamReader m_srActiveFile;
        private int m_iCurrentLinePointer;
        private int m_cOverallTokenPointer;
        private bool m_bEndOfStream;
        private string[] m_asTokens;

        public Disk()
        {
            m_lReadRequests = new Queue<ReadTokenRequest>();
        }

        // Add a new request to the system
        public void AddRequest(ReadTokenRequest request)
        {
            if (ActiveRequest == null)
            {
                StartRequest(request);
            }
            else
            {
                m_lReadRequests.Enqueue(request);
            }
        }

        private void StartRequest(ReadTokenRequest request)
        {
            ActiveRequest = request;
            m_srActiveFile = new StreamReader(request.FileName);
            m_cOverallTokenPointer = 0;
            m_bEndOfStream = false;
            ReadNextLine();
        }

        public void ProcessRequest()
        {
            if (ActiveRequest == null) return;

            if (m_bEndOfStream)
            {
                EndRequest();
            }
            else if (m_cOverallTokenPointer == ActiveRequest.TokenNumber)
            {
                if (m_asTokens != null && m_iCurrentLinePointer < m_asTokens.Length)
                {
                    ActiveRequest.Token = m_asTokens[m_iCurrentLinePointer];
                }
                EndRequest();
            }
            else
            {
                m_iCurrentLinePointer++;
                m_cOverallTokenPointer++;

                if (m_asTokens != null && m_iCurrentLinePointer >= m_asTokens.Length)
                {
                    ReadNextLine();
                }
            }
        }

        private void ReadNextLine()
        {
            if (m_srActiveFile.EndOfStream)
            {
                m_asTokens = null;
                m_bEndOfStream = true;
            }
            else
            {
                string sLine = m_srActiveFile.ReadLine();
                if (!string.IsNullOrEmpty(sLine))
                {
                    m_asTokens = sLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    m_iCurrentLinePointer = 0;
                }
                else
                {
                    // Empty line - continue reading
                    ReadNextLine();
                }
            }
        }

        private void EndRequest()
        {
            m_srActiveFile?.Close();
            m_srActiveFile = null;

            ReadTokenRequest completedRequest = ActiveRequest;
            ActiveRequest = null;

            // Send interrupt to the OS
            OperatingSystem?.Interrupt(completedRequest);

            // Start the next request in the queue if any
            if (m_lReadRequests.Count > 0)
            {
                StartRequest(m_lReadRequests.Dequeue());
            }
        }
    }
}