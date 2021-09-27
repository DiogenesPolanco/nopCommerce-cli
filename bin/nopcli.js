#! /usr/bin/env node

let shell = require("shelljs");
let yargs = require("yargs");
let fs = require('fs');
let root_path = process.env.NODE_ENV === 'Development' ? "./" : `${process.mainModule.paths[2]}/nopcli`;

var argv = yargs.usage("$0 command")
    .option('g', {
        alias: 'group',
        type: 'string',
        default: 'Widgets',
        describe: 'Only support Widgets, Payments, DiscountRules and Misc'
    })
    .option('p', {
        alias: 'plugin',
        type: 'string'
    })
    .option('v', {
        alias: 'version',
        type: 'string',
        default: '430',
        describe: 'Only support ["4.30", "4.40"]'
    })
    .command("new", "create plugin -[g] -[p] -[v]", function (yargs) {
        let slPath = fs.existsSync(`./Plugins`) ? `.` : `src`;
        let srcPluginName = `Nop.Plugin.${yargs.argv.g}.NopCliGeneric`;
        let pluginName = `Nop.Plugin.${yargs.argv.g}.${yargs.argv.p}`;
        let pluginsPath = `${slPath}/Plugins/${pluginName}`;
        let version = yargs.argv.v !== undefined ? yargs.argv.v : `430`;

        if(yargs.argv.v === undefined){
            if (fs.existsSync(`${slPath}/Libraries/Nop.Services/Plugins/Samples/uploadedItems.json`)   ) {
                fs.readFile(`${slPath}/Libraries/Nop.Services/Plugins/Samples/uploadedItems.json`, 'utf8', (err, data) => {
                    if (err) throw err;
                     let nopVersionFile = JSON.parse(data.replace(new RegExp("\/\/(.*)","g"), ''));
                    version = nopVersionFile[3].SupportedVersion;
                });
            }
        }


        if (fs.existsSync(`${pluginsPath}`) && fs.existsSync(`${pluginsPath}/${pluginName}.csproj`)) {
            shell.echo(`this plugin ${pluginName} exists!`);
        } else {
            shell.mkdir('-p', `${pluginsPath}`);
            shell.cp('-R', `${root_path}/src/nopCommerce-${version}/${srcPluginName}/`, pluginsPath); //Local
            shell.mv(`${pluginsPath}/${srcPluginName}.csproj`, `${pluginsPath}/${pluginName}.csproj`);

            shell.find(`${pluginsPath}`)
                .forEach(function (fileOrFolder) {
                    fs.lstat(fileOrFolder, (err, stats) => {
                        if (stats.isFile()) {
                            let fileName = fileOrFolder.replace("NopCliGeneric", yargs.argv.p);
                            shell.sed('-i', /NopCliGeneric/g, yargs.argv.p, fileOrFolder);
                            if (fileName !== fileOrFolder) {
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
    .command("build", "build plugin -[g] -[p]", function (yargs) {
        let slPath = fs.existsSync(`./Plugins`) ? `.` : `src`;
        let pluginName = `Nop.Plugin.${yargs.argv.group}.${yargs.argv.plugin}`;
        let pluginsPath = `${slPath}/Plugins/${pluginName}`;
        shell.cd(pluginsPath);
        shell.exec(`dotnet build ${pluginName}.csproj`);
    })

    .demand(1, "must provide a valid command")
    .showHelpOnFail(true)
    .help("h")
    .alias("h", "help")
    .argv
