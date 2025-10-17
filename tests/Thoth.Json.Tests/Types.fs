module Thoth.Json.Tests.Types

open Thoth.Json.Core
open System.Threading
#if !NETFRAMEWORK
open Fable.Core
#endif

type Record2 =
    {
        a: float
        b: float
    }

    static member Create a b =
        {
            a = a
            b = b
        }

type Record3 =
    {
        a: float
        b: float
        c: float
    }

    static member Create a b c =
        {
            a = a
            b = b
            c = c
        }

type Record4 =
    {
        a: float
        b: float
        c: float
        d: float
    }

    static member Create a b c d =
        {
            a = a
            b = b
            c = c
            d = d
        }

type Record5 =
    {
        a: float
        b: float
        c: float
        d: float
        e: float
    }

    static member Create a b c d e =
        {
            a = a
            b = b
            c = c
            d = d
            e = e
        }

type Record6 =
    {
        a: float
        b: float
        c: float
        d: float
        e: float
        f: float
    }

    static member Create a b c d e f =
        {
            a = a
            b = b
            c = c
            d = d
            e = e
            f = f
        }

type Record7 =
    {
        a: float
        b: float
        c: float
        d: float
        e: float
        f: float
        g: float
    }

    static member Create a b c d e f g =
        {
            a = a
            b = b
            c = c
            d = d
            e = e
            f = f
            g = g
        }

type Record8 =
    {
        a: float
        b: float
        c: float
        d: float
        e: float
        f: float
        g: float
        h: float
    }

    static member Create a b c d e f g h =
        {
            a = a
            b = b
            c = c
            d = d
            e = e
            f = f
            g = g
            h = h
        }

type MyUnion = | Foo of int

type Record9 =
    {
        a: int
        b: string
        c: (bool * int) list
        d: (MyUnion option)[]
        e: Map<string, Record2>
        f: System.DateTime
        g: Set<Record2>
        h: System.TimeSpan
        i: sbyte
        j: byte
        k: int16
        l: uint16
        m: uint32
        n: int64
        o: uint64
        p: unit
        r: Map<Record2, string>
        s: char
    // s: string seq
    }

type Record10 =

    {
        a: int
        b: int
        c: int
        d: int
        e: int
        f: int
        g: int
        h: int
        i: int
        j: int
        k: int
    }

    static member Create a b c d e f g h i j k =
        {
            a = a
            b = b
            c = c
            d = d
            e = e
            f = f
            g = g
            h = h
            i = i
            j = j
            k = k
        }

type User =
    {
        Id: int
        Name: string
        Email: string
        Followers: int
    }

    static member Create id name email followers =
        {
            Id = id
            Name = name
            Email = email
            Followers = followers
        }

type SmallRecord =
    {
        fieldA: string
    }

    static member Decoder =
        Decode.object (fun get ->
            {
                fieldA = get.Required.Field "fieldA" Decode.string
            }
        )

    static member Encoder x =
        Encode.object [ "fieldA", Encode.string x.fieldA ]

type MediumRecord =
    {
        FieldA: string
        FieldB: string
        FieldC: int
        FieldD: bool
    }

type SmallRecord2 =
    {
        optionalField: string option
    }

type Model =
    {
        User: User option
        Field2: int
    }

type MyList<'T> =
    | Nil
    | Cons of 'T * MyList<'T>

type TestMaybeRecord =
    {
        Maybe: string option
        Must: string
    }

type BaseClass =
    class
    end

[<NoComparison>]
type RecordWithOptionalClass =
    {
        MaybeClass: BaseClass option
        Must: string
    }

[<NoComparison>]
type RecordWithRequiredClass =
    {
        Class: BaseClass
        Must: string
    }


type Shape =
    | Circle of radius: int
    | Rectangle of width: int * height: int

    static member DecoderCircle =
        Decode.field "radius" Decode.int |> Decode.map Circle

    static member DecoderRectangle =
        Decode.tuple2
            (Decode.field "width" Decode.int)
            (Decode.field "height" Decode.int)
        |> Decode.map Rectangle

type MyObj =
    {
        Enabled: bool
        Shape: Shape
    }

type MyObj2 =
    {
        Enabled: bool
        Shape: Shape option
    }

exception CustomException

type BigIntRecord =
    {
        bigintField: bigint
    }

type ChildType =
    {
        ChildField: string
    }

    static member Encode(x: ChildType) = Encode.string x.ChildField

    static member Decoder =
        Decode.string
        |> Decode.map (fun x ->
            {
                ChildField = x
            }
        )

type ParentRecord =
    {
        ParentField: ChildType
    }

type Price =
    | Normal of float
    | Reduced of float option
    | Zero


[<NoComparison>]
type RecordWithStrangeType =
    {
        Id: int
        Thread: Thread option
    }

type UserCaseSensitive =
    {
        Id: int
        Name: string
        Email: string
        followers: int
    }

type IAmAnInterface =
    abstract member DoIt: unit -> unit

[<NoComparison>]
type RecordWithInterface =
    {
        Id: int
        Interface: IAmAnInterface option
    }

type MyRecType =
    {
        Name: string
        Children: MyRecType List
    }

#if !NETFRAMEWORK
[<StringEnum>]
type Camera =
    | FirstPerson
    | ArcRotate
    | IsometricTopDown

[<StringEnum(CaseRules.LowerFirst)>]
type Framework =
    | React
    | VueJs

[<StringEnum(CaseRules.None)>]
type Language =
    | Fsharp
    | [<CompiledName("C#")>] Csharp
#endif

type Enum_Int8 =
    | Zero = 0y
    | NinetyNine = 99y

type Enum_UInt8 =
    | Zero = 0uy
    | NinetyNine = 99uy

type Enum_Int =
    | Zero = 0
    | One = 1
    | Two = 2

type Enum_Int16 =
    | Zero = 0s
    | NinetyNine = 99s

type Enum_UInt16 =
    | Zero = 0us
    | NinetyNine = 99us

type Enum_UInt32 =
    | Zero = 0u
    | NinetyNine = 99u

#if FABLE_COMPILER
type NoAlloc = Fable.Core.EraseAttribute
#else
type NoAlloc = StructAttribute
#endif

[<NoAlloc>]
type NoAllocAttributeId = | NoAllocAttributeId of System.Guid

type TestStringWithHTML =
    {
        FeedName: string
        Content: string
    }

type RecordForCharacterCase =
    {
        One: int
        TwoPart: int
        ThreePartField: int
    }

module IntAsRecord =

    let encode (value: int) =
        Encode.object
            [
                "type", Encode.string "int"
                "value", Encode.int value
            ]

    let decode: Decoder<int> =
        Decode.field "type" Decode.string
        |> Decode.andThen (fun typ ->
            if typ = "int" then
                Decode.field "value" Decode.int
            else
                Decode.fail "Invalid type"
        )

type Person =
    {
        Name: string
    }

type Post =
    {
        Title: string
    }

type Data =
    {
        Person: Person
        Post: Post option
    }
