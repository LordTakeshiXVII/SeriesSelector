using System.Collections.Generic;

namespace SeriesSelector.Data
{
    public interface IMovieService
    {
        IList<EpisodeType> GetMovies(string sourcePath);
        IList<EpisodeType> TryMapMovies(IList<EpisodeType> list);
        List<string> SearchFor(string oldName);
    }
}