
$('.form-check-input').on('blur', function() {
    $('.accordion-collapse.collapse').collapse('hide');
});

$('.accordion-button').on('blur', function () {
    setTimeout(collapseAccordion, 100);
    });

function collapseAccordion() {
    if (!document.activeElement.classList.contains('form-check-input'))
        $('.accordion-collapse.collapse').collapse('hide');
}

$('.form-check-input').on('change', function () {
    updatePageFromSwitch();
});

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
    var rssIds = [];

    $('.form-check-input:checked').each(function () {
        rssIds.push($(this).val());
    });
    return rssIds;
};

function updatePageFromSwitch(rssIds) {
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    var rssIds = getCheckedRssIds();

    $.ajax({
        type: 'POST',
        url: '/News/Index',
        data: {
            __RequestVerificationToken: token,
            rssIds: rssIds,
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

    $.ajax({
        type: 'POST',
        url: '/News/Index',
        data: {
            __RequestVerificationToken: token,
            rssIds: rssIds,
            page: page
        },
        //dataType: 'json',
        success: function (response) {
            console.log('success!');
            $('#outputField').html(response);
        }
    });
};