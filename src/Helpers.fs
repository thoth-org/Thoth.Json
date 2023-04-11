namespace Thoth.Json

[<AutoOpen>]
module internal Helpers =

    let getTypeName (t: System.Type) : TypeName =
        (t.GetGenericTypeDefinition()).FullName
        |> TypeName

    let typeNameToString (TypeName str) = str
