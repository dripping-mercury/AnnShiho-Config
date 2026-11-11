# Set Working Directory
Split-Path $MyInvocation.MyCommand.Path | Push-Location
[Environment]::CurrentDirectory = $PWD

Remove-Item "$env:RELOADEDIIMODS/p5rpc.event.annshihocanon/*" -Force -Recurse
dotnet publish "./AnnShihoConfig.csproj" -c Release -o "$env:RELOADEDIIMODS/p5rpc.event.annshihocanon" /p:OutputPath="./bin/Release" /p:ReloadedILLink="true"

# Restore Working Directory
Pop-Location