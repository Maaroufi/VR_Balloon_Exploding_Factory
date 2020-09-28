using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Driver;
using MongoDB.Bson;

public class DatabaseAccess : MonoBehaviour
{
    private const string MONGO_URI = "mongodb+srv://ObiUser:ObiUserPassword@clustertest.kfalz.mongodb.net/HandDB?retryWrites=true&w=majority";
    private const string DATABASE_NAME = "HandDB";
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




