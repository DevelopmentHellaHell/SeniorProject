import { Chart as ChartJS, ChartData, registerables } from 'chart.js';
import React, { useEffect, useState } from 'react';
import { Line } from 'react-chartjs-2';
import { Ajax } from '../../../Ajax';
import Loading from '../../../components/Loading/Loading';
import './AnalyticsView.css';
import ChartCard from './ChartCard/ChartCard';

const REFRESH_CHARTS_INTERVAL_MILLISECONDS = 60000;

interface IAnalyticsViewProps {

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

const AnalyticsView: React.FC<IAnalyticsViewProps> = (props) => {
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
                            borderColor: `rgb(${getColor("--primary-background-dark")})`,
                            backgroundColor: `rgb(${getColor("--white")})`
                        }],
                    };
                    const indexed = data[key];
                    Object.keys(indexed).forEach(date => {
                        const count = indexed[date];
                        chartData.labels!.push(date);
                        chartData.datasets[0]!.data.push(count);
                    });

                    return (
                        <ChartCard key={`chart-card-${key}`} name={key} data={chartData} />
                    );
                })}
            </div>
        </div>
    );
}

export default AnalyticsView;