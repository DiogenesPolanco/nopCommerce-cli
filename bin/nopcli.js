#! /usr/bin/env node

var shell = require("shelljs");
var yargs = require("yargs");
var fs = require('fs');

var argv = yargs.usage("$0 command")
    .command("new", "create plugin --[group] --[plugin] --[version]\nNote: Only support plugins of Group=Widgets\n", function (yargs) {

        let slPath = fs.existsSync(`./Plugins`) ? `.` : `src`;
        let srcPluginName = `Nop.Plugin.${yargs.argv.group}.NopcliGeneric`;
        let pluginName = `Nop.Plugin.${yargs.argv.group}.${yargs.argv.plugin}`;
        let pluginsPath = `${slPath}/Plugins/${pluginName}`;
        let version = yargs.argv.version !== undefined ? yargs.argv.version : `430`;
        
        //TODO: Pending use var env.
        if (fs.existsSync(`${slPath}/Libraries/Nop.Core/NopVersion.cs`) && yargs.argv.version === undefined) {
            let nopVersionFile = shell.grep(`l`, `${slPath}/Libraries/Nop.Core/NopVersion.cs`);
            if(nopVersionFile.includes(`"4.10"`)){
                version = "410"
            }else if(nopVersionFile.includes(`"4.20"`) || nopVersionFile.includes(`"4.30"`)){
                version = "430"
            }else if(nopVersionFile.includes(`"4.40"`)){
                version = "440"
            }else{
                version = "430"
            }
        }
        
        if (fs.existsSync(`${pluginsPath}`) && fs.existsSync(`${pluginsPath}/${pluginName}.csproj`)) {
            shell.echo(`this plugin ${pluginName} exists!`);
        } else {
            shell.mkdir('-p', `${pluginsPath}`);
            //shell.cp('-R', `${process.mainModule.paths[2]}/nopcli/src/nopCommerce-${version}/${srcPluginName}/`, pluginsPath); //Public
            shell.cp('-R', `${slPath}/nopCommerce-${version}/${srcPluginName}/`, pluginsPath); //Local
            shell.mv(`${pluginsPath}/${srcPluginName}.csproj`, `${pluginsPath}/${pluginName}.csproj`);

            shell.find(`${pluginsPath}`)
                .forEach(function (fileOrFolder) {
                    fs.lstat(fileOrFolder, (err, stats) => {
                        if (stats.isFile()) {
                            let fileName = fileOrFolder.replace("NopcliGeneric", yargs.argv.plugin);
                            shell.sed('-i', /NopcliGeneric/g, yargs.argv.plugin, fileOrFolder);
                            if(fileName !== fileOrFolder) {
                                shell.mv(`${fileOrFolder}`, `${fileName}`);
                            }
                        }
                    });
                });

            setTimeout(function () {
                if (fs.existsSync(`${slPath}/NopCommerce.sln`)) {
                    shell.cd(slPath);
                    shell.exec(`dotnet sln add ./Plugins/${pluginName}/${pluginName}.csproj`);
                }
            }, 2000);
        }
    })
    .command("build", "build plugin --[group] --[plugin]\nNote: Only support plugins of Group=Widgets\n", function (yargs) {
        let slPath = fs.existsSync(`./Plugins`) ? `.` : `src`;
        let pluginName = `Nop.Plugin.${yargs.argv.group}.${yargs.argv.plugin}`;
        let pluginsPath = `${slPath}/Plugins/${pluginName}`;
        shell.cd(pluginsPath);
        shell.exec( `dotnet build ${pluginName}.csproj`);
    })
    .demand(1, "must provide a valid command")
    // .showHelpOnFail(true)
    .help("h")
    .alias("h", "help")
    .argv
