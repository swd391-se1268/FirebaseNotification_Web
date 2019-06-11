
var firebaseConfig = {
    apiKey: "AIzaSyBH8jy2qv1bstUNH1ERzHfyUtJwYDMPTYk",
    authDomain: "notification-4e6a5.firebaseapp.com",
    databaseURL: "https://notification-4e6a5.firebaseio.com",
    projectId: "notification-4e6a5",
    storageBucket: "notification-4e6a5.appspot.com",
    messagingSenderId: "81329239971",
    appId: "1:81329239971:web:6df7fbc0725d40c7"
};

// Initialize Firebase
firebase.initializeApp(firebaseConfig);

const messageHandler = firebase.messaging();
console.log(messageHandler);

Notification.requestPermission().then(function (permission) {
    if (permission === 'granted') {
        console.log('Notification permission granted.');
    } else {
        console.log('Unable to get permission to notify.');
    }
});

messageHandler.getToken().then(function (currentToken) {

    console.log("token = " + currentToken);

}).catch(function (err) {
    console.log(err);
});

messageHandler.onMessage(function (payload) {
    console.log('Message received. ', payload);
});

