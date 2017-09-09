<?php
	if(!isset($_POST['filename'])){
		echo "filename x false";
		exit;
	}
	if(!isset($_POST['title'])){
		echo "title x false";
		exit;
	}
	if(!isset($_POST['album'])){
		echo "album x false";
		exit;
	}
	if(!isset($_POST['artist'])){
		echo "album x false";
		exit;
	}
	
	include("../session/connection.php");

	$conn = MakeConnection();
	if(!$conn){
		echo "conn false";
		exit;
	}
	
	$cmd = "SELECT * FROM `" . $mysql_table_song . "` WHERE `filename` LIKE '%" . mysqli_real_escape_string($conn, $_POST['filename']) . "%' OR `title` LIKE '%" . mysqli_real_escape_string($conn, $_POST['title']) . "%' OR `album` LIKE '%" . mysqli_real_escape_string($conn, $_POST['album']) . "%' OR `artist` LIKE '%" . mysqli_real_escape_string($conn, $_POST['artist']) . "%'";
	
	$fetch = mysqli_query($conn, $cmd); 
	
	if(!$fetch){
		echo "fetch false";
		exit;
	}
	
	$return_arr = array();

	while ($row = mysqli_fetch_array($fetch, MYSQL_ASSOC)) 
	{
		$row_array['id'] = $row['id'];
		$row_array['filename'] = $row['filename'];
		$row_array['title'] = $row['title'];
		$row_array['album'] = $row['album'];
		$row_array['artist'] = $row['artist'];
		$row_array['create_time'] = $row['create_time'];
		$row_array['first_uploader'] = $row['first_uploader'];
		
		array_push($return_arr,$row_array);
	}
	
	mysqli_close($conn);

	echo json_encode($return_arr);
?>