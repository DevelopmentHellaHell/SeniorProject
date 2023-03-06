import { Chart as ChartJS, ChartData, registerables } from 'chart.js';
import React, { useEffect, useState } from 'react';
import { Line } from 'react-chartjs-2';
import './ChartCard.css';

interface IChartCardProps {
    name: string;
    data: ChartData<"line">;
}

const getColor = (theme: string) => {
    return getComputedStyle(document.documentElement).getPropertyValue(theme);
}


const ChartCard: React.FC<IChartCardProps> = (props) => {
    const [mounted, setMounted] = useState(false);

    if (!mounted) {
        ChartJS.defaults.color = `rgb(${getColor("--primary-background-dark")})`;
        ChartJS.defaults.borderColor = `rgb(${getColor("--primary-background-dark")})`;
        ChartJS.register(...registerables);
    }

    useEffect(() => {
        setMounted(true);
    }, []);

    return (
        <div className="chart-box">
            <h2>{props.name.charAt(0).toUpperCase() + props.name.slice(1)}</h2>
            <Line
                data={props.data}
                options={{
                    scales: {
                        x: {
                            title: {
                                display: true,
                                text: "DATE",
                            },  
                            suggestedMin: 0,
                            ticks: {
                                maxTicksLimit: 20,
                            },
                        },
                        y: {
                            title: {
                                display: true,
                                text: "COUNT",
                            },  
                            suggestedMin: 0,
                            ticks: {
                                stepSize: 1,
                                maxTicksLimit: 20,
                            }
                        },
                    },
                    plugins: {
                        legend: {
                            display: false,
                        },
                    },
                }}
            />
        </div>
    );
}

export default ChartCard;