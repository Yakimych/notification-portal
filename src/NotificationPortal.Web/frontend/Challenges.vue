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
            v-for="challenge in orderedChallenges"
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
import { ChallengeApiApi, ChallengeStatus, Configuration } from "./api";
import { ChallengeModel } from "./api";
import { defineComponent } from "vue";
import ChallengeRow from "./ChallengeRow.vue";

type ConnectionStatus = "Unknown" | "Connected" | "Disconnected";

const Challenges = defineComponent({
  components: { ChallengeRow },
  mounted: function () {
    const challengeApi = new ChallengeApiApi(
      new Configuration({ basePath: "" })
    );
    challengeApi
      .apiChallengesGet()
      .then((response) => {
        console.log("Response data: ", response);
        this.challenges = response.challenges;

        this.isLoadingChallenges = false;
      })
      .catch((e) => {
        // TODO: GUI for error state
        console.log("Error loading challenges: ", e);
        this.isLoadingChallenges = false;
      });

    this.connection.onreconnecting(() => {
      this.connectionStatus = "Disconnected";
    });

    this.connection.onreconnected(() => {
      this.connectionStatus = "Connected";
    });

    this.connection.on("ChallengeStatusChanged", (challengeId, newStatus) => {
      console.log("ChallengeStatusChanged: ", challengeId, newStatus);
      const challengeToUpdate = this.challenges.find(
        (c) => c.id === challengeId
      );
      if (challengeToUpdate !== undefined) {
        challengeToUpdate.status = newStatus;
      }
    });

    this.connection.on(
      "NewChallengeIssued",
      (
        challengeId: number,
        communityName: string,
        fromPlayer: string,
        toPlayer: string,
        status: ChallengeStatus,
        date: string
      ) => {
        const challengeToAdd: ChallengeModel = {
          id: challengeId,
          communityName,
          fromPlayer,
          toPlayer,
          status,
          date: new Date(date),
        };

        this.challenges.push(challengeToAdd);
      }
    );

    this.connection
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
    connection: new signalR.HubConnectionBuilder()
      .withAutomaticReconnect()
      .withUrl("/challengehub")
      .build(),
    connectionStatus: "Unknown" as ConnectionStatus,
    challenges: [] as ChallengeModel[],
  }),
  methods: {
    acceptChallenge(challenge: ChallengeModel) {
      challenge.status = ChallengeStatus.Accepting;
      this.connection
        .invoke("AcceptChallenge", challenge.id)
        .catch(function (err) {
          // TODO: Handle error
          console.log("Error accepting challenge: ", err);
        });
    },
    declineChallenge(challenge: ChallengeModel) {
      challenge.status = ChallengeStatus.Declining;
      this.connection
        .invoke("DeclineChallenge", challenge.id)
        .catch(function (err) {
          // TODO: Handle error
          console.log("Error declining challenge: ", err);
        });
    },
  },
  computed: {
    orderedChallenges(): ChallengeModel[] {
      const byIdDesc = (c1: ChallengeModel, c2: ChallengeModel) => {
        if (c1.id < c2.id) {
          return 1;
        } else if (c1.id > c2.id) {
          return -1;
        } else {
          return 0;
        }
      };

      return [...this.challenges].sort(byIdDesc);
    },
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
