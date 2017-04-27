// Bring in required dependencies
var request = require("request");

// **** ENTER YOUR DETAILS HERE ****

// Enter your Comapi API Space Id here e.g. 11164198-3f3f-4993-ab8f-70680c1113b1
var yourComapiAPISpaceId = 'YOUR_API_SPACE_ID';

// Enter your Comapi access token here
var yourComapiAccessToken = 'YOUR_ACCESS_TOKEN';

// Enter your Comapi profile id for a profile who has already clicked the Send to Messenger control demonstrated in the example web site
// Comapi will automatically lookup the fbMessengerId field from the profile.
var profileId = 'joe.blogs@acme.com';

console.log('');
console.log('Sending to Facebook using Comapi and NodeJS');
console.log('-------------------------------------------');

// Create Comapi RESTful URL with API Space Id in it
var comapiUrl = "https://api.comapi.com/apispaces/" + yourComapiAPISpaceId + "/messages";

// Setup Comapi request JSON
var myRequest = {
    body: 'Your text based message',
    to: { profileId: profileId },
    rules: ['fbMessenger'],
    customBody: {
        fbMessenger: {
            text: 'Pick a color:',
            quick_replies: [
                {
                    content_type: 'text',
                    title: 'Red',
                    payload: 'DEVELOPER_DEFINED_PAYLOAD_FOR_PICKING_RED'
                },
                {
                    content_type: 'text',
                    title: 'Green',
                    payload: 'DEVELOPER_DEFINED_PAYLOAD_FOR_PICKING_GREEN'
                }
            ]
        }
    }
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
console.log('');
console.log('Calling Comapi...');

request(options, function (error, response, body) {
    if (error) throw new Error(error);

    console.log("HTTP status code returned: " + response.statusCode);

    // Check status
    if (response.statusCode == 201) {
        // All ok
        console.log('Message successfully sent via Comapi "One" API');
    }
    else {
        // Something went wrong
        console.log('Something went wrong!');
    }

    console.log(body);
});