FinanceTracker

FinanceTracker is a full-stack web application for managing personal finances. It allows users to track accounts, transactions, and budget goals, and provides insights on spending vs. budgets. The frontend is built with ASP.NET Core Razor Pages, while the backend is an ASP.NET Core Web API connected to MongoDB.

Features

Accounts Management

Create, update, and delete accounts

Track balances in real-time

Prevent duplicate account names

Transactions

Add, edit, and remove transactions

Automatically updates associated account balances

Prevents transactions exceeding account balance

Budgets

Set budgets for each category

View budget progress and status (On Track, Near Limit, Exceeded)

Prevent duplicate budgets per category

Dashboard / Home Page

Quick overview of total spent and total budgets

Category-wise budget progress

Authentication

User registration and login

JWT-based session management

Tech Stack
Layer	Technology
Backend	ASP.NET Core Web API
Frontend	ASP.NET Core Razor Pages
Database	MongoDB
HTTP Client	HttpClient / JSON.NET
Authentication	JWT
UI Framework	Bootstrap 5, Bootstrap Icons
