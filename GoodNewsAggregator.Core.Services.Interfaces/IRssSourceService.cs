﻿using GoodNewsAggregator.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Core.Services.Interfaces
{
    public interface IRssSourceService
    {
        Task<IEnumerable<RssSourceDto>> GetAllRssSources();

        Task<RssSourceDto> GetRssSourceById(Guid id);

        Task<IEnumerable<RssSourceDto>> GetRssSourcesByNameAndUrl(string name, string url);
    }
}
