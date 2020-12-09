<template>
  <tr v-if="challenge">
    <td>{{ challenge.id }}</td>
    <td>{{ challenge.communityName }}</td>
    <td>{{ challenge.fromPlayer }}</td>
    <td>{{ challenge.toPlayer }}</td>
    <td :class="['challenge-status-cell', challengeStatusClass]">
      <div v-if="showSpinner" class="loader" />
      <div v-else>{{ challenge.type }}</div>
    </td>
    <td class="date-cell">{{ formattedDate }}</td>
    <td>
      <div v-if="showButtons">
        <button
          class="accept-challenge-button"
          :disabled="!canRespondToChallenges"
          data-id="@challenge.Id"
        >
          Accept
        </button>
        <button
          class="decline-challenge-button"
          :disabled="!canRespondToChallenges"
          data-id="@challenge.Id"
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
    challenge: Object as PropType<Challenge>,
  },
  mounted: function () {},
  data: () => ({
    someText: "Some text",
  }),
  computed: {
    challengeStatusClass() {
      if (this.challenge !== undefined) {
        switch (this.challenge.type) {
          case "Accepted":
            return "accepted";
          case "Declined":
            return "declined";
          default:
            return "";
        }
      }
      return "";
    },
    showSpinner(): boolean {
      const statesToShowSpinnerFor: ChallengeStatus[] = [
        "Challenging",
        "Accepting",
        "Declining",
      ];
      return (
        this.challenge !== undefined &&
        statesToShowSpinnerFor.includes(this.challenge.type)
      );
    },
    showButtons(): boolean {
      const statesToShowButtonsFor: ChallengeStatus[] = [
        "Challenged",
        "Challenging",
      ];
      return (
        this.challenge !== undefined &&
        statesToShowButtonsFor.includes(this.challenge.type)
      );
    },
    formattedDate(): string {
      return this.challenge ? this.challenge.date.toLocaleDateString() : "";
    },
    canRespondToChallenges(): boolean {
      return true; // TODO: Combine information about being connected to the Hub, with the challenge status
    },
  },
});
</script> 
