
-- phpMyAdmin SQL Dump
-- version 3.5.2.2
-- http://www.phpmyadmin.net
--
-- Host: localhost
-- Generation Time: Jan 13, 2017 at 08:49 PM
-- Server version: 10.0.28-MariaDB
-- PHP Version: 5.2.17

SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `u737384886_symv1`
--

-- --------------------------------------------------------

--
-- Table structure for table `sym_lyrics`
--

CREATE TABLE IF NOT EXISTS `sym_lyrics` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `song_id` int(11) DEFAULT NULL,
  `user_id` int(11) DEFAULT NULL,
  `time` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci AUTO_INCREMENT=36 ;

--
-- Dumping data for table `sym_lyrics`
--

INSERT INTO `sym_lyrics` (`id`, `song_id`, `user_id`, `time`) VALUES
(3, 9, 1, '2016-07-19 02:38:17'),
(4, 4, 1, '2016-07-22 11:11:21'),
(5, 9, 1, '2016-07-22 11:36:11'),
(6, 9, 1, '2016-07-22 11:37:37'),
(7, 9, 1, '2016-07-23 06:55:21'),
(8, 10, 1, '2016-07-25 20:28:45'),
(9, 10, 1, '2016-07-26 15:42:26'),
(10, 11, 1, '2016-07-27 11:35:48'),
(11, 11, 1, '2016-07-27 14:47:55'),
(12, 11, 1, '2016-07-27 16:25:03'),
(13, 11, 1, '2016-07-27 17:09:59'),
(14, 11, 1, '2016-07-27 17:11:02'),
(15, 10, 1, '2016-07-27 17:25:55'),
(16, 12, 1, '2016-07-27 17:30:17'),
(17, 11, 1, '2016-08-26 15:15:24'),
(18, 11, 1, '2016-08-26 15:17:27'),
(19, 11, 1, '2016-08-26 15:24:06'),
(20, 11, 1, '2016-08-28 05:28:23'),
(21, 11, 1, '2016-08-28 06:03:02'),
(22, 11, 1, '2016-08-28 06:16:26'),
(23, 11, 1, '2016-08-28 06:27:25'),
(24, 11, 1, '2016-08-29 03:59:33'),
(25, 13, 1, '2016-09-01 14:14:37'),
(26, 14, 1, '2016-09-05 08:43:11'),
(27, 15, 1, '2016-10-06 20:17:35'),
(28, 16, 1, '2016-10-18 20:14:13'),
(29, 17, 1, '2016-11-04 11:11:29'),
(30, 18, 1, '2016-11-28 13:47:54'),
(31, 17, 1, '2016-12-27 20:22:07'),
(32, 17, 1, '2016-12-29 14:20:40'),
(33, 17, 1, '2016-12-30 20:23:14'),
(34, 17, 1, '2017-01-12 15:01:35'),
(35, 17, 1, '2017-01-13 13:47:06');

-- --------------------------------------------------------

--
-- Table structure for table `sym_plots`
--

CREATE TABLE IF NOT EXISTS `sym_plots` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `song_id` int(11) DEFAULT NULL,
  `user_id` int(11) DEFAULT NULL,
  `time` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- Table structure for table `sym_songs`
--

CREATE TABLE IF NOT EXISTS `sym_songs` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `filename` varchar(256) COLLATE utf8_unicode_ci DEFAULT NULL,
  `title` varchar(256) COLLATE utf8_unicode_ci DEFAULT NULL,
  `album` varchar(256) COLLATE utf8_unicode_ci DEFAULT NULL,
  `artist` varchar(128) COLLATE utf8_unicode_ci DEFAULT NULL,
  `create_time` datetime DEFAULT NULL,
  `first_uploader` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci AUTO_INCREMENT=19 ;

--
-- Dumping data for table `sym_songs`
--

