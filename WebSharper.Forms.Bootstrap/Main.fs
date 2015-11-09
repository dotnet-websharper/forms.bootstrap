namespace WebSharper.Forms.Bootstrap

module Resources =
    open WebSharper.Core.Resources

    type CSS() =
        inherit BaseResource("https://maxcdn.bootstrapcdn.com/bootstrap/3.3.5/css", "bootstrap.min.css")

open WebSharper

[<Require(typeof<Resources.CSS>)>]
[<JavaScript>]
module Controls =
    open WebSharper.UI.Next
    open WebSharper.UI.Next.Html
    open WebSharper.UI.Next.Client
    open WebSharper.Forms

    let private cls = Attr.Class

    let Class = Attr.Class
    
    let Input lbl extras (target, labelExtras, targetExtras) =
        divAttr (cls "form-group" :: extras) [
            labelAttr labelExtras [text lbl]
            Doc.Input (cls "form-control" :: targetExtras) target
        ]

    let InputPassword lbl extras (target, labelExtras, targetExtras) =
        divAttr (cls "form-group" :: extras) [
            labelAttr labelExtras [text lbl]
            Doc.PasswordBox (cls "form-control" :: targetExtras) target
        ]

    let TextArea lbl extras (target, labelExtras, targetExtras) =
        divAttr (cls "form-group" :: extras) [
            labelAttr labelExtras [text lbl]
            Doc.InputArea (cls "form-control" :: targetExtras) target
        ]

    let Checkbox lbl extras (target, labelExtras, targetExtras) =
        divAttr (cls "checkbox" :: extras) [
            labelAttr labelExtras [
                Doc.CheckBox targetExtras target
                text lbl
            ]
        ]

    let Radio lbl extras (target, labelExtras, targetExtras) =
        divAttr (cls "radio" :: extras) [
            labelAttr labelExtras [
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
        divAttr [
            yield cls "form-group"
            yield Attr.DynamicClass "has-error" errorClassOpt (fun opt -> opt.IsSome)
            yield! extras
        ] [
            yield labelAttr labelExtras [text lbl] :> Doc
            yield inputFun (cls "form-control" :: targetExtras) target :> Doc
            yield errorOpt.Doc (function
                | None -> Doc.Empty
                | Some error ->
                    spanAttr [cls "help-block"] [text error] :> Doc
            )
        ]

    let InputWithError lbl = __InputWithError Doc.Input lbl
    let InputPasswordWithError lbl = __InputWithError Doc.PasswordBox lbl
    let TextAreaWithError lbl = __InputWithError Doc.InputArea lbl
    
    let Button = Doc.Button
    
    let ShowErrors extras (submit: Submitter<Result<_>>) =
        Doc.ShowErrors submit.View (function
            | [] ->
                Doc.Empty
            | errors ->
                divAttr extras [
                    errors
                    |> Seq.map (fun m -> p [text m.Text])
                    |> Seq.cast
                    |> divAttr [cls "alert alert-danger"]
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
        
