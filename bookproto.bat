@echo off
if exist runtime\php.exe (runtime\php bookproto.php) else (php bookproto.php)
pause