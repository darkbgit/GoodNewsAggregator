let commentsDisplayElement = document.querySelector('#comments-display');
let isCommentsDisplay = false;

//let newsId = document.onload.querySelector('#newsId').value;

commentsDisplayElement.onclick = function() {
    toggleComments();
}

function toggleComments() {
    const url = window.location.pathname;
    const id = url.substring(url.lastIndexOf('/') + 1);
    const commentsContainer = document.querySelector('#comments-container');
    if (commentsDisplayElement != null) {
        if (isCommentsDisplay === true) {
            commentsDisplayElement.firstChild.nodeValue = 'Показать коментарии';
            commentsContainer.innerHTML = '';
        } else {
            commentsDisplayElement.firstChild.nodeValue = 'Скрыть коментарии';
            loadComments(id, commentsContainer);
        }
        isCommentsDisplay = !isCommentsDisplay;
    }
    
}

function loadComments(newsId, commentsContainer) {
    let request = new XMLHttpRequest();

    request.open('GET', `/Comments/List?newsId=${newsId}`, true);

    request.onload = function() {
        if (request.status >= 200 && request.status < 400) {
            let response = request.responseText;
            commentsContainer.innerHTML = response;

            document.querySelector('#create-comment-btn')
                .addEventListener('click', createComment, false);
        }
    }

    request.send();
}



function createComment(e) {
    //e.preventDefault();
    let commentText = document.querySelector('#commentText').value;
    let newsId = document.querySelector('#newsId').value;

    if (commentText.length === 0) {
        return alert('Введите коментарий');
    }

    let request = new XMLHttpRequest();

    request.open('POST', '/Comments/Create', true);
    request.setRequestHeader('Content-Type', 'application/json');
    request.onload = function() {
        if (request.status >= 200 && request.status < 400) {
            commentText.value = '';
            const commentsContainer = document.querySelector('#comments-container');
            loadComments(newsId, commentsContainer);
            alert('Комментарий успешно отправлен');
        } else if (request.status = 401) {
            alert('Зарегистрируйтусь для отправки коментариев');
        }
    }

    request.send(JSON.stringify({
        commentText: commentText,
        newsId: newsId
    }));
}

setInterval(function () {
    const commentsContainer = document.querySelector('#comments-container');
    let newsId = document.querySelector('#newsId').value;
    loadComments(newsId, commentsContainer);
}, 15000);