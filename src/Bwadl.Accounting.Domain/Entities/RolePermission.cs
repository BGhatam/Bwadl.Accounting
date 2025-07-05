using Bwadl.Accounting.Domain.Common;

namespace Bwadl.Accounting.Domain.Entities;

public class RolePermission : IVersionedEntity
{
    public int Id { get; private set; }
    public int RoleId { get; private set; }
    public int PermissionId { get; private set; }
    public bool IsActive { get; private set; } = true;
    
    // IVersionedEntity implementation
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime UpdatedAt { get; set; }
    public string UpdatedBy { get; set; } = null!;

    // Navigation properties
    public Role Role { get; private set; } = null!;
    public Permission Permission { get; private set; } = null!;

    // Private constructor for EF Core
    private RolePermission() { }

    // Public constructor
    public RolePermission(int roleId, int permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
        IsActive = true;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