INSERT INTO `sym_songs` (`id`, `filename`, `title`, `album`, `artist`, `create_time`, `first_uploader`) VALUES
(3, '누덕누덕스타카토.mp3', '누덕누덕스타카토', NULL, NULL, '2016-06-29 19:56:55', 1),
(4, 'ステレオポニ - ツキアカリのミチシルベ.mp3', 'ツキアカリのミチシルベ', 'ツキアカリのミチシルベ [限定版]', '', '2016-07-01 00:13:54', 1),
(5, 'ステレオポニ - 橙色.mp3', '橙色', 'ツキアカリのミチシルベ [限定版]', '', '2016-07-01 00:26:28', 1),
(6, '나는-친구가-적다-OP---念系隣人.mp3', '유감 계 이웃 부 ★ ★ ☆ (별 두개 반) 【 "나는 친구가 적다"오프닝 테마】', '残念系隣人部★★☆（星二つ半）', '', '2016-07-01 00:28:17', 1),
(7, '01.+Be+My+Friend.mp3', 'Be My Friend', 'Be My Friend', '×öìÑÝ»', '2016-07-01 00:30:10', 13),
(8, 'ステレオポニ - fuzz.mp3', 'fuzz', 'ツキアカリのミチシルベ [限定版]', '', '2016-07-02 20:12:31', 1),
(9, 'ゆめこ - 添い遂げたアンドロイドへ.flac', '添い遂げたアンドロイドへ', '添い遂げたアンドロイドへ', 'ゆめこ', '2016-07-19 01:12:39', 1),
(12, 'ぶれないアイで.wav', 'ぶれないアイで', 'VOCALOID', '初音ミク', '2016-07-27 17:30:16', 1),
(11, 'Madeon - Nonsense (feat. Mark Foster).flac', 'Nonsense (feat. Mark Foster)', 'Adventure (Deluxe)', 'Madeon', '2016-07-27 11:35:47', 1),
(13, '리엘 (Lielle) - 죽어버린 별의 넋두리 ED.mp3', '리엘 (Lielle) - 죽어버린 별의 넋두리 ED', '', '', '2016-09-01 14:14:36', 1),
(14, 'サムライハート(Some Like It Hot!!).flac', 'サムライハート(Some Like It Hot!!)', 'サムライハート(Some Like It Hot!!)', 'SPYAIR', '2016-09-05 08:43:11', 1),
(15, '01. Stay Alive.mp3', 'Stay Alive', 'Stay Alive', 'Emilia (CV. Rie Takahashi)', '2016-10-06 20:17:34', 1),
(16, 'Tia-ハートリアライズ.mp3', 'ハートリアライズ', 'ハートリアライズ', 'Tia', '2016-10-18 20:14:11', 1),
(17, '01. daze.flac', 'daze', 'daze/days', 'じん feat.メイリア from GARNiDELiA', '2016-11-04 11:11:29', 1),
(18, 'DECO-27 - Ghost Rule feat. Hatsune Miku.mp3', 'Ghost Rule feat. Hatsune Miku', '', 'DECO*27', '2016-11-28 13:47:53', 1);

-- --------------------------------------------------------

--
-- Table structure for table `sym_users`
--

CREATE TABLE IF NOT EXISTS `sym_users` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `username` varchar(24) CHARACTER SET utf8 COLLATE utf8_bin DEFAULT NULL,
  `pwd` varchar(255) CHARACTER SET utf8 COLLATE utf8_bin DEFAULT NULL,
  `email` varchar(128) CHARACTER SET utf8 COLLATE utf8_bin DEFAULT NULL,
  `register_date` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `reported` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`),
  UNIQUE KEY `username` (`username`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci AUTO_INCREMENT=18 ;

--
-- Dumping data for table `sym_users`
--

INSERT INTO `sym_users` (`id`, `username`, `pwd`, `email`, `register_date`, `reported`) VALUES
(1, 'AinL', '*858BA7C111ABFC4EDF374F96A1DF9B2A59C0CBC0', 'gmlwns5176@naver.com', '2016-10-25 10:39:50', 0),
(2, 'admin', '*858BA7C111ABFC4EDF374F96A1DF9B2A59C0CBC0', 'gmlwns5176@gmail.com', '2016-06-27 08:06:02', 0),
(12, 'gmlwns5176', '*858BA7C111ABFC4EDF374F96A1DF9B2A59C0CBC0', 'gmlwns5176@gmail.com', '2016-06-27 11:56:41', 0),
(13, 'acc2', '*858BA7C111ABFC4EDF374F96A1DF9B2A59C0CBC0', 'test2@gmail.com', '2016-06-30 15:29:46', 0),
(15, 'wns11', '*E56A114692FE0DE073F9A1DD68A00EEB9703F3F1', 'a@aa.com', '2016-07-18 18:45:45', 0),
(16, 'hello', '*858BA7C111ABFC4EDF374F96A1DF9B2A59C0CBC0', 'lanians@gmail.com', '2016-10-04 14:25:43', 0),
(17, 'amoeba', '*5E07E22D077DFAC33185B14C7FFF48164FB2EE7E', 'piskia@naver.com', '2016-11-21 16:11:13', 0);

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
