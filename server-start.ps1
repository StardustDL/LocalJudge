if ($args.Count -eq 0) {
    Write-Output "Please input type."
}
else {
    switch ($args[0]) {
        "api" { 
            dotnet run -p ./src/LocalJudge.Server.API -- -d ../../temp/test/ --http-port 5000 --https-port 5001
        }
        "host" {
            dotnet run -p ./src/LocalJudge.Server.Host -- -s "https://localhost:5001" --http-port 6000 --https-port 6001
        }
        "judger" {
            dotnet run -p ./src/LocalJudge.Server.Judger -- -d ./temp/test/
        }
        default {
            Write-Output "The type is not found."
        }
    }
}