using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Driver;
using MongoDB.Bson;

//Not in use
public class DatabaseAccess : MonoBehaviour
{
    private const string MONGO_URI = "";
    private const string DATABASE_NAME = "";
    private MongoClient client;
    private IMongoDatabase db;
    private IMongoCollection<BsonDocument> collection;

    void Start()
    {
        client = new MongoClient(MONGO_URI);
        db = client.GetDatabase(DATABASE_NAME);
        collection = db.GetCollection<BsonDocument>("HandCollection");
    }
}




