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
import { defineComponent, ref, onMounted, Ref, computed } from "vue";
import ChallengeRow from "./ChallengeRow.vue";

type ConnectionStatus = "Unknown" | "Connected" | "Disconnected";

const Challenges = defineComponent({
  components: { ChallengeRow },
  setup(props) {
    // TODO: DU for loading states
    const isLoadingChallenges: Ref<boolean> = ref(true);
    const connection = new signalR.HubConnectionBuilder()
      .withAutomaticReconnect()
      .withUrl("/challengehub")
      .build();
    const connectionStatus: Ref<ConnectionStatus> = ref("Unknown");
    const challenges: Ref<ChallengeModel[]> = ref([]);

    const acceptChallenge = (challenge: ChallengeModel) => {
      challenge.status = ChallengeStatus.Accepting;
      connection.invoke("AcceptChallenge", challenge.id).catch(function (err) {
        // TODO: Handle error
        console.log("Error accepting challenge: ", err);
      });
    };

    const declineChallenge = (challenge: ChallengeModel) => {
      challenge.status = ChallengeStatus.Declining;
      connection.invoke("DeclineChallenge", challenge.id).catch(function (err) {
        // TODO: Handle error
        console.log("Error declining challenge: ", err);
      });
    };

    const orderedChallenges = computed(() => {
      const byIdDesc = (c1: ChallengeModel, c2: ChallengeModel) => {
        if (c1.id < c2.id) {
          return 1;
        } else if (c1.id > c2.id) {
          return -1;
        } else {
          return 0;
        }
      };

      return [...challenges.value].sort(byIdDesc);
    });

    const connectionStatusBarClass = computed(() => {
      switch (connectionStatus.value) {
        case "Connected":
          return "connected";
        case "Disconnected":
          return "disconnected";
        case "Unknown":
        default:
          return "unknown";
      }
    });

    const canRespondToChallenges = computed(() => {
      return connectionStatus.value === "Connected";
    });

    onMounted(function () {
      const challengeApi = new ChallengeApiApi(
        new Configuration({ basePath: "" })
      );
      challengeApi
        .apiChallengesGet()
        .then((response) => {
          console.log("Response data: ", response);
          challenges.value = response.challenges;

          isLoadingChallenges.value = false;
        })
        .catch((e) => {
          // TODO: GUI for error state
          console.log("Error loading challenges: ", e);
          isLoadingChallenges.value = false;
        });

      connection.onreconnecting(() => {
        connectionStatus.value = "Disconnected";
      });

      connection.onreconnected(() => {
        connectionStatus.value = "Connected";
      });

      connection.on("ChallengeStatusChanged", (challengeId, newStatus) => {
        console.log("ChallengeStatusChanged: ", challengeId, newStatus);
        const challengeToUpdate = challenges.value.find(
          (c) => c.id === challengeId
        );
        if (challengeToUpdate !== undefined) {
          challengeToUpdate.status = newStatus;
        }
      });

      connection.on(
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

          challenges.value.push(challengeToAdd);
        }
      );

      connection
        .start()
        .then(() => {
          connectionStatus.value = "Connected";
        })
        .catch(function (err) {
          return console.error(err.toString());
        });
    });

    return {
      isLoadingChallenges,
      connectionStatus,
      challenges,
      acceptChallenge,
      declineChallenge,
      orderedChallenges,
      connectionStatusBarClass,
      canRespondToChallenges,
    };
  },
});

export default Challenges;
</script>
