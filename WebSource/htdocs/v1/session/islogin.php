<?php
	if(isset($_POST['PHPSESSID'])) 
	{
		session_id($_POST['PHPSESSID']);
		session_cache_expire(120);
		session_start();
		echo "session id is: " . $_POST['PHPSESSID'] . "\n"; 
		print_r($_SESSION);
	}
	else
	{
		echo "false";
		exit;
	}
	
	echo SID . "\n";
	
	if(!isset($_SESSION['user_id']))
	{
		echo "userID false";
		exit;
	}
	
	if(!isset($_SESSION['user_email']))
	{
		echo "userEmail false";
	}
	
	if(!isset($_SESSION['user_index']))
	{
		echo "user_index false";
	}
	
	echo " ID: " . $_SESSION['user_id'] . " EMAIL: ". $_SESSION['user_email'];
	echo "\nlogined, true";
?>