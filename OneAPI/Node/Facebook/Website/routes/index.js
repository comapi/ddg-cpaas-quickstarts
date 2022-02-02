var express = require('express');
var router = express.Router();

/* GET home page route. */
router.get('/', function (req, res, next) {
  // Get the encrypted Facebook metadata for the current user, in this example joe.blogs@acme.com
  generateFacebookMetaData('joe.blogs@acme.com').then(function (response) {
    // Got the Facebook metadata, render the page passing the data
    res.render('pages/index', {
      title: 'Comapi Facebook Tutorial',
      metadata: response
    });
  });
}, function (error) {
  // Failed to retrieve the required metadata for Facebook
  throw error;
});

module.exports = router;

// Generates the secure encrypted metadata to be passed to Facebook so that Comapi can stitch the
// Facebook Messenger Id to the correct profile.
function generateFacebookMetaData(profileId) {
  // Return a new promise.
  return new Promise(function (resolve, reject) {
    // Bring in required dependencies
    var request = require("request");

    // **** ENTER YOUR DETAILS HERE ****

    // Enter your Comapi API Space Id here e.g. 11164198-3f3f-4993-ab8f-70680c1113b1
    var yourComapiAPISpaceId = '01722b21-6c71-4854-bd7e-cf565586c3c3';

    // Enter your Comapi access token here, it must include the Create State For Other Profile from the Facebook permissions category
    var yourComapiAccessToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI3YzQ5NjYxYi0yNTJhLTQ3ZWItOTNiNi05ZmM3YzJkNjY0NmYiLCJpc3MiOiJodHRwczovL2FwaS5jb21hcGkuY29tL2FjY2Vzc3Rva2VucyIsImF1ZCI6Imh0dHBzOi8vYXBpLmNvbWFwaS5jb20iLCJhY2NvdW50SWQiOjM0NzI3LCJhcGlTcGFjZUlkIjoiMDE3MjJiMjEtNmM3MS00ODU0LWJkN2UtY2Y1NjU1ODZjM2MzIiwicGVybWlzc2lvbnMiOlsiY29udGVudDp3IiwiY2hhbjpyIiwiZmI6c3RhdGU6d2EiLCJtc2c6YW55OnMiLCJtc2c6ciIsIm1zZzpicmFuY2giLCJwcm9mOnJhIiwidG1wbDpyIiwiYXBpczpybyJdLCJzdWIiOiI3YzQ5NjYxYi0yNTJhLTQ3ZWItOTNiNi05ZmM3YzJkNjY0NmYiLCJuYW1lIjoiRmFjZWJvb2siLCJpYXQiOjE1OTk3NDI5OTR9.ANvZl0ZL2g9GbN03CFNNYTFU8BOvwIAMOgQXjpX6BkM';

    // Create Comapi RESTful URL with API Space Id in it
    var comapiUrl = "https://api.comapi.com/apispaces/" + yourComapiAPISpaceId + "/channels/facebook/state";

    // Setup Comapi request JSON, any properties in addition to the profileId will be added to the Comapi profile
    var myRequest = {
      profileId: profileId
    };

    // Call Comapi API
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
      if (error) reject(new Error(error));

      // Check status
      if (response.statusCode == 200) {
        // All ok, resolve the promise with the response body which is the metadata
        resolve(body);
      }
      else {
        // Something went wrong
        reject(Error("Call to create Facebook metadata failed with HTTP code: " + response.statusCode));
      }
    });
  });
}