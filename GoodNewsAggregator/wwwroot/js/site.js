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

$(document).ready(function () {
    $('#msform').submit(function () {
        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            success: function (result) {
                $('#registermodel').html(reult);
                $('registermodel').show();
            },
            error: function (result) {
                $('#registermodel').html(reult);
                $('registermodel').show();
            }
        });
    });
});

$(function () {
    $.ajaxSetup({ cache: false });
    $('.loginClick').click(function (e) {
        e.preventDefault();
        $.get(this.href, function (data) {
            $('#modalDialogContent').html(data);
            $('#modalLogin').modal('show');
        });
    });
});


$(document).ready(function () {
    $('#loginForm').submit(function () {
        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            success: function (data) {
                $('#registermodel').html(data);
                $('registermodel').show();
            },
            error: function (data) {
                console.log('error');
                $('#modalDialogContent').html(data.responseText);
                //$('#modalLogin').modal('show');
            }
        });
    });
});
