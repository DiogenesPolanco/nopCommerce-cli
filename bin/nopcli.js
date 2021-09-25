#! /usr/bin/env node

var shell = require("shelljs");
var yargs = require("yargs");
var fs = require('fs');

var argv = yargs.usage("$0 command")
    .command("new", "create plugin --[group] --[plugin] --[version]", function (yargs) {

        let slPath = `src`;
        let srcPluginName = `Nop.Plugin.${yargs.argv.group}.Generic`;
        let pluginName = `Nop.Plugin.${yargs.argv.group}.${yargs.argv.plugin}`;
        let pluginsPath = `${slPath}/Plugins/${pluginName}`;

        if (fs.existsSync(`${pluginsPath}`)) {
            shell.echo(`this plugin ${pluginName} exists!`);
        } else {
            shell.mkdir('-p', `${pluginsPath}`);
            shell.cp('-R', `${process.mainModule.paths[2]}/nopcli/src/nopCommerce-${yargs.argv.version}/${srcPluginName}/`, pluginsPath);
            shell.mv(`${pluginsPath}/${srcPluginName}.csproj`, `${pluginsPath}/${pluginName}.csproj`);

            shell.find(`${pluginsPath}`)
                .forEach(function (fileOrFolder) {
                    fs.lstat(fileOrFolder, (err, stats) => {
                        if (stats.isFile()) {
                            let fileName = fileOrFolder.replace("Generic", yargs.argv.plugin);
                            shell.sed('-i', /Generic/g, yargs.argv.plugin, fileOrFolder);
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
    .command("build", "build plugin", function (yargs) {
        shell.echo('this build command is not available');
    })
    .demand(1, "must provide a valid command")
    // .showHelpOnFail(true)
    .help("h")
    .alias("h", "help")
    .argv
