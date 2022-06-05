<script lang="ts">
import { defineComponent } from '@vue/runtime-core'
import { GetRevitDataFromDatabase } from './modules/api';
import HelloWorld from './components/HelloWorld.vue'
import { DatabaseModel } from './modules/models/DatabaseModel';
import { ElementModel } from './modules/models/ElementModel';

export default defineComponent({
  name: 'App',
  components: {
    HelloWorld,
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
    this.walls = databaseModel.walls;
    this.floors = databaseModel.floors;
    this.plumbingFixtures = databaseModel.plumbingFixtures;
  },
})
</script>

<template>
  <HelloWorld msg="Hello Vue 3 + TypeScript + Vite" />
</template>
