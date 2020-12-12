export function sum(a: number, b: number) {
  return a + b;
}

import "./css/main.css";

const a = 678;
const b = 3;
const resultText = `${a} + ${b} is ${sum(a, b)}`;

console.log("Loaded index.ts");
