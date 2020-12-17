@echo off
rem 移除 ghetl 服務
echo Removing serivce ghetl...
sc stop ghetl
sc delete ghetl
echo Done.
echo on