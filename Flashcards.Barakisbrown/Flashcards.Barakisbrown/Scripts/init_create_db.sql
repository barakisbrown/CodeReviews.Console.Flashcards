USE [master]
GO

/****** Object:  Database [Flash Cards]    Script Date: 7/13/2023 5:25:57 PM ******/
CREATE DATABASE [Flash Cards]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Flash Cards', FILENAME = N'C:\Users\barak\Flash Cards.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Flash Cards_log', FILENAME = N'C:\Users\barak\Flash Cards_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Flash Cards].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO