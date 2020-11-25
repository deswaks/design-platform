using Neo4jClient.Cypher;
using System;

namespace Neo4jClient {
    public interface ICypherGraphClient : IDisposable {
        ICypherFluentQuery Cypher { get; }
    }
}