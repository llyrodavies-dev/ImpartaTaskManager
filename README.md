# Task Management System

A simple task management system for users to manage jobs and their tasks. Designed with **Clean Architecture**, **CQRS**, and **Mediator** pattern, this project demonstrates a professional .NET 8 + React full-stack application.

---

## Table of Contents

1. [Description](#description)  
2. [Features](#features)  
3. [Tech Stack](#tech-stack)  
4. [How to Run the Application](#how-to-run-the-application)   
5. [Architecture Notes](#architecture-notes)
6. [Additional Notes](#additional-notes)

---

## Description

This application allows users to:

- Create **Jobs** (units of work)
- Add **Tasks** to Jobs with status tracking (Not Started / In Progress / Blocked / Completed)
- View **Dashboard** with overall job and task summaries
- View all tasks across jobs (Task-centric view)
- Manage user profile, including uploading a profile image
- Authenticate via JWT tokens

The design emphasizes **Clean Architecture principles**, keeping domain logic separate from infrastructure and presentation, making the system **testable, maintainable, and scalable**.

---

## Features

- User registration and authentication with **JWT tokens**
- CRUD operations for Jobs and Tasks
- Dashboard summarizing user’s work:
  - Totals for jobs and tasks
  - Overall completion percentage
  - Jobs needing attention
- Task-centric view for quick access to all tasks
- Profile image upload and display
- Fully typed frontend using React + TypeScript
- Styling with Tailwind + CSS Modules
- Frontend routing with React Router
- Clean separation of concerns with MediatR, CQRS, and repository patterns

---

## Tech Stack

**Backend:**
- .NET 8 Web API
- SQLite
- Entity Framework Core
- ASP.NET Identity
- MediatR (CQRS)
- FluentValidation
- JWT Authentication

**Frontend:**
- React.js + TypeScript
- Vite
- React Router
- Tailwind CSS
- CSS Modules
- Fetch API for backend calls

---

## How to Run the Application

### 1. Clone the repository

### 2. Run the Backend (.NET API)

1. Open the solution in Visual Studio (2022 or 2026)
2. Set the API project as the startup project
3. Restore NuGet packages
4. Run the project
5. The SQLite database will be created automatically using EF migrations
5. The backend will be available at https://localhost:7114.

### 3. Run the Frontend (React + Vite)
1. Open a terminal in TaskManager-Web (frontend folder)
2. Install dependencies:
``` npm install ```
3. Start the development server:
``` npm run dev ```
4. The frontend will run at http://localhost:5173 (or displayed by Vite)
5. The frontend communicates with the backend via fetch requests using JWT authentication.

## Architecture Notes

- Domain Layer – Contains business entities, enums, and domain exceptions. No external dependencies.
- Application Layer – Contains commands, queries, DTOs, services and interfaces. Uses lightweight Mediator for CQRS patterns.
- Infrastructure Layer – Implements repositories, JWT token generation, EF Core persistence, and Identity.
- API Layer – Thin controllers that orchestrate commands/queries. Handles authentication and presentation.
- CQRS is used for separation of reads (Queries) and writes (Commands).
- JWT Authentication ensures secure access to API endpoints.
- Frontend uses React + TypeScript + Vite with Tailwind for styling and CSS Modules for component-scoped styles.

## Additional Notes

- Included Utility.Mediator which I created and use in other personal projects due to the recent changes to MediatR package.
- Included Utility.Filtering which I created and use in other personal projects to handle dynamic filtering using Microsoft.EntityFrameworkCore.DynamicLinq
