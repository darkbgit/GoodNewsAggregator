
let rssCheckBoxItems = document.querySelectorAll('.form-check-input');

[].forEach.call(rssCheckBoxItems, function (item) {
    item.onchange = updatePageFromSwitch;
    item.onblur = rssOnBlur;
})

function rssOnBlur() {
    let collapseEl = document.querySelector('.accordion-collapse.collapse'); //.collapse('hide');
    return new bootstrap.Collapse(collapseEl);
}

function collapseAccordion() {
    if (!document.activeElement.classList.contains('form-check-input'))
        rssOnBlur();
}


document.querySelector('.accordion-button').onblur = function () {
    setTimeout(collapseAccordion, 100);
    };




//document.querySelector
$('body').on('click', '.page-item', function () {
    var page = $(this).children('a').attr('value');
    //alert(page);
    updatePageFromPagination(page);
});

$('#aggregate').on('click', function () {
    let _this = $(this);
    let form = $('#__AjaxAntiForgeryForm');
    let token = $('input[name="__RequestVerificationToken"]', form).val();
    $.ajax({
        type: 'POST',
        url: '/News/Aggregate',
        data: {
            __RequestVerificationToken: token,
        },
        beforeSend: function () {
            _this.find('.spinner-border').removeClass('d-none');
        },
        success: function () {
            window.location.href = '/News/Index';
        },
        error: function () {
            _this.find('.spinner-border').addClass('d-none');
            alert('Error. Please aggregate news few moments later');
        }
    });
});

function gettoken() {
    var token = '@Html.AntiForgeryToken()';
    token = $(token).val();
    return token;
}

function getCheckedRssIds() {
    let rssIds = [];

    $('.form-check-input:checked').each(function () {
        rssIds.push($(this).val());
    });
    return rssIds;
};

function updatePageFromSwitch() {
    let form = $('#__AjaxAntiForgeryForm');
    let token = $('input[name="__RequestVerificationToken"]', form).val();
    let rssIds = getCheckedRssIds();
    let orderByDate = document.querySelector('#orderByDate').getAttribute('value');
    let orderByRating = document.querySelector('#orderByRating').getAttribute('value');

    $.ajax({
        type: 'POST',
        url: '/News/Index',
        data: {
            __RequestVerificationToken: token,
            rssIds: rssIds,
            orderByDate: orderByDate,
            orderByRating: orderByRating
        },
        //dataType: 'json',
        success: function (response) {
            console.log('success!');
            $('#outputField').html(response);
        }
    });
};

function updatePageFromPagination(page) {
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    var rssIds = getCheckedRssIds();
    let orderByDate = document.querySelector('#orderByDate').getAttribute('value');
    let orderByRating = document.querySelector('#orderByRating').getAttribute('value');

    $.ajax({
        type: 'POST',
        url: '/News/Index',
        data: {
            __RequestVerificationToken: token,
            rssIds: rssIds,
            page: page,
            orderByDate: orderByDate,
            orderByRating: orderByRating
        },
        //dataType: 'json',
        success: function (response) {
            console.log('success!');
            $('#outputField').html(response);
        }
    });
};

$('#sort-for-date').on('click', function () {
    let _this = $(this);
    let form = $('#__AjaxAntiForgeryForm');
    let token = $('input[name="__RequestVerificationToken"]', form).val();
    $.ajax({
        type: 'POST',
        url: '/News/Aggregate',
        data: {
            __RequestVerificationToken: token,
        },
        beforeSend: function () {
            _this.find('.spinner-border').removeClass('d-none');
        },
        success: function () {
            window.location.href = '/News/Index';
        },
        error: function () {
            _this.find('.spinner-border').addClass('d-none');
            alert('Error. Please aggregate news few moments later');
        }
    });
});