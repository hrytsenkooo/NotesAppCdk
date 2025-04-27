using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.AppSync;
using System.Collections.Generic;
using Constructs;
using System.IO;

namespace NotesAppCdk
{
    /// <summary>
    /// Defines the resources and infrastructure for the NotesApp application in AWS using AWS CDK.
    /// The stack includes DynamoDB tables for Users and Notes, an AWS Lambda function, 
    /// and an AppSync API to handle GraphQL queries and mutations.
    /// </summary>
    public class NotesAppStack : Stack
    {
        /// <summary>
        /// Initializes the stack, creating the necessary infrastructure components 
        /// such as DynamoDB tables, Lambda function, and AppSync API with resolvers.
        /// </summary>
        /// <param name="scope">The scope in which to define this resource (usually the app).</param>
        /// <param name="id">The ID of the stack.</param>
        /// <param name="props">Optional stack properties.</param>
        public NotesAppStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            var usersTable = new Table(this, "UsersTable", new TableProps
            {
                BillingMode = BillingMode.PAY_PER_REQUEST,
                PartitionKey = new Attribute
                {
                    Name = "Id",
                    Type = AttributeType.STRING
                },
                TableName = "Users",
                RemovalPolicy = RemovalPolicy.DESTROY
            });

            usersTable.AddGlobalSecondaryIndex(new GlobalSecondaryIndexProps
            {
                IndexName = "EmailIndex",
                PartitionKey = new Attribute
                {
                    Name = "Email",
                    Type = AttributeType.STRING
                }
            });

            usersTable.AddGlobalSecondaryIndex(new GlobalSecondaryIndexProps
            {
                IndexName = "UsernameIndex",
                PartitionKey = new Attribute
                {
                    Name = "Username",
                    Type = AttributeType.STRING
                }
            });

            var notesTable = new Table(this, "NotesTable", new TableProps
            {
                TableName = "Notes",
                BillingMode = BillingMode.PAY_PER_REQUEST,
                PartitionKey = new Attribute { Name = "Id", Type = AttributeType.STRING },
                RemovalPolicy = RemovalPolicy.DESTROY 
            });

            notesTable.AddGlobalSecondaryIndex(new GlobalSecondaryIndexProps
            {
                IndexName = "UserIdIndex",
                PartitionKey = new Attribute { Name = "UserId", Type = AttributeType.STRING }
            });

            string lambdaPackagePath = Path.Combine(Directory.GetCurrentDirectory(), "lambda-package.zip");

            var notesLambda = new Function(this, "NotesAppFunction", new FunctionProps
            {
                Runtime = Runtime.DOTNET_8,
                Handler = "NotesApp.Lambda::NotesApp.Lambda.Function::FunctionHandler",
                Code = Amazon.CDK.AWS.Lambda.Code.FromAsset(lambdaPackagePath),
                Timeout = Duration.Seconds(30),
                MemorySize = 512,
                Environment = new Dictionary<string, string>
                {
                    { "USERS_TABLE", usersTable.TableName },
                    { "NOTES_TABLE", notesTable.TableName }
                }
            });

            usersTable.GrantReadWriteData(notesLambda);
            notesTable.GrantReadWriteData(notesLambda);

            string schemaPath = Path.Combine(Directory.GetCurrentDirectory(), "schema.graphql");

            var api = new GraphqlApi(this, "NotesApi", new GraphqlApiProps
            {
                Name = "notes-api",
                Definition = Definition.FromFile(schemaPath),
                AuthorizationConfig = new AuthorizationConfig
                {
                    DefaultAuthorization = new AuthorizationMode
                    {
                        AuthorizationType = AuthorizationType.API_KEY,
                        ApiKeyConfig = new ApiKeyConfig
                        {
                            Expires = Expiration.After(Duration.Days(365))
                        }
                    }
                },
                XrayEnabled = true
            });

            var lambdaDataSource = api.AddLambdaDataSource("LambdaDataSource", notesLambda);


            /// <summary>
            /// Сreate resolvers for all GraphQL queries and mutations.
            /// </summary>
            lambdaDataSource.CreateResolver("getUserById", new BaseResolverProps
            {
                TypeName = "Query",
                FieldName = "getUserById"
            });

            lambdaDataSource.CreateResolver("getAllUsers", new BaseResolverProps
            {
                TypeName = "Query",
                FieldName = "getAllUsers"
            });

            lambdaDataSource.CreateResolver("getNoteById", new BaseResolverProps
            {
                TypeName = "Query",
                FieldName = "getNoteById"
            });

            lambdaDataSource.CreateResolver("getAllNotes", new BaseResolverProps
            {
                TypeName = "Query",
                FieldName = "getAllNotes"
            });

            lambdaDataSource.CreateResolver("getNotesByUserId", new BaseResolverProps
            {
                TypeName = "Query",
                FieldName = "getNotesByUserId"
            });

            lambdaDataSource.CreateResolver("createUser", new BaseResolverProps
            {
                TypeName = "Mutation",
                FieldName = "createUser"
            });

            lambdaDataSource.CreateResolver("updateUser", new BaseResolverProps
            {
                TypeName = "Mutation",
                FieldName = "updateUser"
            });

            lambdaDataSource.CreateResolver("deleteUser", new BaseResolverProps
            {
                TypeName = "Mutation",
                FieldName = "deleteUser"
            });

            lambdaDataSource.CreateResolver("createNote", new BaseResolverProps
            {
                TypeName = "Mutation",
                FieldName = "createNote"
            });

            lambdaDataSource.CreateResolver("updateNote", new BaseResolverProps
            {
                TypeName = "Mutation",
                FieldName = "updateNote"
            });

            lambdaDataSource.CreateResolver("deleteNote", new BaseResolverProps
            {
                TypeName = "Mutation",
                FieldName = "deleteNote"
            });

            new CfnOutput(this, "GraphQLApiURL", new CfnOutputProps
            {
                Value = api.GraphqlUrl
            });

            new CfnOutput(this, "GraphQLApiKey", new CfnOutputProps
            {
                Value = api.ApiKey ?? "No API Key"
            });
        }
    }
}