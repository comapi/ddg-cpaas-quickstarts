<html>
<head></head>
<body>

<?php
require_once("JsonHelper.php");

ini_set('display_errors', 'On');
session_start();

// Enter your credentials here.
$username = "@YourUsername@";
$password = "@YourPassword@";

$phoneNumber = isset($_POST['phoneNumber']) ? $_POST["phoneNumber"] : "447990766636";
if ($phoneNumber)
{
    $request = new stdClass();
    $request->to = $phoneNumber;
    $request->from = "Example";
    $result = CallJsonService("POST", "verify", $username, $password, $request);

    if ($result != NULL)
    {
        $_SESSION['requestId'] = $result->requestId;
        header('Location: verify.php', true, 302);

        exit();
    }
    else
    {
        echo "<div>Sorry, we couldn't sent you a code.</div>";
        
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
?>
<form action="Register.php" method="POST">
    <p>To complete your registration, please enter your mobile phone number. A verification code will be sent to this number.</p>
    <input type="text" name="phoneNumber" value="447700090000" />
    <input type="submit" value="Register" />
</form>
