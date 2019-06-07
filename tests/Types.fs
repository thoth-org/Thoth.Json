module Tests.Types

open Thoth.Json
open System.Threading
open Fable.Core

type Record2 =
    { a : float
      b : float }

    static member Create a b =
        { a = a
          b = b }

type Record3 =
    { a : float
      b : float
      c : float }

    static member Create a b c =
        { a = a
          b = b
          c = c }

type Record4 =
    { a : float
      b : float
      c : float
      d : float }

    static member Create a b c d =
        { a = a
          b = b
          c = c
          d = d }

type Record5 =
    { a : float
      b : float
      c : float
      d : float
      e : float }

    static member Create a b c d e =
        { a = a
          b = b
          c = c
          d = d
          e = e }

type Record6 =
    { a : float
      b : float
      c : float
      d : float
      e : float
      f : float }

    static member Create a b c d e f =
        { a = a
          b = b
          c = c
          d = d
          e = e
          f = f }

type Record7 =
    { a : float
      b : float
      c : float
      d : float
      e : float
      f : float
      g : float }

    static member Create a b c d e f g =
        { a = a
          b = b
          c = c
          d = d
          e = e
          f = f
          g = g }

type Record8 =
    { a : float
      b : float
      c : float
      d : float
      e : float
      f : float
      g : float
      h : float }

    static member Create a b c d e f g h =
        { a = a
          b = b
          c = c
          d = d
          e = e
          f = f
          g = g
          h = h }

type MyUnion = Foo of int

type Record9 =
    { a: int
      b: string
      c: (bool * int) list
      d: (MyUnion option) []
      e: Map<string, Record2>
      f: System.DateTime
      g: Set<Record2>
    }

type User =
    { Id : int
      Name : string
      Email : string
      Followers : int }

    static member Create id name email followers =
        { Id = id
          Name = name
          Email = email
          Followers = followers }

type SmallRecord =
    { fieldA: string }

    static member Decoder =
        Decode.object (fun get ->
            { fieldA = get.Required.Field "fieldA" Decode.string }
        )

    static member Encoder x =
        Encode.object [
            "fieldA", Encode.string x.fieldA
        ]

type MediumRecord =
    { FieldA: string
      FieldB: string
      FieldC: int
      FieldD: bool }

type SmallRecord2 =
    { optionalField : string option }

type Model =
    { User : User option
      Field2 : int }

type MyList<'T> =
    | Nil
    | Cons of 'T * MyList<'T>

type TestMaybeRecord =
    { Maybe : string option
      Must : string }

type BaseClass =
    class end

type RecordWithOptionalClass =
    { MaybeClass : BaseClass option
      Must : string }

type RecordWithRequiredClass =
    { Class : BaseClass
      Must : string }


type Shape =
    | Circle of radius: int
    | Rectangle of width: int * height: int

    static member DecoderCircle =
        Decode.field "radius" Decode.int
        |> Decode.map Circle

    static member DecoderRectangle =
        Decode.tuple2
            (Decode.field "width" Decode.int)
            (Decode.field "height" Decode.int)
        |> Decode.map Rectangle

type MyObj =
    { Enabled: bool
      Shape: Shape }

type MyObj2 =
    { Enabled: bool
      Shape: Shape option }

exception CustomException

type BigIntRecord =
    { bigintField: bigint }

type ChildType =
    { ChildField: string }
    static member Encode(x: ChildType) =
        Encode.string x.ChildField
    static member Decoder =
        Decode.string |> Decode.map (fun x -> { ChildField = x })

type ParentRecord =
    { ParentField: ChildType }

type Price =
    | Normal of float
    | Reduced of float option
    | Zero


type RecordWithStrangeType =
    { Id : int
      Thread : Thread option }

type UserCaseSensitive =
    { Id : int
      Name : string
      Email : string
      followers : int }

type IAmAnInterface =
    abstract member DoIt : unit -> unit

type RecordWithInterface =
    { Id : int
      Interface : IAmAnInterface option }

type MyRecType =
    { Name: string
      Children: MyRecType List }

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
