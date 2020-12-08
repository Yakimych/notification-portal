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
          <!-- @foreach (var challenge in Model.Challenges.OrderByDescending(c => c.Id))
                { -->
          <tr>
            <td>@challenge.Id</td>
            <td>@challenge.CommunityName</td>
            <td>@challenge.FromPlayer</td>
            <td>@challenge.ToPlayer</td>
            <td
              style="background-color: @GetStatusColor (challenge.Status)"
              class="challenge-status-cell"
              data-id="@challenge.Id"
            >
              <!-- @{
                                if (ShowSpinner(challenge.Status))
                                { -->
              <div class="loader">Waiting...</div>
              <!-- }
                                else
                                { -->
              @challenge.Status
              <!-- }
                            } -->
            </td>
            <td class="date-cell">@challenge.Date.FormatDateTime()</td>
            <td>
              <!-- @if (challenge.Status == ChallengeStatus.Challenged || challenge.Status ==
                                ChallengeStatus.Challenging)
                            { -->
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
              <!-- } -->
            </td>
          </tr>
          <!-- } -->
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
import { defineComponent } from "vue";

type ConnectionStatus = "Unknown" | "Connected" | "Disconnected";

const Challenges = defineComponent({
  mounted: function () {
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
    someText: "Some text",
    connectionStatus: "Unknown" as ConnectionStatus,
  }),
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
