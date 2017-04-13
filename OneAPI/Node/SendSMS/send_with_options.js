// Bring in required dependencies
var request = require("request");

// **** ENTER YOUR DETAILS HERE ****

// Enter your Comapi API Space Id here e.g. 11164198-3f3f-4993-ab8f-70680c1113b1
var yourComapiAPISpaceId = 'YOUR_API_SPACE_ID';

// Enter your Comapi access token here
var yourComapiAccessToken = 'YOUR_ACCESS_TOKEN';

// Enter your mobile number in international format here e.g. for the UK 447123123123
var yourMobileNumber = 'YOUR_MOBILE_NUMBER';

// Create Comapi RESTful URL with API Space Id in it
var comapiUrl = "https://api.comapi.com/apispaces/" + yourComapiAPISpaceId + "/messages";

// Setup Comapi request JSON
var myRequest = {
    body: 'Your SMS message',
    to: { phoneNumber: yourMobileNumber },
    channelOptions: {
        sms: {
            from: 'Comapi',
            allowUnicode: true
        }
    },
    rules: ['sms']
};

// Call Comapi "One" API
var options = {
    method: 'POST',
    url: comapiUrl,
    headers:
    {
        'cache-control': 'no-cache',
        'content-type': 'application/json',
        authorization: 'Bearer ' + yourComapiAccessToken
    },
    body: myRequest,
    json: true
};

// Send the request
request(options, function (error, response, body) {
    if (error) throw new Error(error);
    console.log('SMS messgae successfully sent via Comapi "One" API');
    console.log(body);
});