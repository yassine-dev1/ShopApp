# 🚀 AI-Powered E-Commerce Platform (.NET + Redis + RAG)

A modern **AI-powered e-commerce platform** built with **ASP.NET Core (.NET 8)** that integrates:

* ⚡ **High-performance caching with Redis**
* 🤖 **AI conversational assistant**
* 🧠 **RAG (Retrieval Augmented Generation)** for contextual responses based on product data
* 🛒 **Dynamic shopping cart management**
* 📦 **Complete product catalog management**

This project demonstrates the integration of **modern .NET backend architecture**, **distributed caching**, and **LLM-powered AI** to build an intelligent e-commerce experience.

---
![.NET](https://img.shields.io/badge/.NET-8-purple)
![Redis](https://img.shields.io/badge/Redis-Cache-red)
![AI](https://img.shields.io/badge/AI-RAG-green)
![LLM](https://img.shields.io/badge/LLM-HuggingFace-yellow)
![Architecture](https://img.shields.io/badge/Architecture-Clean-orange)

---

# 🧠 AI Assistant with RAG

The application includes an **AI chatbot** powered by **Hugging Face models** and a **RAG (Retrieval Augmented Generation)** pipeline.

## RAG Pipeline

User Question
↓
Product Retrieval Service
↓
Context Builder (PromptContext)
↓
LLM (HuggingFace Model)
↓
AI Response

    ### Advantages
      - Accurate responses
      - Real-time product information
      - Reduced hallucinations
      - Context-aware recommendations

## How it Works

1. The user asks a question in the chat interface
2. The system retrieves relevant information from the **product catalog**
3. This information is injected into the **LLM prompt**
4. The model generates a **contextual and accurate response**
5. The user receives a response based on **real product data**
6. response is in the french language

## Example Questions

* *"Which laptops are currently in stock?"*
* *"Can you recommend a similar product?"*
* *"What is the price of this product?"*

Thanks to the **RAG architecture**, responses are:

* Based on **real database data**
* More **accurate**
* Less prone to **LLM hallucinations**

---

# 🛠 Tech Stack

| Technology                  | Purpose                                 |
| --------------------------- | --------------------------------------- |
| **.NET 8 / ASP.NET Core**   | Backend framework                       |
| **Razor Pages**             | Web UI                                  |
| **Entity Framework Core**   | ORM for database access                 |
| **SQL Server **             | Main relational database                |
| **Redis**                   | Distributed cache for cart management   |
| **Hugging Face API**        | Large Language Model (LLM)              |
| **RAG Architecture**        | Contextual AI based on application data |

---

# ⚙️ Features

## 📦 Product Catalog Management

* Full **CRUD operations** for products
* Full **CRUD operations** for categories
* **Stock management**
* **Product detail visualization**

---

## 🛒 Smart Shopping Cart (Redis)

The shopping cart is stored in **Redis** to ensure:

* ⚡ Very fast data access
* 📉 Reduced load on the main database
* 🔄 Temporary session persistence

### Cart Operations

* Add product to cart
* Remove product from cart
* Update product quantity
* View cart contents

---

## 🤖 AI Assistant

Integrated chatbot providing:

* Automated **customer support**
* **Natural language product search**
* **Smart product recommendations**
* **Personalized purchasing advice**

---

# 🏗 Project Architecture

The project follows a **modular architecture inspired by Clean Architecture principles**, ensuring:

* Maintainability
* Scalability
* Clear separation of responsibilities

## Core Architecture Overview

Services
├── AI
│    ├── LlmService.cs
│    ├── ProductRetrievalService.cs
│    └── PromptContext.cs
│
├── RedisManagement
│    ├── CartRedisService.cs
│    └── ICartService.cs
│
└── ServiceDAO
├── PanierContent.cs
└── ProduitDAO.cs

Data
└── WebApplication1Context.cs

Module
├── Product.cs
├── Category.cs
├── CartItem.cs
└── CartItemCache.cs

Pages
├── Products
├── Categories
├── Cart
├── ChatAI
└── ProductClient

---

# 📂 Backend Services

| Service                     | Description                                 |
| --------------------------- | ------------------------------------------- |
| **LlmService**              | Handles communication with the LLM          |
| **ProductRetrievalService** | Retrieves product data for the RAG pipeline |
| **PromptContext**           | Builds contextual prompts sent to the LLM   |
| **CartRedisService**        | Manages shopping cart storage in Redis      |
| **ProduitDAO**              | Handles product database operations         |

---

# ⚙️ Configuration

## Prerequisites

Before running the project, install the following tools:

* **Visual Studio 2022 or later**
* **.NET 8 SDK**
* **SQL Server **
* **Redis Server**
* **Hugging Face API Key**

---

# 🗄 Database Configuration

Create a database and configure the connection string in:

appsettings.json

Example:

"ConnectionStrings": {
"WebApplication1Context": "Server=localhost;Database=EcommerceAI;Trusted_Connection=True;"
}

---

# ⚡ Redis Cart Workflow

The shopping cart system uses Redis for ultra-fast access.

Client Action
↓
Cart Controller
↓
Redis Cart Service
↓
Redis Cache Storage

    ## Benefits:

    - extremely fast cart operations

    - reduced database load

    - scalable session management

---

# ⚡ Redis Configuration

Start Redis locally:

redis-server

If needed, configure the Redis port inside:

Program.cs

---

# 🔑 Hugging Face Configuration

Add your Hugging Face API key to the project configuration in appsettings.json file.

Example environment variable:

HuggingFaceAI :
                {
                    ApiKey : your_Key
                }
            

---

# 🚀 Installation

## 1. Clone the Repository

git clone https://github.com/yassine-dev1/Shop-App.git

cd Shop-App

---

## 2. Restore Dependencies

dotnet restore

---

## 3. Apply Database Migrations

dotnet ef database update

---

## 4. Run the Application

dotnet run

The application will be available at:

http://localhost:5000

---

# 🔮 Future Improvements

* 🔍 Semantic search using **vector databases**
* 📊 Advanced **recommendation systems**
* 🧠 Product **embeddings**
* 📱 Mobile API support
* ☁ Deployment using **Docker / Kubernetes**

---

# 👨‍💻 Author

**Yassine EL JARJINI**

Software Engineering Student

### Skills

* .NET / ASP.NET Core
* Software Architecture
* AI Integration
* Distributed Systems
* Redis Caching
* LLM Applications

---

# ⭐ Project Goal

The objective of this project is to demonstrate the integration of:

* Modern **backend architecture**
* **Distributed caching**
* **LLM-based AI systems**
* **Retrieval Augmented Generation (RAG)**
* **Conversational AI applied to e-commerce**

within a **scalable and production-ready application**.
