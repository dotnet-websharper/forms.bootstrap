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

module Site =
    open WebSharper.UI.Server
    open WebSharper.UI.Html

    [<Website>]
    let Main =
        Application.SinglePage (fun ctx ->
            Content.Page(
                Title = "WebSharper.Forms.Bootstrap Tests",
                Body = [ client <@ Client.LoginForm() @> ]
            )
        )
