import Vue from "vue";

console.log("Loaded component1.ts");

const TestComponent = Vue.component("test-component", {
  template: "<div>Vue-rendered content</div>",
});

var app = new Vue({
  el: "#vue_placeholder",
  render: (h) => h(TestComponent),
});
