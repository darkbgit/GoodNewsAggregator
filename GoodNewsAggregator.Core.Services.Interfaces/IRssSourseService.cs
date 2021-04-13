using GoodNewsAggregator.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Core.Services.Interfaces
{
    public interface IRssSourseService
    {
        Task<IEnumerable<RssSourseDto>> GetAllRssSourses();
    }
}
