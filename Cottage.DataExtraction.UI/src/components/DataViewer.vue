<script lang="ts">
import { defineComponent, PropType } from 'vue';
import { ElementModel } from '../modules/models/ElementModel';
import { Chart, DoughnutController, registerables } from 'chart.js';

export default defineComponent({
    name: 'DataViewer',
    props: {
        title: {
            type: String,
            required: true,
        },
        inputData: {
            type: Object as PropType<ElementModel[]>,
            required: true
        },
        stepSize: {
            type: Number,
            required: true,
        },
        units: {
            type: String,
            required: false,
            default: ""
        }
    },
    data: function() {
        return {}
    },
    computed: {
        chartId: function(): string {
            return `chart-${this.title.replaceAll(' ', '-')}`
        },
        totals: function(): string {
            return this.inputData.reduce((total: number, model: ElementModel) => {
                return total + model.value;
            }, 0).toFixed(2);
        },
        chartColors: function() {
            const safeLength = this.inputData.length > 0 ? this.inputData.length : this.stepSize;
            const steps =  safeLength / this.stepSize;

            return this.inputData.map((d: ElementModel, i: number) => {
                const currentIndex = i + 1;
                const hueSteps = (360 / steps) > 100 ? 100 : (360 / steps);
                
                const hue = hueSteps * Math.ceil(currentIndex / this.stepSize);
                const lightness = 10 + ((80 /this.stepSize) * ((currentIndex % this.stepSize) + 1)); // we want this to be between 10 and 90

                return `hsl(${hue}deg, 100%, ${lightness}%)`
            })
        },
        chartData: function() {
            const labels = this.inputData.map(el => el.name);
            const values = this.inputData.map(el => el.value);

            return {
                labels: labels,
                datasets: [{
                    label: this.title,
                    borderColor: this.chartColors,
                    backgroundColor: this.chartColors,
                    data: values,
                }]
            }
        },
    },
    watch: {
        inputData: function() {
            const chart = Chart.getChart(this.chartId);
            if (chart != null) {
                chart.destroy();
                this.showChart();
            }
        },
        stepSize: function() {
            const chart = Chart.getChart(this.chartId);
            if (chart != null) {
                chart.destroy();
                this.showChart();
            }
        }
    },
    created: function() {
        Chart.register(...registerables);
    },
    mounted: function() {
        this.showChart();
    },
    methods: {
        showChart: function() {
            const chartCanvas = this.$refs.chart as HTMLCanvasElement;
            const chart = new Chart(
                chartCanvas,
                {
                    type: 'doughnut',
                    data: this.chartData,
                    options: {}
                }
            );
             console.log('new chart created!', chart);
        }
    }
})
</script>

<template>
    <article class="w-full h-auto px-2 py-4 rounded-md border-2 border-solid border-gray-700 drop-shadow-lg  bg-white">
        <div>
            <header class="w-full flex flex-col mb-2">
                <h1 class="max-w-full flex mx-auto text-3xl font-bold truncate">{{ title }}</h1>
                <h1 class="flex mx-auto text-lg">total: {{ totals }} {{units}}</h1>
            </header>
            <canvas :id="chartId" ref="chart" class="w-full"></canvas>
        </div>
    </article>
</template>