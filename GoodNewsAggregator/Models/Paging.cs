using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;

namespace GoodNewsAggregator.Models
{
    public class Paging
    {
        public Paging(int pageSize, int pageNumber, int total, string url)
        {
            PageSize = pageSize;
            PageNumber = pageNumber;
            Total = total;
            Url = url;
        }

        private int PageSize { get; set; }
        private int PageNumber { get; set; }
        private int Total { get; set; }
        private string Url { get; set; }

        private int MaxPageCount => (int) Math.Ceiling((double) Total / PageSize);

        public int[] GetPaging()
        {
            const int maxBefore = 5;
            const int maxAfter = 5;

            var first = (PageNumber - maxBefore < 1) ? PageNumber - maxBefore : 1;
            var last = (PageNumber + maxAfter > MaxPageCount) ? PageNumber + maxAfter : MaxPageCount;

            return Enumerable.Range(first, last - first).ToArray();
        }

        public bool IsCurrent(int i) => PageNumber == i;
        public bool IsFirst => IsCurrent(1);
        public bool IsLast => IsCurrent(MaxPageCount);

        public string GetUrl(int i)
        {
            var uri = new Uri(Url);
            var baseUri = uri.GetComponents(UriComponents.Scheme |
                                                UriComponents.Port |
                                                UriComponents.Host |
                                                UriComponents.Path,
                                                UriFormat.UriEscaped);
            var query = QueryHelpers.ParseQuery(uri.Query);

            var items = query.SelectMany(x => x.Value,
                    (col, value) => new KeyValuePair<string, string>(col.Key, value))
                .ToList();
            items.RemoveAll(x => x.Key == "PageNumber");

            var qb = new QueryBuilder(items);
            qb.Add("PageNumber", i.ToString());

            var fullUri = baseUri + qb.ToQueryString();
            return fullUri;

        }

        public string FirstUrl => GetUrl(1);
        public string LastUrl => GetUrl(MaxPageCount);

        public string PrevUrl => GetUrl(PageNumber - 1);
        public string NextUrl => GetUrl(PageNumber + 1);

    }
}
