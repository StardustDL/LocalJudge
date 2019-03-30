if ($args.Count -eq 0) {
    Write-Output "Please input type."
}
else {
    $wdir = Join-Path $(Get-Location) "temp/test"
    Write-Output ("Workspace: " + $wdir)
    switch ($args[0]) {
        "api" { 
            dotnet run -p ./src/LocalJudge.Server.API -- -d $wdir --http-port 5000 --https-port 5001
        }
        "host" {
            dotnet run -p ./src/LocalJudge.Server.Host -- -s "https://localhost:5001" --http-port 6000 --https-port 6001
        }
        "judger" {
            dotnet run -p ./src/LocalJudge.Server.Judger -- -d $wdir
        }
        default {
            Write-Output "The type is not found."
        }
    }
}