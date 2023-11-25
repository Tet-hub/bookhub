//script for image
function displayImage(input) {
    var imageElement = document.getElementById("bookImage");
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            imageElement.src = e.target.result;
        };
        reader.readAsDataURL(input.files[0]);
    }
}

//script for genre
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
