using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoodNewsAggregator.Models.ViewModels;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GoodNewsAggregator.HtmlHelpers
{
    public static class PaginationHelper
    {
        public static HtmlString PageLinks(this IHtmlHelper html,
            PageInfo pageInfo, Func<int, string> pageUrl)
        {
            var result = new StringBuilder();
            for (int i = 0; i <= pageInfo.TotalPages; i++)
            {
                var tagLi = new TagBuilder("li");
                tagLi.AddCssClass("page-item");
                tagLi.AddCssClass("disabled");


                var tagA = new TagBuilder("a");
                tagA.MergeAttribute("class", "page-link");
                tagA.MergeAttribute("href", pageUrl(i));
                tagA.InnerHtml.Append(i.ToString());

                tagLi.InnerHtml.Append(tagA.ToString());

                result.Append(tagLi.ToString());
            }

            return new HtmlString(result.ToString());
        }
    }
}
