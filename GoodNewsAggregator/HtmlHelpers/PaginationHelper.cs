using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
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
            var nav = new TagBuilder("nav");
            nav.MergeAttribute("aria-label", "Page navigation");

            var ul = new TagBuilder("ul");
            ul.AddCssClass("pagination");
            ul.AddCssClass("justify-content-center");

            var isFirstPage = pageInfo.PageNumber == 1;
            var isLastPage = pageInfo.PageNumber == pageInfo.TotalPages;

            var previousPage = isFirstPage ? 1 : pageInfo.PageNumber - 1;

            ul.InnerHtml.AppendHtml(GetHtmlButtonCode(previousPage,
                pageUrl(previousPage),
                "Previous",
                isFirstPage));

            if (pageInfo.TotalPages <= 11)
            {
                for (int i = 0; i < pageInfo.TotalPages; i++)
                {
                    ul.InnerHtml.AppendHtml(GetHtmlButtonCode(i + 1,
                        pageUrl(i + 1),
                        (i + 1).ToString(),
                        i + 1 == pageInfo.PageNumber));
                }
            }
            else
            {
                for (int i = 0; i < pageInfo.TotalPages; i++)
                {
                    ul.InnerHtml.AppendHtml(GetHtmlButtonCode(i + 1,
                        pageUrl(i + 1),
                        (i + 1).ToString(),
                        i + 1 == pageInfo.PageNumber));


                }
            }

            var nextPage = isLastPage ? pageInfo.TotalPages : pageInfo.PageNumber + 1;

            ul.InnerHtml.AppendHtml(GetHtmlButtonCode(nextPage,
                pageUrl(nextPage),
                "Next",
                isLastPage));
            
            nav.InnerHtml.AppendHtml(ul);

            var writer = new System.IO.StringWriter();
            nav.WriteTo(writer, HtmlEncoder.Default);

            return new HtmlString(writer.ToString());
        }

        private static IHtmlContent GetHtmlButtonCode(int pageNumber,
            string pageUrl,
            string buttonName,
            bool isDisabled)
        {
            var li = new TagBuilder("li");
            li.AddCssClass("page-item");
            if (isDisabled) li.AddCssClass("disabled");

            var a = new TagBuilder("a");
            a.MergeAttribute("class", "page-link");
            a.MergeAttribute("value", pageNumber.ToString());
            //a.MergeAttribute("href", pageUrl);
            a.InnerHtml.Append(buttonName);

            li.InnerHtml.AppendHtml(a);

            return li;
        }
    }
}
