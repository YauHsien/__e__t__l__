Write-Host "Removing service ghetl"
Stop-Service -Name "ghetl"
if ((Get-Host | Select-Object Version).Value -lt 6)
{
	$service = Get-WmiObject -Class Win32_Service -Filter "Name='ghetl'"
	$service.delete()
}
else
{
	Remove-Service -Name "ghetl"
}
Get-Service -Name "ghetl"
Write-Host "Done."