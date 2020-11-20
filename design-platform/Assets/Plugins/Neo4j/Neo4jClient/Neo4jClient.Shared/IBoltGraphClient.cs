using Neo4jClient.Cypher;
using Neo4jClient.Execution;
using Neo4jClient.Serialization;
using System;
using System.Threading.Tasks;

namespace Neo4jClient {
    public interface IBoltGraphClient : ICypherGraphClient {
        event OperationCompletedEventHandler OperationCompleted;

        CypherCapabilities CypherCapabilities { get; }

        Version ServerVersion { get; }

        ISerializer Serializer { get; }

        ExecutionConfiguration ExecutionConfiguration { get; }

        bool IsConnected { get; }

        Task ConnectAsync(NeoServerConfiguration configuration = null);
    }
}