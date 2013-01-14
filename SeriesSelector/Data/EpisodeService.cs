using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using SeriesSelector.Frame;
using SeriesSelector.Properties;

namespace SeriesSelector.Data
{
    [Export(typeof(IEpisodeService))]
    public class EpisodeService : IEpisodeService
    {
        public IList<EpisodeType> GetSourceEpisode(string sourcePath)
        {
            var fileTypeValues = Settings.Default.FileTypes.Split('|');

            var fileList = new ArrayList();

            foreach (var fileTypeValue in fileTypeValues)
            {
                var di = Directory.GetFiles(sourcePath, string.Format("*.{0}", fileTypeValue),
                                            SearchOption.AllDirectories);

                fileList.AddRange(di);
            }

            IList<EpisodeType> episodeList = new List<EpisodeType>();

            foreach (string file in fileList)
            {
                var episodeType = new EpisodeType();
                var fileName = Path.GetFileName(file);

                if(string.IsNullOrEmpty(fileName))
                    continue;

                if(fileName.ToLower().Contains("sample"))
                    continue;

                var checker = BootStrapper.ResolveAll<IEpisodeChecker>();
                Tuple<string, string> result = null;

                foreach (var c in checker)
                {
                    result = c.CheckSeasonEpisode(CleanName(fileName));
                    if (result != null)
                        break;
                }

                if (result == null)
                    continue;
                var seasonString = result.Item1;
                var episodeString = result.Item2;

                var fName = fileName.Replace(seasonString, "");
                fName = fName.Replace(episodeString, "");

                episodeType.FileName = fName;
                episodeType.Season = seasonString;
                episodeType.Episode = episodeString;
                episodeType.FullPath = file;
                episodeType.FileSize = Math.Round((((double)new FileInfo(file).Length) / 1048576), 2).ToString();

                episodeList.Add(episodeType);
            }
            return episodeList;
        }

        public IList<EpisodeType> GetNewFileList(IList<EpisodeType> sourceFiles)
        {
            var newList = new List<EpisodeType>();
            var mappings = GetMappingValues();

            foreach (var episodeType in sourceFiles)
            {
                string newName = null;
                var oldName = episodeType.FileName;
                var matcher = BootStrapper.ResolveAll<ISeriesMatcher>();
                foreach (var seriesMatcher in matcher)
                {
                    newName = seriesMatcher.Match(mappings, oldName);
                    if (!string.IsNullOrEmpty(newName)) break;
                }
                
                if(newName != null)
                {
                    episodeType.SeriesName = newName;
                    episodeType.NewName = newName + " " + episodeType.Season.ToUpper() +
                                      episodeType.Episode.ToUpper();
                }
               
                episodeType.FileType = Path.GetExtension(episodeType.FileName);

                episodeType.IsSelected = true;

                newList.Add(episodeType);
            }

            return newList;
        }

        public Dictionary<string, string> GetMappingValues()
        {
            if (!File.Exists(Constants.MappingFilePath))
            {
                var d = new Dictionary<string, string>();
                WriteMappingValue(d);
            }

            var mappingTable = CreateMappingTable();
            mappingTable.ReadXml(Constants.MappingFilePath);
            var mappings = new Dictionary<string, string>();

            foreach (DataRow row in mappingTable.Rows)
            {
                mappings.Add(row.ItemArray[0].ToString(), row.ItemArray[1].ToString());
            }
            return mappings;
        }

        public void WriteMappingValue(Dictionary<string, string> currentMappings)
        {
            var mappingTable = CreateMappingTable();
            foreach (var currentMapping in currentMappings)
            {
                mappingTable.Rows.Add(currentMapping.Key, currentMapping.Value);
            }

            mappingTable.WriteXml(Constants.MappingFilePath);
        }

        private DataTable CreateMappingTable()
        {
            var mappingTable = new DataTable("Mappings");
            mappingTable.Columns.Add("OldName");
            mappingTable.Columns.Add("NewName");
            return mappingTable;
        }

        private string CleanName(string fileName)
        {
            var cleanedName = fileName;
            var cleanlist = Settings.Default.CleanList.Split('|');
            cleanedName = cleanlist.Aggregate(cleanedName.ToLower(), (current, word) => current.Replace(word.ToLower(), string.Empty));
            cleanedName = cleanedName.Replace('.', ' ');
            cleanedName = cleanedName.Insert(cleanedName.LastIndexOf(' '), ".").Remove(cleanedName.LastIndexOf(' '), 1);
            cleanedName = cleanedName.Replace('_', ' ');
            return cleanedName;
        }
    }
}