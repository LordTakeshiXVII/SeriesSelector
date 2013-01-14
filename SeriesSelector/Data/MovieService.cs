using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using SeriesSelector.Frame;
using SeriesSelector.Properties;

namespace SeriesSelector.Data
{
    [Export(typeof(IMovieService))]
    public class MovieService : IMovieService
    {
        public IList<EpisodeType> GetMovies(string sourcePath)
        {
            var fileTypeValues = Settings.Default.FileTypes.Split('|');

            var fileList = new ArrayList();

            foreach (var fileTypeValue in fileTypeValues)
            {
                var di = Directory.GetFiles(sourcePath, string.Format("*.{0}", fileTypeValue),
                                            SearchOption.AllDirectories);

                fileList.AddRange(di);
            }

            IList<EpisodeType> movieList = new List<EpisodeType>();

            foreach (string file in fileList)
            {
                var episodeType = new EpisodeType();
                var fileName = Path.GetFileName(file);

                if (string.IsNullOrEmpty(fileName))
                    continue;

                if (fileName.ToLower().Contains("sample"))
                    continue;

                var checker = BootStrapper.ResolveAll<IEpisodeChecker>();
                Tuple<string, string> result = null;

                foreach (var c in checker)
                {
                    result = c.CheckSeasonEpisode(CleanName(fileName));
                    if (result != null)
                        break;
                }

                if (result != null)
                    continue;

                episodeType.FileName = fileName;
                episodeType.FullPath = file;
                episodeType.FileSize = Math.Round((((double)new FileInfo(file).Length) / 1048576), 2).ToString();

                movieList.Add(episodeType);
            }
            return movieList;
        }

        public IList<EpisodeType> TryMapMovies(IList<EpisodeType> list)
        {
            foreach (var movie in list)
            {
                var mappings = SearchFor(movie.FileName);
                if (mappings.Count == 1)
                {
                    movie.NewName = mappings[0];
                    movie.IsSelected = true;
                }
                movie.FileType = Path.GetExtension(movie.FileName);
            }

            return list;
        }

        public List<string> SearchFor(string oldName)
        {
            var requestName = CleanName(oldName).Replace(' ', '+');
            requestName = Settings.Default.FileTypes.Split('|').Aggregate(requestName, (current, ending) => current.Replace(ending, string.Empty));

            var resultList = new List<string>();

            var doc = new XmlDocument();
            doc.Load(string.Format("http://www.imdb.com/xml/find?xml=1&nr=1&tt=on&q={0}", requestName));
            var itemNodes = doc.SelectNodes("//IMDbResults/ResultSet/ImdbEntity");
            foreach (XmlNode itemNode in itemNodes)
            {
                var title = itemNode.InnerXml.Substring(0, itemNode.InnerXml.IndexOf('<'));
                var year = Regex.Match(itemNode.InnerXml, @"\d\d\d\d");
                resultList.Add(string.Format("{0} ({1})", title, year));
            }

            return resultList;
        }

        private string CleanName(string fileName)
        {
            var cleanedName = fileName;
            var cleanlist = Settings.Default.CleanList.Split('|');
            cleanedName = cleanlist.Aggregate(cleanedName.ToLower(), (current, word) => current.Replace(word.ToLower(), string.Empty));
            cleanedName = cleanedName.Replace('.', ' ');
            if(cleanedName.LastIndexOf(' ') >= 0)
                cleanedName = cleanedName.Insert(cleanedName.LastIndexOf(' '), ".").Remove(cleanedName.LastIndexOf(' '), 1);
            cleanedName = cleanedName.Replace('_', ' ');
            cleanedName = cleanedName.Trim();
            return cleanedName;
        }
    }
}