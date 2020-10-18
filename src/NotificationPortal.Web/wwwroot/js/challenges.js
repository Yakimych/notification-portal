"use strict";

const connection = new signalR.HubConnectionBuilder()
  .withUrl("/challengehub")
  .build();

const acceptButtons = document.getElementsByClassName(
  "accept-challenge-button"
);
const declineButtons = document.getElementsByClassName(
  "decline-challenge-button"
);

function findStatusCell(notificationId) {
  const allStatusCells = document.getElementsByClassName("challenge-type");
  for (const statusCell of allStatusCells) {
    if (Number(statusCell.dataset.id) === notificationId) return statusCell;
  }

  return null;
}

function findAcceptButton(notificationId) {
  for (const acceptButton of acceptButtons) {
    if (Number(acceptButton.dataset.id) === notificationId) return acceptButton;
  }

  return null;
}

function findDeclineButton(notificationId) {
  for (const declineButton of declineButtons) {
    if (Number(declineButton.dataset.id) === notificationId)
      return declineButton;
  }

  return null;
}

connection.on("ChallengeReceived", function (
  challengeId,
  communityName,
  fromPlayer,
  toPlayer,
  date
) {
  console.log(
    "Challenge received: ",
    challengeId,
    communityName,
    fromPlayer,
    toPlayer,
    date
  );
  // document.getElementById("messagesList").appendChild(li);
});

connection.on("ChallengeAccepted", function (challengeId) {
  const statusCell = findStatusCell(challengeId);
  const acceptButton = findAcceptButton(challengeId);
  const declineButton = findDeclineButton(challengeId);

  if (statusCell !== null && acceptButton !== null && declineButton !== null) {
    statusCell.innerHTML = "Accepted";
    statusCell.style.backgroundColor = "lightgreen";
    acceptButton.remove();
    declineButton.remove();
  }
});

connection.on("ChallengeDeclined", function (challengeId) {
  const statusCell = findStatusCell(challengeId);
  const acceptButton = findAcceptButton(challengeId);
  const declineButton = findDeclineButton(challengeId);

  if (statusCell !== null && acceptButton !== null && declineButton !== null) {
    statusCell.innerHTML = "Declined";
    statusCell.style.backgroundColor = "darkorange";
    acceptButton.remove();
    declineButton.remove();
  }
});

connection
  .start()
  .then(function () {
    for (const acceptButton of acceptButtons) {
      acceptButton.disabled = false;
      acceptButton.addEventListener("click", function (event) {
        connection
          .invoke("AcceptChallenge", Number(event.target.dataset.id))
          .catch(function (err) {
            return console.error(err.toString());
          });
        event.preventDefault();
      });
    }
    for (const declineButton of declineButtons) {
      declineButton.disabled = false;
      declineButton.addEventListener("click", function (event) {
        connection
          .invoke("DeclineChallenge", Number(event.target.dataset.id))
          .catch(function (err) {
            return console.error(err.toString());
          });
        event.preventDefault();
      });
    }

    console.log("Connected to Hub!");
  })
  .catch(function (err) {
    return console.error(err.toString());
  });
