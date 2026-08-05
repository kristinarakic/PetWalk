# 🐾 PetWalk - Dog Walking Platform

WPF desktop application that connects dog owners with dog walkers. Built with .NET 10, MVVM architecture, and Entity Framework Core.

## Features

- **User Registration & Login** — Owner and Walker accounts with password masking
- **Dog Management** — Add, edit, and remove dogs (CRUD)
- **Walker Search & Filtering** — Find available walkers by name or location
- **Walker Details** — View completed walks, reviews, and average rating
- **Walk Scheduling** — Schedule walks based on walker availability slots
- **Availability Management** — Walkers set available dates and time ranges
- **Accept/Decline/Start/Complete Walks** — Full walk lifecycle management
- **Review System** — Owners rate and review walkers after completed walks
- **Profile Editing** — Both roles can update their profile information
- **Data Export** — Export walk data to JSON or XML with Save File dialog
- **PDF Reports** — Generate professional walk history reports using QuestPDF
- **Real-time Notifications** — Observer pattern for walk status changes

## Architecture

**Layered Architecture + MVVM Pattern**

## Design Patterns

| Pattern | Implementation |
|---------|---------------|
| **Singleton** | AuthService — single authentication instance |
| **Observer** | Walk (ISubject) notifies OwnerObserver (IObserver) on status change |
| **Repository** | IRepository<T> implemented by UserRepository, DogRepository, WalkRepository |

## Project Structure (MVVM)

PetWalk/
├── Models/ — Domain entities (User, Owner, Walker, Dog, Walk, Review, AvailabilitySlot)
├── ViewModels/ — UI logic, bindings and commands (BaseDashboardViewModel inheritance)
├── Views/ — XAML user interface
├── Services/ — Business logic (AuthService, WalkService, SerializationService, ReportService)
├── Repositories/ — Data access layer (IRepository pattern)
├── Data/ — EF Core DbContext with SQLite
├── Helpers/ — RelayCommand
PetWalk.Tests/ — NUnit unit tests (12 tests)

## Class Hierarchy

BaseViewModel
├── LoginViewModel
├── RegistrationViewModel
└── BaseDashboardViewModel (abstract)
├── OwnerDashboardViewModel
└── WalkerDashboardViewModel

User (abstract)
├── Owner
└── Walker

## Walk Status Flow

Scheduled → Accepted → InProgress → Completed
↘ Declined
↘ Cancelled

## Technologies

- **UI**: WPF (.NET 10), XAML
- **Architecture**: MVVM (Model-View-ViewModel)
- **ORM**: Entity Framework Core
- **Database**: SQLite
- **Testing**: NUnit (12 unit tests)
- **Serialization**: System.Text.Json, XmlSerializer
- **PDF**: QuestPDF
- **CI/CD**: GitHub Actions

## How to Run

1. Clone the repository
2. Open `PetWalk.sln` in Visual Studio 2022/2026
3. Restore NuGet packages
4. Build and run (F5)

## NuGet Packages

- Microsoft.EntityFrameworkCore.Sqlite
- Microsoft.EntityFrameworkCore.Tools
- QuestPDF
- NUnit
- NUnit3TestAdapter

