<?php

require_once("JsonHelper.php");

ini_set('display_errors', 'On');

// Enter your credentials here.
$username = "@YourUsername@";
$password = "@YourPassword@";

// Enter your phone number here.
$phonenumber = "447000123123";

$validate = new stdClass();
$validate->Number = $phonenumber;
$validate->IsInternational = true;
$validate->RequiredCountryCode = null;

$result = CallJsonService("POST", "phonenumbervalidation/validatenumber", $username, $password, $validate);
if ($result != null)
{
    switch ($result->NumberStatus)
    {
        case 2:
            echo "<div>You phone is on, we''re sending you a message.</div>";
            break;
        case 3:
            echo "<div>You phone is off, please switch it on and try again in a few minutes.</div>";
            break;
        case 4:
            echo "<div>You phone number appears to be dead.</div>";
            break;
        case 5:
            echo "<div>You phone number isn''t on a mobile network. Have you entered a landline number.</div>";
            break;
        case 6:
            echo "<div>It looks like you entered something that isn''t a phone number.</div>";
            break;
    }

    if ($result->NumberStatus == 2)
    {
        if (!isset($_SESSION['messageId'])) {
            $_SESSION['messageId'] = 0;
        } else {
            $_SESSION['messageId']++;
        }

        $request[0] = new stdClass();
        $request[0]->to = $phonenumber;
        $request[0]->from = "Example";
        $request[0]->body = "Hello, this is a test message";
        $request[0]->deliveryStatusUrl = "https://inbound.example.com/SmsReceipts";
        $request[0]->clientRef = "msg-" . $_SESSION['messageId'];
        $result = CallJsonService("POST", "message/send", $username, $password, $request);

        if ($result != null)
        {
            if ($result[0]->successful == TRUE)
            {
                echo "<div>We sent you a message.</div>";
            }
            else
            {
                echo "<div>Sorry, we couldn't sent you a message.</div>";
                
                foreach ($result[0]->validationFailures as $failure)
                {
                    if ($failure->failureCode == "ToInvalid")
                    {
                        echo "<div>Your phone number doesn't look like a valid number.</div>";
                        break;
                    }
                }
            }
        }
    }
}
?>
