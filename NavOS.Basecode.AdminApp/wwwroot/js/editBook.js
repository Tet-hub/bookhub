document.addEventListener("DOMContentLoaded", function () {
    updateButtonText();
});

function updateButtonText(event) {
    if (event) {
        event.preventDefault();
    }

    var selectedGenres = document.querySelectorAll('input[name="SelectedGenres"]:checked');
    var buttonText = "Choose Genre";
    var maxTextLength = 50;

    if (selectedGenres.length > 0) {
        var selectedGenresText = Array.from(selectedGenres).map(function (genre) {
            return genre.value;
        }).join(', ');

        if (selectedGenresText.length > maxTextLength) {
            selectedGenresText = selectedGenresText.substring(0, maxTextLength) + '...';
        }

        buttonText = selectedGenresText;
    }

    document.getElementById('genreDropdownBtn').textContent = buttonText;
}