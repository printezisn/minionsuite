# MinionSuite

A dotnet CLI tool with scaffolding generators to make everyday development easier.

## Installation

The CLI tool is inside the [MinionSuite/MinionSuite.Tool](MinionSuite/MinionSuite.Tool) folder. You can install it by following the [instructions](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools-how-to-create) on how to install a dotnet CLI tool.
For example, here are the instructions on how to install it as a global tool.

**Navigate to the folder**

```
cd MinionSuite/MinionSuite.Tool
```

**Package the tool**

```
dotnet build
dotnet pack
```

**Install the tool**

```
cd ..
dotnet tool install -g --add-source ./MinionSuite.Tool/nupkg MinionSuite.Tool
```

## Usage

You can use the tool by invoking the `minionsuite` command and passing a generator as a parameter. Below you can find a list of available generators.

### Service generator

Generates a service layer based on a model class. The service layer contains an interface and a class implementation. Please note that the ORM used is Entity Framework Core.

Usage: `minionsuite servicegen [parameters]`
Parameters:
- `-m <path>` or `--model-path <path>`: The path to the model class.
- `-ns <name>` or `--namespace <name>`: The namespace of the generated classes.
- `-o <path>` or `--output <path>`: The path to the output folder (default: .).
- `-gpm` or `--generate-page-model`: Generate page model.
- `-grm` or `--generate-result-model`: Generate result model.
- `-db <name>` or `--db-context <name>`: The database context class.

Example: `minionsuite servicegen -m ./Models/Post.cs -ns Example.Services -db ApplicationContext`

The layer provides the following asynchronous methods:
- **CreateAsync**: Creates a new entity.
- **DeleteAsync**: Deletes an entity.
- **GetAsync**: Returns an entity.
- **GetAllAsync**: Returns a list of entities. The result is paged and sorted based on a sorting field.
- **SearchAsync**: Searches for entities based on a string term. The result is paged and sorted based on a sorting field.
- **UpdateAsync**: Updates an entity.

### Service test generator

Generates tests for the service layer based on a model class.

Usage: `minionsuite servicegen:test [parameters]`
Parameters:
- `-m <path>` or `--model-path <path>`: The path to the model class.
- `-ns <name>` or `--namespace <name>`: The namespace of the generated classes.
- `-o <path>` or `--output <path>`: The path to the output folder (default: .).
- `-db <name>` or `--db-context <name>`: The database context class.

Example: `minionsuite servicegen:test -m ./Models/Post.cs -ns Example.Services -db ApplicationContext`

### Page model generator

Generates a model to handle paging for queryables.

Usage: `minionsuite pagemodel [parameters]`
Parameters:
- `-ns <name>` or `--namespace <name>`: The namespace of the generated class.
- `-o <path>` or `--output <path>`: The path to the output folder (default: .).

### Result model generator

Generates a model that represents the result of a service operation.

Usage: `minionsuite resultmodel [parameters]`
Parameters:
- `-ns <name>` or `--namespace <name>`: The namespace of the generated class.
- `-o <path>` or `--output <path>`: The path to the output folder (default: .).

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
