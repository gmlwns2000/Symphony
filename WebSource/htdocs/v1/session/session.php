<?php
function CheckUserExist($uid, $pwd)
{
	include("connection.php");
	
	$conn = MakeConnection();
	if(!$conn)
	{
		return false;
	}
	
	$cmd = "SELECT * FROM `sym_users` WHERE `username` LIKE '" . mysqli_real_escape_string($conn, $uid) . "' AND `pwd` LIKE password('" . mysqli_real_escape_string($conn, $pwd) . "')";
	
	$r = mysqli_query($conn, $cmd);
	
	if(mysqli_num_rows($r) > 0)
	{
		return true;
	}
	else
	{
		print_r ($r);
		return false;
	}
	
	mysqli_close($conn);
}
?>