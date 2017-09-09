<?php
	$mysql_serverAddr = "localhost";
	$mysql_uid = "symphony";
	$mysql_pwd = "Fractalize5176.";
	$mysql_db = "symphony";

	$mysql_table_user = "sym_users";
	$mysql_table_lyric = "sym_lyrics";
	$mysql_table_song = "sym_songs";
	$mysql_table_plot = "sym_plots";

	function MakeConnection()
	{
		$mysql_serverAddr = "localhost";
		$mysql_uid = "symphony";
		$mysql_pwd = "Fractalize5176.";
		$mysql_db = "symphony";

		$conn = mysqli_connect($mysql_serverAddr, $mysql_uid, $mysql_pwd);
		mysqli_select_db($conn, $mysql_db);

		if (!$conn)
		{
			echo "Error: mysql " . PHP_EOL;
			echo "Debugging errno: " . mysqli_connect_errno() . PHP_EOL;
			echo "Debugging error: " . mysqli_connect_error() . PHP_EOL;
		}

		mysqli_query($conn, "set session character_set_connection=utf8;");
		mysqli_query($conn, "set session character_set_results=utf8;");
		mysqli_query($conn, "set session character_set_client=utf8;");

		return $conn;
	}
?>
