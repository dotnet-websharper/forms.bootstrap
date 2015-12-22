#load "tools/includes.fsx"
open IntelliFactory.Build

let bt =
    BuildTool().PackageId("Zafir.Forms.Bootstrap")
        .VersionFrom("Zafir", "alpha")
        .WithFSharpVersion(FSharpVersion.FSharp30)
        .WithFramework(fun f -> f.Net40)

let main =
    bt.Zafir.Library("WebSharper.Forms.Bootstrap")
        .SourcesFromProject()
        .Embed([])
        .References(fun r ->
            [
                r.NuGet("Zafir.Forms").Latest(allowPreRelease=true).ForceFoundVersion().Reference()
            ])

let tests =
    bt.Zafir.SiteletWebsite("WebSharper.Forms.Bootstrap.Tests")
        .SourcesFromProject()
        .Embed([])
        .References(fun r ->
            [
                r.Project(main)
                r.NuGet("Zafir.UI.Next").Latest(true).ForceFoundVersion().Reference()
                r.NuGet("Zafir.Forms").Latest(true).ForceFoundVersion().Reference()
            ])

bt.Solution [
    main
    tests

    bt.NuGet.CreatePackage()
        .Configure(fun c ->
            { c with
                Title = Some "Zafir.Forms.Bootstrap"
                LicenseUrl = Some "http://websharper.com/licensing"
                ProjectUrl = Some "https://github.com/intellifactory/websharper.forms.bootstrap"
                Description = "A reactive Zafir forms library using Bootstrap"
                RequiresLicenseAcceptance = true })
        .Add(main)
]
|> bt.Dispatch
