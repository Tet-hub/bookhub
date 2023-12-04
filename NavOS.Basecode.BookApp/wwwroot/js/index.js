const leftArrow = document.getElementById("left-arrow");
const rightArrow = document.getElementById("right-arrow");
const imagesContainer = document.getElementById("image-container");
let images = document.querySelectorAll(".model-images");

// This was done in case the user wanted to start first by pressing the left button
let cloneLastImage = images[images.length - 1].cloneNode(true);
imagesContainer.prepend(cloneLastImage);

// For the progressBar
const imageCount = images.length;
let currentImage = 0;

let firstImageIndexValue = 0;
let currentFirstImage = images[0];
let currentLastImage = images[images.length - 1];

// GSAP Animations
function buttonPressedAnimation(buttonId) {
    let rule = CSSRulePlugin.getRule(buttonId);
    let tl = gsap.timeline();

    gsap.set(rule, {
        cssRule: {
            scale: 1,
            border: "solid 0.1rem #fff",
            opacity: 0,
        },
    });

    tl.to(rule, {
        duration: 0.2,
        cssRule: {
            scale: 1.5,
            opacity: 1,
        },
    });

    tl.to(rule, {
        duration: 0.2,
        cssRule: {
            scale: 3,
            opacity: 0,
        },
        ease: "power2.out",
    });

    tl.to(rule, {
        duration: 0.2,
        cssRule: {
            scale: 1,
        },
        ease: "power2.in",
    });
}

function staggerImageAnimation(fromValue, toValue, direction) {
    gsap.fromTo(
        ".model-images",
        {
            translate: fromValue,
        },
        {
            translate: toValue,
            stagger: {
                from: direction,
                amount: 0.3,
            },
            ease: "power2.inOut",
        }
    );
}

function progressBarAnimation() {
    gsap.to("#progress-bar", {
        scaleX: `${1 / imageCount + (currentImage % imageCount) / imageCount}`,
        duration: 0.3 * ((imageCount - 1) / 2),
        ease: "power2.inOut",
    });
}

gsap.set("#progress-bar", {
    scaleX: `${1 / imageCount + (currentImage % imageCount) / imageCount}`,
});

// Image Placements
function moveImagesTotheLeft() {
    images = document.querySelectorAll(".model-images");
    let cloneFirstImage = images[1].cloneNode(true);
    imagesContainer.append(cloneFirstImage);

    let fromValue = `0`;
    let toValue = `calc(-100% - 0.5rem) `;

    staggerImageAnimation(fromValue, toValue, "start");
    images[0].remove();
}

function moveImagesTotheRight() {
    images = document.querySelectorAll(".model-images");
    let cloneLastImage = images[images.length - 2].cloneNode(true);

    imagesContainer.prepend(cloneLastImage);
    let fromValue = `calc(-200% - 1rem)`;
    let toValue = `calc(-100% - 0.5rem) `;
    staggerImageAnimation(fromValue, toValue, "end");
    images[images.length - 1].remove();
}

// Swipe functionality
let touchStartX = 0;
let touchEndX = 0;

function handleTouchStart(event) {
    touchStartX = event.touches[0].clientX;
}

function handleTouchMove(event) {
    touchEndX = event.touches[0].clientX;
}

function handleTouchEnd() {
    const swipeThreshold = 50;

    const deltaX = touchEndX - touchStartX;

    if (deltaX > swipeThreshold) {
        // Swipe right
        moveImagesTotheLeft();
        buttonPressedAnimation("#right-arrow:before");
    } else if (deltaX < -swipeThreshold) {
        // Swipe left
        moveImagesTotheRight();
        buttonPressedAnimation("#left-arrow:before");
    }

    // Reset touch coordinates
    touchStartX = 0;
    touchEndX = 0;

    // Update progress bar and current image
    gsap.set("#progress-bar", {
        scaleX: `${1 / imageCount + (currentImage % imageCount) / imageCount}`,
    });

    progressBarAnimation();
}

// AutoPlay configuration
let autoPlayInterval; // Variable to store the interval ID

const autoPlayDelay = 5000; // Adjust the delay (in milliseconds) between auto-play swipes

function startAutoPlay() {
    autoPlayInterval = setInterval(() => {
        moveImagesTotheLeft();
        buttonPressedAnimation("#right-arrow:before");
        currentImage = (currentImage + 1) % imageCount;

        gsap.set("#progress-bar", {
            scaleX: `${1 / imageCount + (currentImage % imageCount) / imageCount}`,
        });

        progressBarAnimation();
    }, autoPlayDelay);
}

function stopAutoPlay() {
    clearInterval(autoPlayInterval);
}

// Start autoplay when the page loads
startAutoPlay();

// Add a function to restart autoplay when it's stopped
function restartAutoPlay() {
    stopAutoPlay();
    startAutoPlay();
}

// Add a click event listener to the imagesContainer to restart autoplay on any interaction
imagesContainer.addEventListener("click", restartAutoPlay);

// Event Listeners
leftArrow.addEventListener("click", () => {
    moveImagesTotheRight();
    buttonPressedAnimation("#left-arrow:before");
    currentImage = (currentImage - 1 + imageCount) % imageCount;

    gsap.set("#progress-bar", {
        scaleX: `${1 / imageCount + (currentImage % imageCount) / imageCount}`,
    });

    progressBarAnimation();
});

rightArrow.addEventListener("click", () => {
    moveImagesTotheLeft();
    buttonPressedAnimation("#right-arrow:before");
    currentImage = (currentImage + 1) % imageCount;

    gsap.set("#progress-bar", {
        scaleX: `${1 / imageCount + (currentImage % imageCount) / imageCount}`,
    });

    progressBarAnimation();
});
