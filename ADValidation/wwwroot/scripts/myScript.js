(function () {
    const getUsers = function (users, callback) {
        
    }
    
    const createWatermark = ({
                                 text,
                                 description,
                                 opacity = 0.2,
                                 fontSize = "1.4rem",
                                 angle = -45,
                                 color = "black",
                                 spacing = "100",
                                 scaling = 1
                             }) => {
        // Create the watermark container
        const watermarkContainer = document.createElement("div");
        watermarkContainer.style.position = "fixed";
        watermarkContainer.style.top = "0";
        watermarkContainer.style.left = "0";
        watermarkContainer.style.width = "100vw";
        watermarkContainer.style.height = "100vh";
        watermarkContainer.style.pointerEvents = "none";
        watermarkContainer.style.zIndex = "9999";
        watermarkContainer.style.opacity = opacity;
        watermarkContainer.style.display = "grid";
        watermarkContainer.style.overflow = "hidden";
        watermarkContainer.style.justifyItems = "center";
        watermarkContainer.style.alignItems = "center";

        const bodyElement = document.body;

        const updateWatermarks = () => {
            // Calculate watermark grid based on viewport size
            const textLength = text.length || 1;
            const spacingPx = (parseInt(spacing, 10) + textLength) * 1;
            const rows = Math.ceil(window.innerHeight / spacingPx);
            const cols = Math.ceil(window.innerWidth / spacingPx);

            watermarkContainer.style.gridTemplateColumns = `repeat(${cols}, 1fr)`;
            watermarkContainer.style.gridTemplateRows = `repeat(${rows}, 1fr)`;

            // Clear existing watermarks
            watermarkContainer.innerHTML = "";

            // Add watermarks
            for (let i = 0; i < rows * cols; i++) {
                const watermarkElement = document.createElement("div");
                watermarkElement.style.width = `${spacingPx}px`;
                watermarkElement.style.height = `${spacingPx}px`;
                watermarkElement.style.display = "flex";
                watermarkElement.style.flexDirection = "column";
                watermarkElement.style.alignItems = "center";
                watermarkElement.style.justifyContent = "center";
                watermarkElement.style.color = color;
                watermarkElement.style.fontSize = fontSize;
                watermarkElement.style.whiteSpace = "nowrap";
                watermarkElement.style.transform = `rotate(${angle}deg)`;

                const textNode = document.createElement("span");
                textNode.textContent = text;

                const descNode = document.createElement("span");
                descNode.textContent = description;
                descNode.style.fontSize = `calc(${fontSize} / 1.5)`;

                watermarkElement.appendChild(textNode);
                watermarkElement.appendChild(descNode);

                watermarkContainer.appendChild(watermarkElement);
            }
        };

        // Append watermark container to body
        bodyElement.appendChild(watermarkContainer);

        // Initialize and update on resize
        updateWatermarks();
        window.addEventListener("resize", updateWatermarks);

        // Cleanup function
        return () => {
            window.removeEventListener("resize", updateWatermarks);
            watermarkContainer.remove();
        };
    };

    // Usage example
    createWatermark({
        text: "Watermark",
        description: "Confidential",
        opacity: 0.2,
        fontSize: "2rem",
        angle: -45,
        color: "gray",
        spacing: "250",
        scaling: 1.2
    });
})();
