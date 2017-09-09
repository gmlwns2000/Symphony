<?php
if(!isset($_POST['PHPSESSID'])){
	echo "sid false";
	exit;
}

session_id($_POST['PHPSESSID']);
session_cache_expire(120);
session_start();

if(!isset($_SESSION['user_index'])){
	echo "login error false";
	exit;
}

$cmd = "DELETE FROM `sym_users` WHERE `sym_users`.`id` = " . mysqli_real_escape_string($conn, $_SESSION['user_index']) . ";";

include("connection.php");

$conn = MakeConnection();
if(!$conn){
	echo "conn false";
	exit;
}

$r = mysqli_query($conn, $cmd);
if($r)
{
	echo "\n true";
}
else
{
	echo mysqli_errno($conn) . ", " . mysqli_error($conn);
	echo "\n false";
}

mysqli_close($conn);
?>