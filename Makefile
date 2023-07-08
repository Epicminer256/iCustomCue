dotnet = dotnet
distributable_runtime = --self-contained true --runtime win-x86 --configuration Release
single_file = -p:PublishSingleFile=true

all: build clean-build

build: Program.cs
	@echo "Building with built-in runtime..."
	@${dotnet} publish ${single_file} ${distributable_runtime} -o publish .

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