// app.js

const messageToggleBtn = document.querySelector("#messageToggleBtn");
const messageCount = document.querySelector("#messageCount");
const messagesList = document.querySelector("#messages");
const messageContainer = document.querySelector("#chat");
let showMessage = true;

const chatConnection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .build();

const myPeer = new Peer();

let localStream = null;
let screenStream = null;
let isScreenSharing = false;
let isMuted = false;
let videoPause = false;
const Peers = {};
let userId = null;
let msgCount = 0;

const videoGrid = document.querySelector('#video-grid');
const localVideo = document.createElement('video');
localVideo.setAttribute("class", "main-video");
localVideo.setAttribute("playsinline", true);
localVideo.setAttribute("muted", true);

document.addEventListener('DOMContentLoaded', async () => {


    const screenShareButton = document.getElementById('screenShareButton');
    screenShareButton.addEventListener('click', toggleScreenSharing);

    const muteButton = document.getElementById('muteButton');
    muteButton.addEventListener('click', toggleMute);

    const pauseVideoButton = document.getElementById('pauseVideoButton');
    pauseVideoButton.addEventListener('click', toggleVideoPause);

    const sendButton = document.getElementById('send-button');
    sendButton.addEventListener('click', sendMessage);

    const messageInput = document.getElementById('message-input');
    messageInput.addEventListener('keypress', function (e) {
        if (e.key === 'Enter') {
            sendMessage();
        }
    });

    messageToggleBtn.addEventListener('click', toggleMessageContainer);

    try {
        localStream = await navigator.mediaDevices.getUserMedia({
            video: { facingMode: 'user' },
            audio: true
        });
        addVideoStream(localVideo, localStream);
        startSignalR();
    } catch (error) {
        console.error('Error accessing media devices:', error);
        alert('Error accessing media devices. Please check your permissions and try again.');
    }
});

function startSignalR() {
    myPeer.on('open', id => {
        userId = id;
        chatConnection.start()
            .then(() => chatConnection.invoke("JoinRoom", userId))
            .catch(error => console.error('Error connecting to SignalR:', error));
    });

    chatConnection.on("user-connected", id => {
        if (userId === id) return;
        connectNewUser(id, localStream);
    });

    chatConnection.on('user-disconnected', id => {
        console.log("User disconnected: " + id);
        if (Peers[id]) {
            Peers[id].close();
        }
    });

    myPeer.on('call', call => {
        call.answer(localStream);
        const userVideo = document.createElement('video');
        userVideo.setAttribute("class", "user-video");
        userVideo.setAttribute("playsinline", true);
        userVideo.setAttribute("muted", true);

        call.on('stream', userVideoStream => {
            addVideoStream(userVideo, userVideoStream);
        });

        call.on("close", () => {
            userVideo.remove();
        });

        Peers[call.peer] = call;
    });

    chatConnection.on("ReceiveMessage", (senderName, message) => {
        const li = document.createElement("li");
        const checkUser = senderName.split(" ")[0] === userName;

        li.classList.add("message_container");
        li.innerHTML = `<span>${checkUser ? "You" : senderName}</span> <p>${message}</p>`;

        messagesList.appendChild(li);
        if (showMessage) {
            messageCount.textContent = ++msgCount;
            messageCount.style.visibility = "visible";
        }
    });
}

function addVideoStream(videoElement, stream) {
    videoElement.srcObject = stream;
    videoElement.addEventListener('loadedmetadata', () => {
        videoElement.play().catch(error => {
            console.error('Autoplay was prevented:', error);
            videoElement.setAttribute("controls", true); // Fallback to controls if autoplay fails
        });
    });
    videoGrid.appendChild(videoElement);
}

function connectNewUser(userId, stream) {
    const call = myPeer.call(userId, stream);
    const userVideo = document.createElement('video');
    userVideo.setAttribute("class", "user-video");
    userVideo.setAttribute("playsinline", true);
    userVideo.setAttribute("muted", true);

    call.on('stream', userVideoStream => {
        addVideoStream(userVideo, userVideoStream);
    });

    call.on("close", () => {
        userVideo.remove();
    });

    Peers[userId] = call;
}

async function toggleScreenSharing() {
    try {
        if (!isScreenSharing) {
            screenStream = await navigator.mediaDevices.getDisplayMedia({ video: true });
            addVideoStream(localVideo, screenStream);

            Object.values(Peers).forEach(peer => {
                const sender = peer.peerConnection.getSenders().find(sender => sender.track.kind === 'video');
                sender.replaceTrack(screenStream.getVideoTracks()[0]);
            });

            isScreenSharing = true;
            screenShareButton.textContent = 'Stop Sharing';
        } else {
            screenStream.getTracks().forEach(track => track.stop());
            localVideo.srcObject = localStream;

            Object.values(Peers).forEach(peer => {
                const sender = peer.peerConnection.getSenders().find(sender => sender.track.kind === 'video');
                sender.replaceTrack(localStream.getVideoTracks()[0]);
            });

            isScreenSharing = false;
            screenShareButton.textContent = 'Share Screen';
        }
    } catch (error) {
        console.error('Error when screen sharing:', error);
    }
}

function toggleMute() {
    if (!localStream) return;

    localStream.getAudioTracks().forEach(track => {
        track.enabled = !track.enabled;
    });

    isMuted = !isMuted;
    muteButton.textContent = isMuted ? 'Unmute Audio' : 'Mute Audio';
}

function toggleVideoPause() {
    if (!localStream) return;

    localStream.getVideoTracks().forEach(track => {
        track.enabled = !track.enabled;
    });

    videoPause = !videoPause;
    pauseVideoButton.textContent = videoPause ? 'Resume Video' : 'Pause Video';
}

function sendMessage() {
    const messageInput = document.getElementById('message-input');
    const message = messageInput.value.trim();
    if (message !== "") {
        chatConnection.invoke("SendMessage", message);
        messageInput.value = "";
    }
}

function toggleMessageContainer() {
    if (showMessage) {
        messageContainer.style.display = "none";
        showMessage = false;
        msgCount = 0;
        messageCount.textContent = msgCount;
        messageCount.style.visibility = "hidden";
    } else {
        messageContainer.style.display = "block";
        showMessage = true;
    }
}
