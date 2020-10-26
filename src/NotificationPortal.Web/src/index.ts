import * as signalR from "@microsoft/signalr";

const connectionStatusIndicator = document.getElementById(
    "connectionStatusIndicator"
);

const connection = new signalR.HubConnectionBuilder()
    .withAutomaticReconnect()
    .withUrl("/challengehub")
    .build();

connection.onreconnecting(() => {
    connectionStatusIndicator.style.backgroundColor = "red";
    console.log("Reconnecting...");
});

connection.onreconnected(() => {
    connectionStatusIndicator.style.backgroundColor = "lightgreen";
    console.log("Reconnected!");
});

const acceptButtons = document.getElementsByClassName(
    "accept-challenge-button"
);
const declineButtons = document.getElementsByClassName(
    "decline-challenge-button"
);

function findStatusCell(notificationId) {
    const allStatusCells = document.getElementsByClassName(
        "challenge-status-cell"
    );
    for (const statusCell of Array.from(allStatusCells)) {
        if (statusCell instanceof HTMLElement && Number(statusCell.dataset.id) === notificationId) return statusCell;
    }

    return null;
}

connection.on("NewChallengeIssued", function (
    challengeId,
    communityName,
    fromPlayer,
    toPlayer,
    status,
    date
) {
    console.log(
        "Challenge received: ",
        challengeId,
        communityName,
        fromPlayer,
        toPlayer,
        status,
        date
    );

    const tr = document.createElement("tr");
    const idTd = document.createElement("td");
    idTd.innerText = challengeId;
    const communityNameTd = document.createElement("td");
    communityNameTd.innerText = communityName;
    const fromPlayerTd = document.createElement("td");
    fromPlayerTd.innerText = fromPlayer;
    const toPlayerTd = document.createElement("td");
    toPlayerTd.innerText = toPlayer;
    const statusTd = document.createElement("td");
    statusTd.innerHTML = getHtmlForStatus(status);
    statusTd.style.backgroundColor = getColorForStatus(status);
    statusTd.className = "challenge-status-cell";
    statusTd.dataset.id = challengeId;
    const dateTd = document.createElement("td");
    dateTd.className = "date-cell";
    dateTd.innerText = date;

    const acceptButton = document.createElement("button");
    acceptButton.className = "accept-challenge-button";
    acceptButton.disabled = status !== "Challenged";
    acceptButton.dataset.id = challengeId;
    acceptButton.innerText = "Accept";
    acceptButton.addEventListener("click", acceptClickEventListener);

    const declineButton = document.createElement("button");
    declineButton.className = "decline-challenge-button";
    declineButton.disabled = status !== "Challenged";
    declineButton.dataset.id = challengeId;
    declineButton.innerText = "Decline";
    declineButton.addEventListener("click", declineClickEventListener);

    const buttonsTd = document.createElement("td");
    buttonsTd.appendChild(acceptButton);
    buttonsTd.appendChild(declineButton);

    tr.appendChild(idTd);
    tr.appendChild(communityNameTd);
    tr.appendChild(fromPlayerTd);
    tr.appendChild(toPlayerTd);
    tr.appendChild(statusTd);
    tr.appendChild(dateTd);
    tr.appendChild(buttonsTd);

    document.getElementById("challengeListTableBody").prepend(tr);
});

function getColorForStatus(status) {
    switch (status) {
        case "Accepted":
            return "lightgreen";
        case "Declined":
            return "darkorange";
        default:
            return "white";
    }
}

function getHtmlForStatus(status) {
    switch (status) {
        case "Challenged":
        case "Accepted":
        case "Declined":
            return status;
        case "Challenging":
        case "Accepting":
        case "Declining":
        default:
            return "<div class='loader'>Waiting...</div>";
    }
}

connection.on("ChallengeStatusChanged", function (challengeId, newStatus) {
    console.log("ChallengeStatusChanged: ", challengeId, newStatus);
    const statusCell = findStatusCell(challengeId);

    if (statusCell !== null) {
        statusCell.innerHTML = getHtmlForStatus(newStatus);
        statusCell.style.backgroundColor = getColorForStatus(newStatus);
    }

    const buttonsShouldBeDisabled = newStatus !== "Challenged";
    const buttonsShouldBeVisible =
        newStatus === "Challenging" || newStatus === "Challenged";

    for (const acceptButton of Array.from(acceptButtons)) {
        if (acceptButton instanceof HTMLButtonElement && Number(acceptButton.dataset.id) === challengeId) {
            if (!buttonsShouldBeVisible) {
                acceptButton.remove();
            } else {
                acceptButton.disabled = buttonsShouldBeDisabled;
            }
        }
    }
    for (const declineButton of Array.from(declineButtons)) {
        if (declineButton instanceof HTMLButtonElement && Number(declineButton.dataset.id) === challengeId) {
            if (!buttonsShouldBeVisible) {
                declineButton.remove();
            } else {
                declineButton.disabled = buttonsShouldBeDisabled;
            }
        }
    }
});

function acceptClickEventListener(event) {
    const notificationId = Number(event.target.dataset.id);
    connection.invoke("AcceptChallenge", notificationId).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
}

function declineClickEventListener(event) {
    const notificationId = Number(event.target.dataset.id);
    connection.invoke("DeclineChallenge", notificationId).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
}

connection
    .start()
    .then(function () {
        connectionStatusIndicator.style.backgroundColor = "lightgreen";
        for (const acceptButton of Array.from(acceptButtons)) {
            if (acceptButton instanceof HTMLButtonElement) {
                acceptButton.disabled = false;
                acceptButton.addEventListener("click", acceptClickEventListener);
            }
        }
        for (const declineButton of Array.from(declineButtons)) {
            if (declineButton instanceof HTMLButtonElement) {
                declineButton.disabled = false;
                declineButton.addEventListener("click", declineClickEventListener);
            }
        }

        console.log("Connected to Hub!");
    })
    .catch(function (err) {
        return console.error(err.toString());
    });
