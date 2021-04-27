// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


    function gettoken() {
        var token = '@Html.AntiForgeryToken()';
        token = $(token).val();
        return token;
    }

//checkedCheckBox = [];

//function OnChangeCheckBoxFunc(e) {
//    var assetId = e.value;

//    if (checkedCheckBox.indexOf(assetId) > -1)
//    {
//        var elementIndex = checkedCheckBox.indexOf(assetId);
//        checkedCheckBox.splice(elementIndex, 1);
//    }
//    else
//    {
//        checkedCheckBox.push(assetId);
//    }
//}

//function getCheckedCheckBoxesMove() {

//    $.ajax({
//        type: 'POST',
//        url: '@Url.Action("Index", "News")',
//        data: {
//            assetId: checkedCheckBox,
//        },
//        success: function (data) {
//            console.log('success!');
//        }
//    });
//}

//$(document).ready(function () {
//    $('#msform').submit(function () {
//        $.ajax({
//            url: this.action,
//            type: this.method,
//            data: $(this).serialize(),
//            success: function (result) {
//                $('#registermodel').html(reult);
//                $('registermodel').show();
//            },
//            error: function (result) {
//                $('#registermodel').html(reult);
//                $('registermodel').show();
//            }
//        });
//    });
//});

$(function () {
    var placeholder = $('#modal-placeholder');
    var pageUrl;
    $.ajaxSetup({ cache: false });
    $('button[data-toggle="ajax-modal"]').click(function (e) {
        //e.preventDefault();
        pageUrl = window.href;
        var url = $(this).data('url');
        $.get(url).done(function (data) {
            placeholder.html(data);
            placeholder.find('.modal').modal('show');
        });
    });

    placeholder.on('click', '[data-login="modal"]', function (e) {
        e.preventDefault();
        pageUrl = window.location.pathname;
        var form = $(this).parents('.modal').find('form');
        var actionUrl = form.attr('action');
        var dataToSend = form.serialize();
        dataToSend += '&ReturnUrl=' + pageUrl;

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

