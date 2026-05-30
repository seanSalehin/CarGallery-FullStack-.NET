# Car Gallery

<p align="center">
  <img src="https://img.shields.io/badge/.NET-ASP.NET_Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
  <img src="https://img.shields.io/badge/C%23-Backend-239120?style=for-the-badge&logo=csharp&logoColor=white" />
  <img src="https://img.shields.io/badge/Bootstrap-UI-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white" />
  <img src="https://img.shields.io/badge/SQL_Server-Database-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" />
  <img src="https://img.shields.io/badge/GitHub-Repository-181717?style=for-the-badge&logo=github&logoColor=white" />
</p>


A full-stack ASP.NET Core car gallery application with a clean MVC front end, secure authentication, role-based management, and a versioned Web API.

The application lets users browse cars through a modern gallery interface, while admin users can manage the collection by creating, editing, and deleting car listings.

## Table of Contents

- [Screenshots](#screenshots)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Main Pages](#main-pages)
- [API Highlights](#api-highlights)
- [Getting Started](#getting-started)
- [Author](#author)


## Screenshots

### Home Page

<p align="center">
  <img src="https://i.ibb.co/Q3Q2SSdJ/localhost-7154-page-1-filter-By-name-filter-Query-Toyota-sort-By-sort-Order-asc-page-Size-6-3.png" alt="Home Page" width="650" />
</p>

### Register Page

<p align="center">
  <img src="https://i.ibb.co/9mbPvDWg/Screenshot-2026-05-30-143645.jpg" alt="Register Page" width="650" />
</p>

### Admin Page

<p align="center">
  <img src="https://i.ibb.co/twt8887D/Screenshot-2026-05-30-143741.jpg" alt="Admin Page" width="650" />
</p>

## Features

* Modern car gallery UI built with ASP.NET Core MVC and Bootstrap
* Public car browsing page with image cards and car details
* Admin dashboard for managing car listings
* Create, edit, and delete car records
* User registration and login
* ASP.NET Core Identity integration
* JWT authentication in the API
* Cookie authentication in the MVC client
* Role-based access control for admin features
* API versioning with versioned routes
* Filtering, sorting, and pagination support
* Uniform API response structure
* DTO-based data transfer
* AutoMapper integration
* Entity Framework Core with SQL Server
* Scalar/OpenAPI documentation
* Responsive design for desktop and mobile screens
* Refresh token support with token rotation
* Refresh token validation for generating new access tokens
* Token family invalidation for improved security
* Protected API calls using bearer tokens
* Automatic token handling between the MVC client and API

## Tech Stack

### Backend

* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* ASP.NET Core Identity
* JWT Bearer Authentication
* AutoMapper
* API Versioning
* Scalar / OpenAPI

### Frontend

* ASP.NET Core MVC
* Razor Views
* Bootstrap
* Bootstrap Icons
* CSS


## Main Pages

### Home Page

The home page displays the car collection in a modern gallery layout. Users can view car images, prices, ratings, and details.

### Admin Dashboard

The dashboard allows admin users to manage the car collection. Admins can add new cars, update existing listings, and delete cars from the system.

### Authentication Pages

The login and registration pages allow users to create an account and sign in. Authenticated users are handled through cookie authentication on the MVC client, while the API uses JWT tokens.

## API Highlights

The authentication system supports JWT access tokens and refresh tokens. Refresh-token rotation is used to issue a new refresh token when the access token is renewed, while invalidating older tokens to reduce the risk of token reuse.

The API includes endpoints for:

* Getting all cars
* Getting a car by ID
* Creating a car
* Updating a car
* Deleting a car
* Registering users
* Logging in users

The car API also supports:

* Filtering by name, details, and rating
* Sorting by name, rating, and price
* Pagination using page and page size query parameters


Example API route:

```text
GET /api/v2/car?page=1&pageSize=10&sortBy=price&sortOrder=asc
```

## Getting Started

### Prerequisites

Make sure you have the following installed:

* .NET SDK
* SQL Server
* Visual Studio or Visual Studio Code

### Setup

Clone the repository:

```bash
git clone https://github.com/YOUR-USERNAME/YOUR-REPOSITORY-NAME.git
```

Open the solution in Visual Studio.

Update the database connection string in `appsettings.json`.

Run database migrations:

```bash
dotnet ef database update
```

Start the API project first, then run the MVC client project.

## Author

Sean Salehin
