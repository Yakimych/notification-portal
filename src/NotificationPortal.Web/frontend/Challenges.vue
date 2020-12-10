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
import { ChallengeApiApi } from "./api/apis/ChallengeApiApi";
import { ChallengeModel } from "./api/models/ChallengeModel";
import { defineComponent } from "vue";
import ChallengeRow, { Challenge, ChallengeStatus } from "./ChallengeRow.vue";

type ConnectionStatus = "Unknown" | "Connected" | "Disconnected";

const toChallengeType = (typeString: string): ChallengeStatus => {
  switch (typeString) {
    case "Challenging":
    case "Challenged":
    case "Accepting":
    case "Accepted":
    case "Declining":
    case "Declined":
      return typeString;
    default:
      throw Error(`Challenge type: ${typeString} does not exist`);
  }
};

const toChallenge = (challengeModel: ChallengeModel): Challenge => {
  return {
    ...challengeModel,
    type: toChallengeType(challengeModel.type),
  };
};

const Challenges = defineComponent({
  components: { ChallengeRow },
  mounted: function () {
    const challengeApi = new ChallengeApiApi();
    challengeApi
      .apiChallengesGet()
      .then((response) => {
        console.log("Response data: ", response);
        this.challenges = response.challenges.map(toChallenge);

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
        challengeToUpdate.type = newStatus;
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
        const challengeToAdd: Challenge = {
          id: challengeId,
          communityName,
          fromPlayer,
          toPlayer,
          type: status,
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
    challenges: [] as Challenge[],
  }),
  methods: {
    acceptChallenge(challenge: Challenge) {
      challenge.type = "Accepting";
      this.connection
        .invoke("AcceptChallenge", challenge.id)
        .catch(function (err) {
          // TODO: Handle error
          console.log("Error accepting challenge: ", err);
        });
    },
    declineChallenge(challenge: Challenge) {
      challenge.type = "Declining";
      this.connection
        .invoke("DeclineChallenge", challenge.id)
        .catch(function (err) {
          // TODO: Handle error
          console.log("Error declining challenge: ", err);
        });
    },
  },
  computed: {
    orderedChallenges(): Challenge[] {
      const byIdDesc = (c1: Challenge, c2: Challenge) => {
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
