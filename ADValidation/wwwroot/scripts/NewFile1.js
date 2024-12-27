document.addEventListener("DOMContentLoaded", function() {
    // Create and append the CSS to the head of the document
    const style = document.createElement("style");
    style.innerHTML = `
        body {
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 0;
            height: 100vh;
            overflow: hidden;
            background-color: #f0f0f0;
        }

        .watermark-container {
            position: relative;
            width: 100%;
            height: 100%;
        }

        .watermark {
            position: absolute;
            top: 0;
            left: 0;
            pointer-events: none;
            z-index: 999;
            display: grid;
            justify-items: center;
            align-items: center;
            opacity: 0.1;
            transform: rotate(-45deg);
            color: gray;
            font-size: 2rem;
        }

        .watermark-text {
            display: block;
            white-space: nowrap;
        }
    `;
    document.head.appendChild(style);

    // Create and append the watermark container to the body
    const container = document.createElement("div");
    container.classList.add("watermark-container");

    // Define watermark options (these can be customized)
    const watermarkOptions = {
        text: 'Watermark Text',
        opacity: 0.1,
        fontSize: '2rem',
        angle: -45,
        color: 'gray',
        spacing: '200px'
    };

    // Function to create watermark
    function createWatermark(options) {
        const { text, opacity = 0.1, fontSize = "1.5rem", angle = -45, color = "gray", spacing = "250px" } = options;

        const watermarkLayer = document.createElement('div');
        watermarkLayer.classList.add('watermark');
        watermarkLayer.style.opacity = opacity;
        watermarkLayer.style.transform = `rotate(${angle}deg)`;
        watermarkLayer.style.fontSize = fontSize;
        watermarkLayer.style.color = color;

        const containerWidth = window.innerWidth;
        const containerHeight = window.innerHeight;
        const spacingPx = parseInt(spacing, 10);

        const rows = Math.ceil(containerHeight / spacingPx);
        const cols = Math.ceil(containerWidth / spacingPx);

        // Create watermark text divs
        for (let i = 0; i < rows * cols; i++) {
            const watermarkText = document.createElement('div');
            watermarkText.classList.add('watermark-text');
            watermarkText.style.margin = `-${spacing}`;
            watermarkText.style.width = spacing;
            watermarkText.style.height = spacing;
            watermarkText.style.display = 'flex';
            watermarkText.style.alignItems = 'center';
            watermarkText.style.justifyContent = 'center';
            watermarkText.textContent = text;
            watermarkLayer.appendChild(watermarkText);
        }

        // Append watermark layer to the container
        container.appendChild(watermarkLayer);
    }

    // Initialize watermark after page load
    createWatermark(watermarkOptions);

    // Append the watermark container to the body
    document.body.appendChild(container);
});
