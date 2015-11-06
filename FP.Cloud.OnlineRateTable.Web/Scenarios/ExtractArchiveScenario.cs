using FP.Cloud.OnlineRateTable.Common.RateTable;
using FP.Cloud.OnlineRateTable.Common.ScenarioRunner;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace FP.Cloud.OnlineRateTable.Web.Scenarios
{
    public class ExtractArchiveScenario
    {
        #region members
        private ScenarioRunner m_ScenarioRunner;
        #endregion

        #region constructor
        public ExtractArchiveScenario(ScenarioRunner scenarioRunner)
        {
            m_ScenarioRunner = scenarioRunner;
        }
        #endregion

        #region public
        public ScenarioResult<List<RateTableFileInfo>> Execute(Stream inputStream)
        {
            return m_ScenarioRunner.Run(() => ExtractFiles(inputStream));
        }
        #endregion

        #region private
        private ScenarioResult<List<RateTableFileInfo>> ExtractFiles(Stream inputStream)
        {
            List<RateTableFileInfo> rateTableFileList = new List<RateTableFileInfo>();
            using (ZipArchive archive = new ZipArchive(inputStream))
            {
                if (null != archive.Entries)
                {
                    foreach (var entry in archive.Entries)
                    {
                        //start with basic file info
                        RateTableFileInfo fileInfo = new RateTableFileInfo();
                        fileInfo.FileName = entry.Name;
                        switch (Path.GetExtension(entry.Name))
                        {
                            case ".meta":
                                fileInfo.FileType = EFileType.Meta;
                                break;
                            case ".bin":
                                fileInfo.FileType = EFileType.RateTable;
                                break;
                            case ".amx":
                                fileInfo.FileType = EFileType.PawnCode;
                                break;
                            case ".data":
                                fileInfo.FileType = EFileType.Zip2Zone;
                                break;
                            default:
                                fileInfo.FileType = EFileType.Undefined;
                                break;
                        }
                        using (MemoryStream reader = new MemoryStream())
                        {
                            int read;
                            byte[] buffer = new byte[entry.Length];
                            using (Stream input = entry.Open())
                            {
                                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    reader.Write(buffer, 0, read);
                                }
                                fileInfo.FileData = new List<byte>(reader.ToArray());

                                rateTableFileList.Add(fileInfo);
                            }
                        }
                    }
                }
            }
            return new ScenarioResult<List<RateTableFileInfo>>() { Success = true, Value = rateTableFileList };
        }
        #endregion
    }
}