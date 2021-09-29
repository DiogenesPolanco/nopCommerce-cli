import shell from 'shelljs'
import fs from 'fs'
import {messages} from '../helper/messages.js'
import {Helper} from '../helper/index.js'

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

    async validateVersion(version) {
        let self = this;
        let slPath = self.getSrcSolutionPath();
        if (await fs.existsSync(`${slPath}/Libraries/Nop.Services/Plugins/Samples/uploadedItems.json`)) {
            fs.readFile(`${slPath}/Libraries/Nop.Services/Plugins/Samples/uploadedItems.json`, 'utf8', (err, data) => {
                if (err) throw err;
                let nopVersionFile = JSON.parse(data.replace(new RegExp("\/\/(.*)", "g"), ''));
                version = nopVersionFile[3].SupportedVersion;
            });
        }
        return version;
    }

    async getSrcVersion(args) {
        let self = this;
        let result = args.v !== undefined && args.v !== 420 ? args.v === 450 ? 440 : args.v : 430;
        return await self.validateVersion(result);
    }

    getOutProjectPathPluginName(args) {
        let self = this;
        return `${self.getFullSrcPlugin(args)}/${self.getOutPluginName(args)}.csproj`;
    }

    async existOutProjectAsync(args) {
        let self = this;
        return new Promise(async (resolve, reject) => {
            let path = self.getOutProjectPathPluginName(args);
            if (path === undefined) {
                reject(messages['005']);
            } else {
                resolve(await fs.existsSync(path));
            }
        });
    }

    async copyFiles(root_path, args) {
        let self = this;
        let pluginsPath = self.getFullSrcPlugin(args);
        return new Promise(async (resolve, reject) => {
            let srcPluginName = self.getSrcPluginName(args);
            let pluginName = self.getOutPluginName(args);

            shell.mkdir('-p', `${pluginsPath}`);
            shell.cp('-R', `${root_path}/src/nopCommerce-${await self.getSrcVersion(args)}/${srcPluginName}/`, pluginsPath);
            shell.mv(`${pluginsPath}/${srcPluginName}.csproj`, `${pluginsPath}/${pluginName}.csproj`);

            await fs.readFileSync(`${root_path}/src/assets/images/logos/logo.png`, function (err, data) {
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
        return new Promise(async (resolve, reject) => {
            let pluginsPath = self.getFullSrcPlugin(args);
            let files = shell.find(`${pluginsPath}`);
            if (files.length > 0) {
                for (const fileOrFolder of files) {
                    fs.lstat(fileOrFolder, (err, stats) => {
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

    wait(ms = 100) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    async addSolution(args) {
        let self = this;
        return new Promise(async (resolve, reject) => {
            if (await fs.existsSync(`${self.getSrcSolutionPath()}/NopCommerce.sln`)) {
                Helper.printHandler(null, messages["006"])
                shell.config.silent = true;
                self.wait().then(() => {
                    shell.cd(self.getSrcSolutionPath());
                    shell.exec(`dotnet sln add ./Plugins/${self.getOutPluginName(args)}`);
                    resolve(messages["002"]);
                })
            } else {
                reject(messages["001"]);
            }
        });
    }

    async createProjectAsync(args, root_path) {
        let self = this;

        return new Promise(async (resolve, reject) => {
            await self.copyFiles(root_path, args).then(async (copied) => {
                if (copied) {
                    await self.replaceContentFiles(args).then(async (success) => {
                        if (success) {
                            self.addSolution(args).then(async (message) => {
                                resolve(message);
                            }).catch((error) => {
                                reject(error);
                            });
                        } else {
                            reject(messages["001"]);
                        }
                    }).catch((error) => {
                        reject(error);
                    });
                } else {
                    reject(messages["001"]);
                }
            }).catch((error) => {
                reject(error);
            });
        });
    }

    async CreateAsync(yargs, root_path) {
        let self = this;
        return new Promise(async (resolve, reject) => {
            self.existOutProjectAsync(yargs.argv).then(async (existProject) => {
                if (existProject == false) {
                    self.createProjectAsync(yargs.argv, root_path, existProject).then((messages) => {
                        resolve(messages);
                    }).catch((error) => {
                        reject(error);
                    });
                } else {
                    reject(messages["001"]);
                }
            }).catch((error) => {
                reject(error);
            });
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


