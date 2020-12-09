<template>
  <div>
    <h1 class="display-4">Challenges</h1>
    <div id="notification_list">
      <table class="table">
        <thead>
          <th scope="col">Id</th>
          <th scope="col">Community</th>
          <th scope="col">From</th>
          <th scope="col">To</th>
          <th scope="col">Type</th>
          <th scope="col">Date</th>
          <th scope="col" style="width: 150px">Action</th>
        </thead>

        <tbody id="challengeListTableBody">
          <div v-if="isLoadingChallenges">Loading...</div>
          <challenge-row
            v-else
            v-for="challenge in challenges"
            :key="challenge.id"
            :challenge="challenge"
            :isEnabled="canRespondToChallenges"
            @accept-challenge="acceptChallenge"
            @decline-challenge="declineChallenge"
          />
        </tbody>
      </table>
    </div>
    <div
      v-bind:class="['connection-status-indicator', connectionStatusBarClass]"
    ></div>
  </div>
</template>

<script lang="ts">
import * as signalR from "@microsoft/signalr";
import axios from "axios";
import { defineComponent } from "vue";
import ChallengeRow, { Challenge } from "./ChallengeRow.vue";

type ConnectionStatus = "Unknown" | "Connected" | "Disconnected";

const fakeChallenges: Challenge[] = [
  {
    id: 1,
    communityName: "TestCommunity",
    fromPlayer: "P11",
    toPlayer: "P12",
    type: "Challenging",
    date: new Date(),
  },
  {
    id: 2,
    communityName: "TestCommunity",
    fromPlayer: "P1",
    toPlayer: "P2",
    type: "Challenged",
    date: new Date(),
  },
  {
    id: 3,
    communityName: "TestCommunity",
    fromPlayer: "P3",
    toPlayer: "P4",
    type: "Accepting",
    date: new Date(),
  },
  {
    id: 4,
    communityName: "TestCommunity",
    fromPlayer: "P5",
    toPlayer: "P6",
    type: "Accepted",
    date: new Date(),
  },
  {
    id: 5,
    communityName: "TestCommunity",
    fromPlayer: "P7",
    toPlayer: "P8",
    type: "Declining",
    date: new Date(),
  },
  {
    id: 6,
    communityName: "TestCommunity",
    fromPlayer: "P9",
    toPlayer: "P10",
    type: "Declined",
    date: new Date(),
  },
];

const Challenges = defineComponent({
  components: { ChallengeRow },
  mounted: function () {
    // TODO: Fetch challenges from /api/challenges
    axios
      .get("/api/challenges")
      .then((response) => {
        console.log("Response data: ", response.data);
        // TODO: Decode/parse challenges (including date strings)
        this.challenges = response.data;
        this.isLoadingChallenges = false;
      })
      .catch((e) => {
        // TODO: GUI for error state
        console.log("Error loading challenges: ", e);
        this.isLoadingChallenges = false;
      });

    const connection = new signalR.HubConnectionBuilder()
      .withAutomaticReconnect()
      .withUrl("/challengehub")
      .build();

    connection.onreconnecting(() => {
      this.connectionStatus = "Disconnected";
    });

    connection.onreconnected(() => {
      this.connectionStatus = "Connected";
    });

    connection
      .start()
      .then(() => {
        this.connectionStatus = "Connected";
      })
      .catch(function (err) {
        return console.error(err.toString());
      });
  },
  data: () => ({
    // TODO: DU for loading states
    isLoadingChallenges: true,
    connectionStatus: "Unknown" as ConnectionStatus,
    challenges: fakeChallenges,
  }),
  methods: {
    acceptChallenge(challenge: Challenge) {
      challenge.type = "Accepting";
    },
    declineChallenge(challenge: Challenge) {
      challenge.type = "Declining";
    },
  },
  computed: {
    connectionStatusBarClass() {
      switch (this.connectionStatus) {
        case "Connected":
          return "connected";
        case "Disconnected":
          return "disconnected";
        case "Unknown":
        default:
          return "unknown";
      }
    },
    canRespondToChallenges(): boolean {
      return this.connectionStatus === "Connected";
    },
  },
});

export default Challenges;
</script>
