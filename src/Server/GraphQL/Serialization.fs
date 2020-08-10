module Cookbook.Server.GraphQL.Serialization

open Newtonsoft.Json
open Newtonsoft.Json.Linq
open Newtonsoft.Json.Serialization
open Microsoft.FSharp.Reflection
open System.Linq

[<Sealed>]
type OptionConverter() =
    inherit JsonConverter()

    override __.CanConvert(t) =
        t.IsGenericType && t.GetGenericTypeDefinition() = typedefof<option<_>>

    override __.WriteJson(writer, value, serializer) =
        let value =
            if isNull value then null
            else
                let _,fields = FSharpValue.GetUnionFields(value, value.GetType())
                fields.[0]
        serializer.Serialize(writer, value)

    override __.ReadJson(reader, t, _, serializer) =
        let innerType = t.GetGenericArguments().[0]
        let innerType =
            if innerType.IsValueType then (typedefof<System.Nullable<_>>).MakeGenericType([|innerType|])
            else innerType
        let value = serializer.Deserialize(reader, innerType)
        let cases = FSharpType.GetUnionCases(t)
        if isNull value then FSharpValue.MakeUnion(cases.[0], [||])
        else FSharpValue.MakeUnion(cases.[1], [|value|])

let jsonSerializerSettings (converters : JsonConverter seq) =
    JsonSerializerSettings()
    |> (fun s ->
        s.Converters <- converters.ToList()
        s.ContractResolver <- CamelCasePropertyNamesContractResolver()
        s
    )

let private converters : JsonConverter [] = [| OptionConverter() |]
let private jsonSettings = jsonSerializerSettings converters

let serialize d = JsonConvert.SerializeObject(d, jsonSettings)

let deserialize (data : string) =
    let getMap (token : JToken) =
        let rec mapper (name : string) (token : JToken) =
            match name, token.Type with
            | "variables", JTokenType.Object -> token.Children<JProperty>() |> Seq.map (fun x -> x.Name, mapper x.Name x.Value) |> Map.ofSeq |> box
            | name, JTokenType.Array -> token |> Seq.map (fun x -> mapper name x) |> Array.ofSeq |> box
            | _ -> (token :?> JValue).Value
        token.Children<JProperty>()
        |> Seq.map (fun x -> x.Name, mapper x.Name x.Value)
        |> Map.ofSeq
    if System.String.IsNullOrWhiteSpace(data)
    then None
    else data |> JToken.Parse |> getMap |> Some
