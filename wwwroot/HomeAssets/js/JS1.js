//code to resize images + 3sec interval between each image
const images = document.querySelectorAll('.resize-image');

const MyWidth = 400;
const MyHeight = 300;

images.forEach((image) => {
    image.width = MyWidth;
    image.height = MyHeight;
});

//..................................................

//code to change img every 3 sec
const imageSources = [
    "../homeassets/img/44.png",
    "../homeassets/img/tw.png",
    "../homeassets/img/222.png"
];

const imageElement = document.getElementById("LOGINimage");
const imageContainer = document.getElementById("baseImgId");

let currentIndex = 0;

function changeImageWithFadeIn() {
    imageContainer.style.opacity = 0;

    setTimeout(() => {
        imageElement.src = imageSources[currentIndex];
        currentIndex = (currentIndex + 1) % imageSources.length;
        imageContainer.style.opacity = 1;
    }, 1000);
}

changeImageWithFadeIn();
setInterval(changeImageWithFadeIn, 3000);


//......................................................................................................