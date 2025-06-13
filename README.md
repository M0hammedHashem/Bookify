
# Bookify

# 📚 ASP.NET Core MVC Book Store E-Commerce Application (.NET 9)

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-9.0-512BD4?style=flat-square&logo=dotnet)](https://docs.microsoft.com/en-us/aspnet/core/)
[![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core-512BD4?style=flat-square&logo=dotnet)](https://docs.microsoft.com/en-us/ef/)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?style=flat-square&logo=bootstrap)](https://getbootstrap.com/)

# 🎥 Demo

Experience the BookStore application in action through our comprehensive video demonstration.

**[📺 Watch Live Demo](https://drive.google.com/file/d/1ZobkURmuJ8H90DdJpkRL3r5zBYi4kETS/view?usp=sharing)**

The demo showcases:
- User interface and navigation
- Core bookstore functionality
- Key features and workflows
- Real-time application performance

> **Note**: Click the link above to view the video demonstration on Google Drive.
## 📋 Table of Contents
- [Overview](#overview)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Architecture](#architecture)
- [Getting Started](#getting-started)
- [Installation](#installation)
- [Configuration](#configuration)
- [Database Setup](#database-setup)
- [Usage](#usage)
- [Contributing](#contributing)

## 🎯 Overview

This repository contains a comprehensive E-Commerce BookStore Application built with ASP.NET Core MVC (.NET 9). Featuring complete shopping functionality including user authentication, cart management, order processing, and secure checkout, this project demonstrates enterprise-level architecture patterns using Repository pattern, Dependency Injection, and Entity Framework Core. Perfect for developers seeking to master modern web development with Microsoft technologies or build impressive portfolio projects.
The application is part of the comprehensive course **".NET Core MVC - The Complete Guide 2025 [E-commerce]"** and showcases best practices in building scalable, secure, and maintainable web applications.

## ✨ Features

### 🛍️ E-Commerce Functionality
- **Product Catalog Management**: Browse, search, and filter books by categories, authors, and price ranges
- **Shopping Cart**: Add, remove, and modify items with real-time price calculations
- **Order Management**: Complete order processing workflow from cart to delivery
- **Payment Integration**: Secure payment processing with Stripe integration


### 👤 User Management & Security
- **ASP.NET Core Identity Integration**: Complete user authentication and authorization
- **Role-Based Access Control**: Admin, Customer, Company ,and Employee role management
- **Social Login**: Google and Facebook authentication integration
- **User Profiles**: Extended user information and profile management

### 🎨 User Interface & Experience
- **Responsive Design**: Bootstrap 5 integration for mobile-first responsive UI
- **Modern UI Components**: Custom tag helpers and view components
- **Interactive Elements**: Dynamic content loading and real-time updates


### 🔧 Advanced Features
- **Session Management**: Secure session handling and state management
- **TempData Implementation**: Efficient data transfer between controller actions
- **Partial Views**: Modular and reusable UI components
- **Custom Tag Helpers**: Enhanced HTML generation and form helpers

### 📊 Administrative Features
- **Admin Dashboard**: Comprehensive management interface
- **Content Management**: Books, categories, and author management
- **Order Processing**: Order tracking and fulfillment management
- **User Management**: Customer and employee account administration

## 🛠️ Technology Stack

### Backend Technologies
- **Framework**: ASP.NET Core MVC 9.0
- **Runtime**: .NET 9.0
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **Payment Processing**: Stripe API Integration

### Frontend Technologies
- **UI Framework**: Bootstrap 5.3
- **JavaScript**: Vanilla JS and jQuery
- **CSS**: Custom SCSS/CSS with Bootstrap customization
- **Icons**: Font Awesome and Bootstrap Icons

### Development Tools & Patterns
- **ORM**: Entity Framework Core with Code-First approach
- **Architecture**: Repository Pattern and Dependency Injection
- **Version Control**: Git with conventional commit messages
- **Package Management**: NuGet Package Manager

## 🏗️ Architecture

The application follows the **MVC (Model-View-Controller)** architectural pattern with additional layers for better separation of concerns:


### Project Structure
```
📁 Bookify Solution
├── 📁 Bookify.DataAccess/
│   ├── 📂 Dependencies
│   ├── 📁 Data/
│   │   ├── 📄 DbInitializer.cs
│   │   └── 📄 IDbInitializer.cs
│   ├── 📁 Migrations/           # Entity Framework migrations
│   ├── 📁 Repository/           # Repository pattern implementation
│   └── 📁 ExtensionDACs/        # Data access extensions
│
├── 📁 Bookify.Models/
│   ├── 📂 Dependencies
│   ├── 📁 ViewModels/           # Data transfer objects for views
│   ├── 📄 ApplicationUser.cs    # Extended user model
│   ├── 📄 Category.cs           # Book categories
│   ├── 📄 Company.cs            # Company entities
│   ├── 📄 ErrorViewModel.cs     # Error handling
│   ├── 📄 ModelsExtensions.cs   # Model extensions
│   ├── 📄 OrderDetails.cs       # Order line items
│   ├── 📄 OrderHeader.cs        # Order information
│   ├── 📄 Product.cs            # Book/Product entities
│   ├── 📄 ProductImage.cs       # Product image management
│   └── 📄 ShoppingCarts.cs      # Shopping cart entities
│
├── 📁 Bookify.Utility/
│   ├── 📂 Dependencies
│   └── 🔧 Utility classes and helpers
│
└── 📁 Bookify.Web/              # Main web application
    ├── 🌐 Connected Services    # External service integrations
    ├── 📂 Dependencies
    ├── 🔧 Properties/           # Application properties
    ├── 🌍 wwwroot/              # Static web assets
    │   ├── 🎨 css/              # Stylesheets
    │   ├── 📱 js/               # JavaScript files
    │   └── 🖼️ images/           # Static images
    ├── 📁 Areas/                # MVC Areas for organization
    └── 📁 ViewComponents/       # Reusable view components
    └── 📁 Views/                # MVC views and layouts
    ├── 📄 Program.cs            # Application entry point
    └── 📄 appsettings.json      # Configuration settings
    
```


## 🚀 Getting Started

### Prerequisites
Before running this application, ensure you have the following installed:

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) or [Visual Studio Code](https://code.visualstudio.com/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB or Express)
- [Git](https://git-scm.com/downloads)

### System Requirements
- **OS**: Windows 10/11, macOS 10.15+, or Linux (Ubuntu 18.04+)
- **RAM**: Minimum 4GB, Recommended 8GB+
- **Storage**: 2GB free space
- **Browser**: Chrome, Firefox, Safari, or Edge (latest versions)

## 📦 Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/M0hammedHashem/Bookify.git
   ```


2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Trust the development certificate**
   ```bash
   dotnet dev-certs https --trust
   ```

## ⚙️ Configuration

### Application Settings
Update the `appsettings.json` file with your configuration:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Put_Your_DbConnectionString_Here"

  },
  "Stripe": {
    "SecretKey": "Put_Your_Stripe_SecretKey_Here",
    "PublishableKey": "Put_Your_Stripe_PublishKey_Here"
  },
  "Facebook": {
    "AppId": "Put_Your_Facebook_AppId_Here",
    "AppSecret": "Put_Your_Facebook_AppSecret_Here"
  },
  "GoogleKeys": {
    "ClientID": "Put_Your_Google_ClientID_Here",
    "ClientSecret": "Put_Your_Google_ClientSecret_Here"
  }
}
```

## 🗄️ Database Setup

1. **Update the database connection string** in `appsettings.json`

2. **Run Entity Framework migrations**
   ```bash
   dotnet ef database update
   ```

3. **Seed initial data** (if seed data is configured)
   ```bash
   dotnet run --seed-data
   ```

### Database Schema
The application uses the following main entities:
- **Users**: Customer and admin user accounts
- **Categories**: Book categories and genres
- **Products**: Book inventory and details
- **Orders**: Customer orders and order items
- **ShoppingCarts**: Temporary cart storage
- **Companies**: B2B customer companies

## 🎮 Usage


### Default Accounts
The application seeds the following default accounts:

**Administrator Account:**
- Email: `admin@gmail.com`
- Password: `Admin@123*`

### Key Workflows

1. **Customer Journey**
   - Browse books by category
   - Add items to shopping cart
   - Proceed to checkout
   - Complete payment with Stripe
   - Receive order confirmation

2. **Admin Management**
   - Manage book inventory
   - Process customer orders
   - Manage user accounts
   

## 🤝 Contributing

We welcome contributions! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Development Guidelines
- Follow C# coding conventions
- Write unit tests for new features
- Update documentation for API changes
- Use meaningful commit messages
- Ensure all tests pass before submitting PR


## 🏆 Project Highlights

- **Real-world Application**: Complete e-commerce solution with all essential features
- **Modern Tech Stack**: Latest .NET 9 with current best practices
- **Security First**: Comprehensive security implementation
- **Scalable Architecture**: Enterprise-ready design patterns
- **Mobile Responsive**: Bootstrap 5 responsive design
  
---

# 📖 Learning & Application Journey:
This project represents **months of intensive learning and hands-on application** through the comprehensive course **".NET Core MVC - The Complete Guide 2025 [E-commerce]"** on Udemy. But here's what made the difference - **I didn't just watch, I built alongside every single lesson!**

🔗 **Course Link**: https://www.udemy.com/course/complete-aspnet-core-21-course/?couponCode=LETSLEARNNOW  
👨‍💻 **GitHub Repository**: https://github.com/M0hammedHashem/Bookify

## 💡 My Learning-by-Doing Approach:
🔹 **Learned** MVC architecture → **Applied** it to build modular, scalable components  
🔹 **Studied** Entity Framework → **Implemented** complex database relationships & migrations  
🔹 **Understood** Identity framework → **Built** complete user management system  
🔹 **Learned** Repository pattern → **Applied** it for clean, testable code architecture  
🔹 **Mastered** Stripe integration → **Deployed** real payment processing functionality  
🔹 **Studied** security best practices → **Implemented** role-based authorization  

Every concept I learned, I immediately **coded**, **tested**, and **refined**. This wasn't just theory - it was **building real features that actually work**.

The transformation from following tutorials to **independently solving complex problems** and **debugging real issues** has been incredible. I can now confidently architect, develop, and deploy enterprise-level web applications!

Feel free to explore the complete source code on GitHub - every line represents hours of learning, experimentation, and problem-solving! 🔍

## 🌟 Key Learning & Application Highlights:
• **Deep-dived** into MVC architecture and **applied** it across 15+ controllers  
• **Mastered** Entity Framework relationships and **implemented** 8 complex data models  
• **Learned** advanced authentication patterns and **built** multi-role security system  
• **Studied** payment integration concepts and **deployed** live Stripe functionality  
• **Understood** responsive design principles and **crafted** mobile-first UI/UX  
• **Absorbed** repository patterns and **architected** maintainable, testable code  

## 🎯 The Real Growth:
This wasn't just about completing a course - it was about **transforming theoretical knowledge into practical skills**. I spent countless hours debugging, refactoring, and optimizing. Every error message became a learning opportunity, every successful feature a confidence booster.

**From zero to deployment-ready** - that's the journey this project represents! 🚀

Looking forward to applying these skills in real-world projects and continuing my journey in .NET development! 

**⭐ If you find this project helpful, please consider giving it a star!**

**📚 Happy Learning and Coding!**

