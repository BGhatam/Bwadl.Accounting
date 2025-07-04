using System.ComponentModel;

namespace Bwadl.Accounting.Domain.Common;

public interface IVersionedEntity
{
    /// <summary>
    /// Entity version number for optimistic concurrency control.
    /// </summary>
    int Version { get; set; }
    
    /// <summary>
    /// When the entity was created.
    /// </summary>
    DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Who created the entity.
    /// </summary>
    string CreatedBy { get; set; }
    
    /// <summary>
    /// When the entity was last updated.
    /// </summary>
    DateTime UpdatedAt { get; set; }
    
    /// <summary>
    /// Who last updated the entity.
    /// </summary>
    string UpdatedBy { get; set; }
}
