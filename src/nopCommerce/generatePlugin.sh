#! /bin/bash

# Create the solution
# /${appName}
#    /Plugins/
#    	Nop.Plugin.${group}.${name}/
#		    ${group}.${name}.csproj
#		    Controllers/xxx
#		    Models/xxx
#		    Views/xxx
##

#Methos
Success() {
    local message=$1
    tput setaf 2
    echo -e "$message\n"
    tput setaf 7
}
Warning() {
    local message=$1
    tput setaf 3
    echo -e "$message\n"
    tput setaf 7
}
WarningCreating() {
    local message=$1 seconds=$2
    tput setaf 3
    echo -e "$message..."
    sleep $seconds
    tput setaf 7
}
Error() {
    local message=$1
    tput setaf 1
    echo -e "$message\n"
    tput setaf 7
    exit 0
}
SuccessTag() {
    local message=$1
    tput setaf 2
    echo -e "Success: $message\n"
    tput setaf 7
}
WarningTag() {
    local message=$1
    tput setaf 3
    echo -e "Warning: $message\n"
    tput setaf 7
}
ErrorTag() {
    local message=$1
    tput setaf 1
    echo -e "Error: $message\n"
    tput setaf 7
}
Message() {
    local message=$1
    tput setaf 6
    echo -e "$message"
    tput setaf 7
}
Help() {
    echo "nopCommerce Plugin, versions supported are: 4.10 and up [4.10, 4.20, 4.30, 4.40, 4.50, ...]"
    echo -e "Usage: plugin [plugin-group] [plugin-name] [nop-version]\n"

    echo -e "Plugin options:"
    echo -e "   -d|--diagnostics  Enable diagnostic output."
    echo -e "   -h|--help         Show command line help."
    echo -e "   --info            Display plugin information.\n"

    echo -e "Plugin commands:"
    echo -e "   add               Add a package or reference to a .NET project.\n"
    echo -e "Run 'plugin [command] --help' for more information on a command."
    exit 0
}
CommandHelp() {
    echo "nopCommerce Plugin, versions supported are: 4.10 and up [4.10, 4.20, 4.30, 4.40, 4.50, ...]"
    echo -e "Usage: plugin [plugin-group] [plugin-name] [nop-version]\n"

    echo -e "Plugin options:"
    echo -e "   -d|--diagnostics  Enable diagnostic output."
    echo -e "   -h|--help         Show command line help."
    echo -e "   --info            Display plugin information.\n"

    echo -e "Plugin commands:"
    echo -e "   new               Add new plugin in the nopCommerce project.\n"
    echo -e "Run 'plugin [command] --help' for more information on a command."
    exit 0
}
DirHelp() {
    tput setaf 1
    echo -e "please, put the file in the nopCommerce root example:"
    tput setaf 7
    echo -e "nopCommerce/"
    echo -e "        /plugin                <--  this es the file to run (plugin.sh)"
    echo -e "        /nopCommnerce.sln      <--  this is the source file (file.sln)"
    echo -e "        /Plugin/..."
    echo -e "        /Presentation/..."
    echo -e "        /Libraries/...\n"

    echo -e "Usage: plugin [plugin-group] [plugin-name] [nop-version]"
    echo -e "example: plugin Payment Manual 4.30\n"

    echo -e "Plugin options:"
    echo -e "   -d|--diagnostics  Enable diagnostic output."
    echo -e "   -h|--help         Show command line help."
    echo -e "   --info            Display plugin information.\n"

    echo -e "Plugin commands:"
    echo -e "   new               Add new plugin in the nopCommerce project.\n"
    echo -e "Run 'plugin [command] --help' for more information on a command."
    exit 0
}
HelpExample() {
    echo -e "Example you can run command like: plugin [plugin-group] [plugin-name] 4.30."
    echo -e "or:                             ./plugin [plugin-group] [plugin-name] 4.30."
    exit 0
}
NoSDKInstalled() {
    local sdk=$1
    Error "You do not have the dotnet SDK ($sdk) installed.\n$(tput setaf 7)For more information plaese type followin command: dotnet --list-sdks\nTo install additional .NET runtimes or SDKs: https://aka.ms/dotnet-download"
}
if [ "$1" = "-h" -o "$1" = "--help" ]; then Help; fi
if [ "$1" = "new" ] && [ "$2" = "-h" -o "$2" = "--help" ]; then CommandHelp; fi
if [ ! -d ./Plugins -o ! -d ./Presentation -o ! -f ./*.sln ]; then DirHelp; fi
if [ "$1" = "" -o "$2" = "" ]; then
    HelpExample
else
    path=. ## the path of the nopCommnerce root in this case the same path ./
    if [ "$3" = "" ] && [ -f $path/Libraries/Nop.Core/NopVersion.cs ]; then
        if [ $(grep -c -F '"4.10"' ./Libraries/Nop.Core/NopVersion.cs) != 0 ]; then
            version="4.10"
            framework=netcoreapp2.1
            if [ $(dotnet --list-sdks | grep -c 2.1) = 0 ]; then NoSDKInstalled $framework; fi
            Success "Creating the Plugin with the dotnet SDK Version: $framework"
        elif [ $(grep -c '"4.20"' ./Libraries/Nop.Core/NopVersion.cs) != 0 ]; then
            version="4.20"
            framework=netcoreapp2.2
            if [ $(dotnet --list-sdks | grep -c 2.2) = 0 ]; then NoSDKInstalled $framework; fi
            Success "Creating the Plugin with the dotnet SDK Version: $framework"
        elif [ $(grep -c '"4.30"' $path/Libraries/Nop.Core/NopVersion.cs) != 0 ]; then
            version="4.30"
            framework=netcoreapp3.1
            if [ $(dotnet --list-sdks | grep -c 3.1) = 0 ]; then NoSDKInstalled $framework; fi
            Success "Creating the Plugin with the dotnet SDK Version: $framework"
        elif [ $(grep -c '"4.40"' $path/Libraries/Nop.Core/NopVersion.cs) != 0 ]; then
            version="4.40"
            framework=net5.0
            if [ $(dotnet --list-sdks | grep -c 5.0) = 0 ]; then NoSDKInstalled $framework; fi
            Success "Creating the Plugin with the dotnet SDK Version: $framework"
        fi
    elif [ "$3" != "" ]; then
        version="$3"
        if [ "$3" = "4.10" ]; then
            framework=netcoreapp2.1
            if [ $(dotnet --list-sdks | grep -c 2.1) = 0 ]; then NoSDKInstalled $framework; fi
            Success "Creating the Plugin with the dotnet SDK Version: $framework"
        elif [ "$3" = "4.20" ]; then
            framework=netcoreapp2.2
            if [ $(dotnet --list-sdks | grep -c 2.2) = 0 ]; then NoSDKInstalled $framework; fi
            Success "Creating the Plugin with the dotnet SDK Version: $framework"
        elif [ "$3" = "4.30" ]; then
            framework=netcoreapp3.1
            if [ $(dotnet --list-sdks | grep -c 3.1) = 0 ]; then NoSDKInstalled $framework; fi
            Success "Creating the Plugin with the dotnet SDK Version: $framework"
        elif [ "$3" = "4.40" ]; then
            framework=net5.0
            if [ $(dotnet --list-sdks | grep -c 5.0) = 0 ]; then NoSDKInstalled $framework; fi
            Success "Creating the Plugin with the dotnet SDK Version: $framework"
        else
            ErrorTag "The nopCommerce version is not compatible with this script. \nPlease verify the currents vesion are [4.10, 4.20, 4.30, 4.40, 4.50, ..., 4.60]"
            exit 0
        fi
    else
        HelpExample
    fi
fi
## NopCommerce info
echo -e "######################  Create new Nop Commnerce Plugin  ######################\n"

## Plugin info
group=$1                                 ##
name=$2                                  ##
modelName=${name}Model                   ##
controllerName=${group}${name}Controller ##

## Json file (plugin.json)
PLUGIN_GROUP="${group} Methods"
FRIENDLY_NAME="${group} ${name}"
SYSTEM_NAME="${group}.${name}"
PLUGIN_VERSION="1.00"
SUPPORTED_VERSIONS="$version"
PLUGIN_AUTHOR="${group} team"
DISPLAY_ORDER=1 # integer
FILE_NAME="Nop.Plugin.${group}.${name}.dll"
PLUGIN_DESCRIPTION="The plugin ${group} ${name} Description"
LIMITED_TO_STORE="[]"
LIMITED_TO_CUSTOMER_ROLE="[]"
DEPENDS_ON_SYSTEM_NAMES="[]"

## Nop.Plugin.${group}.${name}.csproj
SOME_COPYRIGHT="Copyright © Nop Solutions, Ltd"              ##
YOUR_COMPANY="Nop Solutions, Ltd"                            ##
SOME_AUTHORS="Nop Solutions, Ltd"                            ##
PACKAGE_LICENSE_URL=""                                       ## the url most to bi like this format: "https://www.page.com"
PACKAGE_PROJECT_URL="https://www.nopcommerce.com"            ## the url most to bi like this format: "https://www.nopcommerce.com"
REPOSITORY_URL="https://github.com/nopSolutions/nopCommerce" ## the url most to bi like this format: "https://github.com/nopSolutions/nopCommerce"
REPOSITORY_TYPE=Git                                          #REPOSITORY_TYPE				                ##
PLUGIN_OUTPUT_DIRECTORY=${group}.${name}                     ##

## create the plugin
if [ ! -f ${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj ]; then
    dotnet new classlib -n Nop.Plugin.${group}.${name} -f $framework -o ${path}/Plugins/Nop.Plugin.${group}.${name}
else
    ErrorTag "The Plugin named: Nop.Plugin.${group}.${name} already exists!"
    exit 0
fi

if [ -f ${path}/Plugins/Nop.Plugin.${group}.${name}/Class1.cs ]; then
    rm ${path}/Plugins/Nop.Plugin.${group}.${name}/Class1.cs
fi

if [ -d ${path}/Plugins/Nop.Plugin.${group}.${name} ]; then
    mkdir -p -v ${path}/Plugins/Nop.Plugin.${group}.${name}
fi

## Editting Nop.Plugin.${group}.${name}.csproj
WarningCreating "Creating the Nop.Plugin.${group}.${name}.csproj file..." 1
if [ $framework = "4.10" ]; then
    echo "<Project Sdk="'"Microsoft.NET.Sdk"'">" >${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "    <PropertyGroup>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "        <TargetFramework>$framework</TargetFramework>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "    </PropertyGroup>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "    <PropertyGroup Condition=\"'\$(Configuration)|\$(Platform)'=='Release|AnyCPU'\">" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "        <OutputPath>..\..\Presentation\Nop.Web\Plugins\\${PLUGIN_OUTPUT_DIRECTORY}</OutputPath>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "        <OutDir>\$(OutputPath)</OutDir>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "    </PropertyGroup>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "    <PropertyGroup Condition=\"'\$(Configuration)|\$(Platform)'=='Debug|AnyCPU'\">" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "        <OutputPath>..\..\Presentation\Nop.Web\Plugins\\${PLUGIN_OUTPUT_DIRECTORY}</OutputPath>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "        <OutDir>\$(OutputPath)</OutDir>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "    </PropertyGroup>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "    <!-- This target execute after "'"Build"'" target -->" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "    <Target Name="'"NopTarget"'" AfterTargets="'"Build"'">" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "        <!-- Delete unnecessary libraries from plugins path -->" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "        <MSBuild Projects=\"\$(MSBuildProjectDirectory)\..\..\Build\ClearPluginAssemblies.proj\"    Properties=\"PluginPath=\$(MSBuildProjectDirectory)\$(OutDir)\" Targets=\"NopClear\" />" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "    </Target>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "</Project>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
else
    echo "<Project Sdk="'"Microsoft.NET.Sdk"'">" >${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "    <PropertyGroup>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "        <TargetFramework>$framework</TargetFramework>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "        <Copyright>$SOME_COPYRIGHT</Copyright>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "        <Company>$YOUR_COMPANY</Company>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "        <Authors>$SOME_AUTHORS</Authors>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "        <PackageLicenseUrl>$PACKAGE_LICENSE_URL</PackageLicenseUrl>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "        <PackageProjectUrl>$PACKAGE_PROJECT_URL</PackageProjectUrl>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "        <RepositoryUrl>$REPOSITORY_URL</RepositoryUrl>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "        <RepositoryType>$REPOSITORY_TYPE</RepositoryType>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "        <OutputPath>..\..\Presentation\Nop.Web\Plugins\\${PLUGIN_OUTPUT_DIRECTORY}</OutputPath>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "        <OutDir>\$(OutputPath)</OutDir>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "        <!--Set this parameter to true to get the dlls copied from the NuGet cache to the output of your project. You need to set this parameter to true if your plugin has a nuget package to ensure that  the dlls copied from the NuGet cache to the output of your project-->" this parameter to true to get the dlls copied from the NuGet cache to the output of your >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj <!--Set
    echo "        <CopyLocalLockFileAssemblies>false</CopyLocalLockFileAssemblies>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "    </PropertyGroup>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "    <ItemGroup>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "        <None Remove=\"plugin.json\" />" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "    </ItemGroup>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "    <ItemGroup>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "        <Content Include=\"plugin.json\">" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "            <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "        </Content>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "    </ItemGroup>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "    <ItemGroup>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "        <ProjectReference Include="'"..\..\Presentation\Nop.Web.Framework\Nop.Web.Framework.csproj"'" />" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "        <ClearPluginAssemblies Include="'"$(MSBuildProjectDirectory)\..\..\Build\ClearPluginAssemblies.proj"'" />" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "    </ItemGroup>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "    <!-- This target execute after "'"Build"'" target -->" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "    <Target Name="'"NopTarget"'" AfterTargets="'"Build"'">" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "        <!-- Delete unnecessary libraries from plugins path -->" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "        <MSBuild Projects="'"@(ClearPluginAssemblies)"'" Properties="'"PluginPath=$(MSBuildProjectDirectory)\$(OutDir)"'" Targets="'"NopClear"'" />" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "    </Target>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
    echo "</Project>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
fi
if [ -f ${path}/Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj ]; then
    Success "The file Nop.Plugin.${group}.${name}.csproj was edited!"
else
    ErrorTag "There was and error creating the file Nop.Plugin.${group}.${name}.csproj."
    exit 0
fi

## Json file (plugin.json)
WarningCreating "Creating the plugin.json file..." 0.5
echo "{" >${path}/Plugins/Nop.Plugin.${group}.${name}/plugin.json # line 1
echo "    "'"Group"'": "\"$PLUGIN_GROUP"\"," >>${path}/Plugins/Nop.Plugin.${group}.${name}/plugin.json
echo "    "'"FriendlyName"'": "\"$FRIENDLY_NAME"\"," >>${path}/Plugins/Nop.Plugin.${group}.${name}/plugin.json
echo "    "'"SystemName"'": "\"$SYSTEM_NAME"\"," >>${path}/Plugins/Nop.Plugin.${group}.${name}/plugin.json
echo "    "'"Version"'": "\"$PLUGIN_VERSION"\"," >>${path}/Plugins/Nop.Plugin.${group}.${name}/plugin.json
echo "    "'"SupportedVersions"'": [ "\"$SUPPORTED_VERSIONS"\" ]," >>${path}/Plugins/Nop.Plugin.${group}.${name}/plugin.json
echo "    "'"Author"'": "\"$PLUGIN_AUTHOR"\"," >>${path}/Plugins/Nop.Plugin.${group}.${name}/plugin.json
echo "    "'"DisplayOrder"'": $DISPLAY_ORDER," >>${path}/Plugins/Nop.Plugin.${group}.${name}/plugin.json
echo "    "'"FileName"'": "\"$FILE_NAME"\"," >>${path}/Plugins/Nop.Plugin.${group}.${name}/plugin.json
echo "    "'"Description"'": "\"$PLUGIN_DESCRIPTION"\"," >>${path}/Plugins/Nop.Plugin.${group}.${name}/plugin.json
echo "    "'"LimitedToStores"'": $LIMITED_TO_STORE," >>${path}/Plugins/Nop.Plugin.${group}.${name}/plugin.json
echo "    "'"LimitedToCustomerRoles"'": $LIMITED_TO_CUSTOMER_ROLE," >>${path}/Plugins/Nop.Plugin.${group}.${name}/plugin.json
echo "    "'"DependsOnSystemNames"'": $DEPENDS_ON_SYSTEM_NAMES" >>${path}/Plugins/Nop.Plugin.${group}.${name}/plugin.json
echo "}" >>${path}/Plugins/Nop.Plugin.${group}.${name}/plugin.json
if [ -f ${path}/Plugins/Nop.Plugin.${group}.${name}/plugin.json ]; then
    Success "The plugin.json was created success!"
else
    ErrorTag "There was and error creating the file plugin.json."
    exit 0
fi

## Create the mvc, Components and (Infrastructure) dirs
if [ ! -f ${path}/Plugins/Nop.Plugin.${group}.${name}/Models ] && [ ! -f ${path}/Plugins/Nop.Plugin.${group}.${name}/Views ] && [ ! -f ${path}/Plugins/Nop.Plugin.${group}.${name}/Controllers ] && [ ! -f ${path}/Plugins/Nop.Plugin.${group}.${name}/Infrastructure ]; then
    WarningCreating "Creating the folders {Models, Views, Controllers, Components, Infrastructure}" 0
    mkdir -v ${path}/Plugins/Nop.Plugin.${group}.${name}/{Models,Views,Controllers,Components,Infrastructure}
    Success "Folders created success!"
fi

## ${group}${name}Plugin extends Base plugin class
WarningCreating "Creating the ${group}${name}Plugin class..." 0.5

echo "using System;" >${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs # line 1
echo "using Nop.Core;" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
echo "using Nop.Services.Cms;" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
echo "using Nop.Core.Domain.Cms;" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
echo "using Nop.Services.Common;" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
echo "using Nop.Services.Stores;" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
echo "using Nop.Services.Plugins;" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
echo "using Nop.Core.Domain.Tasks;" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
echo "using Nop.Services.Messages;" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
echo "using System.Threading.Tasks;" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
echo "using Nop.Services.Localization;" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
echo "using Nop.Services.Configuration;" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
echo "using System.Collections.Generic;" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
echo "using Nop.Web.Framework.Infrastructure;" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
echo "" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
if [ $version = "4.10" -o $version = "4.20" -o $version = "4.30" ]; then
    echo "namespace Nop.Plugin.${group}.${name}" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "{" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "    /// <summary>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "    /// ${group}${name}" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "    /// </summary>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "    public class ${group}${name}Plugin : BasePlugin" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "    {" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        #region Fields" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        //" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        #endregion" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        #region Ctor" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        public ${group}${name}Plugin() { }" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        #endregion" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        #region Methods" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        /// <summary>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        /// Install plugin " >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        /// </summary>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        public override void Install()" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        {" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "            base.Install();" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        }" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        /// <summary>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        /// Uninstall plugin " >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        /// </summary>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        public override void Uninstall()" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        {" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "            base.Uninstall();" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        }" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        #endregion" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "    }" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "}" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
else
    echo "namespace Nop.Plugin.${group}.${name}" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "{" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "    /// <summary>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "    /// ${group}${name}" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "    /// </summary>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "    public class ${group}${name}Plugin : BasePlugin" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "    {" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        #region Fields" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        //" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        #endregion" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        #region Ctor" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        public ${group}${name}Plugin() { }" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        #endregion" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        #region Methods" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        /// <summary>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        /// Install plugin " >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        /// </summary>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        public override async Task InstallAsync()" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        {" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "            await base.InstallAsync();" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        }" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        /// <summary>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        /// Uninstall plugin " >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        /// </summary>" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        public override async Task UninstallAsync()" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        {" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "            await base.UninstallAsync();" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        }" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "        #endregion" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "    }" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
    echo "}" >>${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs
fi
if [ -f ${path}/Plugins/Nop.Plugin.${group}.${name}/${group}${name}Plugin.cs ]; then
    Success "The ${group}${name}Plugin class was created success!"
else
    ErrorTag "There was and error creating the file ${group}${name}Plugin.cs."
    exit 0
fi

# validate if the reference exists
cd ${path}
if [ ! $(dotnet sln list | grep Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj) ]; then
    dotnet sln add ./Plugins/Nop.Plugin.${group}.${name}/Nop.Plugin.${group}.${name}.csproj
fi
cd ..

tput setaf 7
echo "The Plugin: Nop.Plugin.${group}.${name} was successfull created!"
echo "Thanks for using this plugin script, enjoy your plugin!"
exit 0