<?php
	$uploaddir = '../upload/lyric/';
	
	//input check
	if(!isset($_POST['user_index'])){
		echo "userindex false";
		exit;
	}
	if(!isset($_POST['song_index'])){
		echo "songindex false";
		exit;
	}
	if(!is_numeric($_POST['song_index'])){
		echo "non int songindex false";
		exit;
	}
	if(!is_numeric($_POST['user_index'])){
		echo "non int userindex false";
		exit;
	}
	if(!isset($_POST['PHPSESSID'])){
		echo "unable session false";
		exit;
	}
	
	session_id($_POST['PHPSESSID']);
	session_cache_expire(120);
	session_start();
	
	//login check
	if(!isset($_SESSION['user_index'])){
		echo "userid false";
		exit;
	}
	
	if($_SESSION['user_index'] != $_POST['user_index']){
		print_r($_SESSION);
		print_r($_POST);
		
		echo "login error false";
		exit;
	}
	
	//start commit
	include("../session/connection.php");
	
	$success = false;
	$conn = MakeConnection();
	if(!$conn){
		echo "conn false";
		exit;
	}
	
	mysqli_query($conn, "SET AUTOCOMMIT=0");
	mysqli_query($conn, "BEGIN");
	
	$cmd = "INSERT INTO `sym_lyrics` (`id`, `song_id`, `user_id`, `time`) VALUES (NULL, '" . $_POST['song_index'] . "', '" . mysqli_real_escape_string($conn, $_POST['user_index']) . "', CURRENT_TIMESTAMP)";
	mysqli_query($conn, $cmd);
	
	$last_id = mysqli_insert_id($conn);
	
	//file commiting
	echo "\nFile Name : " . $_FILES["file"]["name"];
	echo "\nFile Size (Bytes) : " . $_FILES["file"]["size"];
	echo "\nFile MIME Type : " . $_FILES["file"]["type"];
	echo "\nTemp File Name : " . $_FILES["file"]["tmp_name"];
	
	$dest = $uploaddir . $last_id . ".lyric";
	echo "\nCopy File To " . $dest;
	
	if(!move_uploaded_file(realpath($_FILES["file"]["tmp_name"]), $dest))
	{
		print_r($_FILES);
		
		echo "\nfile false";
		$success = false;
	}
	else
	{
		echo "\nFile ".$_FILES['file']['name']." uploaded successfully. CODE:true";
		$success = true;
	}
	
	//end transaction
	if($success)
	{
		mysqli_query($conn, "COMMIT");
		echo "\ncommited true";
	}
	else
	{
		mysqli_query($conn, "ROLLBACK");
		echo "\nrollbacked false";
	}
	
	mysqli_close($conn);
?>