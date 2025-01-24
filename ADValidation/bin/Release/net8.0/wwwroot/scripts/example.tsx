import React, { useEffect, useRef, useState } from "react";

type WatermarkOptions = {
    text: string; // The watermark text
    description: string;
    opacity?: number; // Watermark opacity (default: 0.1)
    fontSize?: string; // Watermark font size (default: '2rem')
    angle?: number; // Angle of the watermark in degrees (default: -45)
    color?: string; // Watermark text color (default: 'gray')
    spacing?: string; // Spacing between repeated watermarks (default: '200px')
    scaling?: number;
};

type WatermarkProps = {
    children: React.ReactNode; // Content inside the watermark wrapper
    options: WatermarkOptions; // Watermark options
};

const Watermark: React.FC<WatermarkProps> = ({ children, options }) => {
    const {
        text,
        description,
        opacity = 0.2,
        fontSize = "1.4rem",
        angle = -45,
        color = "black",
        spacing = "200",
    } = options;

    const [watermarkCount, setWatermarkCount] = useState({ rows: 0, cols: 0 });

    useEffect(() => {
        const calculateWatermarkCount = () => {
            const textLength = text?.length || 1;
            const { innerWidth: width, innerHeight: height } = window;

            const spacingPx = (parseInt(spacing, 10) + textLength) * 1.5;
            const rows = Math.ceil(height / spacingPx);
            const cols = Math.ceil(width / spacingPx);
            setWatermarkCount({ rows, cols });
        };

        calculateWatermarkCount();
        window.addEventListener("resize", calculateWatermarkCount);

        return () => {
            window.removeEventListener("resize", calculateWatermarkCount);
        };
    }, [spacing]);

    return (
            <div
                className="fixed inset-0 flex flex-wrap pointer-events-none"
                style={{
                    zIndex: 10,
                    opacity,
                    color,
                    fontSize,
                    display: "grid",
                    gridTemplateColumns: `repeat(${watermarkCount.cols}, 1fr)`,
                    gridTemplateRows: `repeat(${watermarkCount.rows}, 1cfr)`,
                    justifyItems: "center",
                    alignItems: "center",
                }}
            >
                {Array.from({length: watermarkCount.rows * watermarkCount.cols}).map((_, idx) => (
                    <div
                        key={idx}
                        className="text-center"
                        style={{
                            gap: spacing,
                            color: color,
                            width: spacing,
                            height: spacing,
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "center",
                            whiteSpace: "nowrap",
                            transform: `rotate(${angle}deg)`,
                        }}
                    >
                        <span className="flex flex-col">
                            {text}
                            <span style={{fontSize: `calc(${fontSize} / 1.5)`}}>{description}</span>
                        </span>
                    </div>
                ))}
            </div>
    );
};

export default Watermark;
