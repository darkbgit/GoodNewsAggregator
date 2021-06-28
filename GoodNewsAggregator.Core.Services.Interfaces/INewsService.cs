using GoodNewsAggregator.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Core.Services.Interfaces
{
    public interface INewsService
    {
        Task<IEnumerable<NewsDto>> GetNewsByRssSourceIds(Guid[] ids);
        Task<NewsDto> GetNewsById(Guid id);
        Task<IEnumerable<NewsDto>> GetAllNews();
        Task<NewsWithRssNameDto> GetNewsWithRssSourceNameById(Guid id);
        Task<Tuple<IEnumerable<NewsDto> , int>> GetNewsPerPage(Guid[] rssIds,
            int pageNumber,
            int newsPerPage,
            string sortOrder,
            double? minimalRating);

        Task Aggregate();
        Task GetBodies();
        Task Rate30News();

        Task<int> Add(NewsDto news);
        Task<int> AddRange(IEnumerable<NewsDto> news);

        Task<int> Update(NewsDto news);
        Task<int> UpdateRange(IEnumerable<NewsDto> news);
        Task<int> Delete(Guid id);
    }
}
