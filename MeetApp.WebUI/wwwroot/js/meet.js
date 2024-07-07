//Message Container Toggler


document.addEventListener("DOMContentLoaded", () => {

    const messageToggleBtn = document.querySelector(".messageBtn");
    const messageCount = document.querySelector("#messageCount");
    const messagesList = document.querySelector("#messages");
    const messageContainer = document.querySelector("#chat");
    const callShareBtn = document.querySelector(".call-share");
    const callShareModal = document.querySelector(".call-share__modal");
    const chatCloseBtn = document.querySelector("#chatCloseBtn");
    let showMessage = true;


    // Assuming SignalR and PeerJS scripts are loaded properly

    const chatConnection = new signalR.HubConnectionBuilder()
        .withUrl("/chathub").build();

    const ROOM_ID = document.getElementById("roomId").value;
    const userName = document.getElementById("userName").value;

    let userId = null;
    let localStream = null;
    let screenStream = null;
    let isScreenSharing = false;
    let isMuted = false;
    let videoPause = false;
    const Peers = {};

    const myPeer = new Peer();

    myPeer.on('open', id => {
        userId = id;
        const startSignalR = async () => {
            await chatConnection.start();
            await chatConnection.invoke("JoinRoom", ROOM_ID, userId);
        };
        startSignalR();
    });

    const videoGrid = document.querySelector('[video-grid]');
    let myVideo = document.createElement('video');
    myVideo.setAttribute("class", "main-video");
    myVideo.setAttribute("playsinline", true);
    myVideo.setAttribute("muted", true);
    myVideo.muted = true;


    // working on pc
    //navigator.mediaDevices.getUserMedia({
    //    audio: true,
    //    video: true
    //}).then(stream => {
    //    addVideoStream(myVideo, stream);
    //    localStream = stream;
    //});

    navigator.mediaDevices.getUserMedia({
        audio: true,
        video: {
            facingMode: "user", // Use "environment" for the rear camera
            width: { ideal: 1280 },
            height: { ideal: 720 }
        }
    }).then(stream => {
        addVideoStream(myVideo, stream);
        localStream = stream;
    }).catch(error => {
        console.error('Error accessing media devices.', error);
        alert('Error accessing media devices. Please check your permissions and try again.');
    });



    chatConnection.on("user-connected", id => {
        if (userId === id) return;
        console.log(`User connected ${id}`);
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
        userVideo.setAttribute("autoplay", true);





        call.on('stream', userVideoStream => {
            addVideoStream(userVideo, userVideoStream);
        });

        call.on("close", () => {
            userVideo.remove();
        });

        Peers[call.peer] = call;
    });


    const addVideoStream = (video, stream) => {
        video.srcObject = stream;
        video.addEventListener('loadedmetadata', () => {
            video.play();
        });
        videoGrid.appendChild(video);
    };

    const connectNewUser = (userId, stream) => {
        const call = myPeer.call(userId, stream);
        const userVideo = document.createElement('video');
        userVideo.setAttribute("class", "user-video");
        userVideo.setAttribute("playsinline", true);
        userVideo.setAttribute("muted", true);
        userVideo.setAttribute("autoplay", true);

        call.on('stream', userVideoStream => {
            addVideoStream(userVideo, userVideoStream);
        });

        call.on("close", () => {
            userVideo.remove();
        });

        Peers[userId] = call;
    };

    const screenShareButton = document.querySelector('#screen-share-button');
    screenShareButton.addEventListener('click', async () => {
        try {
            if (!isScreenSharing) {
                screenStream = await navigator.mediaDevices.getDisplayMedia({ video: true });
                addVideoStream(myVideo, screenStream);

                Object.values(Peers).forEach(peer => {
                    const sender = peer.peerConnection.getSenders().find(sender => sender.track.kind === 'video');
                    sender.replaceTrack(screenStream.getVideoTracks()[0]);
                });

                isScreenSharing = true;
                const iconElement = screenShareButton.querySelector('i');
                iconElement.classList.remove('fa-screencast');
                iconElement.classList.add('fa-stop');
            } else {
                screenStream.getTracks().forEach(track => track.stop());
                myVideo.srcObject = localStream;

                Object.values(Peers).forEach(peer => {
                    const sender = peer.peerConnection.getSenders().find(sender => sender.track.kind === 'video');
                    sender.replaceTrack(localStream.getVideoTracks()[0]);
                });

                isScreenSharing = false;
                const iconElement = screenShareButton.querySelector('i');
                iconElement.classList.remove('fa-stop');
                iconElement.classList.add('fa-screencast');
            }
        } catch (error) {
            console.error('Error when screen sharing:', error);
        }
    });

    const muteButton = document.querySelector('#mute-button');
    muteButton.addEventListener('click', () => {
        if (!localStream) return;
        const pausedIcon = document.querySelector(".paused.audio")
        const notPausedIcon = document.querySelector(".not-paused.audio")

        const audioTracks = localStream.getAudioTracks();
        if (audioTracks.length === 0) return;

        audioTracks.forEach(track => {
            track.enabled = !track.enabled;
        });

        isMuted = !isMuted;
        const iconElement = muteButton.querySelector('i');
        if (isMuted) {
            pausedIcon.style.display = "block";
            notPausedIcon.style.display = "none";
            muteButton.style.backgroundColor = "#e30000"
        } else {
            pausedIcon.style.display = "none";
            notPausedIcon.style.display = "block";
            muteButton.style.backgroundColor = "rgb(60,64,67)"
        }
    });

    const pauseVideoButton = document.querySelector('#pause-video-button');


    //worked


    //// Function to toggle video pause and display initial
    pauseVideoButton.addEventListener('click', () => {

        const pausedIcon = document.querySelector(".paused.video")
        const notPausedIcon = document.querySelector(".not-paused.video")
        const videoTracks = localStream.getVideoTracks();

        if (videoTracks.length === 0) return;

        videoTracks.forEach(track => {
            track.enabled = !track.enabled;
        });

        videoPause = !videoPause;
        const iconElement = pauseVideoButton.querySelector('i');
        if (videoPause) {
            pausedIcon.style.display = "block";
            notPausedIcon.style.display = "none";
            pauseVideoButton.style.backgroundColor = "#e30000"

        } else {
            pausedIcon.style.display = "none";
            notPausedIcon.style.display = "block";
            pauseVideoButton.style.backgroundColor = "rgb(60,64,67)"
        }
    });







    document.querySelector("#send-button").addEventListener("click", async () => {
        sendMessage();
    });

    document.querySelector("#message-input").addEventListener("keypress", async (event) => {
        if (event.key === "Enter") {
            sendMessage();
        }
    });

    //// Function to swap main video with user video
    //const swapVideo = (userVideo) => {
    //    // Save the srcObject of the main video
    //    const mainVideoStream = myVideo.srcObject;

    //    // Set the srcObject of the main video to the srcObject of the clicked user video
    //    myVideo.srcObject = userVideo.srcObject;

    //    // Set the srcObject of the clicked user video to the saved srcObject of the main video
    //    userVideo.srcObject = mainVideoStream;
    //};


    // Function to swap main video with user video
    const swapVideo = (userVideo) => {
        // Save the srcObject of the main video
        const mainVideoStream = myVideo.srcObject;

        // Save the initial content of the main video
        const mainVideoInitial = document.querySelector('.main-video-container');

        // Set the srcObject of the main video to the srcObject of the clicked user video
        myVideo.srcObject = userVideo.srcObject;

        // Set the srcObject of the clicked user video to the saved srcObject of the main video
        userVideo.srcObject = mainVideoStream;

        // Swap the video-initial elements
        const userVideoInitial = userVideo.parentElement.querySelector('.video-initial');
        if (mainVideoInitial && userVideoInitial) {
            myVideo.parentElement.insertBefore(userVideoInitial, myVideo);
            userVideo.parentElement.insertBefore(mainVideoInitial, userVideo);
        }
    };


    // Adding event listener using event delegation
    document.addEventListener('click', (event) => {
        // Check if the clicked element is a user video
        if (event.target && event.target.classList.contains('user-video')) {
            // Call swapVideo function when user video is clicked
            swapVideo(event.target);
        }
    });

    async function sendMessage() {
        const messageInput = document.querySelector("#message-input");
        const message = messageInput.value;
        if (message.trim() !== "") {
            await chatConnection.invoke("SendMessage", message);
            messageInput.value = "";
        }
    }


    let msgCount = 0;

    chatConnection.on("ReceiveMessage", (senderName, message) => {
        const messagesList = document.querySelector("#messages");
        const li = document.createElement("li");
        let checkUser = senderName.split(" ")[0] == userName
        li.classList.add("message_container");

        if (checkUser) {
            li.innerHTML = `<span name>You</span> <p>${message}</p>`;
            li.style.marginLeft = "auto"
            li.style.borderRadius = "20px 0px 20px 20px";
        } else {
            li.innerHTML = `<span name>${checkUser ? "You" : userName}</span> <p>${message}</p>`;
        }

        messagesList.appendChild(li);
        if (showMessage) {
            ++msgCount;
            messageCount.textContent = msgCount;
            messageCount.style.visibility = "visible"
        }
    });





    messageToggleBtn.addEventListener("click", () => {
        checkMessage()
    })
    chatCloseBtn.addEventListener("click", () => {
        checkMessage()
    })

    function checkMessage() {
        if (showMessage) {
            messageContainer.classList.add("opened");
            msgCount = 0;
            messageCount.textContent = msgCount;
            messageCount.style.visibility = "hidden"
        } else {
            messageContainer.classList.remove("opened");
        }
        showMessage = !showMessage;

    }




    callShareBtn.addEventListener("click", () => {
        callShareModal.classList.toggle("opened");
    })

    const copyLinkButton = document.getElementById('copy-link-button');
    const shareWhatsappButton = document.getElementById('share-whatsapp-button');

    function copyToClipboard(text) {
        navigator.clipboard.writeText(text).then(() => {
            alert('Link copied to clipboard!');
        }).catch(err => {
            console.error('Failed to copy text: ', err);
        });
    }

    function shareViaWhatsApp(text) {
        const whatsappUrl = `https://api.whatsapp.com/send?text=${encodeURIComponent(text)}`;
        window.open(whatsappUrl, '_blank');
    }

    copyLinkButton.addEventListener('click', () => {
        const linkToCopy = window.location.href; // Replace with the link you want to copy
        copyToClipboard(linkToCopy);
    });

    shareWhatsappButton.addEventListener('click', () => {
        const textToShare = window.location.href; // Replace with the text you want to share
        shareViaWhatsApp(textToShare);
    });



})


function cancelCall() {
    Swal.fire({
        title: "Are you sure to live call?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes!"
    }).then((result) => {
        if (result.isConfirmed) {
            location.replace('/')
        }
    });

}