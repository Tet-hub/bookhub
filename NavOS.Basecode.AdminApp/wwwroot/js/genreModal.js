function modal_control(modal_name, action) {
    document.getElementById(modal_name).style.display = action;
}

function openModalSet(id, genreName) {
    modal_control("deleteModal", "block");
    document.getElementById('IdGenre').innerHTML = id;
    document.getElementById('NameGenre').innerHTML = genreName + "?";
}

function deleteGenre() {
    let GenreId = document.getElementById("IdGenre").innerHTML;
    window.location.href = "/Genre/Delete?genreId=" + GenreId;
}

window.onclick = function (event) {
    let modal = document.getElementById('deleteModal');
    if (event.target == modal) {
        modal.style.display = 'none';
    }
}