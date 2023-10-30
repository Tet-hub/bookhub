document.getElementById('AdminProfile').addEventListener('change', function () {
	const file = this.files[0];
	const image = document.getElementById('imagePreview');
	const uploadText = document.querySelector('.UploadText');

	if (file) {
		const reader = new FileReader();

		reader.onload = function (e) {
			image.src = e.target.result;
			image.style.display = 'block';
			uploadText.style.display = 'none';
		};

		reader.readAsDataURL(file);
	} else {
		image.src = '#';
		image.style.display = 'none';
		uploadText.style.display = 'block';
	}
});

$(document).ready(function () {
	// Check if the image source is '#' (fallback URL)
	if ($("#imagePreview").attr("src") === "#") {
		// If the image source is the placeholder URL, hide the element
		$("#imagePreview").css("display", "none");
		$(".UploadText").text("Upload Image");
	}
});