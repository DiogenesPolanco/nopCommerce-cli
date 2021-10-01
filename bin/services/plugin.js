import shell from 'shelljs'
import fs from 'fs'
import {messages} from '../helper/messages.js'
import Helper from '../helper/index.js'
import ProgressService from "./progress.js";
import Config from '../config/index.js'


class PluginService {

    getSrcPluginName(args) {
        return `Nop.Plugin.${args.g}.NopCliGeneric`;
    }

    getOutPluginName(args) {
        return `Nop.Plugin.${args.g}.${args.p}`;
    }

    getFullSrcPlugin(args) {
        let self = this;
        return `${self.getSrcSolutionPath()}/Plugins/${self.getOutPluginName(args)}`;
    }

    getSrcSolutionPath() {
        return fs.existsSync(`Plugins`) ? `.` : `src`;
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
        return new Promise(async (resolve) => {
            let pluginsPath = self.getFullSrcPlugin(args);
            let srcPluginName = self.getSrcPluginName(args);
            let pluginName = self.getOutPluginName(args);

            shell.mkdir('-p', `${pluginsPath}`);
            shell.cp('-R', `${root_path}/src/nopCommerce-${await self.getSrcVersion(args)}/${srcPluginName}/`, pluginsPath);
            shell.mv(`${pluginsPath}/${srcPluginName}.csproj`, `${pluginsPath}/${pluginName}.csproj`);

            fs.readFile(`${root_path}/src/assets/images/logos/logo.png`, function (err, data) {
                if (err) {
                    resolve(false);
                } else {
                    fs.writeFile(`${pluginsPath}/logo.png`, data, 'base64', function (err) {
                        if (err) resolve(false);
                    });
                }
            });
            resolve(true);
        });
    }

    async replaceContentFiles(args) {
        let self = this;
        return new Promise(async (resolve) => {
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
                resolve(false);
            }
        });
    }

    async addSolution(args) {
        let self = this;
        return new Promise(async (resolve, reject) => {
            if (await fs.existsSync(`${self.getSrcSolutionPath()}/NopCommerce.sln`)) {
                Helper.printHandler(null, messages["006"].message.replace('{{nopCli}}', self.getOutPluginName(args)))
                shell.config.silent = true;
                ProgressService.waitProgress().then(() => {
                    shell.cd(self.getSrcSolutionPath());
                    shell.exec(`dotnet sln add ./Plugins/${self.getOutPluginName(args)}`);
                    resolve(messages["002"].message.replace('{{nopCli}}', self.getOutPluginName(args)));
                });
            } else {
                reject(messages["001"].message.replace('{{nopCli}}', self.getOutPluginName(args)));
            }
        });
    }

    async clearPlugin(args) {
        let self = this;
        return new Promise(async (resolve, reject) => {
            self.existOutProjectAsync(args).then((result) => {
                if (result) {
                    shell.rm("-r", self.getFullSrcPlugin(args));
                }
                resolve(result);
            }).catch((error) => {
                reject(error);
            });
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
                            reject(messages["001"].message.replace('{{nopCli}}', self.getOutPluginName(args)));
                        }
                    }).catch((error) => {
                        reject(error);
                    });
                } else {
                    reject(messages["001"].message.replace('{{nopCli}}', self.getOutPluginName(args)));
                }
            }).catch((error) => {
                reject(error);
            });
        });
    }

    async TryToCreate(args, root_path) {
        let self = this;
        return new Promise(async (resolve, reject) => {
            self.createProjectAsync(args, root_path).then((messages) => {
                resolve(messages);
            }).catch(() => {
                self.clearPlugin(args).then((result) => {
                    resolve(messages[result ? "002" : "001"].message.replace('{{nopCli}}', self.getOutPluginName(args)));
                }).catch((error) => {
                    reject(error);
                });
            });
        });
    }

    async CreateAsync(yargs, root_path) {
        let self = this;
        return new Promise(async (resolve, reject) => {
            self.existOutProjectAsync(yargs.argv).then(async (existProject) => {
                if (existProject === false) {
                    self.TryToCreate(yargs.argv, root_path).then((messages) => {
                        resolve(messages)
                    }).catch((error) => {
                        reject(error);
                    });
                } else {
                    if (yargs.argv.c) {
                        self.clearPlugin(yargs.argv).then((result) => {
                            self.TryToCreate(yargs.argv, root_path, result)
                                .then((messages) => {
                                    resolve(messages)
                                }).catch((error) => {
                                reject(error);
                            });
                        })
                    } else {
                        reject(messages["001"].message.replace('{{nopCli}}', self.getOutPluginName(yargs.argv)));
                    }
                }
            }).catch((error) => {
                reject(error);
            });
        });
    }

    async cloneAsync() {
        return new Promise(async (resolve, reject) => {
            if (!shell.which('git')) {
                resolve('Sorry, this script requires git');
                shell.exit(1);
            } else {
                ProgressService.waitInfinityProgress((progress) => {
                    shell.exec(Config.getCloneNopDefaultCommand(), function (code, stdout, stderr) {
                        progress.setTotal(100);
                        progress.update(100);
                        progress.stop();
                        if (stderr) {
                            reject(stderr)
                        } else {
                            shell.rm("-r", Config.getGitNopCommercePath());
                            shell.exec("git init && git add *.*", function (codeGit, stdoutGit, stderrGit) {
                                if (stderrGit) {
                                    reject(stderrGit)
                                } else {
                                    resolve(stdoutGit);
                                }
                            });
                        }
                    });
                });
            }
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

    async Init(yargs, root_path) {
        let self = this;
        return new Promise(async (resolve, reject) => {
            await self.cloneAsync().then((msg) => {
                if (yargs.argv.p) {
                    self.createProjectAsync(yargs.argv, root_path).then((msgCreated) => {
                        if (yargs.argv.b) {
                            self.Build(yargs).then((msgBuild) => {
                                resolve(msgBuild);
                            });
                        } else {
                            resolve(msgCreated);
                        }
                    })
                } else {
                    resolve(msg);
                }
            }).catch((error) => {
                reject(error);
            });
        });
    }
}

const pluginService = new PluginService();
export default pluginService;