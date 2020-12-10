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
    <td class="date-cell">{{ formattedDate }}</td>
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
import { computed, defineComponent, PropType } from "vue";

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
  setup(props) {
    const challengeStatusClass = computed(() => {
      switch (props.challenge.type) {
        case "Accepted":
          return "accepted";
        case "Declined":
          return "declined";
        default:
          return "";
      }
    });

    const showSpinner = computed(() => {
      const statesToShowSpinnerFor: ChallengeStatus[] = [
        "Challenging",
        "Accepting",
        "Declining",
      ];
      return statesToShowSpinnerFor.includes(props.challenge.type);
    });

    const showButtons = computed(() => {
      const statesToShowButtonsFor: ChallengeStatus[] = [
        "Challenged",
        "Challenging",
      ];
      return statesToShowButtonsFor.includes(props.challenge.type);
    });

    const canRespondToChallenges = computed(() => {
      return props.isEnabled && props.challenge.type === "Challenged";
    });

    const formattedDate = computed(() => {
      return props.challenge.date.toLocaleDateString();
    });

    return {
      formattedDate,
      challengeStatusClass,
      showSpinner,
      showButtons,
      canRespondToChallenges,
    };
  },
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
});
</script> 
