function modal_control(modal_name, action) {
    document.getElementById(modal_name).style.display = action;
}

function openModalSet(id, bookTitle) {
    modal_control("deleteModal", "block");
    document.getElementById('BookId').innerHTML = id;
    document.getElementById('BookTitle').innerHTML = bookTitle + "?";
}

function deleteBook() {
    let BookId = document.getElementById("BookId").innerHTML;
    window.location.href = "/Book/DeleteBook?bookId=" + BookId;
}

window.onclick = function (event) {
    let modal = document.getElementById('deleteModal');
    if (event.target == modal) {
        modal.style.display = 'none';
    }
}