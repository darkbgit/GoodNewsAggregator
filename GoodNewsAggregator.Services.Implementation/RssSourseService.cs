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
    public class RssSourseService : IRssSourseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RssSourseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<RssSourseDto>> GetAllRssSourses()
        {
            return await _unitOfWork.RssSourses.FindBy(sourse =>
            !string.IsNullOrEmpty(sourse.Name))
                .Select(sourse => new RssSourseDto()
                {
                    Id = sourse.Id,
                    Name = sourse.Name,
                    Url = sourse.Url
                }).ToListAsync();
        }
    }
}
