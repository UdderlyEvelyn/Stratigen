@echo off
echo Translating Shaders...
%1\ShaderAbstractionLayer %1\Blocks.fx
%1\ShaderAbstractionLayer %1\ShadowMap.fx
echo Compiling Shaders...
%1\2mgfx %1\SM2ShadowMap.fx %1\SM2ShadowMap.mgfx
%1\2mgfx %1\SM4ShadowMap.fx %1\SM4ShadowMap.mgfx /DX11
%1\2mgfx %1\SM2Blocks.fx %1\SM2Blocks.mgfx
%1\2mgfx %1\SM4Blocks.fx %1\SM4Blocks.mgfx /DX11
echo Copying...
copy /y %1\*.mgfx %1\..\Data\Shaders
echo Cleaning Up...
del /f /q %1\*.mgfx
del /f /q %1\SM2*.fx
del /f /q %1\SM4*.fx
echo Done!
