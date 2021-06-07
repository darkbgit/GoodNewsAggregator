const arrowCircleUp =
    '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-arrow-up-circle" viewBox="0 0 16 16"><path fill-rule="evenodd" d="M1 8a7 7 0 1 0 14 0A7 7 0 0 0 1 8zm15 0A8 8 0 1 1 0 8a8 8 0 0 1 16 0zm-7.5 3.5a.5.5 0 0 1-1 0V5.707L5.354 7.854a.5.5 0 1 1-.708-.708l3-3a.5.5 0 0 1 .708 0l3 3a.5.5 0 0 1-.708.708L8.5 5.707V11.5z" /></svg>';
const arrowCircleDown =
    '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-arrow-down-circle" viewBox="0 0 16 16"><path fill-rule="evenodd" d = "M1 8a7 7 0 1 0 14 0A7 7 0 0 0 1 8zm15 0A8 8 0 1 1 0 8a8 8 0 0 1 16 0zM8.5 4.5a.5.5 0 0 0-1 0v5.793L5.354 8.146a.5.5 0 1 0-.708.708l3 3a.5.5 0 0 0 .708 0l3-3a.5.5 0 0 0-.708-.708L8.5 10.293V4.5z" /></svg >';


//let rssCheckBoxItems = document.querySelectorAll('.form-check-input');

//[].forEach.call(rssCheckBoxItems,
//    function(item) {
//        item.onchange = updatePageFromSwitch;
//        item.onblur = rssOnBlurTimer;
//    });

//function rssOnBlur() {
//    if (!document.activeElement.classList.contains('form-check-input')) {
//        let collapseEl = document.querySelector('.accordion-collapse.collapse'); //.collapse('hide');
//        return new bootstrap.Collapse(collapseEl);
//    }
//}

//function rssOnBlurTimer() {
//    setTimeout(rssOnBlur, 100);
//}


/*document.querySelector('.accordion-button').onblur = rssOnBlurTimer;*/


$('body').on('click', '.page-item', function () {
    var page = $(this).children('a').attr('value');
    updatePageFromPagination(page);
});

function gettoken() {
    var token = '@Html.AntiForgeryToken()';
    token = $(token).val();
    return token;
}

function getCheckedRssIds() {
    let rssIds = [];

    $('.rss-source').each(function () {
        let value = $(this).val().toLowerCase();
        if ($(this).val().toLowerCase() === 'true') {
            rssIds.push($(this).attr('id'));
        }
    });
    return rssIds;
};

function updatePageFromPagination(page) {
    let form = $('#__AjaxAntiForgeryForm');
    let token = $('input[name="__RequestVerificationToken"]', form).val();
    let rssIds = getCheckedRssIds();
    let sortOrder = document.querySelector('#sortOrder').getAttribute('value');

    $.ajax({
        type: 'POST',
        url: '/News/Index',
        data: {
            __RequestVerificationToken: token,
            rssIds: rssIds,
            page: page,
            sortOrder: sortOrder
        },
        //dataType: 'json',
        success: function (response) {
            console.log('success!');
            $('#outputField').html(response);
        }
    });
};

function orderByDateClick () {
    let form = document.querySelector('#__AjaxAntiForgeryForm');
    let token = $('input[name="__RequestVerificationToken"]', form).val();
    let rssIds = getCheckedRssIds();
    let sortOrder = document.querySelector('#sortOrder').getAttribute('value');
    let orderByDate = document.querySelector('#orderByDate').getAttribute('value');
    let orderByRating = document.querySelector('#orderByRating').getAttribute('value');
    let sortOrderNew;
    let orderByDateNew;
    let orderByRatingNew;
    if (sortOrder == 'date_desc' || sortOrder == 'Date') {
        sortOrderNew = sortOrder == 'date_desc' ? 'Date' : 'date_desc';
        orderByDateNew = orderByDate == 0 ? 1 : 0;
        orderByRatingNew = 0;
    } else {
        sortOrderNew = 'date_desc';
        orderByDateNew = 1;
        orderByRatingNew = 0;
    }

    $.ajax({
        type: 'POST',
        url: '/News/Index',
        data: {
            __RequestVerificationToken: token,
            rssIds: rssIds,
            sortOrder: sortOrderNew
        },
        success: function (response) {
            console.log('success!');
            $('#outputField').html(response);
            document.querySelector('#sortOder').setAttribute('value', sortOrderNew);
            document.querySelector('#orderByDate').setAttribute('value', orderByDateNew);
            document.querySelector('#orderByDate').innerHTML = orderByDateNew == 0 ? arrowCircleUp : arrowCircleDown;
            document.querySelector('#orderByRating').setAttribute('value', orderByRatingNew);
            document.querySelector('#orderByRating').innerHTML = '';
        }
    });
};

function orderByRatingClick() {
    let form = document.querySelector('#__AjaxAntiForgeryForm');
    let token = $('input[name="__RequestVerificationToken"]', form).val();
    let rssIds = getCheckedRssIds();
    let sortOrder = document.querySelector('#sortOrder').getAttribute('value');
    let orderByDate = document.querySelector('#orderByDate').getAttribute('value');
    let orderByRating = document.querySelector('#orderByRating').getAttribute('value');
    let sortOrderNew;
    let orderByDateNew;
    let orderByRatingNew;
    if (sortOrder == 'rating_desc' || sortOrder == 'Rating') {
        sortOrderNew = sortOrder == 'rating_desc' ? 'Rating' : 'rating_desc';
        orderByRatingNew = orderByRating == 0 ? 1 : 0;
        orderByDateNew = 0;
    } else {
        sortOrderNew = 'rating_desc';
        orderByRatingNew = 1;
        orderByDateNew = 0;
    }

    $.ajax({
        type: 'POST',
        url: '/News/Index',
        data: {
            __RequestVerificationToken: token,
            rssIds: rssIds,
            sortOrder: sortOrderNew
        },
        success: function (response) {
            console.log('success!');
            $('#outputField').html(response);
            document.querySelector('#sortOrder').setAttribute('value', sortOrderNew);
            document.querySelector('#orderByDate').setAttribute('value', orderByDateNew);
            document.querySelector('#orderByDate').innerHTML = '';
            document.querySelector('#orderByRating').setAttribute('value', orderByRatingNew);
            document.querySelector('#orderByRating').innerHTML = orderByRatingNew == 0 ? arrowCircleUp : arrowCircleDown;
        }
    });
};