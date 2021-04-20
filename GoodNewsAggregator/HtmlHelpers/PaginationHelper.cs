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

            {
                var li = new TagBuilder("li");
                li.AddCssClass("page-item");
                if (pageInfo.PageNumber == 1) li.AddCssClass("disabled");

                var a = new TagBuilder("a");
                a.MergeAttribute("class", "page-link");
                a.MergeAttribute("value", "1");
                a.MergeAttribute("href", pageUrl(1));
                a.InnerHtml.Append("<<");

                li.InnerHtml.AppendHtml(a);

                ul.InnerHtml.AppendHtml(li);
            }

            {
                var li = new TagBuilder("li");
                li.AddCssClass("page-item");
                if (pageInfo.PageNumber == 1) li.AddCssClass("disabled");

                var a = new TagBuilder("a");
                a.MergeAttribute("class", "page-link");
                a.MergeAttribute("value", (pageInfo.PageNumber - 1).ToString());
                a.MergeAttribute("href", pageUrl(pageInfo.PageNumber - 1));
                a.InnerHtml.Append("<");

                li.InnerHtml.AppendHtml(a);

                ul.InnerHtml.AppendHtml(li);
            }

            {
                for (int i = 0; i < pageInfo.TotalPages; i++)
                {
                    var li = new TagBuilder("li");
                    li.AddCssClass("page-item");

                    if (i + 1 == pageInfo.PageNumber) li.AddCssClass("disabled");

                    var a = new TagBuilder("label");
                    a.MergeAttribute("class", "page-link");
                    //a.MergeAttribute("method", "post");
                    a.MergeAttribute("value", (i + 1).ToString());
                    //a.MergeAttribute("href", "");
                    a.InnerHtml.Append((i + 1).ToString());

                    li.InnerHtml.AppendHtml(a);

                    ul.InnerHtml.AppendHtml(li);
                }
            }

            {
                var li = new TagBuilder("li");
                li.AddCssClass("page-item");
                if (pageInfo.PageNumber == pageInfo.TotalPages) li.AddCssClass("disabled");

                var a = new TagBuilder("a");
                a.MergeAttribute("class", "page-link");
                a.MergeAttribute("value", (pageInfo.PageNumber + 1).ToString());
                a.MergeAttribute("href", pageUrl(pageInfo.PageNumber + 1));
                a.InnerHtml.Append(">");

                li.InnerHtml.AppendHtml(a);

                ul.InnerHtml.AppendHtml(li);
            }

            {
                var li = new TagBuilder("li");
                li.AddCssClass("page-item");
                if (pageInfo.PageNumber == pageInfo.TotalPages) li.AddCssClass("disabled");

                var a = new TagBuilder("a");
                a.MergeAttribute("class", "page-link");
                a.MergeAttribute("value", pageInfo.TotalPages.ToString());
                a.MergeAttribute("href", pageUrl(pageInfo.TotalPages));
                a.InnerHtml.Append(">>");

                li.InnerHtml.AppendHtml(a);

                ul.InnerHtml.AppendHtml(li);
            }

            nav.InnerHtml.AppendHtml(ul);

            var writer = new System.IO.StringWriter();
            nav.WriteTo(writer, HtmlEncoder.Default);

            return new HtmlString(writer.ToString());
        }
    }
}
