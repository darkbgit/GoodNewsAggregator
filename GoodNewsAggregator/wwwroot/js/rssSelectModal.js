$(function () {
    let placeholder = $('#modal-placeholder-rss');
    
    //$.ajaxSetup({ cache: false });
    $('div[data-toggle="ajax-modal"]').click(function (e) {
        //e.preventDefault();
        let rssIds = getCheckedRssIds();
        let url = $(this).data('url');
        $.ajax({
            type: 'POST',
            url: url,
            data: {
                rssIds: rssIds
            },
                success: function(data) {
                placeholder.html(data);
                let rssModal = new window.bootstrap.Modal(document.getElementById('modalRss'));
                //placeholder.find('.modal').modal('show');
                rssModal.show();
            }
        });
    });

});

//let rssCheckBoxItems = document.querySelectorAll('.form-check-input');

//[].forEach.call(rssCheckBoxItems,
//    function (item) {
//        item.onchange = updatePageFromSwitch;
//    });

//function gettoken() {
//    var token = '@Html.AntiForgeryToken()';
//    token = $(token).val();
//    return token;
//}

function getCheckedRssIdsFromModal() {
    let rssIds = [];

    $('.form-check-input:checked').each(function () {
        rssIds.push($(this).val());
    });
    return rssIds;
};

function updatePageFromSwitch(par) {
    let form = $('#__AjaxAntiForgeryForm');
    let token = $('input[name="__RequestVerificationToken"]', form).val();
    let rssIds = getCheckedRssIdsFromModal();
    let sortOrder = document.querySelector('#sortOrder').getAttribute('value');
    let rssId = $(par).val();
    let isChecked = par.checked;

    $.ajax({
        type: 'POST',
        url: '/News/Index',
        data: {
            __RequestVerificationToken: token,
            rssIds: rssIds,
            sortOrder: sortOrder
        },
        //dataType: 'json',
        success: function (response) {
            console.log('success!');
            $('#outputField').html(response);
            document.getElementById(rssId).setAttribute('value', isChecked ? true : false);
        }
    });
};