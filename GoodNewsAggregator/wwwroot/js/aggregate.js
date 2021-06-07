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