var express = require('express');
var request = require('request-promise');
var router = express.Router();

global.verifyRequestId = null;

const _useranme = "**Your username**";
const _password = "**Your password**";

router.get('/', function(req, res){
    res.redirect('/register');
  });

router.get('/register', function(req, res, next) {
    res.render('register');
});

router.post('/register', function(req, res, next) {
    var phoneNumber = req.body.phoneNumber;
    if (phoneNumber)
    {
        request({
            method: 'POST',
            url: 'https://services.dynmark.com/webapi/verify',
            auth: {
                'user': _useranme,
                'pass': _password
            },
            timeout: 130000,
            json: true,
            body: {
                from: "Example",
                to: phoneNumber
            }
        })
        .then(function(body) {
            if (body.numberStatus === "On" || body.numberStatus === "Off")
            {
                global.verifyRequestId = body.requestId;
                res.redirect("verify");
                res.end();
            }
            else
            {
                res.render('register', {message:"Sorry, that number appears to be dead."});
            }
        })
        .catch(function(err)
        {
            var errors = "";
            if (err.statusCode == 401)
            {
                errors = "Invalid credentials";
            }
            else if (err.statusCode == 400)
            {
                if (err.error.some(r => r.failureCode == "ToInvalid"))
                {
                    errors += "Your phone number doesn't look like a valid number.";
                }
                if (err.error.some(r => r.failureCode == "Throttled"))
                {
                    errors += "Please wait for 30 seconds before requesting a resend.";
                }
            }
            else
            {
                errors = "Non success response";
            }
            
            res.render('register', {message:errors});
        })
        .finally(function()
        {
        });
    }
    else
    {
        res.render('register');
    }
});


router.get('/verify', function(req, res, next) {
    res.render('verify');
});

router.post('/verify', function(req, res, next) {
    var code = req.body.code;
    if (code && global.verifyRequestId)
    {
        request({
            method: 'PUT',
            url: 'https://services.dynmark.com/webapi/verify/' + global.verifyRequestId + "/validate",
            auth: {
                'user': _useranme,
                'pass': _password
            },
            timeout: 130000,
            json: true,
            body: {
                code: code
            }
        })
        .then(function(body) {
            if (body && body.status == "CodeVerified")
            {
                res.render('verify', {message:"Your code was correct and is verified."});
            }
            else
            {
                res.render('verify', {message:"Sorry, that code was not recognised. Reason " + (body ? body.status : "CodeUnrecognised")});
            }
        })
        .catch(function(err)
        {
            var errors = "";
            if (err.statusCode == 401)
            {
                errors = "Invalid credentials";
            }
            else if (err.statusCode == 400)
            {
                errors = "Please correct errors";
            }
            else
            {
                errors = "Non success response";
            }
            
            res.render('verify', {message:errors});
        })
        .finally(function()
        {
        });
    }
    else
    {
        res.render('verify');
    }
});

module.exports = router;
