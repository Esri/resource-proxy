using FP.Cloud.OnlineRateTable.Common.Interfaces;
using FP.Cloud.OnlineRateTable.Common.ProductCalculation;
using FP.Cloud.OnlineRateTable.Common.RateTable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP.Cloud.OnlineRateTable.BusinessLogic
{
    public class RateCalculationFileHandler
    {
        #region members
        private IRateTableManager m_Manager;
        private string m_PawnFile = string.Empty;
        private string m_RateTableFile = string.Empty;
        private string[] m_AdditionalFiles = new string[0];

        public string PawnFile
        {
            get{ return m_PawnFile; }
        }

        public string RateTableFile
        {
            get{ return m_RateTableFile; }
        }

        public string[] AdditionalFiles
        {
            get{ return m_AdditionalFiles; }
        }

        public bool IsValid
        {
            get
            {
                return string.IsNullOrEmpty(m_PawnFile) == false &&
                string.IsNullOrEmpty(m_RateTableFile) == false;
            }
        }
        #endregion

        #region constructor
        public RateCalculationFileHandler(IRateTableManager manager)
        {
            m_Manager = manager;
        }
        #endregion

        #region public
        public async Task Initialize(EnvironmentInfo environment)
        {
            IEnumerable<RateTableInfo> rateTables = await m_Manager.GetFiltered(string.Empty, string.Empty, 
                environment.CarrierId, DateTime.MinValue, environment.UtcDate, environment.Culture, 0, 0, true);
            RateTableInfo currentTable = rateTables.OrderByDescending(t => t.ValidFrom).FirstOrDefault();
            if (null != currentTable)
            {
                List<string> otherRateCalculationFiles = new List<string>();
                string storagePath = Path.GetTempPath();
                foreach (var file in currentTable.PackageFiles)
                {
                    string filePath = Path.Combine(storagePath, file.FileName);
                    if (File.Exists(filePath) == false)
                    {
                        using (FileStream fs = File.Create(filePath))
                        {
                            await fs.WriteAsync(file.FileData.ToArray(), 0, file.FileData.Count());
                        }
                    }
                    if (file.FileType == EFileType.PawnCode)
                    {
                        m_PawnFile = filePath;
                    }
                    else if (file.FileType == EFileType.RateTable)
                    {
                        m_RateTableFile = filePath;
                    }
                    else
                    {
                        otherRateCalculationFiles.Add(filePath);
                    }
                }
                m_AdditionalFiles = otherRateCalculationFiles.ToArray();
            }
        }
        #endregion
    }
}
