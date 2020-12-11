<template>
  <tr>
    <td>{{ challenge.id }}</td>
    <td>{{ challenge.communityName }}</td>
    <td>{{ challenge.fromPlayer }}</td>
    <td>{{ challenge.toPlayer }}</td>
    <td :class="['challenge-status-cell', challengeStatusClass]">
      <div v-if="showSpinner" class="loader" />
      <div v-else>{{ challenge.status }}</div>
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
import { ChallengeModel, ChallengeStatus } from "./api/models";

export default defineComponent({
  props: {
    isEnabled: { type: Boolean, required: true },
    challenge: { type: Object as PropType<ChallengeModel>, required: true },
  },
  setup(props) {
    const challengeStatusClass = computed(() => {
      switch (props.challenge.status) {
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
        ChallengeStatus.Challenging,
        ChallengeStatus.Accepting,
        ChallengeStatus.Declining,
      ];
      return statesToShowSpinnerFor.includes(props.challenge.status);
    });

    const showButtons = computed(() => {
      const statesToShowButtonsFor: ChallengeStatus[] = [
        ChallengeStatus.Challenged,
        ChallengeStatus.Challenging,
      ];
      return statesToShowButtonsFor.includes(props.challenge.status);
    });

    const canRespondToChallenges = computed(() => {
      return (
        props.isEnabled && props.challenge.status === ChallengeStatus.Challenged
      );
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
