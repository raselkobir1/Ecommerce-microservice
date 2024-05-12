using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Cart.api.Models
{
    public class CartItem
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string? CatalogItemId { get; init; }
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
