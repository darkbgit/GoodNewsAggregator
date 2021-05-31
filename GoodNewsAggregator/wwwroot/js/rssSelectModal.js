$(function () {
    let placeholder = $('#modal-placeholder-rss');
    //let myModal = new bootstrap.Modal(document.getElementById('modalLogin'));

    var pageUrl;
    $.ajaxSetup({ cache: false });
    $('div[data-toggle="ajax-modal"]').click(function (e) {
        e.preventDefault();
        pageUrl = window.href;
        var url = $(this).data('url');
        $.get(url).done(function (data) {
            placeholder.html(data);
            placeholder.find('.modal').modal('show');
        });
    });

    placeholder.on('change', '.form-check-label', function (e) {
        e.preventDefault();
        pageUrl = window.location.pathname;
        var form = $(this).parents('.modal').find('form');
        var actionUrl = form.attr('action');
        var dataToSend = form.serialize();
        if (pageUrl !== "\/") dataToSend += '&ReturnUrl=' + pageUrl;

        $.post(actionUrl, dataToSend).done(function (data) {

            if (data.result == 'Redirect') {
                window.location.href = data.url;
            } else {
                var newBody = $('.modal-body', data);


                var isValid = newBody.find('[name="IsValid"]').val() == 'True';
                if (isValid) {
                    placeholder.find('.modal').modal('hide');
                } else {
                    placeholder.find('.modal-body').replaceWith(newBody);
                }
            }


        });
    });

    placeholder.on('click', '[data-login="modal"]', function (e) {
        e.preventDefault();
        pageUrl = window.location.pathname;
        var form = $(this).parents('.modal').find('form');
        var actionUrl = form.attr('action');
        var dataToSend = form.serialize();
        if (pageUrl !== "\/") dataToSend += '&ReturnUrl=' + pageUrl;

        $.post(actionUrl, dataToSend).done(function (data) {

            if (data.result == 'Redirect') {
                window.location.href = data.url;
            } else {
                var newBody = $('.modal-body', data);


                var isValid = newBody.find('[name="IsValid"]').val() == 'True';
                if (isValid) {
                    placeholder.find('.modal').modal('hide');
                } else {
                    placeholder.find('.modal-body').replaceWith(newBody);
                }
            }


        });
    });

});

//let rssCheckBoxItems = document.querySelectorAll('.form-check-input');

//[].forEach.call(rssCheckBoxItems,
//    function (item) {
//        item.onchange = updatePageFromSwitch;
//    });

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