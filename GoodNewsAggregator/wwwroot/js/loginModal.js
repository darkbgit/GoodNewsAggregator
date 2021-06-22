$(function () {
    var placeholder = $('#modal-placeholder');
    //let myModal = new bootstrap.Modal(document.getElementById('modalLogin'));

    var pageUrl;
    $.ajaxSetup({ cache: false });
    $('button[data-toggle="ajax-modal"]').click(function (e) {
    e.preventDefault();
    pageUrl = window.href;
    var url = $(this).data('url');
        $.get(url).done(function (data) {
            placeholder.html(data);
            //placeholder.find('.modal').modal('show');
            const myModal = new window.bootstrap.Modal(document.getElementById('modalLogin'));
            myModal.show();
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

//function loginClick(e) {
//    e.preventDefault();
//    pageUrl = window.href;
//    var url = $(this).data('url');
//    $.get(url).done(function (data) {
//            placeholder.html(data);
//            const myModal = new window.bootstrap.Modal(document.getElementById('modalLogin'));
//            myModal.show();
//        });
//    });
//};


//let loginRequest = new XMLHttpRequest();

//loginRequest.open('GET', 'Account/UserInfo', true);

//loginRequest.onload = function () {
//    if (loginRequest.status >= 200 && loginRequest.status < 400) {
//        let response = loginRequest.responseText;
//        document.querySelector('#navbar-login').innerHTML = response;
//        document.querySelector('button[data-toggle="ajax-modal"]').addEventListener('click',
//            function() {
//            }, false);
//    }
//}

//loginRequest.send();