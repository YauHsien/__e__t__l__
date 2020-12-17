@echo off
rem 啟動 ghetl 服務
echo Creating serivce ghetl...
sc create ghetl binPath=%~dp0\GH.ETL.Main.exe
sc start ghetl
echo Done.
echo on