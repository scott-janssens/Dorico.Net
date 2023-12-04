using DoricoNet.Attributes;
using DoricoNet.DataStructures;

namespace DoricoNet.Responses;

/// <summary>
/// Contains a list of entities in a library collection in a score. May contain 
/// some additional details for each entity depending on the collection type.
/// </summary>
/// <param name="OpenScoreId">The ID of the score to which these entities belong</param>
/// <param name="LibraryEntities">The list of entities</param>
[ResponseMessage("libraryentitieslist")]
public record LibraryEntitiesListResponse(int OpenScoreId, LibraryEntityCollection LibraryEntities) : DoricoResponseBase;
