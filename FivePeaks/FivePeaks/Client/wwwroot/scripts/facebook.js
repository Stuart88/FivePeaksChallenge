
(function (d, s, id) {
    var js, fjs = d.getElementsByTagName(s)[0];
    if (d.getElementById(id)) { return; }
    js = d.createElement(s); js.id = id;
    js.src = "https://connect.facebook.net/en_US/sdk.js";
    fjs.parentNode.insertBefore(js, fjs);
}(document, 'script', 'facebook-jssdk'));

window.fbAsyncInit = function () {
    FB.init({
        appId: '2852016631566500',
        cookie: true,
        xfbml: true,
        version: 'v8.0'
    });

    FB.AppEvents.logPageView();
};

window.isUserFBLoggedIn = function () {

    FB.getLoginStatus(function (response) {
        if (response) {
            DotNet.invokeMethodAsync('FivePeaks.Client', 'LoginResult', response.status === "connected");
        } else {
            DotNet.invokeMethodAsync('FivePeaks.Client', 'LoginResult', false);
        }
    });
}

window.fbLogin = function() {
    FB.login(function (response) {
        if (response) {
            DotNet.invokeMethodAsync('FivePeaks.Client', 'LoginResult', response.status === "connected");
        } else {
            DotNet.invokeMethodAsync('FivePeaks.Client', 'LoginResult', false);
        }
    }, { scope: 'public_profile,email' });
}

window.fbLogout = function () {
    FB.logout(function (response) {
        if (response) {
            DotNet.invokeMethodAsync('FivePeaks.Client', 'LogoutResult', true);
        } else {
            DotNet.invokeMethodAsync('FivePeaks.Client', 'LogoutResult', false);
        }
    });
}