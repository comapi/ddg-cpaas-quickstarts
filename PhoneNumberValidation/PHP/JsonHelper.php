<?php
function CallJsonService($method, $servicePath, $username, $password, $requestObject)
{
	$jsonData = json_encode($requestObject);
	$ch = curl_init();
	$options = array(
        CURLOPT_CUSTOMREQUEST   => $method,
		CURLOPT_RETURNTRANSFER 	=> true,     // return web page
		CURLOPT_HEADER         	=> false,    // don't return headers
		CURLOPT_ENCODING       	=> "",       // handle all encodings
		CURLOPT_CONNECTTIMEOUT 	=> 10,       // timeout on connect
		CURLOPT_TIMEOUT        	=> 130,      // timeout on response
		CURLOPT_HTTPHEADER		=> array(
			"Content-type: application/json",
			"Connection: close",
			"Content-length: " . strlen($jsonData),
			"Accept: application/json",
			"Authorization: Basic " . base64_encode($username . ":" . $password)),
		CURLOPT_URL				=> "https://services.dynmark.com/WebApi/" . $servicePath,
		CURLOPT_POSTFIELDS		=> $jsonData
	);
	curl_setopt_array($ch, $options);

	$response = curl_exec($ch);
	$status = curl_getinfo($ch, CURLINFO_HTTP_CODE);
	if ($status == 0)
	{
			print curl_error($ch);
			print "";
	}
	
	curl_close($ch);
	
	if ($status == 204)
	{
		return new stdClass();
	}
	else if ($status >= 200 && $status < 300)
	{
		return json_decode($response);
	}
	else if ($status == 401)
	{
		echo "<div>Invalid credentials</div>";
	}
	else if ($status == 400)
	{
		echo "<div>Bad request format</div>";
		return json_decode($response);
	}
	else
	{		
		echo "<div>Non success response (" . $status . ") - " . $response . "</div>";
	}
}
?>