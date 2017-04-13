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

$code = isset($_POST['code']) ? $_POST["code"] : NULL;
$requestId = isset($_SESSION['requestId']) ? $_SESSION['requestId'] : NULL;

if ($code && $requestId)
{
    $request = new stdClass();
    $request->code = $_POST["code"];
    $result = CallJsonService("PUT", "verify/" . $requestId . "/validate", $username, $password, $request);

    if ($result != null && property_exists($result, "status") && $result->status == "CodeVerified")
    {
        echo "<div>Your code was correct and is verified.</div>";
    }
    else
    {
        echo "<div>Sorry, that code was not recognised. Reason " . ($result == null || !property_exists($result, "status") ? "CodeUnrecognised" : $result->status) . "</div>";
    }
}
?>
<form action="Verify.php" method="POST">
    <p>Please enter the code we sent to complete your registration</p>
    <input type="text" name="code" />
    <input type="submit" value="Verify"/>
</form>
