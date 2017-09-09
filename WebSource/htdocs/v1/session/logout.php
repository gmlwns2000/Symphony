<?php
	if(isset($_POST['PHPSESSID'])) 
	{
		session_id($_POST['PHPSESSID']);
		session_start();
		echo "session id is: " . $_POST['PHPSESSID'] . "\n"; 
	}
	else
	{
		echo "false";
		exit;
	}
	session_unset();
	session_destroy();
?>