<template>
  <tr>
    <td>{{ challenge.id }}</td>
    <td>{{ challenge.communityName }}</td>
    <td>{{ challenge.fromPlayer }}</td>
    <td>{{ challenge.toPlayer }}</td>
    <td :class="['challenge-status-cell', challengeStatusClass]">
      <div v-if="showSpinner" class="loader" />
      <div v-else>{{ challenge.type }}</div>
    </td>
    <!-- <td class="date-cell">{{ formattedDate }}</td> -->
    <td class="date-cell">TODO</td>
    <td>
      <div v-if="showButtons">
        <button
          class="accept-challenge-button"
          :disabled="!canRespondToChallenges"
          @click="emitAccept"
        >
          Accept
        </button>
        <button
          class="decline-challenge-button"
          :disabled="!canRespondToChallenges"
          @click="emitDecline"
        >
          Decline
        </button>
      </div>
    </td>
  </tr>
</template>

<script lang="ts">
import * as signalR from "@microsoft/signalr";
import { defineComponent, PropType } from "vue";

export type ChallengeStatus =
  | "Challenging"
  | "Challenged"
  | "Accepting"
  | "Accepted"
  | "Declining"
  | "Declined";

export type Challenge = {
  id: number;
  communityName: string;
  fromPlayer: string;
  toPlayer: string;
  type: ChallengeStatus;
  date: Date;
};

export default defineComponent({
  props: {
    isEnabled: { type: Boolean, required: true },
    challenge: { type: Object as PropType<Challenge>, required: true },
  },
  mounted: function () {},
  data: () => ({
    someText: "Some text",
  }),
  methods: {
    emitAccept() {
      if (this.canRespondToChallenges) {
        this.$emit("accept-challenge", this.challenge);
      }
    },
    emitDecline() {
      if (this.canRespondToChallenges) {
        this.$emit("decline-challenge", this.challenge);
      }
    },
  },
  computed: {
    challengeStatusClass() {
      switch (this.challenge.type) {
        case "Accepted":
          return "accepted";
        case "Declined":
          return "declined";
        default:
          return "";
      }
    },
    showSpinner(): boolean {
      const statesToShowSpinnerFor: ChallengeStatus[] = [
        "Challenging",
        "Accepting",
        "Declining",
      ];
      return statesToShowSpinnerFor.includes(this.challenge.type);
    },
    showButtons(): boolean {
      const statesToShowButtonsFor: ChallengeStatus[] = [
        "Challenged",
        "Challenging",
      ];
      return statesToShowButtonsFor.includes(this.challenge.type);
    },
    formattedDate(): string {
      return this.challenge.date.toLocaleDateString();
    },
    canRespondToChallenges(): boolean {
      return this.isEnabled && this.challenge.type === "Challenged";
    },
  },
});
</script> 
