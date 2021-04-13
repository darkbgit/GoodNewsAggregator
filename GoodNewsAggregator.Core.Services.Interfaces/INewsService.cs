using GoodNewsAggregator.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Core.Services.Interfaces
{
    public interface INewsService
    {
        Task<IEnumerable<NewsDto>> GetNewsBySourseId(Guid? id);
        Task<NewsDto> GetNewsById(Guid id);
        Task<IEnumerable<NewsDto>> GetNewsInfoFromRssSourse(RssSourseDto rssSourse);

        
        Task Add(NewsDto news);
        Task AddRange(IEnumerable<NewsDto> news);

        Task<int> Edit(NewsDto news);
        Task<int> Delete(NewsDto news);
    }
}
