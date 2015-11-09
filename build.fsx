#load "tools/includes.fsx"
open IntelliFactory.Build

let bt =
    BuildTool().PackageId("WebSharper.Forms.Bootstrap")
        .VersionFrom("WebSharper")
        .WithFSharpVersion(FSharpVersion.FSharp30)
        .WithFramework(fun f -> f.Net40)

let main =
    bt.WebSharper.Library("WebSharper.Forms.Bootstrap")
        .SourcesFromProject()
        .Embed([])
        .References(fun r ->
            [
                r.NuGet("WebSharper.Forms").Latest(allowPreRelease=true).Reference()
            ])

let tests =
    bt.WebSharper.SiteletWebsite("WebSharper.Forms.Bootstrap.Tests")
        .SourcesFromProject()
        .Embed([])
        .References(fun r ->
            [
                r.Project(main)
                r.NuGet("WebSharper.UI.Next").Reference()
                r.NuGet("WebSharper.Forms").Latest(allowPreRelease=true).Reference()
            ])

bt.Solution [
    main
    tests

    bt.NuGet.CreatePackage()
        .Configure(fun c ->
            { c with
                Title = Some "WebSharper.Forms.Bootstrap"
                LicenseUrl = Some "http://websharper.com/licensing"
                ProjectUrl = Some "https://github.com/intellifactory/websharper.forms.bootstrap"
                Description = "A reactive WebSharper forms library using Bootstrap"
                RequiresLicenseAcceptance = true })
        .Add(main)
]
|> bt.Dispatch
