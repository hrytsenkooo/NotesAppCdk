# NotesApp Infrastructure (AWS CDK)

This repository contains the infrastructure definition for the **NotesApp** application using **AWS CDK**. This infrastructure provides an easy way to deploy all the necessary resources for the NotesApp backend.

## Overview

The infrastructure stack includes the following components:

- **DynamoDB Tables**:
  - `Users` table: stores user information.
  - `Notes` table: stores notes associated with users.
- **Lambda Function**: handles CRUD operations for users and notes, integrated with the DynamoDB tables.
- **AppSync API**: a GraphQL API that enables interaction with the users and notes data.
- **Resolvers**: connected to the Lambda function to handle GraphQL queries and mutations.

**Note**: for a complete working environment, you'll need to follow the setup guide from the [backend repository](https://github.com/hrytsenkooo/NotesApp) to generate the Lambda package and GraphQL schema (alternatively, you can always use the zip file, which is also added to this repository)

## Prerequisites

Before you start, ensure you have the following tools installed:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [AWS CLI](https://aws.amazon.com/cli/)
- [AWS CDK](https://docs.aws.amazon.com/cdk/latest/guide/work-with-cdk.html)
- [Node.js](https://nodejs.org/) (required for AWS CDK)

Additionally, make sure your **AWS credentials** are configured using the AWS CLI:

```bash
aws configure
```

## Setup and installation

1. **Clone the repository**:

   ```bash
   git clone https://github.com/hrytsenkooo/NotesAppCdk.git
   cd NotesAppCdk

 2. **Install AWS CDK dependencies**:

       Make sure that you have the AWS CDK dependencies installed on your system
       ```bash
        npm install -g aws-cdk

  3. **Deploy the infrastructure**:

       To deploy the infrastructure stack to your AWS account, run the following command:
      
        ```bash
        cdk deploy
        ```

      This will create the necessary resources such as the DynamoDB tables, Lambda function, and AppSync API in your AWS account.

  4. **Verify the infrastructure**:

       After successful deployment, AWS CDK will output the GraphQL API URL and API Key:
      
      ```bash
      GraphQLApiURL: https://<your-api-id>.appsync-api.<region>.amazonaws.com/graphql
      GraphQLApiKey: <your-api-key>
      ```

      You can use the provided URL and API Key to interact with the GraphQL API.

  5. **Clean up**:

       To remove the deployed resources, run the following command
      
      ```bash
      cdk destroy
      ```
      This will delete all resources created by the stack (DynamoDB tables, Lambda function, AppSync API).

Commands for Windows only. Alternatively, you can always use the zip file, which is also added to this repository
