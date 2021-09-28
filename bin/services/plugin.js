import shell from 'shelljs'
import fs from 'fs'
import {messages} from '../helper/messages.js'

export class PluginService {
    getSrcPluginName(args) {
        return `Nop.Plugin.${args.g}.NopCliGeneric`;
    }

    getOutPluginName(args) {
        return `Nop.Plugin.${args.g}.${args.p}`;
    }

    getFullSrcPlugin(args) {
        let self = this;
        return `${self.getSrcSolutionPath(args)}/Plugins/${self.getOutPluginName(args)}`;
    }

    getSrcSolutionPath() {
        return fs.existsSync(`./Plugins`) ? `.` : `src`;
    }

    validateVersion(version) {
        let self = this;
        let slPath = self.getSrcSolutionPath();
        if (fs.existsSync(`${slPath}/Libraries/Nop.Services/Plugins/Samples/uploadedItems.json`)) {
            fs.readFile(`${slPath}/Libraries/Nop.Services/Plugins/Samples/uploadedItems.json`, 'utf8', (err, data) => {
                if (err) throw err;
                let nopVersionFile = JSON.parse(data.replace(new RegExp("\/\/(.*)", "g"), ''));
                version = nopVersionFile[3].SupportedVersion;
            });
        }
        return version;
    }

    getSrcVersion(args) {
        let self = this;
        let result = args.v !== undefined && args.v !== '420' ? args.v : `430`;
        return self.validateVersion(result);
    }

    getOutProjectPathPluginName(args) {
        let self = this;
        return `${self.getFullSrcPlugin(args)}/${self.getSrcPluginName(args)}.csproj`;
    }

    async existOutProjectAsync(args) {
        let self = this;
        return await new Promise((resolve, reject) => {
            let path = self.getOutProjectPathPluginName(args);
            if (path === undefined) {
                reject(messages['005']);
            }
            let result = fs.existsSync(path);
            resolve(result);
        });
    }

    async copyFiles(root_path, args) {
        let self = this;
        let pluginsPath = self.getFullSrcPlugin(args);
        return await new Promise(async (resolve, reject) => {
            let srcPluginName = self.getSrcPluginName(args);
            let pluginName = self.getOutPluginName(args);

            shell.mkdir('-p', `${pluginsPath}`);
            shell.cp('-R', `${root_path}/src/nopCommerce-${self.getSrcVersion(args)}/${srcPluginName}/`, pluginsPath);
            shell.mv(`${pluginsPath}/${srcPluginName}.csproj`, `${pluginsPath}/${pluginName}.csproj`);

            await fs.readFileSync(`${root_path}/src/assets/images/logos/nopcli.png`, function (err, data) {
                if (err) {
                    reject(err);
                } else {
                    fs.writeFile(`${pluginsPath}/logos.png`, data, 'base64', function (err) {
                        if (err) reject(err);
                    });
                }
            });
            resolve(true);
        });
    }

    async replaceContentFiles(args) {
        let self = this;
        let pluginsPath = self.getFullSrcPlugin(args);
        return await new Promise(async (resolve, reject) => {
            let files = shell.find(`${pluginsPath}`);
            if (files.length > 0) {
                for (const fileOrFolder of files) {
                    await fs.lstatSync(fileOrFolder, (err, stats) => {
                        if (stats.isFile()) {
                            let fileName = fileOrFolder.replace("NopCliGeneric", args.p);
                            shell.sed('-i', /NopCliGeneric/g, args.p, fileOrFolder);
                            if (fileName !== fileOrFolder) {
                                shell.mv(`${fileOrFolder}`, `${fileName}`);
                            }
                        }
                    });
                }
                resolve(true);
            } else {
                reject(false);
            }
        });
    }

    async addSolution(args) {
        let self = this;
        return await new Promise(async (resolve, reject) => {
            if (fs.existsSync(`${self.getSrcSolutionPath()}/NopCommerce.sln`)) {
                shell.cd(self.getSrcSolutionPath());
                shell.exec(`dotnet sln add ./Plugins/${self.getOutPluginName(args)}`);
                resolve(messages["002"]);
            } else {
                reject(false);
            }
        });
    }

    async createProjectAsync(args, root_path, existProject) {
        let self = this;
        return await new Promise(async (resolve, reject) => {
            if (existProject) {
                reject(messages["001"]);
            } else {
                await self.copyFiles(root_path, args).then(async (copied) => {
                    if (copied) {
                        await self.replaceContentFiles(args).then(async (success) => {
                            if (success) {
                                return self.addSolution(args);
                            } else {
                                reject(messages["001"]);
                            }
                        });
                    }
                })
            }
        });
    }

    async CreateAsync(yargs, root_path) {
        let self = this;
        return self.existOutProjectAsync(yargs.argv).then((existProject) => {
            return self.createProjectAsync(yargs.argv, root_path, existProject);
        });
    }

    async Build(yargs) {
        let self = this;
        return new Promise(async (resolve, reject) => {
            if (await self.existOutProjectAsync(yargs.argv)) {
                shell.cd(self.getSrcPluginName(yargs.argv));
                shell.exec(`dotnet build ${self.getOutPluginName(yargs.argv)}.csproj`);
                resolve(messages['003']);
            } else {
                reject(messages['004']);
            }
        });
    }
}


