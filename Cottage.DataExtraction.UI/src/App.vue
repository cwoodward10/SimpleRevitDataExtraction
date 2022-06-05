<script lang="ts">
import { defineComponent } from '@vue/runtime-core'
import { GetRevitDataFromDatabase } from './modules/api';
import HelloWorld from './components/HelloWorld.vue'
import { DatabaseModel } from './modules/models/DatabaseModel';
import { ElementModel } from './modules/models/ElementModel';
import DataViewer from './components/DataViewer.vue';

export default defineComponent({
  name: 'App',
  components: {
    HelloWorld,
    DataViewer
},
  data: function() {
    return {
      walls: [] as ElementModel[],
      floors: [] as ElementModel[],
      plumbingFixtures: [] as ElementModel[],
    }
  },
  created: async function() {
    const databaseModel: DatabaseModel = await GetRevitDataFromDatabase();
    console.log(databaseModel);
    this.walls = databaseModel.walls;
    this.floors = databaseModel.floors;
    this.plumbingFixtures = databaseModel.plumbingFixtures;
  },
})
</script>

<template>
  <main class="grid grid-cols-3 m-10 gap-4">
    <DataViewer :title="'Walls'" :data="walls" />
    <DataViewer :title="'Floors'" :data="floors" />
    <DataViewer :title="'Plumbing Fixtures'" :data="plumbingFixtures" />
  </main>
</template>
