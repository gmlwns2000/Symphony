<?php
include("session.php");

if(!isset($_POST['user_id'])){
	echo "user id false";
	exit;
}
if(!isset($_POST['user_pwd'])){
	echo "user pwd false";
	exit;
}

if(CheckUserExist($_POST['user_id'], $_POST['user_pwd'])){
	echo "\ntrue";
}else{
	echo "\nfalse";
}
?>