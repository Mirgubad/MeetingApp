const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .build();

let localStream;
let screenStream;
let peerConnections = {};
const remoteVideosContainer = document.getElementById('remoteVideos');
const messagesContainer = document.getElementById('messagesContainer');
const iconInfoContainer = document.querySelector('.video-info-icons');
const servers = {
    iceServers: [
        { urls: "stun:stun.l.google.com:19302" }
    ]
};

connection.on("ReceiveSignal", async (senderId, peerId, data) => {
    if (!peerConnections[peerId]) {
        createPeerConnection(peerId);
    }

    const signal = JSON.parse(data);

    if (signal.sdp) {
        await peerConnections[peerId].setRemoteDescription(new RTCSessionDescription(signal.sdp));
        if (signal.sdp.type === "offer") {
            const answer = await peerConnections[peerId].createAnswer();
            await peerConnections[peerId].setLocalDescription(answer);
            connection.invoke("SendSignal", peerId, JSON.stringify({ sdp: peerConnections[peerId].localDescription }));
        }
    } else if (signal.ice) {
        await peerConnections[peerId].addIceCandidate(new RTCIceCandidate(signal.ice));
    }
});

connection.on("ReceiveMessage", (user, message) => {
    const messageElement = document.createElement('div');
    messageElement.innerText = `${user}: ${message}`;
    messagesContainer.appendChild(messageElement);
});


connection.start().then(() => {
    console.log("SignalR connected");
}).catch(err => console.error(err));
+
    async function startCall() {
        localStream = await navigator.mediaDevices.getUserMedia({ video: true, audio: true });
        document.getElementById('localVideo').srcObject = localStream;

        // Create peer connections for all existing clients
        connection.invoke("GetConnectedPeers").then(peerIds => {
            peerIds.forEach(peerId => {
                createPeerConnection(peerId);
            });
        }).catch(err => console.error("Error getting connected peers:", err));

        // Update button text
        document.getElementById('startButton').innerText = 'Call Started';
        document.getElementById('screenShareButton').innerText = 'Share Screen';
        document.getElementById('stopVideoButton').innerText = 'Stop Video';
        document.getElementById('muteButton').innerText = 'Mute Voice';
        document.getElementById('endCallButton').innerText = 'End Call';
    }

function createPeerConnection(peerId) {
    peerConnections[peerId] = new RTCPeerConnection(servers);


    peerConnections[peerId].addEventListener('icecandidate', event => {
        console.log("icecandidate event:", event)
        if (event.candidate) {
            connection.invoke("SendSignal", peerId, JSON.stringify({ ice: e.candidate }));
        }
    });


    //peerConnections[peerId].onicecandidate = e => {
    //    console.log("onicecandidate event:", e)
    //    if (e.candidate) {
    //        connection.invoke("SendSignal", peerId, JSON.stringify({ ice: e.candidate }));
    //    }
    //};

    peerConnections[peerId].ontrack = event => {
        const remoteVideo = document.getElementById(`remoteVideo-${peerId}`);
        if (!remoteVideo) {
            const newRemoteVideo = document.createElement('video');
            newRemoteVideo.id = `remoteVideo-${peerId}`;
            newRemoteVideo.autoplay = true;
            newRemoteVideo.playsinline = true;
            remoteVideosContainer.appendChild(newRemoteVideo);
        }

        document.getElementById(`remoteVideo-${peerId}`).srcObject = event.streams[0];
    };

    localStream.getTracks().forEach(track => peerConnections[peerId].addTrack(track, localStream));
}

async function toggleScreenShare() {
    if (!screenStream) {
        startScreenShare();
    } else {
        stopScreenShare();
    }
}

