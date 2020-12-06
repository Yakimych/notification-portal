import Vue from "vue";
import TestComponent from "./TestComponent.vue";

console.log("Loaded component1.ts");

new Vue({
  el: "#vue_placeholder",
  render: (h) => h(TestComponent),
});
