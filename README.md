# Blazor PDF Chat
A Blazor web app demonstrating interaction with PDF documents using AI through a Retrieval-Augmented Generation (RAG) approach.

## Overview
This project showcases:
- **Document Processing**: Load, extract, and chunk PDF documents.
- **Vectorization**: Convert text chunks into vectors for efficient similarity search.
- **Vector Storage**: Store these vectors in a vector database.
- **Query Handling**: When a user asks a question:
  - The query is vectorized.
  - A search for the nearest neighbors in the vector database is performed using cosine similarity.
  - The query prompt is augmented with relevant document snippets to provide accurate responses.

![Blazor PDF Chat screenshot](https://github.com/user-attachments/assets/397e65cb-45b8-4706-b3b2-f30c88fb6bf6)

## Key Features
- **Blazor Implementation**: Frontend built with Blazor for a rich, interactive user experience.
- **Semantic Kernel Vector Store**: Utilizes a Semantic Kernel in-memory vector store implementation, but you can drop in a different vector store connector.
- **Flexible AI Configuration**: Leverages [PredictionGuard.NET](https://www.nuget.org/packages/PredictionGuard.NET) NuGet package, but you can drop in another Microsoft.Extensions.AI abstraction.
