import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import App from './App';
import Analytics from './pages/Analytics/Analytics';

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);

root.render(
  <React.StrictMode>
    <Analytics />
  </React.StrictMode>
);