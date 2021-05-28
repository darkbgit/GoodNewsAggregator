using GoodNewsAggregator.Core.DTOs;
using GoodNewsAggregator.Core.Services.Interfaces;
using GoodNewsAggregator.DAL.Repositories.Implementation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace GoodNewsAggregator.Services.Implementation
{
    public class RssSourceService : IRssSourceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RssSourceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RssSourceDto>> GetAllRssSources()
        {
            return await _unitOfWork.RssSources.FindBy(source =>
            !string.IsNullOrEmpty(source.Name))
                .OrderBy(r => r.Name)
                .Select(source => _mapper.Map<RssSourceDto>(source))
                .ToListAsync();
        }
    }
}
