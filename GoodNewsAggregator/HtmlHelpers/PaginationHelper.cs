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
        private const int PAGES_FROM_CURRENT_PAGE = 3;
        private const int NUMBER_OF_FIRST_PAGES = 2;
        private const int NUMBER_OF_LAST_PAGES = 2;

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
            var nextPage = isLastPage ? pageInfo.TotalPages : pageInfo.PageNumber + 1;

            ul.InnerHtml.AppendHtml(GetHtmlButtonCode(previousPage,
                pageUrl(previousPage),
                "Previous",
                isFirstPage));

            if (pageInfo.TotalPages <= NUMBER_OF_FIRST_PAGES + 
                                        PAGES_FROM_CURRENT_PAGE * 2 +
                                        NUMBER_OF_LAST_PAGES + 1)
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
                bool leftDots = false;
                bool rightDots = false;
                for (int i = 0; i < pageInfo.TotalPages; i++)
                {
                    
                    if (i > 1 && pageInfo.PageNumber - PAGES_FROM_CURRENT_PAGE > 2 && !leftDots && pageInfo.PageNumber != NUMBER_OF_FIRST_PAGES + PAGES_FROM_CURRENT_PAGE + 1)
                    {
                        ul.InnerHtml.AppendHtml(GetHtmlButtonCode(0,
                            pageUrl(0),
                            "...",
                            true));
                        leftDots = true;
                        i = pageInfo.PageNumber - PAGES_FROM_CURRENT_PAGE - 1;
                    }

                    if ((leftDots||pageInfo.PageNumber < 7) && !rightDots && i > pageInfo.PageNumber + PAGES_FROM_CURRENT_PAGE - 1 && pageInfo.PageNumber != pageInfo.TotalPages - (PAGES_FROM_CURRENT_PAGE + NUMBER_OF_LAST_PAGES + 1))
                    {
                        ul.InnerHtml.AppendHtml(GetHtmlButtonCode(0,
                            pageUrl(0),
                            "...",
                            true));
                        rightDots = true;
                        i = pageInfo.TotalPages - 2;
                    }
                    ul.InnerHtml.AppendHtml(GetHtmlButtonCode(i + 1,
                        pageUrl(i + 1),
                        (i + 1).ToString(),
                        i + 1 == pageInfo.PageNumber));
                }
            }
        
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
