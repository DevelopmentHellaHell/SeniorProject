import { Chart as ChartJS, ChartData, registerables } from 'chart.js';
import React, { useEffect, useState } from 'react';
import { Line } from 'react-chartjs-2';
import { Ajax } from '../../../Ajax';
import Footer from '../../../components/Footer/Footer';
import Loading from '../../../components/Loading/Loading';
import NavbarUser from '../../../components/NavbarUser/NavbarUser';
import './Analytics.css';

const REFRESH_CHARTS_INTERVAL_MILLISECONDS = 60000;

interface IAnalyticsProps {

}

interface IAnalyticsData {
    [name: string]: {
        [date: string]: number,
    },
}

const getColor = (theme: string) => {
    return getComputedStyle(document.documentElement).getPropertyValue(theme);
}

ChartJS.register(...registerables);
ChartJS.defaults.color = getColor("--secondary-background-dark");
ChartJS.defaults.borderColor = getColor("--secondary-background-dark");

const Analytics: React.FC<IAnalyticsProps> = (props) => {
    const [data, setData] = useState<IAnalyticsData | null>(null);
    const [error, setError] = useState("");
    const [loaded, setLoaded] = useState(false);

    useEffect(() => {
        const getData = async () => {
            const response = await Ajax.get<IAnalyticsData>("/analytics/data");
            setData(response.data);
            setError(response.error);
            setLoaded(response.loaded);
        }

        getData();

        const interval = setInterval(() => {
            getData();
         }, REFRESH_CHARTS_INTERVAL_MILLISECONDS);

        return (() => { clearInterval(interval) });
    }, []);
    
    return (
        <div className="analytics-wrapper">
            <h1>Analytics</h1>
            
            <div className="chart-card">
                {error && 
                    <p>{error}</p>
                }
                {!loaded && !error &&
                    <Loading title="Loading chart data..."/>
                }
                {loaded && !error && data && Object.keys(data).map(key => {
                    const chartData: ChartData<"line"> = {
                        labels: [],
                        datasets: [{
                            data: [],
                            borderColor: getColor("--primary-background-dark")
                        }],
                    };
                    const indexed = data[key];
                    Object.keys(indexed).forEach(date => {
                        const count = indexed[date];
                        chartData.labels!.push(date);
                        chartData.datasets[0]!.data.push(count);
                    });

                    return (
                        <div className="chart-box" key={`chart-${key}`}>
                            <h2>{key.charAt(0).toUpperCase() + key.slice(1)}</h2>
                            <Line
                                data={chartData}
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
                })}
            </div>
        </div>
    );
}

export default Analytics;