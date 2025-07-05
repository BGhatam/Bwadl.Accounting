# Clean Architecture Configuration Guide

## Overview

This guide explains the proper placement and organization of configuration classes in a Clean Architecture solution.

## 🏛️ **Clean Architecture Configuration Layers**

### **1. Domain Layer (`Bwadl.Accounting.Domain`)**
**Purpose**: Core business rules and domain constraints
**Location**: `/src/Bwadl.Accounting.Domain/Settings/`

```csharp
// Example: Business rules
public class BusinessRulesSettings
{
    public int MaxLoginAttempts { get; set; } = 5;
    public int PasswordMinLength { get; set; } = 8;
    public decimal MaxTransactionAmount { get; set; } = 10000;
}
```

**What belongs here:**
- ✅ Business validation rules
- ✅ Domain constraints
- ✅ Business process parameters
- ❌ Framework-specific settings
- ❌ Infrastructure details

### **2. Shared Layer (`Bwadl.Accounting.Shared`)**
**Purpose**: Cross-cutting concerns shared between layers
**Location**: `/src/Bwadl.Accounting.Shared/Configuration/`

```csharp
// Examples of what's currently here:
ApplicationOptions.cs     ✅ App metadata
CacheOptions.cs          ✅ Caching settings
FeatureOptions.cs        ✅ Feature flags
MessageBusOptions.cs     ✅ Messaging settings
SecurityOptions.cs       ✅ Security policies
ResiliencyOptions.cs     ✅ Retry policies (NEW)
```

**What belongs here:**
- ✅ Cross-cutting concerns (caching, logging, resiliency)
- ✅ Feature flags
- ✅ Application-wide settings
- ✅ Policy configurations
- ❌ Infrastructure implementation details
- ❌ API-specific configurations

### **3. Infrastructure Layer (`Bwadl.Accounting.Infrastructure`)**
**Purpose**: Technical implementation details
**Location**: `/src/Bwadl.Accounting.Infrastructure/Configuration/`

```csharp
// Examples:
AuthenticationConfiguration.cs  ✅ JWT implementation details
ConfigurationService.cs         ✅ Configuration service implementation
```

**What belongs here:**
- ✅ Database connection strings (implementation)
- ✅ External service endpoints
- ✅ Framework-specific configurations
- ✅ Infrastructure service implementations
- ❌ Business logic settings
- ❌ API presentation concerns

### **4. API Layer (`Bwadl.Accounting.API`)**
**Purpose**: Presentation layer concerns
**Location**: `/src/Bwadl.Accounting.API/Configuration/`

```csharp
// Examples:
SwaggerConfiguration.cs         ✅ API documentation
HealthCheckConfiguration.cs     ✅ Health check endpoints
ApiVersioningConfiguration.cs   ✅ API versioning
AuthorizeOperationFilter.cs     ✅ Swagger authorization
```

**What belongs here:**
- ✅ Swagger/OpenAPI settings
- ✅ API versioning
- ✅ Health check configurations
- ✅ HTTP-specific settings
- ✅ API documentation settings
- ❌ Business logic
- ❌ Data access concerns

## 📋 **Configuration Placement Rules**

### **Ask These Questions:**

1. **"Is this a business rule?"** → Domain Layer
2. **"Is this used across multiple layers?"** → Shared Layer
3. **"Is this infrastructure/technical implementation?"** → Infrastructure Layer
4. **"Is this API/HTTP specific?"** → API Layer

### **Decision Matrix:**

| Configuration Type | Domain | Shared | Infrastructure | API |
|-------------------|---------|---------|----------------|-----|
| Business Rules | ✅ | ❌ | ❌ | ❌ |
| Validation Rules | ✅ | ❌ | ❌ | ❌ |
| Feature Flags | ❌ | ✅ | ❌ | ❌ |
| Caching Policy | ❌ | ✅ | ❌ | ❌ |
| Retry Policy | ❌ | ✅ | ❌ | ❌ |
| Security Policy | ❌ | ✅ | ❌ | ❌ |
| Database Connection | ❌ | ❌ | ✅ | ❌ |
| External APIs | ❌ | ❌ | ✅ | ❌ |
| JWT Implementation | ❌ | ❌ | ✅ | ❌ |
| Swagger Settings | ❌ | ❌ | ❌ | ✅ |
| API Versioning | ❌ | ❌ | ❌ | ✅ |
| Health Checks | ❌ | ❌ | ❌ | ✅ |

## 🔧 **Configuration Service Pattern**

### **Current Implementation:**

```csharp
// ConfigurationController.cs
[ApiController]
public class ConfigurationController : ControllerBase
{
    private readonly IConfigurationService _configurationService;
    
    // Provides runtime configuration access
    // This is CORRECT - it's an API concern
}
```

### **Purpose:**
- ✅ Runtime configuration access via API
- ✅ Dynamic configuration updates
- ✅ Configuration validation
- ✅ Environment-specific settings

## 🚫 **Anti-Patterns to Avoid**

### **1. Configuration in Application Layer**
```csharp
// ❌ WRONG: Application/Common/Configuration/
// Application layer should not contain configuration classes
// It should only reference configuration from Shared layer
```

### **2. Business Logic in Infrastructure**
```csharp
// ❌ WRONG: Infrastructure/Configuration/BusinessRules.cs
// Business rules belong in Domain layer
```

### **3. Infrastructure Details in Shared**
```csharp
// ❌ WRONG: Shared/Configuration/DatabaseConnectionSettings.cs
// Database specifics belong in Infrastructure layer
```

### **4. Cross-Layer Configuration Mixing**
```csharp
// ❌ WRONG: API/Configuration/RetryPolicy.cs
// Retry policy is cross-cutting, belongs in Shared layer
```

## ✅ **Corrected Structure**

### **Before (Messy):**
```
Application/Common/Configuration/ResiliencySettings.cs  ❌
Infrastructure/Configuration/AuthenticationConfiguration.cs  ✅
API/Configuration/SwaggerConfiguration.cs  ✅
Shared/Configuration/CacheOptions.cs  ✅
```

### **After (Clean):**
```
Domain/Settings/BusinessRulesSettings.cs  ✅
Shared/Configuration/ResiliencyOptions.cs  ✅ (MOVED)
Infrastructure/Configuration/AuthenticationConfiguration.cs  ✅
API/Configuration/SwaggerConfiguration.cs  ✅
```

## 🎯 **Benefits of Correct Structure**

### **1. Clear Separation of Concerns**
- Each layer has its own configuration responsibility
- No circular dependencies
- Clean dependency flow

### **2. Maintainability**
- Easy to find configuration by purpose
- Changes isolated to appropriate layers
- Clear ownership of settings

### **3. Testability**
- Configuration can be mocked per layer
- Unit tests don't require infrastructure setup
- Clear configuration boundaries

### **4. Reusability**
- Shared configurations available across layers
- Domain settings independent of implementation
- Infrastructure settings swappable

## 📖 **Best Practices**

1. **Name consistently**: Use `Options` suffix for shared configurations
2. **Document purpose**: Add XML comments explaining the configuration's role
3. **Validate settings**: Implement validation attributes where appropriate
4. **Use typed configuration**: Avoid `IConfiguration` directly in business logic
5. **Environment awareness**: Support dev/staging/production variations
6. **Default values**: Provide sensible defaults for all settings

The configuration is now properly organized according to Clean Architecture principles! 🏗️
