using System.Collections.Generic;

namespace SeriesSelector.Data
{
    public interface IEpisdoeService
    {
        IList<EpisodeType> GetSourceEpisode(string sourcePath);
        IList<EpisodeType> GetNewFileList(IList<EpisodeType> sourceFiles);
        Dictionary<string, string> GetMappingValues();
        void WriteMappingValue(Dictionary<string, string> currentMappings);
    }
}