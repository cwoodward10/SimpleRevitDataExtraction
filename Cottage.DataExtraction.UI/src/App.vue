<script lang="ts">
import { defineComponent } from '@vue/runtime-core'
import { GetRevitDataFromDatabase } from './modules/api';
import { DatabaseModel } from './modules/models/DatabaseModel';
import { ElementModel } from './modules/models/ElementModel';
import DataViewer from './components/DataViewer.vue';

export default defineComponent({
  name: 'App',
  components: {
    DataViewer
},
  data: function() {
    return {
      walls: [] as ElementModel[],
      floors: [] as ElementModel[],
      plumbingFixtures: [] as ElementModel[],
      stepSize: 4,
    }
  },
  created: async function() {
    await this.resetData();
  },
  methods: {
    resetData: async function() {
      const databaseModel: DatabaseModel = await GetRevitDataFromDatabase();
      this.walls = databaseModel.walls;
      this.floors = databaseModel.floors;
      this.plumbingFixtures = databaseModel.plumbingFixtures;
    },
    updateStepSize: function(e: Event) {
      this.stepSize = Number.parseInt((e.target as HTMLInputElement).value);
    }
  }
})
</script>

<template>
  <header class="flex flex-col w-40 mx-auto my-3">
    <div class="flex flex-col">
      <label name="changeStep">Elements per Color</label>
      <div class="flex space-x-2">
        <input name="changeStep"
                type="range"
                min="1"
                max="10"
                value="stepSize"
                @change="updateStepSize" />
        <p>{{ stepSize }}</p>
      </div>
    </div>
    <button class="mx-auto w-16 border border-red-400 border-solid hover:bg-red-400 text-red-400 hover:text-white font-medium rounded hover:drop-shadow-lg" @click="resetData">Reset</button>
  </header>
  <main class="grid grid-cols-3 m-10 gap-4">
    <DataViewer :title="'Walls'" :inputData="walls" :stepSize="stepSize" />
    <DataViewer :title="'Floors'" :inputData="floors" :stepSize="stepSize" />
    <DataViewer :title="'Plumbing Fixtures'" :inputData="plumbingFixtures" :stepSize="stepSize" />
  </main>
</template>
