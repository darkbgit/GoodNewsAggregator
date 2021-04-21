using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodNewsAggregator.DAL.Core.Entities
{
    public class RssSourse : IBaseEntity
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        public string Url { get; set; }

        public virtual ICollection<News> NewsCollection { get; set; }
    }
}
