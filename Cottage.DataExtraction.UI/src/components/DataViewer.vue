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
        data: {
            type: Object as PropType<ElementModel[]>,
            required: true
        }
    },
    data: function() {
        return {
            chart: null as null | Chart,
        }
    },
    computed: {
        totals: function(): number {
            return this.data.reduce((total: number, model: ElementModel) => {
                return total + model.value;
            }, 0).toFixed(2);
        },
        chartData: function() {
            const labels = this.data.map(el => el.name);
            const values = this.data.map(el => el.value);

            return {
                labels: labels,
                datasets: [{
                    label: this.title,
                    borderColor: 'rgb(255, 99, 132)',
                    data: values,
                }]
            }
        },
    },
    watch: {
        data: function() {
            if (this.chart != null) {
                this.chart.destroy();
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
            this.chart = new Chart(
                chartCanvas,
                {
                    type: 'doughnut',
                    data: this.chartData,
                    options: {}
                }
            ) as Chart;
            console.log(this.chart);
        },
    }
})
</script>

<template>
    <article class="w-full h-auto px-2 py-4 rounded-md border-2 border-solid border-pink-800 drop-shadow-lg  bg-gray-200">
        <div>
            <header class="w-full flex flex-col mb-2">
                <h1 class="flex mx-auto text-lg font-bold">{{ title }}</h1>
                <h1 class="flex mx-auto text-3xl">total: {{ totals }}</h1>
            </header>
            <canvas ref="chart" class="w-full"></canvas>
        </div>
    </article>
</template>