<?php
if(!isset($_POST['tablename'])){
	echo "tablename false";
	exit;
}
if(!isset($_POST['songid'])){
	echo "songid false";
	exit;
}
if(!is_numeric($_POST['songid'])){
	echo "id is not number false";
	exit;
}

include("../session/connection.php");

$conn = MakeConnection();
if(!$conn){
	echo "conn false";
	exit;
}

$cmd = "SELECT * FROM `" . mysqli_real_escape_string($conn, $_POST['tablename']) . "` WHERE `song_id` = " . mysqli_real_escape_string($conn, $_POST['songid']) . " ORDER BY `" . mysqli_real_escape_string($conn, $_POST['tablename']) . "`.`time` DESC";

$r = mysqli_query($conn, $cmd);

if($r){
	$return_arr = array();

	while ($row = mysqli_fetch_array($r, MYSQL_ASSOC)) 
	{
		$row_array['id'] = $row['id'];
		$row_array['song_id'] = $row['song_id'];
		$row_array['user_id'] = $row['user_id'];
		$row_array['time'] = $row['time'];
		
		array_push($return_arr,$row_array);
	}
	
	echo json_encode($return_arr);
}else{
	print_r($r);
	echo "false";
}

mysqli_close($conn);
?>