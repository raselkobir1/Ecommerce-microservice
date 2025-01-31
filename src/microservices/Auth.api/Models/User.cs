﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Utility.Encryption;

namespace Auth.api.Models
{
    public class User
    {
        public static readonly string DocumentName = nameof(User);

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; init; } = string.Empty;
        public required string Email { get; init; }
        public required string Password { get; set; }
        public string? Salt { get; set; } = string.Empty;
        public bool IsAdmin { get; init; } = true;

        public void SetPassword(string password, IEncryptor encryptor)
        {
            Salt = encryptor.GetSalt();
            Password = encryptor.GetHash(password, Salt);
        }

        public bool ValidatePassword(string password, IEncryptor encryptor) =>
            Password == encryptor.GetHash(password, Salt);
    }
}
