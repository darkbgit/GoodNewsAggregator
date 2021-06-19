using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GoodNewsAggregator.Models.ViewModels.News
{
    public class EditNewsViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Заголовок")]
        public string Title { get; set; }

        [Required]
        public string Url { get; set; }

        [Required]
        [Display(Name = "Тело новости")]
        public string Body { get; set; }

        [Required]
        [Display(Name = "Краткое описание")]
        public string ShortNewsFromRssSource { get; set; }

        [Display(Name = "Адрес картинки")]
        public string ImageUrl { get; set; }

        public DateTime PublicationDate { get; set; }

        [Display(Name = "Категория")]
        public string Category { get; set; }

        [Display(Name = "Автор новости")]
        public string Author { get; set; }

        [Range(0.0, 10.0)]
        [Display(Name = "Рейтинг")]
        public double Rating { get; set; }


        public Guid RssSourceId { get; set; }

        [Display(Name = "RSS источник")]
        public SelectList Sources { get; set; }
    }
}
