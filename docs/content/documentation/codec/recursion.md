---
title: Recursion
---

A codec for a type that refers to itself can't be written as a plain `let` binding: the codec would
have to exist before it is defined.

`Codec.fix` hands you the codec being defined, so you can use it inside its own definition.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

type Tree = Tree of value: int * children: Tree list

module Tree =

    let codec: Codec<Tree> =
        Codec.fix (fun self ->
            objectCodec {
                let! value = Codec.field "value" (fun (Tree(v, _)) -> v) Codec.int

                and! children =
                    Codec.field
                        "children"
                        (fun (Tree(_, c)) -> c)
                        (Codec.list self)

                return Tree(value, children)
            }
        )

let json =
    Tree(1, [ Tree(2, []); Tree(3, [ Tree(4, []) ]) ])
    |> Encode.codec Tree.codec
    |> Encode.toString 0

printfn "%s" json

Decode.fromString Tree.codec json |> Docs.print
```

## Mutually recursive types

Two types that refer to each other need `Codec.lazily`, which defers the construction of a codec
until it is first used.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

type Node =
    {
        Name: string
        Content: Content
    }

and Content =
    | Leaf of string
    | Children of Node list

module rec Codecs =

    let node: Codec<Node> =
        objectCodec {
            let! name = Codec.field "name" _.Name Codec.string

            and! content =
                Codec.field "content" _.Content (Codec.lazily (lazy content))

            return
                {
                    Name = name
                    Content = content
                }
        }

    let content: Codec<Content> =
        variantCodec {
            let! leaf = Codec.case "leaf" Leaf Codec.string

            and! children =
                Codec.case "children" Children (Codec.list (Codec.lazily (lazy node)))

            return
                function
                | Leaf text -> leaf text
                | Children nodes -> children nodes
        }

let json =
    {
        Name = "root"
        Content =
            Children
                [
                    {
                        Name = "a"
                        Content = Leaf "hello"
                    }
                ]
    }
    |> Encode.codec Codecs.node
    |> Encode.toString 0

printfn "%s" json

Decode.fromString Codecs.node json |> Docs.print
```

## Decoders and encoders

The manual API has the same two functions: `Decode.fix`, `Decode.lazily`, and `Encode.lazily`.
