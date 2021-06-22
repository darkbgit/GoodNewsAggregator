////$('#aggregate').on('click', function () {
////    let _this = $(this);
////    let form = $('#__AjaxAntiForgeryForm');
////    let token = $('input[name="__RequestVerificationToken"]', form).val();
////    $.ajax({
////        type: 'POST',
////        url: '/News/Aggregate',
////        data: {
////            __RequestVerificationToken: token,
////        },
////        beforeSend: function () {
////            _this.find('.spinner-border').removeClass('d-none');
////        },
////        success: function () {
////            window.location.href = '/News/Index';
////        },
////        error: function () {
////            _this.find('.spinner-border').addClass('d-none');
////            alert('Error. Please aggregate news few moments later');
////        }
////    });
////});

window.onload = function() {
    let aggregateElement = document.querySelector('#aggregate');

    aggregateElement.onclick = function () {
        const token = document.querySelector('#__AjaxAntiForgeryForm input[name="__RequestVerificationToken"]').value;
        aggregate(token, aggregateElement);
    };
}

function aggregate(token, aggregateElement) {
    const request = new XMLHttpRequest();
    let fd = new FormData();
    fd.append('__RequestVerificationToken', token);
    request.open('POST', '/News/Aggregate', true);

    aggregateElement.querySelector('.spinner-border').classList.remove('d-none');
    request.onload = function () {
        aggregateElement.querySelector('.spinner-border').classList.add('d-none');
        if (request.status >= 200 && request.status < 400) {
            alert('Новости аггригированны');
        } else {
            alert('Error. Please aggregate news few moments later');
        }
    }
    request.send(fd);
}

