using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using GoodNewsAggregator.DAL.CQRS.Commands.NewsC;

namespace GoodNewsAggregator.DAL.CQRS.Validators.NewsV
{
    public class UpdateNewsValidator : AbstractValidator<UpdateNewsCommand>
    {
        public UpdateNewsValidator()
        {
            RuleFor(n => n.Title).NotEmpty();
            RuleFor(n => n.Url).NotEmpty();
            RuleFor(n => n.Body).NotEmpty();
            RuleFor(n => n.ShortNewsFromRssSource).NotEmpty();
            RuleFor(n => n.RssSourceId).NotEmpty();
        }
    }
}
