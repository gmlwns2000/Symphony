<?php
if(!isset($_POST['title'])){
	echo "title false";
	exit;
}
if(!isset($_POST['artist'])){
	echo "artist false";
	exit;
}
if(!isset($_POST['album'])){
	echo "album false";
	exit;
}
if(!isset($_POST['filename'])){
	echo "filename false";
	exit;
}
if(!isset($_POST['PHPSESSID'])){
	echo "ses false";
	exit;
}

session_id($_POST['PHPSESSID']);
session_cache_expire(120);
session_start();

if(!isset($_SESSION['user_index'])){
	echo "login false";
	exit;
}

include("../session/connection.php");

$conn = MakeConnection();
if(!$conn){
	echo "conn false";
	exit;
}

$cmd = "INSERT INTO `" . $mysql_table_song . "` (`id`, `filename`, `title`, `album`, `artist`, `create_time`, `first_uploader`)"
. " VALUES (NULL, '" . mysqli_real_escape_string($conn, $_POST['filename']) . "', '" . mysqli_real_escape_string($conn, $_POST['title']) . "', '" . mysqli_real_escape_string($conn, $_POST['album']) . "', '" . mysqli_real_escape_string($conn, $_POST['artist']) . "', CURRENT_TIMESTAMP, '" . mysqli_real_escape_string($conn, $_SESSION['user_index']) . "')";

$r = mysqli_query($conn, $cmd);
if($r)
{
	$last_id = mysqli_insert_id($conn);
	echo "\n" . $last_id . "\ntrue";
}
else
{
	echo "\n ERRNO: ".mysqli_errno().",".mysqli_error()." false";
}

mysqli_close($conn);
?>