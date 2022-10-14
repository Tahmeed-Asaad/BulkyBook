# BulkyBook

## Topics of ASP.NET Core 6.0 MVC  Covered

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

Following are the User Account that exists by default (Password: Admin123*):
  * Admin User         
  * Employee User         
  * Individual User         
  * Company User  

* Users can register to the website by their Facebook account or by filling up the form
* User can place order after adding items to cart
* Once order is placed one can login as Manager/Admin User and Manage orders and see the flow of application
* In order to place order you can use any test credit card number supported by stripe
* A default example is 4242 4242 4242 4242, valid date , any 3 digit CVV
* Admin user can register new employees/admins for the website
* Admin/Employees can change Order Status to processing/shipped
* When order is cancelled the customers get their money refunded
* Admin can create/read/update/delete new products with proper description and pricing to be displayed on the website
* Individual users has to pay first to complete the order
* Company users(B2B commerce) can pay later(30 days) after completing their order
