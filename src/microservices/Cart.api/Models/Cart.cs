﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Cart.api.Models
{
    public class Cart
    {
        public static readonly string DocumentName = nameof(Cart);

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; init; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string? UserId { get; init; }
        public List<CartItem> CartItems { get; init; } = new();
    }
}
