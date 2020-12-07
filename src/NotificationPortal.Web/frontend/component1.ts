import { createApp } from "vue";
import TestComponent from "./TestComponent.vue";

console.log("Loaded component1.ts");

createApp(TestComponent).mount("#vue_placeholder");
