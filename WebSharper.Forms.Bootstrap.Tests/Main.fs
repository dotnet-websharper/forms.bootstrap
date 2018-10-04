// $begin{copyright}
//
// This file is part of WebSharper
//
// Copyright (c) 2008-2018 IntelliFactory
//
// Licensed under the Apache License, Version 2.0 (the "License"); you
// may not use this file except in compliance with the License.  You may
// obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
// implied.  See the License for the specific language governing
// permissions and limitations under the License.
//
// $end{copyright}
namespace WebSharper.Forms.Bootstrap.Tests

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI
open WebSharper.UI.Html

[<JavaScript>]
module Client =
    open WebSharper.JavaScript
    open WebSharper.UI.Client
    open WebSharper.Forms
    open WebSharper.Forms.Bootstrap

    module B = Controls

    let cls = Attr.Class

    [<SPAEntryPoint>]
    let LoginForm () =
        Form.Return (fun user pass check -> user, pass, check)
        <*> (Form.Yield ""
            |> Validation.IsNotEmpty "Must enter a username")
        <*> (Form.Yield ""
            |> Validation.IsNotEmpty "Must enter a password")
        <*> Form.Yield false
        |> Form.WithSubmit
        |> Form.Run (fun (user, pass, check) ->
            JS.Alert("Welcome, " + user + "!")
        )
        |> Form.Render (fun user pass check submit ->
            form [] [
                B.Input "Username" [] (user, [cls "sr-only"], [cls "input-lg"; attr.readonly ""])
                B.Simple.TextAreaWithError "Username - echoed" user submit.View
                B.TextAreaWithError "Username - echoed" [] (user, [], []) submit.View
                B.Simple.InputWithError "Username - echoed" user submit.View
                B.Simple.InputPasswordWithError "Password" pass submit.View
                B.Checkbox "Keep me logged in" [] (check, [cls "input-lg"], [])
                B.Radio "This is a radio button" [] (check, [], [])
                B.Button "Log in" [B.Class "btn btn-primary"] submit.Trigger
                B.ShowErrors [attr.style "margin-top:1em;"] submit.View
            ]
        )
        |> Doc.RunById "main"
