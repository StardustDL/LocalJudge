if ($args.Count -eq 0) {
    Write-Output "Please input type."
}
else {
    switch ($args[0]) {
        "redb" {
            Remove-Item ./src/StarOJ.Data.Provider.SqlServer/Migrations/*
            dotnet ef migrations add Initial -p ./src/StarOJ.Data.Provider.SqlServer
            dotnet ef database update -p ./src/StarOJ.Data.Provider.SqlServer
        }
        "api" {
            dotnet run -p ./src/StarOJ.Server.API
        }
        "host" {
            dotnet run -p ./src/StarOJ.Server.Host -- -s "https://localhost:5001" --http-port 6000 --https-port 6001
        }
        "judger" {
            $wdir = Join-Path $(Get-Location) "temp/judger"
            dotnet run -p ./src/StarOJ.Server.Judger -- -s "https://localhost:5001" -d $wdir -c "config.json"
        }
        "judger-fs" {
            $wdir = Join-Path $(Get-Location) "temp/test"
            dotnet run -p ./src/StarOJ.Server.Judger.FileSystem -- -d $wdir
        }
        default {
            Write-Output "The type is not found."
        }
    }
}