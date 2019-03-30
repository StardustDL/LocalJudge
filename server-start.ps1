if ($args.Count -eq 0) {
    Write-Output "Please input type."
}
else {
    switch ($args[0]) {
        "api" { 
            dotnet run -p ./src/LocalJudge.Server.API
        }
        "host" {
            dotnet run -p ./src/LocalJudge.Server.Host
        }
        "judger" {
            dotnet run -p ./src/LocalJudge.Server.Judger -- -d ./temp/test/
        }
        default {
            Write-Output "The type is not found."
        }
    }
}