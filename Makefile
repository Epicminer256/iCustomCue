dotnet = dotnet
iscc="C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
distributable_runtime = --self-contained true --runtime win-x86 --configuration Release
single_file = -p:PublishSingleFile=true

all: build clean-build build-setup

build: Program.cs
	@echo "Building with built-in runtime..."
	@${dotnet} publish ${single_file} ${distributable_runtime} -o publish .

build-setup:
	@echo "Building Setup..."
	@${iscc} InnoSetupScript.iss

clean-build:	
	@rm -r bin || true
	@rm -r obj || true
	@rm publish/*.dll || true
	@rm publish/*.pdb || true

clean:
	@echo "Cleaning..."
	@rm -r publish || true
	@rm -r bin || true
	@rm -r obj || true