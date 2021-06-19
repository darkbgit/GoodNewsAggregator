using GoodNewsAggregator.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Core.Services.Interfaces
{
    public interface INewsService
    {
        Task<IEnumerable<NewsDto>> GetNewsBySourceId(Guid? id);
        Task<NewsDto> GetNewsById(Guid id);
        Task<IEnumerable<NewsDto>> GetNewsInfoFromRssSource(RssSourceDto rssSource);
        Task<NewsWithRssNameDto> GetNewsWithRssSourceNameById(Guid id);
        Task<Tuple<IEnumerable<NewsDto> , int>> GetNewsPerPage(Guid[] rssIds,
            int pageNumber,
            int newsPerPage,
            string sortOrder);


        Task Add(NewsDto news);
        Task AddRange(IEnumerable<NewsDto> news);

        Task<int> Update(NewsDto news);
        Task<int> Delete(Guid id);
    }
}
