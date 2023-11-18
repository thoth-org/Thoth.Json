module Build.Test.Newtonsoft

open System.IO
open System
open BlackFox.CommandLine
open Build
open SimpleExec

// let private mochaComand =
//     CmdLine.empty
//     |> CmdLine.appendRaw "npx"
//     |> CmdLine.appendRaw "mocha"
//     |> CmdLine.appendRaw Wo


// let handle (args: string list) =
//     let isWatch = args |> List.contains "--watch"

//     if isWatch then
//         testNewtonsoft isWatch
//     else
//         handleMainTests isWatch
//         ()
