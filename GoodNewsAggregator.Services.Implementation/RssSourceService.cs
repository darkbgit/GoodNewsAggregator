using GoodNewsAggregator.Core.DTOs;
using GoodNewsAggregator.Core.Services.Interfaces;
using GoodNewsAggregator.DAL.Repositories.Implementation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Services.Implementation
{
    public class RssSourceService : IRssSourceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RssSourceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<RssSourceDto>> GetAllRssSources()
        {
            return await _unitOfWork.RssSources.FindBy(source =>
            !string.IsNullOrEmpty(source.Name))
                .Select(source => new RssSourceDto()
                {
                    Id = source.Id,
                    Name = source.Name,
                    Url = source.Url
                }).ToListAsync();
        }
    }
}
