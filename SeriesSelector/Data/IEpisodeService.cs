using System.Collections.Generic;

namespace SeriesSelector.Data
{
    public interface IEpisodeService
    {
        IList<EpisodeType> GetSourceEpisode(string sourcePath);
        IList<EpisodeType> GetNewFileList(IList<EpisodeType> sourceFiles);
        Dictionary<string, string> GetMappingValues();
        void WriteMappingValue(Dictionary<string, string> currentMappings);
    }
}