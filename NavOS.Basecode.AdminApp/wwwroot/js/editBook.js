document.addEventListener("DOMContentLoaded", function () {
    updateButtonText();
});

function updateButtonText() {
    var selectedGenres = document.querySelectorAll('input[name="SelectedGenres"]:checked');
    var buttonText = "Choose Genre";

    if (selectedGenres.length > 0) {
        var selectedGenresText = Array.from(selectedGenres).map(function (genre) {
            return genre.value;
        }).join(', ');

        buttonText = selectedGenresText;
    }

    document.getElementById('genreDropdownBtn').textContent = buttonText;
}