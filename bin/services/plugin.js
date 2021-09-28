import shell from 'shelljs'
import fs from 'fs'
import {messages} from '../helper/messages.js'

export class PluginService {
    create(yargs, root_path) {
        return new Promise((resolve, reject) => {
            let slPath = fs.existsSync(`./Plugins`) ? `.` : `src`;
            let srcPluginName = `Nop.Plugin.${yargs.argv.g}.NopCliGeneric`;
            let pluginName = `Nop.Plugin.${yargs.argv.g}.${yargs.argv.p}`;
            let pluginsPath = `${slPath}/Plugins/${pluginName}`;
            let version = yargs.argv.v !== undefined && yargs.argv.v !== '420' ? yargs.argv.v : `430`;

            if (yargs.argv.v === undefined) {
                if (fs.existsSync(`${slPath}/Libraries/Nop.Services/Plugins/Samples/uploadedItems.json`)) {
                    fs.readFile(`${slPath}/Libraries/Nop.Services/Plugins/Samples/uploadedItems.json`, 'utf8', (err, data) => {
                        if (err) throw err;
                        let nopVersionFile = JSON.parse(data.replace(new RegExp("\/\/(.*)", "g"), ''));
                        version = nopVersionFile[3].SupportedVersion;
                    });
                }
            }

            if (fs.existsSync(`${pluginsPath}`) && fs.existsSync(`${pluginsPath}/${pluginName}.csproj`)) {
                reject(messages['001']);
            } else {
                shell.mkdir('-p', `${pluginsPath}`);
                shell.cp('-R', `${root_path}/src/nopCommerce-${version}/${srcPluginName}/`, pluginsPath);
                shell.mv(`${pluginsPath}/${srcPluginName}.csproj`, `${pluginsPath}/${pluginName}.csproj`);

                fs.readFile(`${root_path}/src/assets/images/logos/nopcli.png`, function (err, data) {
                    if (err) throw err;
                    fs.writeFile(`${pluginsPath}/logos.png`, data, 'base64', function (err) {
                        if (err) throw err;
                    });
                });
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
                resolve(messages['001']);
            }
        })
    }

    build(yargs) {
        return new Promise((resolve, reject) => {
            let slPath = fs.existsSync(`./Plugins`) ? `.` : `src`;
            let pluginName = `Nop.Plugin.${yargs.argv.g}.${yargs.argv.p}`;
            let pluginsPath = `${slPath}/Plugins/${pluginName}`;
            if (fs.existsSync(`${pluginName}.csproj`)) {
                shell.cd(pluginsPath);
                shell.exec(`dotnet build ${pluginName}.csproj`);
                resolve(messages['003']);
            }else{
                reject(messages['004']);
            }
        });
    }
}