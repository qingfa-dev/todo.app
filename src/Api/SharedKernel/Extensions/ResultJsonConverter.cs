using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

using Todo.Api.SharedKernel.Models;

namespace Todo.Api.SharedKernel.Extensions;

public sealed class ResultConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(Result).IsAssignableFrom(typeToConvert);
    }

    public override JsonConverter CreateConverter(
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (typeToConvert.IsGenericType &&
            typeToConvert.GetGenericTypeDefinition() == typeof(PagedResult<>))
        {
            var itemType = typeToConvert.GetGenericArguments()[0];
            return (JsonConverter)Activator.CreateInstance(
                typeof(PagedResultJsonConverter<>).MakeGenericType(itemType))!;
        }

        if (typeToConvert.IsGenericType &&
            typeToConvert.GetGenericTypeDefinition() == typeof(Result<>))
        {
            return (JsonConverter)Activator.CreateInstance(
                typeof(ResultJsonConverter<>).MakeGenericType(
                    typeToConvert.GetGenericArguments()[0]))!;
        }

        return new NonGenericResultJsonConverter();
    }
}

internal sealed class NonGenericResultJsonConverter : JsonConverter<Result>
{
    public override Result? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(
        Utf8JsonWriter writer,
        Result value,
        JsonSerializerOptions options)
    {
        if (value.IsSuccess)
        {
            writer.WriteStartObject();
            writer.WriteBoolean("isSuccess", true);
            writer.WriteEndObject();
        }
        else
        {
            writer.WriteStartObject();
            writer.WriteBoolean("isSuccess", false);
            writer.WritePropertyName("errors");
            JsonSerializer.Serialize(writer, value.Errors, options);
            writer.WriteEndObject();
        }
    }
}

internal sealed class ResultJsonConverter<TValue> : JsonConverter<Result<TValue>>
{
    public override Result<TValue>? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(
        Utf8JsonWriter writer,
        Result<TValue> value,
        JsonSerializerOptions options)
    {
        if (value is PagedResult<TValue> paged)
        {
            new PagedResultJsonConverter<TValue>().Write(writer, paged, options);
            return;
        }

        if (value.IsSuccess)
        {
            JsonSerializer.Serialize(writer, value.Value, options);
        }
        else
        {
            writer.WriteStartObject();
            writer.WriteBoolean("isSuccess", false);
            writer.WritePropertyName("errors");
            JsonSerializer.Serialize(writer, value.Errors, options);
            writer.WriteEndObject();
        }
    }
}

internal sealed class PagedResultJsonConverter<T> : JsonConverter<PagedResult<T>>
{
    public override PagedResult<T>? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(
        Utf8JsonWriter writer,
        PagedResult<T> value,
        JsonSerializerOptions options)
    {
        if (value.IsSuccess)
        {
            writer.WriteStartObject();
            writer.WriteNumber("page", value.Page);
            writer.WriteNumber("pageSize", value.PageSize);
            writer.WriteNumber("totalCount", value.TotalCount);
            writer.WriteNumber("totalPages", value.TotalPages);
            writer.WriteBoolean("hasPreviousPage", value.HasPreviousPage);
            writer.WriteBoolean("hasNextPage", value.HasNextPage);
            writer.WritePropertyName("items");
            JsonSerializer.Serialize(writer, value.Items, options);
            writer.WriteEndObject();
        }
        else
        {
            writer.WriteStartObject();
            writer.WriteBoolean("isSuccess", false);
            writer.WritePropertyName("errors");
            JsonSerializer.Serialize(writer, value.Errors, options);
            writer.WriteEndObject();
        }
    }
}
