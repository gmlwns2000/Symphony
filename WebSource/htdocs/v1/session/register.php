<?php
if(!isset($_POST['user_id'])){
	echo "uid false";
	exit;
}
if(!isset($_POST['user_pwd'])){
	echo "upwd false";
	exit;
}
if(!isset($_POST['user_email'])){
	echo "uemail false";
	exit;
}

include("connection.php");

$conn = MakeConnection();
if(!$conn){
	echo "conn false";
	exit;
}

$cmd = "INSERT INTO `sym_users` (`id`, `username`, `pwd`, `email`, `register_date`, `reported`) VALUES (NULL, '" . mysqli_real_escape_string($conn, $_POST['user_id']) . "', password('" . mysqli_real_escape_string($conn, $_POST['user_pwd']) . "'), '" . mysqli_real_escape_string($conn, $_POST['user_email']) . "', CURRENT_TIMESTAMP, '0')";

$r = mysqli_query($conn, $cmd);
if($r)
{
	echo "true";
}
else
{
	print_r($r);
	echo "\n ERRNO: ".mysqli_errno($conn).",".mysqli_error($conn)." false";
}

mysqli_close($conn);
?>