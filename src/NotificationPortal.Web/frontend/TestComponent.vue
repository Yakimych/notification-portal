<template>
  <div>
    <h1>TestComponent from .vue file</h1>
    <span>Some text: {{ someText }}</span>
    <child-component someProp="qwe" />
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import ChildComponent from "./ChildComponent.vue";

const TestComponent = defineComponent({
  components: { ChildComponent },
  data: function () {
    return { someText: "Some text" };
  },
});

export default TestComponent;
</script> 
