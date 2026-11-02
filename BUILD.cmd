@echo off
cd /d "%~dp0"
set MSBuildEnableWorkloadResolver=false
dotnet build QuanLyThuVien.GUI/QuanLyThuVien.GUI.csproj -m:1 -p:UseSharedCompilation=false
if errorlevel 1 goto end
dotnet publish QuanLyThuVien.GUI/QuanLyThuVien.GUI.csproj -c Release --self-contained false -m:1 -p:UseSharedCompilation=false -o ChayNgay
:end
pause
