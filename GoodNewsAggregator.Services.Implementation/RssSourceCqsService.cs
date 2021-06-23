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
using GoodNewsAggregator.DAL.CQRS.Queries;
using GoodNewsAggregator.DAL.CQRS.Queries.RssSourceQ;
using MediatR;

namespace GoodNewsAggregator.Services.Implementation
{
    public class RssSourceCqsService : IRssSourceService
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public RssSourceCqsService(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<IEnumerable<RssSourceDto>> GetAllRssSources()
        {
            throw new NotImplementedException();
        }

        public async Task<RssSourceDto> GetRssSourceById(Guid id)
        {
            var query = new GetRssSourceByIdQuery(id);
            var result = await _mediator.Send(query);
            return result;
        }

        public async Task<IEnumerable<RssSourceDto>> GetRssSourcesByNameAndUrl(string name, string url)
        {
            var query = new GetRssSourceByNameAndUrlQuery(name, url);
            var result = await _mediator.Send(query);
            return result;
        }
    }
}
