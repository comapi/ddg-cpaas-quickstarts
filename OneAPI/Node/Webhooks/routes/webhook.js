var express = require('express');
var router = express.Router();
var cryptoJS = require("crypto-js");
var util = require("util");

/* Comapi Webhook Router. */

/* GET to easily check the page exists */
router.get('/', function(req, res, next) {
  res.render('index', null);
});

router.post('/', function (req, res, next) {
  // Process data received from Comapi
  try {
    // Grab the body and parse to a JSON object
    if (req.body == null) {
      // No body, bad request.
      res.status(400).send("Bad request - No JSON body found!");
      return;
    }

    // We have a request body so lets look at what we have

    // First lets ensure it hasn't been tampered with and it came from Comapi
    // We do this by checking the HMAC from the X-Comapi-Signature header
    let hmac = req.headers["x-comapi-signature"]

    if (hmac == null) {
      // No HMAC, invalid request.
      res.status(401).send("Invalid request: No HMAC value found!");
      return;
    }
    else {
      // Validate the HMAC, ensure you has exposed the rawBody, see app.js for how to do this
      let hash = cryptoJS.HmacSHA1(req.rawBody, ">>>YOUR SECRET<<<");

      if (hmac != hash) {
        // The request is not from Comapi or has been tampered with
        res.status(401).send("Invalid request: HMAC hash check failed!");
        return;
      }
    }

    // Store the received event for later processing, remember you only have 10 secs to process, in this simple example we output to the console
    console.log("");
    console.log(util.format("Received a %s event id: %s",req.body.name, req.body.eventId));
    console.dir(req.body, {depth: null, colors: true});

    // You could use queuing tech such as RabbitMQ, or possibly a distributed cache such as Redis

    // All good return a 200
    res.status(200).send();
  }
  catch (err) {
    // An error occurred
    let msg = "An error occurred receiving data, the error was: " + err;
    console.error(msg);
    res.status(500).send(msg);
  }
});

module.exports = router;