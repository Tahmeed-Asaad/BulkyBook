# BulkyBook

## Table of contents

* [Introduction](#introduction)

* [Topics](#topics) 

* [Features](#features)

* [Technologies](#technologies)

* [A Short Tour of the Project](#a-short-tour-of-the-project)
* [Demo Screenshots](#demo-screenshots)

* [Setup](#setup)

## Introduction

 An e-commerce website built using **ASP.NET Core 6.0 MVC** framework. The web application is an online bookstore/bookshop where users can buy books online with a few clicks and save their time and money. Here customers can view the products and place an order by adding them cart with their credit cards. Admin can then view the orders, process them, and do all the transactions. This web application has both B2B and B2C features. The project is a personal one which was built after going through the [course](https://www.dotnetmastery.com/Home/Details?courseId=9) offered by [Bhrugen Patel](https://github.com/bhrugen). The course is also available on [Udemy](https://www.udemy.com/course/complete-aspnet-core-21-course/?referralCode=0533F3B61F426407BE00) where it was selected for their collection of top-rated courses trusted by famous businesses worldwide like: **Nasdaq**, **NetApp**, **Volkswagen**. 

## Topics 

* N-Tier Architecture
* Repository Pattern and UnitOfWork
* TempData/ViewBag/ViewData in .NET core
* API Controllers with Razor Pages
* SweetAlerts, Rich Text Editor and DataTables with .NET Core
* Scaffold Identity (Razor Class Library) 
* Roles and Authorization in .NET Core
* Stripe Payment/Refund with .NET Core
* Session in .NET Core
* View Components in .NET Core
* Seed Database with DbInitializer

## Features

* 4 types of users: Admin, Employee, Company, Individual
* Users can register to the website by their Facebook account or by filling up the form
* User can place order after adding items to cart
* Once order is placed one can login as Manager/Admin User and Manage orders and see the flow of application
* In order to place order you can use any test credit card number supported by stripe
* A default example is 4242 4242 4242 4242, valid date , any 3 digit CVV
* Admin user can register new employees/admins for the website
* Admin/Employees can change Order Status to processing/shipped
* When order is cancelled the customers get their money refunded
* Admin can create/read/update/delete new products with proper description and pricing to be displayed on the website
* Individual users(B2C commerce) has to pay first to complete the order
* Company users(B2B commerce) can pay later(30 days) after completing their order

 ## Technologies
  * ASP.NET Core 6.0 MVC
  * HTML
  * Javascript
  * CSS
  * SCSS
  * C#
  * Microsoft SQL Server
  * Microsoft Visual Studio
  * Bootstrap v4
  * Identity Framework
  * Stripe API


 ## A Short Tour of the Project
  
<div> 
 
  

 <img src="BulkyBookWeb/wwwroot/images/final_634a0f0e49ae2f00b7916895_644606.gif">

 </div>
 
 
   ## Demo Screenshots
  
<div> 
 
  <h2> Home Page  </h2>

 <img src="BulkyBookWeb/wwwroot/images/Home Page.png">

 </div>


<div>

  <h2>Adding books to cart </h2>
  
<img src="BulkyBookWeb/wwwroot/images/Adding books to cart.png">
 
 </div>
 
 <div>
 
  <h2>Shopping Cart</h2>
  
<img src="BulkyBookWeb/wwwroot/images/Shopping Cart.png">
 
 </div>
 
  <div>
 
  <h2>Finalizing Order </h2>
  
<img src="BulkyBookWeb/wwwroot/images/Finalizing Order.png">
 
 </div>
 
  <div>
 
  <h2>Credit Card Payment </h2>
  
<img src="BulkyBookWeb/wwwroot/images/Credit Card Payment.png">
 
 </div>
 
 
  <div>
 
  <h2>Successful Order </h2>
  
<img src="BulkyBookWeb/wwwroot/images/Successful Order.png">
 
 </div>
 
 ## Setup
 
It is better to use Microsoft Visual Studio IDE to run this project properly. The 'BulkyBook.sln' file should be opened with the IDE to  built the project and to run it.
