<?php
//need to post user_id and user_pw0
	include("connection.php");
	
	if(!isset($_POST['user_id'])) 
	{
		echo "id is not posted.";
		print_r($_POST);
		echo "false";
		exit;
	}
	
	if(!isset($_POST['user_pw']))
	{
		echo "pw is not posted.";
		print_r($_POST);
		echo "false";
		exit;
	}
	
	$user_id = $_POST['user_id'];
	$user_pw = $_POST['user_pw'];
	
	$conn = MakeConnection();
	if (!$conn) 
	{
		exit;
	}
	
	$mysql_cmd = "SELECT * FROM `" . $mysql_table_user . "` WHERE `username` LIKE '" . mysqli_real_escape_string($conn, $user_id) . "' AND `pwd` LIKE password('" . mysqli_real_escape_string($conn, $user_pw) . "')";
	
	$r = mysqli_query($conn, $mysql_cmd);
	$row = mysqli_fetch_array($r, MYSQLI_ASSOC);
	
	if($row)
	{
		$user_email = $row["email"];
		$user_index = $row["id"];
		
		session_cache_expire(120);
		session_start();
		
		$_SESSION['user_index'] = $user_index;
		$_SESSION['user_id'] = $user_id;
		$_SESSION['user_email'] = $user_email;
		
		echo session_id() . "\n" . $_SESSION['user_index'] . "\n" . $_SESSION['user_email'] . "\n";
		echo "true";
	}
	else
	{
		echo "false";
	}
	mysqli_close($conn);
?>