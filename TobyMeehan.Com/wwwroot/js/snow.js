var particlesOnScreen = 200;
var canvas, ctx, particlesArray, w, h, snowInterval;

function snow_random(min, max) {
    return min + Math.random() * (max - min + 1);
};

function clientResize(ev) {
    w = canvas.width = window.innerWidth;
    h = canvas.height = window.innerHeight;
};
window.addEventListener("resize", clientResize);

function createSnowFlakes() {
    for (var i = 0; i < particlesOnScreen; i++) {
        particlesArray.push({
            x: Math.random() * w,
            y: Math.random() * h,
            opacity: Math.random(),
            speedX: snow_random(-7, 7),
            speedY: snow_random(5, 9),
            radius: snow_random(0.5, 4.2),
        })
    }
};

function drawSnowFlakes() {
    for (var i = 0; i < particlesArray.length; i++) {
        var gradient = ctx.createRadialGradient(
            particlesArray[i].x,
            particlesArray[i].y,
            0,
            particlesArray[i].x,
            particlesArray[i].y,
            particlesArray[i].radius
        );

        gradient.addColorStop(0, "rgba(255, 255, 255," + particlesArray[i].opacity + ")");  // white
        gradient.addColorStop(.8, "rgba(210, 236, 242," + particlesArray[i].opacity + ")");  // bluish
        gradient.addColorStop(1, "rgba(237, 247, 249," + particlesArray[i].opacity + ")");   // lighter bluish

        ctx.beginPath();
        ctx.arc(
            particlesArray[i].x,
            particlesArray[i].y,
            particlesArray[i].radius,
            0,
            Math.PI * 2,
            false
        );

        ctx.fillStyle = gradient;
        ctx.fill();
    }
};

function moveSnowFlakes() {
    for (var i = 0; i < particlesArray.length; i++) {
        particlesArray[i].x += particlesArray[i].speedX;
        particlesArray[i].y += particlesArray[i].speedY;

        if (particlesArray[i].y > h) {
            particlesArray[i].x = Math.random() * w * 1.5;
            particlesArray[i].y = -50;
        }
    }
};

function updateSnowFall() {
    ctx.clearRect(0, 0, w, h);
    drawSnowFlakes();
    moveSnowFlakes();
};

function startSnow(canvas) {
    particlesArray = [];
    clearInterval(snowInterval);
    canvas = document.getElementById(canvas);
    ctx = canvas.getContext("2d");
    w, h;
    w = canvas.width = window.innerWidth;
    h = canvas.height = window.innerHeight;

    snowInterval = setInterval(updateSnowFall, 18);
    createSnowFlakes();
};

window.StartSnow = (canvas) => startSnow(canvas);