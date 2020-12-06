<template>
  <div>
    <h1>Child component</h1>
    <span>Some prop: {{ someProp }}</span>
    <div>
      <button v-on:click="decrement">-</button>
      <span>Counter: {{ counter }}</span>
      <input type="text" v-model="counter" />
      <button v-on:click="increment">+</button>
    </div>
  </div>
</template>

<script lang="ts">
import Vue from "vue";

const ChildComponent = Vue.extend({
  props: {
    someProp: String,
  },
  data: function () {
    return { counter: 1 };
  },
  methods: {
    increment: function () {
      this.counter++;
    },
    decrement: function () {
      this.counter--;
    },
  },
});

export default ChildComponent;
</script> 
