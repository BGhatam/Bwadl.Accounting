using Bwadl.Accounting.Domain.Common;

namespace Bwadl.Accounting.Domain.Entities;

public class Permission : IVersionedEntity
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public string Resource { get; private set; } = null!;
    public string Action { get; private set; } = null!;
    public bool IsActive { get; private set; } = true;
    
    // IVersionedEntity implementation
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime UpdatedAt { get; set; }
    public string UpdatedBy { get; set; } = null!;

    // Navigation properties
    public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    // Private constructor for EF Core
    private Permission() { }

    // Public constructor
    public Permission(string name, string resource, string action, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Permission name cannot be empty", nameof(name));
        }
        
        if (string.IsNullOrWhiteSpace(resource))
        {
            throw new ArgumentException("Resource cannot be empty", nameof(resource));
        }
        
        if (string.IsNullOrWhiteSpace(action))
        {
            throw new ArgumentException("Action cannot be empty", nameof(action));
        }

        Name = name;
        Resource = resource;
        Action = action;
        Description = description;
        IsActive = true;
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Permission name cannot be empty", nameof(name));
        }
        
        Name = name;
    }

    public void UpdateDescription(string? description)
    {
        Description = description;
    }

    public void UpdateResource(string resource)
    {
        if (string.IsNullOrWhiteSpace(resource))
        {
            throw new ArgumentException("Resource cannot be empty", nameof(resource));
        }
        
        Resource = resource;
    }

    public void UpdateAction(string action)
    {
        if (string.IsNullOrWhiteSpace(action))
        {
            throw new ArgumentException("Action cannot be empty", nameof(action));
        }
        
        Action = action;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    // Predefined system permissions
    public static class SystemPermissions
    {
        // User permissions
        public const string UsersRead = "Users.Read";
        public const string UsersWrite = "Users.Write";
        public const string UsersDelete = "Users.Delete";
        
        // Admin permissions
        public const string AdminRead = "Admin.Read";
        public const string AdminWrite = "Admin.Write";
        public const string AdminDelete = "Admin.Delete";
        
        // System permissions
        public const string SystemRead = "System.Read";
        public const string SystemWrite = "System.Write";
        public const string SystemDelete = "System.Delete";
        
        // API Key permissions
        public const string ApiKeysRead = "ApiKeys.Read";
        public const string ApiKeysWrite = "ApiKeys.Write";
        public const string ApiKeysDelete = "ApiKeys.Delete";
    }
}
