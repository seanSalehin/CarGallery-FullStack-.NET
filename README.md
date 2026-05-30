# Car Gallery

A full-stack ASP.NET Core car gallery application with a clean MVC front end, secure authentication, role-based management, and a versioned Web API.

The application lets users browse cars through a modern gallery interface, while admin users can manage the collection by creating, editing, and deleting car listings.

## Screenshots

### Home Page

![Home Page](https://i.ibb.co/Q3Q2SSdJ/localhost-7154-page-1-filter-By-name-filter-Query-Toyota-sort-By-sort-Order-asc-page-Size-6-3.png)

### Register Page

![Login Page](https://i.ibb.co/9mbPvDWg/Screenshot-2026-05-30-143645.jpg)

### Admin Page

![Edit Car Page](https://i.ibb.co/twt8887D/Screenshot-2026-05-30-143741.jpg)

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
