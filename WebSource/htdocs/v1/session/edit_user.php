<?php
if(!isset($_POST['PHPSESSID']))
{
	echo "session false";
	exit;
}

if(!isset($_POST['user_index']))
{
	echo "user index false";
	exit;
}

session_id($_POST['PHPSESSID']);
session_cache_expire(120);
session_start();

if(!isset($_SESSION['user_index']))
{
	echo "index login error false";
	exit;
}
if(!isset($_SESSION['user_email']))
{
	echo "email login error false";
	exit;
}

if($_SESSION['user_index'] != $_POST['user_index'])
{
	echo "2 login error false";
	exit;
}

include("connection.php");

$conn = MakeConnection();
if(!$conn)
{
	echo "conn false";
	exit;
}

if(isset($_POST['new_user_email']))
{
	$emailCMD = "UPDATE `sym_users` SET `email` = '" . mysqli_real_escape_string($conn, $_POST['new_user_email']) . "' WHERE `sym_users`.`id` = " . $_SESSION['user_index'];
	$emailR = mysqli_query($conn, $emailCMD);
	
	if($emailR)
	{
		$_SESSION['user_email'] = $_POST['new_user_email'];
		echo "\nemail updated";	
	}
	else
	{
		echo "email query false";
		mysqli_close($conn);
		exit;
	}
}

if(isset($_POST['new_user_pwd']))
{
	$pwdCMD = "UPDATE `sym_users` SET `pwd` = password('" . mysqli_real_escape_string($conn, $_POST['new_user_pwd']) . "') WHERE `sym_users`.`id` = " . $_SESSION['user_index'];
	$pwdR = mysqli_query($conn, $pwdCMD);
	
	if($pwdR)
	{
		echo "\n pass updated";
	}
	else
	{
		echo "\n pass false";
		mysqli_close($conn);
		exit;
	}
}

echo "\ntrue";
mysqli_close($conn);
?>