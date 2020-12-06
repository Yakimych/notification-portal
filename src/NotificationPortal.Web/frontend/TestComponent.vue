<template>
  <div>
    <h1>TestComponent from .vue file</h1>
    <span>{{ someText }}</span>
  </div>
</template>

<script lang="ts">
import Vue from "vue";

const TestComponent = Vue.extend({
  data: function () {
    return { someText: "Some text" };
  },
});

export default TestComponent;
</script> 

