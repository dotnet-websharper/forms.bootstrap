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
namespace WebSharper.Forms.Bootstrap

module Resources =
    open WebSharper.Core.Resources

    type CSS() =
        inherit BaseResource("https://maxcdn.bootstrapcdn.com/bootstrap/3.3.5/css", "bootstrap.min.css")

open WebSharper

[<Require(typeof<Resources.CSS>)>]
[<JavaScript>]
module Controls =
    open WebSharper.UI
    open WebSharper.UI.Html
    open WebSharper.UI.Client
    open WebSharper.Forms

    let private cls = Attr.Class

    let Class = Attr.Class
    
    let Input lbl extras (target, labelExtras, targetExtras) =
        div (cls "form-group" :: extras) [
            label labelExtras [text lbl]
            Doc.Input (cls "form-control" :: targetExtras) target
        ]

    let InputPassword lbl extras (target, labelExtras, targetExtras) =
        div (cls "form-group" :: extras) [
            label labelExtras [text lbl]
            Doc.PasswordBox (cls "form-control" :: targetExtras) target
        ]

    let TextArea lbl extras (target, labelExtras, targetExtras) =
        div (cls "form-group" :: extras) [
            label labelExtras [text lbl]
            Doc.InputArea (cls "form-control" :: targetExtras) target
        ]

    let Checkbox lbl extras (target, labelExtras, targetExtras) =
        div (cls "checkbox" :: extras) [
            label labelExtras [
                Doc.CheckBox targetExtras target
                text lbl
            ]
        ]

    let Radio lbl extras (target, labelExtras, targetExtras) =
        div (cls "radio" :: extras) [
            label labelExtras [
                Doc.Radio targetExtras true target
                text lbl
            ]
        ]

    let private __InputWithError inputFun lbl extras ((target: Var<_>), labelExtras, targetExtras) (submitView: View<Result<_>>) =
        let tv = submitView.Through target
        let errorOpt, errorClassOpt =
            tv.Map (fun res ->
                // Extract a single line of errors, and the optional attribute for them
                match res with
                | Result.Success _
                | Result.Failure [] ->
                    None, None
                | Result.Failure errs ->
                    let errors =
                        errs
                        |> List.map (fun e -> e.Text)
                        |> List.reduce (fun a b -> a + "; " + b)
                    Some errors, Some "has-error"
            )
            |> fun view ->
                view.Map fst, view.Map snd
        div [
            yield cls "form-group"
            yield Attr.DynamicClass "has-error" errorClassOpt (fun opt -> opt.IsSome)
            yield! extras
        ] [
            yield label labelExtras [text lbl] :> Doc
            yield inputFun (cls "form-control" :: targetExtras) target :> Doc
            yield errorOpt.Doc (function
                | None -> Doc.Empty
                | Some error ->
                    span [cls "help-block"] [text error] :> Doc
            )
        ]

    let InputWithError lbl = __InputWithError Doc.Input lbl
    let InputPasswordWithError lbl = __InputWithError Doc.PasswordBox lbl
    let TextAreaWithError lbl = __InputWithError Doc.InputArea lbl
    
    let Button = Doc.Button
    
    let ShowErrors extras (submit: View<Result<_>>) =
        Doc.ShowErrors submit (function
            | [] ->
                Doc.Empty
            | errors ->
                div extras [
                    errors
                    |> Seq.map (fun m -> p [] [text m.Text])
                    |> Seq.cast
                    |> div [cls "alert alert-danger"]
                ] :> Doc
        )

    module Simple =
        let Input lbl target = Input lbl [] (target, [], [])
        let InputWithError lbl target submit = InputWithError lbl [] (target, [], []) submit
        let InputPassword lbl target = InputPassword lbl [] (target, [], [])
        let InputPasswordWithError lbl target submit = InputPasswordWithError lbl [] (target, [], []) submit
        let TextArea lbl target = TextArea lbl [] (target, [], [])
        let TextAreaWithError lbl target submit = TextAreaWithError lbl [] (target, [], []) submit
        let Checkbox lbl target = Checkbox lbl [] (target, [], [])
        
        let Button label f = Button label [cls "button btn-primary"] f

        let ShowErrors submit = ShowErrors [] submit
