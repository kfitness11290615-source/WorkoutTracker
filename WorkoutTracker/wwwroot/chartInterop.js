window.renderProgressChart = (canvasId, labels, data, tooltips, minY, maxY) => {
    if (typeof Chart === 'undefined') {
        console.error('Chart.js is not loaded yet.');
        return;
    }
    if (window.progressChart) {
        window.progressChart.destroy();
    }
    const canvas = document.getElementById(canvasId);
    if (!canvas) {
        console.error('Canvas not found:', canvasId);
        return;
    }
    const ctx = canvas.getContext('2d');
    window.progressChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: '推定1RM (e1RM)',
                data: data,
                borderColor: '#4CAF50',
                backgroundColor: 'rgba(76, 175, 80, 0.2)',
                borderWidth: 2,
                pointRadius: 4,
                pointHoverRadius: 6,
                fill: true,
                tension: 0.1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                x: {
                    ticks: { color: '#888' },
                    grid: { color: '#333' }
                },
                y: {
                    min: minY,
                    max: maxY,
                    ticks: { color: '#888' },
                    grid: { color: '#333' }
                }
            },
            plugins: {
                legend: {
                    labels: { color: '#ccc' }
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            let label = context.dataset.label || '';
                            if (label) {
                                label += ': ';
                            }
                            if (context.parsed.y !== null) {
                                label += context.parsed.y.toFixed(1) + ' kg';
                            }
                            return label;
                        },
                        afterLabel: function(context) {
                            return tooltips[context.dataIndex];
                        }
                    }
                }
            }
        }
    });
};
