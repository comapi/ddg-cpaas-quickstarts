// Bring in required dependencies
var request = require("request");

// **** ENTER YOUR DETAILS HERE ****

// Enter your Comapi API Space Id here e.g. 11164198-3f3f-4993-ab8f-70680c1113b1
var yourComapiAPISpaceId = 'YOUR_API_SPACE_ID';

// Enter your Comapi access token here
var yourComapiAccessToken = 'YOUR_ACCESS_TOKEN';

// Enter your mobile number in international format here e.g. for the UK 447123123123
var yourMobileNumber = 'YOUR_MOBILE_NUMBER';

console.log('');
console.log('Sending a batch of SMS using Comapi and NodeJS');
console.log('----------------------------------------------');

// Create Comapi RESTful URL with API Space Id in it
var comapiUrl = "https://api.comapi.com/apispaces/" + yourComapiAPISpaceId + "/messages/batch";

// Setup Comapi batch request JSON, this is an array of messages to be sent, we are sending to the SMS channel but they could easily
// be a mix of any messages targeting any channels.
var myRequestBatch = [
    {
        body: 'This is message 1',
        to: { phoneNumber: yourMobileNumber },
        rules: ['sms']
    },
    {
        body: 'This is message 2',
        to: { phoneNumber: yourMobileNumber },
        channelOptions: {
            sms: {
                from: 'Comapi',
                allowUnicode: true
            }
        },
        rules: ['sms']
    }
];

// Call Comapi "One" API
var options = {
    method: 'POST',
    url: comapiUrl,
    headers:
    {
        'cache-control': 'no-cache',
        'content-type': 'application/json',
        'accept': 'application/json',
        authorization: 'Bearer ' + yourComapiAccessToken
    },
    body: myRequestBatch,
    json: true
};

// Send the request
console.log('');
console.log('Calling Comapi...');

request(options, function (error, response, body) {
    if (error) throw new Error(error);

    console.log("HTTP status code returned: " + response.statusCode);
    console.log(body);

    // Check status of Accepted
    if (response.statusCode == 202) {
        // All ok
        console.log('Message batch successfully sent via Comapi "One" API');
        console.log('An array of message ids has been returned mapping to your request array');
    }
    else {
        // Something went wrong
        console.log('Something went wrong!');
    }
});