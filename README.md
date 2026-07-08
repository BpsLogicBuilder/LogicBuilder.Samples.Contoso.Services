# LogicBuilder.Samples.Contoso.Services

[![CI](https://github.com/BpsLogicBuilder/LogicBuilder.Samples.Contoso.Services/actions/workflows/ci.yml/badge.svg)](https://github.com/BpsLogicBuilder/LogicBuilder.Samples.Contoso.Services/actions/workflows/ci.yml)
[![CodeQL](https://github.com/BpsLogicBuilder/LogicBuilder.Samples.Contoso.Services/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/BpsLogicBuilder/LogicBuilder.Samples.Contoso.Services/actions/workflows/github-code-scanning/codeql)
[![codecov](https://codecov.io/gh/BpsLogicBuilder/LogicBuilder.Samples.Contoso.Services/graph/badge.svg?token=MA1I7KK30S)](https://codecov.io/gh/BpsLogicBuilder/LogicBuilder.Samples.Contoso.Services)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=BpsLogicBuilder_LogicBuilder.Samples.Contoso.Services&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=BpsLogicBuilder_LogicBuilder.Samples.Contoso.Services)

A sample server-side workflow application demonstrating the BPS Logic Builder framework with a university management domain (Contoso University). This solution showcases how to build .NET applications where business logic workflows are defined visually using BPS Logic Builder and serialized as `.module` files, enabling a low-code/no-code approach to complex business process automation.

## Overview

This repository demonstrates a multi-tier architecture where:
- **Contoso.Api** - API gateway layer that receives external requests
- **Contoso.Bsl** - Business Service Layer that orchestrates workflow execution
- **Contoso.Bsl.Flow** - Core workflow definitions and execution engine containing serialized workflows (`.module` files)
- Domain and data access layers handle entity models and persistence

## Architecture

### Request Flow
1. Service calls are initiated through the **Contoso.Api** service (API gateway)
2. Requests are forwarded to **Contoso.Bsl** (Business Service Layer)
3. **Contoso.Bsl** executes workflows defined in **Contoso.Bsl.Flow**
4. Workflows interact with repositories and database through Entity Framework Core

### Project Structure

#### **Contoso.Api** (.NET 10)
- Web API gateway service
- Handles external HTTP requests
- Forwards requests to the Business Service Layer
- Uses LogicBuilder JSON serialization for data contracts

#### **Contoso.Bsl** (.NET 10)
- Business Service Layer web application
- Orchestrates workflow execution
- Configures dependency injection for:
  - SQL Server database connectivity
  - AutoMapper profiles
  - Dynamic rules loader
  - LogicBuilder workflow services
- Hosts API controllers that trigger workflows

#### **Contoso.Bsl.Flow** (.NET 10)
- Contains the core workflow engine and definitions
- Workflow logic is serialized in `*.module` files located in the `Rulesets` folder
- Includes workflows for:
  - **Course Management**: `savecourse.module`, `validatecourse.module`, `deletecourse.module`
  - **Student Management**: `savestudent.module`, `validatestudent.module`, `deletestudent.module`
  - **Instructor Management**: `saveinstructor.module`, `validateinstructor.module`, `deleteinstructor.module`
  - **Department Management**: `savedepartment.module`, `validatedepartment.module`, `deletedepartment.module`
  - **Testing**: `comparisontest.module`, `justloop.module`, `callterminate.module`
- References `LogicBuilder.RulesDirector` for workflow execution
- Implements custom actions (e.g., `CustomActions.cs` for logging)
- Module files are embedded as resources in the assembly

#### **Contoso.Domain** (.NET Standard 2.0)
- Domain entities representing the university model:
  - `CourseModel` - Course information with credits and department
  - `StudentModel` - Student data with enrollments
  - `InstructorModel` - Instructor details with course assignments and office
  - `DepartmentModel` - Department with budget and administrator
  - Related models: `EnrollmentModel`, `CourseAssignmentModel`, `OfficeAssignmentModel`
- Decorated with `LogicBuilder.Attributes` for visual workflow designer integration
- Targets .NET Standard 2.0 for maximum compatibility

#### **Contoso.Data** (.NET Standard 2.0)
- Data contracts and interfaces
- Uses `LogicBuilder.Data` package
- .NET Standard 2.0 for shared data definitions

#### **Contoso.Contexts** (.NET 10)
- Entity Framework Core database contexts
- SQL Server database integration
- Contains EF Core configurations

#### **Contoso.Stores** (.NET 10)
- Data access layer using Entity Framework Core
- Repository implementations
- Uses `LogicBuilder.EntityFrameworkCore` package

#### **Contoso.Repositories** (.NET 10)
- Repository pattern implementation
- Abstracts data access from business logic

#### **Contoso.BSL.AutoMapperProfiles** (.NET 10)
- AutoMapper configuration profiles
- Maps between domain entities and DTOs

## Key Technologies

- **.NET 10** - Primary target framework for application layers
- **.NET Standard 2.0** - For domain and data contract layers
- **BPS Logic Builder** - Visual workflow designer and rules engine
  - `LogicBuilder.RulesDirector` - Workflow execution engine
  - `LogicBuilder.Attributes` - Designer integration attributes
  - `LogicBuilder.DataContracts` - Data contract support
- **Entity Framework Core 10** - Data access and SQL Server integration
- **AutoMapper 16** - Object-to-object mapping
- **ASP.NET Core** - Web API hosting

## Workflow Definitions

Workflows are defined using BPS Logic Builder and serialized as `.module` files in the `Contoso.Bsl.Flow/Rulesets` folder. Each workflow is accompanied by a `.resources` file. The workflows handle:

- **CRUD Operations**: Create, Read, Update, Delete for entities
- **Validation**: Business rule validation before persistence
- **Complex Logic**: Multi-step processes, conditional branching, loops

## Getting Started

### Prerequisites
- .NET 10 SDK
- SQL Server (for database)
- BPS Logic Builder (for modifying workflows)

### Configuration
1. Configure the SQL Server connection string in `Contoso.Bsl/appsettings.json`
2. Run database migrations if needed
3. Start the `Contoso.Bsl` service
4. Start the `Contoso.Api` gateway (if using separate hosting)

### Modifying Workflows
Workflow logic is defined in the `.module` files using BPS Logic Builder. To modify:
1. From the FlowProjects folder, open Contoso.BSL.lbproj using BPS Logic Builder.
2. Open the required flow diagrams and make changes as needed.
3. Build the changes in BPS Logic Builder and deploy the resulting `.module` files to the `Contoso.Bsl.Flow` RuleSets folder.
4. Save and rebuild the `Contoso.Bsl.Flow` project to embed the updated modules.

## Benefits of This Architecture

- **Visual Workflow Design**: Business logic is defined visually, not coded
- **Separation of Concerns**: Clear boundaries between API, business logic, and data layers
- **Maintainability**: Workflows can be modified without recompiling application code
- **Type Safety**: Strong typing with .NET entities and LogicBuilder attributes
- **Testability**: Workflows can be tested independently
- **Scalability**: Modular design supports independent scaling of layers

## License

This is a sample project demonstrating BPS Logic Builder capabilities.

## Related Projects

- [BPS Logic Builder](https://github.com/BpsLogicBuilder) - Visual workflow designer and rules engine