async function startScreenShare() {
    const displayMediaOptions = {
        video: {
            cursor: "always"
        },
        audio: false // Screen share typically doesn't include audio
    };

    try {
        screenStream = await navigator.mediaDevices.getDisplayMedia(displayMediaOptions);
        Object.keys(peerConnections).forEach(peerId => {
            const screenTrack = screenStream.getTracks()[0];
            peerConnections[peerId].addTrack(screenTrack, screenStream);
        });

        // Update button text and icon
        document.getElementById('screenShareButton').innerText = 'Stop Screen Share';
        addIcon('screenShareIcon', 'Screen Shared');

    } catch (err) {
        console.error("Error accessing screen share:", err);
    }
}

function stopScreenShare() {
    if (screenStream) {
        Object.keys(peerConnections).forEach(peerId => {
            const sender = peerConnections[peerId].getSenders().find(sender => sender.track.kind === 'video');
            if (sender) {
                peerConnections[peerId].removeTrack(sender);
            }
        });

        screenStream.getTracks().forEach(track => track.stop());
        screenStream = null;

        // Update button text and icon
        document.getElementById('screenShareButton').innerText = 'Share Screen';
        removeIcon('screenShareIcon');
    }
}

function toggleVideo() {
    localStream.getVideoTracks().forEach(track => {
        track.enabled = !track.enabled;
    });

    // Update button text and icon
    const button = document.getElementById('stopVideoButton');
    if (button.innerText === 'Stop Video') {
        button.innerText = 'Video Off';
        addIcon('videoOffIcon', 'Video Off');
    } else {
        button.innerText = 'Stop Video';
        removeIcon('videoOffIcon');
    }
}

function toggleAudio() {
    localStream.getAudioTracks().forEach(track => {
        track.enabled = !track.enabled;
    });

    // Update button text and icon
    const button = document.getElementById('muteButton');
    if (button.innerText === 'Mute Voice') {
        button.innerText = 'Voice Muted';
        addIcon('voiceMutedIcon', 'Voice Muted');
    } else {
        button.innerText = 'Mute Voice';
        removeIcon('voiceMutedIcon');
    }
}

function endCall() {
    Object.keys(peerConnections).forEach(peerId => {
        peerConnections[peerId].close();
        delete peerConnections[peerId];
    });

    if (localStream) {
        localStream.getTracks().forEach(track => track.stop());
        localStream = null;
    }

    if (screenStream) {
        screenStream.getTracks().forEach(track => track.stop());
        screenStream = null;
    }

    while (remoteVideosContainer.firstChild) {
        remoteVideosContainer.removeChild(remoteVideosContainer.firstChild);
    }

    // Reset button text and icons
    document.getElementById('startButton').innerText = 'Start Call';
    document.getElementById('screenShareButton').innerText = 'Share Screen';
    document.getElementById('stopVideoButton').innerText = 'Stop Video';
    document.getElementById('muteButton').innerText = 'Mute Voice';
    document.getElementById('endCallButton').innerText = 'End Call';

    // Remove icons
    removeIcon('screenShareIcon');
    removeIcon('videoOffIcon');
    removeIcon('voiceMutedIcon');
}

function sendMessage() {
    const messageInput = document.getElementById('messageInput');
    const message = messageInput.value;
    if (message) {
        connection.invoke("SendMessage", message)
            .then(() => {
                messageInput.value = '';
            })
            .catch(err => console.error("Error sending message:", err));
    }
}

messageInput.addEventListener("keypress", function (event) {
    // If the user presses the "Enter" key on the keyboard
    if (event.key === "Enter") {
        // Cancel the default action, if needed
        event.preventDefault();
        sendMessage();
    }
});

function addIcon(iconId, labelText) {
    const iconContainer = document.createElement('div');
    iconContainer.id = iconId;
    iconContainer.classList.add('icon-container');
    iconContainer.innerText = labelText;

    iconInfoContainer.appendChild(iconContainer);
}

function removeIcon(iconId) {
    const icon = document.getElementById(iconId);
    if (icon) {
        iconInfoContainer.removeChild(icon);
    }
}

