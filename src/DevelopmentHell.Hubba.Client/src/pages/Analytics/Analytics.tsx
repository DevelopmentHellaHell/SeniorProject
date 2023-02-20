import { Chart as ChartJS, ChartData, registerables } from 'chart.js';
import React, { useEffect, useState } from 'react';
import { Line } from 'react-chartjs-2';
import { Ajax } from '../../Ajax';
import Loading from '../../components/Loading/Loading';
import NavbarUser from '../../components/NavbarUser/NavbarUser';
import './Analytics.css';

interface Props {

}

interface AnalyticsData {
  [name: string]: {
    [date: string]: number,
  },
}

ChartJS.register(...registerables);
ChartJS.defaults.color = "rgb(240, 255, 255)";
ChartJS.defaults.borderColor = "rgb(40, 40, 40)";

const DUMMY_DATA: AnalyticsData = {
  "logins": {
    "2/10": 5,
    "2/11": 6,
    "2/12": 8,
  },
  "registrations": {
    "2/10": 7,
    "2/11": 3,
    "2/12": 15,
  },
  "listings": {
    "2/10": 1,
    "2/11": 3,
  },
  "bookings": {
    "2/10": 1,
    "2/12": 2,
  }
}

const Analytics: React.FC<Props> = (props) => {
  const [data, setData] = useState(null);
  const [error, setError] = useState("");
  const [loaded, setLoaded] = useState(false);

  useEffect(() => {
    async function getData() {
      const response = await Ajax.get<any>("https://jsonplaceholder.typicode.com/users"); //any type temp
      setData(response.data);
      setError(response.error);
      setLoaded(response.loaded);
    }

    const interval = setInterval(() => {
      getData();
    }, 1000);

    return (() => { clearInterval(interval) });
  }, []);

  return (
    <div className="wrapper">
      <NavbarUser />
      <h1>Analytics</h1>
      
      <div className="chart-card">
        {error && 
          <p>{error}</p>
        }
        {!loaded && !error &&
          <Loading title="Loading chart data..."/>
        }
        {loaded && !error && data && Object.keys(DUMMY_DATA).map(key => {
          const chartData: ChartData<"line"> = {
            labels: [],
            datasets: [{
              data: [],
            }],
          };
          const indexed = DUMMY_DATA[key];
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
